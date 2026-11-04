using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Selenium
{
    public class UITests : IDisposable
    {
        private readonly IWebDriver _driver;

        public UITests()
        {
            _driver = new ChromeDriver();
            _driver.Navigate().GoToUrl("http://127.0.0.1:5500/index.html");
        }

        public void Dispose()
        {
            _driver.Quit();
        }

        [Fact]
        public void PageTitle_IsCorrect()
        {
            Assert.Equal("DR Musikplader", _driver.Title);
        }
    }
}