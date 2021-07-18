using System.ComponentModel.DataAnnotations;

namespace TimeOffTracker.Model.DTO {
    public class AuthDto {
        [Required]
        public string Login { get; set; }
    
        [Required]
        public string Password { get; set; }
    }
}