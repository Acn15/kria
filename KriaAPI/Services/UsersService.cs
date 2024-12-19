using KriaAPI.Data;
using KriaAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KriaAPI.Services
{
    public class UsuarioService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(AppDbContext context, ILogger<UsuarioService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all users from the database.");

            try
            {
                var usuarios = await _context.Usuarios.ToListAsync();
                _logger.LogInformation("Successfully fetched {Count} users.", usuarios.Count);
                return usuarios;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all users.");
                throw;
            }
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching user with ID {Id}.", id);

            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    _logger.LogWarning("User with ID {Id} not found.", id);
                }
                else
                {
                    _logger.LogInformation("Successfully fetched user with ID {Id}.", id);
                }
                return usuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the user with ID {Id}.", id);
                throw;
            }
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            _logger.LogInformation("Fetching user with email {Email}.", email);

            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
                if (usuario == null)
                {
                    _logger.LogWarning("User with email {Email} not found.", email);
                }
                else
                {
                    _logger.LogInformation("Successfully fetched user with email {Email}.", email);
                }
                return usuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the user with email {Email}.", email);
                throw;
            }
        }

        public async Task<Usuario> AddAsync(Usuario usuario)
        {
            _logger.LogInformation("Starting to add a new user.");

            try
            {
                var usuarioExistente = await _context.Usuarios
                                                     .FirstOrDefaultAsync(u => u.Email == usuario.Email);

                if (usuarioExistente != null)
                {
                    _logger.LogWarning("A user with the email {Email} already exists.", usuario.Email);
                    throw new InvalidOperationException("Já existe um usuário com este e-mail.");
                }

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User with ID {Id} added successfully.", usuario.Id);
                return usuario;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation failed while adding a new user: {Message}.", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding a new user.");
                throw;
            }
        }
    }
}
