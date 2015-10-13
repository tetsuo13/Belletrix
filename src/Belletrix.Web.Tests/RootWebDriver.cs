using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace Belletrix.Web.Tests
{
    public class RootWebDriver
    {
        protected const string BaseUrl = "http://localhost:50758/";

        protected const string TestLoginUserName = "aaAuXiIo";
        protected const string TestLoginPassword = "tEd4VxAnMbm9E";

        protected IWebDriver WebDriver { get; set; }

        [TestInitialize]
        public void Setup()
        {
            WebDriver = new FirefoxDriver();
            WebDriver.Navigate().GoToUrl(BaseUrl);
        }

        [TestCleanup]
        public void TearDown()
        {
            WebDriver.Close();
            WebDriver.Dispose();
        }
    }
}
