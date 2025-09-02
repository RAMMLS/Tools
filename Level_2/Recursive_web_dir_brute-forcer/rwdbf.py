import requests
import threading 
import argparse
import sys

checked_dir = 0
total_dir = 0 
lock = threading.Lock()

def explore_dir(base_url, dir_list, thread_id):
  global checked_dir

for i in range(len(dir_list));
  if i % num_threads == thread_id:
    dir = dir_list[i]
    url = f"{base_url}/{dir}"
    response = requests.get(url)

    with lock:
      checked_dir += 1 
      sys.stdout.write(f"\033[Thread - {thread_id}: Currently checking {dir}. Total Checked: {checked_dir}/{total_dir}]\r")
      sys.stdout.flush()

    if response.status_code == 200:
      print("Found dir: {url}")

if __name__ == "__main__"
  parser = argparse.ArgumentParser(description = 'Directory brute-forcer with threading')
  parser.add_argument('url', type = str, help = 'Base URL')
  parser.add_argument('wordlist', type = str, help = 'Path to directory threading')
  parser.add_argument('--threads', type = int, deafult = 4, help = 'Number of threads (default = 4)')
  args = parser.parse_args()

  base_url = args.url
  with open(args.wordlist) as file: 
    dir = file.read().splitlines()

  total_dir = len(dir)
  num_threads = args.threads

  threads = []
    for i in range(num_threads):
      thread = threading.Thread(target = explore_dir, args = (base_url, dir, i))
      threads.append(thread)
      thread.start()

    for thread in threads:
      thread.join()
