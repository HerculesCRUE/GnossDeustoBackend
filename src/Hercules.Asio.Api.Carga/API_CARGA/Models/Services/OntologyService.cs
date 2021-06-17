// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Contiene los métodos necesarios para poder cargar una ontologia
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net;

namespace API_CARGA.Models.Services
{

    /// <summary>
    /// Contiene los métodos necesarios para poder cargar una ontologia
    /// </summary>
    public static class OntologyService
    {
        readonly private static string ONTOLOGY_FOLDER = "Config/Ontology/";
        readonly private static string ONTOLOGY_FILE = "Config/Ontology/roh-v2.owl";

        /// <summary>
        /// Lee el fichero de la ontologia
        /// </summary>
        /// <returns>Devuelve el contenido de la ontologia</returns>
        public static string GetOntology()
        {
            return File.ReadAllText(ONTOLOGY_FILE);
        }

        /// <summary>
        /// Reemplaza la ontologia
        /// </summary>
        /// <param name="newOntology">Nueva ontologia</param>
        /// <returns></returns>
        public static void SetOntology(IFormFile newOntology)
        {
            FileStream stream = null;
            Directory.CreateDirectory(ONTOLOGY_FOLDER);
            stream = File.Create(ONTOLOGY_FILE);
            newOntology.CopyTo(stream);
            stream.Close();
        }
    }
}
