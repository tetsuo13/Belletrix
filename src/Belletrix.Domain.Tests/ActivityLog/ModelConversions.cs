using Belletrix.Entity.Enum;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Belletrix.Domain.Tests
{
    [TestClass]
    public class ModelConversions
    {
        [TestMethod]
        public void ActivityLogEditViewModel_ActivityLogModel()
        {
            ActivityLogEditViewModel vm = new ActivityLogEditViewModel()
            {
                Types = new List<int> { (int)ActivityLogTypes.Conference, (int)ActivityLogTypes.SiteVisit }
            };
            ActivityLogModel model = (ActivityLogModel)vm;

            Assert.IsNotNull(model.Types);
            Assert.AreEqual(2, model.Types.Length);
            Assert.AreEqual(ActivityLogTypes.Conference, model.Types[0]);
            Assert.AreEqual(ActivityLogTypes.SiteVisit, model.Types[1]);
        }
    }
}
