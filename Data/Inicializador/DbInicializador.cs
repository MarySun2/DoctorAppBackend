using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Entidades;

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
            var provider = _db.Database.ProviderName ?? string.Empty;
            _logger.LogInformation("Inicializando base de datos. Provider detectado: {Provider}", provider);

            if (provider.Contains("Npgsql", StringComparison.OrdinalIgnoreCase))
            {
                await InicializarPostgreSqlAsync();
            }
            else
            {
                await InicializarConMigracionesAsync();
            }

            await CrearRolesAsync();
            await CrearUsuarioAdministradorAsync();
        }

        private async Task InicializarPostgreSqlAsync()
        {
            var aspNetUsersExiste = await TablaExisteAsync("AspNetUsers");
            if (!aspNetUsersExiste)
            {
                _logger.LogInformation("La tabla AspNetUsers no existe. Creando esquema completo con EnsureCreatedAsync().");
                await _db.Database.EnsureCreatedAsync();
            }
            else
            {
                _logger.LogInformation("La tabla AspNetUsers ya existe en PostgreSQL.");
            }
        }

        private async Task InicializarConMigracionesAsync()
        {
            var pendientes = await _db.Database.GetPendingMigrationsAsync();
            if (pendientes.Any())
            {
                _logger.LogInformation("Aplicando migraciones pendientes: {Cantidad}", pendientes.Count());
                await _db.Database.MigrateAsync();
            }
            else
            {
                _logger.LogInformation("No hay migraciones pendientes.");
            }
        }

        private async Task<bool> TablaExisteAsync(string tableName)
        {
            const string sql = "SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = 'public' AND table_name = {0})";
            return await _db.Database.SqlQueryRaw<bool>(sql, tableName).SingleAsync();
        }

        private async Task CrearRolesAsync()
        {
            string[] roles = ["Admin", "Agendador", "Doctor"];

            foreach (var role in roles)
            {
                if (await _roleManager.FindByNameAsync(role) != null)
                {
                    continue;
                }

                var resultadoRol = await _roleManager.CreateAsync(new RolAplicacion { Name = role });
                if (!resultadoRol.Succeeded)
                {
                    throw new InvalidOperationException($"No se pudo crear el rol {role}: {string.Join(", ", resultadoRol.Errors.Select(e => e.Description))}");
                }

                _logger.LogInformation("Rol creado correctamente: {Role}", role);
            }
        }

        private async Task CrearUsuarioAdministradorAsync()
        {
            var usuarioAdmin = await _userManager.FindByNameAsync("administrador");

            if (usuarioAdmin == null)
            {
                usuarioAdmin = new UsuarioAplicacion
                {
                    UserName = "administrador",
                    Email = "administrador@doctorapp.com",
                    Apellidos = "Piedra",
                    Nombres = "Carlos"
                };

                var resultadoUsuario = await _userManager.CreateAsync(usuarioAdmin, "Admin123");
                if (!resultadoUsuario.Succeeded)
                {
                    throw new InvalidOperationException($"No se pudo crear el usuario administrador: {string.Join(", ", resultadoUsuario.Errors.Select(e => e.Description))}");
                }

                _logger.LogInformation("Usuario administrador creado correctamente.");
            }
            else
            {
                _logger.LogInformation("El usuario administrador ya existe.");
            }

            if (!await _userManager.IsInRoleAsync(usuarioAdmin, "Admin"))
            {
                var resultadoRol = await _userManager.AddToRoleAsync(usuarioAdmin, "Admin");
                if (!resultadoRol.Succeeded)
                {
                    throw new InvalidOperationException($"No se pudo asignar el rol Admin al usuario administrador: {string.Join(", ", resultadoRol.Errors.Select(e => e.Description))}");
                }

                _logger.LogInformation("Rol Admin asignado al usuario administrador.");
            }
        }
    }
}
