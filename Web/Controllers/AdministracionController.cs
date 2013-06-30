﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Newtonsoft.Json;
using Kendo.Mvc.UI;
using Web.Controllers;
using Web.Models;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using Negocio.Util;
using System.ComponentModel.DataAnnotations;
using Web.Util;
using System.Net;

namespace Web.Controllers
{
    [HandleError]
    public class AdministracionController : Controller
    {
        //
        // GET: /Administracion/

//........Concepto de pago

        public ActionResult getConceptoPago([DataSourceRequest] DataSourceRequest request)
        {
            IEnumerable<Models.ConceptoDePago> ListaConceptoDePago = Models.ConceptoDePago.SeleccionarTodo();
            DataSourceResult result = ListaConceptoDePago.ToDataSourceResult(request);
            return Json(result);
        }

        public ActionResult EditarInscripcion(Web.Models.ConceptoDePago concepto)
        {
            concepto = ConceptoDePago.SeleccionarporId(concepto.id);
            return View("TiposPago", concepto);
        }

        public ActionResult FilterMenuCustomization_Nombre()
        {
            return Json(Negocio.ConceptoDePago.SeleccionarTodoTiposDePago().Select(e => e.nombre).Distinct(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult FilterMenuCustomization_Monto()
        {
            return Json(Negocio.ConceptoDePago.SeleccionarTodoTiposDePago().Select(e => e.monto).Distinct(), JsonRequestBehavior.AllowGet);
        } 

        //public ActionResult Remote_Data()
        //{
        //    return View("AjaxBinding");
        //}

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult TiposPago(Web.Models.ConceptoDePago conceptoDePago)
        {
            return View(conceptoDePago);
        }

        public ActionResult getActividad([DataSourceRequest] DataSourceRequest request)
        {
            IEnumerable<Models.Actividad> ListaActividad = Models.Actividad.SeleccionarTodo();
            DataSourceResult result = ListaActividad.ToDataSourceResult(request);
            return Json(result);
        }

        public ActionResult MantenerActividad() 
        {
            Datos.Actividad actividad = new Datos.Actividad();
            Models.Actividad Actividad = new Models.Actividad(actividad);
            return View(Actividad);
        }

        public ActionResult FilterActividad_Nombre()
        {
            return Json(Negocio.Actividad.SeleccionarTodo().Select(e => e.nombre).Distinct(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult FilterActividad_Precio()
        {
            return Json(Negocio.Actividad.SeleccionarTodo().Select(e => e.precio).Distinct(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CancelarTipoDePago()
        {
            return RedirectToAction("TiposPago", "Administracion");
        }


        [HttpPost]
        public ActionResult AñadirTipoDePago(Web.Models.ConceptoDePago concepto) 
        {
            if (concepto != null) 
            {
                    if (concepto.nombre != null && concepto.descripcion != null)
                    {
                        if (concepto.id == 0)
                        {
                            //concepto.estado = 1;
                            //ConceptoDePago.insertarConceptoDePago(concepto);
                        }
                        else
                        {
                            ConceptoDePago.modificarConceptoDePago(concepto);
                        }
                    }   
            }
            return View("TiposPago",concepto);
        }


        /////////////Mantenimiento Productos//////////////////////////
        public ActionResult getProducto([DataSourceRequest] DataSourceRequest request)
        {
            IEnumerable<Models.Producto> ListaProducto = Models.Producto.SeleccionarTodo();
            DataSourceResult result = ListaProducto.ToDataSourceResult(request);
            return Json(result);
        }

        public ActionResult EditarProducto(Web.Models.Producto producto)
        {
            producto = Producto.SeleccionarporId(producto.id);
            return View("MantenimientoProducto", producto);
        }

        public ActionResult FilterMenuCustomization_NombreProd()
        {
            return Json(Negocio.Producto.SeleccionarTodoProducto().Select(e => e.nombre).Distinct(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult FilterMenuCustomization_PrecUnitarioProd()
        {
            return Json(Negocio.Producto.SeleccionarTodoProducto().Select(e => e.precioUnitario).Distinct(), JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult MantenimientoProducto(Web.Models.Producto producto)
        {
            ViewData["message"] = null;
            return View((IView)null);
        }

        public ActionResult CancelarProducto()
        {
            return RedirectToAction("MantenimientoProducto", "Administracion");
        }

        [HttpPost]
        public ActionResult AñadirProducto(Web.Models.Producto producto)
        {
            if (producto != null)
            { 
                    if (producto.nombre != null && producto.descripcion != null)
                    {
                        if (producto.id == 0)
                        {
                            producto.estado = 1;
                            if (Producto.insertarProducto(producto) == 1)
                            {
                                ViewData["message"] = "E";
                            }
                            else { ViewData["message"] = "F"; }
                        }
                        else
                        {
                            if (Producto.modificarProducto(producto) == 1)
                            {
                                ViewData["message"] = "E";
                            }
                            else { ViewData["message"] = "F"; }
                        }
                    } 
            }
            return View("MantenimientoProducto", producto);
        }
         
        [HttpPost]
        public ActionResult EliminarProducto(Web.Models.Producto producto) 
        {
            if (producto != null)
            {
                Producto.eliminar(producto);  
            }
            return View("MantenimientoProducto", producto);
        }


        // Mantener Servicios
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult MantenerServicio(Web.Models.Servicio servicio)
        {

            
            return View((IView)null);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsertarServicio(Web.Models.Servicio servicio)
        {
            try
            {
                if (servicio.sedesAux != null)
                {
                    List<Sede> lista = new List<Sede>(servicio.sedesAux.Count());
                    foreach (short codigo in servicio.sedesAux)
                    {
                        lista.Add(Sede.buscarId(codigo));
                    }
                    servicio.sedes = lista.AsEnumerable();
                }

                if (servicio.id > 0)
                {
                    Servicio.modificar(servicio);
                    return View("MantenerServicio", servicio);
                }
                else
                {
                    servicio.estado = ListaEstados.ESTADO_ACTIVO;
                    Servicio.insertar(servicio);
                    return View("MantenerServicio", servicio);
                }

            }
            catch (ConstraintException)
            {
                return View("MantenerServicio", servicio);
            }
            catch (UpdateException)
            {
                return View("MantenerServicio", servicio);
            }
            catch (SqlException)
            {
                return View("MantenerServicio", servicio);
            }
            catch (ValidationException)
            {
                return View("MantenerServicio", servicio);
            }
        }

        public ActionResult EditarServicio(Web.Models.Servicio servicio)
        {
            servicio = Servicio.buscarId(servicio.id);
            return View("MantenerServicio", servicio);
        }

        public ActionResult LeerServicio([DataSourceRequest] DataSourceRequest request)
        {
            IEnumerable<Models.Servicio> lista = Servicio.SeleccionarTodo();
            DataSourceResult result = lista.ToDataSourceResult(request);
            return Json(result);
        }

        public ActionResult EliminarServicio(Web.Models.Servicio servicio)
        {
            Servicio.eliminar(servicio);
            return View("MantenerServicio", servicio);
        }

        public IEnumerable<String> EnlistarSedes()
        {
            return Servicio.listaSedes();
        }


        


        /****************TEMPORADA ALTA*******/

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        //try y catch de dos tipos SQl y Exception
        public ActionResult MantenerTemporadaAlta(Web.Models.TemporadaAlta tempA)
        {
            try
            {
                ViewData["message"] = null;
                return View((IView)null);
            }
            catch (SqlException ex)
            {
                ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;
                logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return View((IView)null);
            }
            catch (EntityException ex)
            {
                ViewData["message"] = TransactionMessages.ENTITY_EXCEPTION_MESSAGE;
                logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return View((IView)null);
            }
        }

        //try y catch de dos tipos SQl y Exception
        public ActionResult getTemporadaAlta([DataSourceRequest] DataSourceRequest request)
        {
            try
            {
                IEnumerable<Datos.TemporadaAlta> ListaTempA = Negocio.TemporadaAlta.seleccionarTodo();
                DataSourceResult result = Models.TemporadaAlta.ConvertirLista(ListaTempA).ToDataSourceResult(request);
                return Json(result);
            }
            catch (EntityException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return Json("");
            }
            catch (SqlException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return Json("");
            }
        }

        //try catch de tres tipos
        // [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult eliminarTemporadaAlta([DataSourceRequest] DataSourceRequest request, Web.Models.TemporadaAlta tempA)
        {
            try
            {
            //if (Models.TemporadaAlta.HaySorteos(tempA) == false){
                if (tempA != null)
                {
                    TemporadaAlta.eliminarTemporadaAlta(tempA);
                }

                return View("MantenerTemporadaAlta", (IView)null);
            //
            }
            catch (SqlException ex)
            {
                ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;
                logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return View("MantenerTemporadaAlta", (IView)null);
            }
            catch (EntityException ex)
            {
                ViewData["message"] = TransactionMessages.ENTITY_EXCEPTION_MESSAGE;
                logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return View("MantenerTemporadaAlta", (IView)null);
            }
            catch (InvalidOperationException ex)
            {
                ViewData["message"] = TransactionMessages.SINGLE_NOT_FOUND_MESSAGE;
                logito.ElLogeador(TransactionMessages.SINGLE_NOT_FOUND_MESSAGE, ex.GetType().ToString());
                return View("MantenerTemporadaAlta", (IView)null);
            }
           
        }


        public ActionResult modificarTemporadaAlta(Web.Models.TemporadaAlta tempA)
        {
            //try y catch de los tres tipos
            try
            {
                tempA = TemporadaAlta.SeleccionarporId(tempA.id);

                return View("MantenerTemporadaAlta", tempA);
            }
            catch (SqlException ex)
            {
                ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;
                logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return View("MantenerTemporadaAlta", (IView)null);
            }
            catch (EntityException ex)
            {
                ViewData["message"] = TransactionMessages.ENTITY_EXCEPTION_MESSAGE;
                logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return View("MantenerTemporadaAlta", (IView)null);
            }
            catch (InvalidOperationException ex)
            {
                ViewData["message"] = TransactionMessages.SINGLE_NOT_FOUND_MESSAGE;
                logito.ElLogeador(TransactionMessages.SINGLE_NOT_FOUND_MESSAGE, ex.GetType().ToString());
                return View("MantenerTemporadaAlta", (IView)null);
            }

        }

        public bool ValidarFechas(Models.TemporadaAlta tempA) {
            if (tempA.fechaFin > tempA.fechaInicio) return true;
             return false;
        }


        [HttpPost]
        //Try y catch de los 3 tipos
        public ActionResult agregarTemporadaAlta(Web.Models.TemporadaAlta tempA)
        {
            try
            {
                if (ValidarFechas(tempA))
                {
                    if (tempA == null || tempA.id == 0)
                    {

                        tempA.estado = ListaEstados.ESTADO_ACTIVO;
                        TemporadaAlta.insertarTemporadaAlta(tempA);
                        //mensaje de exito en insertar
                        ViewData["message"] = TransactionMessages.OK_ADD_DATA_MESSAGE;
                    }
                    else
                    {
                        TemporadaAlta.modificarTemporadaAlta(tempA);
                        //mensaje de exito en modificar
                        ViewData["message"] = TransactionMessages.OK_CHANGE_DATA_MESSAGE;
                    }
                }
                else
                    ViewData["message"] = "El rango de fechas ingresadas no es valido";
                return View("MantenerTemporadaAlta", tempA);

            }           
                    
            catch (SqlException ex)
            {
                ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;
                logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return View("MantenerTemporadaAlta", (IView)null);
            }
            catch (EntityException ex)
            {
                ViewData["message"] = TransactionMessages.ENTITY_EXCEPTION_MESSAGE;
                logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return View("MantenerTemporadaAlta", (IView)null);
            }
            catch (InvalidOperationException ex)
            {
                ViewData["message"] = TransactionMessages.SINGLE_NOT_FOUND_MESSAGE;
                logito.ElLogeador(TransactionMessages.SINGLE_NOT_FOUND_MESSAGE, ex.GetType().ToString());
                return View("MantenerTemporadaAlta", (IView)null);
            }
            
        }

//----------------------------Mantener Parametros: Dorita ------------------------//
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult MantenerParametros(Web.Models.Parametros parametro)
        {
            try
            {
                return View((IView)null);
            }
            catch (SqlException ex)
            {
                ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;
                logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return View((IView)null);
            }
            catch (EntityException ex)
            {
                ViewData["message"] = TransactionMessages.ENTITY_EXCEPTION_MESSAGE;
                logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return View((IView)null);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsertarParametros(Web.Models.Parametros parametro)
        {
            try
            {
                    Parametros.insertarParametros(parametro);
                    ViewData["message"] = TransactionMessages.OK_ADD_DATA_MESSAGE;
                    return View("MantenerParametros", parametro);
            }
            catch (SqlException ex)
            {
                ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;
                logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return View("MantenerParametros", (IView)null);
            }
            catch (EntityException ex)
            {
                ViewData["message"] = TransactionMessages.ENTITY_EXCEPTION_MESSAGE;
                logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return View("MantenerParametros", (IView)null);
            }
            catch (InvalidOperationException ex)
            {
                ViewData["message"] = TransactionMessages.SINGLE_NOT_FOUND_MESSAGE;
                logito.ElLogeador(TransactionMessages.SINGLE_NOT_FOUND_MESSAGE, ex.GetType().ToString());
                return View("MantenerParametros", (IView)null);
            }
        }

       public ActionResult LeerParametros([DataSourceRequest] DataSourceRequest request)
        {
            try
            {
                IEnumerable<Models.Parametros> lista = Parametros.SeleccionarTodo();
                DataSourceResult result = lista.ToDataSourceResult(request);
                return Json(result);
            }
            catch (EntityException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return Json("");
            }
            catch (SqlException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, ex.GetType().ToString());
                return Json("");
            }
        }

//-----------------------------------Agregar Cuotas: Dorita--------------------------------------------
       [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
       public ActionResult AgregarCuotas(Web.Models.Pago cuota)
       {

           try
           {
               ViewData["familiaSeleccionarTodo"] = Familia.SeleccionarTodo();
               Validar.ValidarIEnumerable(ViewData["familiaSeleccionarTodo"]);
               return View((IView)null);
           }
           catch (SqlException ex)
           {
               ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;
               logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, ex.GetType().ToString());
               ViewData["familiaSeleccionarTodo"] = null;
               return View((IView)null);
           }
           catch (EntityException ex)
           {
               ViewData["message"] = TransactionMessages.ENTITY_EXCEPTION_MESSAGE;
               logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, ex.GetType().ToString());
               ViewData["familiaSeleccionarTodo"] = null;
               return View((IView)null);
           }
       }

       [AcceptVerbs(HttpVerbs.Post)]
       public ActionResult InsertarCuotas(Web.Models.Pago cuota)
       {
           try
           {
               
               Pago.InsertarCuota(cuota);
               ViewData["message"] = TransactionMessages.OK_ADD_DATA_MESSAGE;
               ViewData["familiaSeleccionarTodo"] = Familia.SeleccionarTodo();
               Validar.ValidarIEnumerable(ViewData["familiaSeleccionarTodo"]);
               return View("AgregarCuotas", cuota);
           }
           catch (SqlException ex)
           {
               ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;
               logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, ex.GetType().ToString());
               ViewData["familiaSeleccionarTodo"] = null;
               return View("AgregarCuotas", (IView)null);
           }
           catch (EntityException ex)
           {
               ViewData["message"] = TransactionMessages.ENTITY_EXCEPTION_MESSAGE;
               logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, ex.GetType().ToString());
               ViewData["familiaSeleccionarTodo"] = null;
               return View("AgregarCuotas", (IView)null);
           }
           catch (InvalidOperationException ex)
           {
               ViewData["message"] = TransactionMessages.SINGLE_NOT_FOUND_MESSAGE;
               logito.ElLogeador(TransactionMessages.SINGLE_NOT_FOUND_MESSAGE, ex.GetType().ToString());
               ViewData["familiaSeleccionarTodo"] = null;
               return View("AgregarCuotas", (IView)null);
           }
       }

       public ActionResult LeerCuotas([DataSourceRequest] DataSourceRequest request)
       {
           try
           {
               IEnumerable<Models.Pago> lista = Pago.SeleccionarCuotas();
               DataSourceResult result = lista.ToDataSourceResult(request);
               return Json(result);
           }
           catch (EntityException ex)
           {
               Response.StatusCode = (int)HttpStatusCode.BadRequest;
               logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, ex.GetType().ToString());
               return Json("");
           }
           catch (SqlException ex)
           {
               Response.StatusCode = (int)HttpStatusCode.BadRequest;
               logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, ex.GetType().ToString());
               return Json("");
           }
       }

       public ActionResult EliminarCuotas([DataSourceRequest] DataSourceRequest request, Web.Models.Pago cuota)
       {
           try
           {
               Pago.Cancelar(cuota);
               ViewData["familiaSeleccionarTodo"] = Familia.SeleccionarTodo();
               Validar.ValidarIEnumerable(ViewData["familiaSeleccionarTodo"]);
               return View("AgregarCuotas", (IView)null);

           }
           catch (SqlException ex)
           {
               ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;
               logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, ex.GetType().ToString());
               ViewData["familiaSeleccionarTodo"] = null;
               return View("AgregarCuotas", (IView)null);
           }
           catch (EntityException ex)
           {
               ViewData["message"] = TransactionMessages.ENTITY_EXCEPTION_MESSAGE;
               logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, ex.GetType().ToString());
               ViewData["familiaSeleccionarTodo"] = null;
               return View("AgregarCuotas", (IView)null);
           }
           catch (InvalidOperationException ex)
           {
               ViewData["message"] = TransactionMessages.SINGLE_NOT_FOUND_MESSAGE;
               logito.ElLogeador(TransactionMessages.SINGLE_NOT_FOUND_MESSAGE, ex.GetType().ToString());
               ViewData["familiaSeleccionarTodo"] = null;
               return View("AgregarCuotas", (IView)null);
           }
       }

//------Pagos de membresia
        [HttpGet]
       public ActionResult GenerarPagosMembresia([DataSourceRequest] DataSourceRequest request)
       {
           try
           {
               if (Negocio.Pago.BuscarMes())
               {
                   Pago.GenerarPagosMembresia();
                   ViewData["message"] = TransactionMessages.OK_ADD_DATA_MESSAGE;
                   
               }
               else
               {
                   ViewData["message"] = "Ya se ha registrado un pago de membresia para este mes";
               }
               ViewData["familiaSeleccionarTodo"] = Familia.SeleccionarTodo();
               Validar.ValidarIEnumerable(ViewData["familiaSeleccionarTodo"]);
               return View("AgregarCuotas");
           }
           catch (SqlException ex)
           {
               ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;
               logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, ex.GetType().ToString());
               ViewData["familiaSeleccionarTodo"] = null;
               return View("AgregarCuotas", (IView)null);
           }
           catch (EntityException ex)
           {
               ViewData["message"] = TransactionMessages.ENTITY_EXCEPTION_MESSAGE;
               logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, ex.GetType().ToString());
               ViewData["familiaSeleccionarTodo"] = null;
               return View("AgregarCuotas", (IView)null);
           }
           catch (InvalidOperationException ex)
           {
               ViewData["message"] = TransactionMessages.SINGLE_NOT_FOUND_MESSAGE;
               logito.ElLogeador(TransactionMessages.SINGLE_NOT_FOUND_MESSAGE, ex.GetType().ToString());
               ViewData["familiaSeleccionarTodo"] = null;
               return View("AgregarCuotas", (IView)null);
           }
       }
               
    }

}

