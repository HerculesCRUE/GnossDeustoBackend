﻿// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase que sirve para mostrar los datos de una tarea

using System;
using System.Diagnostics.CodeAnalysis;

namespace CronConfigure.ViewModels
{
    ///<summary>
    ///Clase que sirve para mostrar los datos de una tarea
    ///</summary>
    [ExcludeFromCodeCoverage]
    public class JobViewModel
    {
        /// <summary>
        /// Método Ejecutado
        /// </summary>
        public string Job { get; set; }
        /// <summary>
        /// Estado de la ejecución
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// Identificador de le tarea
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Excepción producida en caso de error
        /// </summary>
        public string ExceptionDetails { get; set; }
        /// <summary>
        /// Fecha de ejecución
        /// </summary>
        public DateTime? ExecutedAt { get; set; }
        /// <summary>
        /// Número de elementos procesados
        /// </summary>
        public int ProcessNumIdentifierOAIPMH { get; set; }
        /// <summary>
        /// Número de elementos a procesar
        /// </summary>
        public int TotalNumIdentifierOAIPMH { get; set; }
        /// <summary>
        /// Último elemento procesado
        /// </summary>
        public string LastIdentifierOAIPMH { get; set; }
    }
}
