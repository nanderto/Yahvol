namespace Yahvol.Services.Commands
{
    using System;

    public interface ICommand
    {
        string CommandAction { get; set; }

        string ExternalSystem { get; set; }

        string CreatedBy { get; set; }

        DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the Id of Command, created when the command is issued, and returned to the caller to allow the caller to query for the command
        /// </summary>
        int? Id { get; set; }

        string ConnectionId { get; set; }

        /// <summary>
        /// Gets or sets the UniqueKey for the command. Commands that are issued with Unique keys that have already been used, will be rejected.
        /// Client should generate a GUID or other unique key and add it to the command. 
        /// </summary>
        string UniqueKey { get; set; }
    }
}