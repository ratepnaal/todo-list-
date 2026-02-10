﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspSqlProject.Data;
using AspSqlProject.Models;

[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    private readonly AppDbContext _context;

    public TodoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodos()
    {
        // استرجاع البيانات مرتبة (مثلاً الأحدث أولاً)
        return await _context.Todos.OrderByDescending(t => t.Id).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> PostTodo(TodoDto todoDto)
    {
        // التحقق من صحة البيانات
        if (string.IsNullOrWhiteSpace(todoDto.Text))
            return BadRequest("Text cannot be empty.");

        var todo = new TodoItem
        {
            Text = todoDto.Text,
            Done = todoDto.Done
        };

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTodo), new { id = todo.Id }, todo);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTodo(int id, TodoDto todoDto)
    {
        var existingTask = await _context.Todos.FindAsync(id);

        if (existingTask == null)
            return NotFound($"Task with ID {id} not found.");

        // تحديث الحقول فقط
        existingTask.Text = todoDto.Text;
        existingTask.Done = todoDto.Done;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return StatusCode(500, "Database error.");
        }

        return Ok(existingTask); // أو return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null) return NotFound();

        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();

        return NoContent(); // المعيار الصحيح للحذف هو 204 NoContent
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> GetTodo(int id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null) return NotFound();
        return todo;
    }
}
