using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Milestone.Models;

namespace Milestone.Models
{
    public class Report
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime DateSubmitted { get; set; } = DateTime.Now;

        public int TaskId { get; set; }
        public ProjectTask? Task { get; set; }

        public string? SubmittedById { get; set; }

        [ForeignKey("SubmittedById")]
        public ApplicationUser? SubmittedBy { get; set; }
    }
}