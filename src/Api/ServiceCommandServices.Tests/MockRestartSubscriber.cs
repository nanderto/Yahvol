namespace TransfereeService.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using BrookfieldGrs.Services;

    public class MockRestartSubscriber : Subscriber
    {
        public int Counter;

        private object SyncLock = new object();
        
        private TimeSpan timeSpan;
        
        private int p;
        
        public int Id { get; set; }

        public int NumberofExceptionsToThrow { get; set; }
        
        public Action Action { get; set; }
        
        public TimeSpan TimeToExpire { get; set; }

        public MockRestartSubscriber()
        {
            this.TimeToDelay = new TimeSpan(0, 0, 0, 0, 10);
        }

        public MockRestartSubscriber(ManualResetEvent manualResetEvent)
        {
            this.ManualResetEvent = manualResetEvent;
            base.TimeToExpire = new TimeSpan(0, 0, 60, 0);
            base.Workload = new mockWorkload(this.ManualResetEvent);
        }

        public MockRestartSubscriber(System.Threading.ManualResetEvent manualResetEvent, TimeSpan timeToExpire, int numberofExceptionsToThrow, TimeSpan timeToDelay)
        {
            this.TimeToDelay = timeToDelay;// TODO: Complete member initialization
            this.ManualResetEvent = manualResetEvent;
            base.TimeToExpire = timeToExpire;
            this.NumberofExceptionsToThrow = numberofExceptionsToThrow;
            base.Workload = new MockRestarterWorkload(this.ManualResetEvent, timeToExpire, numberofExceptionsToThrow, this);
        }

        public void Add()
        {
            lock (this.SyncLock)
            {
                this.Counter = ++this.Counter;
            }
        }

        public int Count()
        {
            lock (this.SyncLock)
            {
                return this.Counter;
            }
        }

        public bool Completed { get; set; }

        public ManualResetEvent ManualResetEvent { get; set; }
        
        public TimeSpan TimeToDelay { get; set; }


        public Task<Subscriber> Finished()
        {
            throw new NotImplementedException();
        }
    }
}