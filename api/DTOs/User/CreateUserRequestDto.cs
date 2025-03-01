using System.ComponentModel.DataAnnotations;

namespace api.DTOs.User;

public class CreateUserRequestDto
{   [Required]
    public String Name { get; set; } = string.Empty;

    [Required]
    public string EmailAddress { get; set; } = string.Empty;
}