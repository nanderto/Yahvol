namespace Yahvol.Web.Http.Tests
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
    using Core;
    using Mocks;

    using Yahvol.Web.HttpTests;

    [TestClass]
    public class HubTests : HubTesterBase
    {
        [TestMethod]
        public async Task HubSendsMessage()
        {
            var manualResetEventMockSubscriber1Completed = new ManualResetEvent(false);
            var manualResetEventMockSubscriber2Completed = new ManualResetEvent(false);

            var subscribers = new List<Subscriber>();
            var mocSubscriber = new MockSubscriber(manualResetEventMockSubscriber1Completed);
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
            
            IServiceCommandService serviceCommandService = new WebCommandService(new ServiceCommandContextInstanceFactory());

            var manualResetEventCommandCompleted = new ManualResetEvent(false);
            var manualResetEventWorkloadCompleted = new ManualResetEvent(false);


           // var manualResetEvent3 = new ManualResetEvent(false);
            var hubConnection = new HubConnection("http://localhost:8081/signalr");
            
            IHubProxy serviceCommandHub = hubConnection.CreateHubProxy("ServiceCommandHub");
            
            serviceCommandHub.On<MockTransfereeCommand>("CommandCompleted", c =>
                {
                    var cmd = c as MockTransfereeCommand;
                    Assert.AreEqual(transfereeCommand.ConnectionId, cmd.ConnectionId);
                    Assert.AreEqual(transfereeCommand.Id, c.Id);
                    manualResetEventCommandCompleted.Set();
                });

            serviceCommandHub.On<string>("WorkloadCompleted", (c) =>
            {                
                Assert.AreEqual("MockWorkload", c);
                manualResetEventWorkloadCompleted.Set();
            });

            await hubConnection.Start();
            transfereeCommand.ConnectionId = hubConnection.ConnectionId;
            
            // Act
            var command = serviceCommandService.AddCommand<MockTransfereeCommand>(transfereeCommand)
                .WithWorkload(new MockWorkload(manualResetEventMockSubscriber1Completed), new TimeSpan(0, 0, 60, 0))
                .WithWorkload(new MockWorkload(manualResetEventMockSubscriber2Completed), new TimeSpan(0, 0, 60, 0))
                .Run<MockTransfereeCommand>();
            
            manualResetEventCommandCompleted.WaitOne();
            ManualResetEvent[] manualEvents = new ManualResetEvent[3];
            manualEvents[0] = manualResetEventCommandCompleted;
            manualEvents[1] = manualResetEventMockSubscriber1Completed;
            manualEvents[2] = manualResetEventMockSubscriber2Completed;

            ManualResetEvent.WaitAll(manualEvents);
            // Assert

            Assert.IsTrue(manualResetEventMockSubscriber1Completed.WaitOne(1000));
            Assert.IsTrue(manualResetEventMockSubscriber2Completed.WaitOne(1000));
            Assert.IsTrue(command.Id > 0);
        }
    }
}
