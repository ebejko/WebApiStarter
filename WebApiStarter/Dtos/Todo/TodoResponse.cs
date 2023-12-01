namespace WebApiStarter.Dtos.Todo
{
    public class TodoResponse(long id, string? name, bool isComplete)
	{
		public long Id { get; } = id;

		public string? Name { get; } = name;

		public bool IsComplete { get; } = isComplete;
	}
}
