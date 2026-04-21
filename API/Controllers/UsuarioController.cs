using Data;
using Data.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.DTOs;
using Models.Entidades;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")] // api/usuario  HttpGet, HttpPost, HttpPut, HttpDelete
    [ApiController]
    public class UsuarioController : BaseApiController
    {
        //private readonly ApplicationDbContext _db;
        private readonly UserManager<UsuarioAplicacion> _userManager;
        private readonly ITokenServicio _tokenservicio;
        private ApiResponse _response;
        private readonly RoleManager<RolAplicacion> _roleManager;

        //Constructor
        public UsuarioController(UserManager<UsuarioAplicacion> userManager, ITokenServicio tokenServicio,
            RoleManager<RolAplicacion>roleManager)
        {
            //_db = db
            _userManager = userManager;
            _tokenservicio = tokenServicio;
            _response = new();
            _roleManager = roleManager;
        }

        [Authorize(Policy = "AdminRol")]
        [HttpGet] // api/usuario
        public async Task<ActionResult> GetUsuarios()
        {
            var usuarios = await _userManager.Users.Select(u => new UsuarioListaDto
            {
                Username = u.UserName,
                Apellidos = u.Apellidos,
                Nombres = u.Nombres,
                Email = u.Email,
                Rol = string.Join(", ", _userManager.GetRolesAsync(u).Result.ToArray()) // Obtener los roles del usuario
            }).ToListAsync();
            _response.Resultado = usuarios;
            _response.IsExitoso = true;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        //[Authorize]
        //[HttpGet("{id}")]//{id} // api/usuario/1
        //public async Task<ActionResult<Usuario>> GetUsuario(int id)
        //{
        //    var usuario = await _db.Usuarios.FindAsync(id);
        //    return Ok(usuario);
        //}
        [Authorize(Policy ="AdminRol")]
        [HttpPost("registro")] //POST: api/usuario/registro
        public async Task<ActionResult<UsuarioDto>> Registro(RegistroDto registroDto) 
        {
            if (await UsuarioExiste(registroDto.Username)) return BadRequest("El Usuario ya Esta Registrado");

            //using var hmac = new HMACSHA512();
            var usuario = new UsuarioAplicacion
            {
                UserName = registroDto.Username.ToLower(),
                Email = registroDto.Email.ToLower(),
                Apellidos = registroDto.Apellidos,
                Nombres = registroDto.Nombres

                //PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registroDto.Password)),
                //PasswordSalt = hmac.Key
            };
            //_db.Usuarios.Add(usuario);
            //await _db.SaveChangesAsync();
            var resultado = await _userManager.CreateAsync(usuario, registroDto.Password);
            if (!resultado.Succeeded) return BadRequest(resultado.Errors);

            var rolResultado = await _userManager.AddToRoleAsync(usuario, registroDto.Rol);
            if (!rolResultado.Succeeded) return BadRequest("Error al Agregar el Rol al Usuario");

            return new UsuarioDto
            {
                Username = usuario.UserName,
                Token = await _tokenservicio.CrearToken(usuario)
            };
        }

        [HttpPost("login")] //POST: api/usuario/login
        public async Task<ActionResult<UsuarioDto>>Login(LoginDto loginDto)
        {
            var usuario = await _userManager.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username);
            if (usuario == null) return Unauthorized("Usuario no Valido");
            var resultado = await _userManager.CheckPasswordAsync(usuario, loginDto.Password);

            if (!resultado) return Unauthorized("Contraseña no Valida");
            //using var hmac = new HMACSHA512(usuario.PasswordSalt);
            //var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            //for (int i = 0; i < computedHash.Length; i++)
            //{
            //    if (computedHash[i] != usuario.PasswordHash[i]) return Unauthorized("Contraseña no Valida");
            //}
            return new UsuarioDto
            {
                Username = usuario.UserName,
                Token = await _tokenservicio.CrearToken(usuario)
            };
        }
        [Authorize(Policy = "AdminRol")]
        [HttpGet("ListadoRoles")]
        public IActionResult GetRoles()
        {
            var roles = _roleManager.Roles.Select(r => new { NombreRol = r.Name }).ToList();
            _response.Resultado = roles;
            _response.IsExitoso = true;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        private async Task<bool> UsuarioExiste(string username)
        { 
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}
