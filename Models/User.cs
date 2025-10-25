namespace CMS2.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; } = "User";
        public ICollection<TaskModel> Tasks { get; set; } = new List<TaskModel>();
        

    }
}
