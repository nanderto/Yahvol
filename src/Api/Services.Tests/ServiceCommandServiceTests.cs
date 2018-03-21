// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCommandServiceTests.cs" company="Anderton Engineered Solutions">
// Copyright © 2014 All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Yahvol.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;

    using Yahvol.GlobalConstants;
    using Yahvol.Services.Tests.Mocks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ServiceCommandServiceTests
    {
        [TestMethod, TestCategory("IntegrationDB")]
        public void GetByCreatedBy()
        {
            IServiceCommandService serviceCommandService = new ServiceCommandService();
            var commands = serviceCommandService.Get<ServiceCommand>(a => a.CreatedBy == "Created By");
            Assert.IsTrue(commands.Skip(1).Any());
        }

        [TestMethod, TestCategory("IntegrationDB")]
        public void GetTest()
        {
            string[] rolesArray = { "managers", "executives" };
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("user", "Passport"), rolesArray);
            var username = (ClaimsPrincipal)Thread.CurrentPrincipal;

            var serviceCommandService = new ServiceCommandService();

            var commands = serviceCommandService.Get<ServiceCommand>(a => a.User == username.Identity.Name);

            var count = 0;
            foreach (var serviceCommand in commands)
            {
                count++;
                Assert.AreEqual(username.Identity.Name, serviceCommand.User);
            }

            Assert.IsTrue(count > 1);
        }

        [TestMethod]
        public void LogEntriesTest()
        {
            var contextFactory = new MockServiceCommandContextInstanceFactory();
            var serviceCommandService = new ServiceCommandService(contextFactory);
            var command = new MockCommand("ThrowExceptionThenSucceed", ExternalSystems.Sec, "LogEntriesTest");
            var manualResetEvent = new ManualResetEvent(false);

            serviceCommandService.AddCommand(command).WithWorkload(new MockWorkload(manualResetEvent, 1), new TimeSpan(0, 0, 0, 30)).Run<MockCommand>();

            Assert.IsTrue(manualResetEvent.WaitOne(120000));

            Task.Delay(TimeSpan.FromMilliseconds(300)); //this manual reset event fires before the loggin is done so we need a little delay
            Assert.AreEqual(4, contextFactory.Context.LogEntries.Count());
            Assert.AreEqual(3, contextFactory.Context.LogEntries.Count(l => l.Severity == LoggingEventType.Information));
            Assert.AreEqual(1, contextFactory.Context.LogEntries.Count(l => l.Severity == LoggingEventType.Error));
        }

        [TestMethod]
        public void ReplaceDefaultLoggerTest()
        {
            var contextFactory = new MockServiceCommandContextInstanceFactory();
            var loggerFactory = new MockLoggerInstanceFactory();
            var serviceCommandService = new ServiceCommandService(contextFactory, loggerFactory);
            var command = new MockCommand("DoNothing", ExternalSystems.Sec, "LogEntriesTest");
            var manualResetEvent = new ManualResetEvent(false);

            serviceCommandService.AddCommand(command).WithWorkload(new MockWorkload(manualResetEvent), new TimeSpan(0, 0, 0, 30)).Run<MockCommand>();

            Assert.IsTrue(manualResetEvent.WaitOne(2000));

            // Logs to DB and Additional Place
            Assert.AreEqual(1, contextFactory.Context.LogEntries.Count());
            Assert.AreEqual(1, loggerFactory.Logger.LogEntries.Count);
        }

        [TestMethod]
        public void SubscriberFailsRetriesUpdatedTest()
        {
            var contextFactory = new MockServiceCommandContextInstanceFactory();
            var serviceCommandService = new ServiceCommandService(contextFactory);
            var command = new MockCommand("ThrowExceptionThenWait", ExternalSystems.Sec, "SubscriberFailsRetriesUpdatedTest");
            var manualResetEvent = new ManualResetEvent(false);

            serviceCommandService.AddCommand(command).WithWorkload(new MockWorkload(manualResetEvent, 3), new TimeSpan(0, 0, 0, 30));

            serviceCommandService.Run<MockCommand>();

            Assert.IsTrue(manualResetEvent.WaitOne(8000));

            var subscriber = contextFactory.Context.Subscribers.First();

            Assert.IsNotNull(subscriber);
            Assert.AreEqual(3, subscriber.RetryCount);
            Assert.IsFalse(subscriber.Completed);
            Assert.IsNotNull(subscriber.LastUpdatedDate);
        }

        [TestMethod]
        public async Task SubscriberSucceedsRetriesUpdatedTest()
        {
            var contextFactory = new MockServiceCommandContextInstanceFactory();
            var serviceCommandService = new ServiceCommandService(contextFactory);
            var command = new MockCommand(string.Empty, ExternalSystems.Sec, "SubscriberSucceedsRetriesUpdatedTest");
            var manualResetEvent = new ManualResetEvent(false);

            serviceCommandService.AddCommand(command).WithWorkload(new MockWorkload(manualResetEvent), new TimeSpan(0, 0, 0, 30));

            command = serviceCommandService.Run<MockCommand>();

            Assert.IsTrue(manualResetEvent.WaitOne(1000));
	        await Task.Delay(200);

            var subscriber = contextFactory.Context.Subscribers.First();
            var serviceCommand = contextFactory.Context.ServiceCommands.First();

            Assert.IsNotNull(subscriber);
            Assert.AreEqual(1, subscriber.RetryCount);
            Assert.IsTrue(subscriber.Completed);
            Assert.IsNotNull(subscriber.LastUpdatedDate);
            Assert.IsTrue(serviceCommand.Completed);
        }

        [TestMethod, TestCategory("IntegrationDB")]
        public async Task ThreeSubscribersSucceedIntegrationTest()
        {
            var contextFactory = new ServiceCommandContextInstanceFactory();
            var serviceCommandService = new ServiceCommandService(contextFactory);
            var command = new MockCommand(string.Empty, ExternalSystems.Sec, "ThreeSubscribersSucceedIntegrationTest");
            var manualResetEvent1 = new ManualResetEvent(false);
            var manualResetEvent2 = new ManualResetEvent(false);
            var manualResetEvent3 = new ManualResetEvent(false);

            serviceCommandService.AddCommand(command).WithWorkload(new MockWorkload(manualResetEvent1), new TimeSpan(0, 0, 0, 30)).WithWorkload(new MockWorkload(manualResetEvent2), new TimeSpan(0, 0, 0, 30)).WithWorkload(new MockWorkload(manualResetEvent3), new TimeSpan(0, 0, 0, 30));

            command = serviceCommandService.Run<MockCommand>();

            Assert.IsTrue(manualResetEvent1.WaitOne(4000));
            Assert.IsTrue(manualResetEvent2.WaitOne(4000));
            Assert.IsTrue(manualResetEvent3.WaitOne(4000));
            await Task.Delay(500);

            var serviceCommand = serviceCommandService.Get<ServiceCommand>(s => s.Id == command.Id).First();
            var subscribers = serviceCommandService.Get<Subscriber>(s => s.ServiceCommandId == command.Id).ToList();

            Assert.IsNotNull(serviceCommand);
            Assert.AreEqual(3, subscribers.Count);

            Assert.IsTrue(subscribers.All(s => s.RetryCount == 1));
            Assert.IsTrue(subscribers.All(s => s.Completed));
            Assert.IsTrue(subscribers.All(s => s.LastUpdatedDate != null));

            Assert.IsTrue(serviceCommand.Completed);
        }

        [TestMethod, TestCategory("IntegrationDB")]
        public async Task TwoSubscribersRetryAndSucceedIntegrationTest()
        {
            var contextFactory = new ServiceCommandContextInstanceFactory();
            var serviceCommandService = new ServiceCommandService(contextFactory);
            var command = new MockCommand("ThrowExceptionThenSucceed", ExternalSystems.Sec, "TwoSubscribersRetryAndSucceedIntegrationTest");
            var manualResetEvent1 = new ManualResetEvent(false);
            var manualResetEvent2 = new ManualResetEvent(false);

            serviceCommandService.AddCommand(command).WithWorkload(new MockWorkload(manualResetEvent1, 2), new TimeSpan(0, 0, 0, 30)).WithWorkload(new MockWorkload(manualResetEvent2, 1), new TimeSpan(0, 0, 0, 30));

            command = serviceCommandService.Run<MockCommand>();

            Assert.IsTrue(manualResetEvent1.WaitOne(8000));
            Assert.IsTrue(manualResetEvent2.WaitOne(2000));
            await Task.Delay(2000);

            var serviceCommand = serviceCommandService.Get<ServiceCommand>(s => s.Id == command.Id).First();
            var subscribers = serviceCommandService.Get<Subscriber>(s => s.ServiceCommandId == command.Id).ToList();
            var subscriber1 = subscribers.OrderBy(u => u.Id).First();
            var subscriber2 = subscribers.OrderByDescending(u => u.Id).First();

            Assert.IsNotNull(serviceCommand);
            Assert.IsNotNull(subscriber1);
            Assert.IsNotNull(subscriber2);

            Assert.AreEqual(3, subscriber1.RetryCount);
            Assert.AreEqual(2, subscriber2.RetryCount);

            Assert.IsTrue(subscriber1.Completed);
            Assert.IsTrue(subscriber2.Completed);
            Assert.IsNotNull(subscriber1.LastUpdatedDate);
            Assert.IsNotNull(subscriber2.LastUpdatedDate);

            Assert.IsTrue(serviceCommand.Completed);
        }

        [TestMethod]
        public void WithOneExceptionThatDoesNotCauseRestartTest()
        {
            var manualResetEvent = new ManualResetEvent(false);

            var contextFactory = new MockServiceCommandContextInstanceFactory();
            var loggerFactory = new MockLoggerInstanceFactory();
            var serviceCommandService = new ServiceCommandService(contextFactory, loggerFactory);
            var command = new MockCommand("DoNothing", ExternalSystems.Sec, "LogEntriesTest");
            var mockRestarterWorkload = new MockRestarterWorkload(manualResetEvent, new TimeSpan(1000), 3, typeof(NullReferenceException), new RetryPolicy(20, 20, BackOffStrategy.Squared, 3, new List<Type> { typeof(NullReferenceException) }));

            serviceCommandService.AddCommand(command).WithWorkload(mockRestarterWorkload, new TimeSpan(0, 0, 0, 30)).Run<MockCommand>();

            Assert.IsTrue(manualResetEvent.WaitOne(10000));
            Assert.AreEqual(1, mockRestarterWorkload.Count()); //does not matter howmany Exceptions it is configureed to throw the first exempt exception that matches will stop the looping - we should never see more than 1
        }

        [TestMethod]
        public void WithOneExceptionThatIsNotExempt_CauseRestartTest()
        {
            var manualResetEvent = new ManualResetEvent(false);

            var contextFactory = new MockServiceCommandContextInstanceFactory();
            var loggerFactory = new MockLoggerInstanceFactory();
            var serviceCommandService = new ServiceCommandService(contextFactory, loggerFactory);
            var command = new MockCommand("DoNothing", ExternalSystems.Sec, "LogEntriesTest");

            var mockRestarterWorkload = new MockRestarterWorkload(manualResetEvent, 2, typeof(NullReferenceException), new RetryPolicy(20, 20, BackOffStrategy.Linear, 5, new List<Type> { typeof(ArgumentNullException) })); //will throw 2 exceptions //will retry 5 exceptions

            serviceCommandService.AddCommand(command).WithWorkload(mockRestarterWorkload, new TimeSpan(0, 0, 0, 30)).Run<MockCommand>();

            Assert.IsTrue(manualResetEvent.WaitOne(10000));

            Assert.AreEqual(3, mockRestarterWorkload.Count()); //1st try + 2 retrys
        }

        [TestMethod]
        public void WithOneWorkloadThrowing3ExceptionsAndNoPolicy()
        {
            var manualResetEvent = new ManualResetEvent(false);

            var contextFactory = new MockServiceCommandContextInstanceFactory();
            var loggerFactory = new MockLoggerInstanceFactory();
            var serviceCommandService = new ServiceCommandService(contextFactory, loggerFactory);
            var command = new MockCommand("DoNothing", ExternalSystems.Sec, "LogEntriesTest");
            var mockRestarterWorkload = new MockRestarterWorkload(manualResetEvent, new TimeSpan(1000), 3, typeof(NullReferenceException), new RetryPolicy(2, 2, BackOffStrategy.Squared, 3)); //will try to throw 3 exceptions

            serviceCommandService.AddCommand(command).WithWorkload(mockRestarterWorkload, new TimeSpan(0, 0, 0, 30)).Run<MockCommand>();

            Assert.IsTrue(manualResetEvent.WaitOne(1000));

            Assert.AreEqual(3, mockRestarterWorkload.Count()); //Count includes first attempt to execute workload  + 2 retrys based on the number of exceptions to throw above.
        }

        [TestMethod]
        public void WithRetryConfigurationTest()
        {
            var contextFactory = new MockServiceCommandContextInstanceFactory();
            var serviceCommandService = new ServiceCommandService(contextFactory);
            var command = new MockCommand("ThrowExceptionThenWait", ExternalSystems.Sec, "SubscriberFailsRetriesUpdatedTest");
            var manualResetEvent = new ManualResetEvent(false);

            var mockWorkload = new MockWorkload(manualResetEvent, 3);

            serviceCommandService.AddCommand(command).WithWorkload(mockWorkload, new TimeSpan(0, 0, 0, 30));
            var attachedWorkload = (MockWorkload)serviceCommandService.Subscribers.FirstOrDefault().Workload;

            Assert.IsTrue(mockWorkload == attachedWorkload);
        }

        [TestMethod]
        public async Task WithRetryPolicySquareandAndExceptionToBeThrownNotIncludedInExemptions()
        {
            var manualResetEvent = new ManualResetEvent(false);
            var mockRestarterWorkload = new MockRestarterWorkload(manualResetEvent, new TimeSpan(0, 0, 0, 0, 1000), 2, typeof(NullReferenceException));
            var mocSubscriber = new MockRestartSubscriber(mockRestarterWorkload);

            var reStarter = new ReStarter(mocSubscriber);
	        await reStarter.StartAsync();
            manualResetEvent.WaitOne(10000);
            var count = mockRestarterWorkload.Count();
            Assert.AreEqual(3, count);
        }

        [TestMethod]
        public void WithRetryPolicySquareTest()
        {
            var contextFactory = new MockServiceCommandContextInstanceFactory();
            var loggerFactory = new MockLoggerInstanceFactory();
            var serviceCommandService = new ServiceCommandService(contextFactory, loggerFactory);
            var command = new MockCommand("DoNothing", ExternalSystems.Sec, "LogEntriesTest");
            var manualResetEvent = new ManualResetEvent(false);

            var mockWorkload = new MockWorkload(manualResetEvent, new RetryPolicy(2, 2, BackOffStrategy.Squared, 3));

            serviceCommandService.AddCommand(command).WithWorkload(mockWorkload, new TimeSpan(0, 0, 0, 30)).Run<MockCommand>();

            Assert.IsTrue(manualResetEvent.WaitOne(2000));

            // Logs to DB and Additional Place
            Assert.AreEqual(1, contextFactory.Context.LogEntries.Count());
            Assert.AreEqual(1, loggerFactory.Logger.LogEntries.Count);
        }

        [TestMethod]
        public void WithRetryPolicyThatIsNottheDefault()
        {
            var contextFactory = new MockServiceCommandContextInstanceFactory();
            var loggerFactory = new MockLoggerInstanceFactory();
            var serviceCommandService = new ServiceCommandService(contextFactory, loggerFactory);
            var command = new MockCommand("DoNothing", ExternalSystems.Sec, "LogEntriesTest");
            var manualResetEvent = new ManualResetEvent(false);

            var mockRestarterWorkload = new MockRestarterWorkload(manualResetEvent, 4, typeof(NullReferenceException), new RetryPolicy(2, 2, BackOffStrategy.Squared, 3));

            serviceCommandService.AddCommand(command).WithWorkload(mockRestarterWorkload, new TimeSpan(0, 0, 0, 30)).Run<MockCommand>();

            Assert.IsTrue(manualResetEvent.WaitOne(1000));
            Assert.AreEqual(3, mockRestarterWorkload.Count());
        }
    }
}