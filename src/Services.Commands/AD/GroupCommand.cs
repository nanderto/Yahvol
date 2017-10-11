namespace Jawohl.Services.Commands.AD
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    public class GroupCommand : CommandBase, ICommand
    {
		public GroupCommand() : base()
	    {
		}

		public GroupCommand(string commandAction, string createdBy, DateTime? createDate = null)
			: this(commandAction, "", createdBy, createDate)
		{
		}

		public GroupCommand(string commandAction, string grsSystem, string createdBy, DateTime? createDate = null)
            : base(commandAction, grsSystem, createdBy, createDate)
        {
        }

        [DataMember]
        public string CompanyAlias { get; set; }

        [DataMember]
        public string GroupName { get; set; }

        [DataMember]
        public string MoveType { get; set; }

        [DataMember]
        public List<string> Usernames { get; set; }

        public static new class Actions
        {
            /// <summary>Create ClientRoles group.
            /// <para/>Required: CompanyAlias (string), GroupName (string)
            /// </summary>
            public const string AddClientRole = "AddClientRole";

            /// <summary>Create Organization group.
            /// <para/>Required: CompanyAlias (string), GroupName (string)
            /// </summary>
            public const string AddOrganizationGroup = "AddOrganizationGroup";

            /// <summary>Create Client Role in the Consultant OU (_I_Cons[CompanyAlias])
            /// <para/>Required: CompanyAlias (string)
            /// </summary>
            public const string AddConsultantRole = "AddConsultantRole";

            /// <summary>Create/Update program group, including adding to Domestic/International groups based on move type.
            /// <para/>Required: CompanyAlias (string), GroupName (string), MoveType (string - G, D, or I)
            /// </summary>
            public const string UpsertProgram = "UpsertProgram";

            /// <summary>Delete AD FS group.
            /// <para/>Required: GroupName (string)
            /// </summary>
            public const string Delete = "Delete";

            /// <summary>Add list of users to group.
            /// <para/>Required: GroupName (string), Usernames (List(string))
            /// </summary>
            public const string AddUsers = "AddUsers";

			/// <summary>Remove list of users from group.
			/// <para/>Required: GroupName (string), Usernames (List(string))
			/// </summary>
			public const string RemoveUsers = "RemoveUsers";
		}
	}
}