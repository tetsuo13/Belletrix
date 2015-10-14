using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace Belletrix.Web.Tests
{
    public class RootWebDriver
    {
        protected const string BaseUrl = "http://localhost:50758/";

        protected IWebDriver WebDriver { get; set; }

        private const string TestLoginUserName = "aaAuXiIo";
        private const string TestLoginPassword = "tEd4VxAnMbm9E";

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

        protected void Login()
        {
            WebDriver.FindElement(By.Id("UserName")).SendKeys(TestLoginUserName);
            WebDriver.FindElement(By.Id("Password")).SendKeys(TestLoginPassword);
            WebDriver.FindElement(By.XPath("/html/body/div/form/button")).Submit();

            Assert.AreEqual(BaseUrl + "home/index", WebDriver.Url);
        }
    }
}
