using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Servicios.interfaces
{
    public interface IHistoriaClinicaServicio
    {
        Task<IEnumerable<HistoriaClinicaDto>> ObtenerTodos();
        Task<HistoriaClinicaDto> Agregar(HistoriaClinicaDto modelDto);
        Task Actualizar(HistoriaClinicaDto modelDto);
        Task Remover(int id);
    }
}
