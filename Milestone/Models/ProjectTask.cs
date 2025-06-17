using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Milestone.Models;

namespace Milestone.Models
{
    public class ProjectTask
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        public DateTime Deadline { get; set; }

        public int Priority { get; set; }

        public bool IsCompleted { get; set; }

        public int ProjectId { get; set; }
        public Project? Project { get; set; }

        public string? AssignedUserId { get; set; }

        [ForeignKey("AssignedUserId")]
        public ApplicationUser? AssignedUser { get; set; }

        public ICollection<Report>? Reports { get; set; }
        public enum TaskStatus
        {
            NotStarted,
            InProgress,
            Completed,
            Overdue
        }
        public TaskStatus Status { get; set; } = TaskStatus.NotStarted;

    }
}