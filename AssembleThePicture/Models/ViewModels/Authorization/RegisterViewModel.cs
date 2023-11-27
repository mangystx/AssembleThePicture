using System.ComponentModel.DataAnnotations;

namespace AssembleThePicture.Models.ViewModels.Authorization;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Name")]
    public string Name { get; set; }
    
    [Required]
    [Display(Name = "Password")]
    [MinLength(8)]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Display(Name = "Confirm password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "passwords do not match")]
    public string ConfirmPassword { get; set; }
    
    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
}