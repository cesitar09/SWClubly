using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Datos;

namespace Negocio
{
    public class TemporadaAlta
    {
        public static Entities context()
        {

            return Context.context();
        }
//todos los metodos que devolvian exception ahora devuelven void y se les quita el try/ catch dejando solo lo que estaba dentro del try
        public static void insertar(Datos.TemporadaAlta tempAlta)
        {
                tempAlta.estado = 1;
                context().TemporadaAlta.AddObject(tempAlta);
                context().SaveChanges();
              //Aqui se agregan el loguito de insertar
                Negocio.Util.logito.ElLogeador("Se inserto una temporada Alta", tempAlta.id.ToString());
            
        }

        public static IEnumerable<Datos.TemporadaAlta> seleccionarTodo()
        {
            IEnumerable<Datos.TemporadaAlta> listaTemp = context().TemporadaAlta.Where(p => p.estado != 0);
            return listaTemp;
        }

        public static Datos.TemporadaAlta seleccionarId(short id)
        {
            Datos.TemporadaAlta tempAlta = context().TemporadaAlta.Single(p => p.id == id);
            return tempAlta;
        }

        public static Datos.TemporadaAlta buscarId(short id)
        {
            return context().TemporadaAlta.Single(p => p.id == id);
        }

        public static void modificar(Datos.TemporadaAlta tempAlta)
        {
            
                context().TemporadaAlta.ApplyCurrentValues(tempAlta);
                context().SaveChanges();
              //Aqui se agregan el loguito de modificar
                Negocio.Util.logito.ElLogeador("Se modificar una temporada Alta", tempAlta.id.ToString());
        }

        public static void eliminar(Datos.TemporadaAlta tempAlta)
        {
            
                tempAlta.estado = 0;
                context().TemporadaAlta.ApplyCurrentValues(tempAlta);
                context().SaveChanges();
            //Aqui se agregan el loguito de eliminar
                Negocio.Util.logito.ElLogeador("Se eliminar una temporada Alta", tempAlta.id.ToString());
            
        }

        //Metodo que valida si hay sorteos con ese id
        //public static bool HaySorteos(short id) {
        //    Datos.Sorteo sorteo = new Datos.Sorteo();
        //    sorteo=
        //}


    }
}
