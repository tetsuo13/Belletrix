using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace Belletrix.Web.Tests
{
    [TestClass]
    public class LoginPageTests
    {
        private IWebDriver WebDriver { get; set; }

        [TestInitialize]
        public void Setup()
        {
            WebDriver = new FirefoxDriver();
            WebDriver.Navigate().GoToUrl("http://localhost:50758/");
        }

        [TestCleanup]
        public void TearDown()
        {
            WebDriver.Close();
            WebDriver.Dispose();
        }

        [TestMethod]
        public void LoginFieldsExist()
        {
            Assert.IsNotNull(WebDriver.FindElement(By.Id("UserName")));
            Assert.IsNotNull(WebDriver.FindElement(By.Id("Password")));
            Assert.IsNotNull(WebDriver.FindElement(By.XPath("/html/body/div/form/button")));
        }

        [TestMethod]
        public void SubmitWithoutUsernameOrPassword_DisplaysErrors()
        {
            WebDriver.FindElement(By.XPath("/html/body/div/form/button")).Submit();

            Assert.IsNotNull(WebDriver.FindElement(By.XPath("/html/body/div/form/div[1]/span/span")));
            Assert.IsNotNull(WebDriver.FindElement(By.XPath("/html/body/div/form/div[2]/span/span")));
        }

        [TestMethod]
        public void SubmitWithoutPassword_DisplaysError()
        {
            WebDriver.FindElement(By.Id("UserName")).SendKeys("derp");
            WebDriver.FindElement(By.XPath("/html/body/div/form/button")).Submit();

            Assert.AreEqual(0, WebDriver.FindElements(By.XPath("/html/body/div/form/div[1]/span/span")).Count);
            Assert.IsNotNull(WebDriver.FindElement(By.XPath("/html/body/div/form/div[2]/span/span")));
        }

        [TestMethod]
        public void SubmitWithoutUsername_DisplaysError()
        {
            WebDriver.FindElement(By.Id("Password")).SendKeys("derp");
            WebDriver.FindElement(By.XPath("/html/body/div/form/button")).Submit();

            Assert.IsNotNull(WebDriver.FindElement(By.XPath("/html/body/div/form/div[1]/span/span")));
            Assert.AreEqual(0, WebDriver.FindElements(By.XPath("/html/body/div/form/div[2]/span/span")).Count);
        }

        [TestMethod]
        public void LoginWithInvalidCredentials_PageReloadWithErrors()
        {
            WebDriver.FindElement(By.Id("UserName")).SendKeys("foo");
            WebDriver.FindElement(By.Id("Password")).SendKeys("bar");
            WebDriver.FindElement(By.XPath("/html/body/div/form/button")).Submit();

            Assert.IsNotNull(WebDriver.FindElement(By.ClassName("validation-summary-errors")));
        }
    }
}
