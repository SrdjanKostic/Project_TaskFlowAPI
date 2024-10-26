using System.ComponentModel.DataAnnotations;

namespace TaskFlowAPI.Models
{
    public class TaskItem
    {
        [Key]
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }

        public int ProjectId { get; set; }
        public Project? Project { get; set; }

        public TaskItem()
        {
            Title = String.Empty;
            Description = String.Empty;
            IsCompleted = false;
        }
    }
}
