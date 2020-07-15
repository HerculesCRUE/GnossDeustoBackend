// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Contiene los métodos necesarios para poder cargar una ontologia
using API_CARGA.Models.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    
    /// <summary>
    /// Contiene los métodos necesarios para poder cargar una ontologia
    /// </summary>
    public class OntologyService
    {

        public static string ONTOLOGYROH_PATH = "Config/Ontology/roh-v2.owl";
        public static string ONTOLOGYROHES_PATH = "Config/Ontology/rohes.owl";
        public static string ONTOLOGYROHUM_PATH = "Config/Ontology/rohum.owl";

        /// <summary>
        /// Comprueba que un fichero exista remotamente
        /// </summary>
        /// <param name="url">Ulr del fichero</param>
        /// <returns></returns>
        public static bool RemoteFileExists(string url)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

                request.Timeout = 20;

                //Configurando el Request method HEAD, puede ser GET tambien.
                request.Method = "HEAD";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Lee el fichero de la ontologia
        /// </summary>
        /// <param name="url">Ulr del fichero</param>
        /// <returns>Devuelve el contenido de la ontologia</returns>
        public static string GetOntology(OntologyEnum ontologyType)
        {
            if (ontologyType.Equals(OntologyEnum.OntologyRoh))
            {
                return File.ReadAllText(ONTOLOGYROH_PATH);
            }
            else if (ontologyType.Equals(OntologyEnum.OntologyRohes))
            {
                return File.ReadAllText(ONTOLOGYROHES_PATH);
            }
            else
            {
                return File.ReadAllText(ONTOLOGYROHUM_PATH);
            }
            
        }

        /// <summary>
        /// Remplaza la ontologia
        /// </summary>
        /// <param name="newOntology">Nueva ontologia</param>
        /// <returns></returns>
        public static void SetOntology(IFormFile newOntology, OntologyEnum ontologyType)
        {
            FileStream stream = null;
            if (ontologyType.Equals(OntologyEnum.OntologyRoh))
            {
                stream = File.Create(ONTOLOGYROH_PATH);
            }
            else if (ontologyType.Equals(OntologyEnum.OntologyRohes))
            {
                stream = File.Create(ONTOLOGYROHES_PATH);
            }
            else
            {
                stream = File.Create(ONTOLOGYROHUM_PATH);
            }
            newOntology.CopyTo(stream);
            stream.Close();
        }
    }
}
