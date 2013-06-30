using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Newtonsoft.Json;
using Kendo.Mvc.UI;
using System.Net;
using System.Data.SqlClient;
using Web.Util;
using System.Data;
using Negocio.Util;
namespace Web.Controllers
{
    public class ReservasController : Controller
    {
        //********************************RESERVAR CANCHA*******************************************************************************************

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ReservarCancha(Models.ReservaCancha reserva)
        {

                ViewData["message"] = null;
                return View((IView)null);
        }

        public bool BuscarDisponibilidad(Models.ReservaCancha reserva) {
            IEnumerable<Models.ReservaCancha> ListaReserva = Models.ReservaCancha.SeleccionarTodo();
            bool disponibilidad=true;

            if (ListaReserva != null) //Para que la Lista no este Vacio
            {
                foreach (var resev in ListaReserva)
                {
                    if (resev.cancha != null && reserva.cancha!=null)
                    {
                        if (resev.cancha.id == reserva.cancha.id)
                        {
                            if (resev.horaInicio.Date == reserva.horaInicio.Date)
                            {
                                if ((resev.horaInicio > reserva.horaInicio && resev.horaInicio < reserva.horaFin) ||
                                    (resev.horaInicio == reserva.horaInicio && resev.horaInicio == reserva.horaFin) ||
                                    (resev.horaInicio < reserva.horaInicio && resev.horaFin > reserva.horaInicio))
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

        public bool ValidarHoras(Models.ReservaCancha reserva) {

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
                            else ViewData["message"] = "El tiempo minimo para reservar es de media hora.";
                    }

            }
            return false;
        }

        public bool VecesMaxima(Models.ReservaCancha reserva) {
            int contador = 0;
            IEnumerable<Models.ReservaCancha> ListaReserva = Models.ReservaCancha.SeleccionarTodo();
            bool aceptable = false;
            foreach (var resev in ListaReserva)
            {
                if (resev.fechaInicio.Date == reserva.fechaInicio.Date && resev.idFamilia == reserva.idFamilia) {
                    contador = contador + 1;
                }
            }

            short max=Models.Parametros.SeleccionarParametros().maxReservas;
            if (contador+1 <= Models.Parametros.SeleccionarParametros().maxReservas)
                aceptable = true;

            return aceptable;
        }

        public ActionResult IngresarReservaCancha(Models.ReservaCancha reserva)
        {
            bool disponibilidad=false;
            short idusuario;

            try
            {

                if (ValidarHoras(reserva))
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
                                reserva.idFamilia = Models.Familia.buscarIdUsuario(Convert.ToInt16(Session["idUsuario"])).id;
                                reserva.familia = Models.Familia.buscarId(reserva.idFamilia);
                                if (VecesMaxima(reserva))
                                {
                                    Models.ReservaCancha.Insertar(reserva);
                                    ViewData["message"] = TransactionMessages.OK_ADD_DATA_MESSAGE;
                                    string nombre = (string)Session["Nombre"];
                                    Negocio.Util.logito.ElLogeador("Insertó Reserva Cancha", nombre);
                                }
                                else { 
                                   ViewData["message"] = "La cantidad maxima de reservas al dia ha sido excedida";
                                }
                                return View("ReservarCancha", reserva);
                            }
                            else
                            {
                                ViewData["message"] = "Cancha no disponible, Intente en otro horario";
                                return View("ReservarCancha", reserva);
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
                                reserva.idFamilia = Models.Familia.buscarIdUsuario(Convert.ToInt16(Session["idUsuario"])).id;
                                reserva.familia = Models.Familia.buscarId(reserva.idFamilia);
                            
                                reserva.idCancha = reserva.cancha.id;
                                if (VecesMaxima(reserva))
                                {
                                    Models.ReservaCancha.Insertar(reserva);
                                    ViewData["message"] = TransactionMessages.OK_ADD_DATA_MESSAGE;
                                    string nombre = (string)Session["Nombre"];
                                    Negocio.Util.logito.ElLogeador("Insertó Reserva Cancha", nombre);
                                }
                                else {
                                    ViewData["message"] = "La cantidad maxima de reservas al dia ha sido excedida";
                                }
                                return View("ReservarCancha", reserva);

                            }
                            else
                            {
                                ViewData["message"] = "No se encuentra ninguna Cancha Disponible, lo sentimos. Intente de nuevo";
                                return View("ReservarCancha", reserva);
                            }

                        }

                    }
                    
                return View("ReservarCancha", reserva);
            }
            catch (SqlException)
            {
                ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;
                string nombre = (string)Session["Nombre"];
                logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, nombre);
                return View("ReservarCancha", null);
            }
            catch (EntityException)
            {
                ViewData["message"] = TransactionMessages.ENTITY_EXCEPTION_MESSAGE;
                string nombre = (string)Session["Nombre"];
                logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, nombre);
                return View("ReservarCancha", null);
            }
            catch (InvalidOperationException)
            {
                ViewData["message"] = TransactionMessages.SINGLE_NOT_FOUND_MESSAGE;
                string nombre = (string)Session["Nombre"];
                logito.ElLogeador(TransactionMessages.SINGLE_NOT_FOUND_MESSAGE, nombre);
                return View("ReservarCancha", null);
            }
        }

