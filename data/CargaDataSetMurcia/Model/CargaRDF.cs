using CargaDataSetMurcia.Model.XML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Update;
using VDS.RDF.Writing;

namespace CargaDataSetMurcia.Model
{
    public static class CargaRDF
    {
        /// <summary>
        /// Genera los ficheros RDF para realizar la carga
        /// </summary>
        /// <param name="pUrlUrisFactory">Url del servicio UrisFactory</param>
        public static void GenerarRDF(string pUrlUrisFactory)
        {
            string inputFolder = "Dataset";
            string outputFolder = "RDF";

            //Leemos los XML            
            List<Persona> personas = LeerPersonas(inputFolder + "/Personas.xml");
            List<Articulo> articulos = LeerArticulos(inputFolder + "/Articulos.xml");
            List<AutorArticulo> autoresArticulos = LeerAutoresArticulos(inputFolder + "/Autores articulos.xml");
            List<AutorCongreso> autoresCongresos = LeerAutoresCongresos(inputFolder + "/Autores congresos.xml");
            List<AutorExposicion> autoresExposiciones = LeerAutoresExposiciones(inputFolder + "/Autores exposiciones.xml");
            List<Centro> centros = LeerCentros(inputFolder + "/Centros.xml");
            List<Congreso> congresos = LeerCongresos(inputFolder + "/Congresos.xml");
            List<Departamento> departamentos = LeerDepartamentos(inputFolder + "/Departamentos.xml");
            List<EquipoProyecto> equiposProyectos = LeerEquiposProyectos(inputFolder + "/Equipos proyectos.xml");
            List<Exposicion> exposiciones = LeerExposiciones(inputFolder + "/Exposiciones.xml");
            List<FechaEquipoProyecto> fechasEquiposProyectos = LeerFechasEquiposProyectos(inputFolder + "/Fechas equipos proyectos.xml");
            List<FechaProyecto> fechasProyectos = LeerFechasProyectos(inputFolder + "/Fechas proyectos.xml");
            List<Proyecto> proyectos = LeerProyectos(inputFolder + "/Proyectos.xml");
            List<TipoParticipacionGrupo> tipoParticipacionGrupos = LeerTipoParticipacionGrupos(inputFolder + "/Tipo participacion grupo.xml");
            List<DatoEquipoInvestigacion> datoEquiposInvestigacion = LeerDatoEquiposInvestigacion(inputFolder + "/Datos equipo investigacion.xml");
            List<GrupoInvestigacion> gruposInvestigacion = LeerGruposInvestigacion(inputFolder + "/Grupos de investigacion.xml");

            List<Feature> features = LeerFeatures();

            //Generamos los RDFs
            if (System.IO.Directory.Exists(outputFolder))
            {
                System.IO.Directory.Delete(outputFolder, true);
            }
            System.IO.Directory.CreateDirectory(outputFolder);
            GenerarAmbitosProyectos(outputFolder + "/Ambito/", features);
            GenerarPersonas(outputFolder + "/Personas/", pUrlUrisFactory, personas, centros, departamentos, datoEquiposInvestigacion, tipoParticipacionGrupos, gruposInvestigacion);
            GenerarArticulos(outputFolder + "/Articulos/", pUrlUrisFactory, articulos, personas, autoresArticulos);
            GenerarOrganizaciones(outputFolder + "/Organizaciones/", pUrlUrisFactory, centros, departamentos, gruposInvestigacion);
            GenerarProyectos(outputFolder + "/Proyectos/", pUrlUrisFactory, proyectos, fechasProyectos, equiposProyectos, fechasEquiposProyectos, personas,features);
            GenerarActividades(outputFolder + "/Actividades/", pUrlUrisFactory, congresos, exposiciones, autoresCongresos, autoresExposiciones, personas);
            
        }

        public static void PublicarRDF(string pUriUrisFactory, List<SparqlConfig> pSparqlASIO, string pSparqlASIO_Graph, string pSparqlASIO_Domain, List<SparqlConfig> pSparqlUnidata, string pSparqlUnidata_Graph, string pSparqlUnidata_Domain)
        {
            string sparqlASIO_Graph_temp = pSparqlASIO_Graph + "_temporal";
            string sparqlUnidata_Graph_temp = pSparqlUnidata_Graph + "_temporal";

            #region Eliminamos los grafos temporales
            foreach (SparqlConfig sparqlConfig in pSparqlASIO)
            {
                SparqlUtility.SelectData(sparqlConfig.endpoint, sparqlASIO_Graph_temp, $"DROP SILENT GRAPH <{sparqlASIO_Graph_temp }>", sparqlConfig.username, sparqlConfig.pass);
            }
            foreach (SparqlConfig sparqlConfig in pSparqlUnidata)
            {

                SparqlUtility.SelectData(sparqlConfig.endpoint, sparqlUnidata_Graph_temp, $"DROP SILENT GRAPH <{sparqlUnidata_Graph_temp }>", sparqlConfig.username, sparqlConfig.pass);
            }
            #endregion

            #region Creamos grafos temporales
            foreach (SparqlConfig sparqlConfig in pSparqlASIO)
            {
                SparqlUtility.SelectData(sparqlConfig.endpoint, sparqlASIO_Graph_temp, $"CREATE GRAPH <{sparqlASIO_Graph_temp }>", sparqlConfig.username, sparqlConfig.pass);
            }
            foreach (SparqlConfig sparqlConfig in pSparqlUnidata)
            {
                SparqlUtility.SelectData(sparqlConfig.endpoint, sparqlUnidata_Graph_temp, $"CREATE GRAPH <{sparqlUnidata_Graph_temp }>", sparqlConfig.username, sparqlConfig.pass);
            }
            #endregion

            #region Cargamos los triples generales
            {
                //ASIO
                Graph asioGeneralGraph = new Graph();
                {
                    INode s = asioGeneralGraph.CreateUriNode(UriFactory.Create(pSparqlASIO_Graph));
                    INode p = asioGeneralGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
                    INode o = asioGeneralGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#Graph"));
                    asioGeneralGraph.Assert(new Triple(s, p, o));
                }
                {
                    INode s = asioGeneralGraph.CreateUriNode(UriFactory.Create(pSparqlASIO_Graph));
                    INode p = asioGeneralGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/ns/prov#wasAttributedTo"));
                    INode o = GetUri(pUriUrisFactory, asioGeneralGraph, "http://purl.org/roh/mirror/vivo#University", "um");
                    asioGeneralGraph.Assert(new Triple(s, p, o));
                }
                {
                    INode s = GetUri(pUriUrisFactory, asioGeneralGraph, "http://purl.org/roh/mirror/vivo#University", "um");
                    INode p = asioGeneralGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
                    INode o = asioGeneralGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/vivo#University"));
                    asioGeneralGraph.Assert(new Triple(s, p, o));
                }
                {
                    INode s = GetUri(pUriUrisFactory, asioGeneralGraph, "http://purl.org/roh/mirror/vivo#University", "um");
                    INode p = asioGeneralGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#name"));
                    INode o = asioGeneralGraph.CreateLiteralNode("Universidad de Murcia");
                    asioGeneralGraph.Assert(new Triple(s, p, o));
                }
                {
                    INode s = GetUri(pUriUrisFactory, asioGeneralGraph, "http://purl.org/roh/mirror/vivo#University", "um");
                    INode p = asioGeneralGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#homePage"));
                    INode o = asioGeneralGraph.CreateLiteralNode("https://www.um.es/");
                    asioGeneralGraph.Assert(new Triple(s, p, o));
                }
                CargarGrafo(asioGeneralGraph, pSparqlASIO, pSparqlASIO_Graph, pSparqlASIO_Domain, null, null, null, pUriUrisFactory);

            }
            //Si existe Unidata creamos los triples generales
            if (pSparqlUnidata.Count > 0)
            {
                //Unidata
                Graph unidataGeneralGraph = new Graph();
                {
                    INode s = unidataGeneralGraph.CreateUriNode(UriFactory.Create(pSparqlUnidata_Graph));
                    INode p = unidataGeneralGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
                    INode o = unidataGeneralGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#Graph"));
                    unidataGeneralGraph.Assert(new Triple(s, p, o));
                }
                string uriOrgUnidata = GetUri(pUriUrisFactory, unidataGeneralGraph, "http://purl.org/roh/mirror/foaf#Organization", "ASIO Unidata").ToString().Replace(pSparqlASIO_Domain, pSparqlUnidata_Domain);
                {
                    INode s = unidataGeneralGraph.CreateUriNode(UriFactory.Create(pSparqlUnidata_Graph));
                    INode p = unidataGeneralGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/ns/prov#wasAttributedTo"));
                    INode o = unidataGeneralGraph.CreateUriNode(UriFactory.Create(uriOrgUnidata));
                    unidataGeneralGraph.Assert(new Triple(s, p, o));
                }
                {

                    INode s = unidataGeneralGraph.CreateUriNode(UriFactory.Create(uriOrgUnidata));
                    INode p = unidataGeneralGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
                    INode o = unidataGeneralGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#Organization"));
                    unidataGeneralGraph.Assert(new Triple(s, p, o));
                }
                {
                    INode s = unidataGeneralGraph.CreateUriNode(UriFactory.Create(uriOrgUnidata));
                    INode p = unidataGeneralGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#name"));
                    INode o = unidataGeneralGraph.CreateLiteralNode("ASIO Unidata");
                    unidataGeneralGraph.Assert(new Triple(s, p, o));
                }

                string uriOrgMurcia = GetUri(pUriUrisFactory, unidataGeneralGraph, "http://purl.org/roh/mirror/vivo#University", "um").ToString().Replace(pSparqlASIO_Domain, pSparqlUnidata_Domain);
                {

                    INode s = unidataGeneralGraph.CreateUriNode(UriFactory.Create(uriOrgMurcia));
                    INode p = unidataGeneralGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
                    INode o = unidataGeneralGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/vivo#University"));
                    unidataGeneralGraph.Assert(new Triple(s, p, o));
                }
                {
                    INode s = unidataGeneralGraph.CreateUriNode(UriFactory.Create(uriOrgMurcia));
                    INode p = unidataGeneralGraph.CreateUriNode(UriFactory.Create("http://purl.org/roh/mirror/foaf#name"));
                    INode o = unidataGeneralGraph.CreateLiteralNode("Universidad de Murcia");
                    unidataGeneralGraph.Assert(new Triple(s, p, o));
                }


                CargarGrafo(unidataGeneralGraph, null, null, pSparqlASIO_Domain, pSparqlUnidata, pSparqlUnidata_Graph, pSparqlUnidata_Domain, pUriUrisFactory);
            }

            #endregion

            #region Cargamos los RDF
            int numTotal = 0;
            foreach (string directory in Directory.GetDirectories("RDF/"))
            {
                numTotal += Directory.GetFiles(directory).Count();
            }
            Console.Write($"PublicandoRDFs 0/{numTotal}");
            Graph grafoCarga = new Graph();
            int numEntities = 0;
            int numProcesados = 0;
            foreach (string directory in Directory.GetDirectories("RDF/"))
            {
                foreach (string file in Directory.GetFiles(directory))
                {
                    numEntities++;
                    numProcesados++;
                    Graph g = new Graph();
                    g.LoadFromFile(file);

                    HashSet<string> asioSubjects = new HashSet<string>();
                    {
                        SparqlResultSet sparqlResultSet = (SparqlResultSet)g.ExecuteQuery("select distinct ?s where{?s ?p ?o. FILTER(isURI(?s)) MINUS {?s2 ?p2 ?s }}");
                        foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
                        {
                            string s = sparqlResult["s"].ToString();
                            asioSubjects.Add(s);
                        }
                    }
                    //Modificamos el grafo para añadir la fecha de actualización
                    foreach (string id in asioSubjects)
                    {
                        INode sSameAs = g.CreateUriNode(UriFactory.Create(id));
                        INode pSameAs = g.CreateUriNode(UriFactory.Create("http://www.w3.org/ns/prov#endedAtTime"));
                        INode oSameAs = g.CreateLiteralNode(DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzzzz"), UriFactory.Create("http://www.w3.org/2001/XMLSchema#datetime"));
                        g.Assert(new Triple(sSameAs, pSameAs, oSameAs));
                    }

                    grafoCarga.Merge(g);
                    if (numEntities >= 100)
                    {
                        CargarGrafo(grafoCarga, pSparqlASIO, pSparqlASIO_Graph, pSparqlASIO_Domain, pSparqlUnidata, pSparqlUnidata_Graph, pSparqlUnidata_Domain, pUriUrisFactory);
                        numEntities = 0;
                        grafoCarga = new Graph();
                        Console.Write($"\rPublicandoRDFs {numProcesados}/{numTotal}");
                    }

                }
            }
            CargarGrafo(grafoCarga, pSparqlASIO, pSparqlASIO_Graph, pSparqlASIO_Domain, pSparqlUnidata, pSparqlUnidata_Graph, pSparqlUnidata_Domain, pUriUrisFactory);
            Console.WriteLine($"\rPublicandoRDFs {numProcesados}/{numTotal}");
            #endregion

            //Movemos los grafos temporales
            foreach (SparqlConfig sparqlConfig in pSparqlASIO)
            {
                SparqlUtility.SelectData(sparqlConfig.endpoint, sparqlASIO_Graph_temp, $"MOVE <{sparqlASIO_Graph_temp }> TO <{pSparqlASIO_Graph }>", sparqlConfig.username, sparqlConfig.pass);
            }
            foreach (SparqlConfig sparqlConfig in pSparqlUnidata)
            {

                SparqlUtility.SelectData(sparqlConfig.endpoint, sparqlUnidata_Graph_temp, $"MOVE <{sparqlUnidata_Graph_temp }> TO <{pSparqlUnidata_Graph }>", sparqlConfig.username, sparqlConfig.pass);
            }
        }

        private static void CargarGrafo(Graph pGraph, List<SparqlConfig> pSparqlASIO, string pSparqlASIO_Graph, string pSparqlASIO_Domain, List<SparqlConfig> pSparqlUnidata, string pSparqlUnidata_Graph, string pSparqlUnidata_Domain, string pUriUrisFactory)
        {
            //Obtenemos sujetos del grafo a cargar en ASIO
            HashSet<string> asioSubjects = new HashSet<string>();
            {
                SparqlResultSet sparqlResultSet = (SparqlResultSet)pGraph.ExecuteQuery("select distinct ?s where{?s ?p ?o. FILTER(isURI(?s))}");
                foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
                {
                    string s = sparqlResult["s"].ToString();
                    asioSubjects.Add(s);
                }
            }

            //Obtenemos objetos del grafo a cargar en ASIO
            HashSet<string> asioObjects = new HashSet<string>();
            {
                SparqlResultSet sparqlResultSet = (SparqlResultSet)pGraph.ExecuteQuery("select distinct ?o where{?s ?p ?o. FILTER(isURI(?o))}");
                foreach (SparqlResult sparqlResult in sparqlResultSet.Results)
                {
                    string o = sparqlResult["o"].ToString();
                    if (o.StartsWith(pSparqlASIO_Domain))
                    {
                        asioObjects.Add(o);
                    }
                }
            }

            //Preparamos el grafo de ASIO
            Graph asioGRaph = CloneGraph(pGraph);

            //Si existe Unidata y ASIO creamos los sameAs
            if (pSparqlUnidata != null && pSparqlUnidata.Count > 0 && pSparqlASIO != null && pSparqlASIO.Count > 0)
            {
                //Insertamos en todas las entidades cuyo sujeto tenga 'domainASIO' un sameAs hacia la misma URL pero con el dominio 'domainUnidata'
                foreach (string uri in asioSubjects)
                {
                    IUriNode sSameAs = asioGRaph.CreateUriNode(UriFactory.Create(uri));
                    IUriNode pSameAs = asioGRaph.CreateUriNode(UriFactory.Create("http://www.w3.org/2002/07/owl#sameAs"));
                    IUriNode oSameAs = asioGRaph.CreateUriNode(UriFactory.Create(pSparqlUnidata_Domain + uri.Substring(pSparqlASIO_Domain.Length)));
                    asioGRaph.Assert(new Triple(sSameAs, pSameAs, oSameAs));
                }
            }


            //Insertamos en el triplestore de ASIO
            if (pSparqlASIO != null)
            {
                foreach (SparqlConfig sparqlConfig in pSparqlASIO)
                {
                    SparqlUtility.LoadTriples(SparqlUtility.GetTriplesFromGraph(asioGRaph), sparqlConfig.endpoint, pSparqlASIO_Graph + "_temporal", sparqlConfig.username, sparqlConfig.pass);
                }
            }

            //Si existe Unidata hacemos la carga en Unidata
            if (pSparqlUnidata != null && pSparqlUnidata.Count > 0)
            {
                //Creamos el grafo de unidata
                Graph unidataGraph = CloneGraph(pGraph);
                //Modificamos todas las uris de sujetos u objetos que contengan 'domainASIO' por la misma url pero cambiando el dominio por 'domainUnidata'
                TripleStore store = new TripleStore();
                store.Add(unidataGraph);
                LeviathanUpdateProcessor processor = new LeviathanUpdateProcessor(store);
                foreach (string sujeto in asioSubjects)
                {
                    if (sujeto.StartsWith(pSparqlASIO_Domain))
                    {
                        SparqlUpdateParser parser = new SparqlUpdateParser();
                        SparqlUpdateCommandSet updateSubjects = parser.ParseFromString(@$"  DELETE {{ ?s ?p ?o. }}
                                                                                    INSERT {{ ?newS ?p ?o. }}
                                                                                    WHERE 
                                                                                    {{
                                                                                        ?s ?p ?o.   
                                                                                        FILTER(?s =<{sujeto}>)
                                                                                        BIND(<{pSparqlUnidata_Domain + sujeto.Substring(pSparqlASIO_Domain.Length)}> as ?newS)
                                                                                    }}");
                        processor.ProcessCommandSet(updateSubjects);
                    }
                }
                foreach (string objeto in asioObjects)
                {
                    if (objeto.StartsWith(pSparqlASIO_Domain))
                    {
                        SparqlUpdateParser parser = new SparqlUpdateParser();
                        //Creamos SameAs los sujetos
                        SparqlUpdateCommandSet createSameAs = parser.ParseFromString(@$"    DELETE {{ ?s ?p ?o. }}
                                                                                    INSERT {{ ?s ?p ?newO. }}
                                                                                    WHERE 
                                                                                    {{
                                                                                        ?s ?p ?o.   
                                                                                        FILTER(?o =<{objeto}>)
                                                                                        BIND(<{pSparqlUnidata_Domain + objeto.Substring(pSparqlASIO_Domain.Length)}> as ?newO)
                                                                                    }}");
                        processor.ProcessCommandSet(createSameAs);
                    }
                }
                //Insertamos los sameAs
                if (pSparqlUnidata != null && pSparqlUnidata.Count > 0 && pSparqlASIO != null && pSparqlASIO.Count > 0)
                {
                    foreach (string uri in asioSubjects)
                    {
                        IUriNode sSameAs = unidataGraph.CreateUriNode(UriFactory.Create(pSparqlUnidata_Domain + uri.Substring(pSparqlASIO_Domain.Length)));
                        IUriNode pSameAs = unidataGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/2002/07/owl#sameAs"));
                        IUriNode oSameAs = unidataGraph.CreateUriNode(UriFactory.Create(uri));
                        unidataGraph.Assert(new Triple(sSameAs, pSameAs, oSameAs));
                    }
                }

                foreach (string uri in asioSubjects)
                {
                    IUriNode sProv = unidataGraph.CreateUriNode(UriFactory.Create(pSparqlUnidata_Domain + uri.Substring(pSparqlASIO_Domain.Length)));
                    IUriNode pProv = unidataGraph.CreateUriNode(UriFactory.Create("http://www.w3.org/ns/prov#wasAttributedTo"));
                    string uriOrgMurcia = GetUri(pUriUrisFactory, unidataGraph, "http://purl.org/roh/mirror/vivo#University", "um").ToString().Replace(pSparqlASIO_Domain, pSparqlUnidata_Domain);
                    IUriNode oProv = unidataGraph.CreateUriNode(UriFactory.Create(uriOrgMurcia));
                    unidataGraph.Assert(new Triple(sProv, pProv, oProv));
                }

                //Insertamos en el triplestore de Unidata
                foreach (SparqlConfig sparqlConfig in pSparqlUnidata)
                {
                    SparqlUtility.LoadTriples(SparqlUtility.GetTriplesFromGraph(unidataGraph), sparqlConfig.endpoint, pSparqlUnidata_Graph + "_temporal", sparqlConfig.username, sparqlConfig.pass);
                }
            }
        }


        #region Lectura de XMLs
        private static List<Persona> LeerPersonas(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<Persona> elementos = new List<Persona>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                Persona elemento = new Persona();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "IDPERSONA":
                            elemento.IDPERSONA = node.SelectSingleNode("IDPERSONA").InnerText;
                            break;
                        case "NOMBRE":
                            elemento.NOMBRE = node.SelectSingleNode("NOMBRE").InnerText;
                            break;
                        case "PERS_CENT_CODIGO":
                            elemento.PERS_CENT_CODIGO = node.SelectSingleNode("PERS_CENT_CODIGO").InnerText;
                            break;
                        case "CED_NOMBRE":
                            elemento.CED_NOMBRE = node.SelectSingleNode("CED_NOMBRE").InnerText;
                            break;
                        case "PERS_DEPT_CODIGO":
                            elemento.PERS_DEPT_CODIGO = node.SelectSingleNode("PERS_DEPT_CODIGO").InnerText;
                            break;
                        case "DEP_NOMBRE":
                            elemento.DEP_NOMBRE = node.SelectSingleNode("DEP_NOMBRE").InnerText;
                            break;
                        case "SEXO":
                            elemento.SEXO = node.SelectSingleNode("SEXO").InnerText;
                            break;
                        case "PERSONAL_ACTIVO":
                            elemento.PERSONAL_ACTIVO = node.SelectSingleNode("PERSONAL_ACTIVO").InnerText;
                            break;
                        case "PERSONAL_UMU":
                            elemento.SEXO = node.SelectSingleNode("PERSONAL_UMU").InnerText;
                            break;
                        case "EMAIL":
                            elemento.SEXO = node.SelectSingleNode("EMAIL").InnerText;
                            break;
                        default:
                            throw new Exception("Propiedad no controlada");
                    }
                }
                elementos.Add(elemento);
            }
            Console.WriteLine($"\rLeídos {elementos.Count} elementos de {pFile}");
            return elementos;
        }

