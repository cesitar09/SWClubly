using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Newtonsoft.Json;
using Kendo.Mvc.UI;
using System.Data.SqlClient;
using Web.Util;
using System.Data;
using System.Net;
using Negocio.Util;


namespace Web.Controllers
{
    public class GestionarReservaController : Controller
    {

 //************************************RESERVA CANCHA*********************************************************************************//
       
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]

        public ActionResult GestionarReservarCancha(Models.ReservaCancha reserva)
        {
         
                return View((IView)null);
          
        }

        public bool BuscarDisponibilidad(Models.ReservaCancha reserva)
        {
            IEnumerable<Models.ReservaCancha> ListaReserva = Models.ReservaCancha.SeleccionarTodo();
            bool disponibilidad = true;

            if (ListaReserva != null) //Para que la Lista no este Vacio
            {
                foreach (var resev in ListaReserva)
                {
                    if (resev.cancha != null && reserva.cancha != null)
                    {
                        if (resev.cancha.id == reserva.cancha.id)
                        {
                            if (resev.horaInicio.Date == reserva.horaInicio.Date)
                            {
                                if ((resev.horaInicio>reserva.horaInicio && resev.horaInicio < reserva.horaFin) ||
                                    (resev.horaInicio==reserva.horaInicio && resev.horaInicio==reserva.horaFin) ||
                                    (resev.horaInicio<reserva.horaInicio && resev.horaFin>reserva.horaInicio ))
                                {
                                    disponibilidad = false;
                                }

                            }

                        }   
                    }       
                }        
                           
            }                  
            return disponibilidad;
        }

        public bool ValidarHoras(Models.ReservaCancha reserva)
        {
            DateTime fechaError = new DateTime();

            if (reserva.horaInicio == fechaError || reserva.horaFin == fechaError)
            {
                ViewData["message"] = "Error las horas no son validas por favor ingresar correctamente";
            }
            else
            {

                if (reserva.horaFin.Hour > reserva.horaInicio.Hour)
                {
                    if (reserva.horaFin.Hour - reserva.horaInicio.Hour <= Models.Parametros.SeleccionarParametros().tiempo) return true;
                    else ViewData["message"] = "El tiempo maximo permitido ha sido excedido";
                }
                else
                    if (reserva.horaFin.Hour == reserva.horaInicio.Hour)
                    {
                        if (reserva.horaFin.Minute >= reserva.horaInicio.Minute)
                            if (reserva.horaFin.Minute - reserva.horaInicio.Minute >= 30) return true;
                            else ViewData["message"] = "El tiempo minimo para reservar es de media hora. Por favor vuelva a seleccionar una hora";
                    }

            }
            return false;
        }

        public bool VecesMaxima(Models.ReservaCancha reserva)
        {
            int contador = 0;
            IEnumerable<Models.ReservaCancha> ListaReserva = Models.ReservaCancha.SeleccionarTodo();
            bool aceptable = false;
            foreach (var resev in ListaReserva)
            {
                if (resev.fechaInicio.Date == reserva.fechaInicio.Date && resev.idFamilia == reserva.idFamilia)
                {
                    contador = contador + 1;
                }
            }

            if (contador <= 2)
                aceptable = true;

            return aceptable;
        }

