// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace API_DISCOVER.Models.Entities
{
    /// <summary>
    /// Clase para interactuar con el API de PUBMED
    /// </summary>
    public static class PUBMED_API
    {
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

        private PubmedArticleSetPubmedArticlePubmedData pubmedDataField;

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

        /// <remarks/>
        public PubmedArticleSetPubmedArticlePubmedData PubmedData
        {
            get
            {
                return this.pubmedDataField;
            }
            set
            {
                this.pubmedDataField = value;
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

        private PubmedArticleSetPubmedArticleMedlineCitationDateCompleted dateCompletedField;

        private PubmedArticleSetPubmedArticleMedlineCitationDateRevised dateRevisedField;

        private PubmedArticleSetPubmedArticleMedlineCitationArticle articleField;

        private PubmedArticleSetPubmedArticleMedlineCitationMedlineJournalInfo medlineJournalInfoField;

        private PubmedArticleSetPubmedArticleMedlineCitationChemical[] chemicalListField;

        private string citationSubsetField;

        private string[] geneSymbolListField;

        private PubmedArticleSetPubmedArticleMedlineCitationCommentsCorrectionsList commentsCorrectionsListField;

        private PubmedArticleSetPubmedArticleMedlineCitationMeshHeading[] meshHeadingListField;

        private PubmedArticleSetPubmedArticleMedlineCitationKeywordList keywordListField;

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
        public PubmedArticleSetPubmedArticleMedlineCitationDateCompleted DateCompleted
        {
            get
            {
                return this.dateCompletedField;
            }
            set
            {
                this.dateCompletedField = value;
            }
        }

        /// <remarks/>
        public PubmedArticleSetPubmedArticleMedlineCitationDateRevised DateRevised
        {
            get
            {
                return this.dateRevisedField;
            }
            set
            {
                this.dateRevisedField = value;
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
        public PubmedArticleSetPubmedArticleMedlineCitationMedlineJournalInfo MedlineJournalInfo
        {
            get
            {
                return this.medlineJournalInfoField;
            }
            set
            {
                this.medlineJournalInfoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Chemical", IsNullable = false)]
        public PubmedArticleSetPubmedArticleMedlineCitationChemical[] ChemicalList
        {
            get
            {
                return this.chemicalListField;
            }
            set
            {
                this.chemicalListField = value;
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
        public PubmedArticleSetPubmedArticleMedlineCitationCommentsCorrectionsList CommentsCorrectionsList
        {
            get
            {
                return this.commentsCorrectionsListField;
            }
            set
            {
                this.commentsCorrectionsListField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("MeshHeading", IsNullable = false)]
        public PubmedArticleSetPubmedArticleMedlineCitationMeshHeading[] MeshHeadingList
        {
            get
            {
                return this.meshHeadingListField;
            }
            set
            {
                this.meshHeadingListField = value;
            }
        }

        /// <remarks/>
        public PubmedArticleSetPubmedArticleMedlineCitationKeywordList KeywordList
        {
            get
            {
                return this.keywordListField;
            }
            set
            {
                this.keywordListField = value;
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
    public partial class PubmedArticleSetPubmedArticleMedlineCitationDateCompleted
    {

        private ushort yearField;

        private byte monthField;

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
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationDateRevised
    {

        private ushort yearField;

        private byte monthField;

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
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticle
    {

        private PubmedArticleSetPubmedArticleMedlineCitationArticleJournal journalField;

        private PubmedArticleSetPubmedArticleMedlineCitationArticleArticleTitle articleTitleField;

        private PubmedArticleSetPubmedArticleMedlineCitationArticlePagination paginationField;

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
        public PubmedArticleSetPubmedArticleMedlineCitationArticlePagination Pagination
        {
            get
            {
                return this.paginationField;
            }
            set
            {
                this.paginationField = value;
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
    public partial class PubmedArticleSetPubmedArticleMedlineCitationArticlePagination
    {

        private string medlinePgnField;

        /// <remarks/>
        public string MedlinePgn
        {
            get
            {
                return this.medlinePgnField;
            }
            set
            {
                this.medlinePgnField = value;
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

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationMedlineJournalInfo
    {

        private string countryField;

        private string medlineTAField;

        private string nlmUniqueIDField;

        private string iSSNLinkingField;

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

        /// <remarks/>
        public string MedlineTA
        {
            get
            {
                return this.medlineTAField;
            }
            set
            {
                this.medlineTAField = value;
            }
        }

        /// <remarks/>
        public string NlmUniqueID
        {
            get
            {
                return this.nlmUniqueIDField;
            }
            set
            {
                this.nlmUniqueIDField = value;
            }
        }

        /// <remarks/>
        public string ISSNLinking
        {
            get
            {
                return this.iSSNLinkingField;
            }
            set
            {
                this.iSSNLinkingField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationChemical
    {

        private string registryNumberField;

        private PubmedArticleSetPubmedArticleMedlineCitationChemicalNameOfSubstance nameOfSubstanceField;

        /// <remarks/>
        public string RegistryNumber
        {
            get
            {
                return this.registryNumberField;
            }
            set
            {
                this.registryNumberField = value;
            }
        }

        /// <remarks/>
        public PubmedArticleSetPubmedArticleMedlineCitationChemicalNameOfSubstance NameOfSubstance
        {
            get
            {
                return this.nameOfSubstanceField;
            }
            set
            {
                this.nameOfSubstanceField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationChemicalNameOfSubstance
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
    public partial class PubmedArticleSetPubmedArticleMedlineCitationCommentsCorrectionsList
    {

        private PubmedArticleSetPubmedArticleMedlineCitationCommentsCorrectionsListCommentsCorrections commentsCorrectionsField;

        /// <remarks/>
        public PubmedArticleSetPubmedArticleMedlineCitationCommentsCorrectionsListCommentsCorrections CommentsCorrections
        {
            get
            {
                return this.commentsCorrectionsField;
            }
            set
            {
                this.commentsCorrectionsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationCommentsCorrectionsListCommentsCorrections
    {

        private string refSourceField;

        private PubmedArticleSetPubmedArticleMedlineCitationCommentsCorrectionsListCommentsCorrectionsPMID pMIDField;

        private string refTypeField;

        /// <remarks/>
        public string RefSource
        {
            get
            {
                return this.refSourceField;
            }
            set
            {
                this.refSourceField = value;
            }
        }

        /// <remarks/>
        public PubmedArticleSetPubmedArticleMedlineCitationCommentsCorrectionsListCommentsCorrectionsPMID PMID
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RefType
        {
            get
            {
                return this.refTypeField;
            }
            set
            {
                this.refTypeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationCommentsCorrectionsListCommentsCorrectionsPMID
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
    public partial class PubmedArticleSetPubmedArticleMedlineCitationMeshHeading
    {

        private PubmedArticleSetPubmedArticleMedlineCitationMeshHeadingDescriptorName descriptorNameField;

        private PubmedArticleSetPubmedArticleMedlineCitationMeshHeadingQualifierName qualifierNameField;

        /// <remarks/>
        public PubmedArticleSetPubmedArticleMedlineCitationMeshHeadingDescriptorName DescriptorName
        {
            get
            {
                return this.descriptorNameField;
            }
            set
            {
                this.descriptorNameField = value;
            }
        }

        /// <remarks/>
        public PubmedArticleSetPubmedArticleMedlineCitationMeshHeadingQualifierName QualifierName
        {
            get
            {
                return this.qualifierNameField;
            }
            set
            {
                this.qualifierNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticleMedlineCitationMeshHeadingDescriptorName
    {

        private string uiField;

        private string majorTopicYNField;

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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MajorTopicYN
        {
            get
            {
                return this.majorTopicYNField;
            }
            set
            {
                this.majorTopicYNField = value;
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
    public partial class PubmedArticleSetPubmedArticleMedlineCitationMeshHeadingQualifierName
    {

        private string uiField;

        private string majorTopicYNField;

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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MajorTopicYN
        {
            get
            {
                return this.majorTopicYNField;
            }
            set
            {
                this.majorTopicYNField = value;
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
    public partial class PubmedArticleSetPubmedArticleMedlineCitationKeywordList
    {

        private PubmedArticleSetPubmedArticleMedlineCitationKeywordListKeyword[] keywordField;

        private string ownerField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Keyword")]
        public PubmedArticleSetPubmedArticleMedlineCitationKeywordListKeyword[] Keyword
        {
            get
            {
                return this.keywordField;
            }
            set
            {
                this.keywordField = value;
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
    public partial class PubmedArticleSetPubmedArticleMedlineCitationKeywordListKeyword
    {

        private string majorTopicYNField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MajorTopicYN
        {
            get
            {
                return this.majorTopicYNField;
            }
            set
            {
                this.majorTopicYNField = value;
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
    public partial class PubmedArticleSetPubmedArticlePubmedData
    {

        private PubmedArticleSetPubmedArticlePubmedDataPubMedPubDate[] historyField;

        private string publicationStatusField;

        private PubmedArticleSetPubmedArticlePubmedDataArticleId[] articleIdListField;

        private PubmedArticleSetPubmedArticlePubmedDataReference[] referenceListField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("PubMedPubDate", IsNullable = false)]
        public PubmedArticleSetPubmedArticlePubmedDataPubMedPubDate[] History
        {
            get
            {
                return this.historyField;
            }
            set
            {
                this.historyField = value;
            }
        }

        /// <remarks/>
        public string PublicationStatus
        {
            get
            {
                return this.publicationStatusField;
            }
            set
            {
                this.publicationStatusField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ArticleId", IsNullable = false)]
        public PubmedArticleSetPubmedArticlePubmedDataArticleId[] ArticleIdList
        {
            get
            {
                return this.articleIdListField;
            }
            set
            {
                this.articleIdListField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Reference", IsNullable = false)]
        public PubmedArticleSetPubmedArticlePubmedDataReference[] ReferenceList
        {
            get
            {
                return this.referenceListField;
            }
            set
            {
                this.referenceListField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticlePubmedDataPubMedPubDate
    {

        private ushort yearField;

        private byte monthField;

        private byte dayField;

        private byte hourField;

        private bool hourFieldSpecified;

        private byte minuteField;

        private bool minuteFieldSpecified;

        private string pubStatusField;

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
        public byte Hour
        {
            get
            {
                return this.hourField;
            }
            set
            {
                this.hourField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool HourSpecified
        {
            get
            {
                return this.hourFieldSpecified;
            }
            set
            {
                this.hourFieldSpecified = value;
            }
        }

        /// <remarks/>
        public byte Minute
        {
            get
            {
                return this.minuteField;
            }
            set
            {
                this.minuteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool MinuteSpecified
        {
            get
            {
                return this.minuteFieldSpecified;
            }
            set
            {
                this.minuteFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string PubStatus
        {
            get
            {
                return this.pubStatusField;
            }
            set
            {
                this.pubStatusField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticlePubmedDataArticleId
    {

        private string idTypeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string IdType
        {
            get
            {
                return this.idTypeField;
            }
            set
            {
                this.idTypeField = value;
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
    public partial class PubmedArticleSetPubmedArticlePubmedDataReference
    {

        private string citationField;

        private PubmedArticleSetPubmedArticlePubmedDataReferenceArticleIdList articleIdListField;

        /// <remarks/>
        public string Citation
        {
            get
            {
                return this.citationField;
            }
            set
            {
                this.citationField = value;
            }
        }

        /// <remarks/>
        public PubmedArticleSetPubmedArticlePubmedDataReferenceArticleIdList ArticleIdList
        {
            get
            {
                return this.articleIdListField;
            }
            set
            {
                this.articleIdListField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticlePubmedDataReferenceArticleIdList
    {

        private PubmedArticleSetPubmedArticlePubmedDataReferenceArticleIdListArticleId articleIdField;

        /// <remarks/>
        public PubmedArticleSetPubmedArticlePubmedDataReferenceArticleIdListArticleId ArticleId
        {
            get
            {
                return this.articleIdField;
            }
            set
            {
                this.articleIdField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PubmedArticleSetPubmedArticlePubmedDataReferenceArticleIdListArticleId
    {

        private string idTypeField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string IdType
        {
            get
            {
                return this.idTypeField;
            }
            set
            {
                this.idTypeField = value;
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


}
