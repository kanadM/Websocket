using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{

    public class CredentialUpdateInputParam
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }

    }
}