using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Newtonsoft.Json;
using Web.Controllers;
using Web.Models;
using System.Data;
using System.Data.SqlClient;
using Web.Util;
using Negocio.Util;
using System.Net;
namespace Web.Controllers
{
    public class RRHHController : Controller
    {
        //
        // GET: /Empleado/

        public JsonResult L()
        {
            IEnumerable<Models.TipoEmpleado> Listatipos = Models.TipoEmpleado.seleccionarTodo();
            
            return Json(Listatipos  , JsonRequestBehavior.AllowGet);
        }

        public ActionResult Leer([DataSourceRequest] DataSourceRequest request)
        {
            try
            {
                IEnumerable<Models.Empleado> ListaEmpleado = Models.Empleado.seleccionarTodo();
                DataSourceResult result = ListaEmpleado.ToDataSourceResult(request);
                return Json(result);
            }
            catch (EntityException) {
                return View("MantenerEmpleado");
            }
        }    
            

        public ActionResult Leer_Sedes()
        {
            IEnumerable<Models.Sede> ListaSedes = Models.Sede.SeleccionarTodo();
           
            return Json(ListaSedes, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Leer_Estados()
        {
            IEnumerable<Models.Persona.Estado_Civil> Listaestados = Models.Persona.listestadocivil.ToList();
            return Json(Listaestados, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Leer_Tipos()
        {
            IEnumerable<Models.TipoEmpleado> Listatipos = Models.TipoEmpleado.seleccionarTodo();
           
            return Json(Listatipos, JsonRequestBehavior.AllowGet);
        } 
        public ActionResult FilterMenuCustomization_Nombre()
        {
            return Json(Models.Empleado.seleccionarTodo().Select(e => e.persona.nombre).Distinct(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult FilterMenuCustomization_Sueldo()
        {
            return Json(Models.Empleado.seleccionarTodo().Select(e => e.sueldo).Distinct(), JsonRequestBehavior.AllowGet);
        }

     
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult MantenerEmpleado( Web.Models.Empleado empleado)
        {
            try
            {
                ViewData["Leer_TiposEmpleado"] = Models.TipoEmpleado.seleccionarTodo();
                ViewData["Leer_Sedes"] = Models.Sede.SeleccionarTodo();
                ViewData["Leer_Turnos"] = Models.TurnoDeTrabajo.seleccionarTodo();
                Validar.ValidarIEnumerable(ViewData["Leer_TiposEmpleado"]);
                Validar.ValidarIEnumerable(ViewData["Leer_Sedes"]);
                Validar.ValidarIEnumerable(ViewData["Leer_Turnos"]);
                return View((IView)null);
            }
            catch (SqlException ex)
            {
                ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;
                ViewData["Leer_TiposEmpleado"] = null;
                ViewData["Leer_Sedes"] = null;
                ViewData["Leer_Turnos"] = null;
                logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return View((IView)null);
            }
            catch (EntityException ex) {
                ViewData["message"] = TransactionMessages.ENTITY_EXCEPTION_MESSAGE;
                ViewData["Leer_TiposEmpleado"] = null;
                ViewData["Leer_Sedes"] = null;
                ViewData["Leer_Turnos"] = null;
                logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return View((IView)null);
            }
            //ViewData["message"] = null;
            //return View((IView)null);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Eliminar([DataSourceRequest] DataSourceRequest request, Web.Models.Empleado empleado)
        {
            try
            {
                if (empleado != null)
                {
                    Empleado.eliminar(empleado);
                    ViewData["Leer_TiposEmpleado"] = Models.TipoEmpleado.seleccionarTodo();
                    ViewData["Leer_Sedes"] = Models.Sede.SeleccionarTodo();
                    ViewData["Leer_Turnos"] = Models.TurnoDeTrabajo.seleccionarTodo();
                    Validar.ValidarIEnumerable(ViewData["Leer_TiposEmpleado"]);
                    Validar.ValidarIEnumerable(ViewData["Leer_Sedes"]);
                    Validar.ValidarIEnumerable(ViewData["Leer_Turnos"]);
                    ViewData["message"] = TransactionMessages.OK_CHANGE_DATA_MESSAGE;
                    }
                return View("MantenerEmpleado");
            }
            catch (SqlException ex){
                ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;
                ViewData["Leer_TiposEmpleado"] = null;
                ViewData["Leer_Sedes"] = null;
                ViewData["Leer_Turnos"] = null;
                logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return View("MantenerEmpleado",null);
            }
            catch (EntityException ex)
            {
                ViewData["message"] = TransactionMessages.ENTITY_EXCEPTION_MESSAGE;
                ViewData["Leer_TiposEmpleado"] = null;
                ViewData["Leer_Sedes"] = null;
                ViewData["Leer_Turnos"] = null;
                logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return View("MantenerEmpleado", null);
            }
            catch (InvalidOperationException ex)
            {
                ViewData["message"] = TransactionMessages.SINGLE_NOT_FOUND_MESSAGE;
                ViewData["Leer_TiposEmpleado"] = null;
                ViewData["Leer_Sedes"] = null;
                ViewData["Leer_Turnos"] = null;
                logito.ElLogeador(TransactionMessages.SINGLE_NOT_FOUND_MESSAGE, ex.GetType().ToString());
                return View("MantenerEmpleado", null);
            }
        }

         [AcceptVerbs( HttpVerbs.Post)]
        public ActionResult Guardar(Web.Models.Empleado empleado) {
            try
            {
                
                
                    if (empleado.persona == null || empleado.persona.id == 0)
                    {
                        if (Web.Models.Empleado.existeDni(empleado.persona.dni, empleado.persona.id) == false)
                        {
                            Empleado.insertar(empleado);
                            ViewData["Leer_TiposEmpleado"] = Models.TipoEmpleado.seleccionarTodo();
                            ViewData["Leer_Sedes"] = Models.Sede.SeleccionarTodo();
                            ViewData["Leer_Turnos"] = Models.TurnoDeTrabajo.seleccionarTodo();
                            Validar.ValidarIEnumerable(ViewData["Leer_TiposEmpleado"]);
                            Validar.ValidarIEnumerable(ViewData["Leer_Sedes"]);
                            Validar.ValidarIEnumerable(ViewData["Leer_Turnos"]);
                            ViewData["message"] = TransactionMessages.OK_CHANGE_DATA_MESSAGE;
                        }
                        else {
                            ViewData["message"] = "El DNI ingresado ya existe, ingrese uno distinto";
                        }
                    }
                    else
                    {
                        if (Web.Models.Empleado.existeDni(empleado.persona.dni, empleado.persona.id) == false)
                        {
                            Empleado emp = Empleado.buscarId(empleado.persona.id);

                            Empleado.modificar(emp, empleado);
                            ViewData["Leer_TiposEmpleado"] = Models.TipoEmpleado.seleccionarTodo();
                            ViewData["Leer_Sedes"] = Models.Sede.SeleccionarTodo();
                            ViewData["Leer_Turnos"] = Models.TurnoDeTrabajo.seleccionarTodo();
                            Validar.ValidarIEnumerable(ViewData["Leer_TiposEmpleado"]);
                            Validar.ValidarIEnumerable(ViewData["Leer_Sedes"]);
                            Validar.ValidarIEnumerable(ViewData["Leer_Turnos"]);
                            ViewData["message"] = TransactionMessages.OK_CHANGE_DATA_MESSAGE;
                        }
                        else {
                            ViewData["message"] = "El DNI ingresado ya existe, ingrese uno distinto";
                        }
                    }
                    return View("MantenerEmpleado");
                
            }
            //catch (ConstraintException) {
            //    return View("MantenerEmpleado", empleado);
            //}
            catch (SqlException ex)
            {
                ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;
                ViewData["Leer_TiposEmpleado"] = null;
                ViewData["Leer_Sedes"] = null;
                ViewData["Leer_Turnos"] = null;
                logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return View("MantenerEmpleado", null);
            }
            catch (EntityException ex)
            {
                ViewData["message"] = TransactionMessages.ENTITY_EXCEPTION_MESSAGE;
                ViewData["Leer_TiposEmpleado"] = null;
                ViewData["Leer_Sedes"] = null;
                ViewData["Leer_Turnos"] = null;
                logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return View("MantenerEmpleado", null);
            }
            catch (InvalidOperationException ex)
            {
                ViewData["message"] = TransactionMessages.SINGLE_NOT_FOUND_MESSAGE;
                ViewData["Leer_TiposEmpleado"] = null;
                ViewData["Leer_Sedes"] = null;
                ViewData["Leer_Turnos"] = null;
                logito.ElLogeador(TransactionMessages.SINGLE_NOT_FOUND_MESSAGE, ex.GetType().ToString());
                return View("MantenerEmpleado", null);
            }
        }

       
        public ActionResult Editar(Web.Models.Empleado empleado) {
            try
            {
                if (empleado != null)
                {
                    ViewData["Leer_TiposEmpleado"] = Models.TipoEmpleado.seleccionarTodo();
                    ViewData["Leer_Sedes"] = Models.Sede.SeleccionarTodo();
                    ViewData["Leer_Turnos"] = Models.TurnoDeTrabajo.seleccionarTodo();
                    Validar.ValidarIEnumerable(ViewData["Leer_TiposEmpleado"]);
                    Validar.ValidarIEnumerable(ViewData["Leer_Sedes"]);
                    Validar.ValidarIEnumerable(ViewData["Leer_Turnos"]);
                    empleado = Empleado.buscarId(empleado.persona.id);
                }
                return View("MantenerEmpleado", empleado);
            }
            catch (SqlException ex)
            {
                ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;
                ViewData["Leer_TiposEmpleado"] = null;
                ViewData["Leer_Sedes"] = null;
                ViewData["Leer_Turnos"] = null;
                logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return View("MantenerEmpleado", null);
            }
            catch (InvalidOperationException ex)
            {
                ViewData["message"] = TransactionMessages.SINGLE_NOT_FOUND_MESSAGE;
                ViewData["Leer_TiposEmpleado"] = null;
                ViewData["Leer_Sedes"] = null;
                ViewData["Leer_Turnos"] = null;
                logito.ElLogeador(TransactionMessages.SINGLE_NOT_FOUND_MESSAGE, ex.GetType().ToString());
                return View("MantenerEmpleado", null);
            }
            catch (EntityException ex)
            {
                ViewData["message"] = TransactionMessages.ENTITY_EXCEPTION_MESSAGE;
                ViewData["Leer_TiposEmpleado"] = null;
                ViewData["Leer_Sedes"] = null;
                ViewData["Leer_Turnos"] = null;
                logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return View("MantenerEmpleado", null);
            }

            }

        //ASISTENCIA/////////////////////////////////////////////////////

        public ActionResult Asistencia() 
        {
            return View((IView)null);
        }

        [HttpPost]
        public JsonResult Asistencia(string data)
        {
            try
            {
                long numero = Convert.ToInt64(data);
                Empleado emp = Models.Empleado.buscarId(numero);
                if (emp != null)
                    return Json(new { me = emp.persona.nombre });
                else return Json("");
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AñadirAsistencia(Web.Models.Empleado empleado)
        {
            try
            {
                if (empleado != null)
                {
                    int val = Models.Asistencia.InsertarAsistencia(empleado);
                    if (val == 0)
                    {
                        Negocio.Util.logito.ElLogeador("Codigo o Nombre Ingresados no existen", empleado.persona.nombre);
                        ViewData["message"] = "N";
                    }
                    else if (val == 1)
                    {
                        Negocio.Util.logito.ElLogeador("Registro de entrada", empleado.persona.nombre);
                        ViewData["message"] = "EA";
                    }
                    else if (val == 2)
                    {
                        Negocio.Util.logito.ElLogeador("Registro de salida", empleado.persona.nombre);
                        ViewData["message"] = "EM";
                    }
                    else if (val == 3)
                    {
                        Negocio.Util.logito.ElLogeador("No pudo pasar asistencia", empleado.persona.nombre);
                        ViewData["message"] = "F";
                    }
                    else if (val == 4)
                    {
                        Negocio.Util.logito.ElLogeador("No pudo pasar asistencia, ya los paso habia pasado", empleado.persona.nombre);
                        ViewData["message"] = "YAAM";
                    }
                    else if (val == 5)
                    {
                        Negocio.Util.logito.ElLogeador("No pudo registrar salida, aun no se cumple el minimo de horas", empleado.persona.nombre);
                        ViewData["message"] = "ANC";
                    }
                }
                return View("Asistencia", empleado);
            }
            catch (SqlException)
            {
                ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;
                Negocio.Util.logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, empleado.persona.nombre);
                return View("Asistencia",(IView)null);
            }
            catch (EntityException)
            {
                ViewData["message"] = TransactionMessages.ENTITY_EXCEPTION_MESSAGE;
                Negocio.Util.logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, empleado.persona.nombre);
                return View("Asistencia",(IView)null);
            }
            catch (InvalidOperationException)
            {
                ViewData["message"] = TransactionMessages.SINGLE_NOT_FOUND_MESSAGE;
                Negocio.Util.logito.ElLogeador(TransactionMessages.SINGLE_NOT_FOUND_MESSAGE, empleado.persona.nombre);
                return View("Asistencia",(IView)null);
            }
        }

    }
}
