using System.ComponentModel.DataAnnotations;

namespace CEGA.Models.ViewModels
{
    public class VerifyOtpVM
    {
        [Required]
        public string UserId { get; set; } = "";

        [Required, StringLength(6, MinimumLength = 6)]
        public string Code { get; set; } = "";
    }
}
