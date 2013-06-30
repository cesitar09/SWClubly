﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Util
{
    public abstract class TransactionMessages
    {
        public const string SQL_EXCEPTION_MESSAGE = "Hubo un error al acceder en la base de datos.Seleccione la opción para Limpiar la página y vulva a intentar luego.";
        public const string ENTITY_EXCEPTION_MESSAGE = "No se pudo conectar con la base de datos.Seleccione la opción para Limpiar la página y vulva a intentar luego.";
        public const string SINGLE_NOT_FOUND_MESSAGE = "El dato elegido no existe en la base de datos.Seleccione la opción para Limpiar la página y vulva a intentar luego.";
        public const string NOT_ADD_DATA = "Existe una reserva entre el rango de fechas ingresadas. Por favor ingrese otro rango de fecha o revise la disponibilidad.";
        public const string OK_ADD_DATA_MESSAGE = "Se ingresaron los datos correctamente.";
        public const string OK_CHANGE_DATA_MESSAGE = "Se modificaron los datos correctamente.";
        
    }
}