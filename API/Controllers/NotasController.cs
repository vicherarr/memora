using Application.Features.Notas.Commands;
using Application.Features.Notas.DTOs;
using Application.Features.Notas.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotasController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene las notas del usuario con paginación
    /// </summary>
    /// <param name="pageNumber">Número de página (por defecto 1)</param>
    /// <param name="pageSize">Tamaño de página (por defecto 10, máximo 100)</param>
    /// <returns>Lista paginada de notas del usuario</returns>
    [HttpGet]
    public async Task<ActionResult<PaginatedNotasDto>> GetUserNotas(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            
            var query = new GetUserNotasQuery
            {
                UsuarioId = usuarioId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    /// <summary>
    /// Obtiene una nota específica por ID
    /// </summary>
    /// <param name="id">ID de la nota</param>
    /// <returns>Detalles de la nota incluyendo número de archivos adjuntos</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<NotaDetailDto>> GetNotaById(Guid id)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            
            var query = new GetNotaByIdQuery
            {
                NotaId = id,
                UsuarioId = usuarioId
            };

            var result = await _mediator.Send(query);
            
            if (result == null)
            {
                return NotFound(new { message = "Nota no encontrada" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    /// <summary>
    /// Crea una nueva nota
    /// </summary>
    /// <param name="request">Datos de la nueva nota</param>
    /// <returns>La nota creada</returns>
    [HttpPost]
    public async Task<ActionResult<NotaDto>> CreateNota(CreateNotaDto request)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            
            var command = new CreateNotaCommand
            {
                Titulo = request.Titulo,
                Contenido = request.Contenido,
                UsuarioId = usuarioId
            };

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetNotaById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    /// <summary>
    /// Actualiza una nota existente
    /// </summary>
    /// <param name="id">ID de la nota a actualizar</param>
    /// <param name="request">Nuevos datos de la nota</param>
    /// <returns>La nota actualizada</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<NotaDto>> UpdateNota(Guid id, UpdateNotaDto request)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            
            var command = new UpdateNotaCommand
            {
                NotaId = id,
                Titulo = request.Titulo,
                Contenido = request.Contenido,
                UsuarioId = usuarioId
            };

            var result = await _mediator.Send(command);
            
            if (result == null)
            {
                return NotFound(new { message = "Nota no encontrada" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    /// <summary>
    /// Elimina una nota
    /// </summary>
    /// <param name="id">ID de la nota a eliminar</param>
    /// <returns>Confirmación de eliminación</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteNota(Guid id)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            
            var command = new DeleteNotaCommand
            {
                NotaId = id,
                UsuarioId = usuarioId
            };

            var result = await _mediator.Send(command);
            
            if (!result)
            {
                return NotFound(new { message = "Nota no encontrada" });
            }

            return Ok(new { message = "Nota eliminada exitosamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    private Guid GetCurrentUserId()
    {
        // Método 1: Intentar con claims estándar
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                         ?? User.FindFirst("nameid")?.Value
                         ?? User.FindFirst("sub")?.Value;
        
        if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        
        // Método 2: Parsear JWT directamente del header
        try
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader["Bearer ".Length..];
                var parts = token.Split('.');
                if (parts.Length == 3)
                {
                    var payload = parts[1];
                    // Agregar padding si es necesario
                    while (payload.Length % 4 != 0)
                        payload += "=";
                    
                    var jsonBytes = Convert.FromBase64String(payload);
                    var jsonString = Encoding.UTF8.GetString(jsonBytes);
                    var tokenData = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString);
                    
                    if (tokenData != null && tokenData.TryGetValue("nameid", out var nameId))
                    {
                        var userIdString = nameId.ToString();
                        if (Guid.TryParse(userIdString, out var parsedUserId))
                        {
                            return parsedUserId;
                        }
                    }
                }
            }
        }
        catch
        {
            // Si falla el parsing directo, continuar con error
        }
        
        var availableClaims = string.Join(", ", User.Claims.Select(c => c.Type));
        throw new UnauthorizedAccessException($"No se pudo obtener ID de usuario. Claims: {availableClaims}");
    }
}