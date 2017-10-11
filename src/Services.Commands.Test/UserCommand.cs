namespace Yahvol.Services.Commands.Test
{
    using System;
    using Yahvol.Services.Commands;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    internal class UserCommand : CommandBase, ICommand
    {
        private bool enabled = true;
        private string upsert;
        private string externalSystem;
        private string v;

        public UserCommand(string upsert, string externalSystem, string v)
        {
            this.upsert = upsert;
            this.externalSystem = externalSystem;
            this.v = v;
        }

        protected UserCommand() : base()
        {
        }

        protected UserCommand(string commandAction, string createdBy, DateTime? createdDate) : this(commandAction, "", createdBy, createdDate)
        {
        }

        protected UserCommand(string commandAction, string ExternalSystem, string createdBy, DateTime? createdDate) : base(commandAction, ExternalSystem, createdBy, createdDate)
        {
        }

        [DataMember]
        public string CompanyAlias { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string EmployeeNumber { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string UserType { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public bool ForcePasswordChange { get; set; }

        [DataMember]
        public bool SetPasswordNeverExpires { get; set; }

        [DataMember]
        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }

        [DataMember]
        public List<string> GroupNames { get; set; }

        [DataMember]
        public string GroupName { get; set; }

        public static new class Actions
        {
            /// <summary>Create/Update OE User in AD with groups; remove them from groups that are not in the list
            /// <para/>Required: Username (string)
            /// <para/>Required for Insert, Optional for Update: 
            /// CompanyAlias (string), UserType (string - C, T, A, or AG), EmployeeNumber (string),
            /// FirstName (string), LastName (string), Email (string), Password (string), ForcePasswordChange (bool),
            /// PasswordNeverExpires (bool), Enabled (bool - defaults to true), GroupNames (List(string))
            /// </summary>
            public const string Upsert = "Upsert";

            /// <summary>Add user to a specified group
            /// <para/>Required: Username (string), GroupName (string)
            /// </summary>
            public const string AddToGroup = "AddToGroup";

            /// <summary>Remove user from a specified group
            /// <para/>Required: Username (string), GroupName (string)
            /// </summary>
            public const string RemoveFromGroup = "RemoveFromGroup";

            /// <summary>Removes user from a specified list of groups
            /// <para/>Required: Username (string), GroupNames (List(string))
            /// </summary>
            public const string RemoveFromGroups = "RemoveFromGroups";

            /// <summary>Add user to specified list of groups, without removal
            /// <para/>Required: Username (string), GroupNames (List(string))
            /// </summary>
            public const string AddToGroups = "AddToGroups";

            /// <summary>Add user to specified list of groups, and remove them from groups that are not in the list
            /// <para/>Required: Username (string), GroupNames (List(string))
            /// </summary>
            public const string UpdateGroups = "UpdateGroups";

            /// <summary>Enable/unlock or Disable AD user. Resets password if provided. Expires password if specified.
            /// <para/>Required: Username (string), Enabled (bool - defaults to true)
            /// <para/>Optional: Password (string), ForcePasswordChange (bool)
            /// </summary>
            public const string UpdateStatus = "UpdateStatus";

            /// <summary>Update user email in AD.
            /// <para/>Required: Username (string), NewEmail(string)
            /// </summary>
            public const string UpdateUserEmail = "UpdateUserEmail";
        }
    }
}