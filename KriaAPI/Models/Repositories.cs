using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace KriaAPI.Models
{
    public class Repositorio
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public required string Description { get; set; }

        [Required]
        public required string Language { get; set; }

        public bool IsFavorite { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Valor padrão no momento da criação

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Inicializado no momento da criação

        [Required]
        public int UsuarioId { get; set; } // Chave estrangeira

        public Usuario Usuario { get; set; } = null!;// Relacionamento, garantindo que nunca seja nulo
    }
}
