namespace Yahvol.Services.Tests.Mocks
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Yahvol.Services;

    public class MockRestartSubscriber : Subscriber
    {
        private readonly object syncLock = new object();
        
        private int counter;

        public MockRestartSubscriber(System.Threading.ManualResetEvent manualResetEvent, TimeSpan timeToExpire, int numberofExceptionsToThrow, TimeSpan timeToDelay)
        {
            this.TimeToDelay = timeToDelay; 
            this.ManualResetEvent = manualResetEvent;
            this.TimeToExpire = timeToExpire;
            this.NumberofExceptionsToThrow = numberofExceptionsToThrow;
            this.Workload = new MockRestarterWorkload(this.ManualResetEvent, timeToExpire, numberofExceptionsToThrow, this);
        }

        public MockRestartSubscriber(System.Threading.ManualResetEvent manualResetEvent, TimeSpan timeToExpire, int numberofExceptionsToThrow, TimeSpan timeToDelay, Type exceptionToThrow)
        {
            this.TimeToDelay = timeToDelay;
            this.ManualResetEvent = manualResetEvent;
            this.TimeToExpire = timeToExpire;
            this.NumberofExceptionsToThrow = numberofExceptionsToThrow;
            this.ExceptonToThrow = exceptionToThrow;
            this.Workload = new MockRestarterWorkload(this.ManualResetEvent, timeToExpire, numberofExceptionsToThrow, this, exceptionToThrow);
        }

        public MockRestartSubscriber(MockRestarterWorkload mockRestarterWorkload)
        {
            this.Workload = mockRestarterWorkload;
            this.TimeToDelay = mockRestarterWorkload.TimeToDelay;
        }

        public Type ExceptonToThrow { get; set; }

        public Action Action { get; set; }

        public ManualResetEvent ManualResetEvent { get; set; }

        public int NumberofExceptionsToThrow { get; set; }
        
        public TimeSpan TimeToDelay { get; set; }

        public void Add()
        {
            lock (this.syncLock)
            {
                this.counter = ++this.counter;
            }
        }

        public int Count()
        {
            lock (this.syncLock)
            {
                return this.counter;
            }
        }
    }
}