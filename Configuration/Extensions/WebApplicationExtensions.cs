namespace Configuration.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureMiddlewarePipeline(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Memora API v1");
            c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
        });

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}