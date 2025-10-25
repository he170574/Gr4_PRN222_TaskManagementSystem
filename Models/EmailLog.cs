using System;

namespace CMS2.Models
{
    public class EmailLog
    {
        public int Id { get; set; }
        public string RecipientEmail { get; set; } = "";
        public string Subject { get; set; } = "";
        public DateTime SentAt { get; set; }
    }
}
