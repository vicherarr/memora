using Application.Common.Interfaces;
using Application.Features.Authentication.DTOs;
using Domain.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Authentication.Commands;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterResponseDto>
{
    private readonly MemoraDbContext _context;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IJwtTokenService _jwtTokenService;

    public RegisterUserCommandHandler(MemoraDbContext context, IPasswordHashService passwordHashService, IJwtTokenService jwtTokenService)
    {
        _context = context;
        _passwordHashService = passwordHashService;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<RegisterResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
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

            // Generate JWT token
            var token = _jwtTokenService.GenerateToken(usuario);
            var expiresAt = _jwtTokenService.GetExpirationTime();

            return new RegisterResponseDto
            {
                Token = token,
                Usuario = new UsuarioDto
                {
                    Id = usuario.Id,
                    NombreUsuario = usuario.NombreUsuario,
                    CorreoElectronico = usuario.CorreoElectronico,
                    FechaCreacion = usuario.FechaCreacion
                },
                ExpiresAt = expiresAt
            };
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}