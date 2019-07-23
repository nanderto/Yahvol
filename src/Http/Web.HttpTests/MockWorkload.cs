namespace Yahvol.Web.HttpTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Yahvol.Services;

    public class MockWorkload : WorkloadBase<MockTransfereeCommand>
    {
        private readonly ManualResetEvent manualResetEvent;

        private int numberofExceptionsToThrow;
        
        private TimeSpan timeToExpire;
        
        public MockWorkload(ManualResetEvent manualResetEvent) : base (default(RetryPolicy))
        {
            this.manualResetEvent = manualResetEvent;
        }

        public MockWorkload(ManualResetEvent manualResetEvent, TimeSpan timeToExpire, int numberofExceptionsToThrow) : base(default(RetryPolicy))
        {
            // TODO: Complete member initialization
            this.manualResetEvent = manualResetEvent;
            this.timeToExpire = timeToExpire;
            this.numberofExceptionsToThrow = numberofExceptionsToThrow;
        }

        public bool Completed { get; set; }
        
        public override async Task<IWorkload> Execute(System.Threading.CancellationToken cancellationToken)
        {
            await Task.Delay(100);
            this.manualResetEvent.Set();
            // raise a domain event
            //this.
            return this;
        }
    }
}