        public ActionResult LeerReservasCanchas([DataSourceRequest] DataSourceRequest request)
        {
            try
            {
                short idsocio = Models.Familia.buscarIdUsuario(Convert.ToInt16(Session["idUsuario"])).id;
                IEnumerable<Models.ReservaCancha> listaReservas = Models.ReservaCancha.BuscarIdFamiliaTabla(idsocio);
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
                    return View("ReservarCancha", rese);
                }
                return View("ReservarCancha");
            }
            catch (SqlException)
            {
                ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;
                string nombre = (string)Session["Nombre"];
                logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, nombre);
                return View("ReservarCancha", null);
            }
            catch (EntityException)
            {
                ViewData["message"] = TransactionMessages.ENTITY_EXCEPTION_MESSAGE;
                string nombre = (string)Session["Nombre"];
                logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, nombre);
                return View("ReservarCancha", null);
            }
            catch (InvalidOperationException)
            {
                ViewData["message"] = TransactionMessages.SINGLE_NOT_FOUND_MESSAGE;
                string nombre = (string)Session["Nombre"];
                logito.ElLogeador(TransactionMessages.SINGLE_NOT_FOUND_MESSAGE, nombre);
                return View("ReservarCancha", null);
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
                    Negocio.Util.logito.ElLogeador("Eliminar Reserva Cancha", nombre);
                }

                return View("ReservarCancha");
            }
            catch (SqlException)
            {
                ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;
                string nombre = (string)Session["Nombre"];
                logito.ElLogeador(TransactionMessages.SQL_EXCEPTION_MESSAGE, nombre);
                return View("ReservarCancha", null);
            }
            catch (EntityException)
            {
                ViewData["message"] = TransactionMessages.ENTITY_EXCEPTION_MESSAGE;
                string nombre = (string)Session["Nombre"];
                logito.ElLogeador(TransactionMessages.ENTITY_EXCEPTION_MESSAGE, nombre);
                return View("ReservarCancha", null);
            }
            catch (InvalidOperationException)
            {
                ViewData["message"] = TransactionMessages.SINGLE_NOT_FOUND_MESSAGE;
                string nombre = (string)Session["Nombre"];
                logito.ElLogeador(TransactionMessages.SINGLE_NOT_FOUND_MESSAGE, nombre);
                return View("ReservarCancha", null);
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

        
        //********************************RESERVAR BUNGALOW *******************************************************************************************

        public ActionResult ObtenerUrlImagen(String id)
        {
            return Json("../Content/img/" + id.Trim(new Char[] { '\\', '\"' }) + ".jpg", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerBungalow(String sede)
        {
            short id = short.Parse(sede);
            IEnumerable<Models.Bungalow> bungalows = Models.Bungalow.BuscarSede(id);
            return Json(bungalows, JsonRequestBehavior.AllowGet);
        }

        public JsonResult VerDisponibilidad(String sidBungalow, String sfinicial, String sffinal)
        {
            try
            {
                if (sidBungalow.Trim(new Char[] { '\\', '\"' }) != "" && sfinicial.Trim(new Char[] { '\\', '\"' }) != "" && sffinal.Trim(new Char[] { '\\', '\"' }) != "")
                {
                    short idBungalow = short.Parse(sidBungalow.Trim(new Char[] { '\\', '\"' }));
                    DateTime finicial = DateTime.Parse(sfinicial.Trim(new Char[] { '\\', '\"' }));
                    DateTime ffinal = DateTime.Parse(sffinal.Trim(new Char[] { '\\', '\"' }));
                    IList<DateTime> tiempos = Negocio.ReservaBungalow.Disponibilidad(idBungalow, finicial, ffinal);
                    if (tiempos != null)
                    {
                        string cadena="El bungalow no esta disponible. Puede reservar en las siguientes fechas:";
                        int tam=tiempos.Count;
                        if (tam != 0)
                        {
                            for (int i = 0; i < tam-2; i+=2)
                                cadena = cadena+" del "+tiempos[i].Day+"/"+tiempos[i].Month+"/"+tiempos[i].Year+" al "+
                                    tiempos[i + 1].Day + "/" + tiempos[i + 1].Month + "/" + tiempos[i + 1].Year+",";
                            cadena = cadena + " del " + tiempos[tam - 2].Day + "/" + tiempos[tam - 2].Month + "/" + tiempos[tam - 2].Year +
                                " al " + tiempos[tam - 1].Day + "/" + tiempos[tam - 1].Month + "/" + tiempos[tam - 1].Year+".";
                        }
                        else
                            cadena = "El bungalow esta reservado en el rango de fechas elegido.";
                        return Json(cadena, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json("El bungalow esta libre y puede ser Reservado", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(TransactionMessages.ENTITY_EXCEPTION_MESSAGE);
                }
            }
            catch
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Ha ocurrido un error");
            }
        }

        public JsonResult ObtenerSedes()
        {
            var sedes = Web.Models.Sede.SeleccionarTodo();
            return Json(sedes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LeerFamilias()
        {
            var familia = Web.Models.Familia.SeleccionarTodo();
            return Json(familia, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ReservarBungalow()
        {
            return View();
        }

        public ActionResult ReservarBungalowGlobal()
        {
            return View();
        }

        public ActionResult LeerReservasFamilia([DataSourceRequest]DataSourceRequest request)
        {
            short idUsuario = (short)Session["IdUsuario"];
            IEnumerable<Models.ReservaBungalow> reservas = Models.ReservaBungalow.SeleccionarReservasFamilia(Models.Familia.buscarIdUsuario(idUsuario).id);
            return Json(reservas.ToDataSourceResult(request));
        }

        public ActionResult LeerReservasGlobal([DataSourceRequest]DataSourceRequest request)
        {
            IEnumerable<Models.ReservaBungalow> reservas = Models.ReservaBungalow.SeleccionarReservas();
            return Json(reservas.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult ReservarBungalowGlobal(Models.ReservaBungalow reserva, DateTime finicial, DateTime ffinal, short AutocompleteId)
        {
            try
            {
                Datos.TemporadaAlta temporada = Negocio.ReservaBungalow.FechaDeSorteo(finicial, ffinal);
                if (temporada == null)  //cambiar por ==
                {
                    reserva.fechaInicio = finicial;
                    reserva.fechaFin = ffinal;
                    if (Models.ReservaBungalow.AgregarReservaBungalowF(reserva, AutocompleteId) == 0)
                        ViewData["message"] = TransactionMessages.NOT_ADD_DATA;
                    else
                        ViewData["message"] = TransactionMessages.OK_ADD_DATA_MESSAGE;
                    return View();
                }
                else
                {
                    ViewData["message2"] = "Usted ha querido agregar una reserva en temporada de sorteo (del " + temporada.fechaInicio.Day + "/" +
                        temporada.fechaInicio.Month + "/" + temporada.fechaInicio.Year + " al " + temporada.fechaFin.Day + "/" + temporada.fechaFin.Month +
                        "/" + temporada.fechaFin.Year + "). Desea participar del sorteo?";
                    ViewData["idTemporada"] = temporada.id;
                    ViewData["idBungalow"] = reserva.bungalow.id;
                    return View();
                }
            }
            catch
            {
                ViewData["errorMessage"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;    //no se pudo conectar a la bd
                return View();
            }
        }

        [HttpPost]
        public ActionResult ReservarBungalow(Models.ReservaBungalow reserva,DateTime finicial, DateTime ffinal)
        {
            try
            {
                Datos.TemporadaAlta temporada = Negocio.ReservaBungalow.FechaDeSorteo(finicial, ffinal);
                if (temporada == null)  //cambiar por ==
                {
                    short idUsuario = (short)Session["IdUsuario"];
                    reserva.fechaInicio = finicial;
                    reserva.fechaFin = ffinal;
                    if(Models.ReservaBungalow.AgregarReservaBungalow(reserva, idUsuario)==0)
                        ViewData["message"] = TransactionMessages.NOT_ADD_DATA;
                    else
                        ViewData["message"] = TransactionMessages.OK_ADD_DATA_MESSAGE;
                    return View();
                }
                else
                {
                    ViewData["message2"] = "Usted ha querido agregar una reserva en temporada de sorteo (del " + temporada.fechaInicio.Day + "/" +
                        temporada.fechaInicio.Month + "/" + temporada.fechaInicio.Year + " al " + temporada.fechaFin.Day + "/" + temporada.fechaFin.Month +
                        "/" + temporada.fechaFin.Year + "). Desea participar del sorteo?";
                    ViewData["idTemporada"] = temporada.id;
                    ViewData["idBungalow"] = reserva.bungalow.id;
                    return View();
                }
            }
            catch 
            {
                ViewData["errorMessage"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;    //no se pudo conectar a la bd
                return View();
            }
        }

        [HttpGet]
        public ActionResult ReservarBungalowSorteo(short idTemporada,short idBungalow)
        {
            try
            {
                short idFamilia=Negocio.Familia.buscarIdUsuario((short)Session["IdUsuario"]).id;
                Negocio.ReservaBungalowSorteo.Insertar(idTemporada, idBungalow, idFamilia);
                ViewData["message"] = TransactionMessages.OK_ADD_DATA_MESSAGE;
                return View("ReservarBungalow");

            }
            catch
            {
                ViewData["message"] = TransactionMessages.SQL_EXCEPTION_MESSAGE;    //no se pudo conectar a la bd
                return View("ReservarBungalow");
            }
        }

    }
}

