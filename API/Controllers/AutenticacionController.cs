using Application.Features.Authentication.Commands;
using Application.Features.Authentication.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutenticacionController : ControllerBase
{
    private readonly IMediator _mediator;

    public AutenticacionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("registrar")]
    public async Task<ActionResult<RegisterResponseDto>> Register(RegisterUserDto request)
    {
        try
        {
            var command = new RegisterUserCommand
            {
                NombreUsuario = request.NombreUsuario,
                CorreoElectronico = request.CorreoElectronico,
                Contrasena = request.Contrasena
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Registration error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return StatusCode(500, new { message = "An internal server error occurred", details = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginUserDto request)
    {
        try
        {
            var command = new LoginUserCommand
            {
                CorreoElectronico = request.CorreoElectronico,
                Contrasena = request.Contrasena
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }

    /// <summary>
    /// Endpoint de autenticación simplificado para Swagger UI
    /// </summary>
    /// <remarks>
    /// **ÚSALO PARA AUTENTICARTE EN SWAGGER:**
    /// 
    /// 1. Introduce tu email y contraseña
    /// 2. Ejecuta este endpoint
    /// 3. Copia el token de la respuesta
    /// 4. Haz clic en "Authorize" arriba
    /// 5. Pega el token (sin 'Bearer ') y autoriza
    /// 
    /// **Usuarios de prueba:**
    /// - Email: test@test.com / Contraseña: Test123456
    /// - Email: dockertest@test.com / Contraseña: Test123456
    /// </remarks>
    /// <param name="request">Credenciales de usuario</param>
    /// <returns>Token JWT para usar en Authorization</returns>
    [HttpPost("swagger-auth")]
    public async Task<ActionResult<SwaggerTokenResponseDto>> SwaggerAuth(SwaggerAuthDto request)
    {
        try
        {
            var command = new LoginUserCommand
            {
                CorreoElectronico = request.CorreoElectronico,
                Contrasena = request.Contrasena
            };

            var result = await _mediator.Send(command);
            return Ok(new SwaggerTokenResponseDto { Token = result.Token });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An internal server error occurred" });
        }
    }
}