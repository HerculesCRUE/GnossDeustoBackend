// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace API_DISCOVER.Models.Entities.ExternalAPIs
{
    /// <summary>
    /// Clase para interactuar con el API de scopus
    /// </summary>
    public class SCOPUS_API : I_ExternalAPI
    {
        public string Name { get { return "Scopus"; } }
        public string Description { get { return "Scopus is the largest abstract and citation database of peer-reviewed literature: scientific journals, books and conference proceedings. Delivering a comprehensive overview of the world's research output in the fields of science, technology, medicine, social sciences, and arts and humanities, Scopus features smart tools to track, analyze and visualize research."; } }

        public string HomePage { get { return "https://www.scopus.com/"; } }

        public string Id { get { return "scopus"; } }

        /// <summary>
        /// Obtiene los datos de una persona en el API de SCOPUS
        /// </summary>
        /// <param name="id">Identificador de SCOPUS</param>
        /// <param name="ScopusApiKey">ApiKey de Scopus</param>
        /// <param name="ScopusUrl">Url donde se encuentra el API de scopus</param>
        /// <returns>Objeto con los datos de la persona</returns>
        public static SCOPUSPerson Person(ulong id, string ScopusApiKey,string ScopusUrl)
        {
            string cadena = $"{ScopusUrl}content/author/author_id/{id}?apiKey={ScopusApiKey}";
            var doc = XElement.Load(cadena);
            XmlSerializer s = new XmlSerializer(typeof(SCOPUSPerson));
            var author = s.Deserialize(doc.CreateReader()) as SCOPUSPerson;
            return author;
        }

        /// <summary>
        /// Busca trabajos en el API de SCOPUS
        /// </summary>
        /// <param name="q">Texto a buscar (con urlEncode)</param>
        /// <param name="ScopusApiKey">ApiKey de Scopus</param>
        /// <param name="ScopusUrl">Url donde se encuentra el API de scopus</param>
        /// <returns>Objeto con los trabajos</returns>
        public static SCOPUSWorks Works(string q, string ScopusApiKey, string ScopusUrl)
        {
            string cadena = $"{ScopusUrl}content/search/scopus?query=TITLE({q})&view=COMPLETE&apiKey={ScopusApiKey}&httpAccept=application/xml";
            var doc = XElement.Load(cadena);
            XmlSerializer s = new XmlSerializer(typeof(SCOPUSWorks));
            var article = s.Deserialize(doc.CreateReader()) as SCOPUSWorks;
            article.entry.RemoveAll(x => !string.IsNullOrEmpty(x.error));
            return article;
        }
    }


    // NOTA: El código generado puede requerir, como mínimo, .NET Framework 4.5 o .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2005/Atom")]
    [System.Xml.Serialization.XmlRootAttribute("search-results", Namespace = "http://www.w3.org/2005/Atom", IsNullable = false)]
    public partial class SCOPUSWorks
    {
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("entry")]
        public List<Work> entry
        {
            get; set;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2005/Atom")]
    public partial class Work
    {
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://purl.org/dc/elements/1.1/")]
        public string identifier
        {
            get; set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://purl.org/dc/elements/1.1/")]
        public string title
        {
            get; set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://prismstandard.org/namespaces/basic/2.0/")]
        public string doi
        {
            get; set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("author")]
        public WorkAuthor[] author
        {
            get; set;
        }


        public string error
        {
            get; set;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2005/Atom")]
    public partial class WorkAuthor
    {
        /// <remarks/>
        public ulong authid
        {
            get; set;
        }

         /// <remarks/>
        public string surname
        {
            get; set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("given-name")]
        public string givenname
        {
            get; set;
        }
    }

    // NOTA: El código generado puede requerir, como mínimo, .NET Framework 4.5 o .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute("author-retrieval-response", Namespace = "", IsNullable = false)]
    public partial class SCOPUSPerson
    {
        /// <remarks/>
        public authorretrievalresponseCoredata coredata
        {
            get; set;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class authorretrievalresponseCoredata
    {
        /// <remarks/>
        public string orcid
        {
            get; set;
        }
    }
}
