using TechTalk.SpecFlow;
using CoreTestFramework.Support;
using CoreTestFramework.Pages.Core;
using NUnit.Framework;
using CoreTestFramework.Fixtures;

namespace CoreTestFramework.StepDefinitions
{
    [Binding]
    public class WebsiteNavigationSteps
    {
        private readonly WebDriverSupport _webDriverSupport;
        private readonly NavigationActions _navigationActions;
        private readonly TestConfiguration _config;

        public WebsiteNavigationSteps(WebDriverSupport webDriverSupport, NavigationActions navigationActions, TestConfiguration config)
        {
            _webDriverSupport = webDriverSupport;
            _navigationActions = navigationActions;
            _config = config;
        }

        [Given(@"the browser shows the home page")]
        public void GivenTheBrowserShowsTheHomePage()
        {
            Assert.That(_navigationActions.NavigateToUrl(_config.BaseUrl), 
                Is.True, 
                $"Failed to navigate to home page at {_config.BaseUrl}");
        }

        [Then(@"the page title is ""(.*)""")]
        public void ThenThePageTitleIs(string expectedTitle)
        {
            var actualTitle = _webDriverSupport.GetPageTitle();
            StringAssert.IsMatch(expectedTitle, actualTitle, 
                $"Expected page title to match pattern '{expectedTitle}' but got '{actualTitle}'");
        }
    }
}
