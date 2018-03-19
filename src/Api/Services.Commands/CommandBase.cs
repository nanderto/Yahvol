namespace Yahvol.Services.Commands
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    [DataContract]
    public abstract class CommandBase
    {
        protected CommandBase()
        {
        }

		protected CommandBase(
			string commandAction,
			string createdBy,
			DateTime? createdDate)
		{
			this.CommandAction = commandAction;
			this.CreatedBy = createdBy;
			this.CreateDate = createdDate ?? DateTime.Now;
		}

		protected CommandBase(
            string commandAction,
            string clientSystem,
            string createdBy,
            DateTime? createdDate)
        {
            this.CommandAction = commandAction;
            this.ClientSystem = clientSystem;
            this.CreatedBy = createdBy;
            this.CreateDate = createdDate ?? DateTime.Now;
        }

        [DataMember]
        [Required]
        public string CommandAction { get; set; }

        /// <summary>
        /// record the system from which the command came from
        /// </summary>
        [DataMember]
        public string ClientSystem { get; set; }

        /// <summary>
        /// Record the identity of the requestor.
        /// </summary>
        [DataMember]
        [Required]
        public string CreatedBy { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }

        public string ConnectionId { get; set; }

        public int? Id { get; set; }

        /// <summary>
        /// Ensures the command is created only once
        /// </summary>
        public string UniqueKey { get; set; }

        public static class Actions
        {
            public const string Add = "Add";
            public const string Update = "Update";
            public const string Delete = "Delete";
        }
    }
}
