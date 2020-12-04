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
    public class WOS_API : I_ExternalAPI
    {
        /// <summary>
        /// Cookie para las peticiones
        /// </summary>
        private static string _cookie { get; set; }
        //TODO cambiar
        public string Name { get { return "Web of Science"; } }

        public string HomePage { get { return "http://wos.fecyt.es/"; } }

        public string Id { get { return "wos"; } }

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
                    Thread.Sleep(1000);
                    ActualizarCookie(authorization);
                    Thread.Sleep(1000);
                }
                finally
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
        private returnRecords recordsField;
        
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

        private recordsRECStatic_dataSummaryTitles titlesField;

        private recordsRECStatic_dataSummaryNames namesField;

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
    public partial class recordsRECStatic_dataSummaryTitles
    {

        private recordsRECStatic_dataSummaryTitlesTitle[] titleField;

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

        private string roleField;

        private string reprintField;

        private ulong daisng_idField;

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
        public ulong daisng_id
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
    public partial class recordsRECStatic_dataItem
    {

        private recordsRECStatic_dataItemIds idsField;

        private string bib_idField;

        private recordsRECStatic_dataItemBib_pagecount bib_pagecountField;

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
    public partial class recordsRECStatic_dataContributors
    {

        private recordsRECStatic_dataContributorsContributor[] contributorField;

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
        private recordsRECDynamic_dataCluster_related cluster_relatedField;

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
