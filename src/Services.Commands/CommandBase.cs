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
            string ExternalSystem,
            string createdBy,
            DateTime? createdDate)
        {
            this.CommandAction = commandAction;
            this.ExternalSystem = ExternalSystem;
            this.CreatedBy = createdBy;
            this.CreateDate = createdDate ?? DateTime.Now;
        }

        [DataMember]
        [Required]
        public string CommandAction { get; set; }

        [DataMember]
        public string ExternalSystem { get; set; }

        [DataMember]
        [Required]
        public string CreatedBy { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }

        public string ConnectionId { get; set; }

        public int? Id { get; set; }

        public string UniqueKey { get; set; }

        public static class Actions
        {
            public const string Add = "Add";
            public const string Update = "Update";
            public const string Delete = "Delete";
        }
    }
}
