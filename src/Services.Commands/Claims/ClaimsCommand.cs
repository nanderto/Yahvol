namespace Jawohl.Services.Commands.Claims
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Jawohl.Services.Commands.Models;

    public class ClaimsCommand : CommandBase, ICommand
    {
        public ClaimsCommand()
        {
            this.Claims = new List<Claim>();
        }

        [Required]
        public List<Claim> Claims { get; set; }
    }
}
