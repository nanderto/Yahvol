using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yahvol.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahvol.Services.Tests
{
    [TestClass]
    public class RetryPolicyTests
    {
        [TestMethod]
        public void TimeToWaitTest_DefaultPolicy()
        {
            var retyPolicy = new RetryPolicy();

            var timeToWait = retyPolicy.TimeToWait(1);
            Assert.AreEqual(TimeSpan.FromMilliseconds(2000), timeToWait);

            timeToWait = retyPolicy.TimeToWait(2);
            Assert.AreEqual(TimeSpan.FromMilliseconds(4000), timeToWait);

            timeToWait = retyPolicy.TimeToWait(3);
            Assert.AreEqual(TimeSpan.FromMilliseconds(8000), timeToWait);

            timeToWait = retyPolicy.TimeToWait(4);
            Assert.AreEqual(TimeSpan.FromMilliseconds(16000), timeToWait);

            timeToWait = retyPolicy.TimeToWait(5);
            Assert.AreEqual(TimeSpan.FromMilliseconds(32000), timeToWait);

            timeToWait = retyPolicy.TimeToWait(6);
            Assert.AreEqual(TimeSpan.MaxValue, timeToWait);
        }

        [TestMethod]
        public void TimeToWaitTest_ExponentialPolicy()
        {
            var retyPolicy = new RetryPolicy();
            retyPolicy.InitialBackOffPeriod = 10;
            retyPolicy.InitialDelay = 20;
            retyPolicy.BackOffStrategy = BackOffStrategy.Exponential;

            var timeToWait = retyPolicy.TimeToWait(1);
            Assert.AreEqual(TimeSpan.FromMilliseconds(10), timeToWait);

            timeToWait = retyPolicy.TimeToWait(2);
            Assert.AreEqual(TimeSpan.FromMilliseconds(400), timeToWait);

            timeToWait = retyPolicy.TimeToWait(3);
            Assert.AreEqual(TimeSpan.FromMilliseconds(8000), timeToWait);

            timeToWait = retyPolicy.TimeToWait(4);
            Assert.AreEqual(TimeSpan.FromMilliseconds(160000), timeToWait);

            timeToWait = retyPolicy.TimeToWait(5);
            Assert.AreEqual(TimeSpan.FromMilliseconds(3200000), timeToWait);

            timeToWait = retyPolicy.TimeToWait(6);
            Assert.AreEqual(TimeSpan.MaxValue, timeToWait);
        }

        [TestMethod]
        public void TimeToWaitTest_ExponentialPolicy_UsingMinutes()
        {
            var retyPolicy = new RetryPolicy();
            retyPolicy.InitialBackOffPeriod = 10;
            retyPolicy.InitialDelayInMinutes = 2;
            retyPolicy.BackOffStrategy = BackOffStrategy.Exponential;

            var timeToWait = retyPolicy.TimeToWait(1);
            Assert.AreEqual(TimeSpan.FromMilliseconds(10), timeToWait);

            timeToWait = retyPolicy.TimeToWait(2);
            Assert.AreEqual(TimeSpan.FromMinutes(4), timeToWait);

            timeToWait = retyPolicy.TimeToWait(3);
            Assert.AreEqual(TimeSpan.FromMinutes(8), timeToWait);

            timeToWait = retyPolicy.TimeToWait(4);
            Assert.AreEqual(TimeSpan.FromMinutes(16), timeToWait);

            timeToWait = retyPolicy.TimeToWait(5);
            Assert.AreEqual(TimeSpan.FromMinutes(32), timeToWait);

            timeToWait = retyPolicy.TimeToWait(6);
            Assert.AreEqual(TimeSpan.MaxValue, timeToWait);
        }

        [TestMethod]
        public void TimeToWaitTest_ExponentialPolicy_UsingConstructor_And_Minutes()
        {
            var retyPolicy = new RetryPolicy(10, DelayIntervalType.Minutes, 2, BackOffStrategy.Exponential);

            var timeToWait = retyPolicy.TimeToWait(1);
            Assert.AreEqual(TimeSpan.FromMilliseconds(10), timeToWait);

            timeToWait = retyPolicy.TimeToWait(2);
            Assert.AreEqual(TimeSpan.FromMinutes(4), timeToWait);

            timeToWait = retyPolicy.TimeToWait(3);
            Assert.AreEqual(TimeSpan.FromMinutes(8), timeToWait);

            timeToWait = retyPolicy.TimeToWait(4);
            Assert.AreEqual(TimeSpan.FromMinutes(16), timeToWait);

            timeToWait = retyPolicy.TimeToWait(5);
            Assert.AreEqual(TimeSpan.FromMinutes(32), timeToWait);

            timeToWait = retyPolicy.TimeToWait(6);
            Assert.AreEqual(TimeSpan.MaxValue, timeToWait);
        }

        [TestMethod]
        public void TimeToWaitTest_SquaredPolicy_20_20_3_Retry()
        {
            var retyPolicy = new RetryPolicy(20, 20, BackOffStrategy.Squared, 3);

            var timeToWait = retyPolicy.TimeToWait(1);
            Assert.AreEqual(TimeSpan.FromMilliseconds(20), timeToWait);

            timeToWait = retyPolicy.TimeToWait(2);
            Assert.AreEqual(TimeSpan.FromMilliseconds(40), timeToWait);

            timeToWait = retyPolicy.TimeToWait(3);
            Assert.AreEqual(TimeSpan.MaxValue, timeToWait);
        }

        [TestMethod]
        public void TimeToWaitTest_LinearPolicy_20_3mins_3_Retry()
        {
            var retyPolicy = new RetryPolicy(20, 20, BackOffStrategy.Linear, 3);
            retyPolicy.InitialDelayInMinutes = 3;

            var timeToWait = retyPolicy.TimeToWait(1);
            Assert.AreEqual(TimeSpan.FromMilliseconds(20), timeToWait);

            timeToWait = retyPolicy.TimeToWait(2);
            Assert.AreEqual(TimeSpan.FromMinutes(6), timeToWait);

            timeToWait = retyPolicy.TimeToWait(3);
            Assert.AreEqual(TimeSpan.MaxValue, timeToWait);
        }

        [TestMethod]
        public void TimeToWaitTest_SquaredPolicy_500_20_4_Retry()
        {
            var retyPolicy = new RetryPolicy(500, 20, BackOffStrategy.Squared, 4);

            var timeToWait = retyPolicy.TimeToWait(1);
            Assert.AreEqual(TimeSpan.FromMilliseconds(500), timeToWait);

            timeToWait = retyPolicy.TimeToWait(2);
            Assert.AreEqual(TimeSpan.FromMilliseconds(40), timeToWait);

            timeToWait = retyPolicy.TimeToWait(3);
            Assert.AreEqual(TimeSpan.FromMilliseconds(80), timeToWait);

            timeToWait = retyPolicy.TimeToWait(4);
            Assert.AreEqual(TimeSpan.MaxValue, timeToWait);
        }

        [TestMethod]
        public void TimeToWaitTest_ConstructedWithExemptExceptions()
        {
            var exemptExceptions = new List<Type>()
            {
                typeof(NullReferenceException),
                typeof(NotSupportedException)
            };

            var retyPolicy = new RetryPolicy(exemptExceptions);

            var timeToWait = retyPolicy.TimeToWait(1);
            Assert.AreEqual(TimeSpan.FromMilliseconds(2000), timeToWait);
        }

        [TestMethod]
        public void TimeToWaitTest_ConstructedWithOverrides()
        {
            var exemptExceptions = new List<Type>()
            {
                typeof(NullReferenceException),
                typeof(NotSupportedException)
            };

            var retyPolicy = new RetryPolicy(10, 10, BackOffStrategy.Linear, 5, exemptExceptions);

            var timeToWait = retyPolicy.TimeToWait(1);
            Assert.AreEqual(TimeSpan.FromMilliseconds(10), timeToWait);
            timeToWait = retyPolicy.TimeToWait(2);
            Assert.AreEqual(TimeSpan.FromMilliseconds(20), timeToWait);
            timeToWait = retyPolicy.TimeToWait(3);
            Assert.AreEqual(TimeSpan.FromMilliseconds(30), timeToWait);
            timeToWait = retyPolicy.TimeToWait(4);
            Assert.AreEqual(TimeSpan.FromMilliseconds(40), timeToWait);
        }

        [TestMethod]
        public void TimeToWaitTest_WithDontRetry()
        {

            var retyPolicy = new RetryPolicy(BackOffStrategy.DontRetry);

            var timeToWait = retyPolicy.TimeToWait(1);
            Assert.AreEqual(TimeSpan.MaxValue, timeToWait);
            timeToWait = retyPolicy.TimeToWait(2);
            Assert.AreEqual(TimeSpan.MaxValue, timeToWait);
            timeToWait = retyPolicy.TimeToWait(3);
            Assert.AreEqual(TimeSpan.MaxValue, timeToWait);
            timeToWait = retyPolicy.TimeToWait(4);
            Assert.AreEqual(TimeSpan.MaxValue, timeToWait);
        }
    }
}