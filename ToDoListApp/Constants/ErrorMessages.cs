namespace ToDoListApp.Constants
{
    public class ErrorMessages
    {
        public const string DuplicateToDoName = "A ToDo with the same name already exists and is not completed.";
        public const string UncompletedSubtasks = "Cannot check the ToDo until all subtasks are completed.";
        public const string TodoNotFound = "ToDo with ID '{0}' not found.";
        public const string DuplicateListName = "List name already exists.";
        public const string ListNotFound = "List with ID {0} not found.";
        public const string InvalidListId = "List ID cannot be null.";
    }
}
