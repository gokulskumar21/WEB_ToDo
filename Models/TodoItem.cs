using System;

namespace Todo.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Completed { get; set; }
        public DateTime? CompletionDate { get; set; } 
    }
}
