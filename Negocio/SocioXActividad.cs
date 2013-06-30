﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Datos;
using System.Data.Objects;
using System.Data;
using System.Diagnostics;

namespace Negocio
{
    public class SocioXActividad
    {

    //QUERY PARA INSERTAR
        public static int Insertar(Datos.SocioXActividad socioXActividad)
        {
            bool actividadActualizada = false;
            bool inscripcionActualizada = false;
            Datos.Actividad actividad = null;
            Datos.Socio socio = null;

            //Busca cupo en la actividad

            socio = Negocio.Socio.BuscarId(socioXActividad.idSocio);
            actividad = Negocio.Actividad.BuscarId(socioXActividad.idActividad);
            Datos.SocioXActividad eliminado = Context.context().SocioXActividad.SingleOrDefault(e =>
                (e.idActividad == socioXActividad.idActividad) &&
                (e.idSocio == socioXActividad.idSocio));
            //Paso 1: comprueba que no se encuentre inscrito
            if (eliminado == null || eliminado.estado == 0)
            {
                //Paso 2: trata de reservar cupo modificando la cantidad de vacantesDisponibles
                //Si ha habido otra transaccion que obtuvo un cupo, vuelve a intentarlo
                while (actividad.vacantesDisponibles > 0 && !actividadActualizada)
                {
                    try
                    {
                        actividad.vacantesDisponibles--;
                        Context.context().SaveChanges();
                        actividadActualizada = true;    //ejecuta esto si no hubo problemas al obtener un cupo
                    }
                    catch (OptimisticConcurrencyException)    //ocurre una excepcion si otro usuario ha modificado el valor de vacantesDisponibles al mismo tiempo
                    {
                        Debug.WriteLine("Error al separar cupo para la actividad " + actividad.nombre);
                        Context.context().Refresh(RefreshMode.StoreWins, actividad);
                    }
                }
                //Paso 3: Si obtuvo cupo correctamente inserta en la tabla socioXActividad
                if (actividadActualizada)
                {
                    try
                    {
                        if (eliminado == null)
                        {   //si no esta en la bd
                            if (actividad.precio > 0)
                            {
                                Datos.Pago pago = new Datos.Pago();
                                pago.fechaRegistro = DateTime.Now;
                                pago.fechaLimite = actividad.fechaInicio.AddDays(-Parametros.SeleccionarParametros().diasLimitePago);
                                if (pago.fechaLimite.Date < DateTime.Today) //si esta muy cerca de la fecha de Inicio, solo tendra un dia para pagar
                                    pago.fechaLimite = DateTime.Today;
                                pago.ConceptoDePago = ConceptoDePago.buscarId(2);
                                pago.descripcion = "Incripción de " + socio.Persona.nombre + " " + socio.Persona.apPaterno + " " +
                                    socio.Persona.apMaterno + " para la actividad " + actividad.nombre;
                                pago.monto = actividad.precio;
                                pago.Familia = Negocio.Familia.buscarIdSocio(socioXActividad.idSocio);
                                pago.estado = Pago.PENDIENTE;
                                socioXActividad.Pago = pago;
                            }
                            Context.context().SocioXActividad.AddObject(socioXActividad);
                            Context.context().SaveChanges();
                            inscripcionActualizada = true;
                        }
                        else
                        {   //Si se encuentra en la bd como inactivo
                            if (actividad.precio > 0)
                            {
                                Datos.Pago pago = eliminado.Pago;
                                pago.fechaLimite = actividad.fechaInicio.AddDays(-Parametros.SeleccionarParametros().diasLimitePago);
                                if (pago.fechaLimite.Date < DateTime.Today) //si esta muy cerca de la fecha de Inicio, solo tendra un dia para pagar
                                    pago.fechaLimite = DateTime.Today;
                                pago.ConceptoDePago = ConceptoDePago.buscarId(2);
                                pago.descripcion = "Incripción de " + socio.Persona.nombre + " " + socio.Persona.apPaterno + " " +
                                    socio.Persona.apMaterno + " para la actividad " + actividad.nombre;
                                pago.monto = actividad.precio;
                                pago.estado = Pago.PENDIENTE;
                            }
                            eliminado.estado = 1;
                            Context.context().SaveChanges();
                            inscripcionActualizada = true;
                        }
                        return 1;
                    }
                    catch
                    {
                        while (actividadActualizada && !inscripcionActualizada)
                        {
                            try
                            {
                                actividad.vacantesDisponibles++;
                                Context.context().SaveChanges();
                                actividadActualizada = false;    //ejecuta esto si no hubo problemas al obtener un cupo
                            }
                            catch (OptimisticConcurrencyException)    //ocurre una excepcion si otro usuario ha modificado el valor de vacantesDisponibles al mismo tiempo
                            {
                                Context.context().Refresh(RefreshMode.StoreWins, actividad);
                            }
                        }
                    }
                }
                return 0;
            }
            return 0;
        }
    

