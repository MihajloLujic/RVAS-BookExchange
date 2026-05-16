using System.ComponentModel.DataAnnotations;

namespace RVAS_BookExchange.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email je obavezan.")]
    [EmailAddress(ErrorMessage = "Email nije u ispravnom formatu.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lozinka je obavezna.")]
    public string Password { get; set; } = string.Empty;
}