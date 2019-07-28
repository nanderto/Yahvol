using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Yahvol.Services;
using Yahvol.Web.Http;

namespace AspNet472Sample.Controllers
{
    public class ValuesController : Yahvol.Web.Http.ServiceCommandApiController
    {
        internal ServiceCommandLogger logger = null;

        internal ServiceCommandContext ctx = null;

        internal ServiceCommandLoggerInstanceFactory serviceCommandLoggerInstanceFactory = null;

        internal ServiceCommandContextInstanceFactory ctxFactory = null;

        public ValuesController(ServiceCommandContextInstanceFactory factory, ServiceCommandLoggerInstanceFactory loggerFactory)
            : base (factory)
        {
            logger = loggerFactory.Create();
            ctx = factory.Create();
            ctxFactory = factory;
            serviceCommandLoggerInstanceFactory = loggerFactory;
        }

        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
            var serviceCommandService = new ServiceCommandService(ctxFactory);
            var transfereeCommand = serviceCommandService.AddCommand<SaveValuesCommand>(new SaveValuesCommand());
            transfereeCommand.WithWorkload(SaveValuesWorkload(), new TimeSpan(0, 0, 30))
            .WithWorkload(new DoSomethingElseValuesWorkload(), new TimeSpan(0, 0, 45))
            .WithWorkload(new DoSomethingElseValuesWorkload(new RetryPolicy(20, 50, BackOffStrategy.Exponential, 3)), new TimeSpan(0, 0, 45))
            .Run<SaveValuesCommand>();


            new ServiceCommandService(contextFactory, throttleInstanceFactory.Create()).AddCommand(command)
                .WithWorkload(new MockWorkload(manualResetEvent1, 0, 2000), new TimeSpan(0, 0, 0, 30))
                .Run<MockCommand>();
        }

        private IWorkload SaveValuesWorkload()
        {
            var wl = new SaveValuesWorkload(new RetryPolicy(20,50,BackOffStrategy.Exponential,3));
            return wl;
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
