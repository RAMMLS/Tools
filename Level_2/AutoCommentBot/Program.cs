using AutoCommentBot.Config;
using AutoCommentBot.Models;
using AutoCommentBot.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AutoCommentBot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Load configuration from JSON file
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                .Build();

            BotConfig botConfig = configuration.GetSection("BotConfig").Get<BotConfig>();


            // Configure Chrome options
            ChromeOptions options = new ChromeOptions();
            if (botConfig.HeadlessMode)
            {
                options.AddArgument("--headless"); // Run Chrome in headless mode
            }
            options.AddArgument("--disable-gpu"); // Recommended for headless mode

            // Initialize ChromeDriver
            IWebDriver driver = new ChromeDriver(options);

            WebNavigationService webNavigationService = new WebNavigationService(driver);
            CredentialsProvider credentialsProvider = new CredentialsProvider(botConfig);
            CommentingService commentingService = new CommentingService(webNavigationService, credentialsProvider, botConfig, driver);

            // Post comments
            foreach (var comment in botConfig.Comments)
            {
                await commentingService.PostComment(comment);
            }

            // Close the browser
            driver.Quit();
        }
    }
}

