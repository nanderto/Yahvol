namespace Yahvol.Web.Http.Tests.Mocks
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Yahvol.Services;
    using Services.Commands;

    public class MockWorkload : WorkloadBase<MockTransfereeCommand>
    {
        private readonly ManualResetEvent manualResetEvent;

        private int numberofExceptionsToThrow;
        
        private readonly int timeToWait = 100;

        private int numberofExceptionsThrown;

        private TimeSpan timeToExpire;

        public MockWorkload(ManualResetEvent manualResetEvent)
            : base(default(RetryPolicy))
        {
            this.manualResetEvent = manualResetEvent;
        }

        //public MockWorkload(ManualResetEvent manualResetEvent, TimeSpan timeToExpire, int numberofExceptionsToThrow)
        //{
        //    // TODO: Complete member initialization
        //    this.manualResetEvent = manualResetEvent;
        //    this.timeToExpire = timeToExpire;
        //    this.numberofExceptionsToThrow = numberofExceptionsToThrow;
        //}

        public MockWorkload(ManualResetEvent manualResetEvent, RetryPolicy retryPolicy = default(RetryPolicy)) : base(retryPolicy)
        {
            this.manualResetEvent = manualResetEvent;
            this.HandleCommand("ThrowExceptionThenWait", this.ThrowExceptionThenWait);
            this.HandleCommand("WaitAndThrowException", this.WaitAndThrowException);
            this.HandleCommand("ThrowExceptionThenSucceed", this.ThrowExceptionThenSucceed);
        }

        public void WaitAndThrowException()
        {
            if (this.numberofExceptionsThrown < this.numberofExceptionsToThrow)
            {
                this.numberofExceptionsThrown++;
                Thread.Sleep(this.timeToWait);
                throw new Exception("Exception in MockWorkload.ThrowException");
            }

            Thread.Sleep(this.timeToWait);
            this.manualResetEvent.Set();
        }

        public void ThrowExceptionThenSucceed()
        {
            if (this.numberofExceptionsThrown < this.numberofExceptionsToThrow)
            {
                this.numberofExceptionsThrown++;
                throw new Exception("Exception in MockWorkload.ThrowException");
            }

            this.manualResetEvent.Set();
            this.LogMessage(string.Concat(this.numberofExceptionsThrown.ToString(CultureInfo.InvariantCulture), " exception(s) thrown. Completing command."));
        }
        public void ThrowExceptionThenWait()
        {
            if (this.numberofExceptionsThrown < this.numberofExceptionsToThrow)
            {
                this.numberofExceptionsThrown++;
                if (this.numberofExceptionsThrown == this.numberofExceptionsToThrow)
                {
                    this.manualResetEvent.Set();
                }

                throw new Exception("Exception in MockWorkload.ThrowException");
            }

            Thread.Sleep(this.timeToWait);
        }

        public MockWorkload(ManualResetEvent manualResetEvent, int numberofExceptionsToThrow, int timeToWait = 60000, RetryPolicy retryPolicy = default(RetryPolicy))
            : this(manualResetEvent, retryPolicy)
        {
            this.numberofExceptionsToThrow = numberofExceptionsToThrow;
            this.timeToWait = timeToWait;
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
