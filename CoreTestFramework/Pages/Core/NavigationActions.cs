using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using CoreTestFramework.Support;
using TechTalk.SpecFlow;

namespace CoreTestFramework.Pages.Core
{
    /// <summary>
    /// Provides core navigation actions for web pages.
    /// Handles common browser navigation tasks with proper error handling and logging.
    /// </summary>
    public class NavigationActions
    {
        private readonly IWebDriver _driver;
        private readonly TestLogger _logger;
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;

        public NavigationActions(
            IWebDriver driver,
            TestLogger logger,
            ScenarioContext scenarioContext,
            FeatureContext featureContext)
        {
            _driver = driver;
            _logger = logger;
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
        }

        /// <summary>
        /// Navigates to the specified URL with error handling and logging.
        /// </summary>
        /// <param name="url">The URL to navigate to</param>
        /// <returns>True if navigation was successful, false otherwise</returns>
        public bool NavigateTo(string url)
        {
            try
            {
                _driver.Navigate().GoToUrl(url);
                _logger.LogDiagnostic($"Successfully navigated to {url}");
                return true;
            }
            catch (WebDriverException ex)
            {
                _logger.LogError($"Failed to navigate to {url}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Waits for an element to be present and visible on the page.
        /// </summary>
        /// <param name="by">The locator to find the element</param>
        /// <param name="timeoutSeconds">How long to wait for the element</param>
        /// <returns>True if element was found within timeout, false otherwise</returns>
        public bool WaitForElement(By by, int timeoutSeconds = 10)
        {
            try
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutSeconds));
                wait.Until(d => d.FindElement(by).Displayed);
                _logger.LogDiagnostic($"Element found: {by}");
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                _logger.LogError($"Element not found within {timeoutSeconds} seconds: {by}");
                return false;
            }
        }
    }
}
