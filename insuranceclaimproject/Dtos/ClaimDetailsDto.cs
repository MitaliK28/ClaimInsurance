// Dtos/ClaimDetailsDto.cs
using insuranceclaimproject.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace insuranceclaimproject.Dtos
{
    public class ClaimDetailsDto
    {
        public int ClaimId { get; set; }

        public int PolicyId { get; set; }

        public decimal ClaimAmount { get; set; }

        public DateTime ClaimDate { get; set; }

        public ClaimStatus ClaimStatus { get; set; }

        public int AdjusterId { get; set; }
    }

}