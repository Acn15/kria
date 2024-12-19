using KriaAPI.Data;
using KriaAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KriaAPI.Services
{
    public class RepositorioService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RepositorioService> _logger;

        public RepositorioService(AppDbContext context, ILogger<RepositorioService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Repositorio>> GetAllByUsuarioIdAsync(int usuarioId)
        {
            _logger.LogInformation("Fetching all repositories for user ID {UsuarioId}.", usuarioId);

            try
            {
                var repositorios = await _context.Repositorios
                    .Where(r => r.UsuarioId == usuarioId)
                    .ToListAsync();

                _logger.LogInformation("Successfully fetched {Count} repositories for user ID {UsuarioId}.", repositorios.Count, usuarioId);
                return repositorios;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching repositories for user ID {UsuarioId}.", usuarioId);
                throw;
            }
        }

        public async Task<IEnumerable<Repositorio>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all repositories from the database.");

            try
            {
                var repositorios = await _context.Repositorios
                    .Include(r => r.Usuario)
                    .ToListAsync();

                _logger.LogInformation("Successfully fetched {Count} repositories.", repositorios.Count);
                return repositorios;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all repositories.");
                throw;
            }
        }

        public async Task<Repositorio?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching repository with ID {Id}.", id);

            try
            {
                var repositorio = await _context.Repositorios.FindAsync(id);

                if (repositorio == null)
                    _logger.LogWarning("Repository with ID {Id} not found.", id);

                return repositorio;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching repository with ID {Id}.", id);
                throw;
            }
        }

        public async Task<Repositorio> AddAsync(Repositorio repositorio)
        {
            _logger.LogInformation("Adding a new repository for user ID {UsuarioId}.", repositorio.UsuarioId);

            try
            {
                var usuarioExistente = await _context.Usuarios.FindAsync(repositorio.UsuarioId);
                if (usuarioExistente == null)
                {
                    _logger.LogWarning("User with ID {UsuarioId} does not exist.", repositorio.UsuarioId);
                    throw new InvalidOperationException("O usuário com o ID especificado não existe.");
                }

                var repositorioExistente = await _context.Repositorios
                    .FirstOrDefaultAsync(r => r.Name == repositorio.Name && r.UsuarioId == repositorio.UsuarioId);

                if (repositorioExistente != null)
                {
                    _logger.LogWarning("A repository with the same name already exists for user ID {UsuarioId}.", repositorio.UsuarioId);
                    throw new InvalidOperationException("Já existe um repositório com este nome para este usuário.");
                }

                _context.Repositorios.Add(repositorio);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Repository added successfully with ID {Id}.", repositorio.Id);
                return repositorio;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new repository.");
                throw;
            }
        }

        public async Task<IEnumerable<Repositorio>> GetFavoritesByUsuarioIdAsync(int usuarioId)
        {
            _logger.LogInformation("Fetching favorite repositories for user ID {UsuarioId}.", usuarioId);

            try
            {
                var favoriteRepos = await _context.Repositorios
                    .Where(r => r.UsuarioId == usuarioId && r.IsFavorite)
                    .ToListAsync();

                _logger.LogInformation("Successfully fetched {Count} favorite repositories for user ID {UsuarioId}.", favoriteRepos.Count, usuarioId);
                return favoriteRepos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching favorite repositories for user ID {UsuarioId}.", usuarioId);
                throw;
            }
        }

        public async Task<bool> UpdateIsFavoriteAsync(int id, bool isFavorite)
        {
            _logger.LogInformation("Updating favorite status for repository ID {Id}.", id);

            try
            {
                var repository = await _context.Repositorios.FindAsync(id);

                if (repository == null)
                {
                    _logger.LogWarning("Repository with ID {Id} not found.", id);
                    return false;
                }

                repository.IsFavorite = isFavorite;
                repository.UpdatedAt = DateTime.UtcNow;

                _context.Repositorios.Update(repository);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated favorite status for repository ID {Id}.", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating favorite status for repository ID {Id}.", id);
                throw;
            }
        }

        public async Task<Repositorio> UpdateRepositoryAsync(Repositorio repository)
        {
            _logger.LogInformation("Updating repository ID {Id}.", repository.Id);

            try
            {
                repository.UpdatedAt = DateTime.UtcNow;
                _context.Repositorios.Update(repository);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Repository ID {Id} updated successfully.", repository.Id);
                return repository;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating repository ID {Id}.", repository.Id);
                throw;
            }
        }
    }
}
