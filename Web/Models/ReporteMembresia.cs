using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Datos;
using System.Data.Objects;

namespace Web.Models
{
    public class ReporteMembresia
    {

        public String nombre { get; set; }

        public double DeudaTotal { get; set; }

       // IEnumerable<Models.Pago> PagosPendientes { get; set; }

        public List<Models.DeudasAcumuladas> DatosDeudores { get; set; }

        [DisplayName("Reporte Fecha Inicio")]
        public DateTime fechainicio { get; set; }


        [DisplayName("Reporte Fecha Fin")]
        public DateTime fechafinal { get; set; }
       // IEnumerable<double> PagoTotalPendiente        
        public ReporteMembresia(DateTime FechaIni, DateTime FechaF)
        {
            nombre = "Reporte De Membresia";

            IEnumerable<Models.Familia> familias = Models.Familia.SeleccionarTodo();
            DeudaTotal = 0;
            DatosDeudores = new List<DeudasAcumuladas>();
            foreach (Familia i in familias)
            {
                Models.DeudasAcumuladas deuda = new DeudasAcumuladas(i.id);
                DatosDeudores.Add(deuda);
                DeudaTotal = DeudaTotal + deuda.DeudaTotalSocio;
            }

            fechainicio = FechaIni;
            fechafinal = FechaF;
        }

    }
}