using API.Extensiones;
using API.Middleware;
using Data;
using Data.Inicializador;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AgregarServiciosAplicacion(builder.Configuration);
builder.Services.AgregarServiciosIdentidad(builder.Configuration);
builder.Services.AddScoped<IdbInicializador, DbInicializador>();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseStatusCodePagesWithReExecute("/errores/{0}");
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors(x => x.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader());
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("Iniciando proceso de inicialización de base de datos...");
        var inicializador = services.GetRequiredService<IdbInicializador>();
        await inicializador.Inicializar();
        logger.LogInformation("Inicialización de base de datos completada correctamente.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error ocurrido al inicializar la base de datos");
    }
}

app.MapGet("/", () => Results.Ok("API funcionando"));
app.MapGet("/db-status", async (ApplicationDbContext db) =>
{
    var provider = db.Database.ProviderName ?? "desconocido";
    var existeAspNetUsers = false;
    try
    {
        existeAspNetUsers = await db.Database
            .SqlQueryRaw<bool>("SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'AspNetUsers')")
            .SingleAsync();
    }
    catch
    {
    }

    return Results.Ok(new
    {
        provider,
        existeAspNetUsers,
        databaseProviderConfig = builder.Configuration["DatabaseProvider"]
    });
});
app.MapControllers();

app.Run();
