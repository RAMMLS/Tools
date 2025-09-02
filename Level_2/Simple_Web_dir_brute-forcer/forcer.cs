using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebDirectoryBruteForcer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: WebDirectoryBruteForcer <url> <wordlist_path>");
                Console.WriteLine("Example: WebDirectoryBruteForcer http://example.com/ wordlist.txt");
                return;
            }

            string url = args[0];
            string wordlistPath = args[1];

            if (!url.EndsWith("/"))
            {
                url += "/"; // Add trailing slash if missing for consistent path building.
            }

            if (!File.Exists(wordlistPath))
            {
                Console.WriteLine($"Error: Wordlist file not found at {wordlistPath}");
                return;
            }

            Console.WriteLine($"Starting directory brute-force attack against {url} using wordlist {wordlistPath}");

            await BruteForceDirectories(url, wordlistPath);

            Console.WriteLine("Brute-force attack completed.");
        }


        static async Task BruteForceDirectories(string baseUrl, string wordlistPath)
        {
            // Load wordlist.  Using a HashSet for faster lookups (Contains() is O(1)).
            HashSet<string> directories = new HashSet<string>(File.ReadLines(wordlistPath));

            // Use a ConcurrentBag for thread-safe collection of results.
            ConcurrentBag<string> foundDirectories = new ConcurrentBag<string>();

            // Configure HttpClient - create ONE instance and reuse it!
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(10); // Set a timeout to avoid hanging indefinitely

                // Limit concurrency.  Too much and you'll likely get blocked.  Adjust as needed.
                var semaphore = new SemaphoreSlim(50); //Allow a maximum of 50 concurrent tasks.

                // Prepare tasks - a list of directory checks that will be executed.
                List<Task> tasks = new List<Task>();

                foreach (string directory in directories)
                {
                    //Avoid empty directories
                    if(string.IsNullOrEmpty(directory)) continue;
                    await semaphore.WaitAsync(); //Wait if more than the allowed number of tasks is running.
                    tasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            string fullUrl = baseUrl + directory;
                            HttpResponseMessage response = await client.GetAsync(fullUrl); // Get request

                            if (response.IsSuccessStatusCode)
                            {
                                Console.WriteLine($"[+] Found: {fullUrl} - Status: {response.StatusCode}");
                                foundDirectories.Add(fullUrl);
                            }
                            else if(response.StatusCode == HttpStatusCode.Forbidden)
                            {
                                Console.WriteLine($"[!] Forbidden: {fullUrl} - Status: {response.StatusCode}");
                            }
                            else
                            {
                                //Optionally, log non-success and non-forbidden status codes.
                                //Console.WriteLine($"[-] Not Found: {fullUrl} - Status: {response.StatusCode}");
                            }
                        }
                        catch (HttpRequestException ex)
                        {
                            // Handle connection errors, timeouts, DNS resolution failures, etc.
                            Console.WriteLine($"[!] Error accessing {baseUrl + directory}: {ex.Message}");
                        }
                        catch (TaskCanceledException)
                        {
                            Console.WriteLine($"[!] Timeout accessing {baseUrl + directory}");
                        }
                        finally
                        {
                           semaphore.Release(); //Release the semaphore slot.
                        }
                    }));
                }

                // Wait for all tasks to complete.
                await Task.WhenAll(tasks);
            }


            Console.WriteLine("\nFound the following directories:");
            foreach (string dir in foundDirectories)
            {
                Console.WriteLine(dir);
            }
        }
    }
}
