using AutoCommentBot.Config;
using AutoCommentBot.Models;
using OpenQA.Selenium;
using System;
using System.Threading.Tasks;

namespace AutoCommentBot.Services
{
    public class CommentingService
    {
        private readonly WebNavigationService _webNavigationService;
        private readonly CredentialsProvider _credentialsProvider;
        private readonly BotConfig _botConfig;
        private readonly IWebDriver _driver;

        public CommentingService(WebNavigationService webNavigationService, CredentialsProvider credentialsProvider, BotConfig botConfig, IWebDriver driver)
        {
            _webNavigationService = webNavigationService;
            _credentialsProvider = credentialsProvider;
            _botConfig = botConfig;
            _driver = driver;
        }

        public async Task PostComment(Comment comment)
        {
            try
            {
                _driver.Navigate().GoToUrl(_botConfig.WebsiteUrl); // Navigate to website

                // Example: Assuming there's a login process before commenting
                var userProfile = _credentialsProvider.GetNextUserProfile();
                //if (userProfile != null)
                //{
                //    await _webNavigationService.Login(userProfile); // Implement Login method in WebNavigationService
                //}

                // Locate and fill the comment field
                var commentField = _driver.FindElement(By.CssSelector(_botConfig.CommentSelector));
                commentField.SendKeys(comment.Text);

                // Locate and click the submit button
                var submitButton = _driver.FindElement(By.CssSelector(_botConfig.SubmitButtonSelector));
                submitButton.Click();

                Console.WriteLine($"Comment posted: {comment.Text}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error posting comment: {ex.Message}");
            }

            if (_botConfig.UseRandomDelays)
            {
                Random rnd = new Random();
                int delay = rnd.Next(1, _botConfig.MaxRandomDelaySeconds + 1);
                await Task.Delay(delay * 1000); // Convert to milliseconds
            }
            else
            {
                await Task.Delay(_botConfig.DelayBetweenCommentsSeconds * 1000);
            }
        }
    }
}

