using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entidades;
using Npgsql;
using System.Data.Common;

namespace Data.Inicializador
{
    public class DbInicializador : IdbInicializador
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<UsuarioAplicacion> _userManager;
        private readonly RoleManager<RolAplicacion> _roleManager;
        private readonly ILogger<DbInicializador> _logger;

        public DbInicializador(
            ApplicationDbContext db,
            UserManager<UsuarioAplicacion> userManager,
            RoleManager<RolAplicacion> roleManager,
            ILogger<DbInicializador> logger)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task Inicializar()
        {
            var providerName = _db.Database.ProviderName ?? string.Empty;
            _logger.LogInformation("Iniciando inicialización de base de datos. Provider: {Provider}", providerName);

            if (providerName.Contains("Npgsql", StringComparison.OrdinalIgnoreCase))
            {
                await InicializarPostgreSqlAsync();
            }
            else
            {
                await InicializarSqlServerAsync();
            }

            await CrearDatosSemillaAsync();
            _logger.LogInformation("Inicialización de base de datos completada correctamente.");
        }

        private async Task InicializarPostgreSqlAsync()
        {
            await _db.Database.OpenConnectionAsync();
            try
            {
                var existsBefore = await ExisteTablaAspNetUsersAsync(_db.Database.GetDbConnection());
                _logger.LogInformation("Estado inicial PostgreSQL. Existe AspNetUsers: {Exists}", existsBefore);

                if (!existsBefore)
                {
                    _logger.LogInformation("AspNetUsers no existe. Ejecutando EnsureCreatedAsync para PostgreSQL...");
                    await _db.Database.EnsureCreatedAsync();
                }

                var existsAfter = await ExisteTablaAspNetUsersAsync(_db.Database.GetDbConnection());
                _logger.LogInformation("Estado final PostgreSQL. Existe AspNetUsers: {Exists}", existsAfter);

                if (!existsAfter)
                {
                    throw new InvalidOperationException("No se pudo crear la tabla AspNetUsers en PostgreSQL.");
                }
            }
            finally
            {
                await _db.Database.CloseConnectionAsync();
            }
        }

        private async Task InicializarSqlServerAsync()
        {
            if ((await _db.Database.GetPendingMigrationsAsync()).Any())
            {
                _logger.LogInformation("Aplicando migraciones pendientes en SQL Server...");
                await _db.Database.MigrateAsync();
            }
        }

        private async Task<bool> ExisteTablaAspNetUsersAsync(DbConnection connection)
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
            return result is bool exists && exists;
        }

        private async Task CrearDatosSemillaAsync()
        {
            if (await _roleManager.FindByNameAsync("Admin") == null)
            {
                _logger.LogInformation("Creando roles iniciales...");
                await CrearRolSiNoExisteAsync("Admin");
                await CrearRolSiNoExisteAsync("Agendador");
                await CrearRolSiNoExisteAsync("Doctor");
            }

            var usuarioAdmin = await _userManager.FindByNameAsync("administrador");
            if (usuarioAdmin == null)
            {
                _logger.LogInformation("Creando usuario administrador por defecto...");
                usuarioAdmin = new UsuarioAplicacion
                {
                    UserName = "administrador",
                    Email = "administrador@doctorapp.com",
                    Apellidos = "Piedra",
                    Nombres = "Carlos",
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(usuarioAdmin, "Admin123");
                if (!createResult.Succeeded)
                {
                    var errores = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"No se pudo crear el usuario administrador: {errores}");
                }
            }

            if (!await _userManager.IsInRoleAsync(usuarioAdmin, "Admin"))
            {
                _logger.LogInformation("Asignando rol Admin al usuario administrador...");
                var addRoleResult = await _userManager.AddToRoleAsync(usuarioAdmin, "Admin");
                if (!addRoleResult.Succeeded)
                {
                    var errores = string.Join(", ", addRoleResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"No se pudo asignar el rol Admin: {errores}");
                }
            }
        }

        private async Task CrearRolSiNoExisteAsync(string nombreRol)
        {
            if (await _roleManager.FindByNameAsync(nombreRol) != null)
            {
                return;
            }

            var result = await _roleManager.CreateAsync(new RolAplicacion { Name = nombreRol });
            if (!result.Succeeded)
            {
                var errores = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"No se pudo crear el rol {nombreRol}: {errores}");
            }
        }
    }
}
