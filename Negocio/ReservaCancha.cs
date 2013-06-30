using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Datos;
using Negocio.Util;
using System.Data.Objects.DataClasses;
using System.Data;

namespace Negocio
{
    public class ReservaCancha
    {
        public const short PENDIENTE = 1;
        public const short CANCELADA = 2;
        public const short REALIZADA = 3;

//CONEXION BASE DE DATOS
        public static Entities context()
        {
            return Datos.Context.context();
        }

//QUERY PARA INSERTAR
        public static void Insertar(Datos.ReservaCancha reservaCancha)
        {
                context().ReservaCancha.AddObject(reservaCancha);
                context().SaveChanges();

        }

        public static void Modificar(Datos.ReservaCancha reserva)
        {           
                context().ReservaCancha.ApplyCurrentValues(reserva);
                context().SaveChanges();            
        }


//QUERYS DE BUSCAR
        public static IEnumerable<Datos.ReservaCancha> SeleccionarTodo()
        {
            return context().ReservaCancha.Where(reserva => reserva.estado != ListaEstados.ESTADO_ELIMINADO);
        }

        public static IEnumerable<Datos.ReservaCancha> SeleccionarTodoTabla()
        {
            VerificarVencimiento();
            return context().ReservaCancha.Where(reserva => reserva.estado != ListaEstados.ESTADO_ELIMINADO);
        }

        public static Datos.ReservaCancha BuscarId(short idCancha)
        {
            return context().ReservaCancha.FirstOrDefault(p => p.id == idCancha);
        }

        public static IEnumerable<Datos.ReservaCancha> BuscarIdFamiliaTabla(short idFamilia)
        {
            VerificarVencimiento();
            IEnumerable<Datos.ReservaCancha> listaSocio = context().ReservaCancha.Where(p => (p.estado != 0) && (p.Familia.id == idFamilia));
            return listaSocio;
        }

        //Buscar todas las canchas en las que un socio a reservado
        public static IEnumerable<Datos.ReservaCancha> BuscarCanchaIdFamilia(short idFamilia)
        {
            IEnumerable<Datos.ReservaCancha> listaSocioXCancha = context().ReservaCancha.Where(p => (p.estado != 0) && (p.Familia.id == idFamilia));
            return listaSocioXCancha;
        }
        

//QUERY DE ELIMINAR
        //Metodos para eliminar una Reserva
        public static void Eliminar(short id)
        {
            Datos.ReservaCancha reservaEliminar = ReservaCancha.BuscarId(id);
            reservaEliminar.estado = 2;

            context().SaveChanges();
           
        }        

//OTROS
        private static void VerificarVencimiento()
        {
            IEnumerable<Datos.ReservaCancha> listaReserva = context().ReservaCancha.Where(p => (p.estado == 1 && p.horaFin.CompareTo(DateTime.Now) < 0));
            //foreach (Datos.ReservaCancha reserva in listaReserva)
            for (int i = 0; i < listaReserva.Count(); i++)
            {
                listaReserva.ElementAt(i).estado = REALIZADA;
            }
            context().SaveChanges();
        }


    }
}
