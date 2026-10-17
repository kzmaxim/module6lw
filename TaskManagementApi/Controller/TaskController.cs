using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Models;

namespace TaskManagementApi.Controller;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
public class TasksController : ControllerBase
{
    // Тестовые записи
    private static readonly List<TaskItem> _tasks = new()
    {
        new TaskItem { Id = 1, Title = "Изучить JWT", Description = "Разобраться с Claims и подписью", IsCompleted = true },
        new TaskItem { Id = 2, Title = "Сделать лабу", Description = "Написать Task Management API", IsCompleted = false }
    };

    // Доступ без авторизации
    [HttpGet]
    public IActionResult GetTasks()
    {
        return Ok(_tasks);
    }

    // Доступ без авторизации
    [HttpGet("{id}")]
    public IActionResult GetTask(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task == null) return NotFound();
        return Ok(task);
    }

    // Только с JWT
    [Authorize]
    [HttpPost]
    public IActionResult AddTask([FromBody] TaskItem task)
    {
        // Работа с Claims: получение имени
        var username = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        task.Id = _tasks.Count > 0 ? _tasks.Max(t => t.Id) + 1 : 1;
        _tasks.Add(task);

        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, new 
        { 
            Task = task, 
            CreatedBy = username 
        });
    }

    // Только с JWT
    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult DeleteTask(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task == null) return NotFound();
            
        _tasks.Remove(task);
        return Ok();
    }
}