using API.Extensiones;
using API.Middleware;
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
        logger.LogInformation("Iniciando inicialización de base de datos...");
        var inicializador = services.GetRequiredService<IdbInicializador>();
        await inicializador.Inicializar();
        logger.LogInformation("Inicialización de base de datos finalizada.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Falló la inicialización de la base de datos.");
        throw;
    }
}

app.MapGet("/", () => "API funcionando");

app.MapGet("/db-status", async (ApplicationDbContext db, IConfiguration config) =>
{
    var provider = db.Database.ProviderName ?? "desconocido";
    var connection = db.Database.GetDbConnection();
    await connection.OpenAsync();
    try
    {
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT EXISTS (
                SELECT 1
                FROM information_schema.tables
                WHERE table_schema = 'public'
                  AND table_name = 'AspNetUsers'
            );";

        var result = await cmd.ExecuteScalarAsync();
        var exists = result is bool value && value;

        return Results.Ok(new
        {
            provider,
            existeAspNetUsers = exists,
            databaseProviderConfig = config["DatabaseProvider"]
        });
    }
    finally
    {
        await connection.CloseAsync();
    }
});

app.MapControllers();
app.Run();
