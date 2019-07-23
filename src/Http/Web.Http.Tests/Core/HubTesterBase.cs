using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace Yahvol.Web.Http.Tests.Core
{
    //[TestClass()]
    public class HubTesterBase 
    {
        public static ManualResetEvent manualResetEvent = new ManualResetEvent(false);
        public static ManualResetEvent manualResetEvent2 = new ManualResetEvent(false);
        
        [TestInitialize]
        public void Setup()
        {

            var r = new Runner();
            var result = Task.Run(async () =>
            {
                return r.Run();
            });
            HubTesterBase.manualResetEvent2.WaitOne();
        }

        public class Runner
        {
            public async Task Run()
            {
                string url = "http://localhost:8081";
                try
                {
                    using (WebApp.Start(url))
                    {
                        HubTesterBase.manualResetEvent2.Set();
                        HubTesterBase.manualResetEvent.WaitOne();
                    }
                }

                catch(Exception ex)
                {
                    var exp = ex;
                    HubTesterBase.manualResetEvent2.Set();
                    throw ex;
                }
            }
        }

        [TestCleanup]
        public void TearDown()
        {
            HubTesterBase.manualResetEvent.Set();
        }
    }
}
