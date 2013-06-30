using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Datos;

namespace Negocio
{
    public class Parametros
    {
        public const short INHABILITADO = 1;
        public const short HABILITADO = 2;
        
        public static Entities context()
        {
            return Context.context();
        }

        public static void insertar(Datos.Parametros parametro)
        {
                
                context().Parametros.AddObject(parametro);
                context().SaveChanges();
                Negocio.Util.logito.ElLogeador("Se inserto el parametro", parametro.id.ToString());

        }

        public static void Modificar(Datos.Parametros parametro)
        {

            context().Parametros.ApplyCurrentValues(parametro);
            context().SaveChanges();
            Negocio.Util.logito.ElLogeador("Se modifico el parametro", parametro.id.ToString());
        }

        public static IEnumerable<Datos.Parametros> seleccionarTodo()
        {
                IEnumerable<Datos.Parametros> listaParametros = context().Parametros.Where(p => p.estado > 0);
                return listaParametros;
        }


        public static Datos.Parametros buscarId(short id)
        {
            return context().Parametros.SingleOrDefault(p => p.id == id);
        }


        public static IEnumerable<Datos.Parametros> seleccionarValido()
        {
            
                IEnumerable<Datos.Parametros> listaParametros = context().Parametros.Where(p => p.estado > 0 );
                return listaParametros;
        }

        public static Datos.Parametros SeleccionarParametros()
        {
            return Context.context().Parametros.First(p=>p.estado==HABILITADO && p.fechaFinal==null);
        }


       
    }
}
