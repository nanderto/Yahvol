namespace BrookfieldGrs.Web.Http.Tests.Mocks
{
    using System;
    using BrookfieldGrs.Data;
    using BrookfieldGrs.Services;
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