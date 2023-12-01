using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiStarter.Attributes;
using WebApiStarter.Constants;
using WebApiStarter.Data;
using WebApiStarter.Dtos.Todo;
using WebApiStarter.Models;

namespace WebApiStarter.Controllers
{
	public class TodoController : ApiControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TodoController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Get all Todo items
        /// </summary>
        [HttpGet]
        [ProducesOK(typeof(List<TodoResponse>))]
        public IActionResult GetAll()
        {
            return Ok(_context.TodoItems.Select(x => new TodoResponse(x.Id, x.Name, x.IsComplete)).ToList());
        }

        /// <summary>
        /// Get Todo item by id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesOK(typeof(TodoResponse)), ProducesNotFound]
        public IActionResult GetById([FromRoute] long id)
        {
            var item = _context.TodoItems.Find(id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(new TodoResponse(item.Id, item.Name, item.IsComplete));
        }

        /// <summary>
        /// Create Todo item
        /// </summary>
        [HttpPost]
        [Authorize(Roles = RoleConstants.Admin)]
        [ProducesCreated(typeof(TodoResponse)), ProducesBadRequest]
        public IActionResult Create([FromBody] TodoRequest model)
        {
            var todoItem = new TodoItem
            {
                Name = model.Name!,
                IsComplete = model.IsComplete
            };

            _context.TodoItems.Add(todoItem);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = todoItem.Id }, new TodoResponse(todoItem.Id, todoItem.Name, todoItem.IsComplete));
        }

        /// <summary>
        /// Update Todo item
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = RoleConstants.Admin)]
        [ProducesNoContent, ProducesBadRequest, ProducesNotFound]
        public IActionResult Update([FromRoute] long id, [FromBody] TodoRequest model)
        {
            var todo = _context.TodoItems.Find(id);

            if (todo == null)
            {
                return NotFound();
            }

            todo.Name = model.Name!;
            todo.IsComplete = model.IsComplete;

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
