using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Datos;

namespace Negocio
{
    public class Pago
    {
        public const short PENDIENTE = 1;
        public const short PORDEVOLVER = 2;
        public const short DEVUELTO = 3;
        public const short VENCIDO = 4;
        public const short CANCELADO = 5;

        public static Entities context()
        {
            return Datos.Context.context();
        }

        public static void Insertar(Datos.Pago pago)
        {
            
                pago.estado = PENDIENTE;
                context().Pago.AddObject(pago);
                context().SaveChanges();
                Negocio.Util.logito.ElLogeador("Se inserto el pago", pago.id.ToString());
           
        }

        //public static void GenerarPagosMembresia(Datos.Pago pago)
        //{ }
        public static void Cancelar(short pago)
        {
            Datos.Pago pag = context().Pago.SingleOrDefault(p => p.id == pago);
            if (pag != null)
            {
                pag.estado = CANCELADO;
               // pag.fechaLimite = pago.fechaLimite;
               // pag.fechaRegistro = pago.fechaRegistro;
                context().Pago.ApplyCurrentValues(pag);
                context().SaveChanges();
                Negocio.Util.logito.ElLogeador("Se elimino el pago", pag.id.ToString());
            }
        }

        public static void modificar(Datos.Pago pago)
        {
           
                context().Pago.ApplyCurrentValues(pago);
                context().SaveChanges();
                Negocio.Util.logito.ElLogeador("Se modificio el pago", pago.id.ToString());
        }

        //Esto es un seleccionar todo para las pagos

        public static IEnumerable<Datos.Pago> SeleccionarTodo()
        {

            VerificarVencimiento();
            IEnumerable<Datos.Pago> listaPago = context().Pago.Where(p => p.estado != 0);
            
            return listaPago;

        }

        public static IEnumerable<Datos.Pago> SeleccionarCuotas()
        {

            //VerificarVencimiento();
            IEnumerable<Datos.Pago> listaPago = context().Pago.Where(p => p.ConceptoDePago.id==6 && (p.estado ==1 || p.estado == 4));

            return listaPago;

        }

        private static void VerificarVencimiento()
        {
            IEnumerable<Datos.Pago> listaPago = context().Pago.Where(p => (p.estado == 1 && p.fechaLimite.CompareTo(DateTime.Now)<0));
            foreach (Datos.Pago pago in listaPago)
            {
                pago.estado = VENCIDO;
                if (pago.ConceptoDePago.id == 2) // Si se vencio la membresia se le agrega la multa
                    pago.monto = pago.monto + Negocio.Parametros.SeleccionarParametros().multa;
                if (pago.ConceptoDePago.id == 3) // Si se vencio la actividad, se elimina la inscripcion
                    Negocio.SocioXActividad.Eliminar(pago.SocioXActividad.FirstOrDefault());
                //if (pago.ConceptoDePago.id == 4) 
                  
            }
            context().SaveChanges();
            Negocio.Util.logito.ElLogeador("Se actualizaron los pagos vencidos", "");
        }

        public static IEnumerable<Datos.Pago> SeleccionarPorFamilia(short id)
        {

            IEnumerable<Datos.Pago> listaPago = context().Pago.Where(p =>( p.Familia.id == id) && (p.estado != 0));
            return listaPago;

        }

        public static Datos.Pago BuscarId(short id)
        {
            return context().Pago.FirstOrDefault(p => p.id == id);
        }

        public static bool BuscarMes()
        {
            IEnumerable<Datos.Pago> listaPago = context().Pago.Where(p => p.ConceptoDePago.id == Negocio.ConceptoDePago.ID_MEMBRESIA);
            Datos.Pago pago = listaPago.ElementAt(listaPago.Count()-1);
            if (pago == null)
            {
                return true;
            }
            else
            {
                if (pago.fechaRegistro.Month == DateTime.Today.Month)
                    return false;
                else return true;
            }
                       
        }



        public static void GenerarPagosMembresia()
        {
            IEnumerable<Datos.Familia> listaFamilias = Familia.SeleccionarNoVitalicio();
           
            foreach (Datos.Familia familia in listaFamilias)
            {
                Datos.Pago pago = new Datos.Pago();
                pago.ConceptoDePago = ConceptoDePago.buscarId(ConceptoDePago.ID_MEMBRESIA);
                if(pago.ConceptoDePago!=null)
                        pago.monto = pago.ConceptoDePago.monto.Value ;
                else 
                        pago.monto = 0;
            //{
            //    foreach(Datos.Pago paguito in familia.Pago){
            //        if (paguito.estado == Negocio.Pago.PORDEVOLVER)
            //        {
            //            if (paguito.monto <= monto)
            //            {
            //                montoDevolver = paguito.monto;
            //                paguito.estado = Negocio.Pago.DEVUELTO;
            //            }
            //            else
            //            {

            //            }

            //        }
            //    }             
               
                pago.descripcion = "Membresia del mes "+DateTime.Today.Month.ToString();//cambiar
                pago.montoDevolver = 0;
                pago.fechaRegistro = DateTime.Now;
                pago.fechaLimite = DateTime.Now.AddDays(Parametros.SeleccionarParametros().diasLimitePago);
                
                pago.estado = PENDIENTE;
                familia.Pago.Add(pago);
            }
            Context.context().SaveChanges();
            Negocio.Util.logito.ElLogeador("Se insertaron los pagos por membresia", "Membresia del mes " + DateTime.Today.Month.ToString());
        }

        public static IEnumerable<Datos.Pago> SeleccionarPagosPorDevolver(short idFamilia)
        {
            return Context.context().Pago.Where(p => p.estado == PORDEVOLVER && p.Familia.id == idFamilia);
        }
    }
}