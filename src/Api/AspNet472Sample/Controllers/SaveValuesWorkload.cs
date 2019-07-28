namespace AspNet472Sample.Controllers
{
    using Yahvol.Services;
    internal class SaveValuesWorkload : WorkloadBase<SaveValuesCommand>
    {
        public SaveValuesWorkload() : base (new RetryPolicy (BackOffStrategy.DontRetry))
        {
        }

        public SaveValuesWorkload(RetryPolicy retryPolicy) : base(retryPolicy)
        {
        }
    }
}