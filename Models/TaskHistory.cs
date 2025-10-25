using CMS2.Models;

public class TaskHistory
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string? Action { get; set; } // cho phép null
    public DateTime ChangedAt { get; set; }
    public string? ChangedBy { get; set; }

    public TaskModel Task { get; set; }
}
