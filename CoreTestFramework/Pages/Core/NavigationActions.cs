using OpenQA.Selenium;
using CoreTestFramework.Support;
using TechTalk.SpecFlow;

namespace CoreTestFramework.Pages.Core
{
    /// <summary>
    /// Provides core navigation actions for web pages.
    /// Handles browser navigation tasks with proper error handling and logging.
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
                _logger.LogError(
                    $"Failed to navigate to {url}: {ex.Message}",
                    _featureContext.FeatureInfo.Title,
                    _scenarioContext.ScenarioInfo.Title);
                return false;
            }
        }
    }
}
