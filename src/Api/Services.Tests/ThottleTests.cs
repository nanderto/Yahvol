namespace Yahvol.Services.Tests
{
	using System;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Yahvol.GlobalConstants;
	using Yahvol.Services.Tests.Mocks;

	using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Diagnostics;

    [TestClass]
	public class ThottleTests
	{
		[TestMethod]
		public async Task Throttle_TwoAtATime_ThirdWaitsForCompletion()
		{
			var throttleInstanceFactory = new ThrottleInstanceFactory { MaxExecutingSubscribers = 2 };

			var contextFactory = new MockServiceCommandContextInstanceFactory();

			var command = new MockCommand(string.Empty, ExternalSystems.Sec, "ThrottleTest");
			var manualResetEvent1 = new ManualResetEvent(false);
			var manualResetEvent2 = new ManualResetEvent(false);
			var manualResetEvent3 = new ManualResetEvent(false);
            var stopWatch = new Stopwatch();
            stopWatch.Start();
			new ServiceCommandService(contextFactory, throttleInstanceFactory.Create()).AddCommand(command)
				.WithWorkload(new MockWorkload(manualResetEvent1, 0, 2000), new TimeSpan(0, 0, 0, 30))
				.WithWorkload(new MockWorkload(manualResetEvent2, 0, 2000), new TimeSpan(0, 0, 0, 30))
				.Run<MockCommand>();

            var elapsed = stopWatch.ElapsedMilliseconds;
            Trace.WriteLine($"Elapsed time after starting one: {elapsed}");

            new ServiceCommandService(contextFactory, throttleInstanceFactory.Create()).AddCommand(command)
				.WithWorkload(new MockWorkload(manualResetEvent3, 0, 2000), new TimeSpan(0, 0, 0, 30))
				.Run<MockCommand>();

            elapsed = stopWatch.ElapsedMilliseconds;
            Trace.WriteLine($"Elapsed time after starting two: {elapsed}");
			////Thread.Sleep(3500);

			// First 2 subscribers ran immediately, Set after 2s
 			Assert.IsTrue(manualResetEvent1.WaitOne(2500));

            elapsed = stopWatch.ElapsedMilliseconds;
            Trace.WriteLine($"Elapsed time after waiting one: {elapsed}");
            Assert.IsTrue(manualResetEvent2.WaitOne(2500));

            elapsed = stopWatch.ElapsedMilliseconds;
            Trace.WriteLine($"Elapsed time after waiting two: {elapsed}");
            // Third subscriber waits for first 2 to complete before starting, Set after 4s
            Assert.IsFalse(manualResetEvent3.WaitOne(4000));

            elapsed = stopWatch.ElapsedMilliseconds;
            Trace.WriteLine($"Elapsed time after checking 3rd not finished: {elapsed}");

            //Thread.Sleep(2400);

            //elapsed = stopWatch.ElapsedMilliseconds;
            //Trace.WriteLine($"Elapsed time after waiting :    {elapsed}");

            Assert.IsTrue(manualResetEvent3.WaitOne(2800));

            elapsed = stopWatch.ElapsedMilliseconds;
            Trace.WriteLine($"Elapsed time after waiting three:    {elapsed}");

            var subscribers = contextFactory.Context.Subscribers;
			var serviceCommand = contextFactory.Context.ServiceCommands.First();

			Assert.AreEqual(3, subscribers.Count());
			Assert.IsTrue(subscribers.All(s => s.Completed));
			Assert.IsTrue(serviceCommand.Completed);
		}

		[TestMethod]
		public async Task Throttle_FourAtATime_WaitsForCompletion()
		{
			var throttleInstanceFactory = new ThrottleInstanceFactory { MaxExecutingSubscribers = 4 };

			var contextFactory = new MockServiceCommandContextInstanceFactory();

			var command = new MockCommand(string.Empty, ExternalSystems.Sec, "ThrottleTest");
			var manualResetEvent1 = new ManualResetEvent(false);
			var manualResetEvent2 = new ManualResetEvent(false);
			var manualResetEvent3 = new ManualResetEvent(false);
			var manualResetEvent4 = new ManualResetEvent(false);
			var manualResetEvent5 = new ManualResetEvent(false);

			new ServiceCommandService(contextFactory, throttleInstanceFactory.Create()).AddCommand(command)
				.WithWorkload(new MockWorkload(manualResetEvent1, 0, 2000), new TimeSpan(0, 0, 0, 30))
				.Run<MockCommand>();
			new ServiceCommandService(contextFactory, throttleInstanceFactory.Create()).AddCommand(command)
				.WithWorkload(new MockWorkload(manualResetEvent2, 0, 2000), new TimeSpan(0, 0, 0, 30))
				.Run<MockCommand>();
			new ServiceCommandService(contextFactory, throttleInstanceFactory.Create()).AddCommand(command)
				.WithWorkload(new MockWorkload(manualResetEvent3, 0, 2000), new TimeSpan(0, 0, 0, 30))
				.Run<MockCommand>();
			new ServiceCommandService(contextFactory, throttleInstanceFactory.Create()).AddCommand(command)
				.WithWorkload(new MockWorkload(manualResetEvent4, 0, 2000), new TimeSpan(0, 0, 0, 30))
				.Run<MockCommand>();
			new ServiceCommandService(contextFactory, throttleInstanceFactory.Create()).AddCommand(command)
				.WithWorkload(new MockWorkload(manualResetEvent5, 0, 2000), new TimeSpan(0, 0, 0, 30))
				.Run<MockCommand>();

			await Task.Delay(3500);

			// First 4 subscribers ran immediately, Set after 2s
			Assert.IsTrue(manualResetEvent1.WaitOne(100));
			Assert.IsTrue(manualResetEvent2.WaitOne(100));
			Assert.IsTrue(manualResetEvent3.WaitOne(100));
			Assert.IsTrue(manualResetEvent4.WaitOne(100));

			// Final subscriber waits for first 4 to complete before starting, Set after 4s
			Assert.IsFalse(manualResetEvent5.WaitOne(100));
			await Task.Delay(2100);
			Assert.IsTrue(manualResetEvent5.WaitOne(100));

			var subscribers = contextFactory.Context.Subscribers;
			var serviceCommand = contextFactory.Context.ServiceCommands.First();

			Assert.AreEqual(5, subscribers.Count());
			Assert.IsTrue(subscribers.All(s => s.Completed));
			Assert.IsTrue(serviceCommand.Completed);
		}

		[TestMethod]
		public async Task Throttle_ThrowExceptions_MovesToBackOfQueue()
		{
			var throttleInstanceFactory = new ThrottleInstanceFactory { MaxExecutingSubscribers = 1 };

			var contextFactory = new MockServiceCommandContextInstanceFactory();

			var command = new MockCommand("WaitAndThrowException", ExternalSystems.Nka, "ThrottleTest");
			var manualResetEvent1 = new ManualResetEvent(false);
			var manualResetEvent2 = new ManualResetEvent(false);
			var manualResetEvent3 = new ManualResetEvent(false);

			new ServiceCommandService(contextFactory, throttleInstanceFactory.Create()).AddCommand(command)
				.WithWorkload(new MockWorkload(manualResetEvent1, 1, 2000, new RetryPolicy(4000, 4000)), new TimeSpan(0, 0, 0, 30))
				.Run<MockCommand>();
			new ServiceCommandService(contextFactory, throttleInstanceFactory.Create()).AddCommand(command)
				.WithWorkload(new MockWorkload(manualResetEvent2, 0, 2000), new TimeSpan(0, 0, 0, 30))
				.Run<MockCommand>();
			new ServiceCommandService(contextFactory, throttleInstanceFactory.Create()).AddCommand(command)
				.WithWorkload(new MockWorkload(manualResetEvent3, 0, 2000), new TimeSpan(0, 0, 0, 30))
				.Run<MockCommand>();

			// 1 starts, runs 2s, throws exception
			// 2 starts
			// 3 waiting
			await Task.Delay(2100);
			Assert.IsFalse(manualResetEvent1.WaitOne(100));
			Assert.IsFalse(manualResetEvent2.WaitOne(100));
			Assert.IsFalse(manualResetEvent3.WaitOne(100));

			// 1 waits 2/4s to retry
			// 2 runs 2s, succeeds
			// 3 starts
			await Task.Delay(2100);
			Assert.IsFalse(manualResetEvent1.WaitOne(100));
			Assert.IsTrue(manualResetEvent2.WaitOne(100));
			Assert.IsFalse(manualResetEvent3.WaitOne(100));

			// 4 submitted while 1 still waits for retry - 4 executes before 1
			var manualResetEvent4 = new ManualResetEvent(false);
			new ServiceCommandService(contextFactory, throttleInstanceFactory.Create()).AddCommand(command)
				.WithWorkload(new MockWorkload(manualResetEvent4, 0, 2000), new TimeSpan(0, 0, 0, 30))
				.Run<MockCommand>();

			// 1 waits 4/4s to retry, put back in queue
			// 3 runs 2s, succeeds
			// 4 starts
			await Task.Delay(2100);
			Assert.IsFalse(manualResetEvent1.WaitOne(100));
			Assert.IsTrue(manualResetEvent3.WaitOne(100));
			Assert.IsFalse(manualResetEvent4.WaitOne(100));

			// 1 waiting
			// 4 runs 2s, succeeds
			await Task.Delay(2100);
			Assert.IsFalse(manualResetEvent1.WaitOne(100));
			Assert.IsTrue(manualResetEvent4.WaitOne(100));

			// 1 runs 2s, succeeds
			await Task.Delay(2100);
			Assert.IsTrue(manualResetEvent1.WaitOne(100));
		}
	}
}
