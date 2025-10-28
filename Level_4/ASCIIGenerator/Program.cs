using System;
using System.Collections.Generic;
using System.Threading;
using AsciiNameGenerator.Fonts;
using AsciiNameGenerator.Models;
using AsciiNameGenerator.Styles;

namespace AsciiNameGenerator
{
    class Program
    {
        private static AsciiArtGenerator _generator;
        private static GenerationOptions _options;

        static void Main(string[] args)
        {
            Console.Title = "ASCII Name Generator v2.0";
            Console.WriteLine("🎨 ASCII Name Generator - Create Beautiful Text Art\n");

            InitializeGenerator();
            
            if (args.Length > 0)
            {
                // Режим командной строки
                ProcessCommandLineArgs(args);
            }
            else
            {
                // Интерактивный режим
                ShowInteractiveMenu();
            }
        }

        static void InitializeGenerator()
        {
            _generator = new AsciiArtGenerator();
            _options = new GenerationOptions();
        }

        static void ShowInteractiveMenu()
        {
            while (true)
            {
                Console.Clear();
                DisplayHeader();

                Console.WriteLine("📋 Main Menu:");
                Console.WriteLine("1. ✏️  Generate ASCII Art");
                Console.WriteLine("2. 🎨  Change Font");
                Console.WriteLine("3. 🌈  Change Colors");
                Console.WriteLine("4. ⚙️  Configuration");
                Console.WriteLine("5. 💾  Save to File");
                Console.WriteLine("6. 🎭  Show Examples");
                Console.WriteLine("7. 🚪  Exit");
                Console.Write("\nSelect option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        GenerateArtInteractive();
                        break;
                    case "2":
                        ChangeFont();
                        break;
                    case "3":
                        ChangeColors();
                        break;
                    case "4":
                        ConfigureOptions();
                        break;
                    case "5":
                        SaveToFile();
                        break;
                    case "6":
                        ShowExamples();
                        break;
                    case "7":
                        return;
                    default:
                        ShowError("Invalid option!");
                        break;
                }
            }
        }

        static void GenerateArtInteractive()
        {
            Console.Write("\nEnter text to convert: ");
            var text = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(text))
            {
                ShowError("Text cannot be empty!");
                return;
            }

