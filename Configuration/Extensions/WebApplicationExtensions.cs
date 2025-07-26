using API.Middleware;

namespace Configuration.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureMiddlewarePipeline(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        
        // Only enable Swagger in Development environment for security
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Memora API v1");
                c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
            });
            
            // Add Basic Auth to JWT conversion middleware for Swagger (Development only)
            app.UseMiddleware<BasicAuthToJwtMiddleware>();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}