namespace KriaAPI.DTOs;
using System.ComponentModel.DataAnnotations;


public class CreateUserDto
{

    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; }


    [Required(ErrorMessage = "Position is required.")]
    public string Position { get; set; }


    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

}

public class UserResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Position { get; set; }
    public string Email { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

