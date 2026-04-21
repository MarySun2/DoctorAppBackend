using Data.Interfaces.IRepositorio;
using Models.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositorio
{
    public class MedicoRepositorio : Repositorio<Medico>, IMedicoRepositorio
    {
        private readonly ApplicationDbContext _db;

        public MedicoRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Actualizar(Medico medico)
        {
            var medicoDb = _db.Medicos.FirstOrDefault(e => e.Id == medico.Id);
            if (medicoDb != null) 
            { 
                medicoDb.Apellidos = medico.Apellidos;
                medicoDb.Nombres = medico.Nombres;
                medicoDb.Direccion = medico.Direccion; 
                medicoDb.Estado = medico.Estado;
                medicoDb.Telefono = medico.Telefono;
                medicoDb.Genero = medico.Genero;
                medicoDb.FechaActualizacion = DateTime.Now;
                medicoDb.EspecialidadId = medico.EspecialidadId;
                _db.SaveChanges(); 
            }
        }
    }
}
