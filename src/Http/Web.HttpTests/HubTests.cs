namespace Yahvol.Web.HttpTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Yahvol.Services;
    using Yahvol.Web.Http;

    using Microsoft.AspNet.SignalR.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Data;
    [TestClass]
    public class HubTests : HubTesterBase
    {
        [TestMethod]
        public async Task HubSendsMessage()
        {
            var manualResetEventMockSubscriber1Completed = new ManualResetEvent(false);
            var manualResetEventMockSubscriber2Completed = new ManualResetEvent(false);

            var subscribers = new List<Subscriber>();
            var mocSubscriber = new MockSubscriber(manualResetEventMockSubscriber1Completed );
            var mocSubscriber2 = new MockSubscriber(manualResetEventMockSubscriber2Completed);
            subscribers.Add(mocSubscriber);
            subscribers.Add(mocSubscriber2);

            var transferee = new Transferee
            {
                FirstName = "John",
                LastName = "wilson"
            };

            var startTime = DateTime.Now;
            var transfereeCommand = new MockTransfereeCommand
            {
                CommandType = "Add",
                CreateDate = startTime,
                CreatedBy = Thread.CurrentPrincipal.Identity.Name,
                GrsSystemType = GrsSystemType.Gsp,
                Transferee = transferee
            };
            
            IServiceCommandService serviceCommandService = new WebCommandService(new MockServiceCommandContextInstanceFactory());

            var manualResetEventCommandCompleted = new ManualResetEvent(false);
            var manualResetEventWorkloadCompleted = new ManualResetEvent(false);

            var hubConnection = new HubConnection("http://localhost:8081/signalr");
            IHubProxy serviceCommandHub = hubConnection.CreateHubProxy("ServiceCommandHub");
            serviceCommandHub.On<MockTransfereeCommand>("CommandCompleted", (c) =>
                {
                    Assert.AreEqual(transfereeCommand.ConnectionId, c.ConnectionId);
                    Assert.AreEqual(transfereeCommand.Id, c.Id);
                    manualResetEventCommandCompleted.Set();
                });

            //serviceCommandHub.On<MockTransfereeCommand>("WorkloadCompleted", (w) =>
            //{
            //    Assert.AreEqual(transfereeCommand.GetType().Name, w.GetType().Name);
            //    Assert.AreEqual(transfereeCommand.Id, w.Id);
            //    manualResetEventWorkloadCompleted.Set();
            //});

            await hubConnection.Start();
            transfereeCommand.ConnectionId = hubConnection.ConnectionId;
            
            // Act
            var command = serviceCommandService.AddCommand<MockTransfereeCommand>(transfereeCommand)
                .WithWorkload(new MockWorkload(manualResetEventMockSubscriber1Completed ), new TimeSpan(0, 0, 0, 0, 30))
                .WithWorkload(new MockWorkload(manualResetEventMockSubscriber2Completed), new TimeSpan(0, 0, 0, 0, 30))
                .Run<MockTransfereeCommand>();
            
            manualResetEventCommandCompleted.WaitOne();
            
            // Assert
            Assert.IsTrue(manualResetEventMockSubscriber1Completed .WaitOne(1000));
            Assert.IsTrue(manualResetEventMockSubscriber2Completed.WaitOne(1000));
            Assert.IsTrue(command.Id == 1);
        }
    }
}
