namespace Yahvol.Web.Http.Tests.Mocks
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Yahvol.Services;

    public class MockSubscriber : Subscriber
    {
        public MockSubscriber()
        {
            // this.Action = () => this.ManualResetEvent.Reset();
            this.TimeToExpire = new TimeSpan(0, 0, 60, 0);
            this.Workload = new MockWorkload(this.ManualResetEvent);
        }

        public MockSubscriber(System.Threading.ManualResetEvent manualResetEvent)
        {
            // TODO: Complete member initialization
            this.ManualResetEvent = manualResetEvent;
            this.TimeToExpire = new TimeSpan(0, 0, 60, 0);
            this.Workload = new MockWorkload(this.ManualResetEvent);
        }

        public Action Action { get; set; }

        public new int Id { get; set; }
        
        public ManualResetEvent ManualResetEvent { get; set; }

        public new TimeSpan TimeToExpire { get; set; }
        
        public async Task Execute()
        {
            await Task.Delay(1000);
            //// var result = await Task.Run(() => this.ManualResetEvent.Reset());
            this.ManualResetEvent.Set();
        }

        public new async Task<Subscriber> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(100);
            this.ManualResetEvent.Set();
            return this;
        }
    }
}