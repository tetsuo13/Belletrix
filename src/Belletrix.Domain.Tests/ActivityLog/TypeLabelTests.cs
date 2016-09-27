using Belletrix.Entity.Enum;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Belletrix.Domain.Tests.ActivityLog
{
    [TestClass]
    public class TypeLabelTests
    {
        [TestMethod]
        public void NumberOfLabelsMatchesEnumCount()
        {
            int length = Enum.GetNames(typeof(ActivityLogTypes)).Length;
            IActivityService service = new ActivityService(null, null, null);
            Dictionary<int, string> labels = service.GetActivityTypeLabels();

            Assert.AreEqual(length, labels.Count);
        }

        [TestMethod]
        public void EachEnumValueAssociatedWithLabel()
        {
            IActivityService service = new ActivityService(null, null, null);
            Dictionary<int, string> labels = service.GetActivityTypeLabels();

            var values = Enum.GetValues(typeof(ActivityLogTypes)).Cast<ActivityLogTypes>();

            foreach (ActivityLogTypes type in values)
            {
                string message = string.Format("Checking value {0} ({1})", (int)type, type.ToString());
                Assert.IsTrue(labels.ContainsKey((int)type), message);
            }
        }
    }
}
