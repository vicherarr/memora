using Application.Common.Interfaces;
using Application.Features.Authentication.DTOs;
using Domain.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Authentication.Commands;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UsuarioDto>
{
    private readonly MemoraDbContext _context;
    private readonly IPasswordHashService _passwordHashService;

    public RegisterUserCommandHandler(MemoraDbContext context, IPasswordHashService passwordHashService)
    {
        _context = context;
        _passwordHashService = passwordHashService;
    }

    public async Task<UsuarioDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        try
        {

            // Check if user already exists
            var existingUser = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CorreoElectronico == request.CorreoElectronico ||
                                         u.NombreUsuario == request.NombreUsuario,
                                    cancellationToken);

        
        if (existingUser != null)
        {
            throw new ArgumentException("User with this email or username already exists");
        }

        // Create new user
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            NombreUsuario = request.NombreUsuario,
            CorreoElectronico = request.CorreoElectronico,
            ContrasenaHash = _passwordHashService.HashPassword(request.Contrasena),
            FechaCreacion = DateTime.UtcNow
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync(cancellationToken);

        return new UsuarioDto
        {
            Id = usuario.Id,
            NombreUsuario = usuario.NombreUsuario,
            CorreoElectronico = usuario.CorreoElectronico,
            FechaCreacion = usuario.FechaCreacion
        };
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}