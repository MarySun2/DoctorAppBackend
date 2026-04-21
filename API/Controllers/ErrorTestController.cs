using API.Errores;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entidades;

namespace API.Controllers
{
    public class ErrorTestController : BaseApiController
    {
        private readonly ApplicationDbContext _db; // Cambiado a "ApplicationDbContext" (con doble 'p')

        public ErrorTestController(ApplicationDbContext db) // Corregido el nombre
        {
            _db = db;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetNotAuthorize()
        {
            return "No Autorizado";
        }

        
        [HttpGet("not-found")]
        public ActionResult<Usuario> GetNotFound()
        {
            var objeto = _db.Usuarios.Find(-1); // Asegúrate de que DbSet se llame "Usuarios"
            if (objeto == null) return NotFound(new ApiErrorResponse(404));
            return objeto;
        }

        
        [HttpGet("server-error")]
        public ActionResult<string> GetServerError() 
        {
            var objeto = _db.Usuarios.Find(-1);
            var objetoString = objeto?.ToString();
            return objetoString;
        }

        
        [HttpGet("bad-request")] // Cambiada la ruta para evitar conflicto
        public ActionResult<string> GetBadRequest() // Corregido el nombre del método
        {
            return BadRequest(new ApiErrorResponse(400));
        }
    }
}
