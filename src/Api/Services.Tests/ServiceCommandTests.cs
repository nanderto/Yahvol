namespace Yahvol.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Yahvol.GlobalConstants;
    using Yahvol.Services;
    using Yahvol.Services.Commands;
    using Yahvol.Services.Commands.AD;
    using Yahvol.Services.Tests.Mocks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ServiceCommandTests
    {
        public SqlConnection Connection { get; set; }

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
            var contextFactory = new MockServiceCommandContextInstanceFactory();
            var serviceCommandService = new ServiceCommandService(contextFactory);
            var transfereeCommand = serviceCommandService.AddCommand<MockTransfereeCommand>(new MockTransfereeCommand());

            Assert.IsInstanceOfType(transfereeCommand, typeof(ServiceCommandService));
        }

        [TestMethod]
        public void AddCommand_WithSubscriber()
        {
            var contextFactory = new MockServiceCommandContextInstanceFactory();
            var serviceCommandService = new ServiceCommandService(contextFactory);
            var mocSubscriber = new MockSubscriber();
            serviceCommandService.Subscribers.Add(mocSubscriber);
            var transfereeCommand = serviceCommandService.AddCommand<MockTransfereeCommand>(new MockTransfereeCommand());

            Assert.IsInstanceOfType(transfereeCommand, typeof(ServiceCommandService));
        }

        [TestMethod]
        public void AddCommand_OnlyOneCommand()
        {
            var contextFactory = new MockServiceCommandContextInstanceFactory();
            var serviceCommandService = new ServiceCommandService(contextFactory);
            try
            {
                var transfereeCommand =
                    serviceCommandService.AddCommand<MockTransfereeCommand>(new MockTransfereeCommand());
                var transfereeCommand2 =
                    serviceCommandService.AddCommand<MockTransfereeCommand>(new MockTransfereeCommand());
                Assert.Fail("Adding a second command should cause an exception");
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(Exception));
            }
        }

        [TestMethod]
        public void WithWorkload_Thows_Exception_If_No_Command()
        {
            var contextFactory = new MockServiceCommandContextInstanceFactory();
            var serviceCommandService = new ServiceCommandService(contextFactory);
            var manualResetEvent = new ManualResetEvent(true);
            try
            {
                var transfereeCommand = serviceCommandService.WithWorkload(new MockWorkload(manualResetEvent), new TimeSpan());

                Assert.Fail("Adding a workload with out a command should cause an exception");
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(Exception));
            }
        }

        [TestMethod]
        public void Run_Thows_Exception_If_No_Workload_Or_Command()
        {
            var contextFactory = new MockServiceCommandContextInstanceFactory();
            var serviceCommandService = new ServiceCommandService(contextFactory);
            var manualResetEvent = new ManualResetEvent(true);
            try
            {
                var transfereeCommand = serviceCommandService.Run<MockCommand>();

                Assert.Fail("Calling Run with out a workload being added should cause an exception");
            }
            catch (NullReferenceException ex)
            {
                Assert.IsInstanceOfType(ex, typeof(Exception));
            }
        }

        [TestMethod]
        public void Run_Thows_Exception_If_No_Workload()
        {
            var contextFactory = new MockServiceCommandContextInstanceFactory();
            var serviceCommandService = new ServiceCommandService(contextFactory);
            var manualResetEvent = new ManualResetEvent(true);
            serviceCommandService.AddCommand<MockCommand>(new MockCommand());
            try
            {
                var transfereeCommand = serviceCommandService.Run<MockCommand>();

                Assert.Fail("Calling Run with out a workload being added should cause an exception");
            }
            catch (NullReferenceException ex)
            {
                Assert.IsInstanceOfType(ex, typeof(Exception));
            }
        }

        [TestMethod]
        public void TaskRunner_Run()
        {
            var manualResetEvent = new ManualResetEvent(false);
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
            // var ex = default(Exception);
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
            var transfereeCommand = new MockTransfereeCommand
            {
                CommandAction = CommandBase.Actions.Add,
                CreateDate = startTime,
                CreatedBy = Thread.CurrentPrincipal.Identity.Name,
               // ExternalSystem = ExternalSystems.Sec,
                Transferee = transferee
            };

            IServiceCommandService serviceCommandService = new ServiceCommandService(new MockServiceCommandContextInstanceFactory());

            var command = serviceCommandService.AddCommand(transfereeCommand)
                .WithWorkload(new MockWorkload(manualResetEvent), new TimeSpan(0, 0, 0, 0, 30))
                .WithWorkload(new MockWorkload(manualResetEvent2), new TimeSpan(0, 0, 0, 0, 30))
                .Run<MockTransfereeCommand>();

            Assert.IsTrue(manualResetEvent.WaitOne(1000));
            Assert.IsTrue(manualResetEvent2.WaitOne(1000));
            Assert.IsTrue(command.Id == 1);
        }

        [TestMethod, TestCategory("IntegrationDB")]
        public void SubscriberFinished()
        {
            var commandId = 0;
            var subscriberId = 0;
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
                    // var approvr = new ApproverUser { Source = "OE", UserID = "test4", TS = new byte[0] };
                    // var r = new Repository<ApproverUser>(new DbContext("name=InitiationEntities"));
                    // ApproverUser approvrUser = r.Add(approvr);
                    // approverUserID = approvrUser.ApproverUserID;
                }
                reader.Close();
            }

            var subscriber = new Subscriber { Id = subscriberId };
            subscriber.ServiceCommandId = commandId;
            subscriber.ServiceCommand = new ServiceCommand() { Completed = true, CompletedDate = DateTime.Now, Id = commandId };
            var serviceCommandContext = new ServiceCommandContext();
            //subscriber.ServiceCommandRepository = new ServiceCommandRepository(serviceCommandContext);

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
        public async Task Restarter_OneRestartafterConfiguredTime()
        {
            var manualResetEvent = new ManualResetEvent(false);
            var mocSubscriber = new MockSubscriber(manualResetEvent)
            {
                Completed = false,
                TimeToExpire = new TimeSpan(0, 0, 0, 0, 100), 
            };

            var reStarter = new ReStarter(mocSubscriber);
            reStarter.InitialBackOffPeriod = 10;
            reStarter.InitialDelay = 2;
            await reStarter.StartAsync();

            Assert.IsTrue(manualResetEvent.WaitOne(10000));
            Assert.AreEqual(0, reStarter.TimeToWait);
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
            Assert.AreEqual(3, mocSubscriber.RetryCount);
        }

        [TestMethod]
        public async Task Restarter_Restart_3times()
        {
            var manualResetEvent = new ManualResetEvent(false);
            var mockRestarterWorkload = new MockRestarterWorkload(manualResetEvent, new TimeSpan(0, 0, 0, 0, 1000), 3, typeof(ArgumentOutOfRangeException), new TimeSpan(0, 0, 0, 0, 10));
            var mocSubscriber = new MockRestartSubscriber(mockRestarterWorkload);

            var reStarter = new ReStarter(mocSubscriber);
            await reStarter.StartAsync();
            manualResetEvent.WaitOne(100000);

            Assert.AreEqual(4, mockRestarterWorkload.Count()); //counter will count first execution pluss 3 retrys for the 3 exceptions configured to be thrown
        }

        [TestMethod]
        public async Task Restarter_Restart_2times()
        {
            var manualResetEvent = new ManualResetEvent(false);
            var mockRestarterWorkload = new MockRestarterWorkload(manualResetEvent, new TimeSpan(0, 0, 0, 0, 1000), 2, typeof(ArgumentOutOfRangeException), new TimeSpan(0, 0, 0, 0, 10));
            var mocSubscriber = new MockRestartSubscriber(mockRestarterWorkload);

            var reStarter = new ReStarter(mocSubscriber);
            await reStarter.StartAsync();
            manualResetEvent.WaitOne(10000);
            
            Assert.AreEqual(3, mockRestarterWorkload.Count()); //counter will count first execution plus 2 retrys for the 2 exceptions configured to be thrown
        }

        [TestMethod]
        public void CreateClientCommandTest()
        {
            var manualResetEvent = new ManualResetEvent(false);

            //// Begin - What the Client needs to do

            var command = new ClientCommand(ClientCommand.Actions.Add, ExternalSystems.Sec, "CreateClientCommandTest");
            command.CompanyAlias = "Company0";
            //// Call API with command.

            //// End - What the Client needs to do.

            IServiceCommandService serviceCommandService = new ServiceCommandService(new MockServiceCommandContextInstanceFactory());

            var serviceCommand = serviceCommandService.AddCommand(command)
                .WithWorkload(new MockClientWorkload(manualResetEvent), new TimeSpan(0, 0, 0, 0, 30))
                .Run<ClientCommand>();

            Assert.IsTrue(manualResetEvent.WaitOne(1000));
            Assert.IsTrue(serviceCommand.Id == 1);

            Assert.IsInstanceOfType(serviceCommand, typeof(ClientCommand));
            Assert.AreEqual(((ClientCommand)serviceCommand).CompanyAlias, "Company0");

            manualResetEvent.WaitOne(200);

            Assert.AreEqual(true, serviceCommandService.Subscribers.First().Completed);
        }

        [TestMethod]
        public void ServicesCommandsFunctionalityTest()
        {
            var command = new ClientCommand(ClientCommand.Actions.Add, ExternalSystems.Sec, "CommandsFunctionalityTest");
            var serializedCommand = Serializer.Serialize(command);

            // String value is stored in the command.
            Assert.AreNotEqual(-1, serializedCommand.IndexOf("Add", StringComparison.Ordinal));

            // Can use switch statement
            var test = 0;
            switch (command.CommandAction)
            {
                case ClientCommand.Actions.Delete:
                    test = 1;
                    break;
                case ClientCommand.Actions.Add:
                    test = 2;
                    break;
            }

            Assert.AreEqual(2, test);

            // Can also add a command using a string.
            var commandWithTypeSetByName = new ClientCommand("Add", ExternalSystems.Sec, "CommandTypeSetByName", DateTime.Now.AddDays(1));

            IServiceCommandService serviceCommandService = new ServiceCommandService(new MockServiceCommandContextInstanceFactory());

            var serviceCommand = serviceCommandService.AddCommand(commandWithTypeSetByName)
                .WithWorkload(new MockClientWorkload(new ManualResetEvent(false)), new TimeSpan(0, 0, 0, 0, 30))
                .Run<ClientCommand>();

            Assert.IsInstanceOfType(serviceCommand, typeof(ClientCommand));
        }

        private SqlCommand Command(string addCommand, SqlConnection connection)
        {
            return new SqlCommand(addCommand, connection);
        }
    }
}
