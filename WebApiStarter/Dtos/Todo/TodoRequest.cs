using System.ComponentModel.DataAnnotations;

namespace WebApiStarter.Dtos.Todo
{
    public class TodoRequest
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public bool IsComplete { get; set; }
    }
}
