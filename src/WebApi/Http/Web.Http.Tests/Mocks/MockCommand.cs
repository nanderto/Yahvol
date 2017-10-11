using System;
namespace BrookfieldGrs.Web.Http.Tests.Mocks
{
    using System;
    using BrookfieldGrs.Services.Commands;

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
