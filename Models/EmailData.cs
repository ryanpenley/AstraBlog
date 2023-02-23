using System.ComponentModel.DataAnnotations;

namespace AstraBlog.Models
{
    public class EmailData
    {
        [Required]
        public string? SenderEmailAddress { get; set; }
        [Required]
        public string? Body { get; set; }
        [Required]
        public string? EmailSenderName { get; set; }
    }
}
