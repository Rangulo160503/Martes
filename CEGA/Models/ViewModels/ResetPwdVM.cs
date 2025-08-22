using System.ComponentModel.DataAnnotations;

namespace CEGA.Models.ViewModels
{
    public class ResetPwdVM
    {
        [Required]
        public string UserId { get; set; } = "";

        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; } = "";

        [Required, DataType(DataType.Password), Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; } = "";
    }
}
