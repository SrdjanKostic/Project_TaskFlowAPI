using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowAPI.Data;
using TaskFlowAPI.Models;
using TaskFlowAPI.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace TaskFlowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public TasksController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: api/Tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks(
            int pageNumber = 1,
            int pageSize = 10,
            string sortBy = "title",
            string sortDirection = "asc",
            bool? isCompleted = null,
            int? projectId = null,
            string? search = null)
        {
            var tasks = _context.TaskItems.AsQueryable();

            // Filtriranje
            if (isCompleted.HasValue)
            {
                tasks = tasks.Where(t => t.IsCompleted == isCompleted.Value);
            }

            if (projectId.HasValue)
            {
                tasks = tasks.Where(t => t.ProjectId == projectId.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                tasks = tasks.Where(t => t.Title.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            // Sortiranje
            switch (sortBy.ToLower())
            {
                case "title":
                    tasks = sortDirection == "desc" ? tasks.OrderByDescending(t => t.Title) : tasks.OrderBy(t => t.Title);
                    break;
                case "iscompleted":
                    tasks = sortDirection == "desc" ? tasks.OrderByDescending(t => t.IsCompleted) : tasks.OrderBy(t => t.IsCompleted);
                    break;
                default:
                    tasks = tasks.OrderBy(t => t.TaskId);
                    break;
            }

            // Paginacija
            tasks = tasks.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return await tasks.ToListAsync();
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTaskItem(int id)
        {
            var taskItem = await _context.TaskItems.FindAsync(id);

            if (taskItem == null)
            {
                return NotFound();
            }

            return taskItem;
        }

        // POST: api/Tasks
        [HttpPost]
        public async Task<ActionResult<TaskItem>> PostTaskItem(TaskItem taskItem)
        {
            _context.TaskItems.Add(taskItem);
            await _context.SaveChangesAsync();

            // Slanje email obavestenja :
            await _emailService.SendEmailAsync(
                "recipient@example.com", // Ovde treba da se doda email adresa primaoca
                "Nova Dodela Zadataka",
                $"Dodeljen Vam je novi zadatak: {taskItem.Title}");

            return CreatedAtAction("GetTaskItem", new { id = taskItem.TaskId }, taskItem);
        }

        // PUT: api/Tasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaskItem(int id, TaskItem taskItem)
        {
            if (id != taskItem.TaskId)
            {
                return BadRequest();
            }

            _context.Entry(taskItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskItem(int id)
        {
            var taskItem = await _context.TaskItems.FindAsync(id);
            if (taskItem == null)
            {
                return NotFound();
            }

            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskItemExists(int id)
        {
            return _context.TaskItems.Any(e => e.TaskId == id);
        }
    }
}
