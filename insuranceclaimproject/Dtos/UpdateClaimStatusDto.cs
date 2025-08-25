// Dtos/UpdateClaimStatusDto.cs
using insuranceclaimproject.Models;
using System.ComponentModel.DataAnnotations;

namespace insuranceclaimproject.Dtos
{
    public class UpdateClaimStatusDto
    {
        [Required]
        public ClaimStatus Status { get; set; }
    }
}