            try
            {
                var asciiArt = _generator.Generate(text, _options);
                DisplayAsciiArt(asciiArt);
            }
            catch (Exception ex)
            {
                ShowError($"Generation failed: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        static void ChangeFont()
        {
            Console.WriteLine("\n📝 Available Fonts:");
            Console.WriteLine("1. Standard");
            Console.WriteLine("2. Bold");
            Console.WriteLine("3. Block");
            Console.WriteLine("4. Shadow");
            Console.WriteLine("5. Script");
            Console.Write("Select font: ");

            var fontChoice = Console.ReadLine();
            _options.FontName = fontChoice switch
            {
                "1" => "Standard",
                "2" => "Bold",
                "3" => "Block",
                "4" => "Shadow",
                "5" => "Script",
                _ => "Standard"
            };

            Console.WriteLine($"✅ Font changed to: {_options.FontName}");
            Thread.Sleep(1000);
        }

        static void ChangeColors()
        {
            Console.WriteLine("\n🌈 Color Options:");
            Console.WriteLine("1. Default (White)");
            Console.WriteLine("2. Red");
            Console.WriteLine("3. Green");
            Console.WriteLine("4. Blue");
            Console.WriteLine("5. Rainbow");
            Console.WriteLine("6. Gradient");
            Console.Write("Select color: ");

            var colorChoice = Console.ReadLine();
            _options.ColorStyle = colorChoice switch
            {
                "1" => ColorStyle.Default,
                "2" => ColorStyle.Red,
                "3" => ColorStyle.Green,
                "4" => ColorStyle.Blue,
                "5" => ColorStyle.Rainbow,
                "6" => ColorStyle.Gradient,
                _ => ColorStyle.Default
            };

            Console.WriteLine($"✅ Color style changed");
            Thread.Sleep(1000);
        }

        static void ConfigureOptions()
        {
            Console.WriteLine("\n⚙️  Configuration:");
            Console.Write($"Width (current: {_options.Width}): ");
            if (int.TryParse(Console.ReadLine(), out int width) && width > 0)
                _options.Width = width;

            Console.Write($"Height (current: {_options.Height}): ");
            if (int.TryParse(Console.ReadLine(), out int height) && height > 0)
                _options.Height = height;

            Console.Write($"Border (current: {_options.ShowBorder}): ");
            var borderInput = Console.ReadLine().ToLower();
            if (borderInput == "y" || borderInput == "yes" || borderInput == "true")
                _options.ShowBorder = true;
            else if (borderInput == "n" || borderInput == "no" || borderInput == "false")
                _options.ShowBorder = false;

            Console.WriteLine("✅ Configuration updated");
            Thread.Sleep(1000);
        }

        static void SaveToFile()
        {
            Console.Write("\nEnter text to save: ");
            var text = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(text))
            {
                ShowError("Text cannot be empty!");
                return;
            }

            Console.Write("Enter filename (without extension): ");
            var filename = Console.ReadLine();

            try
            {
                var asciiArt = _generator.Generate(text, _options);
                FileExport.SaveToText(asciiArt, filename + ".txt");
                Console.WriteLine($"✅ ASCII art saved to: {filename}.txt");
            }
            catch (Exception ex)
            {
                ShowError($"Save failed: {ex.Message}");
            }

            Thread.Sleep(1500);
        }

        static void ShowExamples()
        {
            var examples = new[] { "HELLO", "WORLD", "ASCII", "ART", "CODING" };

            Console.WriteLine("\n🎭 Example Gallery:\n");

            foreach (var example in examples)
            {
                Console.WriteLine($"=== {example} ===");
                var art = _generator.Generate(example, _options);
                DisplayAsciiArt(art);
                Console.WriteLine();
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void DisplayAsciiArt(string art)
        {
            if (_options.ColorStyle != ColorStyle.Default)
            {
                ApplyColorStyle(art, _options.ColorStyle);
            }
            else
            {
                Console.WriteLine(art);
            }
        }

        static void ApplyColorStyle(string art, ColorStyle style)
        {
            var lines = art.Split('\n');
            var colors = new ConsoleColor[] 
            { 
                ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Green, 
                ConsoleColor.Cyan, ConsoleColor.Blue, ConsoleColor.Magenta 
            };

            for (int i = 0; i < lines.Length; i++)
            {
                switch (style)
                {
                    case ColorStyle.Rainbow:
                        Console.ForegroundColor = colors[i % colors.Length];
                        break;
                    case ColorStyle.Red:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case ColorStyle.Green:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case ColorStyle.Blue:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                }

                Console.WriteLine(lines[i]);
                Console.ResetColor();
            }
        }

        static void DisplayHeader()
        {
            var header = @"
    ___    ____ ____ _  _ ___   __ _  _ ___  ___ 
   / _ \  / _  | ___| || |__ \ / /| || |__ \|__ \
  / /_\ \/ /_| |___ \ || | / // /_| || |_/ /  / /
 /  _  /  _  |  ___) || |/ /| ||__   _|  _ <  |_|
/__/ \_\_/ |_| |____/\__/____|_|  |_| |____| (_)
            ";
            Console.WriteLine(header);
            Console.WriteLine($"Current Font: {_options.FontName} | Color: {_options.ColorStyle}\n");
        }

        static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ {message}");
            Console.ResetColor();
            Thread.Sleep(2000);
        }

        static void ProcessCommandLineArgs(string[] args)
        {
            // Обработка аргументов командной строки
            // Например: AsciiNameGenerator.exe "Hello" --font Bold --color Red
        }
    }
}
