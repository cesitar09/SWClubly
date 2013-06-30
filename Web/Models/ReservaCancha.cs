﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects.DataClasses;
using System.Collections;
using Negocio.Util;

namespace Web.Models
{
    public class ReservaCancha
    {
        public const short PENDIENTE = 1;
        public const short CANCELADA = 2;
        public const short VENCIDA = 3;

        [Display(Name = "Actividad")]
        public Models.Actividad actividad { get; set; }

        [Display(Name = "Cancha")]
        public Models.Cancha cancha { get; set; }

        [Display(Name = "familia")]
        public Models.Familia familia { get; set; }

        [Display(Name = "Id")]
        public short id { get; set; }

        [Display(Name = "*Fecha Inicio Fecha")]
        public DateTime fechaInicio { get; set; }

        [Display(Name = "*Hora Inicio")]
        [Required(ErrorMessage = "Debe ingresar una Hora")]
        public DateTime horaInicio { get; set; }
        
        [Display(Name= "*Hora Fin")]
        [Required(ErrorMessage = "Debe ingresar una Hora")]
        public DateTime horaFin { get; set; }

        [Display(Name= "Estado")]
        public String estado { get; set; }
        public static ListaEstados listaEstados = new ListaEstados();

        [Display(Name = "idActividad")]
        public short idActividad { get; set; }

        [Display(Name = "idCancha")]
        public short idCancha { get; set; }

        [Display(Name = "*Codigo Familia")]
        public short idFamilia { get; set; }

        [Display(Name = "*Sede")]
        public short idSede { get; set; }

        [Display(Name = "*Tipo de Cancha")]
        public short idTipoCancha { get; set; }

        static ReservaCancha()
        {
            listaEstados.AgregarEstado(PENDIENTE, "Pendiente");
            listaEstados.AgregarEstado(CANCELADA, "Cancelada");
            listaEstados.AgregarEstado(VENCIDA, "Realizado");
        }

//CONSTRUCTORES
        public ReservaCancha() { }

        public ReservaCancha(Datos.ReservaCancha reservaCancha)
        {
                id = reservaCancha.id;
                idCancha = reservaCancha.Cancha.id;
                idSede = reservaCancha.Cancha.Sede.id;
                idTipoCancha = reservaCancha.Cancha.TipoCancha.id;
                if(reservaCancha.Actividad!=null)
                idActividad = reservaCancha.Actividad.id;
                idFamilia = reservaCancha.Familia.id;   
                fechaInicio = reservaCancha.horaInicio;
                horaInicio = reservaCancha.horaInicio;
                horaFin = reservaCancha.horaFin;
                estado = listaEstados.TextoEstado(reservaCancha.estado);
                familia = Familia.Convertir(reservaCancha.Familia);
                if(reservaCancha.Actividad != null)
                actividad = Actividad.Convertir(reservaCancha.Actividad);
                cancha = Cancha.Convertir(reservaCancha.Cancha);
        }

//CONVERTIDORES
        public static IEnumerable<Models.ReservaCancha> ConvertirLista(IEnumerable<Datos.ReservaCancha> listaReservas)
        {
            return listaReservas.Select(reserva => Convertir(reserva));          
        }

        public static Models.ReservaCancha Convertir(Datos.ReservaCancha reservaCancha)
        {
            return new ReservaCancha(reservaCancha);
        }
        
        public static Datos.ReservaCancha Invertir(Models.ReservaCancha reservaCancha)
        {
            Datos.ReservaCancha reserva;
            if (reservaCancha.id == 0)
                reserva = new Datos.ReservaCancha();
            else
               reserva = Negocio.ReservaCancha.BuscarId(reservaCancha.id);
            
            reserva.horaInicio = reservaCancha.fechaInicio.Add(reservaCancha.horaInicio.TimeOfDay);
            reserva.horaFin = reservaCancha.horaFin;//fechaFin.Add(reservaCancha.horaFin.TimeOfDay);
            reserva.estado = listaEstados.EstadoTexto(reservaCancha.estado);
            if (reservaCancha.actividad != null)
                reserva.Actividad = Actividad.Invertir(reservaCancha.actividad);
            else reserva.Actividad = null;
            reserva.Cancha = Cancha.Invertir(reservaCancha.cancha);
            reserva.Familia = Familia.Invertir(reservaCancha.familia);
            return reserva;
        }

//QUERYS
        public static IEnumerable<Models.ReservaCancha> SeleccionarTodo()
        {
            IEnumerable<Datos.ReservaCancha> listaReservas = Negocio.ReservaCancha.SeleccionarTodo();
            return ConvertirLista(listaReservas);
        }

        public static IEnumerable<Models.ReservaCancha> SeleccionarTodoTabla()
        {
            IEnumerable<Datos.ReservaCancha> listaReservas = Negocio.ReservaCancha.SeleccionarTodoTabla();
            return ConvertirLista(listaReservas);
        }

        public static Models.ReservaCancha BuscarId(short idReserva) { 
            return Convertir(Negocio.ReservaCancha.BuscarId(idReserva));
        
        }

        public static IEnumerable<Models.ReservaCancha> BuscarIdFamiliaTabla(short idFamilia)
        {
            IEnumerable<Datos.ReservaCancha> listaReservas = Negocio.ReservaCancha.BuscarIdFamiliaTabla(idFamilia);
            return ConvertirLista(listaReservas);

        }

        public static IEnumerable<Models.ReservaCancha> BuscarIdFamilia(short idfamilia) {
            IEnumerable<Datos.ReservaCancha> listaReservas = Negocio.ReservaCancha.BuscarCanchaIdFamilia(idfamilia);
            return ConvertirLista(listaReservas);
        }



//INSERTAR
        public static void Insertar(ReservaCancha modelsNuevo)
        {
            Datos.ReservaCancha datosNuevo = null;
            if (modelsNuevo.id == 0)
            {
                datosNuevo = new Datos.ReservaCancha();
                datosNuevo.Familia = Negocio.Familia.buscarId(modelsNuevo.familia.id);
                if (modelsNuevo.actividad != null)
                    datosNuevo.Actividad = Negocio.Actividad.BuscarId(modelsNuevo.actividad.id);
                else datosNuevo.Actividad = null;
                datosNuevo.Cancha = Negocio.Cancha.BuscarId(modelsNuevo.cancha.id);
                datosNuevo.horaInicio = modelsNuevo.horaInicio;
                datosNuevo.horaFin = modelsNuevo.horaFin;
                datosNuevo.estado = ListaEstados.ESTADO_ACTIVO;
                Negocio.ReservaCancha.Insertar(datosNuevo);
            }
            else
            {
                datosNuevo = Negocio.ReservaCancha.BuscarId(modelsNuevo.id);
                if(modelsNuevo.actividad!= null)
                    datosNuevo.Actividad=Negocio.Actividad.BuscarId(modelsNuevo.actividad.id);
                else datosNuevo.Actividad = null;
                datosNuevo.Cancha = Negocio.Cancha.BuscarId(modelsNuevo.cancha.id);
                datosNuevo.horaInicio = modelsNuevo.horaInicio;
                datosNuevo.horaFin = modelsNuevo.horaFin;
                datosNuevo.estado = ListaEstados.ESTADO_ACTIVO;
                Negocio.ReservaCancha.Modificar(datosNuevo);
            }

            
        }

//ELIMINAR
        public static void Eliminar(short id)
        {
            Negocio.ReservaCancha.Eliminar(id);
        }
    }

}