    //QUERYS DE BUSCAR
        public static IEnumerable<Datos.SocioXActividad> SeleccionarTodo()
        {
            //using (Entities context = new Entities())
            //{
            IEnumerable<Datos.SocioXActividad> listaSocioXActividad = Context.context().SocioXActividad.Where
                    (p => p.estado != 0 && p.Socio.Persona.estado!=0 && p.Actividad.estado!=0);
                return listaSocioXActividad;
            //}
        }
        public static IEnumerable<Datos.SocioXActividad> BuscarIdActividad(short idActividad)
        {
            IEnumerable<Datos.SocioXActividad> listaSocioXActividad = Context.context().SocioXActividad.Where(p => 
                ( (p.estado != 0) && (p.idActividad==idActividad) ));
            return listaSocioXActividad;
        }

        //Buscar todas las activdades en las que un socio se ha incrito

        // Metodo movido a Negocio.Actividad
        /*public static IEnumerable<Datos.SocioXActividad> BuscarActividadIdFamilia(short idFamilia)
        {
            IEnumerable<Datos.SocioXActividad> listaSocioXActividad = Context.context().SocioXActividad
                .Where(p => (p.estado != 0) && (p.Socio.Familia.id == idFamilia));
            return listaSocioXActividad;
        }*/

        //Busca a una actividad especifica para un socio especifico
        public static IEnumerable<Datos.SocioXActividad> BuscarIdActividadIdFamilia(int idActividad, int idFamilia)
        {
            //using (Entities context = new Entities())
            //{
            IEnumerable<Datos.SocioXActividad> listaSocioXActividad = Context.context().SocioXActividad
                    .Where(p => (p.estado != 0) && (p.idActividad == idActividad) && (p.Socio.Familia.id == idFamilia));
                return listaSocioXActividad;
            //}
        }

    //QUERY DE ELIMINAR
        //Metodos para eliminar una Inscripcion
        public static void Eliminar(Datos.SocioXActividad socioXActividad)
        {
            Eliminar(socioXActividad.idSocio, socioXActividad.idActividad);
        }

        public static void Eliminar(short idSocio, short idActividad)
        {
            bool actividadActualizada = false;
            Datos.Actividad actividad = null;

            //Busca cupo en la actividad
            try
            {
                actividad = Negocio.Actividad.BuscarId(idActividad);
                while (actividadActualizada == false)
                {
                    try
                    {
                        actividad.vacantesDisponibles++;
                        Context.context().SaveChanges();
                        actividadActualizada = true;
                    }
                    catch (OptimisticConcurrencyException)    //ocurre una excepcion si otro usuario ha modificado el valor de vacantesDisponibles
                    {
                        Debug.WriteLine("error de concurrencia");
                        Context.context().Refresh(RefreshMode.StoreWins, actividad);
                    }
                }
            }
            catch
            {   //entra aqui si hubo una excepcion en el buscarid
                throw;
            }
            //Inserta si alcanzo cupo
            if (actividadActualizada)
            {
                try
                {
                    Datos.SocioXActividad encontrado = Context.context().SocioXActividad.FirstOrDefault(e =>
                        (e.idActividad == idActividad) && (e.idSocio == idSocio));
                    if (encontrado != null)
                    {
                        encontrado.estado = 0;
                        if (encontrado.Pago != null)
                        {
                            switch (encontrado.Pago.estado)
                            {
                                case Pago.CANCELADO:
                                    encontrado.Pago.estado = Pago.PORDEVOLVER;
                                    break;
                                case Pago.PENDIENTE:
                                    encontrado.Pago.estado = 0;
                                    break;
                            }
                        }
                        Context.context().SaveChanges();
                    }
                }
                catch (Exception)
                {
                    while (actividadActualizada == true)
                    {
                        try
                        {
                            actividad.vacantesDisponibles--;
                            Context.context().SaveChanges();
                            actividadActualizada = false;
                        }
                        catch (OptimisticConcurrencyException)    //ocurre una excepcion si otro usuario ha modificado el valor de vacantesDisponibles
                        {
                            Debug.WriteLine("error de concurrencia");
                            Context.context().Refresh(RefreshMode.StoreWins, actividad);
                        }
                    }
                    throw;
                }
            }

        }
    //Naty, revisa esto
        public static void EliminarTodo(short idSocio,short idActividad)
        {
            //using (Entities tempContext = new Entities())
            {
                IEnumerable<Datos.SocioXActividad> encontrado = BuscarIdActividadIdFamilia(idActividad, idSocio);

                if (encontrado != null)
                {
                    foreach (var socioxAct in encontrado)
                    {
                        Datos.SocioXActividad eliminado = null;
                        eliminado = Context.context().SocioXActividad.SingleOrDefault(e =>
                        (e.idActividad == socioxAct.idActividad) &&
                        (e.idSocio == socioxAct.idSocio));
                        if (eliminado != null)
                        {
                            eliminado.estado = 0;
                            Context.context().SaveChanges();
                        }

                    }                    
                }
            }
        }
        
    }
}
