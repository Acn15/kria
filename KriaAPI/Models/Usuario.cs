using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KriaAPI.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Position { get; set; }

        [Required]
        public required string Email { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Inicializado no momento da criação

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Inicializado no momento da criação

        public ICollection<Repositorio> Repositorios { get; set; } = new List<Repositorio>();

    }
}