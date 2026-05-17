using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RVAS_BookExchange.ViewModels;

public class BookEditViewModel
{
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "Naslov knjige je obavezan.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Autor je obavezan.")]
    public string Author { get; set; } = string.Empty;

    [Required(ErrorMessage = "Žanr je obavezan.")]
    public string Genre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Stanje knjige je obavezno.")]
    public string Condition { get; set; } = string.Empty;

    [Required(ErrorMessage = "Opis knjige je obavezan.")]
    public string Description { get; set; } = string.Empty;

    public string ConditionDescription { get; set; } = string.Empty;

    [Required(ErrorMessage = "Grad/lokacija je obavezna.")]
    public string City { get; set; } = string.Empty;

    public string? ExistingImagePath { get; set; }

    public IFormFile? NewImage { get; set; }
}