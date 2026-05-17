using System.ComponentModel.DataAnnotations;

namespace TodoList.Application.DTOs;

public class CreateUserDto
{
    [Required(ErrorMessage = "The username is required.")]
    [MinLength(3, ErrorMessage = "The username must be at least 3 characters long.")]
    [MaxLength(15, ErrorMessage = "The username cannot exceed 15 characters.")]
    public string Username { get; set; } = string.Empty;


    [Required(ErrorMessage = "The name is required.")]
    [MinLength(3, ErrorMessage = "The name must be at least 3 characters long.")]
    [MaxLength(15, ErrorMessage = "The name cannot exceed 15 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "The email is required.")]
    [EmailAddress(ErrorMessage = "The email format is invalid.")]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "The Password is required.")]
    [MinLength(8, ErrorMessage = "The password must be at least 8 characters long.")]
    public string Password { get; set; } = string.Empty;
}


