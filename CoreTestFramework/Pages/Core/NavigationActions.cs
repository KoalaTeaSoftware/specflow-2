using OpenQA.Selenium;
using CoreTestFramework.Support;
using System;

namespace CoreTestFramework.Pages.Core
{
    public class NavigationActions
    {
        private readonly IWebDriver _driver;
        private readonly ScreenshotManager _screenshotManager;

        public NavigationActions(IWebDriver driver, ScreenshotManager screenshotManager)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
            _screenshotManager = screenshotManager ?? throw new ArgumentNullException(nameof(screenshotManager));
        }

        public bool NavigateToUrl(string url)
        {
            try
            {
                _driver.Navigate().GoToUrl(url);
                return true;
            }
            catch (WebDriverException ex)
            {
                var errorMessage = $"Failed to navigate to URL: {url}. Error: {ex.Message}";
                Console.WriteLine(errorMessage);
                _screenshotManager.CaptureScreenshot(_driver, "ImmediateError_Navigation");
                return false;
            }
            catch (Exception ex)
            {
                var errorMessage = $"Unexpected error during navigation to URL: {url}. Error: {ex.Message}";
                Console.WriteLine(errorMessage);
                _screenshotManager.CaptureScreenshot(_driver, "ImmediateError_Navigation");
                return false;
            }
        }
    }
}
