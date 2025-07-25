using FluentValidation;

namespace Application.Features.Archivos.Commands;

public class UploadArchivoCommandValidator : AbstractValidator<UploadArchivoCommand>
{
    private readonly string[] _allowedImageTypes = { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
    private readonly string[] _allowedVideoTypes = { "video/mp4", "video/mov", "video/avi", "video/wmv", "video/webm" };
    private const long MaxFileSize = 50 * 1024 * 1024; // 50MB

    public UploadArchivoCommandValidator()
    {
        RuleFor(x => x.NotaId)
            .NotEmpty()
            .WithMessage("El ID de la nota es requerido.");

        RuleFor(x => x.UsuarioId)
            .NotEmpty()
            .WithMessage("El ID del usuario es requerido.");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("El nombre del archivo es requerido.")
            .MaximumLength(255)
            .WithMessage("El nombre del archivo no puede exceder 255 caracteres.");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("El tipo de contenido es requerido.")
            .Must(BeValidContentType)
            .WithMessage("Tipo de archivo no soportado. Solo se permiten imágenes (JPEG, PNG, GIF, WebP) y videos (MP4, MOV, AVI, WMV, WebM).");

        RuleFor(x => x.FileData)
            .NotEmpty()
            .WithMessage("Los datos del archivo son requeridos.")
            .Must(x => x.Length <= MaxFileSize)
            .WithMessage($"El archivo no puede exceder {MaxFileSize / (1024 * 1024)}MB.");
    }

    private bool BeValidContentType(string contentType)
    {
        return _allowedImageTypes.Contains(contentType.ToLower()) || 
               _allowedVideoTypes.Contains(contentType.ToLower());
    }
}