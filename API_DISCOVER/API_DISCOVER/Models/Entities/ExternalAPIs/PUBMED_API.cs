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
    {//TODO cambiar
        public string Name { get { return "PubMed"; } }

        public string HomePage { get { return "https://pubmed.ncbi.nlm.nih.gov/"; } }

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

        private PubmedArticleSetPubmedArticleMedlineCitationPMID pMIDField;


        private PubmedArticleSetPubmedArticleMedlineCitationArticle articleField;


        private string citationSubsetField;

        private string[] geneSymbolListField;

        private string statusField;

        private string ownerField;

        /// <remarks/>
        public PubmedArticleSetPubmedArticleMedlineCitationPMID PMID
        {
            get
            {
                return this.pMIDField;
            }
            set
            {
                this.pMIDField = value;
            }
        }

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

        /// <remarks/>
        public string CitationSubset
        {
            get
            {
                return this.citationSubsetField;
            }
            set
            {
                this.citationSubsetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("GeneSymbol", IsNullable = false)]
        public string[] GeneSymbolList
        {
            get
            {
                return this.geneSymbolListField;
            }
            set
            {
                this.geneSymbolListField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Owner
        {
            get
            {
                return this.ownerField;
            }
            set
            {
                this.ownerField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationPMID
    {

        private byte versionField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
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
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticle
    {

        private PubmedArticleSetPubmedArticleMedlineCitationArticleJournal journalField;

        private PubmedArticleSetPubmedArticleMedlineCitationArticleArticleTitle articleTitleField;

        private PubmedArticleSetPubmedArticleMedlineCitationArticleELocationID[] eLocationIDField;

        private PubmedArticleSetPubmedArticleMedlineCitationArticleAbstract abstractField;

        private PubmedArticleSetPubmedArticleMedlineCitationArticleAuthorList authorListField;

        private string languageField;

        private PubmedArticleSetPubmedArticleMedlineCitationArticleDataBankList dataBankListField;

        private PubmedArticleSetPubmedArticleMedlineCitationArticleGrantList grantListField;

        private PubmedArticleSetPubmedArticleMedlineCitationArticlePublicationType[] publicationTypeListField;

        private PubmedArticleSetPubmedArticleMedlineCitationArticleArticleDate articleDateField;

        private string pubModelField;

        /// <remarks/>
        public PubmedArticleSetPubmedArticleMedlineCitationArticleJournal Journal
        {
            get
            {
                return this.journalField;
            }
            set
            {
                this.journalField = value;
            }
        }

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
        public PubmedArticleSetPubmedArticleMedlineCitationArticleAbstract Abstract
        {
            get
            {
                return this.abstractField;
            }
            set
            {
                this.abstractField = value;
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

        /// <remarks/>
        public string Language
        {
            get
            {
                return this.languageField;
            }
            set
            {
                this.languageField = value;
            }
        }

        /// <remarks/>
        public PubmedArticleSetPubmedArticleMedlineCitationArticleDataBankList DataBankList
        {
            get
            {
                return this.dataBankListField;
            }
            set
            {
                this.dataBankListField = value;
            }
        }

        /// <remarks/>
        public PubmedArticleSetPubmedArticleMedlineCitationArticleGrantList GrantList
        {
            get
            {
                return this.grantListField;
            }
            set
            {
                this.grantListField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("PublicationType", IsNullable = false)]
        public PubmedArticleSetPubmedArticleMedlineCitationArticlePublicationType[] PublicationTypeList
        {
            get
            {
                return this.publicationTypeListField;
            }
            set
            {
                this.publicationTypeListField = value;
            }
        }

        /// <remarks/>
        public PubmedArticleSetPubmedArticleMedlineCitationArticleArticleDate ArticleDate
        {
            get
            {
                return this.articleDateField;
            }
            set
            {
                this.articleDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string PubModel
        {
            get
            {
                return this.pubModelField;
            }
            set
            {
                this.pubModelField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticleJournal
    {

        private PubmedArticleSetPubmedArticleMedlineCitationArticleJournalISSN iSSNField;

        private PubmedArticleSetPubmedArticleMedlineCitationArticleJournalJournalIssue journalIssueField;

        private string titleField;

        private string iSOAbbreviationField;

        /// <remarks/>
        public PubmedArticleSetPubmedArticleMedlineCitationArticleJournalISSN ISSN
        {
            get
            {
                return this.iSSNField;
            }
            set
            {
                this.iSSNField = value;
            }
        }

        /// <remarks/>
        public PubmedArticleSetPubmedArticleMedlineCitationArticleJournalJournalIssue JournalIssue
        {
            get
            {
                return this.journalIssueField;
            }
            set
            {
                this.journalIssueField = value;
            }
        }

        /// <remarks/>
        public string Title
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
        public string ISOAbbreviation
        {
            get
            {
                return this.iSOAbbreviationField;
            }
            set
            {
                this.iSOAbbreviationField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticleJournalISSN
    {

        private string issnTypeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string IssnType
        {
            get
            {
                return this.issnTypeField;
            }
            set
            {
                this.issnTypeField = value;
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
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticleJournalJournalIssue
    {

        private uint volumeField;

        private byte issueField;

        private PubmedArticleSetPubmedArticleMedlineCitationArticleJournalJournalIssuePubDate pubDateField;

        private string citedMediumField;

        /// <remarks/>
        public uint Volume
        {
            get
            {
                return this.volumeField;
            }
            set
            {
                this.volumeField = value;
            }
        }

        /// <remarks/>
        public byte Issue
        {
            get
            {
                return this.issueField;
            }
            set
            {
                this.issueField = value;
            }
        }

        /// <remarks/>
        public PubmedArticleSetPubmedArticleMedlineCitationArticleJournalJournalIssuePubDate PubDate
        {
            get
            {
                return this.pubDateField;
            }
            set
            {
                this.pubDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CitedMedium
        {
            get
            {
                return this.citedMediumField;
            }
            set
            {
                this.citedMediumField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticleJournalJournalIssuePubDate
    {

        private ushort yearField;

        private string monthField;

        private byte dayField;

        /// <remarks/>
        public ushort Year
        {
            get
            {
                return this.yearField;
            }
            set
            {
                this.yearField = value;
            }
        }

        /// <remarks/>
        public string Month
        {
            get
            {
                return this.monthField;
            }
            set
            {
                this.monthField = value;
            }
        }

        /// <remarks/>
        public byte Day
        {
            get
            {
                return this.dayField;
            }
            set
            {
                this.dayField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticleArticleTitle
    {

        private string iField;

        private string[] textField;

        /// <remarks/>
        public string i
        {
            get
            {
                return this.iField;
            }
            set
            {
                this.iField = value;
            }
        }

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

        private string validYNField;

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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ValidYN
        {
            get
            {
                return this.validYNField;
            }
            set
            {
                this.validYNField = value;
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
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticleAbstract
    {

        private PubmedArticleSetPubmedArticleMedlineCitationArticleAbstractAbstractText abstractTextField;

        /// <remarks/>
        public PubmedArticleSetPubmedArticleMedlineCitationArticleAbstractAbstractText AbstractText
        {
            get
            {
                return this.abstractTextField;
            }
            set
            {
                this.abstractTextField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticleAbstractAbstractText
    {

        private string[] iField;

        private string[] textField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("i")]
        public string[] i
        {
            get
            {
                return this.iField;
            }
            set
            {
                this.iField = value;
            }
        }

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
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticleAuthorList
    {

        private PubmedArticleSetPubmedArticleMedlineCitationArticleAuthorListAuthor[] authorField;

        private string completeYNField;

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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CompleteYN
        {
            get
            {
                return this.completeYNField;
            }
            set
            {
                this.completeYNField = value;
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

        private string initialsField;

        private PubmedArticleSetPubmedArticleMedlineCitationArticleAuthorListAuthorIdentifier identifierField;

        private PubmedArticleSetPubmedArticleMedlineCitationArticleAuthorListAuthorAffiliationInfo affiliationInfoField;

        private string validYNField;

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
        public string Initials
        {
            get
            {
                return this.initialsField;
            }
            set
            {
                this.initialsField = value;
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

        /// <remarks/>
        public PubmedArticleSetPubmedArticleMedlineCitationArticleAuthorListAuthorAffiliationInfo AffiliationInfo
        {
            get
            {
                return this.affiliationInfoField;
            }
            set
            {
                this.affiliationInfoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ValidYN
        {
            get
            {
                return this.validYNField;
            }
            set
            {
                this.validYNField = value;
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

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticleAuthorListAuthorAffiliationInfo
    {

        private string affiliationField;

        /// <remarks/>
        public string Affiliation
        {
            get
            {
                return this.affiliationField;
            }
            set
            {
                this.affiliationField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticleDataBankList
    {

        private PubmedArticleSetPubmedArticleMedlineCitationArticleDataBankListDataBank dataBankField;

        private string completeYNField;

        /// <remarks/>
        public PubmedArticleSetPubmedArticleMedlineCitationArticleDataBankListDataBank DataBank
        {
            get
            {
                return this.dataBankField;
            }
            set
            {
                this.dataBankField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CompleteYN
        {
            get
            {
                return this.completeYNField;
            }
            set
            {
                this.completeYNField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticleDataBankListDataBank
    {

        private string dataBankNameField;

        private string[] accessionNumberListField;

        /// <remarks/>
        public string DataBankName
        {
            get
            {
                return this.dataBankNameField;
            }
            set
            {
                this.dataBankNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("AccessionNumber", IsNullable = false)]
        public string[] AccessionNumberList
        {
            get
            {
                return this.accessionNumberListField;
            }
            set
            {
                this.accessionNumberListField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticleGrantList
    {

        private PubmedArticleSetPubmedArticleMedlineCitationArticleGrantListGrant[] grantField;

        private string completeYNField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Grant")]
        public PubmedArticleSetPubmedArticleMedlineCitationArticleGrantListGrant[] Grant
        {
            get
            {
                return this.grantField;
            }
            set
            {
                this.grantField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CompleteYN
        {
            get
            {
                return this.completeYNField;
            }
            set
            {
                this.completeYNField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticleGrantListGrant
    {

        private string grantIDField;

        private string acronymField;

        private string agencyField;

        private string countryField;

        /// <remarks/>
        public string GrantID
        {
            get
            {
                return this.grantIDField;
            }
            set
            {
                this.grantIDField = value;
            }
        }

        /// <remarks/>
        public string Acronym
        {
            get
            {
                return this.acronymField;
            }
            set
            {
                this.acronymField = value;
            }
        }

        /// <remarks/>
        public string Agency
        {
            get
            {
                return this.agencyField;
            }
            set
            {
                this.agencyField = value;
            }
        }

        /// <remarks/>
        public string Country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticlePublicationType
    {

        private string uiField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string UI
        {
            get
            {
                return this.uiField;
            }
            set
            {
                this.uiField = value;
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
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticleArticleDate
    {

        private ushort yearField;

        private byte monthField;

        private byte dayField;

        private string dateTypeField;

        /// <remarks/>
        public ushort Year
        {
            get
            {
                return this.yearField;
            }
            set
            {
                this.yearField = value;
            }
        }

        /// <remarks/>
        public byte Month
        {
            get
            {
                return this.monthField;
            }
            set
            {
                this.monthField = value;
            }
        }

        /// <remarks/>
        public byte Day
        {
            get
            {
                return this.dayField;
            }
            set
            {
                this.dayField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DateType
        {
            get
            {
                return this.dateTypeField;
            }
            set
            {
                this.dateTypeField = value;
            }
        }
    }
}
