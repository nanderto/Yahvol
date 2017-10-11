namespace Jawohl.Services.Commands.AD
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Jawohl.Services.Commands.Models;

    public class ClientCommand : CommandBase, ICommand
    {
		public ClientCommand() : base()
	    {
		}

		public ClientCommand(string commandAction, string createdBy, DateTime? createDate = null)
			: this(commandAction, "", createdBy, createDate)
		{
		}

		public ClientCommand(string commandAction, string grsSystem, string createdBy, DateTime? createDate = null)
            : base(commandAction, grsSystem, createdBy, createDate)
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
