namespace TransfereeService.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using BrookfieldGrs.Services;

    public class MockSubscriber : Subscriber
    {
        public int Id { get; set; }
        public Action Action { get; set; }
        public new TimeSpan TimeToExpire { get; set; }
        public ManualResetEvent ManualResetEvent { get; set; }
        public bool Completed { get; set; }
        public Task<Subscriber> Finished()
        {
            throw new NotImplementedException();
        }

        public MockSubscriber()
        {
            //this.Action = () => this.ManualResetEvent.Reset();
            base.TimeToExpire = new TimeSpan(0, 0, 60, 0);
            base.Workload = new mockWorkload(this.ManualResetEvent);
        }

        public MockSubscriber(System.Threading.ManualResetEvent manualResetEvent)
        {
            // TODO: Complete member initialization
            this.ManualResetEvent = manualResetEvent;
            base.TimeToExpire = new TimeSpan(0, 0, 60, 0);
            base.Workload = new mockWorkload(this.ManualResetEvent);
        }
        public async Task Execute()
        {
            await Task.Delay(1000);
            //var result = await Task.Run(() => this.ManualResetEvent.Reset());
            this.ManualResetEvent.Set();
        }

        public async Task<Subscriber> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100);
            this.ManualResetEvent.Set();
            return this;
        }
    }
}