        public ActionResult IngresarReservaCancha(Models.ReservaCancha reserva)
        {
            bool disponibilidad = false;

            try
            {
                if (ValidarHoras(reserva))
                {
                    if (VecesMaxima(reserva))
                    {
                        //Asigno horas para el model0
                        DateTime fechaInicial = new DateTime();
                        DateTime fechaFinal = new DateTime();

                        fechaInicial = reserva.fechaInicio.Add(reserva.horaInicio.TimeOfDay);
                        fechaFinal = reserva.fechaInicio.Add(reserva.horaFin.TimeOfDay);
                        reserva.horaInicio = fechaInicial;
                        reserva.horaFin = fechaFinal;

                        reserva.actividad = null;

                        //Si selecciona Cancha
                        if (reserva.idCancha != 0)
                        {  
                            reserva.cancha = Models.Cancha.buscarId(reserva.idCancha);
                            if (BuscarDisponibilidad(reserva))
                            {   
                                reserva.familia = Models.Familia.buscarId(reserva.idFamilia);
                                Models.ReservaCancha.Insertar(reserva);
                                ViewData["message"] = TransactionMessages.OK_ADD_DATA_MESSAGE;
                                string nombre = (string)Session["Nombre"];
                                Negocio.Util.logito.ElLogeador("Insertó Reserva Cancha", nombre);
                                return View("GestionarReservarCancha", reserva);

                            }
                            else
                            {
                                ViewData["message"] = "Cancha no disponible, Intente en otro horario";
                                return View("GestionarReservarCancha", reserva);
                            }
                        }
                        else //Sino selecciona Cancha
                        {   
                            IEnumerable<Models.Cancha> ListaCancha = Models.Cancha.BuscarSedeTipo(reserva.idSede, reserva.idTipoCancha);

                            foreach (var cancha in ListaCancha)
                            {
                                reserva.cancha = cancha;
                                if (BuscarDisponibilidad(reserva))
                                {
                                    disponibilidad = true;
                                    break;
                                }
                            }

                            if (disponibilidad)
                            {
                                if (reserva.id == 0)
                                    reserva.familia = Models.Familia.buscarId(10001);

                                reserva.idCancha = reserva.cancha.id;
                                Models.ReservaCancha.Insertar(reserva);
                                ViewData["message"] = TransactionMessages.OK_ADD_DATA_MESSAGE;
                                string nombre = (string)Session["Nombre"];
                                Negocio.Util.logito.ElLogeador("Insertó Reserva Cancha", nombre);
                                return View("GestionarReservarCancha", reserva);

                            }
                            else
                            {
                                ViewData["message"] = "No se encuentra ninguna Cancha Disponible, lo sentimos. Intente de nuevo";
                                return View("GestionarReservarCancha", reserva);
                            }

                        }
                    }
                    else ViewData["message"] = "La cantidad maxima de reservas al dia ha sido excedida";
                }

                return View("GestionarReservarCancha", reserva);
            }
            catch (SqlException )
            {
                ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;
                string nombre = (string)Session["Nombre"];
                logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, nombre);
                return View("GestionarReservarCancha",null);
            }
            catch (EntityException )
            {
                ViewData["message"] = TransactionMessages.ENTITY_EXCEPTION_MESSAGE;
                string nombre = (string)Session["Nombre"];
                logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, nombre);
                return View("GestionarReservarCancha", null);
            }
            catch (InvalidOperationException )
            {
                ViewData["message"] = TransactionMessages.SINGLE_NOT_FOUND_MESSAGE;
                string nombre = (string)Session["Nombre"];
                logito.ElLogeador(TransactionMessages.SINGLE_NOT_FOUND_MESSAGE, nombre);
                return View("GestionarReservarCancha", null);
            }
        }

        public ActionResult LeerReservasCanchas([DataSourceRequest] DataSourceRequest request)
        {
            try
            {
                IEnumerable<Models.ReservaCancha> listaReservas = Models.ReservaCancha.SeleccionarTodoTabla();
                return Json(listaReservas.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("");
            }

        }

        public ActionResult EditarReservaCancha(Web.Models.ReservaCancha reserva)
        {
            try
            {
                Models.ReservaCancha rese = Models.ReservaCancha.BuscarId(reserva.id);
                if (rese.estado == "Cancelada")
                    ViewData["message"] = "Error la reserva no se puede editar ya que se encuentra Cancelada";
                else if (rese.estado == "Realizado")
                    ViewData["message"] = "La reserva no se puede editar ya que se encuentra Vencida";
                else
                {
                    return View("GestionarReservarCancha", rese);
                }
                return View("GestionarReservarCancha");
            }
            catch (SqlException)
            {
                ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;
                string nombre = (string)Session["Nombre"];
                logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, nombre);
                return View("GestionarReservarCancha", null);
            }
            catch (EntityException)
            {
                ViewData["message"] = TransactionMessages.ENTITY_EXCEPTION_MESSAGE;
                string nombre = (string)Session["Nombre"];
                logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, nombre);
                return View("GestionarReservarCancha", null);
            }
            catch (InvalidOperationException)
            {
                ViewData["message"] = TransactionMessages.SINGLE_NOT_FOUND_MESSAGE;
                string nombre = (string)Session["Nombre"];
                logito.ElLogeador(TransactionMessages.SINGLE_NOT_FOUND_MESSAGE, nombre);
                return View("GestionarReservarCancha", null);
            }
        }

        public ActionResult CancelarReservaCancha(short id)
        {
            try
            {
                Models.ReservaCancha reserva = Models.ReservaCancha.BuscarId(id);

                if (reserva.estado == "Cancelada")
                    ViewData["message"] = "Error la reserva no se puede cancelar ya que se encuentra Cancelada";
                else if (reserva.estado == "Vencida")
                    ViewData["message"] = "Error la reserva no se puede cancelar ya que se encuentra Vencida";
                else
                {
                    Models.ReservaCancha.Eliminar(id);
                    string nombre = (string)Session["Nombre"];
                    Negocio.Util.logito.ElLogeador("Insertó Reserva Cancha", nombre);
                }
                return View("GestionarReservarCancha");
            }
            catch (SqlException )
            {
                ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;
                string nombre = (string)Session["Nombre"];
                logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, nombre);
                return View("GestionarReservarCancha", null);
            }
            catch (EntityException )
            {
                ViewData["message"] = TransactionMessages.ENTITY_EXCEPTION_MESSAGE;
                string nombre = (string)Session["Nombre"];
                logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, nombre);
                return View("GestionarReservarCancha", null);
            }
            catch (InvalidOperationException )
            {
                ViewData["message"] = TransactionMessages.SINGLE_NOT_FOUND_MESSAGE;
                string nombre = (string)Session["Nombre"];
                logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, nombre);
                return View("GestionarReservarCancha", null);
            }

        }

        public ActionResult LeerSedes()
        {
            try
            {
                IEnumerable<Models.Sede> ListaSedes = Models.Sede.SeleccionarTodo();

                return Json(ListaSedes, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("");
            }
        }

        public ActionResult LeerTipoCancha(string idSede)
        {
            try
            {
                short idsede = Convert.ToInt16(idSede);
                IEnumerable<Models.TipoCancha> ListaSedes = Models.TipoCancha.BuscarPorSede(idsede);
                return Json(ListaSedes, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("");
            }
        }

        public ActionResult LeerNumeroCancha(string idTipo, string idSede)
        {
            try
            {
                short idtipo = Convert.ToInt16(idTipo);
                short idsede = Convert.ToInt16(idSede);
                IEnumerable<Models.Cancha> ListaSedes = Models.Cancha.BuscarSedeTipo(idsede, idtipo);// buscarTipo(idtipo);
                return Json(ListaSedes, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("");
            }
        }
       
        public ActionResult LeerIdFamilia()
        {
            IEnumerable<short> ListaId = Models.Familia.SeleccionarTodo().Select(p => p.id);
            return Json(ListaId, JsonRequestBehavior.AllowGet);        
        }

        public ActionResult BuscarId(String id)
        {
            try
            {
                bool valido = false;
                IEnumerable<Models.Familia> ListaFamilia = Models.Familia.SeleccionarTodo();
                foreach (Models.Familia familia in ListaFamilia)
                {

                    string palabra = "\"" + Convert.ToString(familia.id) + "\"";
                    if (palabra.Equals(id))
                    {
                        valido = true;
                        break;
                    }
                }
                if (valido)
                {
                    short sid = short.Parse(id.Trim(new Char[] { '\\', '\"' }));
                    IEnumerable<Models.Socio> socio = Models.Socio.SeleccionarTodo().Where(p => p.familia.id == sid);
                    Models.Persona persona = socio.Single(p => p.estado).persona;
                    return Json(persona);
                }
                else
                {
                    ViewData["message"] = "Error el codigo de usuario es incorrecto, por favor vuelvalo a intentar";
                    return Json(null);
                }
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("");
            }

        }



//************************************RESERVA INGRESO DE BUNGALOW*********************************************************************************//
       
        public ActionResult RegistrarIngresoBungalow(short? id)
        {
            if (id.HasValue)
                Models.ReservaBungalow.RegistrarIngresoBungalow(id);
            return View();
        }

        public ActionResult RegistrarSalidaBungalow(short? id)
        {
            if (id.HasValue)
                Models.ReservaBungalow.RegistrarSalidaBungalow(id);
            return View("RegistrarIngresoBungalow");
        }

        public ActionResult IngresarBungalow(short id)
        {
            
            return View("RegistrarIngresoBungalow()");
        }


    //JSONS
       // public ActionResult LeerReservasBungalow([DataSourceRequest] DataSourceRequest request)
       // {
       //     IEnumerable<Models.ReservaCancha> listaReservas = Models.ReservaCancha.SeleccionarTodo();
       //     return Json(listaReservas.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
       // }
        public ActionResult LeerReservasBungalows([DataSourceRequest] DataSourceRequest request)
        {
            IEnumerable<Models.ReservaBungalow> listaReservas = Models.ReservaBungalow.SeleccionarIngreso();
            return Json(listaReservas.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}
