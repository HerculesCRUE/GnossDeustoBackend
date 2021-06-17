// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using VDS.RDF.Query;

namespace API_DISCOVER.Models.Entities
{
    /// <summary>
    /// SparqlObject
    /// </summary>
    public class SparqlObject
    {
        /// <summary>
        /// SparqlObject
        /// </summary>
        public SparqlObject() { }
        /// <summary>
        /// head
        /// </summary>
        public Head head { get; set; }
        /// <summary>
        /// results
        /// </summary>
        public Results results { get; set; }
        /// <summary>
        /// boolean
        /// </summary>
        public bool boolean { get; set; }

        /// <summary>
        /// Data
        /// </summary>
        [DataContract]
        public class Data
        {
            /// <summary>
            /// Data
            /// </summary>
            public Data() { }
            /// <summary>
            /// datatype
            /// </summary>
            [DataMember(Name = "datatype")]            
            public string datatype { get; set; }
            /// <summary>
            /// type
            /// </summary>
            [DataMember(Name = "type")]
            public string type { get; set; }
            /// <summary>
            /// value
            /// </summary>
            [DataMember(Name = "value")]
            public string value { get; set; }
            /// <summary>
            /// lang
            /// </summary>
            [DataMember(Name = "xml:lang")]
            public string lang { get; set; }
        }
        /// <summary>
        /// Head
        /// </summary>
        public class Head
        {
            /// <summary>
            /// Head
            /// </summary>
            public Head() { }
            /// <summary>
            /// link
            /// </summary>
            public HashSet<object> link { get; set; }            
            /// <summary>
            /// vars
            /// </summary>
            public HashSet<string> vars { get; set; }            
        }
        /// <summary>
        /// Results
        /// </summary>
        public class Results
        {
            /// <summary>
            /// Results
            /// </summary>
            public Results() { }
            /// <summary>
            /// bindings
            /// </summary>
            public List<Dictionary<string, Data>> bindings { get; set; }
            /// <summary>
            /// distinct
            /// </summary>
            public bool distinct { get; set; }
            /// <summary>
            /// ordered
            /// </summary>
            public bool ordered { get; set; }
        }
    }
}
