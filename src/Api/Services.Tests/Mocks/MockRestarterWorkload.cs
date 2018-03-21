namespace Yahvol.Services.Tests.Mocks
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using Yahvol.Services;

    public class MockRestarterWorkload : WorkloadBase<MockCommand>
    {
        private readonly ManualResetEvent manualResetEvent;
        
        private readonly object syncLock = new object();
        
        private int counter;
        
        private TimeSpan timeToExpire;

        public MockRestarterWorkload(ManualResetEvent manualResetEvent, TimeSpan timeToExpire, int numberofExceptionsToThrow, MockRestartSubscriber subscriber, RetryPolicy retryPolicy = default(RetryPolicy)) : base(retryPolicy)
        {
            //this.Subscriber = subscriber;
            this.TimeToDelay = subscriber.TimeToDelay;
            this.manualResetEvent = manualResetEvent;
            this.timeToExpire = timeToExpire;
            this.NumberofExceptionsToThrow = numberofExceptionsToThrow;
        }

        public MockRestarterWorkload(ManualResetEvent manualResetEvent, TimeSpan timeToExpire, int numberofExceptionsToThrow, MockRestartSubscriber subscriber, Type exceptionToThrow, RetryPolicy retryPolicy = default(RetryPolicy)) : base(retryPolicy)
        {
            this.Subscriber = subscriber;
            this.manualResetEvent = manualResetEvent;
            this.timeToExpire = timeToExpire;
            NumberofExceptionsToThrow = numberofExceptionsToThrow;
            ExceptionToThrow = exceptionToThrow;
            this.ExceptionToThrow = exceptionToThrow;
        }

        public MockRestarterWorkload(ManualResetEvent manualResetEvent, TimeSpan timeToExpire, int numberofExceptionsToThrow, Type exceptionToThrow, TimeSpan timeToDelay, RetryPolicy retryPolicy = default(RetryPolicy)) : base(retryPolicy)
        {
            this.manualResetEvent = manualResetEvent;
            this.timeToExpire = timeToExpire;
            this.NumberofExceptionsToThrow = numberofExceptionsToThrow;
            this.ExceptionToThrow = exceptionToThrow;
            this.TimeToDelay = timeToDelay;
        }

        public MockRestarterWorkload(ManualResetEvent manualResetEvent, TimeSpan timeToExpire, int numberofExceptionsToThrow, Type exceptionToThrow, RetryPolicy retryPolicy = default(RetryPolicy)) : base(retryPolicy)
        {
            this.manualResetEvent = manualResetEvent;
            this.timeToExpire = timeToExpire;
            this.NumberofExceptionsToThrow = numberofExceptionsToThrow;
            this.ExceptionToThrow = exceptionToThrow;
        }

        public MockRestarterWorkload(ManualResetEvent manualResetEvent, int numberofExceptionsToThrow, Type exceptionToThrow, RetryPolicy retryPolicy = default(RetryPolicy)) : base(retryPolicy)
        {
            this.manualResetEvent = manualResetEvent;
            this.NumberofExceptionsToThrow = numberofExceptionsToThrow;
            this.ExceptionToThrow = exceptionToThrow;
        }

        public MockRestarterWorkload(RetryPolicy retryPolicy) : base(retryPolicy)
        {
        }

        public bool Completed { get; set; }

        public int Id { get; set; }

        public Type ExceptionToThrow { get; set; }

        public int NumberofExceptionsToThrow { get; set; }
        
        public TimeSpan TimeToDelay { get; set; }

        public int Count()
        {
            lock (this.syncLock)
            {
                return this.counter;
            }
        }

        public override async Task<IWorkload> Execute(System.Threading.CancellationToken cancellationToken)
        {
            this.Add();

            cancellationToken.ThrowIfCancellationRequested();

            var count = this.Count();
            if (this.ExceptionToThrow != null)
            {
                if (count <= this.NumberofExceptionsToThrow)
                {
                    if (count == this.RetryPolicy.NumberOfReTrys)
                    {
                        this.manualResetEvent.Set(); //the policy is not gooing to handle any more exceptions so we should tell the other thread that we are done.
                    }

                    ThrowRequestedException(this.ExceptionToThrow.Name);
                }
                else
                {
                    this.manualResetEvent.Set();
                }
            }

            cancellationToken.ThrowIfCancellationRequested();
            if (count < 3)
            {
                await Task.Delay(this.TimeToDelay);
            }

            cancellationToken.ThrowIfCancellationRequested();
            this.Id = 1;
            this.manualResetEvent.Set();
            return this;
        }

        private void ThrowRequestedException(string name)
        {
            if (this.RetryPolicy.ExemptionsFromRetryingExceptions.Any(e => e.Name == name))
            {
                this.manualResetEvent.Set();
            }

            switch (name)
            {
                case "ArgumentException":
                    throw new ArgumentException("ArgumentException");

                case "NullReferenceException":
                    throw new NullReferenceException("NullReferenceException");

                case "InvalidOperationException":
                    throw new InvalidOperationException("InvalidOperationException");
                    
                case "ArgumentOutOfRangeException":
                    throw new InvalidOperationException("ArgumentOutOfRangeException");

                default:
                    throw new Exception();
            }
        }

        private void Add()
        {
            lock (this.syncLock)
            {
                this.counter = ++this.counter;
               // this.Subscriber.Add();
            }
        }
    }
}