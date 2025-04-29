using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Utilities.Exceptions;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly UserBusiness _userBusiness;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(UserBusiness userBusiness, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _userBusiness = userBusiness;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Autentica a un usuario y devuelve un token JWT
        /// </summary>
        /// <param name="loginModel">Modelo con credenciales de usuario</param>
        /// <returns>Token de autenticación y datos básicos de usuario</returns>
        /// <response code="200">Autenticación exitosa</response>
        /// <response code="400">Credenciales inválidas</response>
        /// <response code="401">Usuario no autorizado</response>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginModel)
        {
            try
            {
                // Validación básica
                if (string.IsNullOrEmpty(loginModel.Username) || string.IsNullOrEmpty(loginModel.Password))
                {
                    return BadRequest(new { message = "El usuario y contraseña son requeridos" });
                }

                // Autenticar usuario contra la base de datos
                var user = await _userBusiness.AuthenticateAsync(loginModel.Username, loginModel.Password);
                
                if (user == null)
                {
                    _logger.LogWarning("Intento de inicio de sesión fallido para el usuario: {Username}", loginModel.Username);
                    return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
                }

                // Si el usuario no está activo
                if (!user.Active)
                {
                    _logger.LogWarning("Intento de inicio de sesión con una cuenta inactiva: {Username}", loginModel.Username);
                    return Unauthorized(new { message = "La cuenta está desactivada" });
                }

                // Generar token JWT
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "DefaultSecretKey123!@#$%^&*()" );
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[] 
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        // Aquí se podrían agregar claims para roles y permisos si se implementa esa funcionalidad
                    }),
                    Expires = DateTime.UtcNow.AddHours(8), // Token válido por 8 horas
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature
                    ),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"]
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                _logger.LogInformation("Inicio de sesión exitoso para el usuario: {Username}", loginModel.Username);

                // Devolver el token y la información básica del usuario
                return Ok(new
                {
                    id = user.Id,
                    username = user.Username,
                    token = tokenString,
                    // Agregar información adicional del usuario que pueda ser útil para el frontend
                    user = new
                    {
                        id = user.Id,
                        username = user.Username,
                        email = user.Email,
                        personId = user.PersonId
                        // Puedes incluir más información según se necesite
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el proceso de autenticación");
                return StatusCode(500, new { message = "Error interno del servidor durante la autenticación" });
            }
        }

        /// <summary>
        /// Valida si un token JWT es válido
        /// </summary>
        /// <returns>Estado de validez del token</returns>
        [HttpGet("validate")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            // Si llegamos aquí, el token es válido debido al atributo [Authorize]
            return Ok(new { valid = true });
        }

        /// <summary>
        /// Cierra la sesión actual (implementación del lado del servidor)
        /// </summary>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // En una implementación JWT stateless, no hay mucho que hacer en el servidor
            // El frontend simplemente elimina el token
            // Podrías implementar una lista negra de tokens si necesitas invalidación del lado del servidor

            var username = User.Identity?.Name;
            _logger.LogInformation("Cierre de sesión para el usuario: {Username}", username);

            return Ok(new { message = "Sesión cerrada exitosamente" });
        }
    }
}