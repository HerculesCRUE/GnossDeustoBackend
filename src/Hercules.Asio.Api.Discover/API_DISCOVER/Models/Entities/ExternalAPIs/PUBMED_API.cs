// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using System.Xml.Linq;
using System.Xml.Serialization;

namespace API_DISCOVER.Models.Entities.ExternalAPIs
{
    /// <summary>
    /// Clase para interactuar con el API de PUBMED
    /// </summary>
    public class PUBMED_API : I_ExternalAPI
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get { return "PubMed"; } }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get { return "PubMed® comprises more than 30 million citations for biomedical literature from MEDLINE, life science journals, and online books."; } }
        /// <summary>
        /// HomePage
        /// </summary>
        public string HomePage { get { return "https://pubmed.ncbi.nlm.nih.gov/"; } }
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get { return "pubmed"; } }

        /// <summary>
        /// Busca documentos en función de su título API de PUBMED y obtiene sus identificadores
        /// </summary>
        /// <param name="q">Texto a buscar</param>
        /// <returns>Objeto con los identificadores de los documentos</returns>
        public static uint[] WorkSearchByTitle(string q)
        {
            string cadena = "https://eutils.ncbi.nlm.nih.gov/entrez/eutils/esearch.fcgi?term=" + q + "&field=title&sort=relevance&retmax=10";
            var doc = XElement.Load(cadena);
            XmlSerializer s = new XmlSerializer(typeof(eSearchResult));
            eSearchResult PUBMED_eSearchResult = s.Deserialize(doc.CreateReader()) as eSearchResult;
            return PUBMED_eSearchResult.IdList;
        }

        /// <summary>
        /// Obtiene un documento a través de su identificador
        /// </summary>
        /// <param name="id">Identificador</param>
        /// <returns>Documento</returns>
        public static PubmedArticleSet GetWorkByID(uint id)
        {
            var doc = XElement.Load("https://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi?db=pubmed&id=" + id + "&retmode=xml");
            XmlSerializer s = new XmlSerializer(typeof(PubmedArticleSet));
            var article = s.Deserialize(doc.CreateReader()) as PubmedArticleSet;
            return article;
        }
    }


    // NOTA: El código generado puede requerir, como mínimo, .NET Framework 4.5 o .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class eSearchResult
    {

        private uint[] idListField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Id", IsNullable = false)]
        public uint[] IdList
        {
            get
            {
                return this.idListField;
            }
            set
            {
                this.idListField = value;
            }
        }
    }


    // NOTA: El código generado puede requerir, como mínimo, .NET Framework 4.5 o .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class PubmedArticleSet
    {

        private PubmedArticleSetPubmedArticle pubmedArticleField;

        /// <remarks/>
        public PubmedArticleSetPubmedArticle PubmedArticle
        {
            get
            {
                return this.pubmedArticleField;
            }
            set
            {
                this.pubmedArticleField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticle
    {

        private PubmedArticleSetPubmedArticleMedlineCitation medlineCitationField;

        /// <remarks/>
        public PubmedArticleSetPubmedArticleMedlineCitation MedlineCitation
        {
            get
            {
                return this.medlineCitationField;
            }
            set
            {
                this.medlineCitationField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitation
    {
        private PubmedArticleSetPubmedArticleMedlineCitationArticle articleField;
        /// <remarks/>
        public PubmedArticleSetPubmedArticleMedlineCitationArticle Article
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
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticle
    {
        private PubmedArticleSetPubmedArticleMedlineCitationArticleArticleTitle articleTitleField;

        private PubmedArticleSetPubmedArticleMedlineCitationArticleELocationID[] eLocationIDField;

        private PubmedArticleSetPubmedArticleMedlineCitationArticleAuthorList authorListField;

        /// <remarks/>
        public PubmedArticleSetPubmedArticleMedlineCitationArticleArticleTitle ArticleTitle
        {
            get
            {
                return this.articleTitleField;
            }
            set
            {
                this.articleTitleField = value;
            }
        }


        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ELocationID")]
        public PubmedArticleSetPubmedArticleMedlineCitationArticleELocationID[] ELocationID
        {
            get
            {
                return this.eLocationIDField;
            }
            set
            {
                this.eLocationIDField = value;
            }
        }

        /// <remarks/>
        public PubmedArticleSetPubmedArticleMedlineCitationArticleAuthorList AuthorList
        {
            get
            {
                return this.authorListField;
            }
            set
            {
                this.authorListField = value;
            }
        }


    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticleArticleTitle
    {

        private string[] textField;

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticleELocationID
    {

        private string eIdTypeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string EIdType
        {
            get
            {
                return this.eIdTypeField;
            }
            set
            {
                this.eIdTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticleAuthorList
    {

        private PubmedArticleSetPubmedArticleMedlineCitationArticleAuthorListAuthor[] authorField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Author")]
        public PubmedArticleSetPubmedArticleMedlineCitationArticleAuthorListAuthor[] Author
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
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticleAuthorListAuthor
    {

        private string lastNameField;

        private string foreNameField;

        private PubmedArticleSetPubmedArticleMedlineCitationArticleAuthorListAuthorIdentifier identifierField;
        
        /// <remarks/>
        public string LastName
        {
            get
            {
                return this.lastNameField;
            }
            set
            {
                this.lastNameField = value;
            }
        }

        /// <remarks/>
        public string ForeName
        {
            get
            {
                return this.foreNameField;
            }
            set
            {
                this.foreNameField = value;
            }
        }

        /// <remarks/>
        public PubmedArticleSetPubmedArticleMedlineCitationArticleAuthorListAuthorIdentifier Identifier
        {
            get
            {
                return this.identifierField;
            }
            set
            {
                this.identifierField = value;
            }
        }

    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticleAuthorListAuthorIdentifier
    {

        private string sourceField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Source
        {
            get
            {
                return this.sourceField;
            }
            set
            {
                this.sourceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }
}
