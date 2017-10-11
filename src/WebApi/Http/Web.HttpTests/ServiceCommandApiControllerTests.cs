namespace BrookfieldGrs.Web.Http.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using BrookfieldGrs.Services;
    using BrookfieldGrs.Web.Http;
    using BrookfieldGrs.Web.HttpTests;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ServiceCommandApiControllerTests
    {
        [TestMethod]
        public void GetTest()
        {
            //var controller = new MockServiceCommandController(new MockRepository());
            //controller.SetupControllerRequest("http://localhost/api/Mock");
            //var commands = controller.Get();
            //Assert.IsTrue(commands.Count() == 2);
        }

        [TestMethod]
        public void Get_Restricted_by_User_Test()
        {
            //string[] rolesArray = { "managers", "executives" };
            //Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("Bob", "Passport"), rolesArray);
            //var username = Thread.CurrentPrincipal.Identity.Name;

            //var controller = new MockServiceCommandController(new ServiceCommandRepository());
            //controller.SetupControllerRequest("http://localhost/api/Mock");
            //var commands = controller.Get(scmd => scmd.CreatedBy == "Created By");
            //foreach (var serviceCommand in commands)
            //{
            //    Assert.AreEqual("Created By", serviceCommand.CreatedBy);
            //    Assert.AreEqual(username, serviceCommand.User);
            //}
            //Assert.IsTrue(commands.Count() > 0);
        }
    }
}
