using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlToTriples
{
    class Program
    {
        static void Main(string[] args)
        {
            ToXml();
        }

        public static List<Dictionary<string,string>> LeerXml(string rutaXml)
        {
            List<Dictionary<string, string>> datosXmlList = new List<Dictionary<string, string>>();
            XmlTextReader reader = new XmlTextReader(rutaXml);
            Dictionary<string, string> xmlDataRecord = new Dictionary<string, string>();
            string attribute = string.Empty;
            string value = string.Empty;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        if (reader.Name != "DATA_RECORD")
                        {
                            attribute = reader.Name;
                        }
                        break;
                    case XmlNodeType.Text: //Display the text in each element.
                        value = reader.Value;
                        xmlDataRecord.Add(attribute,value);
                        break;
                    case XmlNodeType.EndElement: //Display the end of the element.
                        if (reader.Name == "DATA_RECORD")
                        {
                            datosXmlList.Add(xmlDataRecord);
                            xmlDataRecord = null;
                            xmlDataRecord = new Dictionary<string, string>();
                        }
                        break;
                }
            }
            return datosXmlList;
        }

        public static void ToXml()
        {
            #region PropiedadesXml
            List<Dictionary<string, string>> datosProyecto = LeerXml("E:\\Cargas\\Proyectos\\Hercules\\Proyectos\\Proyectos.xml");
            List<Dictionary<string, string>> datosFinanciacion = LeerXml("E:\\Cargas\\Proyectos\\Hercules\\Proyectos\\Financiacion proyectos.xml");
            List<Dictionary<string, string>> datosGastos = LeerXml("E:\\Cargas\\Proyectos\\Hercules\\Proyectos\\Gastos proyectos.xml");
            List<Dictionary<string, string>> datosFecha = LeerXml("E:\\Cargas\\Proyectos\\Hercules\\Proyectos\\Fechas proyectos.xml");
            List<Dictionary<string, string>> datosContract = LeerXml("E:\\Cargas\\Proyectos\\Hercules\\Proyectos\\Relaciones origenes proyectos nuevo.xml");
            List<Dictionary<string, string>> datosEquipos = LeerXml("E:\\Cargas\\Proyectos\\Hercules\\Proyectos\\Equipos proyectos nuevo.xml");
            List<Dictionary<string, string>> datosPersona = LeerXml("E:\\Cargas\\Proyectos\\Hercules\\Personas.xml");
            #endregion

            string dirProyecto = $"E:\\Cargas\\Proyectos\\Hercules\\datosProyecto.txt";
            var ficheroProyecto = File.CreateText(dirProyecto);
            string dirContract = $"E:\\Cargas\\Proyectos\\Hercules\\datosPersona.txt";
            var ficheroInvestigador = File.CreateText(dirContract);
            StringBuilder sb = new StringBuilder();
            StringBuilder sbInvestigador = new StringBuilder();
            string url = "http://herc-as-front-desa.atica.um.es/uris/Factory?";
            string pResource_class = string.Empty;
            string pIdentifier = string.Empty;
            string uriProyecto = string.Empty;
            string uriFundedBy = string.Empty;
            string uriFundingAmount = string.Empty;
            string uriSpends = string.Empty;
            string uriDateTimeInterval = string.Empty;
            string uriInvestigador = string.Empty;
            string uriInvestigadorRole = string.Empty;
            string uriContract = string.Empty;
            string sProyecto = string.Empty;
            string sfundedBy = string.Empty;
            string sFundingAmount = string.Empty;
            string spends = string.Empty;
            string dateTimeInterval = string.Empty;
            string sInvestigadorRole = string.Empty;
            string sInvestigador = string.Empty;
            string sContract = string.Empty;
            Dictionary<string, string> investigador;

            foreach (Dictionary<string, string> proyData in datosProyecto)
            {
                pResource_class = "Project";
                pIdentifier = proyData["IDPROYECTO"];
                string fechaInicio = string.Empty;
                string fechaFin = string.Empty;
                bool tieneDateTimeInterval = false;
                if (string.IsNullOrEmpty(uriProyecto)) {
                    WebRequest urlProyecto;
                    urlProyecto = WebRequest.Create(url + "resource_class=" + pResource_class + "&identifier=" + pIdentifier);
                    Stream streamProyecto;
                    streamProyecto = urlProyecto.GetResponse().GetResponseStream();
                    StreamReader readerProyecto = new StreamReader(streamProyecto);
                    sProyecto = readerProyecto.ReadLine();
                    uriProyecto = sProyecto.Substring(0,sProyecto.LastIndexOf('/')+1);
                }
                else
                {
                    sProyecto = uriProyecto + pIdentifier;
                }
                int aux = 1;
                sb.AppendLine($"<{sProyecto}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://vivoweb.org/ontology/core#Project> .");
                sb.AppendLine($"<{sProyecto}> <http://vivoweb.org/ontology/core#identifier> \"{proyData["IDPROYECTO"].Replace("\"","\\\"")}\" .");
                sb.AppendLine($"<{sProyecto}> <http://purl.org/roh#title> \"{proyData["NOMBRE"].Replace("\"", "\\\"")}\" .");
                if (proyData.ContainsKey("PROYECTOFINALISTA")) {
                    switch (proyData["PROYECTOFINALISTA"])
                    {
                        case "S":
                            sb.AppendLine($"<{sProyecto}> <http://purl.org/rohes#finalist> \"Finalista\" .");
                            break;
                        case "F":
                            sb.AppendLine($"<{sProyecto}> <http://purl.org/rohes#finalist> \"Finalista\" .");
                            break;
                        case "P":
                            sb.AppendLine($"<{sProyecto}> <http://purl.org/rohes#finalist> \"Parcialmente Finalista\" .");
                            break;
                        case "N":
                            sb.AppendLine($"<{sProyecto}> <http://purl.org/rohes#finalist> \"No Finalista\" .");
                            break;
                        default:
                            break;
                    }
                }
                if (proyData.ContainsKey("LIMITATIVO"))
                {
                    sb.AppendLine($"<{sProyecto}> <http://purl.org/rohes#limitative> {(proyData["LIMITATIVO"] == "S")} .");
                }
                #region Gastos
                foreach (Dictionary<string, string> gastosData in datosGastos)
                {
                    if (gastosData["IDPROYECTO"] == proyData["IDPROYECTO"] && !tieneDateTimeInterval)
                    {
                        pResource_class = "ProjectExpense";
                        pIdentifier = proyData["IDPROYECTO"] + "_" + aux;
                        if (string.IsNullOrEmpty(uriSpends))
                        {
                            WebRequest urlProjectExpense;
                            urlProjectExpense = WebRequest.Create(url + "resource_class=" + pResource_class + "&identifier=" + pIdentifier);
                            Stream streamExepnses;
                            streamExepnses = urlProjectExpense.GetResponse().GetResponseStream();
                            StreamReader readerExepnses = new StreamReader(streamExepnses);
                            spends = readerExepnses.ReadLine();
                            uriSpends = spends.Substring(0, spends.LastIndexOf('/') + 1);
                        }
                        else
                        {
                            spends = uriSpends + pIdentifier;
                        }
                        aux++;
                        sb.AppendLine($"<{sProyecto}> <http://purl.org/roh#spends>	<{spends}>.");
                        sb.AppendLine($"<{spends}> <http://purl.org/roh#monetaryAmount> {gastosData["IMPORTE"].Replace(',', '.')} .");
                        sb.AppendLine($"<{spends}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://purl.org/roh#ProjectExpense> .");
                        sb.AppendLine($"<{spends}> <http://purl.org/roh#currency> \"{gastosData["CODTIPOMONEDA"].Replace("\"", "\\\"")}\" .");
                        sb.AppendLine($"<{spends}> <http://purl.org/roh#title> \"{gastosData["DESCRIPCION"].Replace("\"", "\\\"")}\" .");
                        tieneDateTimeInterval = true;
                    }
                }
                #endregion
                #region Fecha
                foreach (Dictionary<string, string> fechaData in datosFecha)
                {
                    if (fechaData["IDPROYECTO"] == proyData["IDPROYECTO"])
                    {
                        pResource_class = "DateTimeInterval";
                        pIdentifier = proyData["IDPROYECTO"] + "_" + aux;
                        if (string.IsNullOrEmpty(uriDateTimeInterval))
                        {
                            WebRequest urlDateTime;
                            urlDateTime = WebRequest.Create(url + "resource_class=" + pResource_class + "&identifier=" + pIdentifier);
                            Stream streamDateTime;
                            streamDateTime = urlDateTime.GetResponse().GetResponseStream();
                            StreamReader readerDateTime = new StreamReader(streamDateTime);
                            dateTimeInterval = readerDateTime.ReadLine();
                            uriDateTimeInterval = dateTimeInterval.Substring(0, dateTimeInterval.LastIndexOf('/') + 1);
                        }
                        else
                        {
                            dateTimeInterval = uriDateTimeInterval + pIdentifier;
                        }
                        aux++;
                        sb.AppendLine($"<{sProyecto}> <http://vivoweb.org/ontology/core#dateTimeInterval> <{dateTimeInterval}>.");
                        sb.AppendLine($"<{dateTimeInterval}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://vivoweb.org/ontology/core#DateTimeInterval> .");
                        if (fechaData.ContainsKey("FECHAINICIOPROYECTO") && !string.IsNullOrEmpty(fechaData["FECHAINICIOPROYECTO"]))
                        {
                            DateTime dateInicio = DateTime.Parse(fechaData["FECHAINICIOPROYECTO"]);
                            fechaInicio = dateInicio.ToString("yyyy-MM-ddTHH:mm:ss");
                            sb.AppendLine($"<{dateTimeInterval}> <http://vivoweb.org/ontology/core#start> \"{fechaInicio}\"^^<http://www.w3.org/2001/XMLSchema#dateTime> .");
                        }
                        if (fechaData.ContainsKey("FECHAFINPROYECTO") && !string.IsNullOrEmpty(fechaData["FECHAFINPROYECTO"]))
                        {
                            DateTime dateFin = DateTime.Parse(fechaData["FECHAFINPROYECTO"]);
                            fechaFin = dateFin.ToString("yyyy-MM-ddTHH:mm:ss");
                            sb.AppendLine($"<{dateTimeInterval}> <http://vivoweb.org/ontology/core#end> \"{fechaFin}\"^^<http://www.w3.org/2001/XMLSchema#dateTime> .");
                        }
                    }
                }
                #endregion
                #region Financiacion
                foreach (Dictionary<string, string> finaData in datosFinanciacion)
                {
                    if (finaData["IDPROYECTO"] == proyData["IDPROYECTO"])
                    {
                        pResource_class = "Funding";
                        pIdentifier = proyData["IDPROYECTO"] + "_" + aux;
                        if (string.IsNullOrEmpty(uriFundedBy))
                        {
                            WebRequest urlFunding;
                            urlFunding = WebRequest.Create(url + "resource_class=" + pResource_class + "&identifier=" + pIdentifier);
                            Stream streamFunding;
                            streamFunding = urlFunding.GetResponse().GetResponseStream();
                            StreamReader readerFunding = new StreamReader(streamFunding);
                            sfundedBy = readerFunding.ReadLine();
                            uriFundedBy = sfundedBy.Substring(0, sfundedBy.LastIndexOf('/') + 1);
                        }
                        else
                        {
                            sfundedBy = uriFundedBy + pIdentifier;
                        }
                        aux++;
                        pResource_class = "FundingAmount";
                        pIdentifier = proyData["IDPROYECTO"] + "_" + aux;
                        if (string.IsNullOrEmpty(uriFundingAmount))
                        {
                            WebRequest urlFundingAmount;
                            urlFundingAmount = WebRequest.Create(url + "resource_class=" + pResource_class + "&identifier=" + pIdentifier);
                            Stream streamFundingAmount;
                            streamFundingAmount = urlFundingAmount.GetResponse().GetResponseStream();
                            StreamReader readerFundingAmount = new StreamReader(streamFundingAmount);
                            sFundingAmount = readerFundingAmount.ReadLine();
                            uriFundingAmount = sFundingAmount.Substring(0, sFundingAmount.LastIndexOf('/') + 1);
                        }
                        else
                        {
                            sFundingAmount = uriFundingAmount + pIdentifier;
                        }
                        aux++;
                        sb.AppendLine($"<{sProyecto}> <http://purl.org/roh#fundedBy> <{sfundedBy}> .");
                        switch (finaData["CODTIPOFINANCIACION"])
                        {
                            case "AR":
                                sb.AppendLine($"<{sfundedBy}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://purl.org/roh#RefundableAdvance> .");
                                break;
                            case "SUBV":
                                sb.AppendLine($"<{sfundedBy}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://purl.org/roh#Grant> .");
                                break;
                            case "PRES":
                                sb.AppendLine($"<{sfundedBy}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://purl.org/roh#Loan> .");
                                break;
                            default:
                                break;
                        }
                        sb.AppendLine($"<{sfundedBy}> <http://purl.obolibrary.org/obo/BFO_0000051>	<{sFundingAmount}> .");
                        if (finaData.ContainsKey("NUMEROANUALIDAD") && !string.IsNullOrEmpty(finaData["NUMEROANUALIDAD"]))
                        {
                            int numeroAunualidad;
                            int.TryParse(finaData["NUMEROANUALIDAD"], out numeroAunualidad);
                            if (string.IsNullOrEmpty(uriDateTimeInterval))
                            {
                                pResource_class = "DateTimeInterval";
                                WebRequest urlDateTime;
                                urlDateTime = WebRequest.Create(url + "resource_class=" + pResource_class + "&identifier=" + pIdentifier);
                                Stream streamDateTime;
                                streamDateTime = urlDateTime.GetResponse().GetResponseStream();
                                StreamReader readerDateTime = new StreamReader(streamDateTime);
                                dateTimeInterval = readerDateTime.ReadLine();
                                uriDateTimeInterval = dateTimeInterval.Substring(0, dateTimeInterval.LastIndexOf('/') + 1);
                            }
                            else
                            {
                                dateTimeInterval = uriDateTimeInterval + pIdentifier;
                            }
                            aux++;
                            sb.AppendLine($"<{sfundedBy}> <http://vivoweb.org/ontology/core#dateTimeInterval> <{dateTimeInterval}>.");
                            sb.AppendLine($"<{dateTimeInterval}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://vivoweb.org/ontology/core#DateTimeInterval> .");
                            if (!string.IsNullOrEmpty(fechaInicio))
                            {
                                DateTime dateInicio = DateTime.Parse(fechaInicio);
                                dateInicio.AddYears(numeroAunualidad - 1);
                                fechaInicio = dateInicio.ToString("yyyy-MM-ddTHH:mm:ss");
                                sb.AppendLine($"<{dateTimeInterval}> <http://vivoweb.org/ontology/core#start> \"{fechaInicio}\"^^<http://www.w3.org/2001/XMLSchema#dateTime> .");

                                DateTime dateFin = new DateTime(dateInicio.Year, 12, 31);
                                fechaInicio = dateFin.ToString("yyyy-MM-ddTHH:mm:ss");
                                sb.AppendLine($"<{dateTimeInterval}> <http://vivoweb.org/ontology/core#end> \"{fechaFin}\"^^<http://www.w3.org/2001/XMLSchema#dateTime> .");
                            }
                        }
                        sb.AppendLine($"<{sFundingAmount}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://purl.org/roh#FundingAmount> .");
                        sb.AppendLine($"<{sFundingAmount}> <http://purl.org/roh#monetaryAmount> {finaData["IMPORTE"].Replace(',', '.')} .");
                        sb.AppendLine($"<{sFundingAmount}> <http://purl.org/roh#currency> \"{finaData["CODTIPOMONEDA"]}\" .");
                    }
                }
                #endregion
                #region Investigador
                pResource_class = "InvestigatorRole";
                pIdentifier = proyData["IDPROYECTO"] + "_" + aux;
                if (string.IsNullOrEmpty(uriInvestigadorRole))
                {
                    WebRequest urlResearcher;
                    urlResearcher = WebRequest.Create(url + "resource_class=" + pResource_class + "&identifier=" + pIdentifier);
                    Stream streamResearcher;
                    streamResearcher = urlResearcher.GetResponse().GetResponseStream();
                    StreamReader readerResearcher = new StreamReader(streamResearcher);
                    sInvestigadorRole = readerResearcher.ReadLine();
                    uriInvestigadorRole = sInvestigadorRole.Substring(0, sInvestigadorRole.LastIndexOf('/') + 1);
                }
                else
                {
                    sInvestigadorRole = uriInvestigadorRole + pIdentifier;
                }
                aux++;
                sb.AppendLine($"<{sProyecto}> <http://vivoweb.org/ontology/core#relates> <{sInvestigadorRole}> .");
                sb.AppendLine($"<{sInvestigadorRole}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#Type> <http://vivoweb.org/ontology/core#InvestigatorRole> .");
                foreach (Dictionary<string, string> equipoData in datosEquipos)
                {
                    if (equipoData["IDPROYECTO"] == proyData["IDPROYECTO"])
                    {
                        investigador = datosPersona.FirstOrDefault(x => x["IDPERSONA"] == equipoData["IDPERSONA"]);
                        pResource_class = "Researcher";
                        pIdentifier = investigador["IDPERSONA"];
                        if (string.IsNullOrEmpty(uriInvestigador))
                        {
                            WebRequest urlResearcher;
                            urlResearcher = WebRequest.Create(url + "resource_class=" + pResource_class + "&identifier=" + pIdentifier);
                            Stream streamResearcher;
                            streamResearcher = urlResearcher.GetResponse().GetResponseStream();
                            StreamReader readerResearcher = new StreamReader(streamResearcher);
                            sInvestigador = readerResearcher.ReadLine();
                            uriInvestigador = sInvestigador.Substring(0, sInvestigador.LastIndexOf('/') + 1);
                        }
                        else
                        {
                            sInvestigador = uriInvestigador + pIdentifier;
                        }
                        sb.AppendLine($"<{sInvestigadorRole}> <http://purl.org/roh#roleOf> <{sInvestigador}> .");
                        sbInvestigador.AppendLine($"<{sInvestigador}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#Type>	<http://purl.org/roh#Researcher> .");
                        sbInvestigador.AppendLine($"<{sInvestigador}> <http://vivoweb.org/ontology/core#researcherId> \"{pIdentifier}\" .");
                        sbInvestigador.AppendLine($"<{sInvestigador}> <http://xmlns.com/foaf/spec/#name> \"Investigador {pIdentifier}\" .");
                    }
                }
                #endregion
                #region Contrato
                foreach (Dictionary<string, string> contractData in datosContract)
                {
                    if (contractData["IDPROYECTO"] == proyData["IDPROYECTO"])
                    {
                        pResource_class = "ProjectContract";
                        pIdentifier = contractData["IDORIGENPROYECTO"];
                        if (string.IsNullOrEmpty(uriContract))
                        {
                            WebRequest urlContract;
                            urlContract = WebRequest.Create(url + "resource_class=" + pResource_class + "&identifier=" + pIdentifier);
                            Stream streamContract;
                            streamContract = urlContract.GetResponse().GetResponseStream();
                            StreamReader readerContract = new StreamReader(streamContract);
                            sContract = readerContract.ReadLine();
                            uriContract = sContract.Substring(0, sContract.LastIndexOf('/') + 1);
                        }
                        else
                        {
                            sContract = uriContract + pIdentifier;
                        }
                        sb.AppendLine($"<{sProyecto}> <http://purl.org/roh#hasContract> <{sContract}> .");
                    }
                } 
                #endregion
            }

            ficheroInvestigador.Write(sbInvestigador.ToString());
            ficheroInvestigador.Close();
            ficheroProyecto.Write(sb.ToString());
            ficheroProyecto.Close();
        }
    }
}