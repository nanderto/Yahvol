﻿namespace BrookfieldGrs.Web.HttpTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using BrookfieldGrs.Services;
    using BrookfieldGrs.Web.Http;
    using Data;
    public class MockServiceCommandController : ServiceCommandApiController
    {
        public MockServiceCommandController(IInstanceFactory<IServiceCommandContext> serviceCommandInstanceFactory) : base(serviceCommandInstanceFactory)
        {
        }

        internal void SetupControllerRequest(string v)
        {
           // throw new NotImplementedException();
           // var controllerName = controller.GetType().Name.Replace("Controller", string.Empty);
           // controller.set
        }
    }
}
