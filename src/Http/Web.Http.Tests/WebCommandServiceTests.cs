using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yahvol.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahvol.Web.Http.Tests
{
    using System.Threading;

    //using Yahvol.GlobalConstants;
    using Yahvol.Services;
    using Yahvol.Web.Http.Tests.Mocks;

    [TestClass()]
    public class WebCommandServiceTests
    {

        [TestMethod, Ignore]
        public async Task Throttle_TwoAtATime_ThirdWaitsForCompletion()
        {
            var throttleInstanceFactory = new ThrottleInstanceFactory { MaxExecutingSubscribers = 2 };

            var contextFactory = new MockServiceCommandContextInstanceFactory();

            var command = new MockCommand(string.Empty, "Gsp", "ThrottleTest");
            var manualResetEvent1 = new ManualResetEvent(false);
            var manualResetEvent2 = new ManualResetEvent(false);
            var manualResetEvent3 = new ManualResetEvent(false);

            new ServiceCommandService(contextFactory, throttleInstanceFactory.Create()).AddCommand(command)
                .WithWorkload(new MockWorkload(manualResetEvent1, 0, 2000), new TimeSpan(0, 0, 0, 30))
                .WithWorkload(new MockWorkload(manualResetEvent2, 0, 2000), new TimeSpan(0, 0, 0, 30))
                .Run<MockCommand>();
            new ServiceCommandService(contextFactory, throttleInstanceFactory.Create()).AddCommand(command)
                .WithWorkload(new MockWorkload(manualResetEvent3, 0, 2000), new TimeSpan(0, 0, 0, 30))
                .Run<MockCommand>();

            Thread.Sleep(3500);

            // First 2 subscribers ran immediately, Set after 2s
            Assert.IsTrue(manualResetEvent1.WaitOne(150));
            Assert.IsTrue(manualResetEvent2.WaitOne(150));

            // Third subscriber waits for first 2 to complete before starting, Set after 4s
            Assert.IsFalse(manualResetEvent3.WaitOne(150));
            Thread.Sleep(2400);
            Assert.IsTrue(manualResetEvent3.WaitOne(150));

            var subscribers = contextFactory.Context.Subscribers;
            var serviceCommand = contextFactory.Context.ServiceCommands.First();

            Assert.AreEqual(3, subscribers.Count());
            Assert.IsTrue(subscribers.All(s => s.Completed));
            Assert.IsTrue(serviceCommand.Completed);
        }
    }
}