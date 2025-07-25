using Application.Features.Notas.DTOs;
using Application.Features.Notas.Queries;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Memora.Tests.UnitTests;

public class NotasHandlerUnitTests
{
    private MemoraDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<MemoraDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new MemoraDbContext(options);
    }

    [Fact]
    public async Task GetUserNotasQueryHandler_ShouldReturnUserNotes_WhenUserHasNotes()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        
        var usuarioId = Guid.NewGuid();
        var otroUsuarioId = Guid.NewGuid();
        
        var usuario = new Usuario
        {
            Id = usuarioId,
            NombreUsuario = "testuser",
            CorreoElectronico = "test@example.com",
            ContrasenaHash = "hashedpassword",
            FechaCreacion = DateTime.UtcNow
        };

        var otroUsuario = new Usuario
        {
            Id = otroUsuarioId,
            NombreUsuario = "otheruser",
            CorreoElectronico = "other@example.com",
            ContrasenaHash = "hashedpassword",
            FechaCreacion = DateTime.UtcNow
        };

        var notasDelUsuario = new List<Nota>
        {
            new Nota
            {
                Id = Guid.NewGuid(),
                Titulo = "Nota 1",
                Contenido = "Contenido 1",
                FechaCreacion = DateTime.UtcNow.AddDays(-2),
                FechaModificacion = DateTime.UtcNow.AddDays(-2),
                UsuarioId = usuarioId,
                Usuario = usuario
            },
            new Nota
            {
                Id = Guid.NewGuid(),
                Titulo = "Nota 2",
                Contenido = "Contenido 2",
                FechaCreacion = DateTime.UtcNow.AddDays(-1),
                FechaModificacion = DateTime.UtcNow.AddDays(-1),
                UsuarioId = usuarioId,
                Usuario = usuario
            }
        };

        var notaDeOtroUsuario = new Nota
        {
            Id = Guid.NewGuid(),
            Titulo = "Nota de otro usuario",
            Contenido = "No debería aparecer",
            FechaCreacion = DateTime.UtcNow,
            FechaModificacion = DateTime.UtcNow,
            UsuarioId = otroUsuarioId,
            Usuario = otroUsuario
        };

        context.Usuarios.AddRange(usuario, otroUsuario);
        context.Notas.AddRange(notasDelUsuario);
        context.Notas.Add(notaDeOtroUsuario);
        await context.SaveChangesAsync();

        var handler = new GetUserNotasQueryHandler(context);
        var query = new GetUserNotasQuery
        {
            UsuarioId = usuarioId,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Notas.Should().HaveCount(2);
        result.Notas.Should().AllSatisfy(nota => nota.UsuarioId.Should().Be(usuarioId));
        result.TotalCount.Should().Be(2);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        
        // Verificar que están ordenadas por fecha de modificación descendente
        result.Notas.First().Titulo.Should().Be("Nota 2");
        result.Notas.Last().Titulo.Should().Be("Nota 1");
    }

    [Fact]
    public async Task GetUserNotasQueryHandler_ShouldReturnEmptyList_WhenUserHasNoNotes()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        
        var usuarioId = Guid.NewGuid();
        var usuario = new Usuario
        {
            Id = usuarioId,
            NombreUsuario = "testuser",
            CorreoElectronico = "test@example.com",
            ContrasenaHash = "hashedpassword",
            FechaCreacion = DateTime.UtcNow
        };

        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var handler = new GetUserNotasQueryHandler(context);
        var query = new GetUserNotasQuery
        {
            UsuarioId = usuarioId,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Notas.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task GetUserNotasQueryHandler_ShouldRespectPagination_WhenMultipleNotesExist()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        
        var usuarioId = Guid.NewGuid();
        var usuario = new Usuario
        {
            Id = usuarioId,
            NombreUsuario = "testuser",
            CorreoElectronico = "test@example.com",
            ContrasenaHash = "hashedpassword",
            FechaCreacion = DateTime.UtcNow
        };

        var notas = new List<Nota>();
        for (int i = 1; i <= 15; i++)
        {
            notas.Add(new Nota
            {
                Id = Guid.NewGuid(),
                Titulo = $"Nota {i}",
                Contenido = $"Contenido {i}",
                FechaCreacion = DateTime.UtcNow.AddDays(-i),
                FechaModificacion = DateTime.UtcNow.AddDays(-i),
                UsuarioId = usuarioId,
                Usuario = usuario
            });
        }

        context.Usuarios.Add(usuario);
        context.Notas.AddRange(notas);
        await context.SaveChangesAsync();

        var handler = new GetUserNotasQueryHandler(context);
        var query = new GetUserNotasQuery
        {
            UsuarioId = usuarioId,
            PageNumber = 2,
            PageSize = 5
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Notas.Should().HaveCount(5);
        result.TotalCount.Should().Be(15);
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(5);
    }
}