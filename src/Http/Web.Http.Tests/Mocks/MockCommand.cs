using System;
namespace Yahvol.Web.Http.Tests.Mocks
{
    using System;
    using Yahvol.Services.Commands;

    public class MockCommand : CommandBase, ICommand
    {
        public MockCommand()
        {
        }

        public MockCommand(string commandType, string grsSystem, string createdBy, DateTime? createDate = null)
            : base(commandType, grsSystem, createdBy, createDate)
        {
        }
    }
}
