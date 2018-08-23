using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using WebApiStarter.Attributes;
using WebApiStarter.Constants;
using WebApiStarter.Data;
using WebApiStarter.Models;

namespace WebApiStarter.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TodoController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all Todo items
        /// </summary>
        [HttpGet]
        [ProducesOK(typeof(List<TodoItem>))]
        public IActionResult GetAll()
        {
            return Ok(_context.TodoItems.ToList());
        }

        /// <summary>
        /// Get Todo item by id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesOK(typeof(TodoItem)), ProducesNotFound]
        public IActionResult GetById([FromRoute] long id)
        {
            var item = _context.TodoItems.Find(id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        /// <summary>
        /// Create Todo item
        /// </summary>
        [HttpPost]
        [Authorize(Roles = RoleConstants.Admin)]
        [ProducesCreated(typeof(TodoItem)), ProducesBadRequest]
        public IActionResult Create([FromBody] TodoItem item)
        {
            _context.TodoItems.Add(item);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        /// <summary>
        /// Update Todo item
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = RoleConstants.Admin)]
        [ProducesNoContent, ProducesBadRequest, ProducesNotFound]
        public IActionResult Update([FromRoute] long id, [FromBody] TodoItem item)
        {
            var todo = _context.TodoItems.Find(id);

            if (todo == null)
            {
                return NotFound();
            }

            todo.IsComplete = item.IsComplete;
            todo.Name = item.Name;

            _context.TodoItems.Update(todo);
            _context.SaveChanges();
            return NoContent();
        }

        /// <summary>
        /// Delete Todo item
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = RoleConstants.Admin)]
        [ProducesNoContent, ProducesNotFound]
        public IActionResult Delete([FromRoute] long id)
        {
            var todo = _context.TodoItems.Find(id);

            if (todo == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todo);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
