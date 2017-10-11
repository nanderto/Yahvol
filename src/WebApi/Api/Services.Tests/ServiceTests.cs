namespace BrookfieldGrs.Services.Tests
{
    using System;
    using System.Data.Entity;
    using System.Runtime.Remoting.Messaging;
    using System.Threading;
    using System.Threading.Tasks;

    using BrookfieldGrs.Services;
    using BrookfieldGrs.Services.Fakes;
    using BrookfieldGrs.Services.WebApi.Commands;
    using BrookfieldGrs.Services.WebApi.Models;
    using BrookfieldGrs.Services.WebApi.Models.Fakes;
    using BrookfieldGrs.Services.WebApi.Workloads;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;

    [TestClass]
    public class ServiceTests
    {

        [TestMethod]
        public void TransfereeCommandSave()
        {
            var transfereeCommandService = new ServiceCommandService(new ServiceCommandRepository(new MockServiceCommandContext()));
            var serviceCommand = new ServiceCommand("xxxx");
            var add = transfereeCommandService.Add(serviceCommand);
            Assert.IsTrue(add.Id > 0);
            var serviceCommand2 = new ServiceCommand("yy");
            var add2 = transfereeCommandService.Add(serviceCommand2);
            Assert.IsTrue(add2.Id == 2);
            var serviceCommand3 = new ServiceCommand("yy");
            var add3 = transfereeCommandService.Add(serviceCommand3);
            Assert.IsTrue(add3.Id == 3);
        }

        [TestMethod]
        public void ServiceCommandServiceAddAdd()
        {
            var resetEvent = new ManualResetEvent(false);
            var approver = new ApproverUser { ApproverUserID = 3, Source = "OE", UserID = "test4", TS = new byte[0] };
            ApproverUser addApproverUser = null;
            ApproverUser updateApproverUser = null;
            var approverCommand = new ApproverCommand
                                      {
                                          Approver = approver,
                                          CommandType = CommandType.Add,
                                          CreateDate = DateTime.Now,
                                          GrsSystemType = GrsSystemType.Gsp,
                                          CreatedBy = "XXX"
                                      };
            var serviceCommandService = new ServiceCommandService(new ServiceCommandRepository(new MockServiceCommandContext()));
            var stubApproverRepository = new StubIRepository<ApproverUser>
                {
                    AddT0 = user =>
                        {
                            addApproverUser = user;
                            Assert.AreEqual(approver, addApproverUser);
                            resetEvent.Set();
                            return addApproverUser;
                        },
                    UpdateT0 = user =>
                        {
                            updateApproverUser = user;
                            Assert.AreEqual(approver, updateApproverUser);
                            resetEvent.Set();
                        }
                };

            var returnApprover = serviceCommandService.AddCommand<ApproverCommand>(approverCommand)
                .WithWorkload(new ApproverWorkload(stubApproverRepository), new TimeSpan(0, 0, 0, 30))
                .Run<ApproverCommand>();

            resetEvent.WaitOne();
            Assert.AreEqual(returnApprover.Id, 1);
            Assert.AreEqual(addApproverUser, approver);
            Assert.AreEqual(updateApproverUser, null);
        }

        [TestMethod]
        public void ServiceCommandServiceAddUpdate()
        {
            var resetEvent = new ManualResetEvent(false);
            var approver = new ApproverUser { ApproverUserID = 3, Source = "OE", UserID = "test4", TS = new byte[0] };
            ApproverUser addApproverUser = null;
            ApproverUser updateApproverUser = null;
            var approverCommand = new ApproverCommand
            {
                Approver = approver,
                CommandType = CommandType.Update,
                CreateDate = DateTime.Now,
                GrsSystemType = GrsSystemType.Gsp,
                CreatedBy = "XXX"
            };
            var mockRepo = new MockServiceCommandContext();
            mockRepo.ManualResetEvent = resetEvent;
            var serviceCommandService = new ServiceCommandService(new ServiceCommandRepository(mockRepo));
            var stubApproverRepository = new StubIRepository<ApproverUser>
            {
                AddT0 = user =>
                {
                    addApproverUser = user;
                    Assert.AreEqual(approver, addApproverUser);
                    resetEvent.Set();
                    return addApproverUser;
                },
                UpdateT0 = user =>
                {
                    updateApproverUser = user;
                    Assert.AreEqual(approver, updateApproverUser);
                    resetEvent.Set();
                }
            };

            var returnApprover = serviceCommandService.AddCommand<ApproverCommand>(approverCommand)
                .WithWorkload(new ApproverWorkload(stubApproverRepository), new TimeSpan(0, 0, 0, 30))
                .Run<ApproverCommand>();
            resetEvent.WaitOne();
            Assert.AreEqual(returnApprover.Id, 1);
            Assert.AreEqual(addApproverUser, null);
            Assert.AreEqual(updateApproverUser, approver);
        }

        [TestMethod]
        public void ServiceCommandServiceAddUpdateNew()
        {
            var resetEvent = new ManualResetEvent(false);
            var approver = new ApproverUser { ApproverUserID = 3, Source = "OE", UserID = "test4", TS = new byte[0] };
            ApproverUser addApproverUser = null;
            ApproverUser updateApproverUser = null;
            var approverCommand = new ApproverCommand
            {
                Approver = approver,
                CommandType = CommandType.Update,
                CreateDate = DateTime.Now,
                GrsSystemType = GrsSystemType.Gsp,
                CreatedBy = "XXX"
            };
            var mockRepo = new MockServiceCommandContext();
            mockRepo.ManualResetEvent = resetEvent;
            var serviceCommandService = new ServiceCommandService(new ServiceCommandRepository(mockRepo));
            var stubApproverRepository = new StubIRepository<ApproverUser>
            {
                AddT0 = user =>
                {
                    addApproverUser = user;
                    Assert.AreEqual(approver, addApproverUser);
                    resetEvent.Set();
                    return addApproverUser;
                },
                UpdateT0 = user =>
                {
                    updateApproverUser = user;
                    Assert.AreEqual(approver, updateApproverUser);
                    resetEvent.Set();
                }
            };

            //serviceCommandService.AddWorkload(new ApproverWorkload(stubApproverRepository, approverCommand), new TimeSpan(0, 0, 0, 30));
            ApproverCommand returnComand = serviceCommandService.AddCommand<ApproverCommand>(approverCommand)
                .WithWorkload(new ApproverWorkload(stubApproverRepository), new TimeSpan(0, 0, 0, 30))
                //.WithWorkload(new ApproverWorkload(stubApproverRepository), new TimeSpan(0, 0, 0, 30))
                .Run<ApproverCommand>();
           
            resetEvent.WaitOne();
            Assert.AreEqual(returnComand.Id, 1);
            Assert.AreEqual(addApproverUser, null);
            Assert.AreEqual(updateApproverUser, approver);
        }

        [TestMethod]
        public void ServiceCommandServiceAddUpdateCheckFinished()
        {
            var resetEvent = new ManualResetEvent(false);
            var resetEvent2 = new ManualResetEvent(false);
            var resetEvent3 = new ManualResetEvent(false);
            var approver = new ApproverUser { ApproverUserID = 3, Source = "OE", UserID = "test4", TS = new byte[0] };
            ApproverUser addApproverUser = null;
            ApproverUser updateApproverUser = null;
            var approverCommand = new ApproverCommand
            {
                Approver = approver,
                CommandType = CommandType.Update,
                CreateDate = DateTime.Now,
                GrsSystemType = GrsSystemType.Gsp,
                CreatedBy = "XXX"
            };
            var mockRepo = new StubIServiceCommandRepository 
            { 
                UpdateInt32Int32 = (subscriberId, serviceCommandId) =>
                    {
                        Assert.AreEqual(1, subscriberId);
                        Assert.AreEqual(1, serviceCommandId);
                        resetEvent3.Set();
                        return Task.Factory.StartNew(this.Dummy);
                    },
                SaveServiceCommandRef = (ref ServiceCommand serviceCommand) =>
                    {
                        serviceCommand.Id = 1;
                        var subs = (List<Subscriber>) serviceCommand.Subscribers;
                        subs[0].Id = 1;
                        resetEvent2.Set();
                        return 1;
                    }
            };

            var serviceCommandService = new ServiceCommandService(mockRepo);
            var stubApproverRepository = new StubIRepository<ApproverUser>
            {
                AddT0 = user =>
                {
                    addApproverUser = user;
                    Assert.AreEqual(approver, addApproverUser);
                    resetEvent.Set();
                    return addApproverUser;
                },
                UpdateT0 = user =>
                {
                    updateApproverUser = user;
                    Assert.AreEqual(approver, updateApproverUser);
                    resetEvent.Set();
                }
            };

            var returnApprover = serviceCommandService.AddCommand<ApproverCommand>(approverCommand)
                .WithWorkload(new ApproverWorkload(stubApproverRepository), new TimeSpan(0, 0, 0, 30))
                .Run<ApproverCommand>();
            resetEvent.WaitOne();
            resetEvent2.WaitOne();
            resetEvent3.WaitOne();
            Assert.AreEqual(returnApprover.Id, 1);
            Assert.AreEqual(addApproverUser, null);
            Assert.AreEqual(updateApproverUser, approver);
        }

        private void Dummy()
        {}
    }
}
