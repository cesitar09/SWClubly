﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Datos;

namespace Web.Models
{
    public class TemporadaAlta
    {
        [DisplayName("Id")]
        public short id { get; set; }

        [DisplayName("Descripcion")]
        public String descripcion { get; set; }

        [DisplayName("Fecha Inicio")]
        public DateTime fechaInicio { get; set; }

        [DisplayName("Fecha Fin")]
        public DateTime fechaFin { get; set; }

        [DisplayName("Estado")]
        public short estado { get; set; }

        public TemporadaAlta() { }

        public TemporadaAlta(Datos.TemporadaAlta tempA) 
        {
            id = tempA.id;
            descripcion = tempA.descripcion;
            fechaInicio = tempA.fechaInicio;
            fechaFin = tempA.fechaFin;
            estado = tempA.estado;
        }

        public static TemporadaAlta Convertir(Datos.TemporadaAlta tempA)
        {
            return new TemporadaAlta(tempA);
        }

        public static IEnumerable<TemporadaAlta> ConvertirLista(IEnumerable<Datos.TemporadaAlta> listatempA)
        {

            return listatempA.Select(tempA => Convertir(tempA));

        }

        //....Valida si hay Sorteos....//
        //public static bool HaySorteos(Models.TemporadaAlta tempA)
        //{

        //    return Negocio.TemporadaAlta.HaySorteos(tempA.id);
        //}


        public static Datos.TemporadaAlta Invertir(Models.TemporadaAlta mtempA)
        {
            Datos.TemporadaAlta dtempA;
            if (mtempA.id == 0)
                dtempA = new Datos.TemporadaAlta();
            else
                dtempA = Negocio.TemporadaAlta.buscarId(mtempA.id);
                dtempA.descripcion = mtempA.descripcion;
                dtempA.fechaInicio=mtempA.fechaInicio;
                dtempA.fechaFin=mtempA.fechaFin;
                dtempA.estado = mtempA.estado;
           
            return dtempA;
        }

        public static IEnumerable<Datos.TemporadaAlta> ConvertirListaInverso(IEnumerable<Models.TemporadaAlta> mtempA)
        {
            return mtempA.Select(tempA => Invertir(tempA));
        }

        public static Models.TemporadaAlta buscarId(short id)
        {
            return Convertir(Negocio.TemporadaAlta.buscarId(id));
        }

        public static IEnumerable<TemporadaAlta> SeleccionarTodo()
        {
            IEnumerable<Datos.TemporadaAlta> tempA = Negocio.TemporadaAlta.seleccionarTodo();
            return ConvertirLista(tempA);
        }

        public static Models.TemporadaAlta SeleccionarporId(short id)
        {
            return Convertir(Negocio.TemporadaAlta.buscarId(id));
        }

        public static void modificarTemporadaAlta(Models.TemporadaAlta tempA)
        {
            Negocio.TemporadaAlta.modificar(Invertir(tempA));
            Models.Sorteo.modificar(tempA);
        }

        public static void insertarTemporadaAlta(Models.TemporadaAlta tempA)
        {
            Negocio.TemporadaAlta.insertar(Invertir(tempA));
            Models.Sorteo.insertarSorteos(Models.TemporadaAlta.SeleccionarTodo().Last());
        }
    
        public static void eliminarTemporadaAlta(Models.TemporadaAlta tempA)
        {
            Models.Sorteo.eliminarSorteos(tempA);
            Negocio.TemporadaAlta.eliminar(Invertir(tempA));
            
        
        }

    }
}