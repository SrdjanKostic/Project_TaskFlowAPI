using System.ComponentModel.DataAnnotations.Schema;

namespace TaskFlowAPI.Models
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public User? User { get; set; }
        public ICollection<TaskItem> TasksItems { get; set; }

        public Project()
        {
            Name = string.Empty;
            Description = string.Empty;
            CreatedAt = DateTime.UtcNow;
            TasksItems = new List<TaskItem>();
        }
    }
}
