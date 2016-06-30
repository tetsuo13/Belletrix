using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;

namespace Belletrix.Web.Tests.Student
{
    [TestClass]
    [Ignore]
    public class StudentNoteTests : RootWebDriver
    {
        private void GoToStudentList()
        {
            Login();
            WebDriver.FindElement(By.LinkText("Students")).Click();
            Assert.AreEqual(BaseUrl + "student/list", WebDriver.Url);
        }

        private void VerifyViewingStudentView()
        {
            StringAssert.Contains(WebDriver.Url, BaseUrl + "student/view");
        }

        private void GoToSomeStudentView()
        {
            GoToStudentList();

            // Find the first student's name link that has a notes link which
            // indicates at least one note is available.
            By selector = By.XPath("(//a[contains(@class, 'studentnote')]/i[contains(@class, 'highlight-icon')]/../../../../../td)[1]/a");
            ReadOnlyCollection<IWebElement> studentNotelinks = WebDriver.FindElements(selector);

            studentNotelinks[0].Click();
            VerifyViewingStudentView();
        }

        private void GoToSomeStudentWithoutNotesView()
        {
            GoToStudentList();

            By selector = By.XPath("(//a[contains(@class, 'studentnote')][@data-original-title='Add Notes'])[1]/../../../../td[1]/a");
            ReadOnlyCollection<IWebElement> studentNotelinks = WebDriver.FindElements(selector);

            studentNotelinks[0].Click();
            VerifyViewingStudentView();
        }

        private void LaunchFirstNotesForStudent()
        {
            GoToStudentList();

            By selector = By.XPath("//a[contains(@class, 'studentnote')][@data-original-title='Add Notes']");
            ReadOnlyCollection<IWebElement> studentNotelinks = WebDriver.FindElements(selector);

            Assert.IsTrue(studentNotelinks.Count > 0);

            studentNotelinks[0].Click();
            WebDriver.FindElement(By.XPath("//div[contains(@class, 'bootbox')]"), 2);
        }

        [TestMethod]
        public void StudentList_ModalForStudentWithoutNotes_Loads()
        {
            LaunchFirstNotesForStudent();
        }

        [TestMethod]
        public void StudentList_ModalForStudentWithNotes_Loads()
        {
            GoToStudentList();

            By selector = By.XPath("//a[contains(@class, 'studentnote')]/i[contains(@class, 'highlight-icon')]/..");
            ReadOnlyCollection<IWebElement> studentNotelinks = WebDriver.FindElements(selector);

            Assert.IsTrue(studentNotelinks.Count > 0);

            studentNotelinks[0].Click();
            WebDriver.FindElement(By.XPath("//div[contains(@class, 'bootbox')]"), 2);
        }

        [TestMethod]
        public void StudentView_ModalForStudentWithoutNotes_Loads()
        {
            GoToSomeStudentWithoutNotesView();

            By selector = By.XPath("//a[contains(@class, 'studentnote')]");
            ReadOnlyCollection<IWebElement> studentNotelinks = WebDriver.FindElements(selector);

            Assert.AreEqual(1, studentNotelinks.Count);

            studentNotelinks[0].Click();
            WebDriver.FindElement(By.XPath("//div[contains(@class, 'bootbox')]"), 2);
        }

        [TestMethod]
        public void StudentView_ModalForStudentWithNotes_Loads()
        {
            GoToSomeStudentView();

            By selector = By.XPath("//a[contains(@class, 'studentnote')]");
            ReadOnlyCollection<IWebElement> studentNotelinks = WebDriver.FindElements(selector);

            Assert.IsTrue(studentNotelinks.Count > 0);

            studentNotelinks[0].Click();
            WebDriver.FindElement(By.XPath("//div[contains(@class, 'bootbox')]"), 2);
        }

        [TestMethod]
        public void AddFirstNote_AppearsInList()
        {
            LaunchFirstNotesForStudent();

            By selector = By.XPath("//div[contains(@class, 'bootbox')]/div[@class='list-group']");
            ReadOnlyCollection<IWebElement> existingNotes = WebDriver.FindElements(selector);

            Assert.AreEqual(0, existingNotes.Count, "Shouldn't be any existing notes");

            const string note = "Bacon ipsum dolor amet beef ribs dolore lorem mollit.";

            WebDriver.FindElement(By.Id("Note")).SendKeys(note);
            WebDriver.FindElement(By.Id("newnotebutton")).Click();

            // Wait for the note to appear.
            WebDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

            Assert.AreEqual(1, WebDriver.FindElements(selector).Count, "Should be one note after adding it");
            Assert.AreEqual(note, WebDriver.FindElement(By.XPath("//div[contains(@class, 'bootbox')]/div[@class='list-group']/a/p")).Text,
                "Displayed note should match what was typed in");
            Assert.AreEqual(String.Empty, WebDriver.FindElement(By.Id("Note")).Text);
        }
    }
}
