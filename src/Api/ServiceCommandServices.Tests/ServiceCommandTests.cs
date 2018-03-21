using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TransfereeService.Tests
{
    using BrookfieldGrs.Services;
    using BrookfieldGrs.ServiceEngine.Services;
    using System.Data.SqlClient;
    using System.Configuration;

    [TestClass]
    public class ServiceCommandTests
    {
        public SqlConnection Connection { get; set; }

        private SqlCommand Command(string addCommand, SqlConnection connection)
        {
            return new SqlCommand(addCommand, connection);
        }

        public string GetConnectionString()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ServiceCommandContext"].ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString =
                    ConfigurationManager.ConnectionStrings[this.GetType().Name.Replace("Context", "")].ConnectionString;
            }
            return connectionString;
        }


        [TestMethod]
        public void AddCommand()
        {
            var mockServiceCommandContext = new MockServiceCommandContext();
            var serviceCommandRepository = new ServiceCommandRepository(new MockServiceCommandContext());
            var serviceCommandService = new ServiceCommandService(serviceCommandRepository);
            var transfereeCommand = serviceCommandService.Add<TransfereeCommand>(new TransfereeCommand());

            Assert.IsInstanceOfType(transfereeCommand, typeof(TransfereeCommand));
        }

        [TestMethod]
        public void AddCommand_WithSubscriber()
        {
            var mockServiceCommandContext = new MockServiceCommandContext();
            var serviceCommandRepository = new ServiceCommandRepository(mockServiceCommandContext);
            var serviceCommandService = new ServiceCommandService(serviceCommandRepository);
            var mocSubscriber = new MockSubscriber();
            serviceCommandService.Subscribers.Add(mocSubscriber);
            var transfereeCommand = serviceCommandService.Add<TransfereeCommand>(new TransfereeCommand());

            Assert.IsInstanceOfType(transfereeCommand, typeof(TransfereeCommand));
            Assert.AreEqual(mockServiceCommandContext.ServiceCommands.Count(), 1);

            Assert.AreEqual(mockServiceCommandContext.ServiceCommands.First().Subscribers.Count(), 1);
        }

        [TestMethod]
        public void TaskRunner_Run()
        {
            var manualResetEvent = new ManualResetEvent(false);
            var ex = default(Exception);
            var subscribers = new List<Subscriber>();
            var mocSubscriber = new MockSubscriber(manualResetEvent);
            subscribers.Add(mocSubscriber);
            var taskrunner = new AsyncTaskRunner(subscribers);
            taskrunner.StartTasks();
            Assert.IsTrue(manualResetEvent.WaitOne(1000000));
        }

        [TestMethod]
        public void TaskRunner_Run2Subscribers()
        {
            var manualResetEvent = new ManualResetEvent(false);
            var manualResetEvent2 = new ManualResetEvent(false);
            //var ex = default(Exception);
            var subscribers = new List<Subscriber>();
            var mocSubscriber = new MockSubscriber(manualResetEvent);
            var mocSubscriber2 = new MockSubscriber(manualResetEvent2);
            subscribers.Add(mocSubscriber);
            subscribers.Add(mocSubscriber2);
            var taskrunner = new AsyncTaskRunner(subscribers);
            taskrunner.StartTasks();
            Assert.IsTrue(manualResetEvent.WaitOne(1000));
            Assert.IsTrue(manualResetEvent2.WaitOne(1000));
        }

        [TestMethod]
        public void TaskRunner_Run2Subscribers_By_addingCommand()
        {
            var manualResetEvent = new ManualResetEvent(false);
            var manualResetEvent2 = new ManualResetEvent(false);

            var subscribers = new List<Subscriber>();
            var mocSubscriber = new MockSubscriber(manualResetEvent);
            var mocSubscriber2 = new MockSubscriber(manualResetEvent2);
            subscribers.Add(mocSubscriber);
            subscribers.Add(mocSubscriber2);

            var transferee = new Transferee
            {
                FirstName = "John",
                LastName = "wilson"
            };

            var startTime = DateTime.Now;
            var transfereeCommand = new TransfereeCommand
            {
                CommandType = CommandType.Add,
                CreateDate = startTime,
                CreatedBy = Thread.CurrentPrincipal.Identity.Name,
                GrsSystemType = GrsSystemType.Gsp,
                Transferee = transferee
            };

            IServiceCommandService serviceCommandService = new ServiceCommandService
            {
                ServiceCommandRepository = new ServiceCommandRepository(new MockServiceCommandContext()),
                Subscribers = subscribers
            };
                //.AddSubscriberOfType(typeof(MockSubscriber))
                //.WithTimeToExpire;
           // serviceCommandService.AddSubscriberOfType
            var command = serviceCommandService.Add(transfereeCommand);
            Assert.IsTrue(command.Id == 1);
            Assert.IsTrue(manualResetEvent.WaitOne(1000));
            Assert.IsTrue(manualResetEvent2.WaitOne(1000));

        }

        [TestMethod]
        public async Task SubscriberFinished()
        {
            int commandId = 0;
            int subscriberId = 0;
            using (var setupConnection = new SqlConnection(this.GetConnectionString()))
            {
                SqlCommand command = this.Command("select top 1 sc.Id, s.Id as subscriberId from ServiceCommands sc inner join Subscribers s on sc.Id = s.ServiceCommandId", setupConnection);
                setupConnection.Open();
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        commandId = (int)reader[0];
                        subscriberId = (int)reader[1];
                    }
                }
                else
                {
                    //var approvr = new ApproverUser { Source = "OE", UserID = "test4", TS = new byte[0] };
                    //var r = new Repository<ApproverUser>(new DbContext("name=InitiationEntities"));
                    //ApproverUser approvrUser = r.Add(approvr);
                    //approverUserID = approvrUser.ApproverUserID;
                }
                reader.Close();
            }

            var subscriber = new Subscriber { Id = subscriberId };
            subscriber.ServiceCommandId = commandId;
            subscriber.ServiceCommand = new ServiceCommand() { Completed = true, CompletedDate = DateTime.Now, Id = commandId };
            var serviceCommandContext = new ServiceCommandContext();
            subscriber.ServiceCommandRepository = new ServiceCommandRepository(serviceCommandContext);
            await subscriber.Finished();

            Assert.IsTrue(serviceCommandContext == serviceCommandContext);
        }

        [TestMethod]
        public async Task Restarter_OneRestart()
        {
            var manualResetEvent = new ManualResetEvent(false);
            var mocSubscriber = new MockSubscriber(manualResetEvent)
            {
                Completed = false,
                TimeToExpire = new TimeSpan(0, 0, 0, 0, 100)
            };
            
            var reStarter = new ReStarter(mocSubscriber);
            await reStarter.StartAsync();
            Assert.IsTrue(manualResetEvent.WaitOne(10000));
        }


        [TestMethod]
        public async Task Restarter_Restart_3times()
        {
            var manualResetEvent = new ManualResetEvent(false);
            var mocSubscriber = new MockRestartSubscriber(manualResetEvent, new TimeSpan(0, 0, 10, 100, 0), 3,  new TimeSpan(0, 0, 0, 0, 10));


            var reStarter = new ReStarter(mocSubscriber);
            await reStarter.StartAsync();
            mocSubscriber.ManualResetEvent.WaitOne(10000);
            var count = mocSubscriber.Count();
            Assert.AreEqual(3, count);
        }

        [TestMethod]
        public async Task Restarter_Restart_3times_because_of_cancellation()
        {
            //make sure you understand your set up Shorter timetoExpire than TimeToDelay will ensure that the task is cancelled
            //this Mockrestarter is set to only throw 3 cancelled exceptions, It will not delay after 3 cancellation exceptions
            //so the fourth time thru the thread wil not expire because of time. This will allow the task to finish
            var manualResetEvent = new ManualResetEvent(false);
            TimeSpan timeToExpire = new TimeSpan(0, 0, 0, 0, 20);
            TimeSpan timeToDelay = new TimeSpan(0, 0, 0, 1, 0);
            var mocSubscriber = new MockRestartSubscriber(manualResetEvent, timeToExpire, 0, timeToDelay)
            {
                Completed = false
            };

            var reStarter = new ReStarter(mocSubscriber);
            await reStarter.StartAsync();
            mocSubscriber.ManualResetEvent.WaitOne(100000);
            var count = mocSubscriber.Count();
            Assert.AreEqual(3, reStarter.RetryCount);
        }

        [TestMethod]
        public async Task Restarter_Restart_2times()
        {
            var manualResetEvent = new ManualResetEvent(false);
            var mocSubscriber = new MockRestartSubscriber(manualResetEvent, new TimeSpan(0, 0, 0, 0, 1000), 2, new TimeSpan(0, 0, 0, 0, 10))
            {
                Completed = false
            };

            var reStarter = new ReStarter(mocSubscriber);
            await reStarter.StartAsync();
            mocSubscriber.ManualResetEvent.WaitOne(10000);
            var count = mocSubscriber.Count();
            Assert.AreEqual(2, count);
        }
    }
}