        private static List<Articulo> LeerArticulos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<Articulo> elementos = new List<Articulo>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                Articulo elemento = new Articulo();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "CODIGO":
                            elemento.CODIGO = node.SelectSingleNode("CODIGO").InnerText;
                            break;
                        case "TITULO":
                            elemento.TITULO = node.SelectSingleNode("TITULO").InnerText;
                            break;
                        case "ANO":
                            elemento.ANO = node.SelectSingleNode("ANO").InnerText;
                            break;
                        case "PAIS_CODIGO":
                            elemento.PAIS_CODIGO = node.SelectSingleNode("PAIS_CODIGO").InnerText;
                            break;
                        case "REIS_ISSN":
                            elemento.REIS_ISSN = node.SelectSingleNode("REIS_ISSN").InnerText;
                            break;
                        case "CATALOGO":
                            elemento.CATALOGO = node.SelectSingleNode("CATALOGO").InnerText;
                            break;
                        case "AREA":
                            elemento.AREA = node.SelectSingleNode("AREA").InnerText;
                            break;
                        case "NOMBRE_REVISTA":
                            elemento.NOMBRE_REVISTA = node.SelectSingleNode("NOMBRE_REVISTA").InnerText;
                            break;
                        case "CUARTIL_REVISTA":
                            elemento.CUARTIL_REVISTA = node.SelectSingleNode("CUARTIL_REVISTA").InnerText;
                            break;
                        case "URL_REVISTA":
                            elemento.URL_REVISTA = node.SelectSingleNode("URL_REVISTA").InnerText;
                            break;
                        case "VOLUMEN":
                            elemento.VOLUMEN = node.SelectSingleNode("VOLUMEN").InnerText;
                            break;
                        case "NUMERO":
                            elemento.NUMERO = node.SelectSingleNode("NUMERO").InnerText;
                            break;
                        case "PAGDESDE":
                            elemento.PAGDESDE = node.SelectSingleNode("PAGDESDE").InnerText;
                            break;
                        case "PAGHASTA":
                            elemento.PAGHASTA = node.SelectSingleNode("PAGHASTA").InnerText;
                            break;
                        case "NUMPAG":
                            elemento.NUMPAG = node.SelectSingleNode("NUMPAG").InnerText;
                            break;
                        case "COAUT_EXT":
                            elemento.COAUT_EXT = node.SelectSingleNode("COAUT_EXT").InnerText;
                            break;
                        case "ARTI_DOI":
                            elemento.ARTI_DOI = node.SelectSingleNode("ARTI_DOI").InnerText;
                            break;
                        default:
                            throw new Exception("Propiedad no controlada");
                    }
                }
                elementos.Add(elemento);
            }
            Console.WriteLine($"\rLeídos {elementos.Count} elementos de {pFile}");
            return elementos;
        }

        private static List<AutorArticulo> LeerAutoresArticulos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<AutorArticulo> elementos = new List<AutorArticulo>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                AutorArticulo elemento = new AutorArticulo();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "ARTI_CODIGO":
                            elemento.ARTI_CODIGO = node.SelectSingleNode("ARTI_CODIGO").InnerText;
                            break;
                        case "IDPERSONA":
                            elemento.IDPERSONA = node.SelectSingleNode("IDPERSONA").InnerText;
                            break;
                        case "ORDEN":
                            elemento.ORDEN = node.SelectSingleNode("ORDEN").InnerText;
                            break;
                        default:
                            throw new Exception("Propiedad no controlada");
                    }
                }
                elementos.Add(elemento);
            }
            Console.WriteLine($"\rLeídos {elementos.Count} elementos de {pFile}");
            return elementos;
        }

        private static List<AutorCongreso> LeerAutoresCongresos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<AutorCongreso> elementos = new List<AutorCongreso>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                AutorCongreso elemento = new AutorCongreso();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "CONG_NUMERO":
                            elemento.CONG_NUMERO = node.SelectSingleNode("CONG_NUMERO").InnerText;
                            break;
                        case "IDPERSONA":
                            elemento.IDPERSONA = node.SelectSingleNode("IDPERSONA").InnerText;
                            break;
                        case "ORDEN":
                            elemento.ORDEN = node.SelectSingleNode("ORDEN").InnerText;
                            break;
                        default:
                            throw new Exception("Propiedad no controlada");
                    }
                }
                elementos.Add(elemento);
            }
            Console.WriteLine($"\rLeídos {elementos.Count} elementos de {pFile}");
            return elementos;
        }

        private static List<AutorExposicion> LeerAutoresExposiciones(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<AutorExposicion> elementos = new List<AutorExposicion>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                AutorExposicion elemento = new AutorExposicion();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "EXPO_CODIGO":
                            elemento.EXPO_CODIGO = node.SelectSingleNode("EXPO_CODIGO").InnerText;
                            break;
                        case "IDPERSONA":
                            elemento.IDPERSONA = node.SelectSingleNode("IDPERSONA").InnerText;
                            break;
                        case "ORDEN":
                            elemento.ORDEN = node.SelectSingleNode("ORDEN").InnerText;
                            break;
                        default:
                            throw new Exception("Propiedad no controlada");
                    }
                }
                elementos.Add(elemento);
            }
            Console.WriteLine($"\rLeídos {elementos.Count} elementos de {pFile}");
            return elementos;
        }

        private static List<Centro> LeerCentros(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<Centro> elementos = new List<Centro>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                Centro elemento = new Centro();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "CED_CODIGO":
                            elemento.CED_CODIGO = node.SelectSingleNode("CED_CODIGO").InnerText;
                            break;
                        case "CED_NOMBRE":
                            elemento.CED_NOMBRE = node.SelectSingleNode("CED_NOMBRE").InnerText;
                            break;
                        default:
                            throw new Exception("Propiedad no controlada");
                    }
                }
                elementos.Add(elemento);
            }
            Console.WriteLine($"\rLeídos {elementos.Count} elementos de {pFile}");
            return elementos;
        }

        private static List<Congreso> LeerCongresos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<Congreso> elementos = new List<Congreso>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                Congreso elemento = new Congreso();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "CONG_NUMERO":
                            elemento.CONG_NUMERO = node.SelectSingleNode("CONG_NUMERO").InnerText;
                            break;
                        case "TIPO_PARTICIPACION":
                            elemento.TIPO_PARTICIPACION = node.SelectSingleNode("TIPO_PARTICIPACION").InnerText;
                            break;
                        case "TITULO_CONTRIBUCION":
                            elemento.TITULO_CONTRIBUCION = node.SelectSingleNode("TITULO_CONTRIBUCION").InnerText;
                            break;
                        case "TITULO_CONGRESO":
                            elemento.TITULO_CONGRESO = node.SelectSingleNode("TITULO_CONGRESO").InnerText;
                            break;
                        case "FECHA_CELEBRACION":
                            elemento.FECHA_CELEBRACION = node.SelectSingleNode("FECHA_CELEBRACION").InnerText;
                            break;
                        case "LUGAR_CELEBRACION":
                            elemento.LUGAR_CELEBRACION = node.SelectSingleNode("LUGAR_CELEBRACION").InnerText;
                            break;
                        case "CONGRESO_INTERNACIONAL":
                            elemento.CONGRESO_INTERNACIONAL = node.SelectSingleNode("CONGRESO_INTERNACIONAL").InnerText;
                            break;
                        default:
                            throw new Exception("Propiedad no controlada");
                    }
                }
                elementos.Add(elemento);
            }
            Console.WriteLine($"\rLeídos {elementos.Count} elementos de {pFile}");
            return elementos;
        }

        private static List<Departamento> LeerDepartamentos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<Departamento> elementos = new List<Departamento>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                Departamento elemento = new Departamento();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "DEP_CODIGO":
                            elemento.DEP_CODIGO = node.SelectSingleNode("DEP_CODIGO").InnerText;
                            break;
                        case "DEP_NOMBRE":
                            elemento.DEP_NOMBRE = node.SelectSingleNode("DEP_NOMBRE").InnerText;
                            break;
                        case "DEP_CED_CODIGO":
                            elemento.DEP_CED_CODIGO = node.SelectSingleNode("DEP_CED_CODIGO").InnerText;
                            break;
                        default:
                            throw new Exception("Propiedad no controlada");
                    }
                }
                elementos.Add(elemento);
            }
            Console.WriteLine($"\rLeídos {elementos.Count} elementos de {pFile}");
            return elementos;
        }

        private static List<EquipoProyecto> LeerEquiposProyectos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<EquipoProyecto> elementos = new List<EquipoProyecto>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                EquipoProyecto elemento = new EquipoProyecto();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "IDPROYECTO":
                            elemento.IDPROYECTO = node.SelectSingleNode("IDPROYECTO").InnerText;
                            break;
                        case "NUMEROCOLABORADOR":
                            elemento.NUMEROCOLABORADOR = node.SelectSingleNode("NUMEROCOLABORADOR").InnerText;
                            break;
                        case "IDPERSONA":
                            elemento.IDPERSONA = node.SelectSingleNode("IDPERSONA").InnerText;
                            break;
                        case "CODTITULACION":
                            elemento.CODTITULACION = node.SelectSingleNode("CODTITULACION").InnerText;
                            break;
                        case "FECHAINICIO":
                            elemento.FECHAINICIO = node.SelectSingleNode("FECHAINICIO").InnerText;
                            break;
                        case "FECHAFIN":
                            elemento.FECHAFIN = node.SelectSingleNode("FECHAFIN").InnerText;
                            break;
                        default:
                            throw new Exception("Propiedad no controlada");
                    }
                }
                elementos.Add(elemento);
            }
            Console.WriteLine($"\rLeídos {elementos.Count} elementos de {pFile}");
            return elementos;
        }

        private static List<Exposicion> LeerExposiciones(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<Exposicion> elementos = new List<Exposicion>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                Exposicion elemento = new Exposicion();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "CODIGO":
                            elemento.CODIGO = node.SelectSingleNode("CODIGO").InnerText;
                            break;
                        case "NOMBRE":
                            elemento.NOMBRE = node.SelectSingleNode("NOMBRE").InnerText;
                            break;
                        case "FECHA":
                            elemento.FECHA = node.SelectSingleNode("FECHA").InnerText;
                            break;
                        case "LUGAR":
                            elemento.LUGAR = node.SelectSingleNode("LUGAR").InnerText;
                            break;
                        case "TIPO":
                            elemento.TIPO = node.SelectSingleNode("TIPO").InnerText;
                            break;
                        case "CALIDADES":
                            elemento.CALIDADES = node.SelectSingleNode("CALIDADES").InnerText;
                            break;
                        case "INCORPORAR_A_CVN":
                            elemento.INCORPORAR_A_CVN = node.SelectSingleNode("INCORPORAR_A_CVN").InnerText;
                            break;
                        default:
                            throw new Exception("Propiedad no controlada");
                    }
                }
                elementos.Add(elemento);
            }
            Console.WriteLine($"\rLeídos {elementos.Count} elementos de {pFile}");
            return elementos;
        }

        private static List<FechaEquipoProyecto> LeerFechasEquiposProyectos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<FechaEquipoProyecto> elementos = new List<FechaEquipoProyecto>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                FechaEquipoProyecto elemento = new FechaEquipoProyecto();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "IDPROYECTO":
                            elemento.IDPROYECTO = node.SelectSingleNode("IDPROYECTO").InnerText;
                            break;
                        case "NUMEROCOLABORADOR":
                            elemento.NUMEROCOLABORADOR = node.SelectSingleNode("NUMEROCOLABORADOR").InnerText;
                            break;
                        case "NUMERO":
                            elemento.NUMERO = node.SelectSingleNode("NUMERO").InnerText;
                            break;
                        case "CODTIPOPARTICIPACION":
                            elemento.CODTIPOPARTICIPACION = node.SelectSingleNode("CODTIPOPARTICIPACION").InnerText;
                            break;
                        case "HORASDEDICADAS":
                            elemento.HORASDEDICADAS = node.SelectSingleNode("HORASDEDICADAS").InnerText;
                            break;
                        case "CODTIPOMOTIVOCAMBIOFECHA":
                            //elemento.CODTIPOMOTIVOCAMBIOFECHA = node.SelectSingleNode("CODTIPOMOTIVOCAMBIOFECHA").InnerText;
                            break;
                        case "MOTIVOCAMBIOFECHA":
                            elemento.MOTIVOCAMBIOFECHA = node.SelectSingleNode("MOTIVOCAMBIOFECHA").InnerText;
                            break;
                        case "FECHAINICIOPERIODO":
                            elemento.FECHAINICIOPERIODO = node.SelectSingleNode("FECHAINICIOPERIODO").InnerText;
                            break;
                        case "FECHAFINPERIODO":
                            elemento.FECHAFINPERIODO = node.SelectSingleNode("FECHAFINPERIODO").InnerText;
                            break;
                        default:
                            throw new Exception("Propiedad no controlada");
                    }
                }
                elementos.Add(elemento);
            }
            Console.WriteLine($"\rLeídos {elementos.Count} elementos de {pFile}");
            return elementos;
        }

        private static List<FechaProyecto> LeerFechasProyectos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<FechaProyecto> elementos = new List<FechaProyecto>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                FechaProyecto elemento = new FechaProyecto();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "IDPROYECTO":
                            elemento.IDPROYECTO = node.SelectSingleNode("IDPROYECTO").InnerText;
                            break;
                        case "NUMERO":
                            elemento.NUMERO = node.SelectSingleNode("NUMERO").InnerText;
                            break;
                        case "FECHAINICIOEXPEDIENTE":
                            elemento.FECHAINICIOEXPEDIENTE = node.SelectSingleNode("FECHAINICIOEXPEDIENTE").InnerText;
                            break;
                        case "FECHAINICIOPROYECTO":
                            elemento.FECHAINICIOPROYECTO = node.SelectSingleNode("FECHAINICIOPROYECTO").InnerText;
                            break;
                        case "FECHAFINPROYECTO":
                            elemento.FECHAFINPROYECTO = node.SelectSingleNode("FECHAFINPROYECTO").InnerText;
                            break;
                        case "CODTIPOMOTIVOCAMBIOFECHA":
                            //elemento.CODTIPOMOTIVOCAMBIOFECHA = node.SelectSingleNode("CODTIPOMOTIVOCAMBIOFECHA").InnerText;
                            break;
                        case "MOTIVOCAMBIOFECHA":
                            elemento.MOTIVOCAMBIOFECHA = node.SelectSingleNode("MOTIVOCAMBIOFECHA").InnerText;
                            break;
                        default:
                            throw new Exception("Propiedad no controlada");
                    }
                }
                elementos.Add(elemento);
            }
            Console.WriteLine($"\rLeídos {elementos.Count} elementos de {pFile}");
            return elementos;
        }

        private static List<Proyecto> LeerProyectos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<Proyecto> elementos = new List<Proyecto>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                Proyecto elemento = new Proyecto();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "IDPROYECTO":
                            elemento.IDPROYECTO = node.SelectSingleNode("IDPROYECTO").InnerText;
                            break;
                        case "NOMBRE":
                            elemento.NOMBRE = node.SelectSingleNode("NOMBRE").InnerText;
                            break;
                        case "PROYECTOFINALISTA":
                            elemento.PROYECTOFINALISTA = node.SelectSingleNode("PROYECTOFINALISTA").InnerText;
                            break;
                        case "LIMITATIVO":
                            elemento.LIMITATIVO = node.SelectSingleNode("LIMITATIVO").InnerText;
                            break;
                        case "TIPOFINANCIACION":
                            elemento.TIPOFINANCIACION = node.SelectSingleNode("TIPOFINANCIACION").InnerText;
                            break;
                        case "AMBITO_GEOGRAFICO":
                            elemento.AMBITO_GEOGRAFICO = node.SelectSingleNode("AMBITO_GEOGRAFICO").InnerText;
                            break;
                        default:
                            throw new Exception("Propiedad no controlada");
                    }
                }
                elementos.Add(elemento);
            }
            Console.WriteLine($"\rLeídos {elementos.Count} elementos de {pFile}");
            return elementos;
        }

        private static List<TipoParticipacionGrupo> LeerTipoParticipacionGrupos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<TipoParticipacionGrupo> elementos = new List<TipoParticipacionGrupo>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                TipoParticipacionGrupo elemento = new TipoParticipacionGrupo();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "CODTIPOPARTICIPACIONGRUPO":
                            elemento.CODTIPOPARTICIPACIONGRUPO = node.SelectSingleNode("CODTIPOPARTICIPACIONGRUPO").InnerText;
                            break;
                        case "DESCRIPCION":
                            elemento.DESCRIPCION = node.SelectSingleNode("DESCRIPCION").InnerText;
                            break;
                        default:
                            throw new Exception("Propiedad no controlada");
                    }
                }
                elementos.Add(elemento);
            }
            Console.WriteLine($"\rLeídos {elementos.Count} elementos de {pFile}");
            return elementos;
        }

        private static List<DatoEquipoInvestigacion> LeerDatoEquiposInvestigacion(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<DatoEquipoInvestigacion> elementos = new List<DatoEquipoInvestigacion>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                DatoEquipoInvestigacion elemento = new DatoEquipoInvestigacion();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "IDGRUPOINVESTIGACION":
                            elemento.IDGRUPOINVESTIGACION = node.SelectSingleNode("IDGRUPOINVESTIGACION").InnerText;
                            break;
                        case "NUMERO":
                            elemento.NUMERO = node.SelectSingleNode("NUMERO").InnerText;
                            break;
                        case "IDPERSONA":
                            elemento.IDPERSONA = node.SelectSingleNode("IDPERSONA").InnerText;
                            break;
                        case "CODTIPOPARTICIPACION":
                            elemento.CODTIPOPARTICIPACION = node.SelectSingleNode("CODTIPOPARTICIPACION").InnerText;
                            break;
                        case "RESPONSABLE":
                            elemento.RESPONSABLE = node.SelectSingleNode("RESPONSABLE").InnerText;
                            break;
                        case "EDP":
                            elemento.EDP = node.SelectSingleNode("EDP").InnerText;
                            break;
                        case "FECHAINICIOPERIODO":
                            elemento.FECHAINICIOPERIODO = node.SelectSingleNode("FECHAINICIOPERIODO").InnerText;
                            break;
                        case "FECHAFINPERIODO":
                            elemento.FECHAFINPERIODO = node.SelectSingleNode("FECHAFINPERIODO").InnerText;
                            break;
                        default:
                            throw new Exception("Propiedad no controlada");
                    }
                }
                elementos.Add(elemento);
            }
            Console.WriteLine($"\rLeídos {elementos.Count} elementos de {pFile}");
            return elementos;
        }

        private static List<GrupoInvestigacion> LeerGruposInvestigacion(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<GrupoInvestigacion> elementos = new List<GrupoInvestigacion>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                GrupoInvestigacion elemento = new GrupoInvestigacion();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "IDGRUPOINVESTIGACION":
                            elemento.IDGRUPOINVESTIGACION = node.SelectSingleNode("IDGRUPOINVESTIGACION").InnerText;
                            break;
                        case "DESCRIPCION":
                            elemento.DESCRIPCION = node.SelectSingleNode("DESCRIPCION").InnerText;
                            break;
                        case "CODUNIDADADM":
                            elemento.CODUNIDADADM = node.SelectSingleNode("CODUNIDADADM").InnerText;
                            break;
                        case "EXCELENCIA":
                            elemento.EXCELENCIA = node.SelectSingleNode("EXCELENCIA").InnerText;
                            break;
                        case "FECHACREACION":
                            elemento.FECHACREACION = node.SelectSingleNode("FECHACREACION").InnerText;
                            break;
                        case "FECHADESAPARICION":
                            elemento.FECHADESAPARICION = node.SelectSingleNode("FECHADESAPARICION").InnerText;
                            break;
                        default:
                            throw new Exception("Propiedad no controlada");
                    }
                }
                elementos.Add(elemento);
            }
            Console.WriteLine($"\rLeídos {elementos.Count} elementos de {pFile}");
            return elementos;
        }

        private static List<Feature> LeerFeatures()
        {
            Console.Write($"Leyendo features...");
            List<Feature> elementos = new List<Feature>();
            Feature nacional = new Feature() { ID = "NACIONAL", Name = "España", Uri = "https://www.geonames.org/2510769" };
            elementos.Add(nacional);
            Feature regional = new Feature() { ID = "REGIONAL", Name = "Región de Murcia", Uri = "https://www.geonames.org/2513413" };
            elementos.Add(regional);
            Feature propio = new Feature() { ID = "PROPIO", Name = "Universidad de Murcia", Uri = "https://www.geonames.org/6255004" };
            elementos.Add(propio);
            Feature europeo = new Feature() { ID = "EUROPEO", Name = "Europa", Uri = "https://www.geonames.org/6255148" };
            elementos.Add(europeo);
            Feature internacional = new Feature() { ID = "INTERNACIONAL", Name = "Mundo", Uri = "https://www.geonames.org/6295630" };
            elementos.Add(internacional);
            Console.WriteLine($"\rLeídos {elementos.Count} elementos de features");
            return elementos;
        }

        private static List<string> Propiedades(Object objeto)
        {
            List<string> prpos = new List<string>();
            Type type = objeto.GetType();
            System.Reflection.PropertyInfo[] listaPropiedades = type.GetProperties();
            return listaPropiedades.Select(x => x.Name).ToList();
        }
        #endregion

        #region Generación de RDFs

        private static void GenerarPersonas(string pRuta, string pUriUrisFactory, List<Persona> pPersonas, List<Centro> pCentros, List<Departamento> pDepartamento, List<DatoEquipoInvestigacion> pDatoEquipoInvestigacion, List<TipoParticipacionGrupo> pTipoParticipacionGrupo, List<GrupoInvestigacion> pGrupoInvestigacion)
        {
            System.IO.Directory.CreateDirectory(pRuta);
            int i = 0;
            int numPersonasEquipos = 0;
            foreach (Persona persona in pPersonas)
            {
                i++;
                Console.Write($"\rGenerando RDFs en {pRuta} {i}/{pPersonas.Count}");
                string rutaPersona = pRuta + persona.IDPERSONA + ".rdf";
                Graph graph = new Graph();

                //Rdftype
                INode uriSubject = GetUri(pUriUrisFactory, graph, "http://purl.org/roh/mirror/foaf#Person", persona.IDPERSONA);
                INode uriPredicateRdfType = graph.CreateUriNode(new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
                INode uriObjectRdfType = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/foaf#Person"));
                Triple tripleRdfType = new Triple(uriSubject, uriPredicateRdfType, uriObjectRdfType);
                graph.Assert(tripleRdfType);

                //Nombre
                INode uriPredicateFoafName = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/foaf#name"));
                INode objectFoafName = CreateTextNode(graph, persona.NOMBRE);
                Triple tripleNombre = new Triple(uriSubject, uriPredicateFoafName, objectFoafName);
                graph.Assert(tripleNombre);

                {
                    //Identificador
                    INode uriPredicateCrisIdentifier = graph.CreateUriNode(new Uri("http://purl.org/roh#crisIdentifier"));
                    Triple tripleCrisIdentifier = new Triple(uriSubject, uriPredicateCrisIdentifier, GetIdentifier(graph, "Person", persona.IDPERSONA));
                    graph.Assert(tripleCrisIdentifier);
                }
                {
                    Centro centro = pCentros.FirstOrDefault(x => x.CED_CODIGO == persona.PERS_CENT_CODIGO);
                    if (centro != null)
                    {
                        //Rdftype
                        INode uriSubjectCentro = GetUri(pUriUrisFactory, graph, "http://purl.org/roh/mirror/vivo#Center", centro.CED_CODIGO);
                        INode uriObjectRdfTypeCentro = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#Center"));
                        Triple tripleRdfTypeCentro = new Triple(uriSubjectCentro, uriPredicateRdfType, uriObjectRdfTypeCentro);
                        graph.Assert(tripleRdfTypeCentro);

                        //Title
                        INode uriPredicateRohTitle = graph.CreateUriNode(new Uri("http://purl.org/roh#title"));
                        INode objectRohTitle = CreateTextNode(graph, centro.CED_NOMBRE);
                        Triple tripleTitle = new Triple(uriSubjectCentro, uriPredicateRohTitle, objectRohTitle);
                        graph.Assert(tripleTitle);

                        //Identificador
                        INode uriPredicateCrisIdentifier = graph.CreateUriNode(new Uri("http://purl.org/roh#crisIdentifier"));
                        Triple tripleCrisIdentifier = new Triple(uriSubjectCentro, uriPredicateCrisIdentifier, GetIdentifier(graph, "Center", centro.CED_CODIGO));
                        graph.Assert(tripleCrisIdentifier);


                        //Miembro
                        //Rdftype MemberRole
                        INode uriSubjectRole = GetUri(pUriUrisFactory, graph, "http://purl.org/roh/mirror/vivo#MemberRole", "");
                        INode uriObjectRdfTypeMemberRole = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#MemberRole"));
                        Triple tripleRdfTypeMemberRole = new Triple(uriSubjectRole, uriPredicateRdfType, uriObjectRdfTypeMemberRole);
                        graph.Assert(tripleRdfTypeMemberRole);


                        //hasRole
                        INode uriPredicateHasRole = graph.CreateUriNode(new Uri("http://purl.org/roh#hasRole"));
                        Triple tripleHasRole = new Triple(uriSubject, uriPredicateHasRole, uriSubjectRole);
                        graph.Assert(tripleHasRole);

                        //relatedBy
                        INode uriPredicateRelatedBy = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#relatedBy"));
                        Triple tripleRelatedBy = new Triple(uriSubjectRole, uriPredicateRelatedBy, uriSubjectCentro);
                        graph.Assert(tripleRelatedBy);
                    }
                }
                {
                    Departamento departamento = pDepartamento.FirstOrDefault(x => x.DEP_CODIGO == persona.PERS_DEPT_CODIGO);
                    if (departamento != null)
                    {
                        //Rdftype
                        INode uriSubjectDepartamento = GetUri(pUriUrisFactory, graph, "http://purl.org/roh/mirror/vivo#Department", departamento.DEP_CODIGO);
                        INode uriObjectRdfTypDepartamento = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#Department"));
                        Triple tripleRdfTypeCentro = new Triple(uriSubjectDepartamento, uriPredicateRdfType, uriObjectRdfTypDepartamento);
                        graph.Assert(tripleRdfTypeCentro);

                        //Title
                        INode uriPredicateRohTitle = graph.CreateUriNode(new Uri("http://purl.org/roh#title"));
                        INode objectRohTitle = CreateTextNode(graph, departamento.DEP_NOMBRE);
                        Triple tripleTitle = new Triple(uriSubjectDepartamento, uriPredicateRohTitle, objectRohTitle);
                        graph.Assert(tripleTitle);

                        //Identificador
                        INode uriPredicateCrisIdentifier = graph.CreateUriNode(new Uri("http://purl.org/roh#crisIdentifier"));
                        Triple tripleCrisIdentifier = new Triple(uriSubjectDepartamento, uriPredicateCrisIdentifier, GetIdentifier(graph, "Department", departamento.DEP_CODIGO));
                        graph.Assert(tripleCrisIdentifier);


                        //Miembro
                        //Rdftype MemberRole
                        INode uriSubjectRole = GetUri(pUriUrisFactory, graph, "http://purl.org/roh/mirror/vivo#MemberRole", "");
                        INode uriObjectRdfTypeMemberRole = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#MemberRole"));
                        Triple tripleRdfTypeMemberRole = new Triple(uriSubjectRole, uriPredicateRdfType, uriObjectRdfTypeMemberRole);
                        graph.Assert(tripleRdfTypeMemberRole);


                        //hasRole
                        INode uriPredicateHasRole = graph.CreateUriNode(new Uri("http://purl.org/roh#hasRole"));
                        Triple tripleHasRole = new Triple(uriSubject, uriPredicateHasRole, uriSubjectRole);
                        graph.Assert(tripleHasRole);

                        //relatedBy
                        INode uriPredicateRelatedBy = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#relatedBy"));
                        Triple tripleRelatedBy = new Triple(uriSubjectRole, uriPredicateRelatedBy, uriSubjectDepartamento);
                        graph.Assert(tripleRelatedBy);
                    }
                }
                {
                    List<DatoEquipoInvestigacion> datosEquipoInvestigacion = pDatoEquipoInvestigacion.Where(x => x.IDPERSONA == persona.IDPERSONA).ToList();
                    if (datosEquipoInvestigacion != null && datosEquipoInvestigacion.Count > 0)
                    {
                        numPersonasEquipos++;
                        foreach (DatoEquipoInvestigacion datoEquipoInvestigacion in datosEquipoInvestigacion)
                        {
                            GrupoInvestigacion grupoInvestigacion = pGrupoInvestigacion.FirstOrDefault(x => x.IDGRUPOINVESTIGACION == datoEquipoInvestigacion.IDGRUPOINVESTIGACION);
                            if (grupoInvestigacion != null)
                            {
                                //Rdftype
                                INode uriSubjectEquipoInvestigacion = GetUri(pUriUrisFactory, graph, "http://purl.org/roh#ResearchGroup", grupoInvestigacion.IDGRUPOINVESTIGACION);
                                INode uriObjectRdfTypEquipoInvestigacion = graph.CreateUriNode(new Uri("http://purl.org/roh#ResearchGroup"));
                                Triple tripleRdfTypeEquipoInvestigacion = new Triple(uriSubjectEquipoInvestigacion, uriPredicateRdfType, uriObjectRdfTypEquipoInvestigacion);
                                graph.Assert(tripleRdfTypeEquipoInvestigacion);

                                //Title
                                INode uriPredicateRohTitle = graph.CreateUriNode(new Uri("http://purl.org/roh#title"));
                                INode objectRohTitle = CreateTextNode(graph, grupoInvestigacion.DESCRIPCION);
                                Triple tripleTitle = new Triple(uriSubjectEquipoInvestigacion, uriPredicateRohTitle, objectRohTitle);
                                graph.Assert(tripleTitle);

                                //Identificador
                                INode uriPredicateCrisIdentifier = graph.CreateUriNode(new Uri("http://purl.org/roh#crisIdentifier"));
                                Triple tripleCrisIdentifier = new Triple(uriSubjectEquipoInvestigacion, uriPredicateCrisIdentifier, GetIdentifier(graph, "ResearchGroup", grupoInvestigacion.IDGRUPOINVESTIGACION));
                                graph.Assert(tripleCrisIdentifier);

                                INode uriSubjectRole;
                                switch (datoEquipoInvestigacion.CODTIPOPARTICIPACION)
                                {
                                    case "IP":
                                        {
                                            //Rdftype PrincipalInvestigatorRole
                                            uriSubjectRole = GetUri(pUriUrisFactory, graph, "http://purl.org/roh/mirror/vivo#PrincipalInvestigatorRole", "");
                                            INode uriObjectRdfTypeMemberRole = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#PrincipalInvestigatorRole"));
                                            Triple tripleRdfTypeMemberRole = new Triple(uriSubjectRole, uriPredicateRdfType, uriObjectRdfTypeMemberRole);
                                            graph.Assert(tripleRdfTypeMemberRole);
                                        }
                                        break;
                                    case "INV":
                                        {
                                            //Rdftype InvestigatorRole
                                            uriSubjectRole = GetUri(pUriUrisFactory, graph, "http://purl.org/roh/mirror/vivo#InvestigatorRole", "");
                                            INode uriObjectRdfTypeMemberRole = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#InvestigatorRole"));
                                            Triple tripleRdfTypeMemberRole = new Triple(uriSubjectRole, uriPredicateRdfType, uriObjectRdfTypeMemberRole);
                                            graph.Assert(tripleRdfTypeMemberRole);
                                        }
                                        break;
                                    default:
                                        {
                                            //Rdftype MemberRole
                                            uriSubjectRole = GetUri(pUriUrisFactory, graph, "http://purl.org/roh/mirror/vivo#MemberRole", "");
                                            INode uriObjectRdfTypeMemberRole = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#MemberRole"));
                                            Triple tripleRdfTypeMemberRole = new Triple(uriSubjectRole, uriPredicateRdfType, uriObjectRdfTypeMemberRole);
                                            graph.Assert(tripleRdfTypeMemberRole);
                                        }
                                        break;

                                }

                                //hasRole
                                INode uriPredicateHasRole = graph.CreateUriNode(new Uri("http://purl.org/roh#hasRole"));
                                Triple tripleHasRole = new Triple(uriSubject, uriPredicateHasRole, uriSubjectRole);
                                graph.Assert(tripleHasRole);

                                //relatedBy
                                INode uriPredicateRelatedBy = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#relatedBy"));
                                Triple tripleRelatedBy = new Triple(uriSubjectRole, uriPredicateRelatedBy, uriSubjectEquipoInvestigacion);
                                graph.Assert(tripleRelatedBy);

                                //dedicationPercentage
                                INode uriDedicationPercentage = graph.CreateUriNode(new Uri("http://purl.org/roh#dedicationPercentage"));
                                INode objectDedicationPercentage = graph.CreateLiteralNode((float.Parse(datoEquipoInvestigacion.EDP) * 100).ToString(), new Uri("http://www.w3.org/2001/XMLSchema#double"));
                                Triple tripleDedicationPercentage = new Triple(uriSubjectRole, uriDedicationPercentage, objectDedicationPercentage);
                                graph.Assert(tripleDedicationPercentage);

                                if (!string.IsNullOrEmpty(datoEquipoInvestigacion.FECHAINICIOPERIODO) || !string.IsNullOrEmpty(datoEquipoInvestigacion.FECHAFINPERIODO))
                                {
                                    //Rdftype DateTimeInterval
                                    INode uriSubjectDateTimeInterval = GetUri(pUriUrisFactory, graph, "http://purl.org/roh/mirror/vivo#DateTimeInterval", "");
                                    INode uriObjectRdfTypeDateTimeInterval = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#DateTimeInterval"));
                                    Triple tripleRdfTypeDateTimeInterval = new Triple(uriSubjectDateTimeInterval, uriPredicateRdfType, uriObjectRdfTypeDateTimeInterval);
                                    graph.Assert(tripleRdfTypeDateTimeInterval);

                                    //PredicateDateTimeInterval
                                    INode uriPredicateDateTimeInterval = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#dateTimeInterval"));
                                    Triple tripleDateTimeInterval = new Triple(uriSubjectRole, uriPredicateDateTimeInterval, uriSubjectDateTimeInterval);
                                    graph.Assert(tripleDateTimeInterval);

                                    if (!string.IsNullOrEmpty(datoEquipoInvestigacion.FECHAINICIOPERIODO))
                                    {
                                        string anio = datoEquipoInvestigacion.FECHAINICIOPERIODO.Substring(0, 4);
                                        string mes = datoEquipoInvestigacion.FECHAINICIOPERIODO.Substring(5, 2);
                                        string dia = datoEquipoInvestigacion.FECHAINICIOPERIODO.Substring(8, 2);
                                        //Rdftype DateTimeValue inicio
                                        INode uriSubjectDateTimeValueInicio = GetUri(pUriUrisFactory, graph, "http://purl.org/roh/mirror/vivo#DateTimeValue", "");
                                        INode uriObjectRdfTypeDateTimeValue = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#DateTimeValue"));
                                        Triple tripleRdfTypeDateTimeValueInicio = new Triple(uriSubjectDateTimeValueInicio, uriPredicateRdfType, uriObjectRdfTypeDateTimeValue);
                                        graph.Assert(tripleRdfTypeDateTimeValueInicio);

                                        //PredicateStart
                                        INode uriPredicateStart = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#start"));
                                        Triple tripleStart = new Triple(uriSubjectDateTimeInterval, uriPredicateStart, uriSubjectDateTimeValueInicio);
                                        graph.Assert(tripleStart);

                                        //dateTime
                                        INode uriPredicateDateTime = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#dateTime"));
                                        INode uriObjectDateTime = graph.CreateLiteralNode($"{anio}-{mes}-{dia}T00:00:00.000+00:00", new Uri("http://www.w3.org/2001/XMLSchema#datetime"));
                                        Triple tripleDateTime = new Triple(uriSubjectDateTimeValueInicio, uriPredicateDateTime, uriObjectDateTime);
                                        graph.Assert(tripleDateTime);
                                    }

                                    if (!string.IsNullOrEmpty(datoEquipoInvestigacion.FECHAFINPERIODO))
                                    {
                                        string anio = datoEquipoInvestigacion.FECHAFINPERIODO.Substring(0, 4);
                                        string mes = datoEquipoInvestigacion.FECHAFINPERIODO.Substring(5, 2);
                                        string dia = datoEquipoInvestigacion.FECHAFINPERIODO.Substring(8, 2);
                                        //Rdftype DateTimeValue fin
                                        INode uriSubjectDateTimeValueFin = GetUri(pUriUrisFactory, graph, "http://purl.org/roh/mirror/vivo#DateTimeValue", "");
                                        INode uriObjectRdfTypeDateTimeValue = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#DateTimeValue"));
                                        Triple tripleRdfTypeDateTimeValueInicioFin = new Triple(uriSubjectDateTimeValueFin, uriPredicateRdfType, uriObjectRdfTypeDateTimeValue);
                                        graph.Assert(tripleRdfTypeDateTimeValueInicioFin);

                                        //PredicateEnd
                                        INode uriPredicateEnd = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#end"));
                                        Triple tripleEnd = new Triple(uriSubjectDateTimeInterval, uriPredicateEnd, uriSubjectDateTimeValueFin);
                                        graph.Assert(tripleEnd);

                                        //dateTime
                                        INode uriPredicateDateTime = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#dateTime"));
                                        INode uriObjectDateTime = graph.CreateLiteralNode($"{anio}-{mes}-{dia}T00:00:00.000+00:00", new Uri("http://www.w3.org/2001/XMLSchema#datetime"));
                                        Triple tripleDateTime = new Triple(uriSubjectDateTimeValueFin, uriPredicateDateTime, uriObjectDateTime);
                                        graph.Assert(tripleDateTime);
                                    }
                                }
                            }
                        }
                    }
                }
                graph.SaveToFile(rutaPersona);
            }
            Console.WriteLine($"\rGenerando RDFs en {pRuta} {i}/{pPersonas.Count}");
        }

        private static void GenerarArticulos(string pRuta, string pUriUrisFactory, List<Articulo> pArticulos, List<Persona> pPersonas, List<AutorArticulo> pAutorArticulo)
        {
            System.IO.Directory.CreateDirectory(pRuta);
            int i = 0;
            foreach (Articulo articulo in pArticulos)
            {
                i++;
                Console.Write($"\rGenerando RDFs en {pRuta} {i}/{pArticulos.Count}");
                string rutaArticulo = pRuta + articulo.CODIGO + ".rdf";
                Graph graph = new Graph();

                //Rdftype
                INode uriSubject = GetUri(pUriUrisFactory, graph, "http://purl.org/roh/mirror/bibo#AcademicArticle", articulo.CODIGO);
                INode uriPredicateRdfType = graph.CreateUriNode(new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
                INode uriObjectRdfType = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/bibo#AcademicArticle"));
                Triple tripleRdfType = new Triple(uriSubject, uriPredicateRdfType, uriObjectRdfType);
                graph.Assert(tripleRdfType);

                //Title
                INode uriPredicateRohTitle = graph.CreateUriNode(new Uri("http://purl.org/roh#title"));
                INode objectRohTitle = CreateTextNode(graph, articulo.TITULO);
                Triple tripleTitulo = new Triple(uriSubject, uriPredicateRohTitle, objectRohTitle);
                graph.Assert(tripleTitulo);
                {
                    //Identificador
                    INode uriPredicateCrisIdentifier = graph.CreateUriNode(new Uri("http://purl.org/roh#crisIdentifier"));
                    Triple tripleCrisIdentifier = new Triple(uriSubject, uriPredicateCrisIdentifier, GetIdentifier(graph, "AcademicArticle", articulo.CODIGO));
                    graph.Assert(tripleCrisIdentifier);
                }

                //Fecha
                if (!string.IsNullOrEmpty(articulo.ANO))
                {
                    INode uriPredicateDate = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#dateTime"));
                    INode objectDate = graph.CreateLiteralNode(articulo.ANO + "-01-01T00:00:00.000+00:00", new Uri("http://www.w3.org/2001/XMLSchema#datetime"));
                    Triple tripleDate = new Triple(uriSubject, uriPredicateDate, objectDate);
                    graph.Assert(tripleDate);
                }

                //Volumen
                if (!string.IsNullOrEmpty(articulo.VOLUMEN))
                {
                    INode uriPredicateVolumen = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/bibo#volume"));
                    INode objectVolumen = CreateTextNode(graph, articulo.VOLUMEN);
                    Triple tripleVolumen = new Triple(uriSubject, uriPredicateVolumen, objectVolumen);
                    graph.Assert(tripleVolumen);
                }

                //DOI
                if (!string.IsNullOrEmpty(articulo.ARTI_DOI))
                {
                    INode uriPredicateDOI = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/bibo#doi"));
                    INode objectDOI = CreateTextNode(graph, articulo.ARTI_DOI);
                    Triple tripleDOI = new Triple(uriSubject, uriPredicateDOI, objectDOI);
                    graph.Assert(tripleDOI);
                }

                //Inicio
                if (!string.IsNullOrEmpty(articulo.PAGDESDE))
                {
                    INode uriPredicatePageStart = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/bibo#pageStart"));
                    INode objectPageStart = graph.CreateLiteralNode(articulo.PAGDESDE, new Uri("http://www.w3.org/2001/XMLSchema#int"));
                    Triple triplePageStart = new Triple(uriSubject, uriPredicatePageStart, objectPageStart);
                    graph.Assert(triplePageStart);
                }

                //Fin
                if (!string.IsNullOrEmpty(articulo.PAGDESDE))
                {
                    INode uriPredicatePageEnd = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/bibo#pageEnd"));
                    INode objectPageEnd = graph.CreateLiteralNode(articulo.PAGHASTA, new Uri("http://www.w3.org/2001/XMLSchema#int"));
                    Triple triplePageEnd = new Triple(uriSubject, uriPredicatePageEnd, objectPageEnd);
                    graph.Assert(triplePageEnd);
                }

                //Revista
                if (!string.IsNullOrEmpty(articulo.NOMBRE_REVISTA))
                {
                    //Rdftype Journal
                    INode uriSubjectJournal = GetUri(pUriUrisFactory, graph, "http://purl.org/roh/mirror/bibo#Journal", HttpUtility.UrlEncode(articulo.NOMBRE_REVISTA.ToLower()));
                    INode uriObjectRdfTypeJournal = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/bibo#Journal"));
                    Triple tripleRdfTypeJournal = new Triple(uriSubjectJournal, uriPredicateRdfType, uriObjectRdfTypeJournal);
                    graph.Assert(tripleRdfTypeJournal);

                    //Title Journal
                    INode titleJournal = CreateTextNode(graph, articulo.NOMBRE_REVISTA.ToUpper());
                    Triple tripleTitleJournal = new Triple(uriSubjectJournal, uriPredicateRohTitle, titleJournal);
                    graph.Assert(tripleTitleJournal);

                    if (!string.IsNullOrEmpty(articulo.REIS_ISSN))
                    {
                        //ISSN
                        INode uriPredicateISSN = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/bibo#issn"));
                        INode objectISSN = CreateTextNode(graph, articulo.REIS_ISSN);
                        Triple tripleISSN = new Triple(uriSubjectJournal, uriPredicateISSN, objectISSN);
                        graph.Assert(tripleISSN);
                    }

                    //Publicado en
                    INode uriPropertyHasPublicationVenue = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#hasPublicationVenue"));
                    Triple tripleHasPublicationVenue = new Triple(uriSubject, uriPropertyHasPublicationVenue, uriSubjectJournal);
                    graph.Assert(tripleHasPublicationVenue);
                }


                List<AutorArticulo> autores = pAutorArticulo.Where(x => x.ARTI_CODIGO == articulo.CODIGO).ToList();
                if (autores.Count > 0)
                {
                    //Lista de autores
                    INode uriPredicateBiboAuthorList = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/bibo#authorList"));
                    INode uriAuthorList = GetUri(pUriUrisFactory, graph, "http://www.w3.org/1999/02/22-rdf-syntax-ns#Seq", null);
                    Triple tripleAuthorList = new Triple(uriSubject, uriPredicateBiboAuthorList, uriAuthorList);
                    graph.Assert(tripleAuthorList);

                    //Rdftype AuthorList                
                    INode uriObjectRdfTypeSeq = graph.CreateUriNode(new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#Seq"));
                    Triple tripleRdfTypeSeq = new Triple(uriAuthorList, uriPredicateRdfType, uriObjectRdfTypeSeq);
                    graph.Assert(tripleRdfTypeSeq);

                    foreach (AutorArticulo autorArticulo in autores)
                    {
                        Persona persona = pPersonas.FirstOrDefault(x => x.IDPERSONA == autorArticulo.IDPERSONA);
                        if (persona != null)
                        {
                            //Rdftype
                            INode uriSubjectPersona = GetUri(pUriUrisFactory, graph, "http://purl.org/roh/mirror/foaf#Person", persona.IDPERSONA);
                            INode uriObjectRdfTypePersona = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/foaf#Person"));
                            Triple tripleRdfTypePersona = new Triple(uriSubjectPersona, uriPredicateRdfType, uriObjectRdfTypePersona);
                            graph.Assert(tripleRdfTypePersona);

                            //Nombre
                            INode uriPredicateFoafName = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/foaf#name"));
                            INode objectFoafName = CreateTextNode(graph, persona.NOMBRE);
                            Triple tripleNombre = new Triple(uriSubjectPersona, uriPredicateFoafName, objectFoafName);
                            graph.Assert(tripleNombre);

                            //Identificador
                            INode uriPredicateCrisIdentifier = graph.CreateUriNode(new Uri("http://purl.org/roh#crisIdentifier"));
                            Triple tripleCrisIdentifier = new Triple(uriSubjectPersona, uriPredicateCrisIdentifier, GetIdentifier(graph, "Person", persona.IDPERSONA));
                            graph.Assert(tripleCrisIdentifier);

                            //Seq-persona
                            INode uriPredicateSeq = graph.CreateUriNode(new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#_" + autorArticulo.ORDEN));
                            Triple tripleSeqPersona = new Triple(uriAuthorList, uriPredicateSeq, uriSubjectPersona);
                            graph.Assert(tripleSeqPersona);
                        }
                    }
                }
                graph.SaveToFile(rutaArticulo);
            }
            Console.WriteLine($"\rGenerando RDFs en {pRuta} {i}/{pArticulos.Count}");
        }

        private static void GenerarOrganizaciones(string pRuta, string pUrlUrisFactory, List<Centro> pCentros, List<Departamento> pDepartamentos, List<GrupoInvestigacion> pGruposInvestigacion)
        {
            System.IO.Directory.CreateDirectory(pRuta);
            int i = 0;
            int total = pCentros.Count + pDepartamentos.Count + pGruposInvestigacion.Count;
            foreach (Centro centro in pCentros)
            {
                i++;
                Console.Write($"\rGenerando RDFs en {pRuta} {i}/{total}");
                string rutaCentro = pRuta + "Centro_" + centro.CED_CODIGO + ".rdf";
                Graph graph = new Graph();

                //Rdftype
                INode uriSubject = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/vivo#Center", centro.CED_CODIGO);
                INode uriPredicateRdfType = graph.CreateUriNode(new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
                INode uriObjectRdfType = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#Center"));
                Triple tripleRdfType = new Triple(uriSubject, uriPredicateRdfType, uriObjectRdfType);
                graph.Assert(tripleRdfType);

                //Title
                INode uriPredicateRohTitle = graph.CreateUriNode(new Uri("http://purl.org/roh#title"));
                INode objectRohTitle = CreateTextNode(graph, centro.CED_NOMBRE);
                Triple tripleTitle = new Triple(uriSubject, uriPredicateRohTitle, objectRohTitle);
                graph.Assert(tripleTitle);

                //Identificador
                INode uriPredicateCrisIdentifier = graph.CreateUriNode(new Uri("http://purl.org/roh#crisIdentifier"));
                Triple tripleCrisIdentifier = new Triple(uriSubject, uriPredicateCrisIdentifier, GetIdentifier(graph, "Center", centro.CED_CODIGO));
                graph.Assert(tripleCrisIdentifier);

                graph.SaveToFile(rutaCentro);


            }

            foreach (Departamento departamento in pDepartamentos)
            {
                i++;
                Console.Write($"\rGenerando RDFs en {pRuta} {i}/{total}");
                string rutaDepartamento = pRuta + "Departamento_" + departamento.DEP_CODIGO + ".rdf";
                Graph graph = new Graph();

                //Rdftype
                INode uriSubject = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/vivo#Department", departamento.DEP_CODIGO);
                INode uriPredicateRdfType = graph.CreateUriNode(new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));

                INode uriObjectRdfType = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#Department"));
                Triple tripleRdfType = new Triple(uriSubject, uriPredicateRdfType, uriObjectRdfType);
                graph.Assert(tripleRdfType);

                //Title
                INode uriPredicateRohTitle = graph.CreateUriNode(new Uri("http://purl.org/roh#title"));
                INode objectRohTitle = CreateTextNode(graph, departamento.DEP_NOMBRE);
                Triple tripleTitle = new Triple(uriSubject, uriPredicateRohTitle, objectRohTitle);
                graph.Assert(tripleTitle);
                {
                    //Identificador
                    INode uriPredicateCrisIdentifier = graph.CreateUriNode(new Uri("http://purl.org/roh#crisIdentifier"));
                    Triple tripleCrisIdentifier = new Triple(uriSubject, uriPredicateCrisIdentifier, GetIdentifier(graph, "Department", departamento.DEP_CODIGO));
                    graph.Assert(tripleCrisIdentifier);
                }
                if (!string.IsNullOrEmpty(departamento.DEP_CED_CODIGO))
                {
                    Centro centro = pCentros.FirstOrDefault(x => x.CED_CODIGO == departamento.DEP_CED_CODIGO);
                    if (centro != null)
                    {
                        //Rdftype
                        INode uriSubjectCentro = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/vivo#Center", centro.CED_CODIGO);
                        INode uriObjectRdfTypeCentro = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#Center"));
                        Triple tripleRdfTypeCentro = new Triple(uriSubjectCentro, uriPredicateRdfType, uriObjectRdfTypeCentro);
                        graph.Assert(tripleRdfTypeCentro);

                        //Title
                        INode objectRohTitleCentro = CreateTextNode(graph, centro.CED_NOMBRE);
                        Triple tripleTitleCentro = new Triple(uriSubjectCentro, uriPredicateRohTitle, objectRohTitleCentro);
                        graph.Assert(tripleTitleCentro);

                        //Identificador
                        INode uriPredicateCrisIdentifier = graph.CreateUriNode(new Uri("http://purl.org/roh#crisIdentifier"));
                        Triple tripleCrisIdentifier = new Triple(uriSubjectCentro, uriPredicateCrisIdentifier, GetIdentifier(graph, "Center", centro.CED_CODIGO));
                        graph.Assert(tripleCrisIdentifier);

                        INode uriPredicateParteDe = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/obo/ro#BFO_0000050"));
                        Triple tripleParteDe = new Triple(uriSubject, uriPredicateParteDe, uriSubjectCentro);
                        graph.Assert(tripleParteDe);
                    }
                }
                graph.SaveToFile(rutaDepartamento);
            }

            foreach (GrupoInvestigacion grupoInvestigacion in pGruposInvestigacion)
            {
                i++;
                Console.Write($"\rGenerando RDFs en {pRuta} {i}/{total}");
                string rutaGrupoInvestigacion = pRuta + "GrupoInvestigacion_" + grupoInvestigacion.IDGRUPOINVESTIGACION.Replace("/", "--") + ".rdf";
                Graph graph = new Graph();

                //Rdftype
                INode uriSubject = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh#ResearchGroup", grupoInvestigacion.IDGRUPOINVESTIGACION);
                INode uriPredicateRdfType = graph.CreateUriNode(new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
                INode uriObjectRdfType = graph.CreateUriNode(new Uri("http://purl.org/roh#ResearchGroup"));
                Triple tripleRdfType = new Triple(uriSubject, uriPredicateRdfType, uriObjectRdfType);
                graph.Assert(tripleRdfType);

                //Title
                INode uriPredicateRohTitle = graph.CreateUriNode(new Uri("http://purl.org/roh#title"));
                INode objectRohTitle = CreateTextNode(graph, grupoInvestigacion.DESCRIPCION);
                Triple tripleTitle = new Triple(uriSubject, uriPredicateRohTitle, objectRohTitle);
                graph.Assert(tripleTitle);
                {
                    //Identificador
                    INode uriPredicateCrisIdentifier = graph.CreateUriNode(new Uri("http://purl.org/roh#crisIdentifier"));
                    Triple tripleCrisIdentifier = new Triple(uriSubject, uriPredicateCrisIdentifier, GetIdentifier(graph, "ResearchGroup", grupoInvestigacion.IDGRUPOINVESTIGACION));
                    graph.Assert(tripleCrisIdentifier);
                }

                //http://purl.org/roh#excellenceLabel
                INode uriPredicateExcellenceLabel = graph.CreateUriNode(new Uri("http://purl.org/roh#excellenceLabel"));
                INode objectExcellenceLabel = graph.CreateLiteralNode("false", new Uri("http://www.w3.org/2001/XMLSchema#boolean"));
                if (grupoInvestigacion.EXCELENCIA == "S")
                {
                    objectExcellenceLabel = graph.CreateLiteralNode("true", new Uri("http://www.w3.org/2001/XMLSchema#boolean"));
                }
                Triple tripleExcellenceLabel = new Triple(uriSubject, uriPredicateExcellenceLabel, objectExcellenceLabel);
                graph.Assert(tripleExcellenceLabel);

                //Comunidad administrativa
                if (!string.IsNullOrEmpty(grupoInvestigacion.CODUNIDADADM))
                {
                    INode uriSubjectComunidadAdm = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh#ManagementUnit", grupoInvestigacion.CODUNIDADADM);
                    INode uriObjectRdfTypeComunidadAdm = graph.CreateUriNode(new Uri("http://purl.org/roh#ManagementUnit"));
                    Triple tripleRdfTypeComunidadAdm = new Triple(uriSubjectComunidadAdm, uriPredicateRdfType, uriObjectRdfTypeComunidadAdm);
                    graph.Assert(tripleRdfTypeComunidadAdm);

                    //Title
                    INode objectRohTitleComunidadAdm = CreateTextNode(graph, grupoInvestigacion.CODUNIDADADM);
                    Triple tripleTitleComunidadAdm = new Triple(uriSubjectComunidadAdm, uriPredicateRohTitle, objectRohTitleComunidadAdm);
                    graph.Assert(tripleTitleComunidadAdm);
                    {
                        //Identificador
                        INode uriPredicateCrisIdentifier = graph.CreateUriNode(new Uri("http://purl.org/roh#crisIdentifier"));
                        Triple tripleCrisIdentifier = new Triple(uriSubjectComunidadAdm, uriPredicateCrisIdentifier, GetIdentifier(graph, "ManagementUnit", grupoInvestigacion.CODUNIDADADM));
                        graph.Assert(tripleCrisIdentifier);
                    }

                    //Relacion grupo-comunidadadm
                    INode uriPredicateParteDe = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/obo/ro#BFO_0000050"));
                    Triple tripleParteDe = new Triple(uriSubject, uriPredicateParteDe, uriSubjectComunidadAdm);
                    graph.Assert(tripleParteDe);
                }


                if (!string.IsNullOrEmpty(grupoInvestigacion.FECHACREACION) || !string.IsNullOrEmpty(grupoInvestigacion.FECHADESAPARICION))
                {
                    //Rdftype DateTimeInterval
                    INode uriSubjectDateTimeInterval = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/vivo#DateTimeInterval", "");
                    INode uriObjectRdfTypeDateTimeInterval = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#DateTimeInterval"));
                    Triple tripleRdfTypeDateTimeInterval = new Triple(uriSubjectDateTimeInterval, uriPredicateRdfType, uriObjectRdfTypeDateTimeInterval);
                    graph.Assert(tripleRdfTypeDateTimeInterval);

                    //PredicateDateTimeInterval
                    INode uriPredicateDateTimeInterval = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#dateTimeInterval"));
                    Triple tripleDateTimeInterval = new Triple(uriSubject, uriPredicateDateTimeInterval, uriSubjectDateTimeInterval);
                    graph.Assert(tripleDateTimeInterval);

                    if (!string.IsNullOrEmpty(grupoInvestigacion.FECHACREACION))
                    {
                        string anio = grupoInvestigacion.FECHACREACION.Substring(0, 4);
                        string mes = grupoInvestigacion.FECHACREACION.Substring(5, 2);
                        string dia = grupoInvestigacion.FECHACREACION.Substring(8, 2);
                        //Rdftype DateTimeValue inicio
                        INode uriSubjectDateTimeValueInicio = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/vivo#DateTimeValue", "");
                        INode uriObjectRdfTypeDateTimeValue = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#DateTimeValue"));
                        Triple tripleRdfTypeDateTimeValueInicio = new Triple(uriSubjectDateTimeValueInicio, uriPredicateRdfType, uriObjectRdfTypeDateTimeValue);
                        graph.Assert(tripleRdfTypeDateTimeValueInicio);

                        //PredicateStart
                        INode uriPredicateStart = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#start"));
                        Triple tripleStart = new Triple(uriSubjectDateTimeInterval, uriPredicateStart, uriSubjectDateTimeValueInicio);
                        graph.Assert(tripleStart);

                        //dateTime
                        INode uriPredicateDateTime = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#dateTime"));
                        INode uriObjectDateTime = graph.CreateLiteralNode($"{anio}-{mes}-{dia}T00:00:00.000+00:00", new Uri("http://www.w3.org/2001/XMLSchema#datetime"));
                        Triple tripleDateTime = new Triple(uriSubjectDateTimeValueInicio, uriPredicateDateTime, uriObjectDateTime);
                        graph.Assert(tripleDateTime);
                    }

                    if (!string.IsNullOrEmpty(grupoInvestigacion.FECHADESAPARICION))
                    {
                        string anio = grupoInvestigacion.FECHADESAPARICION.Substring(0, 4);
                        string mes = grupoInvestigacion.FECHADESAPARICION.Substring(5, 2);
                        string dia = grupoInvestigacion.FECHADESAPARICION.Substring(8, 2);
                        //Rdftype DateTimeValue fin
                        INode uriSubjectDateTimeValueFin = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/vivo#DateTimeValue", "");
                        INode uriObjectRdfTypeDateTimeValue = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#DateTimeValue"));
                        Triple tripleRdfTypeDateTimeValueInicioFin = new Triple(uriSubjectDateTimeValueFin, uriPredicateRdfType, uriObjectRdfTypeDateTimeValue);
                        graph.Assert(tripleRdfTypeDateTimeValueInicioFin);

                        //PredicateEnd
                        INode uriPredicateEnd = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#end"));
                        Triple tripleEnd = new Triple(uriSubjectDateTimeInterval, uriPredicateEnd, uriSubjectDateTimeValueFin);
                        graph.Assert(tripleEnd);

                        //dateTime
                        INode uriPredicateDateTime = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#dateTime"));
                        INode uriObjectDateTime = graph.CreateLiteralNode($"{anio}-{mes}-{dia}T00:00:00.000+00:00", new Uri("http://www.w3.org/2001/XMLSchema#datetime"));
                        Triple tripleDateTime = new Triple(uriSubjectDateTimeValueFin, uriPredicateDateTime, uriObjectDateTime);
                        graph.Assert(tripleDateTime);
                    }
                }

                graph.SaveToFile(rutaGrupoInvestigacion);
            }


            Console.WriteLine($"\rGenerando RDFs en {pRuta} {i}/{total}");
        }

        private static void GenerarProyectos(string pRuta, string pUrlUrisFactory, List<Proyecto> pProyectos, List<FechaProyecto> pFechaProyecto, List<EquipoProyecto> pEquipoProyecto, List<FechaEquipoProyecto> pFechaEquipoProyecto, List<Persona> pPersonas, List<Feature> pFeatures)
        {
            System.IO.Directory.CreateDirectory(pRuta);
            int i = 0;
            foreach (Proyecto proyecto in pProyectos)
            {
                i++;
                Console.Write($"\rGenerando RDFs en {pRuta} {i}/{pProyectos.Count}");
                string rutaProyecto = pRuta + proyecto.IDPROYECTO.Replace("|","---") + ".rdf";
                Graph graph = new Graph();

                //Rdftype
                INode uriSubject = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/vivo#Project", proyecto.IDPROYECTO);
                INode uriPredicateRdfType = graph.CreateUriNode(new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
                INode uriObjectRdfType = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#Project"));
                Triple tripleRdfType = new Triple(uriSubject, uriPredicateRdfType, uriObjectRdfType);
                graph.Assert(tripleRdfType);

                //Título
                INode uriPredicateRohTitle = graph.CreateUriNode(new Uri("http://purl.org/roh#title"));
                INode objectRohTitle = CreateTextNode(graph, proyecto.NOMBRE);
                Triple tripleTitle = new Triple(uriSubject, uriPredicateRohTitle, objectRohTitle);
                graph.Assert(tripleTitle);

                //Identificador
                INode uriPredicateCrisIdentifier = graph.CreateUriNode(new Uri("http://purl.org/roh#crisIdentifier"));
                Triple tripleCrisIdentifier = new Triple(uriSubject, uriPredicateCrisIdentifier, GetIdentifier(graph, "Project", proyecto.IDPROYECTO));
                graph.Assert(tripleCrisIdentifier);

                //LocatedIn
                if (!string.IsNullOrEmpty(proyecto.AMBITO_GEOGRAFICO))
                {
                    INode uriPredicateLocatedIn = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/geonames#locatedIn"));
                    INode uriObjectLocatedIn = graph.CreateUriNode(new Uri(pFeatures.First(x => x.ID == proyecto.AMBITO_GEOGRAFICO).Uri));
                    Triple tripleLocatedIn = new Triple(uriSubject, uriPredicateLocatedIn, uriObjectLocatedIn);
                    graph.Assert(tripleLocatedIn);
                }

                //Cogemos la última fecha
                List<FechaProyecto> fechasProyecto = pFechaProyecto.Where(x => x.IDPROYECTO == proyecto.IDPROYECTO).ToList();
                FechaProyecto fechaProyecto = fechasProyecto.OrderByDescending(x => int.Parse(x.NUMERO)).FirstOrDefault();
                if (fechaProyecto != null)
                {
                    //Rdftype DateTimeInterval
                    INode uriSubjectDateTimeInterval = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/vivo#DateTimeInterval", "");
                    INode uriObjectRdfTypeDateTimeInterval = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#DateTimeInterval"));
                    Triple tripleRdfTypeDateTimeInterval = new Triple(uriSubjectDateTimeInterval, uriPredicateRdfType, uriObjectRdfTypeDateTimeInterval);
                    graph.Assert(tripleRdfTypeDateTimeInterval);

                    //PredicateDateTimeInterval
                    INode uriPredicateDateTimeInterval = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#dateTimeInterval"));
                    Triple tripleDateTimeInterval = new Triple(uriSubject, uriPredicateDateTimeInterval, uriSubjectDateTimeInterval);
                    graph.Assert(tripleDateTimeInterval);

                    if (!string.IsNullOrEmpty(fechaProyecto.FECHAINICIOPROYECTO))
                    {
                        string anio = fechaProyecto.FECHAINICIOPROYECTO.Substring(0, 4);
                        string mes = fechaProyecto.FECHAINICIOPROYECTO.Substring(5, 2);
                        string dia = fechaProyecto.FECHAINICIOPROYECTO.Substring(8, 2);
                        //Rdftype DateTimeValue inicio
                        INode uriSubjectDateTimeValueInicio = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/vivo#DateTimeValue", "");
                        INode uriObjectRdfTypeDateTimeValue = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#DateTimeValue"));
                        Triple tripleRdfTypeDateTimeValueInicio = new Triple(uriSubjectDateTimeValueInicio, uriPredicateRdfType, uriObjectRdfTypeDateTimeValue);
                        graph.Assert(tripleRdfTypeDateTimeValueInicio);

                        //PredicateStart
                        INode uriPredicateStart = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#start"));
                        Triple tripleStart = new Triple(uriSubjectDateTimeInterval, uriPredicateStart, uriSubjectDateTimeValueInicio);
                        graph.Assert(tripleStart);

                        //dateTime
                        INode uriPredicateDateTime = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#dateTime"));
                        INode uriObjectDateTime = graph.CreateLiteralNode($"{anio}-{mes}-{dia}T00:00:00.000+00:00", new Uri("http://www.w3.org/2001/XMLSchema#datetime"));
                        Triple tripleDateTime = new Triple(uriSubjectDateTimeValueInicio, uriPredicateDateTime, uriObjectDateTime);
                        graph.Assert(tripleDateTime);
                    }

                    if (!string.IsNullOrEmpty(fechaProyecto.FECHAFINPROYECTO))
                    {
                        string anio = fechaProyecto.FECHAFINPROYECTO.Substring(0, 4);
                        string mes = fechaProyecto.FECHAFINPROYECTO.Substring(5, 2);
                        string dia = fechaProyecto.FECHAFINPROYECTO.Substring(8, 2);
                        //Rdftype DateTimeValue fin
                        INode uriSubjectDateTimeValueFin = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/vivo#DateTimeValue", "");
                        INode uriObjectRdfTypeDateTimeValue = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#DateTimeValue"));
                        Triple tripleRdfTypeDateTimeValueInicioFin = new Triple(uriSubjectDateTimeValueFin, uriPredicateRdfType, uriObjectRdfTypeDateTimeValue);
                        graph.Assert(tripleRdfTypeDateTimeValueInicioFin);

                        //PredicateEnd
                        INode uriPredicateEnd = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#end"));
                        Triple tripleEnd = new Triple(uriSubjectDateTimeInterval, uriPredicateEnd, uriSubjectDateTimeValueFin);
                        graph.Assert(tripleEnd);

                        //dateTime
                        INode uriPredicateDateTime = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#dateTime"));
                        INode uriObjectDateTime = graph.CreateLiteralNode($"{anio}-{mes}-{dia}T00:00:00.000+00:00", new Uri("http://www.w3.org/2001/XMLSchema#datetime"));
                        Triple tripleDateTime = new Triple(uriSubjectDateTimeValueFin, uriPredicateDateTime, uriObjectDateTime);
                        graph.Assert(tripleDateTime);
                    }
                }


                //Cofgemos los Equipo proyecto
                List<EquipoProyecto> equiposProyecto = pEquipoProyecto.Where(x => x.IDPROYECTO == proyecto.IDPROYECTO).ToList();
                if (equiposProyecto.Count > 0)
                {
                    foreach (EquipoProyecto equipoProyecto in equiposProyecto)
                    {
                        Persona persona = pPersonas.FirstOrDefault(x => x.IDPERSONA == equipoProyecto.IDPERSONA);
                        if (persona != null)
                        {
                            List<FechaEquipoProyecto> fechasEquipoProyecto = pFechaEquipoProyecto.Where(x => x.IDPROYECTO == equipoProyecto.IDPROYECTO && x.NUMEROCOLABORADOR == equipoProyecto.NUMEROCOLABORADOR).ToList();
                            foreach (FechaEquipoProyecto fechaEquipoProyecto in fechasEquipoProyecto)
                            {
                                INode uriSubjectRole;
                                if (fechaEquipoProyecto.CODTIPOPARTICIPACION == "IP" || fechaEquipoProyecto.CODTIPOPARTICIPACION == "IPRE")
                                {
                                    //Lider
                                    //Rdftype LeaderRole
                                    uriSubjectRole = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/vivo#LeaderRole", "");
                                    INode uriObjectRdfTypeLeaderRole = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#LeaderRole"));
                                    Triple tripleRdfTypeLeaderRole = new Triple(uriSubjectRole, uriPredicateRdfType, uriObjectRdfTypeLeaderRole);
                                    graph.Assert(tripleRdfTypeLeaderRole);
                                }
                                else
                                {
                                    //Miembro
                                    //Rdftype MemberRole
                                    uriSubjectRole = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/vivo#MemberRole", "");
                                    INode uriObjectRdfTypeMemberRole = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#MemberRole"));
                                    Triple tripleRdfTypeMemberRole = new Triple(uriSubjectRole, uriPredicateRdfType, uriObjectRdfTypeMemberRole);
                                    graph.Assert(tripleRdfTypeMemberRole);
                                }

                                //Rdftype
                                INode uriSubjectPersona = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/foaf#Person", persona.IDPERSONA);
                                INode uriObjectRdfTypePersona = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/foaf#Person"));
                                Triple tripleRdfTypePersona = new Triple(uriSubjectPersona, uriPredicateRdfType, uriObjectRdfTypePersona);
                                graph.Assert(tripleRdfTypePersona);

                                //Nombre
                                INode uriPredicateFoafName = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/foaf#name"));
                                INode objectFoafName = CreateTextNode(graph, persona.NOMBRE);
                                Triple tripleNombre = new Triple(uriSubjectPersona, uriPredicateFoafName, objectFoafName);
                                graph.Assert(tripleNombre);

                                //Identificador                                
                                Triple tripleCrisIdentifierPerson = new Triple(uriSubjectPersona, uriPredicateCrisIdentifier, GetIdentifier(graph, "Person", persona.IDPERSONA));
                                graph.Assert(tripleCrisIdentifierPerson);

                                //roleOf
                                INode uriPredicateRoleOf = graph.CreateUriNode(new Uri("http://purl.org/roh#roleOf"));
                                Triple tripleRoleOf = new Triple(uriSubjectRole, uriPredicateRoleOf, uriSubjectPersona);
                                graph.Assert(tripleRoleOf);

                                //relates
                                INode uriPredicateRelates = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#relates"));
                                Triple triplerelates = new Triple(uriSubject, uriPredicateRelates, uriSubjectRole);
                                graph.Assert(triplerelates);

                                if (!string.IsNullOrEmpty(fechaEquipoProyecto.FECHAINICIOPERIODO) || !string.IsNullOrEmpty(fechaEquipoProyecto.FECHAFINPERIODO))
                                {
                                    //Rdftype DateTimeInterval
                                    INode uriSubjectDateTimeInterval = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/vivo#DateTimeInterval", "");
                                    INode uriObjectRdfTypeDateTimeInterval = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#DateTimeInterval"));
                                    Triple tripleRdfTypeDateTimeInterval = new Triple(uriSubjectDateTimeInterval, uriPredicateRdfType, uriObjectRdfTypeDateTimeInterval);
                                    graph.Assert(tripleRdfTypeDateTimeInterval);

                                    //PredicateDateTimeInterval
                                    INode uriPredicateDateTimeInterval = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#dateTimeInterval"));
                                    Triple tripleDateTimeInterval = new Triple(uriSubjectRole, uriPredicateDateTimeInterval, uriSubjectDateTimeInterval);
                                    graph.Assert(tripleDateTimeInterval);

                                    if (!string.IsNullOrEmpty(fechaEquipoProyecto.FECHAINICIOPERIODO))
                                    {
                                        string anio = fechaEquipoProyecto.FECHAINICIOPERIODO.Substring(0, 4);
                                        string mes = fechaEquipoProyecto.FECHAINICIOPERIODO.Substring(5, 2);
                                        string dia = fechaEquipoProyecto.FECHAINICIOPERIODO.Substring(8, 2);
                                        //Rdftype DateTimeValue inicio
                                        INode uriSubjectDateTimeValueInicio = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/vivo#DateTimeValue", "");
                                        INode uriObjectRdfTypeDateTimeValue = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#DateTimeValue"));
                                        Triple tripleRdfTypeDateTimeValueInicio = new Triple(uriSubjectDateTimeValueInicio, uriPredicateRdfType, uriObjectRdfTypeDateTimeValue);
                                        graph.Assert(tripleRdfTypeDateTimeValueInicio);

                                        //PredicateStart
                                        INode uriPredicateStart = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#start"));
                                        Triple tripleStart = new Triple(uriSubjectDateTimeInterval, uriPredicateStart, uriSubjectDateTimeValueInicio);
                                        graph.Assert(tripleStart);

                                        //dateTime
                                        INode uriPredicateDateTime = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#dateTime"));
                                        INode uriObjectDateTime = graph.CreateLiteralNode($"{anio}-{mes}-{dia}T00:00:00.000+00:00", new Uri("http://www.w3.org/2001/XMLSchema#datetime"));
                                        Triple tripleDateTime = new Triple(uriSubjectDateTimeValueInicio, uriPredicateDateTime, uriObjectDateTime);
                                        graph.Assert(tripleDateTime);
                                    }

                                    if (!string.IsNullOrEmpty(fechaEquipoProyecto.FECHAFINPERIODO))
                                    {
                                        string anio = fechaEquipoProyecto.FECHAFINPERIODO.Substring(0, 4);
                                        string mes = fechaEquipoProyecto.FECHAFINPERIODO.Substring(5, 2);
                                        string dia = fechaEquipoProyecto.FECHAFINPERIODO.Substring(8, 2);
                                        //Rdftype DateTimeValue fin
                                        INode uriSubjectDateTimeValueFin = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/vivo#DateTimeValue", "");
                                        INode uriObjectRdfTypeDateTimeValue = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#DateTimeValue"));
                                        Triple tripleRdfTypeDateTimeValueInicioFin = new Triple(uriSubjectDateTimeValueFin, uriPredicateRdfType, uriObjectRdfTypeDateTimeValue);
                                        graph.Assert(tripleRdfTypeDateTimeValueInicioFin);

                                        //PredicateEnd
                                        INode uriPredicateEnd = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#end"));
                                        Triple tripleEnd = new Triple(uriSubjectDateTimeInterval, uriPredicateEnd, uriSubjectDateTimeValueFin);
                                        graph.Assert(tripleEnd);

                                        //dateTime
                                        INode uriPredicateDateTime = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#dateTime"));
                                        INode uriObjectDateTime = graph.CreateLiteralNode($"{anio}-{mes}-{dia}T00:00:00.000+00:00", new Uri("http://www.w3.org/2001/XMLSchema#datetime"));
                                        Triple tripleDateTime = new Triple(uriSubjectDateTimeValueFin, uriPredicateDateTime, uriObjectDateTime);
                                        graph.Assert(tripleDateTime);
                                    }

                                }


                            }
                        }
                    }
                }
                graph.SaveToFile(rutaProyecto);
            }
            Console.WriteLine($"\rGenerando RDFs en {pRuta} {i}/{pProyectos.Count}");
        }

        private static void GenerarActividades(string pRuta, string pUrlUrisFactory, List<Congreso> pCongresos, List<Exposicion> pExposiciones, List<AutorCongreso> pAutoresCongreso, List<AutorExposicion> pAutoresExposicion, List<Persona> pPersonas)
        {
            System.IO.Directory.CreateDirectory(pRuta);
            int i = 0;
            int total = pCongresos.Count + pExposiciones.Count;
            foreach (Congreso congreso in pCongresos)
            {
                i++;
                Console.Write($"\rGenerando RDFs en {pRuta} {i}/{total}");
                string rutaCongreso = pRuta + "Congreso_" + congreso.CONG_NUMERO + ".rdf";
                Graph graph = new Graph();
                //Rdftype
                INode uriSubject = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/vivo#Presentation", congreso.CONG_NUMERO);
                INode uriPredicateRdfType = graph.CreateUriNode(new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
                INode uriObjectRdfType = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#Presentation"));
                Triple tripleRdfType = new Triple(uriSubject, uriPredicateRdfType, uriObjectRdfType);
                graph.Assert(tripleRdfType);

                //Title
                INode uriPredicateRohTitle = graph.CreateUriNode(new Uri("http://purl.org/roh#title"));
                INode objectRohTitle = CreateTextNode(graph, congreso.TITULO_CONTRIBUCION + " - " + congreso.TITULO_CONGRESO);
                Triple tripleTitle = new Triple(uriSubject, uriPredicateRohTitle, objectRohTitle);
                graph.Assert(tripleTitle);

                //Locality
                if (!string.IsNullOrEmpty(congreso.LUGAR_CELEBRACION))
                {
                    INode uriPredicateLocality = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vcard#locality"));
                    INode objectLocality = CreateTextNode(graph, congreso.LUGAR_CELEBRACION);
                    Triple tripleLocality = new Triple(uriSubject, uriPredicateLocality, objectLocality);
                    graph.Assert(tripleLocality);
                }

                //Fecha 
                if (!string.IsNullOrEmpty(congreso.FECHA_CELEBRACION))
                {
                    //Rdftype DateTimeInterval
                    INode uriSubjectDateTimeInterval = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/vivo#DateTimeInterval", "");
                    INode uriObjectRdfTypeDateTimeInterval = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#DateTimeInterval"));
                    Triple tripleRdfTypeDateTimeInterval = new Triple(uriSubjectDateTimeInterval, uriPredicateRdfType, uriObjectRdfTypeDateTimeInterval);
                    graph.Assert(tripleRdfTypeDateTimeInterval);

                    //PredicateDateTimeInterval
                    INode uriPredicateDateTimeInterval = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#dateTimeInterval"));
                    Triple tripleDateTimeInterval = new Triple(uriSubject, uriPredicateDateTimeInterval, uriSubjectDateTimeInterval);
                    graph.Assert(tripleDateTimeInterval);

                    {
                        string anio = congreso.FECHA_CELEBRACION.Substring(0, 4);
                        string mes = congreso.FECHA_CELEBRACION.Substring(5, 2);
                        string dia = congreso.FECHA_CELEBRACION.Substring(8, 2);
                        string h = congreso.FECHA_CELEBRACION.Substring(11, 2);
                        string m = congreso.FECHA_CELEBRACION.Substring(14, 2);
                        string s = congreso.FECHA_CELEBRACION.Substring(17, 2);
                        //Rdftype DateTimeValue inicio
                        INode uriSubjectDateTimeValueInicio = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/vivo#DateTimeValue", "");
                        INode uriObjectRdfTypeDateTimeValue = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#DateTimeValue"));
                        Triple tripleRdfTypeDateTimeValueInicio = new Triple(uriSubjectDateTimeValueInicio, uriPredicateRdfType, uriObjectRdfTypeDateTimeValue);
                        graph.Assert(tripleRdfTypeDateTimeValueInicio);

                        //PredicateStart
                        INode uriPredicateStart = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#start"));
                        Triple tripleStart = new Triple(uriSubjectDateTimeInterval, uriPredicateStart, uriSubjectDateTimeValueInicio);
                        graph.Assert(tripleStart);

                        //dateTime
                        INode uriPredicateDateTime = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#dateTime"));
                        INode uriObjectDateTime = graph.CreateLiteralNode($"{anio}-{mes}-{dia}T{h}:{m}:{s}.000+00:00", new Uri("http://www.w3.org/2001/XMLSchema#datetime"));
                        Triple tripleDateTime = new Triple(uriSubjectDateTimeValueInicio, uriPredicateDateTime, uriObjectDateTime);
                        graph.Assert(tripleDateTime);
                    }

                    {
                        string anio = congreso.FECHA_CELEBRACION.Substring(0, 4);
                        string mes = congreso.FECHA_CELEBRACION.Substring(5, 2);
                        string dia = congreso.FECHA_CELEBRACION.Substring(8, 2);
                        string h = congreso.FECHA_CELEBRACION.Substring(11, 2);
                        string m = congreso.FECHA_CELEBRACION.Substring(14, 2);
                        string s = congreso.FECHA_CELEBRACION.Substring(17, 2);
                        //Rdftype DateTimeValue fin
                        INode uriSubjectDateTimeValueFin = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/vivo#DateTimeValue", "");
                        INode uriObjectRdfTypeDateTimeValue = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#DateTimeValue"));
                        Triple tripleRdfTypeDateTimeValueInicioFin = new Triple(uriSubjectDateTimeValueFin, uriPredicateRdfType, uriObjectRdfTypeDateTimeValue);
                        graph.Assert(tripleRdfTypeDateTimeValueInicioFin);

                        //PredicateEnd
                        INode uriPredicateEnd = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#end"));
                        Triple tripleEnd = new Triple(uriSubjectDateTimeInterval, uriPredicateEnd, uriSubjectDateTimeValueFin);
                        graph.Assert(tripleEnd);

                        //dateTime
                        INode uriPredicateDateTime = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#dateTime"));
                        INode uriObjectDateTime = graph.CreateLiteralNode($"{anio}-{mes}-{dia}T{h}:{m}:{s}.000+00:00", new Uri("http://www.w3.org/2001/XMLSchema#datetime"));
                        Triple tripleDateTime = new Triple(uriSubjectDateTimeValueFin, uriPredicateDateTime, uriObjectDateTime);
                        graph.Assert(tripleDateTime);
                    }
                }

                //Identificador
                {
                    INode uriPredicateCrisIdentifier = graph.CreateUriNode(new Uri("http://purl.org/roh#crisIdentifier"));
                    Triple tripleCrisIdentifier = new Triple(uriSubject, uriPredicateCrisIdentifier, GetIdentifier(graph, "Presentation", congreso.CONG_NUMERO));
                    graph.Assert(tripleCrisIdentifier);
                }

                List<AutorCongreso> autores = pAutoresCongreso.Where(x => x.CONG_NUMERO == congreso.CONG_NUMERO).ToList();
                if (autores.Count > 0)
                {
                    foreach (AutorCongreso autorCongreso in autores)
                    {
                        Persona persona = pPersonas.FirstOrDefault(x => x.IDPERSONA == autorCongreso.IDPERSONA);
                        if (persona != null)
                        {
                            //Rdftype
                            INode uriSubjectPersona = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/foaf#Person", persona.IDPERSONA);
                            INode uriObjectRdfTypePersona = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/foaf#Person"));
                            Triple tripleRdfTypePersona = new Triple(uriSubjectPersona, uriPredicateRdfType, uriObjectRdfTypePersona);
                            graph.Assert(tripleRdfTypePersona);

                            //Nombre
                            INode uriPredicateFoafName = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/foaf#name"));
                            INode objectFoafName = CreateTextNode(graph, persona.NOMBRE);
                            Triple tripleNombre = new Triple(uriSubjectPersona, uriPredicateFoafName, objectFoafName);
                            graph.Assert(tripleNombre);

                            //Identificador
                            INode uriPredicateCrisIdentifier = graph.CreateUriNode(new Uri("http://purl.org/roh#crisIdentifier"));
                            Triple tripleCrisIdentifier = new Triple(uriSubjectPersona, uriPredicateCrisIdentifier, GetIdentifier(graph, "Person", persona.IDPERSONA));
                            graph.Assert(tripleCrisIdentifier);

                            //Participated by
                            INode uriPredicateParticipatedBy = graph.CreateUriNode(new Uri("http://purl.org/roh#participatedBy"));
                            Triple tripleParticipatedBy = new Triple(uriSubject, uriPredicateParticipatedBy, uriSubjectPersona);
                            graph.Assert(tripleParticipatedBy);
                        }
                    }
                }
                graph.SaveToFile(rutaCongreso);
            }

            foreach (Exposicion exposicion in pExposiciones)
            {
                i++;
                Console.Write($"\rGenerando RDFs en {pRuta} {i}/{total}");
                string rutaExposicion = pRuta + "Exposicion_" + exposicion.CODIGO + ".rdf";
                Graph graph = new Graph();
                //Rdftype
                INode uriSubject = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/vivo#Exhibit", exposicion.CODIGO);
                INode uriPredicateRdfType = graph.CreateUriNode(new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
                INode uriObjectRdfType = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#Exhibit"));
                Triple tripleRdfType = new Triple(uriSubject, uriPredicateRdfType, uriObjectRdfType);
                graph.Assert(tripleRdfType);

                //Title
                INode uriPredicateRohTitle = graph.CreateUriNode(new Uri("http://purl.org/roh#title"));
                INode objectRohTitle = CreateTextNode(graph, exposicion.NOMBRE);
                Triple tripleTitle = new Triple(uriSubject, uriPredicateRohTitle, objectRohTitle);
                graph.Assert(tripleTitle);

                //Locality
                if (!string.IsNullOrEmpty(exposicion.LUGAR))
                {
                    //http://purl.org/roh/mirror/vcard#locality
                    INode uriPredicateLocality = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vcard#locality"));
                    INode objectLocality = CreateTextNode(graph, exposicion.LUGAR);
                    Triple tripleLocality = new Triple(uriSubject, uriPredicateLocality, objectLocality);
                    graph.Assert(tripleLocality);
                }

                //Fecha 
                if (!string.IsNullOrEmpty(exposicion.FECHA))
                {
                    //Rdftype DateTimeInterval
                    INode uriSubjectDateTimeInterval = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/vivo#DateTimeInterval", "");
                    INode uriObjectRdfTypeDateTimeInterval = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#DateTimeInterval"));
                    Triple tripleRdfTypeDateTimeInterval = new Triple(uriSubjectDateTimeInterval, uriPredicateRdfType, uriObjectRdfTypeDateTimeInterval);
                    graph.Assert(tripleRdfTypeDateTimeInterval);

                    //PredicateDateTimeInterval
                    INode uriPredicateDateTimeInterval = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#dateTimeInterval"));
                    Triple tripleDateTimeInterval = new Triple(uriSubject, uriPredicateDateTimeInterval, uriSubjectDateTimeInterval);
                    graph.Assert(tripleDateTimeInterval);

                    {
                        string anio = exposicion.FECHA.Substring(0, 4);
                        string mes = exposicion.FECHA.Substring(5, 2);
                        string dia = exposicion.FECHA.Substring(8, 2);
                        string h = exposicion.FECHA.Substring(11, 2);
                        string m = exposicion.FECHA.Substring(14, 2);
                        string s = exposicion.FECHA.Substring(17, 2);
                        //Rdftype DateTimeValue inicio
                        INode uriSubjectDateTimeValueInicio = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/vivo#DateTimeValue", "");
                        INode uriObjectRdfTypeDateTimeValue = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#DateTimeValue"));
                        Triple tripleRdfTypeDateTimeValueInicio = new Triple(uriSubjectDateTimeValueInicio, uriPredicateRdfType, uriObjectRdfTypeDateTimeValue);
                        graph.Assert(tripleRdfTypeDateTimeValueInicio);

                        //PredicateStart
                        INode uriPredicateStart = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#start"));
                        Triple tripleStart = new Triple(uriSubjectDateTimeInterval, uriPredicateStart, uriSubjectDateTimeValueInicio);
                        graph.Assert(tripleStart);

                        //dateTime
                        INode uriPredicateDateTime = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#dateTime"));
                        INode uriObjectDateTime = graph.CreateLiteralNode($"{anio}-{mes}-{dia}T{h}:{m}:{s}.000+00:00", new Uri("http://www.w3.org/2001/XMLSchema#datetime"));
                        Triple tripleDateTime = new Triple(uriSubjectDateTimeValueInicio, uriPredicateDateTime, uriObjectDateTime);
                        graph.Assert(tripleDateTime);
                    }

                    {
                        string anio = exposicion.FECHA.Substring(0, 4);
                        string mes = exposicion.FECHA.Substring(5, 2);
                        string dia = exposicion.FECHA.Substring(8, 2);
                        string h = exposicion.FECHA.Substring(11, 2);
                        string m = exposicion.FECHA.Substring(14, 2);
                        string s = exposicion.FECHA.Substring(17, 2);
                        //Rdftype DateTimeValue fin
                        INode uriSubjectDateTimeValueFin = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/vivo#DateTimeValue", "");
                        INode uriObjectRdfTypeDateTimeValue = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#DateTimeValue"));
                        Triple tripleRdfTypeDateTimeValueInicioFin = new Triple(uriSubjectDateTimeValueFin, uriPredicateRdfType, uriObjectRdfTypeDateTimeValue);
                        graph.Assert(tripleRdfTypeDateTimeValueInicioFin);

                        //PredicateEnd
                        INode uriPredicateEnd = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#end"));
                        Triple tripleEnd = new Triple(uriSubjectDateTimeInterval, uriPredicateEnd, uriSubjectDateTimeValueFin);
                        graph.Assert(tripleEnd);

                        //dateTime
                        INode uriPredicateDateTime = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/vivo#dateTime"));
                        INode uriObjectDateTime = graph.CreateLiteralNode($"{anio}-{mes}-{dia}T{h}:{m}:{s}.000+00:00", new Uri("http://www.w3.org/2001/XMLSchema#datetime"));
                        Triple tripleDateTime = new Triple(uriSubjectDateTimeValueFin, uriPredicateDateTime, uriObjectDateTime);
                        graph.Assert(tripleDateTime);
                    }
                }

                //Identificador
                {
                    INode uriPredicateCrisIdentifier = graph.CreateUriNode(new Uri("http://purl.org/roh#crisIdentifier"));
                    Triple tripleCrisIdentifier = new Triple(uriSubject, uriPredicateCrisIdentifier, GetIdentifier(graph, "Exhibit", exposicion.CODIGO));
                    graph.Assert(tripleCrisIdentifier);
                }

                List<AutorExposicion> autores = pAutoresExposicion.Where(x => x.EXPO_CODIGO == exposicion.CODIGO).ToList();
                if (autores.Count > 0)
                {
                    foreach (AutorExposicion autorExposicion in autores)
                    {
                        Persona persona = pPersonas.FirstOrDefault(x => x.IDPERSONA == autorExposicion.IDPERSONA);
                        if (persona != null)
                        {
                            //Rdftype
                            INode uriSubjectPersona = GetUri(pUrlUrisFactory, graph, "http://purl.org/roh/mirror/foaf#Person", persona.IDPERSONA);
                            INode uriObjectRdfTypePersona = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/foaf#Person"));
                            Triple tripleRdfTypePersona = new Triple(uriSubjectPersona, uriPredicateRdfType, uriObjectRdfTypePersona);
                            graph.Assert(tripleRdfTypePersona);

                            //Nombre
                            INode uriPredicateFoafName = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/foaf#name"));
                            INode objectFoafName = CreateTextNode(graph, persona.NOMBRE);
                            Triple tripleNombre = new Triple(uriSubjectPersona, uriPredicateFoafName, objectFoafName);
                            graph.Assert(tripleNombre);

                            //Identificador
                            INode uriPredicateCrisIdentifier = graph.CreateUriNode(new Uri("http://purl.org/roh#crisIdentifier"));
                            Triple tripleCrisIdentifier = new Triple(uriSubjectPersona, uriPredicateCrisIdentifier, GetIdentifier(graph, "Person", persona.IDPERSONA));
                            graph.Assert(tripleCrisIdentifier);

                            //Participated by
                            INode uriPredicateParticipatedBy = graph.CreateUriNode(new Uri("http://purl.org/roh#participatedBy"));
                            Triple tripleParticipatedBy = new Triple(uriSubject, uriPredicateParticipatedBy, uriSubjectPersona);
                            graph.Assert(tripleParticipatedBy);
                        }
                    }
                }
                graph.SaveToFile(rutaExposicion);
            }
            Console.WriteLine($"\rGenerando RDFs en {pRuta} {i}/{total}");
        }

        private static void GenerarAmbitosProyectos(string pRuta, List<Feature> pFeatures)
        {
            System.IO.Directory.CreateDirectory(pRuta);
            int i = 0;
            int total = pFeatures.Count;
            foreach (Feature feature in pFeatures)
            {
                i++;
                Console.Write($"\rGenerando RDFs en {pRuta} {i}/{total}");
                string rutaCongreso = pRuta + "Feature_" + feature.ID + ".rdf";
                Graph graph = new Graph();
                //Rdftype
                INode uriSubject = graph.CreateUriNode(new Uri(feature.Uri));
                INode uriPredicateRdfType = graph.CreateUriNode(new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
                INode uriObjectRdfType = graph.CreateUriNode(new Uri("http://purl.org/roh/mirror/geonames#Feature"));
                Triple tripleRdfType = new Triple(uriSubject, uriPredicateRdfType, uriObjectRdfType);
                graph.Assert(tripleRdfType);

                //Title
                INode uriPredicateRohTitle = graph.CreateUriNode(new Uri("http://purl.org/roh#title"));
                INode objectRohTitle = CreateTextNode(graph, feature.Name);
                Triple tripleTitle = new Triple(uriSubject, uriPredicateRohTitle, objectRohTitle);
                graph.Assert(tripleTitle);
                graph.SaveToFile(rutaCongreso);
            }
            Console.WriteLine($"\rGenerando RDFs en {pRuta} {i}/{total}");
        }

        private static Dictionary<string, string> dicUrls = new Dictionary<string, string>();

        private static INode GetUri(string pUriUrisFactory, Graph pGraph, string pClass, string pID)
        {
            if (!string.IsNullOrEmpty(pID) && dicUrls.ContainsKey(pClass + pID))
            {
                string uri = dicUrls[pClass + pID];
                if (uri.StartsWith("http"))
                {
                    return pGraph.CreateUriNode(new Uri(uri));
                }
            }
            bool error = true;
            int numError = 0;
            while (error)
            {
                try
                {
                    WebRequest request = WebRequest.Create($"{pUriUrisFactory}?resource_class={HttpUtility.UrlEncode(pClass)}&identifier={pID}");
                    WebResponse response = request.GetResponse();
                    Stream data = response.GetResponseStream();
                    string uri = String.Empty;
                    using (StreamReader sr = new StreamReader(data))
                    {
                        uri = sr.ReadToEnd();
                    }
                    data.Close();
                    response.Close();
                    if (!string.IsNullOrEmpty(uri))
                    {
                        if (!string.IsNullOrEmpty(pID) && uri.StartsWith("http"))
                        {
                            dicUrls[pClass + pID] = uri;
                            return pGraph.CreateUriNode(new Uri(uri));
                        }
                        else
                        {
                            return pGraph.CreateBlankNode(uri);
                        }
                    }
                    error = false;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(1000);
                    numError++;
                    if (numError > 50)
                    {
                        throw ex;
                    }
                }
            }
            throw new Exception();
        }

        private static INode GetIdentifier(Graph pGraph, string pType, string pIdentifier)
        {
            return pGraph.CreateLiteralNode(pType + "-" + pIdentifier, new Uri("http://www.w3.org/2001/XMLSchema#string"));
        }

        private static INode CreateTextNode(Graph pGraph, string pText)
        {
            string escapeText = System.Text.RegularExpressions.Regex.Replace(pText, @"[\u000B-\u000B]", " ").Trim();
            return pGraph.CreateLiteralNode(escapeText, new Uri("http://www.w3.org/2001/XMLSchema#string"));
        }
        #endregion

        #region Carga RDFs

        #endregion 

        #region Auxiliares
        private static Graph CloneGraph(Graph pGraph)
        {
            RdfXmlWriter rdfxmlwriter = new RdfXmlWriter();
            string rdf = VDS.RDF.Writing.StringWriter.Write(pGraph, rdfxmlwriter);
            Graph graphClone = new Graph();
            graphClone.LoadFromString(rdf, new RdfXmlParser());
            return graphClone;
        }
        #endregion
    }
}
