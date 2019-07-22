using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Yahvol.Services;

namespace AspNet472Sample.Controllers
{
    public class ValuesController : Yahvol.Services. ApiController
    {
        internal ServiceCommandLogger logger = null;
        internal ServiceCommandContext ctx = null;
        public ValuesController(ServiceCommandContextInstanceFactory factory, ServiceCommandLoggerInstanceFactory loggerFactory)
        {
            logger = loggerFactory.Create();
            ctx = factory.Create();

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
            logger.Log("Posting from Values controller");
            ctx.
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
