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

            // Configure JWT authentication for Swagger with improved UX
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = @"JWT Authorization header using the Bearer scheme. 
                        
**PASOS PARA AUTENTICARSE:**
1. Ejecuta el endpoint POST /api/autenticacion/swagger-auth con email y contraseña
2. Copia el token de la respuesta
3. Haz clic en 'Authorize' arriba
4. Pega el token (sin 'Bearer ') y haz clic en 'Authorize'

**Ejemplo:** Si el token es 'abc123', introduce solo: abc123"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        return services;
    }
}