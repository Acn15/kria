namespace KriaAPI.DTOs;
using System.ComponentModel.DataAnnotations;


public class CreateRepositoryDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Language { get; set; }

    public bool IsFavorite { get; set; } = false;

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int UsuarioId { get; set; }
}

public class RepositoryResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Language { get; set; }

    public bool IsFavorite { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int UsuarioId { get; set; }
}

public class UpdateFavoriteRepositoryDTO
{
    public bool IsFavorite { get; set; }
}

public class UpdateRepositoryDTO
{
    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Description is required.")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Language is required.")]
    public string Language { get; set; }
}


