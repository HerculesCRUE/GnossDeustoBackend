// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace API_DISCOVER.Models.Entities.ExternalAPIs
{
    /// <summary>
    /// Clase para interactuar con el API de Recolecta
    /// </summary>
    public class RECOLECTA_API : I_ExternalAPI
    {
        public string Name { get { return "Recolecta"; } }
        public string Description { get { return "RECOLECTA, o Recolector de Ciencia Abierta, es el agregador nacional de repositorios de acceso abierto. En esta plataforma se agrupan a todas las infraestructuras digitales españolas en las que se publican y/o depositan resultados de investigación en acceso abierto. "; } }

        public string HomePage { get { return "https://recolecta.fecyt.es/"; } }

        public string Id { get { return "recolecta"; } }

        /// <summary>
        /// Busca trabajos en el API de Recolecta
        /// </summary>
        /// <param name="q">Texto a buscar (con urlEncode)</param>
        /// <returns>Objeto con los trabajos</returns>
        public static List<RecolectaDocument> Works(string q)
        {
            string url = "https://buscador.recolecta.fecyt.es/buscador-recolecta?search_api_fulltext=" + q;
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(url);
            var nodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='views-row']");
            List<RecolectaDocument> docList = new List<RecolectaDocument>();
            if (nodes != null)
            {
                foreach (var item in nodes)
                {
                    RecolectaDocument doc = new RecolectaDocument();
                    doc.authorList = new Dictionary<string, string>();
                    doc.linkList = new List<string>();
                    doc.title = item.SelectSingleNode(".//h4").InnerText;
                    if (item.SelectNodes(".//div[@class='text-danger']//ul") != null)
                    {
                        var authorNode = item.SelectNodes(".//div[@class='text-danger']//ul").First();
                        if (authorNode.ChildNodes != null)
                        {
                            foreach (var node in authorNode.ChildNodes)
                            {
                                string[] authorData = node.InnerText.Split("|||");
                                if (!doc.authorList.ContainsKey(authorData[0]))
                                {
                                    if (authorData.Length > 1)
                                    {
                                        doc.authorList.Add(authorData[0], authorData[1]);
                                    }
                                    else
                                    {
                                        doc.authorList.Add(node.InnerText, "");
                                    }
                                }
                            }
                        }
                    }
                    string[] links = item.SelectSingleNode(".//div[@class='identifier pull-right']//a").InnerText.Split(',');
                    if (links != null)
                    {
                        foreach (string link in links)
                        {
                            doc.linkList.Add(link);
                        }
                    }
                    docList.Add(doc);
                }
            }
            return docList;
        }
    }


    public class RecolectaDocument
    {
        public string title { get; set; }
        public Dictionary<string, string> authorList { get; set; }
        public List<string> linkList { get; set; }
    }

}
