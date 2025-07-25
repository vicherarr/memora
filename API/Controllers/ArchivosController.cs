using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ArchivosController : ControllerBase
{
    /// <summary>
    /// Test endpoint
    /// </summary>
    [HttpGet("test")]
    [AllowAnonymous]
    public IActionResult Test()
    {
        return Ok(new { message = "ArchivosController working", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Sube un archivo a una nota específica
    /// </summary>
    /// <param name="notaId">ID de la nota</param>
    /// <param name="archivo">Archivo a subir</param>
    [HttpPost("upload/{notaId}")]
    public async Task<IActionResult> UploadArchivo(Guid notaId, IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
        {
            return BadRequest("No se proporcionó archivo");
        }

        // Simulación simple
        await Task.Delay(1);
        
        var response = new
        {
            archivoId = Guid.NewGuid(),
            notaId = notaId,
            nombreOriginal = archivo.FileName,
            tamaño = archivo.Length,
            mensaje = "Archivo subido exitosamente"
        };

        return Created($"api/archivos/{response.archivoId}", response);
    }

    /// <summary>
    /// Obtiene información de un archivo específico
    /// </summary>
    /// <param name="id">ID del archivo</param>
    [HttpGet("{id}")]
    public IActionResult GetArchivo(Guid id)
    {
        var response = new
        {
            id = id,
            nombreOriginal = "ejemplo.jpg",
            tamaño = 1024,
            tipoMime = "image/jpeg",
            fechaSubida = DateTime.UtcNow,
            notaId = Guid.NewGuid()
        };

        return Ok(response);
    }

    /// <summary>
    /// Elimina un archivo específico
    /// </summary>
    /// <param name="id">ID del archivo a eliminar</param>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArchivo(Guid id)
    {
        // Simulación simple
        await Task.Delay(1);
        
        var response = new
        {
            archivoId = id,
            mensaje = "Archivo eliminado exitosamente"
        };

        return Ok(response);
    }

    /// <summary>
    /// Descarga el contenido binario de un archivo específico
    /// </summary>
    /// <param name="id">ID del archivo</param>
    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadArchivo(Guid id)
    {
        // Simulación simple - en la implementación real obtendría los datos de la base de datos
        await Task.Delay(1);
        
        var contenido = System.Text.Encoding.UTF8.GetBytes("Contenido simulado del archivo");
        var nombreArchivo = "archivo_descargado.txt";
        var tipoMime = "text/plain";

        return File(contenido, tipoMime, nombreArchivo);
    }
}