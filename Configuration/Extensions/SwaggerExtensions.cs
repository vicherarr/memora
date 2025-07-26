using Microsoft.OpenApi.Models;

namespace Configuration.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Memora API",
                Version = "v1",
                Description = "API RESTful para la aplicación móvil Memora - Sistema de gestión de notas con archivos multimedia"
            });

            // Include XML comments for better documentation
            // Temporarily disabled to fix Swagger loading issues
            // var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            // if (File.Exists(xmlPath))
            // {
            //     options.IncludeXmlComments(xmlPath);
            // }

            // Configure Email/Password Authentication for Swagger (converted to JWT internally)
            options.AddSecurityDefinition("Email y Contraseña", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "basic",
                In = ParameterLocation.Header,
                Description = @"Autenticación con Email y Contraseña (convertida automáticamente a JWT).
                        
**INSTRUCCIONES:**
1. Haz clic en el botón 'Authorize' (candado)
2. En el campo 'Username': introduce tu **EMAIL** (ej: usuario@email.com)
3. En el campo 'Password': introduce tu contraseña
4. Haz clic en 'Authorize'

**⚠️ IMPORTANTE:** 
- El campo 'Username' requiere tu EMAIL, no tu nombre de usuario
- Ejemplo: usuario@gmail.com (no 'Juan Pérez')

**Funcionamiento:** 
- El sistema convierte automáticamente tus credenciales a un token JWT
- No necesitas manejar tokens manualmente
- La autenticación se mantiene durante toda la sesión

**Nota:** Si no tienes cuenta, regístrate primero usando el endpoint /api/autenticacion/registrar"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Email y Contraseña"
                        }
                    },
                    new string[] {}
                }
            });
        });

        return services;
    }
}