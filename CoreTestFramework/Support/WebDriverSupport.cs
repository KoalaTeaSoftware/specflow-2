using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using BoDi;

namespace CoreTestFramework.Support
{
    /// <summary>
    /// Manages WebDriver lifecycle and registration in the test container.
    /// Provides access to the current WebDriver instance and basic browser information.
    /// </summary>
    public class WebDriverSupport : IDisposable
    {
        private IWebDriver? _driver;
        private readonly IObjectContainer _container;

        /// <summary>
        /// Initializes WebDriver support and registers the container for later driver initialization.
        /// </summary>
        /// <param name="container">SpecFlow's object container for dependency injection</param>
        public WebDriverSupport(IObjectContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Gets the current WebDriver instance. Throws if driver is not initialized.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when WebDriver is not initialized</exception>
        public IWebDriver Driver
        {
            get => _driver ?? throw new InvalidOperationException("WebDriver not initialized");
            set
            {
                _driver = value;
                _container.RegisterInstanceAs<IWebDriver>(_driver);
            }
        }

        /// <summary>
        /// Disposes the WebDriver instance, closing the browser and cleaning up resources.
        /// </summary>
        public void Dispose()
        {
            if (_driver != null)
            {
                _driver.Quit();
                _driver.Dispose();
                _driver = null;
            }
        }
    }
}
