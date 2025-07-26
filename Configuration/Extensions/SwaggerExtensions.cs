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

            // Configure OAuth2 Password Flow for Swagger with immediate validation
            options.AddSecurityDefinition("Email y Contraseña", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Password = new OpenApiOAuthFlow
                    {
                        TokenUrl = new Uri("/api/autenticacion/swagger-auth", UriKind.Relative),
                        Scopes = new Dictionary<string, string>
                        {
                            {"api", "Access to Memora API"}
                        }
                    }
                },
                Description = @"Autenticación con Email y Contraseña (OAuth2 Password Flow).
                        
**INSTRUCCIONES:**
1. Haz clic en el botón 'Authorize' (candado)
2. En el campo 'Username': introduce tu **EMAIL** (ej: usuario@email.com)
3. En el campo 'Password': introduce tu contraseña
4. **IMPORTANTE**: Deja los campos Client ID y Client Secret VACÍOS
5. Marca el checkbox 'api' si no está marcado
6. Haz clic en 'Authorize'

**⚠️ VALIDACIÓN INMEDIATA:** 
- Las credenciales se validan AL MOMENTO de hacer clic en 'Authorize'
- Si son incorrectas, el candado NO se cerrará y verás un error
- Si son correctas, el candado se cerrará automáticamente

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
                    new string[] { "api" }
                }
            });
        });

        return services;
    }
}