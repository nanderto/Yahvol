namespace Yahvol.Web.Http.Tests.Mocks
{
    using System;
    using Yahvol.Data;
    using Yahvol.Services;
    public class MockServiceCommandController : ServiceCommandApiController
    {
        public MockServiceCommandController(IInstanceFactory<IServiceCommandService> serviceCommandInstanceFactory)
            : base(serviceCommandInstanceFactory.Create())
        {
        }

        internal void SetupControllerRequest(string v)
        {
            //throw new NotImplementedException();
        }
    }
}