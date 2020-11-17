using System.ComponentModel.DataAnnotations;

namespace FormuleCirkelEntity.ViewModels
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
