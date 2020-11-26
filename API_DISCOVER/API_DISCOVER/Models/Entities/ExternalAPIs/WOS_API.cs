// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace API_DISCOVER.Models.Entities.ExternalAPIs
{
    /// <summary>
    /// Clase para interactuar con el API de WOS
    /// </summary>
    public static class WOS_API
    {
        /// <summary>
        /// Cookie para las peticiones
        /// </summary>
        private static string _cookie { get; set; }

        /// <summary>
        /// Genera la cooie para las peticiones al API de WOS
        /// </summary>
        /// <param name="authorization"></param>
        private static void ActualizarCookie(string authorization)
        {
            //Se pueden hacer un máximo de 5 peticiones cada 5 minutos a este método del API, por lo que si falla hacemos un retintento tras 5 minutos            
            for (int i = 0; i < 2; i++)
            {
                try
                {

                    string xml = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:auth=\"http://auth.cxf.wokmws.thomsonreuters.com\"><soapenv:Header/><soapenv:Body><auth:authenticate/></soapenv:Body></soapenv:Envelope>";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://search.webofknowledge.com/esti/wokmws/ws/WOKMWSAuthenticate");
                    request.Headers.Add("Authorization", authorization);
                    byte[] bytes = System.Text.Encoding.ASCII.GetBytes(xml);

                    // indicate what we are posting in the request
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = bytes.Length;

                    using (Stream os = request.GetRequestStream())
                    {
                        os.Write(bytes, 0, bytes.Length);
                    }

                    string response = "";

                    // get response
                    using (System.Net.WebResponse resp = request.GetResponse())
                    {
                        using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                        {
                            response = sr.ReadToEnd().Trim();
                        }
                    }

                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                    doc.LoadXml(response);

                    _cookie = doc.InnerText.ToString();
                    break;
                }
                catch (Exception ex)
                {
                    if (i == 1)
                    {
                        throw ex;
                    }
                    Thread.Sleep(300000);
                }
            }
        }

        /// <summary>
        /// Busca documentos en función de su título API de WOS 
        /// </summary>
        /// <param name="q">Texto a buscar</param>        
        /// <param name="authorization">Autorización</param>
        /// <returns>Objeto con los identificadores de los documentos</returns>
        public static WOSWorks Works(string q,string authorization)
        {
            //Se pueden hacer un máximo de 2 peticiones cada segundo método del API. por lo que tras cada petición dormimos 500ms
            //Si falla refrescamos la cookie y reintenamos
            if (_cookie == null)
            {
                ActualizarCookie(authorization);
            }
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    string xml = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\"  xmlns:woksearch=\"http://woksearch.v3.wokmws.thomsonreuters.com\"><soapenv:Header/><soapenv:Body><woksearch:search><queryParameters><databaseId>WOS</databaseId>   <userQuery>TI=\"" + q + "\"</userQuery><editions><collection>WOS</collection><edition>SCI</edition></editions>          <queryLanguage>en</queryLanguage></queryParameters><retrieveParameters><firstRecord>1</firstRecord><count>10</count><option><key>RecordIDs</key><value>On</value></option><option>            <key>targetNamespace</key><value>http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord</value></option>            </retrieveParameters></woksearch:search></soapenv:Body></soapenv:Envelope>";

                    var messagesElement = XElement.Parse(xml);

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://search.webofknowledge.com/esti/wokmws/ws/WokSearch");
                    request.Headers.Add("Cookie", "SID=\"" + _cookie + "\"; Version=1;");
                    byte[] bytes = System.Text.Encoding.ASCII.GetBytes(xml);

                    // indicate what we are posting in the request
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = bytes.Length;

                    using (Stream os = request.GetRequestStream())
                    {
                        os.Write(bytes, 0, bytes.Length);
                    }
                    string response = "";
                    using (System.Net.WebResponse resp = request.GetResponse())
                    {
                        using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                        {
                            response = sr.ReadToEnd().Trim();
                        }
                    }
                    response= response.Replace("&lt;", "<").Replace("&gt;", ">");
                    XDocument xdoc = XDocument.Parse(response);
                    foreach (var node in xdoc.Descendants().Where(e => e.Attribute("{http://www.w3.org/2001/XMLSchema-instance}type") != null))
                    {
                        node.Attribute("{http://www.w3.org/2001/XMLSchema-instance}type").Remove();
                    }
                    response = xdoc.ToString();
                    XmlSerializer serializer = new XmlSerializer(typeof(WOSWorks));
                    WOSWorks result = serializer.Deserialize(xdoc.CreateReader()) as WOSWorks;
                    return result;
                }catch(Exception ex)
                {
                    if (i == 1)
                    {
                        throw ex;
                    }
                    ActualizarCookie(authorization);
                }finally
                {
                    Thread.Sleep(500);
                }
            }
            throw new Exception("Ha fallado la llamada al API 'http://search.webofknowledge.com/esti/wokmws/ws/WokSearch' ");
        }
    }


    // NOTA: El código generado puede requerir, como mínimo, .NET Framework 4.5 o .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [System.Xml.Serialization.XmlRootAttribute("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    public partial class WOSWorks
    {

        private EnvelopeBody bodyField;

        /// <remarks/>
        public EnvelopeBody Body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public partial class EnvelopeBody
    {

        private searchResponse searchResponseField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://woksearch.v3.wokmws.thomsonreuters.com")]
        public searchResponse searchResponse
        {
            get
            {
                return this.searchResponseField;
            }
            set
            {
                this.searchResponseField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://woksearch.v3.wokmws.thomsonreuters.com")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://woksearch.v3.wokmws.thomsonreuters.com", IsNullable = false)]
    public partial class searchResponse
    {

        private @return returnField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "")]
        public @return @return
        {
            get
            {
                return this.returnField;
            }
            set
            {
                this.returnField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class @return
    {

        private byte queryIdField;

        private ushort recordsFoundField;

        private uint recordsSearchedField;

        private returnOptionValue optionValueField;

        private returnRecords recordsField;

        /// <remarks/>
        public byte queryId
        {
            get
            {
                return this.queryIdField;
            }
            set
            {
                this.queryIdField = value;
            }
        }

        /// <remarks/>
        public ushort recordsFound
        {
            get
            {
                return this.recordsFoundField;
            }
            set
            {
                this.recordsFoundField = value;
            }
        }

        /// <remarks/>
        public uint recordsSearched
        {
            get
            {
                return this.recordsSearchedField;
            }
            set
            {
                this.recordsSearchedField = value;
            }
        }

        /// <remarks/>
        public returnOptionValue optionValue
        {
            get
            {
                return this.optionValueField;
            }
            set
            {
                this.optionValueField = value;
            }
        }

        /// <remarks/>
        public returnRecords records
        {
            get
            {
                return this.recordsField;
            }
            set
            {
                this.recordsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class returnOptionValue
    {

        private string labelField;

        private string[] valueField;

        /// <remarks/>
        public string label
        {
            get
            {
                return this.labelField;
            }
            set
            {
                this.labelField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("value")]
        public string[] value
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
    public partial class returnRecords
    {

        private recordsREC[] recordsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
        [System.Xml.Serialization.XmlArrayItemAttribute("REC", IsNullable = false)]
        public recordsREC[] records
        {
            get
            {
                return this.recordsField;
            }
            set
            {
                this.recordsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsREC
    {

        private string uIDField;

        private recordsRECStatic_data static_dataField;

        private recordsRECDynamic_data dynamic_dataField;

        private string r_id_disclaimerField;

        /// <remarks/>
        public string UID
        {
            get
            {
                return this.uIDField;
            }
            set
            {
                this.uIDField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_data static_data
        {
            get
            {
                return this.static_dataField;
            }
            set
            {
                this.static_dataField = value;
            }
        }

        /// <remarks/>
        public recordsRECDynamic_data dynamic_data
        {
            get
            {
                return this.dynamic_dataField;
            }
            set
            {
                this.dynamic_dataField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string r_id_disclaimer
        {
            get
            {
                return this.r_id_disclaimerField;
            }
            set
            {
                this.r_id_disclaimerField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_data
    {

        private recordsRECStatic_dataSummary summaryField;

        private recordsRECStatic_dataFullrecord_metadata fullrecord_metadataField;

        private recordsRECStatic_dataItem itemField;

        private recordsRECStatic_dataContributors contributorsField;

        /// <remarks/>
        public recordsRECStatic_dataSummary summary
        {
            get
            {
                return this.summaryField;
            }
            set
            {
                this.summaryField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadata fullrecord_metadata
        {
            get
            {
                return this.fullrecord_metadataField;
            }
            set
            {
                this.fullrecord_metadataField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataItem item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataContributors contributors
        {
            get
            {
                return this.contributorsField;
            }
            set
            {
                this.contributorsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummary
    {

        private recordsRECStatic_dataSummaryEWUID eWUIDField;

        private recordsRECStatic_dataSummaryPub_info pub_infoField;

        private recordsRECStatic_dataSummaryTitles titlesField;

        private recordsRECStatic_dataSummaryNames namesField;

        private recordsRECStatic_dataSummaryDoctypes doctypesField;

        private recordsRECStatic_dataSummaryConferences conferencesField;

        private recordsRECStatic_dataSummaryPublishers publishersField;

        /// <remarks/>
        public recordsRECStatic_dataSummaryEWUID EWUID
        {
            get
            {
                return this.eWUIDField;
            }
            set
            {
                this.eWUIDField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataSummaryPub_info pub_info
        {
            get
            {
                return this.pub_infoField;
            }
            set
            {
                this.pub_infoField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataSummaryTitles titles
        {
            get
            {
                return this.titlesField;
            }
            set
            {
                this.titlesField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataSummaryNames names
        {
            get
            {
                return this.namesField;
            }
            set
            {
                this.namesField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataSummaryDoctypes doctypes
        {
            get
            {
                return this.doctypesField;
            }
            set
            {
                this.doctypesField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataSummaryConferences conferences
        {
            get
            {
                return this.conferencesField;
            }
            set
            {
                this.conferencesField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataSummaryPublishers publishers
        {
            get
            {
                return this.publishersField;
            }
            set
            {
                this.publishersField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryEWUID
    {

        private recordsRECStatic_dataSummaryEWUIDWUID wUIDField;

        private recordsRECStatic_dataSummaryEWUIDEdition[] editionField;

        /// <remarks/>
        public recordsRECStatic_dataSummaryEWUIDWUID WUID
        {
            get
            {
                return this.wUIDField;
            }
            set
            {
                this.wUIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("edition")]
        public recordsRECStatic_dataSummaryEWUIDEdition[] edition
        {
            get
            {
                return this.editionField;
            }
            set
            {
                this.editionField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryEWUIDWUID
    {

        private string coll_idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string coll_id
        {
            get
            {
                return this.coll_idField;
            }
            set
            {
                this.coll_idField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryEWUIDEdition
    {

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryPub_info
    {

        private recordsRECStatic_dataSummaryPub_infoPage pageField;

        private System.DateTime sortdateField;

        private ushort pubyearField;

        private string has_abstractField;

        private string coverdateField;

        private string pubmonthField;

        private byte volField;

        private byte issueField;

        private string pubtypeField;

        /// <remarks/>
        public recordsRECStatic_dataSummaryPub_infoPage page
        {
            get
            {
                return this.pageField;
            }
            set
            {
                this.pageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "date")]
        public System.DateTime sortdate
        {
            get
            {
                return this.sortdateField;
            }
            set
            {
                this.sortdateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort pubyear
        {
            get
            {
                return this.pubyearField;
            }
            set
            {
                this.pubyearField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string has_abstract
        {
            get
            {
                return this.has_abstractField;
            }
            set
            {
                this.has_abstractField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string coverdate
        {
            get
            {
                return this.coverdateField;
            }
            set
            {
                this.coverdateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string pubmonth
        {
            get
            {
                return this.pubmonthField;
            }
            set
            {
                this.pubmonthField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte vol
        {
            get
            {
                return this.volField;
            }
            set
            {
                this.volField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte issue
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string pubtype
        {
            get
            {
                return this.pubtypeField;
            }
            set
            {
                this.pubtypeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryPub_infoPage
    {

        private string beginField;

        private string endField;

        private byte page_countField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string begin
        {
            get
            {
                return this.beginField;
            }
            set
            {
                this.beginField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string end
        {
            get
            {
                return this.endField;
            }
            set
            {
                this.endField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte page_count
        {
            get
            {
                return this.page_countField;
            }
            set
            {
                this.page_countField = value;
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryTitles
    {

        private recordsRECStatic_dataSummaryTitlesTitle[] titleField;

        private byte countField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("title")]
        public recordsRECStatic_dataSummaryTitlesTitle[] title
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryTitlesTitle
    {

        private string typeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryNames
    {

        private recordsRECStatic_dataSummaryNamesName[] nameField;

        private byte countField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("name")]
        public recordsRECStatic_dataSummaryNamesName[] name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryNamesName
    {

        private string display_nameField;

        private recordsRECStatic_dataSummaryNamesNamePreferred_name preferred_nameField;

        private string full_nameField;

        private string wos_standardField;

        private string first_nameField;

        private string last_nameField;

        private string suffixField;

        private recordsRECStatic_dataSummaryNamesNameDataitemid[] dataitemidsField;

        private byte seq_noField;

        private string roleField;

        private string reprintField;

        private uint daisng_idField;

        private bool claim_statusField;

        private bool claim_statusFieldSpecified;

        /// <remarks/>
        public string display_name
        {
            get
            {
                return this.display_nameField;
            }
            set
            {
                this.display_nameField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataSummaryNamesNamePreferred_name preferred_name
        {
            get
            {
                return this.preferred_nameField;
            }
            set
            {
                this.preferred_nameField = value;
            }
        }

        /// <remarks/>
        public string full_name
        {
            get
            {
                return this.full_nameField;
            }
            set
            {
                this.full_nameField = value;
            }
        }

        /// <remarks/>
        public string wos_standard
        {
            get
            {
                return this.wos_standardField;
            }
            set
            {
                this.wos_standardField = value;
            }
        }

        /// <remarks/>
        public string first_name
        {
            get
            {
                return this.first_nameField;
            }
            set
            {
                this.first_nameField = value;
            }
        }

        /// <remarks/>
        public string last_name
        {
            get
            {
                return this.last_nameField;
            }
            set
            {
                this.last_nameField = value;
            }
        }

        /// <remarks/>
        public string suffix
        {
            get
            {
                return this.suffixField;
            }
            set
            {
                this.suffixField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute("data-item-ids")]
        [System.Xml.Serialization.XmlArrayItemAttribute("data-item-id", IsNullable = false)]
        public recordsRECStatic_dataSummaryNamesNameDataitemid[] dataitemids
        {
            get
            {
                return this.dataitemidsField;
            }
            set
            {
                this.dataitemidsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte seq_no
        {
            get
            {
                return this.seq_noField;
            }
            set
            {
                this.seq_noField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string role
        {
            get
            {
                return this.roleField;
            }
            set
            {
                this.roleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string reprint
        {
            get
            {
                return this.reprintField;
            }
            set
            {
                this.reprintField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint daisng_id
        {
            get
            {
                return this.daisng_idField;
            }
            set
            {
                this.daisng_idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool claim_status
        {
            get
            {
                return this.claim_statusField;
            }
            set
            {
                this.claim_statusField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool claim_statusSpecified
        {
            get
            {
                return this.claim_statusFieldSpecified;
            }
            set
            {
                this.claim_statusFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryNamesNamePreferred_name
    {

        private string full_nameField;

        private string first_nameField;

        private string last_nameField;

        /// <remarks/>
        public string full_name
        {
            get
            {
                return this.full_nameField;
            }
            set
            {
                this.full_nameField = value;
            }
        }

        /// <remarks/>
        public string first_name
        {
            get
            {
                return this.first_nameField;
            }
            set
            {
                this.first_nameField = value;
            }
        }

        /// <remarks/>
        public string last_name
        {
            get
            {
                return this.last_nameField;
            }
            set
            {
                this.last_nameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryNamesNameDataitemid
    {

        private string idtypeField;

        private string typeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("id-type")]
        public string idtype
        {
            get
            {
                return this.idtypeField;
            }
            set
            {
                this.idtypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryDoctypes
    {

        private string[] doctypeField;

        private byte countField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("doctype")]
        public string[] doctype
        {
            get
            {
                return this.doctypeField;
            }
            set
            {
                this.doctypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryConferences
    {

        private recordsRECStatic_dataSummaryConferencesConference conferenceField;

        private byte countField;

        /// <remarks/>
        public recordsRECStatic_dataSummaryConferencesConference conference
        {
            get
            {
                return this.conferenceField;
            }
            set
            {
                this.conferenceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryConferencesConference
    {

        private recordsRECStatic_dataSummaryConferencesConferenceConf_infos conf_infosField;

        private recordsRECStatic_dataSummaryConferencesConferenceConf_titles conf_titlesField;

        private recordsRECStatic_dataSummaryConferencesConferenceConf_dates conf_datesField;

        private recordsRECStatic_dataSummaryConferencesConferenceConf_locations conf_locationsField;

        private recordsRECStatic_dataSummaryConferencesConferenceSponsors sponsorsField;

        /// <remarks/>
        public recordsRECStatic_dataSummaryConferencesConferenceConf_infos conf_infos
        {
            get
            {
                return this.conf_infosField;
            }
            set
            {
                this.conf_infosField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataSummaryConferencesConferenceConf_titles conf_titles
        {
            get
            {
                return this.conf_titlesField;
            }
            set
            {
                this.conf_titlesField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataSummaryConferencesConferenceConf_dates conf_dates
        {
            get
            {
                return this.conf_datesField;
            }
            set
            {
                this.conf_datesField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataSummaryConferencesConferenceConf_locations conf_locations
        {
            get
            {
                return this.conf_locationsField;
            }
            set
            {
                this.conf_locationsField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataSummaryConferencesConferenceSponsors sponsors
        {
            get
            {
                return this.sponsorsField;
            }
            set
            {
                this.sponsorsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryConferencesConferenceConf_infos
    {

        private string conf_infoField;

        private byte countField;

        /// <remarks/>
        public string conf_info
        {
            get
            {
                return this.conf_infoField;
            }
            set
            {
                this.conf_infoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryConferencesConferenceConf_titles
    {

        private string conf_titleField;

        private byte countField;

        /// <remarks/>
        public string conf_title
        {
            get
            {
                return this.conf_titleField;
            }
            set
            {
                this.conf_titleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryConferencesConferenceConf_dates
    {

        private recordsRECStatic_dataSummaryConferencesConferenceConf_datesConf_date conf_dateField;

        private byte countField;

        /// <remarks/>
        public recordsRECStatic_dataSummaryConferencesConferenceConf_datesConf_date conf_date
        {
            get
            {
                return this.conf_dateField;
            }
            set
            {
                this.conf_dateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryConferencesConferenceConf_datesConf_date
    {

        private uint conf_startField;

        private uint conf_endField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint conf_start
        {
            get
            {
                return this.conf_startField;
            }
            set
            {
                this.conf_startField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint conf_end
        {
            get
            {
                return this.conf_endField;
            }
            set
            {
                this.conf_endField = value;
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryConferencesConferenceConf_locations
    {

        private recordsRECStatic_dataSummaryConferencesConferenceConf_locationsConf_location conf_locationField;

        private byte countField;

        /// <remarks/>
        public recordsRECStatic_dataSummaryConferencesConferenceConf_locationsConf_location conf_location
        {
            get
            {
                return this.conf_locationField;
            }
            set
            {
                this.conf_locationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryConferencesConferenceConf_locationsConf_location
    {

        private string conf_cityField;

        private string conf_stateField;

        /// <remarks/>
        public string conf_city
        {
            get
            {
                return this.conf_cityField;
            }
            set
            {
                this.conf_cityField = value;
            }
        }

        /// <remarks/>
        public string conf_state
        {
            get
            {
                return this.conf_stateField;
            }
            set
            {
                this.conf_stateField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryConferencesConferenceSponsors
    {

        private string[] sponsorField;

        private byte countField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sponsor")]
        public string[] sponsor
        {
            get
            {
                return this.sponsorField;
            }
            set
            {
                this.sponsorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryPublishers
    {

        private recordsRECStatic_dataSummaryPublishersPublisher publisherField;

        /// <remarks/>
        public recordsRECStatic_dataSummaryPublishersPublisher publisher
        {
            get
            {
                return this.publisherField;
            }
            set
            {
                this.publisherField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryPublishersPublisher
    {

        private recordsRECStatic_dataSummaryPublishersPublisherAddress_spec address_specField;

        private recordsRECStatic_dataSummaryPublishersPublisherNames namesField;

        /// <remarks/>
        public recordsRECStatic_dataSummaryPublishersPublisherAddress_spec address_spec
        {
            get
            {
                return this.address_specField;
            }
            set
            {
                this.address_specField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataSummaryPublishersPublisherNames names
        {
            get
            {
                return this.namesField;
            }
            set
            {
                this.namesField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryPublishersPublisherAddress_spec
    {

        private string full_addressField;

        private string cityField;

        private byte addr_noField;

        /// <remarks/>
        public string full_address
        {
            get
            {
                return this.full_addressField;
            }
            set
            {
                this.full_addressField = value;
            }
        }

        /// <remarks/>
        public string city
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte addr_no
        {
            get
            {
                return this.addr_noField;
            }
            set
            {
                this.addr_noField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryPublishersPublisherNames
    {

        private recordsRECStatic_dataSummaryPublishersPublisherNamesName nameField;

        private byte countField;

        /// <remarks/>
        public recordsRECStatic_dataSummaryPublishersPublisherNamesName name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataSummaryPublishersPublisherNamesName
    {

        private string display_nameField;

        private string full_nameField;

        private string roleField;

        private byte seq_noField;

        private byte addr_noField;

        /// <remarks/>
        public string display_name
        {
            get
            {
                return this.display_nameField;
            }
            set
            {
                this.display_nameField = value;
            }
        }

        /// <remarks/>
        public string full_name
        {
            get
            {
                return this.full_nameField;
            }
            set
            {
                this.full_nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string role
        {
            get
            {
                return this.roleField;
            }
            set
            {
                this.roleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte seq_no
        {
            get
            {
                return this.seq_noField;
            }
            set
            {
                this.seq_noField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte addr_no
        {
            get
            {
                return this.addr_noField;
            }
            set
            {
                this.addr_noField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadata
    {

        private recordsRECStatic_dataFullrecord_metadataLanguages languagesField;

        private recordsRECStatic_dataFullrecord_metadataNormalized_languages normalized_languagesField;

        private recordsRECStatic_dataFullrecord_metadataNormalized_doctypes normalized_doctypesField;

        private recordsRECStatic_dataFullrecord_metadataRefs refsField;

        private recordsRECStatic_dataFullrecord_metadataAddresses addressesField;

        private recordsRECStatic_dataFullrecord_metadataReprint_addresses reprint_addressesField;

        private recordsRECStatic_dataFullrecord_metadataCategory_info category_infoField;

        private recordsRECStatic_dataFullrecord_metadataFund_ack fund_ackField;

        private recordsRECStatic_dataFullrecord_metadataKeywords keywordsField;

        private recordsRECStatic_dataFullrecord_metadataAbstracts abstractsField;

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataLanguages languages
        {
            get
            {
                return this.languagesField;
            }
            set
            {
                this.languagesField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataNormalized_languages normalized_languages
        {
            get
            {
                return this.normalized_languagesField;
            }
            set
            {
                this.normalized_languagesField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataNormalized_doctypes normalized_doctypes
        {
            get
            {
                return this.normalized_doctypesField;
            }
            set
            {
                this.normalized_doctypesField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataRefs refs
        {
            get
            {
                return this.refsField;
            }
            set
            {
                this.refsField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataAddresses addresses
        {
            get
            {
                return this.addressesField;
            }
            set
            {
                this.addressesField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataReprint_addresses reprint_addresses
        {
            get
            {
                return this.reprint_addressesField;
            }
            set
            {
                this.reprint_addressesField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataCategory_info category_info
        {
            get
            {
                return this.category_infoField;
            }
            set
            {
                this.category_infoField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataFund_ack fund_ack
        {
            get
            {
                return this.fund_ackField;
            }
            set
            {
                this.fund_ackField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataKeywords keywords
        {
            get
            {
                return this.keywordsField;
            }
            set
            {
                this.keywordsField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataAbstracts abstracts
        {
            get
            {
                return this.abstractsField;
            }
            set
            {
                this.abstractsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataLanguages
    {

        private recordsRECStatic_dataFullrecord_metadataLanguagesLanguage languageField;

        private byte countField;

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataLanguagesLanguage language
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataLanguagesLanguage
    {

        private string typeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataNormalized_languages
    {

        private recordsRECStatic_dataFullrecord_metadataNormalized_languagesLanguage languageField;

        private byte countField;

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataNormalized_languagesLanguage language
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataNormalized_languagesLanguage
    {

        private string typeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataNormalized_doctypes
    {

        private string[] doctypeField;

        private byte countField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("doctype")]
        public string[] doctype
        {
            get
            {
                return this.doctypeField;
            }
            set
            {
                this.doctypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataRefs
    {

        private byte countField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataAddresses
    {

        private recordsRECStatic_dataFullrecord_metadataAddressesAddress_name[] address_nameField;

        private byte countField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("address_name")]
        public recordsRECStatic_dataFullrecord_metadataAddressesAddress_name[] address_name
        {
            get
            {
                return this.address_nameField;
            }
            set
            {
                this.address_nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataAddressesAddress_name
    {

        private recordsRECStatic_dataFullrecord_metadataAddressesAddress_nameAddress_spec address_specField;

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataAddressesAddress_nameAddress_spec address_spec
        {
            get
            {
                return this.address_specField;
            }
            set
            {
                this.address_specField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataAddressesAddress_nameAddress_spec
    {

        private string full_addressField;

        private recordsRECStatic_dataFullrecord_metadataAddressesAddress_nameAddress_specOrganizations organizationsField;

        private string streetField;

        private recordsRECStatic_dataFullrecord_metadataAddressesAddress_nameAddress_specSuborganizations suborganizationsField;

        private string cityField;

        private string stateField;

        private string countryField;

        private recordsRECStatic_dataFullrecord_metadataAddressesAddress_nameAddress_specZip zipField;

        private byte addr_noField;

        /// <remarks/>
        public string full_address
        {
            get
            {
                return this.full_addressField;
            }
            set
            {
                this.full_addressField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataAddressesAddress_nameAddress_specOrganizations organizations
        {
            get
            {
                return this.organizationsField;
            }
            set
            {
                this.organizationsField = value;
            }
        }

        /// <remarks/>
        public string street
        {
            get
            {
                return this.streetField;
            }
            set
            {
                this.streetField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataAddressesAddress_nameAddress_specSuborganizations suborganizations
        {
            get
            {
                return this.suborganizationsField;
            }
            set
            {
                this.suborganizationsField = value;
            }
        }

        /// <remarks/>
        public string city
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        public string state
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        public string country
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
        public recordsRECStatic_dataFullrecord_metadataAddressesAddress_nameAddress_specZip zip
        {
            get
            {
                return this.zipField;
            }
            set
            {
                this.zipField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte addr_no
        {
            get
            {
                return this.addr_noField;
            }
            set
            {
                this.addr_noField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataAddressesAddress_nameAddress_specOrganizations
    {

        private recordsRECStatic_dataFullrecord_metadataAddressesAddress_nameAddress_specOrganizationsOrganization[] organizationField;

        private byte countField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("organization")]
        public recordsRECStatic_dataFullrecord_metadataAddressesAddress_nameAddress_specOrganizationsOrganization[] organization
        {
            get
            {
                return this.organizationField;
            }
            set
            {
                this.organizationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataAddressesAddress_nameAddress_specOrganizationsOrganization
    {

        private string prefField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string pref
        {
            get
            {
                return this.prefField;
            }
            set
            {
                this.prefField = value;
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataAddressesAddress_nameAddress_specSuborganizations
    {

        private string[] suborganizationField;

        private byte countField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("suborganization")]
        public string[] suborganization
        {
            get
            {
                return this.suborganizationField;
            }
            set
            {
                this.suborganizationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataAddressesAddress_nameAddress_specZip
    {

        private string locationField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataReprint_addresses
    {

        private recordsRECStatic_dataFullrecord_metadataReprint_addressesAddress_name address_nameField;

        private byte countField;

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataReprint_addressesAddress_name address_name
        {
            get
            {
                return this.address_nameField;
            }
            set
            {
                this.address_nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataReprint_addressesAddress_name
    {

        private recordsRECStatic_dataFullrecord_metadataReprint_addressesAddress_nameAddress_spec address_specField;

        private recordsRECStatic_dataFullrecord_metadataReprint_addressesAddress_nameNames namesField;

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataReprint_addressesAddress_nameAddress_spec address_spec
        {
            get
            {
                return this.address_specField;
            }
            set
            {
                this.address_specField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataReprint_addressesAddress_nameNames names
        {
            get
            {
                return this.namesField;
            }
            set
            {
                this.namesField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataReprint_addressesAddress_nameAddress_spec
    {

        private object[] itemsField;

        private ItemsChoiceType[] itemsElementNameField;

        private byte addr_noField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("city", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("country", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("full_address", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("organizations", typeof(recordsRECStatic_dataFullrecord_metadataReprint_addressesAddress_nameAddress_specOrganizations))]
        [System.Xml.Serialization.XmlElementAttribute("street", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("suborganizations", typeof(recordsRECStatic_dataFullrecord_metadataReprint_addressesAddress_nameAddress_specSuborganizations))]
        [System.Xml.Serialization.XmlElementAttribute("zip", typeof(recordsRECStatic_dataFullrecord_metadataReprint_addressesAddress_nameAddress_specZip))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte addr_no
        {
            get
            {
                return this.addr_noField;
            }
            set
            {
                this.addr_noField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataReprint_addressesAddress_nameAddress_specOrganizations
    {

        private recordsRECStatic_dataFullrecord_metadataReprint_addressesAddress_nameAddress_specOrganizationsOrganization[] organizationField;

        private byte countField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("organization")]
        public recordsRECStatic_dataFullrecord_metadataReprint_addressesAddress_nameAddress_specOrganizationsOrganization[] organization
        {
            get
            {
                return this.organizationField;
            }
            set
            {
                this.organizationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataReprint_addressesAddress_nameAddress_specOrganizationsOrganization
    {

        private string prefField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string pref
        {
            get
            {
                return this.prefField;
            }
            set
            {
                this.prefField = value;
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataReprint_addressesAddress_nameAddress_specSuborganizations
    {

        private string[] suborganizationField;

        private byte countField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("suborganization")]
        public string[] suborganization
        {
            get
            {
                return this.suborganizationField;
            }
            set
            {
                this.suborganizationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataReprint_addressesAddress_nameAddress_specZip
    {

        private string locationField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord", IncludeInSchema = false)]
    public enum ItemsChoiceType
    {

        /// <remarks/>
        city,

        /// <remarks/>
        country,

        /// <remarks/>
        full_address,

        /// <remarks/>
        organizations,

        /// <remarks/>
        street,

        /// <remarks/>
        suborganizations,

        /// <remarks/>
        zip,
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataReprint_addressesAddress_nameNames
    {

        private recordsRECStatic_dataFullrecord_metadataReprint_addressesAddress_nameNamesName nameField;

        private byte countField;

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataReprint_addressesAddress_nameNamesName name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataReprint_addressesAddress_nameNamesName
    {

        private string display_nameField;

        private string full_nameField;

        private string wos_standardField;

        private string first_nameField;

        private string last_nameField;

        private string suffixField;

        private byte seq_noField;

        private string roleField;

        private string reprintField;

        private byte addr_noField;

        private bool addr_noFieldSpecified;

        private string displayField;

        /// <remarks/>
        public string display_name
        {
            get
            {
                return this.display_nameField;
            }
            set
            {
                this.display_nameField = value;
            }
        }

        /// <remarks/>
        public string full_name
        {
            get
            {
                return this.full_nameField;
            }
            set
            {
                this.full_nameField = value;
            }
        }

        /// <remarks/>
        public string wos_standard
        {
            get
            {
                return this.wos_standardField;
            }
            set
            {
                this.wos_standardField = value;
            }
        }

        /// <remarks/>
        public string first_name
        {
            get
            {
                return this.first_nameField;
            }
            set
            {
                this.first_nameField = value;
            }
        }

        /// <remarks/>
        public string last_name
        {
            get
            {
                return this.last_nameField;
            }
            set
            {
                this.last_nameField = value;
            }
        }

        /// <remarks/>
        public string suffix
        {
            get
            {
                return this.suffixField;
            }
            set
            {
                this.suffixField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte seq_no
        {
            get
            {
                return this.seq_noField;
            }
            set
            {
                this.seq_noField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string role
        {
            get
            {
                return this.roleField;
            }
            set
            {
                this.roleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string reprint
        {
            get
            {
                return this.reprintField;
            }
            set
            {
                this.reprintField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte addr_no
        {
            get
            {
                return this.addr_noField;
            }
            set
            {
                this.addr_noField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool addr_noSpecified
        {
            get
            {
                return this.addr_noFieldSpecified;
            }
            set
            {
                this.addr_noFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string display
        {
            get
            {
                return this.displayField;
            }
            set
            {
                this.displayField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataCategory_info
    {

        private recordsRECStatic_dataFullrecord_metadataCategory_infoHeadings headingsField;

        private recordsRECStatic_dataFullrecord_metadataCategory_infoSubheadings subheadingsField;

        private recordsRECStatic_dataFullrecord_metadataCategory_infoSubjects subjectsField;

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataCategory_infoHeadings headings
        {
            get
            {
                return this.headingsField;
            }
            set
            {
                this.headingsField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataCategory_infoSubheadings subheadings
        {
            get
            {
                return this.subheadingsField;
            }
            set
            {
                this.subheadingsField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataCategory_infoSubjects subjects
        {
            get
            {
                return this.subjectsField;
            }
            set
            {
                this.subjectsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataCategory_infoHeadings
    {

        private string headingField;

        private byte countField;

        /// <remarks/>
        public string heading
        {
            get
            {
                return this.headingField;
            }
            set
            {
                this.headingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataCategory_infoSubheadings
    {

        private string[] subheadingField;

        private byte countField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("subheading")]
        public string[] subheading
        {
            get
            {
                return this.subheadingField;
            }
            set
            {
                this.subheadingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataCategory_infoSubjects
    {

        private recordsRECStatic_dataFullrecord_metadataCategory_infoSubjectsSubject[] subjectField;

        private byte countField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("subject")]
        public recordsRECStatic_dataFullrecord_metadataCategory_infoSubjectsSubject[] subject
        {
            get
            {
                return this.subjectField;
            }
            set
            {
                this.subjectField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataCategory_infoSubjectsSubject
    {

        private string ascatypeField;

        private string codeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ascatype
        {
            get
            {
                return this.ascatypeField;
            }
            set
            {
                this.ascatypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataFund_ack
    {

        private recordsRECStatic_dataFullrecord_metadataFund_ackGrants grantsField;

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataFund_ackGrants grants
        {
            get
            {
                return this.grantsField;
            }
            set
            {
                this.grantsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataFund_ackGrants
    {

        private recordsRECStatic_dataFullrecord_metadataFund_ackGrantsGrant[] grantField;

        private byte countField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("grant")]
        public recordsRECStatic_dataFullrecord_metadataFund_ackGrantsGrant[] grant
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
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataFund_ackGrantsGrant
    {

        private string grant_agencyField;

        private recordsRECStatic_dataFullrecord_metadataFund_ackGrantsGrantGrant_ids grant_idsField;

        private string grant_sourceField;

        /// <remarks/>
        public string grant_agency
        {
            get
            {
                return this.grant_agencyField;
            }
            set
            {
                this.grant_agencyField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataFund_ackGrantsGrantGrant_ids grant_ids
        {
            get
            {
                return this.grant_idsField;
            }
            set
            {
                this.grant_idsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string grant_source
        {
            get
            {
                return this.grant_sourceField;
            }
            set
            {
                this.grant_sourceField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataFund_ackGrantsGrantGrant_ids
    {

        private string[] grant_idField;

        private ushort countField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("grant_id")]
        public string[] grant_id
        {
            get
            {
                return this.grant_idField;
            }
            set
            {
                this.grant_idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataKeywords
    {

        private string[] keywordField;

        private byte countField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("keyword")]
        public string[] keyword
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
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataAbstracts
    {

        private recordsRECStatic_dataFullrecord_metadataAbstractsAbstract abstractField;

        private byte countField;

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataAbstractsAbstract @abstract
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataAbstractsAbstract
    {

        private recordsRECStatic_dataFullrecord_metadataAbstractsAbstractAbstract_text abstract_textField;

        /// <remarks/>
        public recordsRECStatic_dataFullrecord_metadataAbstractsAbstractAbstract_text abstract_text
        {
            get
            {
                return this.abstract_textField;
            }
            set
            {
                this.abstract_textField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataFullrecord_metadataAbstractsAbstractAbstract_text
    {

        private string[] pField;

        private byte countField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("p")]
        public string[] p
        {
            get
            {
                return this.pField;
            }
            set
            {
                this.pField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataItem
    {

        private recordsRECStatic_dataItemIds idsField;

        private string bib_idField;

        private recordsRECStatic_dataItemBib_pagecount bib_pagecountField;

        private recordsRECStatic_dataItemKeywords_plus keywords_plusField;

        private string coll_idField;

        /// <remarks/>
        public recordsRECStatic_dataItemIds ids
        {
            get
            {
                return this.idsField;
            }
            set
            {
                this.idsField = value;
            }
        }

        /// <remarks/>
        public string bib_id
        {
            get
            {
                return this.bib_idField;
            }
            set
            {
                this.bib_idField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataItemBib_pagecount bib_pagecount
        {
            get
            {
                return this.bib_pagecountField;
            }
            set
            {
                this.bib_pagecountField = value;
            }
        }

        /// <remarks/>
        public recordsRECStatic_dataItemKeywords_plus keywords_plus
        {
            get
            {
                return this.keywords_plusField;
            }
            set
            {
                this.keywords_plusField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string coll_id
        {
            get
            {
                return this.coll_idField;
            }
            set
            {
                this.coll_idField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataItemIds
    {

        private string availField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string avail
        {
            get
            {
                return this.availField;
            }
            set
            {
                this.availField = value;
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataItemBib_pagecount
    {

        private string typeField;

        private ushort valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public ushort Value
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataItemKeywords_plus
    {

        private string[] keywordField;

        private byte countField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("keyword")]
        public string[] keyword
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
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataContributors
    {

        private recordsRECStatic_dataContributorsContributor[] contributorField;

        private byte countField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("contributor")]
        public recordsRECStatic_dataContributorsContributor[] contributor
        {
            get
            {
                return this.contributorField;
            }
            set
            {
                this.contributorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataContributorsContributor
    {

        private recordsRECStatic_dataContributorsContributorName nameField;

        /// <remarks/>
        public recordsRECStatic_dataContributorsContributorName name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_dataContributorsContributorName
    {

        private string display_nameField;

        private string full_nameField;

        private string first_nameField;

        private string last_nameField;

        private byte seq_noField;

        private string roleField;

        private string r_idField;

        private string orcid_idField;

        /// <remarks/>
        public string display_name
        {
            get
            {
                return this.display_nameField;
            }
            set
            {
                this.display_nameField = value;
            }
        }

        /// <remarks/>
        public string full_name
        {
            get
            {
                return this.full_nameField;
            }
            set
            {
                this.full_nameField = value;
            }
        }

        /// <remarks/>
        public string first_name
        {
            get
            {
                return this.first_nameField;
            }
            set
            {
                this.first_nameField = value;
            }
        }

        /// <remarks/>
        public string last_name
        {
            get
            {
                return this.last_nameField;
            }
            set
            {
                this.last_nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte seq_no
        {
            get
            {
                return this.seq_noField;
            }
            set
            {
                this.seq_noField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string role
        {
            get
            {
                return this.roleField;
            }
            set
            {
                this.roleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string r_id
        {
            get
            {
                return this.r_idField;
            }
            set
            {
                this.r_idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string orcid_id
        {
            get
            {
                return this.orcid_idField;
            }
            set
            {
                this.orcid_idField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECDynamic_data
    {

        private recordsRECDynamic_dataCitation_related citation_relatedField;

        private recordsRECDynamic_dataCluster_related cluster_relatedField;

        /// <remarks/>
        public recordsRECDynamic_dataCitation_related citation_related
        {
            get
            {
                return this.citation_relatedField;
            }
            set
            {
                this.citation_relatedField = value;
            }
        }

        /// <remarks/>
        public recordsRECDynamic_dataCluster_related cluster_related
        {
            get
            {
                return this.cluster_relatedField;
            }
            set
            {
                this.cluster_relatedField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECDynamic_dataCitation_related
    {

        private recordsRECDynamic_dataCitation_relatedTc_list tc_listField;

        /// <remarks/>
        public recordsRECDynamic_dataCitation_relatedTc_list tc_list
        {
            get
            {
                return this.tc_listField;
            }
            set
            {
                this.tc_listField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECDynamic_dataCitation_relatedTc_list
    {

        private recordsRECDynamic_dataCitation_relatedTc_listSilo_tc silo_tcField;

        /// <remarks/>
        public recordsRECDynamic_dataCitation_relatedTc_listSilo_tc silo_tc
        {
            get
            {
                return this.silo_tcField;
            }
            set
            {
                this.silo_tcField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECDynamic_dataCitation_relatedTc_listSilo_tc
    {

        private string coll_idField;

        private byte local_countField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string coll_id
        {
            get
            {
                return this.coll_idField;
            }
            set
            {
                this.coll_idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte local_count
        {
            get
            {
                return this.local_countField;
            }
            set
            {
                this.local_countField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECDynamic_dataCluster_related
    {

        private recordsRECDynamic_dataCluster_relatedIdentifier[] identifiersField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("identifier", IsNullable = false)]
        public recordsRECDynamic_dataCluster_relatedIdentifier[] identifiers
        {
            get
            {
                return this.identifiersField;
            }
            set
            {
                this.identifiersField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECDynamic_dataCluster_relatedIdentifier
    {

        private string typeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord", IsNullable = false)]
    public partial class records
    {

        private recordsREC[] rECField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("REC")]
        public recordsREC[] REC
        {
            get
            {
                return this.rECField;
            }
            set
            {
                this.rECField = value;
            }
        }
    }
}
