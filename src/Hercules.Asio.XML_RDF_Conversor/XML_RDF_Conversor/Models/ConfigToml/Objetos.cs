namespace Conversor_XML_RDF.Models.ConfigToml
{
    /// <summary>
    /// Clase encargada de almacenar el listado de entidades.
    /// </summary>
    public class ConversorConfig
    {
        /// <summary>
        /// Lista de entidades.
        /// </summary>
        public Entity[] entities { get; set; }
    }

    /// <summary>
    /// Clase encarga de almacenar la infomración de la entidad.
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// Tipo del rdf al que apunta.
        /// </summary>
        public string rdftype { get; set; }
        /// <summary>
        /// Tipo del rdf del cual habrá que obtener del mapa.
        /// </summary>
        public string rdftypeproperty { get; set; }
        /// <summary>
        /// Mapa con los datos a cambiar.
        /// </summary>
        public Mapping[] mappingrdftype { get; set; }
        /// <summary>
        /// ID de la entidad.
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// Espacio de nombre de la entidad.
        /// </summary>
        public string nameSpace { get; set; }
        /// <summary>
        /// Namespace + Nombre de la etiqueta.
        /// </summary>
        public string source { get; set; }
        /// <summary>
        /// Propiedad la cual hay que acceder y no se encuentra en el nodo.
        /// </summary>
        public string property { get; set; }
        /// <summary>
        /// Tipo de dato de la propiedad que hay que acceder.
        /// </summary>
        public string datatype { get; set; }
        /// <summary>
        /// Listado de propiedades que puede tener la entidad.
        /// </summary>
        public Property[] properties { get; set; }
        /// <summary>
        /// Listado de subentidades que puede tener la entidad.
        /// </summary>
        public Subentity[] subentities { get; set; }
        
    }

    /// <summary>
    /// Clase encargada de almacenar la infomación de las propiedades.
    /// </summary>
    public class Property
    {
        /// <summary>
        /// IRI de la propiedad en cuestión.
        /// </summary>
        public string property { get; set; }
        /// <summary>
        /// Namespace + Nombre de la etiqueta.
        /// </summary>
        public string source { get; set; }
        /// <summary>
        /// Tipo de la propiedad.
        /// </summary>
        public string datatype { get; set; }
    }

    /// <summary>
    /// Clase encargada de almacenar las subentidades.
    /// </summary>
    public class Subentity
    {
        /// <summary>
        /// Propiedad directa. IRI del tipo de la entidad a la que apunta.
        /// </summary>
        public string property { get; set; }
        /// <summary>
        /// Propiedad inversa. IRI del tipo de la entidad a la que apunta.
        /// </summary>
        public string inverseProperty { get; set; }
        /// <summary>
        /// Lista de subentidades.
        /// </summary>
        public Entity[] entities { get; set; }
    }

    /// <summary>
    /// Clase encargada de mapear.
    /// </summary>
    public class Mapping
    {
        /// <summary>
        /// Espacio de nombre (xmlns) del nodo del XML.
        /// </summary>
        public string nameSpace { get; set; }
        /// <summary>
        /// Contenido de la etiqueta en la cual se le ha de aplicar el target.
        /// </summary>
        public string source { get; set; }
        /// <summary>
        /// IRI del tipo de la entidad a la que apunta.
        /// </summary>
        public string target { get; set; }
    }
}
