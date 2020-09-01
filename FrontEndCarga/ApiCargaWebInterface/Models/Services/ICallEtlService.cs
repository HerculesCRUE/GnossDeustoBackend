// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Interfaz para llamar a los métodos que ofrece el controlador etl del API_CARGA 
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    /// <summary>
    /// Interfaz para llamar a los métodos que ofrece el controlador etl del API_CARGA 
    /// </summary>
    public interface ICallEtlService
    {
        /// <summary>
        /// Llama al método del api de carga de getRecord
        /// </summary>
        /// <param name="repoIdentifier">Identificador del repositorio OAI-PMH </param>
        /// <param name="identifier">Identificador de la entidad a recolectar (Los identificadores se obtienen con el metodo /etl/ListIdentifiers/{repositoryIdentifier}).</param>
        /// <param name="type">metadata que se desea recuperar (rdf). Los formatos de metadatos admitidos por un repositorio y para un elemento en particular se pueden recuperar mediante la solicitud /etl/ListMetadataFormats/{repositoryIdentifier}.</param>
        /// <returns></returns>
        public string CallGetRecord(Guid repoIdentifier, string identifier, string type);
        /// <summary>
        /// Valida un rdf
        /// </summary>
        /// <param name="rdf">Rdf a validar</param>
        /// <param name="repositoryIdentifier">Repositorio en el que están configurados los shapes para validar</param>
        public void CallDataValidate(IFormFile rdf, Guid repositoryIdentifier);
        /// <summary>
        /// Llama al método del api de carga de publicación
        /// </summary>
        /// <param name="rdf">rdf a pasar</param>
        public void CallDataPublish(IFormFile rdf);
        /// <summary>
        /// Sube una ontologia
        /// </summary>
        /// <param name="ontology">Ontologia a subir</param>
        /// <param name="ontologyType">tipo de ontologia; siendo el 0 la ontología roh, el 1 la ontología rohes y el 2 la ontología rohum </param>
        public void PostOntology(IFormFile ontologyUri, int ontologyType);
        /// <summary>
        /// Obtiene una ontologia
        /// </summary>
        /// <param name="ontologyType">tipo de ontologia; siendo el 0 la ontología roh, el 1 la ontología rohes y el 2 la ontología rohum </param>
        /// <returns></returns>
        public string GetOntology(int ontologyType);
        /// <summary>
        /// Valida un rdf tanto con un rdf de validación personalizado como por una lista de shapes configuradas en el repositorio
        /// </summary>
        /// <param name="repositoryId">Identificador del repositorio OAIPMH</param>
        /// <param name="rdfToValidate">RDF a validar</param>
        /// <param name="validationRdf">RDF de validación</param>
        /// <param name="validationShapesList">Lista de shapes de validación</param>
        /// <param name="repositoryShapes">Lista de shapes configuradas en el repositorio</param>
        public void ValidateRDFPersonalized(Guid repositoryId, IFormFile rdfToValidate, IFormFile validationRdf, List<Guid> validationShapesList, List<ShapeConfigViewModel> repositoryShapes);
    }
}
