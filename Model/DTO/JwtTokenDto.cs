using System.ComponentModel.DataAnnotations;

namespace TimeOffTracker.Model.DTO {
    public class JwtTokenDto {
        [Required]
        public string Token { get; set; }
    }
}