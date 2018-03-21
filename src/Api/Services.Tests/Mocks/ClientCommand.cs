namespace Yahvol.Services.Tests.Mocks
{

        using System;
        using System.Collections.Generic;
        using System.Runtime.Serialization;
        using Yahvol.Services.Commands.Models;
        using Yahvol.Services.Commands;

        public class ClientCommand : CommandBase, ICommand
        {
            public ClientCommand() : base()
            {
            }

            public ClientCommand(string commandAction, string createdBy, DateTime? createDate = null)
                : this(commandAction, "", createdBy, createDate)
            {
            }

            public ClientCommand(string commandAction, string externalSystem, string createdBy, DateTime? createDate = null)
                : base(commandAction, externalSystem, createdBy, createDate)
            {
            }

            [DataMember]
            public string CompanyAlias { get; set; }

            [DataMember]
            public List<string> ClientRoles { get; set; }

            [DataMember]
            public List<string> OrganizationGroups { get; set; }

            [DataMember]
            public List<ProgramGroup> ProgramGroups { get; set; }

            public static new class Actions
            {
                /// <summary>Create OE Client in AD, including sub-OUs, ClientRoles, and Program Groups 
                /// <para/>Required: CompanyAlias (string)
                /// <para/>Optional: ClientRoles (List(string)), Programs (List(ProgramGroup)), OrganizationGroups (List(string))
                /// </summary>
                public const string Add = "Add";

                /// <summary>Delete OE Client in AD
                /// <para/>Required: CompanyAlias (string)
                /// </summary>
                public const string Delete = "Delete";
            }
        }
    

}

namespace Yahvol.GlobalConstants
{
}

namespace Yahvol.Services.Commands.Models
{
    public class ProgramGroup
    {
        public string GroupName { get; set; }

        public string MoveType { get; set; }
    }
}

namespace Yahvol.Services.Commands.AD
{
}

namespace Yahvol.GlobalConstants
{
    public static class ExternalSystems
    {
        public const string Xrt = "ReloAccess";
        public const string Sec = "Sec";
        public const string Nka = "Nka";
    }
}