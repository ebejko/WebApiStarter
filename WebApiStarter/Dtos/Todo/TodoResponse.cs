namespace WebApiStarter.Dtos.Todo
{
    public class TodoResponse
    {
        public long Id { get; }

        public string Name { get; }

        public bool IsComplete { get; }

        public TodoResponse(long id, string name, bool isComplete)
        {
            Id = id;
            Name = name;
            IsComplete = isComplete;
        }
    }
}
