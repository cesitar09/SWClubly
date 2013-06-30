using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Kendo.Mvc.UI;
using Web.Controllers;
using Web.Models;
using System.Data.Linq;
using Kendo.Mvc.Extensions;
using System.Web.Script.Serialization;
using Web.Util;
using System.Net;
using System.Data.SqlClient;
using System.Data;


namespace Web.Controllers
{
    public class ActividadController : Controller
    {
        // GET: /InscribirActividadSocio/
        public ActionResult InscribirseEnActividad(Models.Actividad actividad)
        {
            return View(actividad);
        }


 //PRIMERA VISTA
    //Metodo para mostrar en la tabla kendo
        public ActionResult LeerActividadesDisponibles([DataSourceRequest] DataSourceRequest request)
        {
            try
            {
                IEnumerable<Models.Actividad> ListaActividades = Models.Actividad.SeleccionarActividadesDisponiblesSocio();
                DataSourceResult result = ListaActividades.ToDataSourceResult(request);
                return Json(result);
            }
            catch (Exception e)
            {
                Console.WriteLine("Excepcion en GestionarActividadController\n", e);
                DataSourceResult result = new DataSourceResult();
                result.Errors = "error";
                return Json(result);
            }
        }

        //Metodo que llama a la segunda vista y muestra los familiares a inscribirse
        public ActionResult RealizarInscripcion(Models.Actividad actividad)
        {
            try
            {
                Models.Actividad actividadXid = Models.Actividad.buscarId(actividad.id);
                return View(actividadXid);
            }
            catch
            {
                ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;    //no se pudo conectar a la bd
                return View();
            }
        }


//SEGUNDA VISTA
        //Muestra a los socio No inscritos para que se puedan inscribir
        //Esto es para la tabla kendo, que te da todos los socios
        public ActionResult LeerSociosNoInscritos([DataSourceRequest] DataSourceRequest request, Models.Actividad actividad)
        {
            try
            {
                short idUsuario = (short)Session["IdUsuario"];
                Familia familia = Models.Familia.buscarIdUsuario(idUsuario);
                IList<Models.Socio> listaSocios = Models.Socio.BuscarIdFamilia(familia.id).ToList();
                IEnumerable<Models.SocioXActividad> listaInscritos = Models.SocioXActividad.BuscarIdActividadIdFamilia(actividad.id, familia.id);
                foreach (SocioXActividad inscrito in listaInscritos)
                {
                    listaSocios.Remove(listaSocios.FirstOrDefault(s => s.persona.id == inscrito.idSocio));
                }
                DataSourceResult result = listaSocios.ToDataSourceResult(request);
                return Json(result);
            }
            catch
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("");    //Mensaje en la vista
            }
        }

        // Esto es para devolver la lista de socios para el listView
        public ActionResult LeerSocios([DataSourceRequest] DataSourceRequest request)
        {
                IEnumerable<Models.Socio> listaSocios = Models.Socio.BuscarIdFamilia(10001);
                DataSourceResult result = listaSocios.ToDataSourceResult(request);
                return Json(result);          
        }
  
        // Esto es para mi tabla kendo, que te da toda la data a mostrar Todos los socios en una actividad especifica de una familia
        public ActionResult LeerSocioxActividad([DataSourceRequest] DataSourceRequest request, Models.Actividad actividad)
        {
            try
            {
                short idUsuario = (short)Session["IdUsuario"];
                Familia familia = Models.Familia.buscarIdUsuario(idUsuario);
                IEnumerable<Models.SocioXActividad> ListaSocioXActividades = Models.SocioXActividad.BuscarIdActividadIdFamilia(actividad.id, familia.id);
                DataSourceResult result = ListaSocioXActividades.ToDataSourceResult(request);
                return Json(result);
            }
            catch
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("");    //Mensaje en la vista
            }
        }


        
        //Agrega socio en actividad
        //NO SE USA, SE USA EL METODO EN GESTIONARACTIVIDADCONTROLLER
        public ActionResult AgregarSocioEnActividad(string strIdSocio, string strIdActividad)
        {
            try
            {
                //var js = new JavaScriptSerializer();
                // short idSocio = (short)js.DeserializeObject(strIdSocio);
                // short idActividad = (short)js.DeserializeObject(strIdActividad);
                short idSocio = short.Parse(strIdSocio);
                short idActividad = short.Parse(strIdActividad);
                Models.SocioXActividad.Insertar(idSocio, idActividad);
                Models.Actividad actividad = Models.Actividad.buscarId(idActividad);
                ViewData["message"] = TransactionMessages.OK_ADD_DATA_MESSAGE;
                return View("InscribirSocioEnActividad", actividad);
            }
            catch (SqlException)
            {
                ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;
                return View("InscribirSocioEnActividad", null);
            }
            catch (EntityException)
            {
                ViewData["message"] = TransactionMessages.ENTITY_EXCEPTION_MESSAGE;
                return View("InscribirSocioEnActividad", null);
            }
            catch (InvalidOperationException)
            {
                ViewData["message"] = TransactionMessages.SINGLE_NOT_FOUND_MESSAGE;
                return View("InscribirSocioEnActividad", null);
            }
            
        }

        //Cambia el estado de 1 a 0 en socioXActividad
        //NO SE USA, SE USA EL METODO EN GESTIONARACTIVIDADCONTROLLER
        public ActionResult EliminarInscripcion(string strIdSocio, string strIdActividad)
        {
            short idSocio = short.Parse(strIdSocio);
            short idActividad = short.Parse(strIdActividad);
            if ((idSocio != 0) && (idActividad != 0))
            {
                SocioXActividad.Eliminar(idSocio, idActividad);
            }

            return Json("");
        }

        //<! <input type='button' value= 'Finalizar'  id='Finalizar' onclick='Finalizar()' class='button fright'/> -->
        //Se utiliza para llamar a la tercera Vista para mostral el historial de las actividades que el socio esta inscrito
        public ActionResult VerActividadesXsocio()
        {   
            return View();
        }

//TERCERA VISTA
 
        //// Esto es para mi tabla kendo, que te da toda la data a mostrar socioxActiviad
        public ActionResult LeerActividadxSocio([DataSourceRequest] DataSourceRequest request)
        {
            try
            {
                short idUsuario = (short)Session["IdUsuario"];
                Familia familia = Models.Familia.buscarIdUsuario(idUsuario);
                IEnumerable<Models.Actividad> ListaSocioXActividades = Models.Actividad.BuscarActividadIdFamilia(familia.id);

                DataSourceResult result = ListaSocioXActividades.ToDataSourceResult(request);
                return Json(result);
            }
            catch
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("");    //Mensaje en la vista
            }
        }
        
        //Metodo que llama el Cancelar y cancela todo para mostrar la pagina inicial otra vez
        public ActionResult CancelarInscripcionTotal()
        {
            return RedirectToAction("InscribirseEnActividad", "Actividad");
        }



        //Cancelar a uno de los socios para ir a la actividad 
        public ActionResult CancelarInscripcion(string strIdSocio,string strIdActividad)
        {
            short idSocio = short.Parse(strIdSocio);
            short idActividad = short.Parse(strIdActividad);
            if ((idSocio != 0) && (idActividad != 0))
            {
                SocioXActividad.EliminarTodo(idSocio,idActividad);
            }

            return Json("");
        }

        public ActionResult EditarInscripcion(Web.Models.Actividad actividad)
        {
            actividad = Actividad.buscarId(actividad.id);
            return View("InscribirseEnActividad", actividad);

        }
        
    }
}

    

