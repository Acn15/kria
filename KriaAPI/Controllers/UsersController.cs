using Microsoft.AspNetCore.Mvc;
using KriaAPI.Models;
using KriaAPI.Services;
using KriaAPI.DTOs;
using Microsoft.Extensions.Logging;

namespace KriaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(UsuarioService usuarioService, ILogger<UsuariosController> logger)
        {
            _usuarioService = usuarioService;
            _logger = logger;
        }

        // GET: api/usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all users.");
            var usuarios = await _usuarioService.GetAllAsync();

            _logger.LogInformation("Fetched {Count} users.", usuarios.Count());
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDTO>> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching user with ID {Id}.", id);

            var usuario = await _usuarioService.GetByIdAsync(id);
            if (usuario == null)
            {
                _logger.LogWarning("User with ID {Id} not found.", id);
                return NotFound();
            }

            var output = new UserResponseDTO
            {
                Id = usuario.Id,
                Name = usuario.Name,
                Position = usuario.Position,
                Email = usuario.Email,
                CreatedAt = usuario.CreatedAt,
                UpdatedAt = usuario.UpdatedAt
            };

            _logger.LogInformation("Fetched user with ID {Id}.", id);
            return Ok(output);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserResponseDTO>> GetByEmailAsync(string email)
        {
            _logger.LogInformation("Fetching user with email {Email}.", email);

            var usuario = await _usuarioService.GetByEmailAsync(email);
            if (usuario == null)
            {
                _logger.LogWarning("User with email {Email} not found.", email);
                return NotFound();
            }

            var output = new UserResponseDTO
            {
                Id = usuario.Id,
                Name = usuario.Name,
                Position = usuario.Position,
                Email = usuario.Email,
                CreatedAt = usuario.CreatedAt,
                UpdatedAt = usuario.UpdatedAt
            };

            _logger.LogInformation("Fetched user with email {Email}.", email);
            return Ok(output);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CreateUserDto input)
        {
            _logger.LogInformation("Starting to add a new user.");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                _logger.LogWarning("Validation failed for new user: {Errors}.", string.Join(", ", errors));
                return BadRequest(new { message = "Validation failed", errors });
            }

            try
            {
                var user = new Usuario
                {
                    Name = input.Name,
                    Position = input.Position,
                    Email = input.Email,
                };

                var newUser = await _usuarioService.AddAsync(user);

                var output = new UserResponseDTO
                {
                    Id = newUser.Id,
                    Name = newUser.Name,
                    Position = newUser.Position,
                    Email = newUser.Email,
                    CreatedAt = newUser.CreatedAt,
                    UpdatedAt = newUser.UpdatedAt
                };

                _logger.LogInformation("User with ID {Id} created successfully.", newUser.Id);
                return Created("", output);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Failed to add new user: {Message}.", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding a new user.");
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
