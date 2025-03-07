using OpenQA.Selenium;
using System;
using System.IO;

namespace CoreTestFramework.Support
{
    /// <summary>
    /// Manages screenshot capture and storage for test failures.
    /// Ensures screenshots are properly named and stored in the test report directory.
    /// </summary>
    public class ScreenshotManager
    {
        private readonly string _screenshotsDirectory;

        public ScreenshotManager(string screenshotsDirectory)
        {
            _screenshotsDirectory = screenshotsDirectory ?? throw new ArgumentNullException(nameof(screenshotsDirectory));
        }

        /// <summary>
        /// Captures a screenshot of the current browser state.
        /// </summary>
        /// <param name="driver">The WebDriver instance to capture from</param>
        /// <param name="screenshotName">Base name for the screenshot file</param>
        /// <returns>Path to the saved screenshot, or null if capture failed</returns>
        public string? CaptureScreenshot(IWebDriver driver, string screenshotName)
        {
            try
            {
                var timestamp = DateTime.Now.ToString("HHmmss");
                var fileName = $"{screenshotName}_{timestamp}.png";
                var filePath = Path.Combine(_screenshotsDirectory, fileName);

                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                screenshot.SaveAsFile(filePath);

                return filePath;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
