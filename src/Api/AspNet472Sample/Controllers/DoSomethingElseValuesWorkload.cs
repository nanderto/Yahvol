using Yahvol.Services;

namespace AspNet472Sample.Controllers
{
    internal class DoSomethingElseValuesWorkload : WorkloadBase<SaveValuesCommand>
    {
        private RetryPolicy retryPolicy;

        public DoSomethingElseValuesWorkload(): base (new RetryPolicy(BackOffStrategy.DontRetry))
        {
        }

        public DoSomethingElseValuesWorkload(RetryPolicy retryPolicy) : base(retryPolicy)
        {
        }
    }
}