using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Datos
{
    public static class Context
    {
        private static Entities ctx = null;
        public static Entities context()
        {
            if (ctx == null)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "Cadena de conexion.txt";
                System.IO.StreamReader file = new System.IO.StreamReader(path, true);
                string cadena = "metadata=res://*/Model.csdl|res://*/Model.ssdl|res://*/Model.msl;provider=System.Data.SqlClient;" +
                    "provider connection string=';Data Source=" + file.ReadLine() + ";Initial Catalog=" + file.ReadLine() +
                    ";Persist Security Info=True;User ID=" + file.ReadLine() + ";Password=" + file.ReadLine() + ";MultipleActiveResultSets=True;" +
                    "Application Name=EntityFramework';";
                file.Close();
                //string cadenaconexion=CreateEntityConnectionString("Entity", cadena);
                Entities temp = new Entities(cadena);
                temp.ConceptoDePago.FirstOrDefault();
                ctx = temp;
            }
            return ctx;
        }


    }
}
