using System.ComponentModel.DataAnnotations;

namespace TodoList.Application.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "The Username is required.")]
    [MinLength(3, ErrorMessage = "The username must be at least 3 characters long.")]
    [MaxLength(15, ErrorMessage = "The username cannot exceed 15 characters.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "The Password is required.")]
    public string Password { get; set; } = string.Empty;
}


