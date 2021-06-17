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
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get { return "Web of Science"; } }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get { return "Los datos del Grafo de Conocimiento de la investigación que contiene Hércules ASIO provienen del Sistema de Gestión de la Investigación de la Universidad de Murcia."; } }
        /// <summary>
        /// HomePage
        /// </summary>
        public string HomePage { get { return "http://wos.fecyt.es/"; } }
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get { return "wos"; } }
        private static DateTime dateActive = DateTime.UtcNow;

        /// <summary>
        /// Genera la cooie para las peticiones al API de WOS
        /// </summary>
        /// <param name="authorization"></param>
        private static void ActualizarCookie(string authorization)
        {
            if(dateActive>DateTime.UtcNow)
            {
                return;
            }
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
                catch (Exception)
                {
                    if (i == 1)
                    {
                        throw;
                    }
                    dateActive= DateTime.UtcNow.AddMinutes(5);
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
            if (dateActive > DateTime.UtcNow)
            {
                return null;
            }
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
                    response = response.Replace("&lt;", "<").Replace("&gt;", ">");
                    XDocument xdoc = XDocument.Parse(response);
                    foreach (var node in xdoc.Descendants().Where(e => e.Attribute("{http://www.w3.org/2001/XMLSchema-instance}type") != null))
                    {
                        node.Attribute("{http://www.w3.org/2001/XMLSchema-instance}type").Remove();
                    }
                    XmlSerializer serializer = new XmlSerializer(typeof(WOSWorks));
                    WOSWorks result = serializer.Deserialize(xdoc.CreateReader()) as WOSWorks;
                    return result;
                }catch(Exception)
                {
                    if (i == 1)
                    {
                        throw;
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
            throw new ArgumentNullException("Ha fallado la llamada al API 'http://search.webofknowledge.com/esti/wokmws/ws/WokSearch' ");
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
}

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://scientific.thomsonreuters.com/schema/wok5.4/public/FullRecord")]
    public partial class recordsRECStatic_data
    {

        private recordsRECStatic_dataSummary summaryField;

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
        private recordsRECStatic_dataSummaryTitles titlesField;

        private recordsRECStatic_dataSummaryNames namesField;

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
}
