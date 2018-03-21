namespace Yahvol.Services.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Yahvol.GlobalConstants;
    using Yahvol.Services;
    using Yahvol.Services.Commands.AD;
    using Yahvol.Services.Tests.Mocks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WorkloadBaseTests
    {
        [TestMethod]
        public void GetCommandTest()
        {
            var manualResetEvent = new ManualResetEvent(false);
            var workload = new MockWorkload(manualResetEvent);
            workload.SetCommand(new MockCommand());
            Assert.IsInstanceOfType(workload.Command, typeof(MockCommand));
        }

        [TestMethod]
        public void RegisterEventTest()
        {
            var manualResetEvent = new ManualResetEvent(false);
            var workload = new MockWorkload(manualResetEvent);
            workload.SetCommand(new MockCommand());

            workload.RegisterEvent("Completed", this.Completed);
            Assert.IsInstanceOfType(workload.Command, typeof(MockCommand));
        }

        public void Completed()
        {
 
        }

        // todo - Break these into smaller chunks.
        [TestMethod]
        public async Task MockWorkloadBaseExtendedTest()
        {
            var clientCommand = new ClientCommand("SomeCommand", ExternalSystems.Sec, "ExtendedWorkload");
            var userCommand = new UserCommand(UserCommand.Actions.Upsert, ExternalSystems.Sec, "ExtendedWorkload");

            var workload = new MockWorkloadBaseExtended(true);
            Assert.IsInstanceOfType(workload, typeof(MockWorkloadBaseExtended));

            workload.SetCommand(clientCommand);
            await workload.Execute(new CancellationToken());
            Assert.IsTrue(workload.DefaultActionCalled);
            Assert.IsFalse(workload.ClientAddCalled);
            Assert.IsFalse(workload.ClientDeleteCalled);
            Assert.IsFalse(workload.UserUpsertCalled);

            clientCommand.CommandAction = ClientCommand.Actions.Add;
            workload.Reset();
            workload.SetCommand(clientCommand);
            await workload.Execute(new CancellationToken());
            Assert.IsFalse(workload.DefaultActionCalled);
            Assert.IsTrue(workload.ClientAddCalled);
            Assert.IsFalse(workload.ClientDeleteCalled);
            Assert.IsFalse(workload.UserUpsertCalled);

            clientCommand.CommandAction = ClientCommand.Actions.Delete;
            workload.Reset();
            workload.SetCommand(clientCommand);
            await workload.Execute(new CancellationToken());
            Assert.IsFalse(workload.DefaultActionCalled);
            Assert.IsFalse(workload.ClientAddCalled);
            Assert.IsTrue(workload.ClientDeleteCalled);
            Assert.IsFalse(workload.UserUpsertCalled);

            workload.Reset();
            workload.SetCommand(userCommand);
            try
            {
                await workload.Execute(new CancellationToken());
            }
            catch (Exception ex)
            {
                {
                    Assert.IsInstanceOfType(ex, typeof(InvalidCastException));
                    Assert.IsFalse(workload.DefaultActionCalled);
                    Assert.IsFalse(workload.ClientAddCalled);
                    Assert.IsFalse(workload.ClientDeleteCalled);
                    Assert.IsFalse(workload.UserUpsertCalled);

                }
            }
        }

        [TestMethod]
        public void MockWorkloadBaseExtendedFailsTest()
        {
            // Try to handle 2 of the same CommandType
            try
            {
                new MockWorkloadBaseExtendedFails1();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentException));
                Assert.IsTrue(ex.Message.Contains("already exists"));
            }
        }
    }

    public class MockWorkloadBaseExtended : WorkloadBase<ClientCommand>
    {
        public MockWorkloadBaseExtended(bool setupHandlers = false, RetryPolicy retryPolicy = default(RetryPolicy)) : base(retryPolicy)
        {
            if (!setupHandlers)
            {
                return;
            }

            this.HandleCommand(ClientCommand.Actions.Add, this.ClientAdd);
            this.HandleCommand("Delete", this.ClientDelete);
        }

        public bool DefaultActionCalled { get; set; }

        public bool ClientAddCalled { get; set; }

        public bool ClientDeleteCalled { get; set; }

        public bool UserUpsertCalled { get; set; }

        public void Reset()
        {
            this.DefaultActionCalled = false;
            this.ClientAddCalled = false;
            this.ClientDeleteCalled = false;
            this.UserUpsertCalled = false;
        }

        public override void DefaultAction()
        {
            this.DefaultActionCalled = true;
        }

        protected void ClientAdd()
        {
            this.ClientAddCalled = true;
        }

        protected void ClientDelete()
        {
            this.ClientDeleteCalled = true;
        }

        protected void UserUpsert()
        {
            this.UserUpsertCalled = true;
        }
    }

    public class MockWorkloadBaseExtendedFails1 : WorkloadBase<ClientCommand>
    {
        public MockWorkloadBaseExtendedFails1(RetryPolicy retryPolicy = default(RetryPolicy)) : base(retryPolicy)
        {
            this.HandleCommand(ClientCommand.Actions.Add, this.DefaultAction);
            this.HandleCommand(ClientCommand.Actions.Add, this.DefaultAction);
        }
    }
}
