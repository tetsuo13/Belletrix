using Belletrix.Entity.Enum;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Belletrix.Core.Tests
{
    [TestClass]
    public class FormatterTests
    {
        private readonly IDictionary<int, string> TypeLabels = new Dictionary<int, string>()
            {
                { 1, "one" },
                { 2, "two" },
                { 3, "three" },
                { 4, "four" },
                { 5, "five" },
                { 6, "six" },
                { 7, "seven" },
                { 8, "eight" },
                { 9, "nine" },
            };

        [TestMethod]
        public void ActivityLog_WithinBounds()
        {
            Assert.AreEqual("seven", Formatter.ActivityLogLabel(ActivityLogTypes.Meeting, TypeLabels));
        }

        [TestMethod]
        public void ActivityLog_Zero_ReturnsDefault()
        {
            Assert.AreEqual("one", Formatter.ActivityLogLabel(ActivityLogTypes.Workshop, TypeLabels));
        }
    }
}
