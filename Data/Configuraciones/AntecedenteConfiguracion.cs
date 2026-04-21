using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Configuraciones
{
    public class AntecedenteConfiguracion : IEntityTypeConfiguration<Paciente>
	{
        public void Configure(EntityTypeBuilder<Antecedente> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.HistoriaClinicaId).IsRequired();
           

            //Relaciones
            builder.HasOne(x => x.HistoriaClinica).WithMany(); 
                   .HasForeignKey(x => x.HistoriaClinicaId); 
                   .OnDelete(DeleteBehavior.NoAction);
        }

    }
}
