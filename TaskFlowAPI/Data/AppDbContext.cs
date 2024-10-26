using Microsoft.EntityFrameworkCore;
using TaskFlowAPI.Models;

namespace TaskFlowAPI.Data
{
    public class AppDbContext:DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<TaskItem> TaskItems { get; set; } = null!;


        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        }

        // Metod za brojanje zadataka prema statusu
        public async Task<Dictionary<string, int>> GetTaskStatusCountAsync()
        {
            int completedCount = await TaskItems.Where(t => t.IsCompleted).CountAsync();
            int notCompletedCount = await TaskItems.Where(t => !t.IsCompleted).CountAsync();

            return new Dictionary<string, int>
            {
                { "Completed", completedCount },
                { "Not Completed", notCompletedCount }
            };
        }

        // Metod za dobijanje sažetka produktivnosti
        public async Task<ProductivityReport> GetProductivitySummaryAsync()
        {
            var totalProjects = await Projects.CountAsync();
            var totalTasks = await TaskItems.CountAsync();
            var completedTasks = await TaskItems.Where(t => t.IsCompleted).CountAsync();
            var taskStatusCount = await GetTaskStatusCountAsync();

            return new ProductivityReport
            {
                TotalProjects = totalProjects,
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                TaskStatusCount = taskStatusCount
            };
        }
    }

    // Dodatna klasa za sažetak produktivnosti
    public class ProductivityReport
    {
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public Dictionary<string, int> TaskStatusCount { get; set; } = new Dictionary<string, int>();
    }
}
