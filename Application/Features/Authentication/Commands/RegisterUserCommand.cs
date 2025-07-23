using Application.Features.Authentication.DTOs;
using MediatR;

namespace Application.Features.Authentication.Commands;

public class RegisterUserCommand : IRequest<UsuarioDto>
{
    public string NombreUsuario { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
}