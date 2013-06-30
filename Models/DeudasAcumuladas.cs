using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class DeudasAcumuladas
    {

        public short idFamilia { get; set; }

        public String NombreSocio { get; set; }

        public String ApPaterno { get; set; }

        public String ApMaterno { get; set; }

        public double DeudaTotalSocio { get; set; }

        public DeudasAcumuladas(short id)
        {
            idFamilia = id;
            Models.Socio socioTitular = Models.Socio.BuscarTitulares().FirstOrDefault(p => p.familia.id == id);
            NombreSocio = socioTitular.persona.nombre;
            ApPaterno = socioTitular.persona.apPaterno;
            ApMaterno = socioTitular.persona.apMaterno;
            DeudaTotalSocio = 0;

            IEnumerable<Pago> PagosVencidos = Models.Pago.SeleccionarPorFamilia(id).Where(p=>p.estado.Equals("Vencido"));
            foreach (Pago i in PagosVencidos)
            {
                DeudaTotalSocio = DeudaTotalSocio + i.monto;
            }

        }
    }
}