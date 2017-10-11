using Microsoft.VisualStudio.TestTools.UnitTesting;
using BrookfieldGrs.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrookfieldGrs.Services;
using BrookfieldGrs.Web.Http.Tests.Mocks;

namespace BrookfieldGrs.Web.Http.Tests
{
    [TestClass]
    public class ServiceCommandApiControllerTests
    {
        [TestMethod]
        public void GetTest()
        {
            var instanceFactory = new MockServiceCommandContextInstanceFactory();
            var controller = new MockServiceCommandController(instanceFactory);
            controller.SetupControllerRequest("http://localhost/api/Mock");
            var commands = controller.Get();
            Assert.IsTrue(commands.Count() == 2);
        }
    }
}