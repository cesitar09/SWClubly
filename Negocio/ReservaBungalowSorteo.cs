using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Datos;

namespace Negocio
{
    public class ReservaBungalowSorteo
    {
        public const short PENDIENTE = 1;

        public static Entities context()
        {
            return Datos.Context.context();
        }

        public static IEnumerable<Datos.ReservaBungalowSorteo> seleccionarPendientes()
        {
            return context().ReservaBungalowSorteo.Where(s => s.estado == PENDIENTE);
        }

        public static void insertar(Datos.ReservaBungalowSorteo rbs)
        {
            context().ReservaBungalowSorteo.AddObject(rbs);
            context().SaveChanges();
        }

        public static void modificar(Datos.ReservaBungalowSorteo rbs)
        {
            var rbsaux = context().ReservaBungalowSorteo.SingleOrDefault(p => p.id == rbs.id);
            rbsaux.estado = rbs.estado;
            rbsaux.Familia = rbs.Familia;
            //rbsaux.fechaFin = rbs.fechaFin;
            //rbsaux.fechaInicio = rbs.fechaInicio;
            //rbsaux.idSorteo = rbs.idSorteo;
            rbsaux.Pago = rbs.Pago;
            rbsaux.resultadoSorteo = rbs.resultadoSorteo;
            rbsaux.Sede = rbs.Sede;
            rbsaux.TipoBungalow = rbs.TipoBungalow;
            context().SaveChanges();
        }

        public static void Insertar(short idTemporada, short idBungalow, short idFamilia)
        {
            Datos.Familia familia = Familia.buscarId(idFamilia);
            Datos.TemporadaAlta temporada = TemporadaAlta.buscarId(idTemporada);
            Datos.Bungalow bungalow = Bungalow.buscarId(idBungalow);

            Datos.ReservaBungalowSorteo reservaSorteo = new Datos.ReservaBungalowSorteo();
            reservaSorteo.TipoBungalow = bungalow.TipoBungalow;
            reservaSorteo.Familia = familia;
            reservaSorteo.TemporadaAlta = temporada;
            //reservaSorteo.fechaInicio = temporada.fechaInicio;
            //reservaSorteo.fechaFin = temporada.fechaFin;
            reservaSorteo.estado = 1;
            reservaSorteo.Sede = bungalow.Sede;

            //crear pago
            Datos.Pago pago = new Datos.Pago();
            pago.Familia = familia;
            pago.ConceptoDePago = ConceptoDePago.buscarId(ConceptoDePago.ID_RESERVABUNGALOW);
            pago.descripcion = "Pago por participar en sorteo del tipo de bungalow " + bungalow.TipoBungalow.nombre+
                " en "+temporada.descripcion;
            pago.fechaRegistro = DateTime.Now;
            pago.fechaLimite = temporada.fechaInicio.AddDays(-Parametros.SeleccionarParametros().diasLimitePago);
            if (pago.fechaLimite.Date > DateTime.Today)
                pago.fechaLimite = DateTime.Today;
            int numDias = (temporada.fechaFin - temporada.fechaInicio).Days + 1;
            pago.monto = bungalow.TipoBungalow.precio * numDias;
            pago.estado = Pago.PENDIENTE;

            reservaSorteo.Pago = pago;
            Context.context().ReservaBungalowSorteo.AddObject(reservaSorteo);
            Context.context().SaveChanges();
        }      
    }
}
