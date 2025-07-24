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
            return StatusCode(500, new { message = "An internal server error occurred" });
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
}