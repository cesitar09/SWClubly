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
using Negocio.Util;

namespace Web.Models
{
    public class Bungalow
    {
        public short id { get; set; }

        [DisplayName("*Número")]
        public short numero { get; set; }

        [DisplayName("*Sede")]
        public Models.Sede sede { get; set; }

        [DisplayName("*Tipo de Bungalow")]
        public Models.TipoBungalow tipobungalow { get; set; }

        [DisplayName("*Estado")]
        public string estado { get; set; }
        [DisplayName("Coordenada X")]
        public short? X { get; set; }
        [DisplayName("Coordenada Y")]
        public short? Y { get; set; }

        public static ListaEstados listaEstados = new ListaEstados();
        public static List<Estado_Bungalow> listestadobung { get; set; }

        public class Estado_Bungalow {
            public short id { get; set; }
            public string nombre { get; set; }

            public Estado_Bungalow(short i, string n) {
                id = i;
                nombre = n;
            }
        }

        static Bungalow() {
            listaEstados = new ListaEstados();
            listaEstados.AgregarEstado(1, "Habilitado");
            listaEstados.AgregarEstado(2, "Inhabilitado");
            Estado_Bungalow estado1 = new Estado_Bungalow(1, "Habilitado");
            Estado_Bungalow estado2 = new Estado_Bungalow(2, "Inhabilitado");
            listestadobung = new List<Estado_Bungalow>();
            listestadobung.Add(estado1);
            listestadobung.Add(estado2);
        }

        public Bungalow() { }

        public Bungalow(Datos.Bungalow bungalow)
        {
            id = bungalow.id;
            numero = bungalow.numero;
            sede = Sede.Convertir(bungalow.Sede);
            tipobungalow = TipoBungalow.Convertir(bungalow.TipoBungalow);
            estado = listaEstados.TextoEstado(bungalow.estado);
            X = bungalow.posX;
            Y = bungalow.posY;
        }

        public static bool HayReserva(Models.Bungalow bung) {
            return Negocio.Bungalow.HayReserva(bung.id);
        }

        public static IEnumerable<Bungalow> ConvertirLista(IEnumerable<Datos.Bungalow> listaBungalows)
        {
            return listaBungalows.Select(bungalow => Convertir(bungalow));
        }

        public static IEnumerable<Bungalow> BuscarSede(short idsede)
        {
            IEnumerable<Datos.Bungalow> bungalow = Negocio.Bungalow.seleccionarTodo().Where(p => p.Sede.id == idsede);
            if (bungalow != null)
                return ConvertirLista(bungalow);
            else
                return null;
        }

        public static Bungalow Convertir(Datos.Bungalow bungalow)
        {
            return new Bungalow(bungalow);
        }

        public static Datos.Bungalow Invertir(Models.Bungalow mBungalow)
        {
            Datos.Bungalow dBungalow;
            if (mBungalow.id == 0)
                dBungalow = new Datos.Bungalow();
            else
                dBungalow = Negocio.Bungalow.buscarId(mBungalow.id);
            dBungalow.numero = mBungalow.numero;
            dBungalow.estado = listaEstados.EstadoTexto(mBungalow.estado);
            dBungalow.Sede = Negocio.Sede.buscarId(mBungalow.sede.id);
            dBungalow.TipoBungalow = Negocio.TipoBungalow.buscarId(mBungalow.tipobungalow.id);
            dBungalow.posX = mBungalow.X;
            dBungalow.posY = mBungalow.Y;
            return dBungalow;
        }

        public static IEnumerable<Datos.Bungalow> ConvertirListaInverso(IEnumerable<Models.Bungalow> mBungalow)
            //Nota Gustavo: Este metodo no va a funcionar. Deberia llamarse InvertirLista y devolver un EntityCollection.
            //En el model de Empleado esta correctamente implementado
        {
            return mBungalow.Select(b => Invertir(b));
        }

        public static Models.Bungalow buscarId(short id)
        {
            return Convertir(Negocio.Bungalow.buscarId(id));
        }

        public static IEnumerable<Bungalow> SeleccionarTodo()
        {
            IEnumerable<Datos.Bungalow> bungalow = Negocio.Bungalow.seleccionarTodo();
            return ConvertirLista(bungalow);
        }

        public static Models.Bungalow SeleccionarporId(short id)
        {
            return Convertir(Negocio.Bungalow.buscarId(id));
        }

        public static void modificarBungalow(Models.Bungalow bungalow)
        {
            Negocio.Bungalow.modificar(Invertir(bungalow));
        }

        //Existe numero
        public static bool ExisteNumero(Models.Bungalow bung)
        {

            return Negocio.Bungalow.ExisteNumero(bung.numero);
        }


        public static void insertarBungalow(Models.Bungalow bungalow)
        {
            Negocio.Bungalow.insertar(Invertir(bungalow));
        }

        public static void eliminarBungalow(Models.Bungalow bungalow)
        {
            Negocio.Bungalow.eliminar(Negocio.Bungalow.buscarId(bungalow.id));
        //    if (bungalow.estado == 1)
        //        Negocio.Bungalow.inhabilitar(Invertir(bungalow));
        //    if (bungalow.estado == 2)
        //        Negocio.Bungalow.habilitar(Invertir(bungalow));
        }



    }
}
