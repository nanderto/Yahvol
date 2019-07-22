using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yahvol.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahvol.Services;
using Yahvol.Web.Http.Tests.Mocks;

namespace Yahvol.Web.Http.Tests
{
    using Yahvol.Data;

    [TestClass]
    public class ServiceCommandApiControllerTests
    {
        [TestMethod]
        public void GetTest()
        {
            var instanceFactory = new MockServiceCommandContextInstanceFactory() as IInstanceFactory<IServiceCommandService>;
            
            var controller = new MockServiceCommandController(instanceFactory);
            controller.SetupControllerRequest("http://localhost/api/Mock");
            var commands = controller.Get();
            Assert.IsTrue(commands.Count() == 2);
        }
    }
}