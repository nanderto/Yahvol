namespace Yahvol.Web.Http.Tests.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Yahvol.Services.Commands;
    public enum CommandType
    {
        Update,
        Add,
        Delete,
        UpdateFamilyMember,
        AddFamilyMember
    }

    public enum GrsSystemType
    {
        Gsp,
        Igp
    }

    public class MockTransfereeCommand : CommandBase, ICommand
    {
        public string CommandType { get; set; }

        public GrsSystemType GrsSystemType { get; set; }
        
        internal Transferee Transferee { get; set; }
    }
}
