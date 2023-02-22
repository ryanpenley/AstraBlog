using System.ComponentModel.DataAnnotations;

namespace AstraBlog.Models
{
    public class EmailData
    {
        [Required]
        public string? EmailAddress { get; set; }
        [Required]
        public string? EmailSubject { get; set; }
        [Required]
        public string? EmailBody { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
    }
}
