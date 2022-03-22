using System.ComponentModel.DataAnnotations;

namespace WebApiStarter.Models
{
    public class TodoItem
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public bool IsComplete { get; set; }
    }
}
