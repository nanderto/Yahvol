namespace BrookfieldGrs.Web.HttpTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using BrookfieldGrs.Services;
    using BrookfieldGrs.Services.Commands;

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

        public string ConnectionId { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreatedBy { get; set; }

        public GrsSystemType GrsSystemType { get; set; }

        public int? Id { get; set; }

        public string UniqueKey { get; set; }

        internal Transferee Transferee { get; set; }
    }
}
