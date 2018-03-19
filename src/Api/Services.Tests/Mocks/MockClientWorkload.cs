namespace Yahvol.Services.Tests.Mocks
{
    using System.Threading;
    using System.Threading.Tasks;
    using Yahvol.Services.Commands.AD;

    public class MockClientWorkload : WorkloadBase<ClientCommand>
    {
        private readonly ManualResetEvent manualResetEvent;

        public MockClientWorkload(ManualResetEvent manualResetEvent, RetryPolicy retryPolicy = default(RetryPolicy)) : base(retryPolicy)
        {
            this.manualResetEvent = manualResetEvent;
        }

        public override async void DefaultAction()
        {
            await Task.Delay(100);
            this.manualResetEvent.Set();
        }
    }
}
