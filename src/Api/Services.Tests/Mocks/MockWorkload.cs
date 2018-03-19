namespace Yahvol.Services.Tests.Mocks
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Yahvol.Services;

    public class MockWorkload : WorkloadBase<MockCommand>
    {
        private readonly ManualResetEvent manualResetEvent;

        private readonly int numberofExceptionsToThrow = 6;

		private readonly int timeToWait = 100;

		private int numberofExceptionsThrown;

        private RetryPolicy retryPolicy;

        public MockWorkload(ManualResetEvent manualResetEvent, RetryPolicy retryPolicy = default(RetryPolicy)) : base(retryPolicy)
        {
            this.manualResetEvent = manualResetEvent;
            this.HandleCommand("ThrowExceptionThenWait", this.ThrowExceptionThenWait);
			this.HandleCommand("WaitAndThrowException", this.WaitAndThrowException);
            this.HandleCommand("ThrowExceptionThenSucceed", this.ThrowExceptionThenSucceed);
        }

		public MockWorkload(ManualResetEvent manualResetEvent, int numberofExceptionsToThrow, int timeToWait = 60000, RetryPolicy retryPolicy = default(RetryPolicy))
            : this(manualResetEvent, retryPolicy)
        {
            this.numberofExceptionsToThrow = numberofExceptionsToThrow;
	        this.timeToWait = timeToWait;
        }

        public MockWorkload(ManualResetEvent manualResetEvent, int numberofExceptionsToThrow, RetryPolicy retryPolicy) : base(retryPolicy)
        {
            this.manualResetEvent = manualResetEvent;
            this.numberofExceptionsToThrow = numberofExceptionsToThrow;
            this.retryPolicy = retryPolicy;
        }

        public bool Completed { get; set; }

        public override void DefaultAction()
        {
			Thread.Sleep(this.timeToWait);
			this.manualResetEvent.Set();
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
    }
}
