using System.ComponentModel.DataAnnotations;

namespace CMS2.Models
{
    public class TaskModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        
        [DataType(DataType.DateTime)] 
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int UserId { get; set; }

        public User? User { get; set; }
       
    }
}
