using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;

namespace Belletrix.Web.Tests
{
    [TestClass]
    [Ignore]
    public class ActivityLogTests : RootWebDriver
    {
        private const string DateTimeFormat = "MM/dd/yyyy";

        private void GoToActivityListing()
        {
            WebDriver.FindElement(By.LinkText("Activity Log")).Click();
            Assert.AreEqual(BaseUrl + "activitylog/list", WebDriver.Url);
        }

        private void GoToAddActivity()
        {
            WebDriver.FindElement(By.LinkText("Add Activity")).Click();
            StringAssert.StartsWith(WebDriver.Url, BaseUrl + "activitylog/add");
        }

        [TestMethod]
        public void ActivityListing_Loads()
        {
            Login();
            GoToActivityListing();
        }

        [TestMethod]
        public void AddActivity_Loads()
        {
            Login();
            GoToActivityListing();
            GoToAddActivity();
        }

        [TestMethod]
        public void AddActivity_SubmitDefaultForm_DisplaysError()
        {
            Login();
            GoToActivityListing();
            GoToAddActivity();

            Assert.AreEqual(0, WebDriver.FindElements(By.Id("Title-error")).Count);
            WebDriver.FindElement(By.XPath("//button[@type='submit'][text()='Add Activity']")).Submit();
            Assert.AreEqual(1, WebDriver.FindElements(By.Id("Title-error")).Count);
        }

        [TestMethod]
        public void AddActivity_SubmitBareMinimum_Succeeds()
        {
            Login();
            GoToActivityListing();
            GoToAddActivity();

            WebDriver.FindElement(By.Id("Title")).SendKeys(Guid.NewGuid().ToString());
            WebDriver.FindElement(By.Id("StartDate")).SendKeys(DateTime.Now.ToString(DateTimeFormat));
            WebDriver.FindElement(By.Id("EndDate")).SendKeys(DateTime.Now.AddDays(3).ToString(DateTimeFormat));

            // Opens the "Types" multiselect dropdown.
            WebDriver.FindElement(By.XPath("//button[@type='button'][@title='None selected']")).Click();

            // Enable the first checkbox shown.
            WebDriver.FindElement(By.XPath("//ul[contains(@class, 'multiselect-container')]/li/a/label/input")).Click();

            WebDriver.FindElement(By.XPath("//button[@type='submit'][text()='Add Activity']")).Submit();

            Assert.AreEqual(BaseUrl + "activitylog/list", WebDriver.Url);
        }
    }
}
