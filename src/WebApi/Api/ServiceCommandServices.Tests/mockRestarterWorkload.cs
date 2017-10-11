namespace TransfereeService.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using BrookfieldGrs.Services;

    public class MockRestarterWorkload : IWorkload
    {
        public int Counter;

        private ManualResetEvent manualResetEvent;

        private object SyncLock = new object();

        private TimeSpan timeToExpire;

        public MockRestarterWorkload(ManualResetEvent manualResetEvent, TimeSpan timeToExpire, int numberofExceptionsToThrow, MockRestartSubscriber subscriber)
        {
            this.Subscriber = subscriber;
            this.TimeToDelay = subscriber.TimeToDelay;
            this.manualResetEvent = manualResetEvent;
            this.timeToExpire = timeToExpire;
            this.NumberofExceptionsToThrow = numberofExceptionsToThrow;
        }

        public bool Completed { get; set; }

        public int Id { get; set; }

        public int NumberofExceptionsToThrow { get; set; }

        public MockRestartSubscriber Subscriber { get; set; }

        public TimeSpan TimeToDelay { get; set; }

        public void Add<T>(T command) where T : ICommand
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            lock (this.SyncLock)
            {
                return this.Counter;
            }
        }

        public async Task<IWorkload> Execute(System.Threading.CancellationToken cancellationToken)
        {
            this.Add();

            cancellationToken.ThrowIfCancellationRequested();

            if (this.Count() < this.NumberofExceptionsToThrow)
            {
                throw new Exception();
            }
            cancellationToken.ThrowIfCancellationRequested();
            if (this.Count() < 3)
            {
                await Task.Delay(this.TimeToDelay);
            }
            cancellationToken.ThrowIfCancellationRequested();
            this.Id = 1;
            this.manualResetEvent.Set();
            return this;
        }

        private void Add()
        {
            lock (this.SyncLock)
            {
                this.Counter = ++this.Counter;
                this.Subscriber.Add();
            }
        }
    }
}