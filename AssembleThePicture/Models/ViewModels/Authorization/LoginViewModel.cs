using System.ComponentModel.DataAnnotations;

namespace AssembleThePicture.Models.ViewModels.Authorization;

public class LoginViewModel
{
    [Required]
    [Display(Name = "Name")]
    public string Name { get; set; }
    
    [Required]
    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
}