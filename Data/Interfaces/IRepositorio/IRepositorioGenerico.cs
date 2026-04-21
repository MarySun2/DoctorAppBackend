using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces.IRepositorio
{
    public interface IRepositorioGenerico<T> where T : class
    {
        // Obtener TODOS los registros (colección)
        Task<IEnumerable<T>> ObtenerTodos(
            Expression<Func<T, bool>> filtro = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string incluirPropiedades = null  
            );
        // Obtener el PRIMER registro que cumpla el filtro (o null)
        Task<T> ObtenerPrimero(
            Expression<Func<T, bool>> filtro = null,
            string incluirPropiedades = null  
            );
        // Agregar una entidad (solo la agrega al contexto, no guarda)
        Task Agregar(T entidad);

        // Remover una entidad
        void Remove(T entidad);
    }
}
