using Microsoft.AspNetCore.Mvc;
using KriaAPI.Models;
using KriaAPI.Services;
using KriaAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KriaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RepositoriosController : ControllerBase
    {
        private readonly RepositorioService _repositorioService;
        private readonly ILogger<RepositoriosController> _logger;

        public RepositoriosController(RepositorioService repositorioService, ILogger<RepositoriosController> logger)
        {
            _repositorioService = repositorioService;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<RepositoryResponseDTO>>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all repositories.");
            var repositorios = await _repositorioService.GetAllAsync();
            _logger.LogInformation("Fetched {Count} repositories.", repositorios.Count());

            var output = repositorios.Select(r => new RepositoryResponseDTO
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Language = r.Language,
                UsuarioId = r.UsuarioId,
                IsFavorite = r.IsFavorite,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            });

            return Ok(output);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RepositoryResponseDTO>> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching repository with ID {Id}.", id);
            var repository = await _repositorioService.GetByIdAsync(id);

            if (repository == null)
            {
                _logger.LogWarning("Repository with ID {Id} not found.", id);
                return NotFound();
            }

            var output = new RepositoryResponseDTO
            {
                Id = repository.Id,
                Name = repository.Name,
                Description = repository.Description,
                Language = repository.Language,
                IsFavorite = repository.IsFavorite,
                UsuarioId = repository.UsuarioId,
                CreatedAt = repository.CreatedAt,
                UpdatedAt = repository.UpdatedAt
            };

            return Ok(output);
        }

        [HttpGet("user/{id}")]
        public async Task<ActionResult<IEnumerable<RepositoryResponseDTO>>> GetAllByUsuarioIdAsync(int id)
        {
            _logger.LogInformation("Fetching all repositories for user ID {Id}.", id);
            var repositories = await _repositorioService.GetAllByUsuarioIdAsync(id);

            if (!repositories.Any())
            {
                _logger.LogWarning("No repositories found for user ID {Id}.", id);
                return NotFound();
            }

            var output = repositories.Select(r => new RepositoryResponseDTO
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Language = r.Language,
                IsFavorite = r.IsFavorite,
                UsuarioId = r.UsuarioId,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            });

            return Ok(output);
        }

        [HttpGet("user/{id}/favorites")]
        public async Task<ActionResult<IEnumerable<RepositoryResponseDTO>>> GetFavoritesByUsuarioIdAsync(int id)
        {
            _logger.LogInformation("Fetching favorite repositories for user ID {Id}.", id);
            var favoriteRepositories = await _repositorioService.GetFavoritesByUsuarioIdAsync(id);

            if (!favoriteRepositories.Any())
            {
                _logger.LogWarning("No favorite repositories found for user ID {Id}.", id);
                return NotFound();
            }

            var output = favoriteRepositories.Select(r => new RepositoryResponseDTO
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Language = r.Language,
                IsFavorite = r.IsFavorite,
                UsuarioId = r.UsuarioId,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            });

            return Ok(output);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CreateRepositoryDTO input)
        {
            try
            {
                _logger.LogInformation("Adding new repository.");
                var repository = new Repositorio
                {
                    Name = input.Name,
                    Description = input.Description,
                    Language = input.Language,
                    UsuarioId = input.UsuarioId,
                    IsFavorite = input.IsFavorite,
                    UpdatedAt = input.UpdatedAt ?? DateTime.UtcNow
                };

                var newRepository = await _repositorioService.AddAsync(repository);

                var output = new RepositoryResponseDTO
                {
                    Id = newRepository.Id,
                    Name = newRepository.Name,
                    Description = newRepository.Description,
                    Language = newRepository.Language,
                    UsuarioId = newRepository.UsuarioId,
                    IsFavorite = newRepository.IsFavorite,
                    CreatedAt = newRepository.CreatedAt,
                    UpdatedAt = newRepository.UpdatedAt
                };

                _logger.LogInformation("Repository added with ID {Id}.", newRepository.Id);
                return Created("", output);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Failed to add repository: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding a repository.");
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpPatch("{id}/favorite")]
        public async Task<IActionResult> UpdateFavoriteStatusAsync(int id, [FromBody] UpdateFavoriteRepositoryDTO input)
        {
            if (input == null)
                return BadRequest(new { message = "Request body cannot be null." });

            try
            {
                _logger.LogInformation("Updating favorite status for repository ID {Id}.", id);
                var updated = await _repositorioService.UpdateIsFavoriteAsync(id, input.IsFavorite);

                if (!updated)
                {
                    _logger.LogWarning("Repository with ID {Id} not found.", id);
                    return NotFound(new { message = "Repository not found." });
                }

                var updatedRepository = await _repositorioService.GetByIdAsync(id);

                if (updatedRepository == null)
                    return StatusCode(500, new { message = "Failed to retrieve updated repository." });

                var output = new RepositoryResponseDTO
                {
                    Id = updatedRepository.Id,
                    Name = updatedRepository.Name,
                    Description = updatedRepository.Description,
                    Language = updatedRepository.Language,
                    IsFavorite = updatedRepository.IsFavorite,
                    UsuarioId = updatedRepository.UsuarioId,
                    CreatedAt = updatedRepository.CreatedAt,
                    UpdatedAt = updatedRepository.UpdatedAt
                };

                _logger.LogInformation("Updated favorite status for repository ID {Id}.", id);
                return Ok(output);
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogWarning("Concurrency issue occurred while updating repository ID {Id}.", id);
                return Conflict(new { message = "A concurrency error occurred. Please try again." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating repository ID {Id}.", id);
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRepositoryAsync(int id, [FromBody] UpdateRepositoryDTO input)
        {
            if (input == null)
                return BadRequest(new { message = "Request body cannot be null." });

            try
            {
                _logger.LogInformation("Updating repository ID {Id}.", id);
                var existingRepository = await _repositorioService.GetByIdAsync(id);

                if (existingRepository == null)
                {
                    _logger.LogWarning("Repository with ID {Id} not found.", id);
                    return NotFound(new { message = "Repository not found." });
                }

                existingRepository.Name = input.Name ?? existingRepository.Name;
                existingRepository.Description = input.Description ?? existingRepository.Description;
                existingRepository.Language = input.Language ?? existingRepository.Language;
                existingRepository.UpdatedAt = DateTime.UtcNow;

                var updatedRepository = await _repositorioService.UpdateRepositoryAsync(existingRepository);

                var output = new RepositoryResponseDTO
                {
                    Id = updatedRepository.Id,
                    Name = updatedRepository.Name,
                    Description = updatedRepository.Description,
                    Language = updatedRepository.Language,
                    IsFavorite = updatedRepository.IsFavorite,
                    UsuarioId = updatedRepository.UsuarioId,
                    CreatedAt = updatedRepository.CreatedAt,
                    UpdatedAt = updatedRepository.UpdatedAt
                };

                _logger.LogInformation("Repository ID {Id} updated successfully.", id);
                return Ok(output);
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogWarning("Concurrency issue occurred while updating repository ID {Id}.", id);
                return Conflict(new { message = "A concurrency error occurred. Please try again." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating repository ID {Id}.", id);
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
