namespace Jawohl.Services.Commands.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Claim
    {
        [Required]
        public Guid ObjectGuid { get; set; }

        public Guid? ClaimTypeId { get; set; }

        public Guid? ClaimValueId { get; set; }

        public Guid? RelyingPartyId { get; set; }

        [Required]
        public string ClaimTypeName { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string ClaimValueName { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
