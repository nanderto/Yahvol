namespace Yahvol.Services.Tests
{
    using System;
    using System.Data.Entity.Infrastructure;
    using System.Data.SqlClient;
    using System.Security.Principal;
    using System.Threading;

    using Yahvol.Services;
    using Yahvol.Services.Tests.Mocks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Threading.Tasks;

    [TestClass]
    public class RepositoryTests
    {
        [TestMethod]
        public void CreateRepository()
        {
            var serviceCommandRepository = new ServiceCommandRepository(new MockServiceCommandContextInstanceFactory());
            Assert.IsInstanceOfType(serviceCommandRepository, typeof(ServiceCommandRepository));
        }

        [TestMethod]
        public void SaveCommand()
        {
            var serviceCommandRepository = new ServiceCommandRepository(new MockServiceCommandContextInstanceFactory());
            var serviceCommand = new ServiceCommand("serialized input", DateTime.Now, "Created By", "Command Type", "user", "uniqueKey", "endpointId");

            var result = serviceCommandRepository.Save(ref serviceCommand);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task SaveCommandAsync()
        {
            var serviceCommandRepository = new ServiceCommandRepository(new MockServiceCommandContextInstanceFactory());
            var serviceCommand = new ServiceCommand("serialized input", DateTime.Now, "Created By", "Command Type", "user", "uniqueKey", "endpointId");

            var result = await serviceCommandRepository.SaveAsync(serviceCommand);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void CreateAdoContext()
        {
            var context = new AdoContext();

            Assert.IsInstanceOfType(context, typeof(AdoContext));
            Assert.IsInstanceOfType(context.Connection, typeof(SqlConnection));
        }

        [TestMethod, TestCategory("IntegrationDB")]
        public void AddServiceCommand()
        {
            var context = new AdoContext();
            var repository = new CommandRepository(context);
            var serviceCommand = new ServiceCommand("serialized input", DateTime.Now, "Created By", "Command Type", "user", Guid.NewGuid().ToString(), "endpointId");

            var result = repository.Save(ref serviceCommand);

            Assert.IsTrue(result > 0);
            var sql = string.Empty;
            var command = new SqlCommand(sql, context.Connection);
        }


        [TestMethod, TestCategory("IntegrationDB")]
        public void AddServiceCommand_with_User()
        {
            string[] rolesArray = { "managers", "executives" };
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("Bob", "Passport"), rolesArray);
            var username = Thread.CurrentPrincipal.Identity.Name;

            var contextFactory = new ServiceCommandContextInstanceFactory();
            var repository = new ServiceCommandRepository(contextFactory);
            var serviceCommand = new ServiceCommand("serialized input", DateTime.Now, "Created By", "Command Type", username, Guid.NewGuid().ToString(), "endpointId");

            var result = repository.Save(ref serviceCommand);

            Assert.IsTrue(result > 0);
        }

        [TestMethod, TestCategory("IntegrationDB")]
        public void AddServiceCommandwithEFContext()
        {
            var contextFactory = new ServiceCommandContextInstanceFactory();
            var repository = new ServiceCommandRepository(contextFactory);
            var serviceCommand = new ServiceCommand("serialized input", DateTime.Now, "Created By", "Command Type", "user", Guid.NewGuid().ToString(), "endpointId");

            var subscriber = new Subscriber { ServiceCommand = serviceCommand };

            serviceCommand.AddSubscriber(subscriber);

            var result = repository.Save(ref serviceCommand);

            Assert.IsTrue(result > 0);
        }

        [TestMethod, TestCategory("IntegrationDB")]
        public void AddServiceCommandWithSameUniqueId()
        {
            var guid = Guid.NewGuid().ToString();
            var repository = new ServiceCommandRepository(new ServiceCommandContextInstanceFactory());
            var serviceCommand = new ServiceCommand("serialized input", DateTime.Now, "Created By", "Command Type", "user", guid, "endpointId");

            var subscriber = new Subscriber { ServiceCommand = serviceCommand };

            serviceCommand.AddSubscriber(subscriber);
            var result = repository.Save(ref serviceCommand);

            var serviceCommand2 = new ServiceCommand("serialized input2", DateTime.Now, "Created By", "Command Type", "user", guid, "endpointId");

            var subscriber2 = new Subscriber { ServiceCommand = serviceCommand };
            serviceCommand2.AddSubscriber(subscriber2);
            var result3 = 0;
            try
            {
                result3 = repository.Save(ref serviceCommand2);
                Assert.Fail("repository should have thrown unique constraint error");
            }
            catch (DbUpdateException ex)
            {
                Assert.AreEqual("Violation of UNIQUE KEY constraint", ex.InnerException.InnerException.Message.Substring(0, 34));
            }
        }

        [TestMethod, TestCategory("IntegrationDB")]
        public void Run_Thows_Exception_If_UniqueKey_Violated()
        {
            var guid = Guid.NewGuid().ToString();
            var contextFactory = new ServiceCommandContextInstanceFactory();
            var repository = new ServiceCommandRepository(contextFactory);
            var serviceCommand = new ServiceCommand("serialized input", DateTime.Now, "Created By", "Command Type", "user", guid, "endpointId");

            var subscriber = new Subscriber { ServiceCommand = serviceCommand };

            serviceCommand.AddSubscriber(subscriber);
            var result = repository.Save(ref serviceCommand);

            var serviceCommand2 = new ServiceCommand("serialized input2", DateTime.Now, "Created By", "Command Type", "user", guid, "endpointId");

            var subscriber2 = new Subscriber { ServiceCommand = serviceCommand };
            serviceCommand2.AddSubscriber(subscriber2);

            var mockCommand = new MockCommand { UniqueKey = guid };
            //var serviceCommandService = new ServiceCommandService(repository);
            var serviceCommandService = new ServiceCommandService(contextFactory);
            var manualResetEvent = new ManualResetEvent(true);
            try
            {
                serviceCommandService.AddCommand<MockCommand>(mockCommand)
                    .WithWorkload(new MockWorkload(manualResetEvent), new TimeSpan(0, 0, 0, 0, 30));
                var transfereeCommand = serviceCommandService.Run<MockCommand>();

                Assert.Fail("Calling Run with a command with a non unique key should have caused an exception");
            }
            catch (DbUpdateException ex)
            {
                Assert.IsInstanceOfType(ex, typeof(Exception));
                Assert.AreEqual("Violation of UNIQUE KEY constraint", ex.InnerException.InnerException.Message.Substring(0, 34));
            }
            
            Assert.IsTrue(manualResetEvent.WaitOne(1000));
        }
    }
}
