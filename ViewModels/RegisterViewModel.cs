using System.ComponentModel.DataAnnotations;

namespace RVAS_BookExchange.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Email je obavezan.")]
    [EmailAddress(ErrorMessage = "Email nije u ispravnom formatu.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lozinka je obavezna.")]
    [MinLength(6, ErrorMessage = "Lozinka mora imati najmanje 6 karaktera.")]
    public string Password { get; set; } = string.Empty;

    public List<string> FavoriteGenres { get; set; } = new();
}
