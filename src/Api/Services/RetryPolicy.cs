// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RetryPolicy.cs" company="Anderton Engineered Solutions">
// Copyright © 2014 All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Yahvol.Services
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Sets the back off strategy to be used
    /// </summary>
    public enum BackOffStrategy
    {
        /// <summary>
        /// backs off by the power of 2, the initial delay times the 2 *2 for each retry
        /// </summary>
        Squared,

        /// <summary>
        ///  backs off exponentially the initialDelay * initialDelay  * initialDelay  * ... n 
        /// </summary>
        Exponential,

        /// <summary>
        /// backs off linearly initialDelay + initialDelay + initialDelay + ... n 
        /// </summary>
        Linear,
        
        /// <summary>
        /// Will not retry.
        /// </summary>
        DontRetry
    }

    public enum DelayIntervalType
    {
        MilliSeconds,
        Minutes
    }

    public class RetryPolicy
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RetryPolicy"/> class. Creates a default policy using the Squared strategy and a 2000 milliseconds initial delay,
        ///     a 2000 milliseconds interval, and a maximum number of 6 attempts to re-try.
        ///     All values will be used unless over-written 
        /// </summary>
        public RetryPolicy()
        {
            this.BackOffStrategy = BackOffStrategy.Squared;
            this.ExemptionsFromRetryingExceptions = new List<Type>();
            this.InitialDelay = 2000;
            this.InitialBackOffPeriod = 2000;
            this.NumberOfReTrys = 6;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryPolicy"/> class that determines how an attached workload behaves when an exception is thrown
        /// Determines if it should be retried, and if so how often and what delay between re-trying.
        /// This constructor will use the default re-try policy values but allow you to specify exceptions which should be exempt from re-trying.
        /// You should exempt any exception that retying will not fix.
        /// </summary>
        /// <param name="exemptionsFromRetryingExceptions">List of exceptions that should not be retried</param>
        public RetryPolicy(IEnumerable<Type> exemptionsFromRetryingExceptions)
            : this()
        {
            this.ExemptionsFromRetryingExceptions = exemptionsFromRetryingExceptions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryPolicy"/> class that determines how an attached workload behaves when an exception is thrown
        /// Determines if it should be retried, and if so how often and what delay between retrying.
        /// This constructor allows you to set all of the values of the re-try policy.
        /// You should exempt any exception that retying will not fix.
        /// </summary>
        /// <param name="initialBackOffPeriod">Set the back off period for the first time that an exception is thrown</param>
        /// <param name="delayIntervalType">specify that the initial delay is in provided minutes or milliseconds, the default is in milliseconds</param>
        /// <param name="initialDelayInMinutes">Initial Delay period in Minutes. This is fed into the strategy to determine the time periods to back off retrying</param>
        /// <param name="backOffStrategy">Choose which strategy to apply</param>
        /// <param name="numberOfReTrys">Number of times to re-try on failure</param>
        /// <param name="exemptionsFromRetryingExceptions">List of exceptions that should not be retried</param>
        public RetryPolicy(int initialBackOffPeriod, DelayIntervalType delayIntervalType, int initialDelayInMinutes, BackOffStrategy backOffStrategy = BackOffStrategy.Squared, int numberOfReTrys = 6, IEnumerable<Type> exemptionsFromRetryingExceptions = default(IEnumerable<Type>))
            : this(exemptionsFromRetryingExceptions)
        {
            this.InitialBackOffPeriod = initialBackOffPeriod;
            this.InitialDelay = 0;
            this.InitialDelayInMinutes = initialDelayInMinutes;
            this.BackOffStrategy = backOffStrategy;
            this.NumberOfReTrys = numberOfReTrys;
            this.DelayIntervalType = delayIntervalType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryPolicy"/> class that determines how an attached workload behaves when an exception is thrown
        /// Determines if it should be retried, and if so how often and what delay between retrying.
        /// This constructor allows you to set all of the values of the re-try policy.
        /// You should exempt any exception that retying will not fix.
        /// </summary>
        /// <param name="initialBackOffPeriod">Set the back off period for the first time that an exception is thrown</param>
        /// <param name="initialDelay">Initial Delay period in Milliseconds. This is fed into the strategy to determine the time periods to back off retrying</param>
        /// <param name="backOffStrategy">Choose which strategy to apply</param>
        /// <param name="numberOfReTrys">Number of times to re-try on failure</param>
        /// <param name="exemptionsFromRetryingExceptions">List of exceptions that should not be retried</param>
        public RetryPolicy(int initialBackOffPeriod,  int initialDelay, BackOffStrategy backOffStrategy = BackOffStrategy.Squared, int numberOfReTrys = 6, IEnumerable<Type> exemptionsFromRetryingExceptions = default(IEnumerable<Type>))
            : this(exemptionsFromRetryingExceptions)
        {
            this.InitialBackOffPeriod = initialBackOffPeriod;
            this.InitialDelay = initialDelay;
            this.InitialDelayInMinutes = 0;
            this.BackOffStrategy = backOffStrategy;
            this.NumberOfReTrys = numberOfReTrys;
        }

        public RetryPolicy(BackOffStrategy backOffStrategy)
        {
            this.BackOffStrategy = backOffStrategy;
        }

        public BackOffStrategy BackOffStrategy { get; set; }

        public DelayIntervalType DelayIntervalType { get; set; }

        public IEnumerable<Type> ExemptionsFromRetryingExceptions { get; set; }
        
        public int InitialBackOffPeriod { get; set; }

        public int InitialDelay { get; set; }

        public int InitialDelayInMinutes { get; set; }

        public int NumberOfReTrys { get; set; }

        public TimeSpan TimeToWait(int retryCount)
        {
            var nmuberOfretrys = this.NumberOfReTrys;

            if (this.BackOffStrategy == BackOffStrategy.DontRetry)
            {
                nmuberOfretrys = 0; // make sure this is 0 and it will make sure that it sets timeToWait to max value
            }

            if (retryCount < nmuberOfretrys)
            {
				var timeToWait = 0;
                TimeSpan timeToWaitTimeSpan;

                if (retryCount == 1)
                {
                    timeToWait = this.InitialBackOffPeriod;
                    timeToWaitTimeSpan = TimeSpan.FromMilliseconds(timeToWait);
                    return timeToWaitTimeSpan;
                }

                var initialDelay = this.InitialDelay;
                if (this.InitialDelayInMinutes != 0)
                {
                    initialDelay = this.InitialDelayInMinutes;
                }

                switch (this.BackOffStrategy)
                {
                    case BackOffStrategy.Squared:
                        var numberOfIntervalsToWait = (int)Math.Pow(2, retryCount - 1);
                        timeToWait = initialDelay * numberOfIntervalsToWait;
                        break;
                    case BackOffStrategy.Linear:
                        timeToWait = initialDelay * retryCount;
                        break;
                    case BackOffStrategy.Exponential:
                        timeToWait = (int)Math.Pow(initialDelay, retryCount);
                        break;
                }

                if (this.InitialDelayInMinutes != 0)
                {
                    timeToWaitTimeSpan = TimeSpan.FromMinutes(timeToWait);
                }
                else
                {
                    timeToWaitTimeSpan = TimeSpan.FromMilliseconds(timeToWait);
                }

                return timeToWaitTimeSpan;
            }

            return TimeSpan.MaxValue;
        }
    }
}