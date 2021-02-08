// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace API_DISCOVER.Models.Entities.ExternalAPIs
{
    /// <summary>
    /// Clase para interactuar con el API de DBLP
    /// </summary>
    public class DBLP_API : I_ExternalAPI
    {
        private static List<string> properties = new List<string>() {
            "r|article|title",
            "r|inproceedings|title",
            "r|proceedings|title",
            "r|incollection|title",
            "r|collection|title"};
        
        public string Name { get { return "DBLP Computer Science Bibliography"; } }
        public string Description { get { return "The dblp computer science bibliography is the on-line reference for bibliographic information on major computer science publications. It has evolved from an early small experimental web server to a popular open-data service for the whole computer science community."; } }

        public string HomePage { get { return "https://dblp.org/"; } }

        public string Id { get { return "dblp"; } }

        /// <summary>
        /// Busca personas en el API de DBLP
        /// </summary>
        /// <param name="q">Texto a buscar (con urlEncode)</param>
        /// <returns>Objeto con las personas encontradas</returns>
        public static DBLPAuthors AuthorSearch(string q)
        {
            string cadena = $"https://dblp.org/search/author/api?q={q}&h=5";
            var doc = XElement.Load(cadena);
            doc=Limpiar(doc);
            XmlSerializer s = new XmlSerializer(typeof(DBLPAuthors));
            var authors = s.Deserialize(doc.CreateReader()) as DBLPAuthors;
            return authors;
        }

        /// <summary>
        /// Obtiene los datos de una persona en el API de DBLP
        /// </summary>
        /// <param name="url">Url de DBLP de ORCID</param>
        /// <returns>Objeto con los datos de la persona</returns>
        public static DBLPPerson Person(string url)
        {
            string cadena = url + ".xml";
            var doc = XElement.Load(cadena);
            doc = Limpiar(doc);
            XmlSerializer s = new XmlSerializer(typeof(DBLPPerson));
            var person = s.Deserialize(doc.CreateReader()) as DBLPPerson;
            return person;
        }

        /// <summary>
        /// Limpia el XML que obtenemos del API de DBLP (en ocasiones está mal formado)
        /// </summary>
        /// <param name="pElement">Elemento XML</param>
        /// <returns>Elemento XML limpio</returns>
        private static XElement Limpiar(XElement pElement)
        {
            foreach (string property in properties)
            {
                List<XElement> elements = null;
                foreach (string propertySplit in property.Split('|'))
                {
                    if (elements == null)
                    {
                        elements = pElement.Descendants(propertySplit).ToList();
                    }else if(elements.Count>0)
                    {
                        elements = elements.Descendants(propertySplit).ToList();
                    }
                }
                if (elements.Count > 0)
                {
                    for (int i = 0; i < elements.Count; i++)
                    {
                        XElement element = elements[i];
                        element.SetValue(element.Value);
                    }
                }
            }
            return pElement;
        }
    }


    // NOTA: El código generado puede requerir, como mínimo, .NET Framework 4.5 o .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute("result", IsNullable = false)]
    public partial class DBLPAuthors
    {
        private resultHits hitsField;

        /// <remarks/>
        public resultHits hits
        {
            get
            {
                return this.hitsField;
            }
            set
            {
                this.hitsField = value;
            }
        }
    }
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class resultHits
    {

        private resultHitsHit[] hitField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("hit")]
        public resultHitsHit[] hit
        {
            get
            {
                return this.hitField;
            }
            set
            {
                this.hitField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class resultHitsHit
    {

        private resultHitsHitInfo infoField;

        /// <remarks/>
        public resultHitsHitInfo info
        {
            get
            {
                return this.infoField;
            }
            set
            {
                this.infoField = value;
            }
        }
}

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class resultHitsHitInfo
    {
        private string authorField;

        private string urlField;

        /// <remarks/>
        public string author
        {
            get
            {
                return this.authorField;
            }
            set
            {
                this.authorField = value;
            }
        }
        /// <remarks/>
        public string url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }
    }

    // NOTA: El código generado puede requerir, como mínimo, .NET Framework 4.5 o .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute("dblpperson", Namespace = "", IsNullable = false)]
    public partial class DBLPPerson
    {

        private dblppersonPerson personField;

        private dblppersonR[] rField;

        /// <remarks/>
        public dblppersonPerson person
        {
            get
            {
                return this.personField;
            }
            set
            {
                this.personField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("r")]
        public dblppersonR[] r
        {
            get
            {
                return this.rField;
            }
            set
            {
                this.rField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class dblppersonPerson
    {

        private string[] urlField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("url")]
        public string[] url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class dblppersonR
    {

        private dblppersonRDocument articleField;
        private dblppersonRDocument inproceedingsField;
        private dblppersonRDocument proceedingsField;
        private dblppersonRDocument incollectionField;
        private dblppersonRDocument collectionField;

        /// <remarks/>
        public dblppersonRDocument article
        {
            get
            {
                return this.articleField;
            }
            set
            {
                this.articleField = value;
            }
        }

        public dblppersonRDocument proceedings
        {
            get
            {
                return this.proceedingsField;
            }
            set
            {
                this.proceedingsField = value;
            }
        }

        public dblppersonRDocument inproceedings
        {
            get
            {
                return this.inproceedingsField;
            }
            set
            {
                this.inproceedingsField = value;
            }
        }

        public dblppersonRDocument incollection
        {
            get
            {
                return this.incollectionField;
            }
            set
            {
                this.incollectionField = value;
            }
        }

        public dblppersonRDocument collection
        {
            get
            {
                return this.collectionField;
            }
            set
            {
                this.collectionField = value;
            }
        }   
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class dblppersonRDocument
    {

        private string titleField;

        private string[] eeField;

        private string keyField;

        /// <remarks/>
        public string title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }


        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ee")]
        public string[] ee
        {
            get
            {
                return this.eeField;
            }
            set
            {
                this.eeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string key
        {
            get
            {
                return this.keyField;
            }
            set
            {
                this.keyField = value;
            }
        }
    }
}
