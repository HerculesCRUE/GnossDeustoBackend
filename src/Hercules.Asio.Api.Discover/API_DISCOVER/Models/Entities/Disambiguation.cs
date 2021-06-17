// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using System;
using System.Collections.Generic;

namespace API_DISCOVER.Models.Entities
{
    /// <summary>
    /// Configuración de desambiguación utilizada para apoyar en la realización de la desambiguación
    /// </summary>
    public class Disambiguation
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pRdfType">rdf:type al que afecta</param>
        /// <param name="pIdentifiers">Url de las propiedades que son identificadores de la entidad</param>
        /// <param name="pProperties">Configuración de propiedades utilizadas para apoyar en la realización de la desambiguación</param>
        public Disambiguation(string pRdfType, List<string> pIdentifiers, List<Property> pProperties)
        {
            rdfType = pRdfType;
            identifiers = pIdentifiers;
            properties = pProperties;
        }
        /// <summary>
        /// Propiedades utilizadas para apoyar en la realización de la desambiguación
        /// </summary>
        public class Property
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="pProperty">Url de la propiedad utilizada</param>
            /// <param name="pMandatory">Indica si es obligatorio que se cumpla esta restricción para considerar la desambiguación</param>
            /// <param name="pInverse">Indica si la propiedad es inverse, es decir, la entidad que se intenta desambiguar es el objeto de propiedad</param>
            /// <param name="pType">Tipo de desambiguación a aplicar</param>
            /// <param name="pMaxNumWordsTitle">Número de palabras a partir de la cual la similitud de tipo 'title' obtiene la máxima puntuación</param>
            /// <param name="pScorePositive">Puntuación positiva de la propiedad en la desambiguación (entre 0 y 1)</param>
            /// <param name="pScoreNegative">Puntuación negativa de la propiedad en la desambiguación (entre 0 y 1)</param>
            public Property(string pProperty, bool pMandatory, bool pInverse, Type pType, int? pMaxNumWordsTitle, float? pScorePositive, float? pScoreNegative)
            {
                property = pProperty;
                mandatory = pMandatory;
                inverse = pInverse;
                type = pType;
                maxNumWordsTitle = pMaxNumWordsTitle;
                if (pScorePositive.HasValue && (pScorePositive.Value < 0 || pScorePositive > 1))
                {
                    throw new ArgumentNullException("El valor de pScorePositive debe estar comprendido entre 0 y 1");
                }
                if (pScoreNegative.HasValue && (pScoreNegative.Value < 0 || pScoreNegative > 1))
                {
                    throw new ArgumentNullException("El valor de pScoreNegative debe estar comprendido entre 0 y 1");
                }
                if (pType == Type.title && !pMaxNumWordsTitle.HasValue)
                {
                    throw new ArgumentNullException("Si la propiedad es del tipo 'Type.title' tiene que tener asignado valor en la propiedad pMaxNumWordsTitle");
                }
                scorePositive = pScorePositive;
                scoreNegative = pScoreNegative;
            }

            /// <summary>
            /// Type
            /// </summary>
            public enum Type
            {
                /// <summary>
                /// Misma entidad o mismo valor de la propiedad
                /// </summary>
                equals,
                /// <summary>
                /// Mismo valor de la propiedad (ignorando mayúsculas y minúsculas)
                /// </summary>
                ignoreCaseSensitive,
                /// <summary>
                /// Mismo nombre (para nombres de personas)
                /// </summary>
                name,
                /// <summary>
                /// Mismo título (para títulos de documentos por ejemplo)
                /// </summary>
                title
            }
            /// <summary>
            /// Url de la propiedad utilizada
            /// </summary>
            public string property { get; set; }
            /// <summary>
            /// Indica si es obligatorio que se cumpla esta restricción para considerar la desambiguación
            /// </summary>
            public bool mandatory { get; set; }
            /// <summary>
            /// Indica si la propiedad es inverse, es decir, la entidad que se intenta desambiguar es el objeto de propiedad
            /// </summary>
            public bool inverse { get; set; }
            /// <summary>
            /// Tipo de desambiguación a aplicar
            /// </summary>
            public Type type { get; set; }
            /// <summary>
            /// Número de palabras a partir de la cual la similitud de tipo 'title' obtiene la máxima puntuación
            /// </summary>
            public int? maxNumWordsTitle { get; set; }
            /// <summary>
            /// Puntuación positiva de la propiedad en la desambiguación (entre 0 y 1)
            /// </summary>
            public float? scorePositive { get; set; }
            /// <summary>
            /// Puntuación negativa de la propiedad en la desambiguación (entre 0 y 1)
            /// </summary>
            public float? scoreNegative { get; set; }
        }
        /// <summary>
        /// rdf:type al que afecta
        /// </summary>
        public string rdfType { get; set; }
        /// <summary>
        /// Url de las propiedades que son identificadores de la entidad
        /// </summary>
        public List<string> identifiers { get; set; }
        /// <summary>
        /// Configuración de propiedades utilizadas para apoyar en la realización de la desambiguación
        /// </summary>
        public List<Property> properties { get; set; }
    }
}
