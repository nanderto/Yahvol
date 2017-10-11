namespace Jawohl.Services.Commands.Initiation
{
    using System.ComponentModel.DataAnnotations;
    using Jawohl.Services.Commands.Models;

    public class ApproverCommand : CommandBase, ICommand
    {
        [Required]
        public ApproverUser Approver { get; set; }
    }
}
