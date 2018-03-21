namespace Yahvol.Services.Tests.Mocks
{
    using System;
    using Yahvol.Services.Commands;

    public class MockCommand : CommandBase, ICommand
    {
        public MockCommand()
        {
        }

        public MockCommand(string commandType, string externalSystem, string createdBy, DateTime? createDate = null) 
            : base(commandType, externalSystem, createdBy, createDate)
        {
        }
    }
}
