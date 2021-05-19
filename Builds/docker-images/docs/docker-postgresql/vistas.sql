-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Versión del servidor:         PostgreSQL 9.2.24 on x86_64-redhat-linux-gnu, compiled by gcc (GCC) 4.8.5 20150623 (Red Hat 4.8.5-39), 64-bit
-- SO del servidor:              
-- HeidiSQL Versión:             11.2.0.6213
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES  */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

-- Volcando datos para la tabla public.Page: 13 rows
/*!40000 ALTER TABLE "Page" DISABLE KEYS */;
INSERT INTO "Page" ("PageID", "Route", "Content", "LastModified", "LastRequested") VALUES
	('f691bde8-30b3-4488-a6c6-7b0689adc36c', '/public/pruebas/sparql', '@model ApiCargaWebInterface.ViewModels.CmsDataViewModel
@using CsvHelper
@using System.Globalization
@using System.IO
@using System.Text
@{
    Layout = "_Layout";
    ViewData["BodyClass"] = "fichaRecurso";
	string result = Model.Results.FirstOrDefault();
    byte[] byteArray = Encoding.UTF8.GetBytes(result);
    MemoryStream stream = new MemoryStream(byteArray);
    var csvReader = new CsvReader(new StreamReader(stream), CultureInfo.InvariantCulture);
    var records = csvReader.GetRecords<PruebaSparql>();
}
@*<% sparql
    select ?type count(?s) as ?count
    where
    {
     ?s rdf:type ?type
     }
     group by ?type
    /%>*@
<div class="row">
    <div class="col col-12 col-lg-12 col-contenido">
        <div class="wrapCol">
            <div class="row">
                <div class="col col-12 col-lg-12 col-contenido">
                    <div class="wrapCol">
                        <h1>N�mero de entidades por tipo</h1>
                        <div class="contenido">
                            <div class="grupo grupo-descripcion">
                                <div class="items tabla">
                                    <div class="cabecera">
                                        <div class="columna">
                                            <p>Type</p>
                                        </div>
                                        <div class="columna">
                                            <p>Count</p>
                                        </div>
                                    </div>
                                    @foreach (var fila in records)
                                    {
                                        <div class="fila">
                                            <div class="columna principal">
                                                <p>@fila.type</p>
                                            </div>
                                            <div class="columna">
                                                <p>@fila.count</p>
                                            </div>
                                        </div>
                                    }
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
@functions{
    public class PruebaSparql
    {
        public string type { get; set; }
        public int count { get; set; }
    }
}
', '2020-12-12 18:08:45.394005', '2021-02-02 18:00:44.464845'),
	('f1b1642c-3fa5-44fe-8f57-3f26571d3730', '/public/GraficoObjeto', '@{ Layout = "_Layout";
    ViewData["Title"] = "Test de Gráficos";
    ViewData["link_scripts"] = "<script src=\"https://cdnjs.cloudflare.com/ajax/libs/d3/4.7.2/d3.min.js\"></script>";
    ViewData["BodyClass"] = "dashboard";
    List<Grafica> listaValores = new List<Grafica>();
    Grafica g1 = new Grafica();
    g1.Label = "Europeo";
    g1.Value = 45;
    g1.Color = "#f30000";
    Grafica g2 = new Grafica();
    g2.Label = "Nacional";
    g2.Value = 29;
    g2.Color = "#0600f3";
    Grafica g3 = new Grafica();
    g3.Label = "Regional";
    g3.Value = 8;
    g3.Color = "#00b109";
    Grafica g4 = new Grafica();
    g4.Label = "Convocatoria";
    g4.Value = 8;
    g4.Color = "#14e4b4";
    Grafica g5 = new Grafica();
    g5.Label = "Otros";
    g5.Value = 8;
    g5.Color = "#0fe7fb";
    listaValores.Add(g1);
    listaValores.Add(g2);
    listaValores.Add(g3);
    listaValores.Add(g4);
    listaValores.Add(g5);
    string content = "";
    foreach (Grafica grafica in listaValores)
    {
        content += "{\"label\" : \""+grafica.Label+ "\",\"value\": " + grafica.Value + ",\"color\": \"" + grafica.Color + "\"},";
    }
    content = content.Remove(content.Length - 1);
}

<div class="row">
    <div class="col col-12">
        <h2>
            <a href="javascript: void(0);">Objeto c#</a>
        </h2>
        <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam eleifend, ex elementum molestie varius, purus erat imperdiet nunc, in scelerisque ipsum ante ac risus. <a href="javascript: void(0);">Praesent et metus id eros rhoncus maximus nec</a> porta ante. In posuere nunc neque, non maximus ipsum placerat quis.</p>
        <div class="entidades">
            <div>
                <p>
                    <a href="javascript: void(0);">
                        <span class="numResultados">256</span>
                        Papers
                    </a>
                </p>
                <p>
                    <a href="javascript: void(0);">
                        <span class="numResultados">3.328</span>
                        Conferencias
                    </a>
                </p>
                <p>
                    <a href="javascript: void(0);">
                        <span class="numResultados">110</span>
                        Proyectos
                    </a>
                </p>
            </div>
        </div>
    </div>
</div>
<div class="row">
    <div class="col col-12">
        <h2>
            <a href="javascript: void(0);">Proyectos por año</a>
        </h2>
        <div class="graficos">
            <img src="http://herc-as-front-desa.atica.um.es/imagenes/barchart-demo.svg" alt="">
        </div>
    </div>
</div>
<div class="row">
    <div class="col col-12">
        <h2>
            <a href="javascript: void(0);">Proyectos</a>
        </h2>
        <div class="graficos">
            <div id="pieChart"></div>
            <div id="pieChart2"></div>
        </div>
    </div>
</div>
<div class="row">
    <div class="col col-12">
        <h2>
            <a href="javascript: void(0);">Investigador</a>
        </h2>
        <p>Mapa y grafo de realizaciones</p>
        <img src="http://herc-as-front-desa.atica.um.es/imagenes/grafo.png" alt="">
    </div>
</div>

<script src="https://herc-as-front-desa.atica.um.es/carga-web/theme/static/d3pie.min.js"></script>
<script>
    var pie = new d3pie("pieChart", {
        "header": {
            "title": {
                "text": "Por convocatoria",
                "fontSize": 20,
                "font": "open sans"
            },
            "subtitle": {
                "text": ".",
                "color": "#ffffff",
                "fontSize": 12,
                "font": "open sans"
            },
            "titleSubtitlePadding": 9
        },
        "footer": {
            "color": "#999999",
            "fontSize": 10,
            "font": "open sans",
            "location": "bottom-left"
        },
        "size": {
            "canvasHeight": 260,
            "canvasWidth": 380,
            "pieInnerRadius": "37%",
            "pieOuterRadius": "92%"
        },
        "data": {
            "sortOrder": "value-desc",
            "content": [ @Html.Raw(content)
            ]
        },
        "labels": {
            "outer": {
                "pieDistance": 20
            },
            "inner": {
                "hideWhenLessThanPercentage": 3
            },
            "mainLabel": {
                "fontSize": 11
            },
            "percentage": {
                "color": "#ffffff",
                "decimalPlaces": 0
            },
            "value": {
                "color": "#adadad",
                "fontSize": 11
            },
            "lines": {
                "enabled": true
            },
            "truncation": {
                "enabled": true,
                "truncateLength": 12
            }
        },
        "tooltips": {
            "enabled": true,
            "type": "placeholder",
            "string": "{label}: {value}, {percentage}%"
        },
        "effects": {
            "pullOutSegmentOnClick": {
                "effect": "none",
                "speed": 400,
                "size": 8
            }
        },
        "misc": {
            "gradient": {
                "enabled": true,
                "percentage": 100
            }
        },
        "callbacks": {
            onClickSegment: function (info) {
                console.log("click:", info);
            }
        }
    });

    var pie = new d3pie("pieChart2", {
        "header": {
            "title": {
                "text": "Por Área/Disciplina",
                "fontSize": 20,
                "font": "open sans"
            },
            "subtitle": {
                "text": ".",
                "color": "#ffffff",
                "fontSize": 12,
                "font": "open sans"
            },
            "titleSubtitlePadding": 9
        },
        "footer": {
            "color": "#999999",
            "fontSize": 10,
            "font": "open sans",
            "location": "bottom-left"
        },
        "size": {
            "canvasHeight": 260,
            "canvasWidth": 380,
            "pieInnerRadius": "37%",
            "pieOuterRadius": "92%"
        },
        "data": {
            "sortOrder": "value-desc",
            "content": [
                {
                    "label": "Química",
                    "value": 40,
                    "color": "#f30000"
                },
                {
                    "label": "Biología",
                    "value": 34,
                    "color": "#0600f3"
                },
                {
                    "label": "Medio ambiente",
                    "value": 12,
                    "color": "#00b109"
                },
                {
                    "label": "Economía circular",
                    "value": 8,
                    "color": "#14e4b4"
                },
                {
                    "label": "Otros",
                    "value": 4,
                    "color": "#0fe7fb"
                }
            ]
        },
        "labels": {
            "outer": {
                "pieDistance": 20
            },
            "inner": {
                "hideWhenLessThanPercentage": 3
            },
            "mainLabel": {
                "fontSize": 11
            },
            "percentage": {
                "color": "#ffffff",
                "decimalPlaces": 0
            },
            "value": {
                "color": "#adadad",
                "fontSize": 11
            },
            "lines": {
                "enabled": true
            },
            "truncation": {
                "enabled": true,
                "truncateLength": 12
            }
        },
        "tooltips": {
            "enabled": true,
            "type": "placeholder",
            "string": "{label}: {value}, {percentage}%"
        },
        "effects": {
            "pullOutSegmentOnClick": {
                "effect": "none",
                "speed": 400,
                "size": 8
            }
        },
        "misc": {
            "gradient": {
                "enabled": true,
                "percentage": 100
            }
        },
        "callbacks": {
            onClickSegment: function (info) {
                console.log("click:", info);
            }
        }
    });</script>
@functions{ public class Grafica
            {
                public string Label { get; set; }
                public int Value { get; set; }
                public string Color { get; set; }
            } }
', '2021-01-26 14:45:18.539622', '2021-02-04 15:02:44.516124'),
	('c436eaee-d1a6-47bb-befd-605648e102c1', '/public/gnossdeustobackend/linked-data-server-constraints', '@model ApiCargaWebInterface.ViewModels.CmsDataViewModel
@using CsvHelper
@using System.Globalization
@using System.IO
@using System.Text
@{
    Layout = "_Layout";
    ViewData["BodyClass"] = "tipo-documento";
	ViewData["Title"] = "ASIO Linked Data Server Constraints";
	ViewData["MetaData"] = "<meta name=\"description\" content=\"This page informs about the constraints of the ASIO Linked Data Server\">";
}
   <div id="contenido-principal">
        <div class="row">
            <div class="col col-12">
				<h1>ASIO Linked Data Server Constraints</h1>
				<p>The Linked Data Server of the Hercules ASIO project has these constraints:</p>
				<ul>
					<li>
						<p>LDP Non-RDF Sources are not supported.</p>
					</li>
					<li>
						<p>Only GET,HEAD and OPTIONS HTTP requests are supported on LDP RDF Sources.</p>
					</li>
					<li>
						<p>Only LDP basic containers are supported.</p>
					</li>
					<li>
						<p>Paging and sorting are not supported.</p>
					</li>
				</ul>
			</div>
		</div>
	</div>
', '2020-12-16 17:54:21.940925', '2021-02-10 12:51:53.497312'),
	('25f9dba5-c864-41bb-ac1b-3cca0c88bb32', '/public/yasgui', '@{
    Layout = "_Layout";
    ViewData["Title"] = "Yasgui";
    ViewData["link_scripts"] = "<link href=\"https://unpkg.com/@triply/yasgui/build/yasgui.min.css\" rel=\"stylesheet\" type=\"text/css\" /> <script src =\"https://unpkg.com/@triply/yasgui/build/yasgui.min.js\"></script>";
}

<div class="row">
    <div class="col col-12 col-lg-12 col-contenido">
        <div class="wrapCol">
            <div id="yasgui"></div>
            <script>
					const yasgui = new Yasgui(document.getElementById("yasgui"), {
						requestConfig: { endpoint: "http://155.54.239.204:8890/sparql" },
						copyEndpointOnNewTab: false
						});
            </script>
        </div>
    </div>
</div>
', '2021-01-21 16:13:55.221725', '2021-02-02 15:33:57.802241'),
	('68cee752-da14-4da9-b399-56a5b18baf84', '/public/gnossdeustobackend/catalogo-de-datos', '@model ApiCargaWebInterface.ViewModels.CmsDataViewModel
@using CsvHelper
@using System.Globalization
@using System.IO
@using System.Text
@{
    Layout = "_Layout";
    ViewData["BodyClass"] = "tipo-documento";
	ViewData["SecondaryMenu"] = "<ul class=\"nav nav-tabs container\" id=\"myTab\" role=\"tablist\"> <li class=\"nav-item active\" role=\"presentation\"> <a class=\"nav-link\" id=\"seccion1-tab\" data-toggle=\"tab\" href=\"#seccion\" role=\"tab\" aria-controls=\"seccion\" aria-selected=\"true\">Datos disponibles</a> </li> <li class=\"nav-item\" role=\"presentation\"> <a class=\"nav-link\" id=\"seccion2-tab\" data-toggle=\"tab\" href=\"#seccion2\" role=\"tab\" aria-controls=\"seccion2\" aria-selected=\"false\">Descargas de datos</a> </li> <li class=\"nav-item\" role=\"presentation\">        <a class=\"nav-link\" id=\"seccion3-tab\" data-toggle=\"tab\" href=\"#seccion3\" role=\"tab\" aria-controls=\"seccion3\" aria-selected=\"false\">Ejemplos SPARQL</a> </li> <li class=\"nav-item\" role=\"presentation\"> <a class=\"nav-link\" id=\"seccion4-tab\" data-toggle=\"tab\" href=\"#seccion4\" role=\"tab\" aria-controls=\"seccion4\" aria-selected=\"false\">Métricas FAIR</a>    </li> </ul>";
	
	string result = Model.Results.FirstOrDefault();
    byte[] byteArray = Encoding.UTF8.GetBytes(result);
    MemoryStream stream = new MemoryStream(byteArray);
    var csvReader = new CsvReader(new StreamReader(stream), CultureInfo.InvariantCulture);
    var records = csvReader.GetRecords<PruebaSparql>();
	
	
	string result2 = Model.Results[1];
    byteArray = Encoding.UTF8.GetBytes(result2);
    stream = new MemoryStream(byteArray);
    csvReader = new CsvReader(new StreamReader(stream), CultureInfo.InvariantCulture);
    var records2 = csvReader.GetRecords<investigadoresSparql>();
}
@*<% sparql
    select count(*) as ?count
    where
    {
     ?s ?p ?o
     }
    /%>*@

@*<% sparql
    select count(?s) as ?cuentaInvestigadores from <http://graph.um.es/graph/um_sgi>
    where
    {
    ?s a <http://purl.org/roh/mirror/foaf#Person>
    }
    /%>*@

        <div class="tab-content">
            <div class="tab-pane active" id="seccion" role="tabpanel" aria-labelledby="seccion-tab">
                <div class="row">
                    <div class="col col-12">
                        <h1>Datos disponibles en Hércules ASIO</h1>
                        <p>Como resultado del proyecto Hércules ASIO, los datos generados por la actividad investigadora de la Universidad de Murcia están disponibles en un grafo de conocimiento. Los datos de este grafo pueden consultarse mediante un SPARQL Endpoint, mientras que sus entidades y relaciones están accesibles como datos enlazados y abiertos (Linked Open Data). El RDF Store cuenta actualmente con:
						<ul>
                            <li>
                                @foreach (var fila in records)
								{
									<strong>@fila.count</strong>
								}						
								triples. 
                            </li>
                            <li>
                                Datos de
								@foreach (var fila2 in records2)
								{
									<strong>@fila2.cuentaInvestigadores</strong>
								}						
								investigadores.
                            </li>
                            <li>Información de 900 proyectos.</li>
                            <li>Información de resultados de investigación, como 10.000 libros o 60.000 conferencias</li>
                        </ul>
						</p>
                    </div>
                </div>
                <div class="row">
                    <div class="col col-12 col-titulo">
                        <h2>Acceso a los volcados de datos</h2>
                    </div>
                    <div class="col col-12 col-md-6 col-imagen">
                        <div class="imagen"></div>
                    </div>
                    <div class="col col-12 col-md-6 col-texto">
                        <p>En esta sección estarán disponibles los enlaces para descargar los datos públicos.</p>
                    </div>
                </div>
                <div class="row">
                    <div class="col col-12 col-titulo">
                        <h2>Ejemplos de consultas SPARQL</h2>
                    </div>
                    <div class="col col-12 col-md-6 col-texto">
                        <p>El SPARQL Endpoint público se encuentra disponible en la siguiente URL:</p>
						<p><a href="http://155.54.239.204:8890/sparql>"http://155.54.239.204:8890/sparql</a></p>
                        <p>Se indican a continuación algunos ejemplos de consultas:
						<ul>
							<li><a href="http://155.54.239.204:8890/sparql?default-graph-uri=&amp;query=select+%3Fs+count%28%3Fo%29+as+%3Fatributos+from+%3Chttp%3A%2F%2Fgraph.um.es%2Fgraph%2Fum_sgi%3E%0D%0Awhere+%7B%3Fs+a+%3Chttp%3A%2F%2Fpurl.org%2Froh%2Fmirror%2Ffoaf%23Person%3E.%0D%0A%3Fs+%3Fp+%3Fo%0D%0A%7D%0D%0Aorder+by+DESC%28%3Fatributos%29%0D%0ALIMIT+100%0D%0A&amp;should-sponge=&amp;format=text%2Fhtml&amp;timeout=0&amp;debug=on&amp;run=+Run+Query+">Obtener los 100 investigadores con más atributos (más datos)</a>.
							</li>
                            <li><a href="http://155.54.239.204:8890/sparql?default-graph-uri=&amp;query=select+*+from+%3Chttp%3A%2F%2Fgraph.um.es%2Fgraph%2Fum_sgi%3E%0D%0Awhere+%7B+%3Chttp%3A%2F%2Fgraph.um.es%2Fres%2Fperson%2F1602%3E+%3Fp+%3Fo%7D%0D%0A%0D%0A&amp;should-sponge=&amp;format=text%2Fhtml&amp;timeout=0&amp;debug=on&amp;run=+Run+Query+">Obtener los atributos de un investigador</a>.
							</li>
						</ul>
						</p>						
                    </div>
                    <div class="col col-12 col-md-6 col-imagen">
                        <div class="imagen"></div>
                    </div>
                </div>
            </div>
            <div class="tab-pane" id="seccion2" role="tabpanel" aria-labelledby="seccion2-tab">
                <div class="row">
                    <div class="col col-12">
                        <h2>Seccion 2</h2>
                    </div>
                </div>
            </div>
            <div class="tab-pane" id="seccion3" role="tabpanel" aria-labelledby="seccion3-tab">
                <div class="row">
                    <div class="col col-12">
                        <h2>Seccion 3</h2>
                    </div>
                </div>
            </div>
            <div class="tab-pane" id="seccion4" role="tabpanel" aria-labelledby="seccion4-tab">
                <div class="row">
                    <div class="col col-12">
                        <h2>Seccion 4</h2>
                    </div>
                </div>
            </div>
        </div>
@functions{
    public class PruebaSparql
    {
        public int count { get; set; }
    }

	public class investigadoresSparql
    {
        public int cuentaInvestigadores { get; set; }
    }
}
', '2020-12-16 09:56:35.912528', '2021-02-02 15:34:01.478907'),
	('9945a302-4258-487f-b9e9-7144d7acfe36', '/public/gnossdeustobackend/adhesion-buenas-practicas-uris', '@model ApiCargaWebInterface.ViewModels.CmsDataViewModel
@using CsvHelper
@using System.Globalization
@using System.IO
@using System.Text
@{
    Layout = "_Layout";
    ViewData["BodyClass"] = "tipo-documento";
	ViewData["Title"] = "Adhesión buenas prácticas URIs";
	ViewData["MetaData"] = "<meta name=\"description\" content=\"Los URI (Uniform Resource Identifier) usados para referenciar la información del SGI mediante Hércules ASIO compartan un esquema común, aplican unas reglas de normalización similares y persisten\">";
}
        <div class="row">
            <div class="col col-12">
				<h1>Documento de adhesión a las recomendaciones de buenas prácticas de URIs</h1>
				<p>La Universidad de Murcia ha implantado el backend del proyecto Hércules Arquitectura
				Semántica e Infraestructura Ontológica (ASIO en adelante) para hacer accesible,
				mediante los estándares de la web semántica, la información de su Sistema de
				Gestión de la Investigación (SGI), lo que incluye datos de sus investigadores,
				sus proyectos y sus resultados de investigación.</p>
				<p>La iniciativa Hércules es parte de la Comisión Sectorial de Tecnologías de la
				Información y las Comunicaciones de la Conferencia de Rectores de las Universidades
				Españolas (CRUE-TIC). Su objetivo es crear un Sistema de Gestión de Investigación (SGI)
				basado en datos abiertos semánticos que ofrezca una visión global de los datos
				de investigación del Sistema Universitario Español (SUE), para mejorar la gestión,
				el análisis y las posibles sinergias entre universidades y el gran público.</p>
				<p>El proyecto Hércules ASIO es uno de los componentes de la iniciativa Hércules,
				que es parte de la Comisión Sectorial de Tecnologías de la Información y las
				Comunicaciones de la Conferencia de Rectores de las Universidades Españolas (CRUE-TIC).</p>
				<p>ASIO está compuesto de:</p>
				<ul>
					<li>
					<p>La Infraestructura Ontológica de la información del SUE comprende una red de
					ontologías que pueda ser usada para describir con fidelidad y alta granularidad
					los datos del dominio de la Gestión de la Investigación.</p>
					</li>
					<li>
					<p>La Arquitectura Semántica de Datos del SUE consiste en una plataforma eficiente
					para almacenar, gestionar y publicar los datos del SGI, basándose en la
					Infraestructura Ontológica, con la capacidad de sincronizar instancias
					instaladas en diferentes Universidades.</p>
					</li>
				</ul>
				<p>Para el cumplimiento de los objetivos propuestos, es importante que los <strong>URI
				(Uniform Resource Identifier)</strong>, usados para referenciar la información del SGI
				mediante Hércules ASIO, <strong>compartan un esquema común, apliquen unas reglas
				de normalización similares y, especialmente, persistan según lo identificado</strong>,
				es decir, que cumplan el principio de persistencia para que la información
				sea accesible en el futuro y se mantenga la integridad de la información.</p>
				<p>Por tanto, la Universidad de Murcia se compromete, en la medida de lo posible,
				a cumplir el documento de <a href="https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/UrisFactory/docs/Buenas%20pr%C3%A1cticas%20URIs.md" target="_blank">Buenas prácticas de URIs de Hércules</a>, 
				lo que implica que los recursos que aloja en su instancia de Hércules ASIO 
				cumplen con las siguientes condiciones:</p>
				<ul>
					<li>
					<p>Los URIs que identifican a los recursos no variarán sin justificación
					y proporcionarán acceso a la información que referencian, mediante
					mecanismos HTTP estándares.</p>
					</li>
					<li>
					<p>En caso de variación, el URI proporcionará mecanismos estándar, mediante
					códigos de estado HTTP, para redirigir al nuevo URI o para informar de
					que el recurso ha sido eliminado.</p>
					</li>
					<li>
					<p>Los URIs se construirán siguiendo la <a href="https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/UrisFactory/docs/Especificaci%C3%B3n%20Esquema%20de%20URIs.md" target="_blank">Especificación del Esquema de URIs
					de Hércules</a> y las recomendaciones de normalización de las
					Buenas prácticas de URIs de Hércules.</p>
					</li>
					<li>
					<p>Los recursos persistentes continuarán estando disponibles durante la
					existencia de la Universidad de Murcia.</p>
					</li>
					<li>
					<p>Si el backend dejase de existir en la Universidad de Murcia, otras entidades
					del ámbito de la universidad española podrían publicar la información
					con nuevos URIs con un dominio distinto, y aplicando la misma política
					de persistencia de datos y cumplimiento de las las Buenas prácticas de
					URIs de Hércules. La Universidad de Murcia proporcionará durante <em>N</em> meses
					mecanismos de redirección hacia el nuevo dominio, mediante códigos de
					estado HTTP 3xx.</p>
					</li>
				</ul>
			</div>
		</div>
', '2020-12-16 09:48:29.38269', '2021-02-10 11:38:58.56209'),
	('a8161a42-692e-4b26-8a8f-bbe2bd76a35b', '/public/prueba/api', '@model ApiCargaWebInterface.ViewModels.CmsDataViewModel
@using Newtonsoft.Json
@{
    Layout = "_Layout";
    ViewData["BodyClass"] = "fichaRecurso";
	string result = Model.Results.FirstOrDefault();
    List<JobPage> resultObject = JsonConvert.DeserializeObject<List<JobPage>>(result);
    var jobs = resultObject.Where(item => item.ExecutedAt.HasValue).OrderByDescending(item => item.ExecutedAt.Value).Take(2);
}
@*<% api http://herc-as-front-desa.atica.um.es/cron-config/Job?type=0&count=100&from=0 /%>*@
<div class="row">
    <div class="col col-12 col-lg-12 col-contenido">
        <div class="wrapCol">
            <div class="row">
                <div class="col col-12 col-lg-12 col-contenido">
                    <div class="wrapCol">
                        <h1>Dos últimas tareas ejecutadas</h1>
                        <div class="contenido">
                            <div class="grupo grupo-descripcion">
                                <div class="items tabla">
                                    <div class="cabecera">
                                        <div class="columna">
                                            <p>Identificador</p>
                                        </div>
                                        <div class="columna">
                                            <p>Fecha de ejecución</p>
                                        </div>
                                    </div>
                                    @foreach (var job in jobs)
                                    {
                                        <div class="fila">
                                            <div class="columna principal">
                                                <p>@job.Id</p>
                                            </div>
                                            <div class="columna">
                                                <p>@job.ExecutedAt.Value</p>
                                            </div>
                                        </div>
                                    }
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
@functions{
    public class JobPage
    {
        public string Job { get; set; }
        public string State { get; set; }
        public string Id { get; set; }
        public string ExceptionDetails { get; set; }
        public DateTime? ExecutedAt { get; set; }
    }
}
', '2020-10-23 12:14:44.478459', '2021-02-03 11:53:25.04244'),
	('1761ab7f-081c-4501-a4cd-80c572ab0c19', '/public/gnossdeustobackend/index', '@{
Layout = "_Layout";
ViewData["BodyClass"] = "fichaRecurso";
ViewData["Title"] = "Index - ASIO";
ViewData["MetaData"] = "<meta name=\"description\" content=\"pagina de inicio del proyecto hércules en github\">";
}
<div class="row">
	<div class="col col-12 col-lg-12 col-contenido">
		<div class="wrapCol">
				<h1>Sobre GnossDeustoBackend</h1>
				<p>Éste es el repositorio del proyecto Hércules ASIO en el que se incluyen los siguientes programas y servicios</p>
				<ul>
					<li>
						<a href="https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/API_CARGA" target="_blank">API_CARGA:</a> 
						Servicio web que realiza las tareas de carga/configuración.
					</li>
					<li>
						<a href="https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/API_DISCOVER" target="_blank">API_DISCOVER:</a> 
						Servicio que realiza el descubrimiento de los RDF y su posterior publicación.
					</li>
					<li>
						<a href="https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/FrontEndCarga" target="_blank">FrontEndCarga:</a> 
						Interfaz web para la parte de Repository y Validation del API_CARGA
					</li>
					<li>
						<a href="https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/OAI_PMH_CVN" target="_blank">OAI_PMH_CVN:</a> 
						Servicio OAI-PMH para la obtención de investigadores de la Universidad de Murcia.
					</li>
					<li>
						<a href="https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/UrisFactory" target="_blank">UrisFactory:</a>
						Servicio que genera las uris de los recursos.		
					</li>
					<li>
						<a href="https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/cvn" target="_blank">cvn:</a> 
						Servidor HTTP que ofrece una API para convertir XML CVN a tripletas ROH.
					</li>
					<li>
						<a href="https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/CronConfigure" target="_blank">CronConfigure:</a> 
						Servicio Web que realiza la creación de tareas para la sincronización de un repositorio.
					</li>
					<li>
						<a href="https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/GestorDocumentacion" target="_blank">GestorDocumentacion:</a> 
						 Servicio web para la creación de páginas de contenido html y su posterior visualización en FrontEndCarga.
					</li>
					<li>
						<a href="https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/IdentityServerHecules" target="_blank">IdentityServerHecules:</a> 
						Servicio encargada de gestionar y proporcionar los tokens de acceso a los diferentes apis.
					</li>
				</ul>

				<h2>Diagrama de componentes del proyecto:</h2>
				<img src="https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/img/diagrama_de_componentes.png?raw=true">
				<p>Todas las aplicaciones aquí descritas pueden usarse de dos formas distintas:</p>
				<ul>
					<li>Como un API, instalando la aplicación como se describe más abajo y llamando a su Endpoint.</li>
					<li>Como una <a href="https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/libraries" target="_blank">librería</a>, añadiendo el ensamblado DLL a la solución de código fuente y usando las clases y métodos definidos en él.</li>
				</ul>

				<h2>Configuración e instalación</h2>
				<p>Las <a href="https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Configuraci%C3%B3n%20e%20Instalaci%C3%B3n.md" target="_blank">instrucciones de configuración e instalación</a> son el punto de partida para comenzar a usar los desarrollos de Hércules ASIO</p>
		</div>
	</div>
</div>
', '2020-12-15 09:24:27.540724', '2021-02-16 08:45:35.06344'),
	('e0457e53-9f6e-436b-be2a-d068c5e74250', '/public/gnossdeustobackend/grafo-de-conocimiento', '@model ApiCargaWebInterface.ViewModels.CmsDataViewModel
@using CsvHelper
@using System.Globalization
@using System.IO
@using System.Text
@{
    Layout = "_Layout";
    ViewData["BodyClass"] = "tipo-documento";
	ViewData["Title"] = "Grafo de Conocimiento de ASIO";
	ViewData["MetaData"] = "<meta name=\"description\" content=\"Como resultado del proyecto Hércules ASIO, los datos generados por la actividad investigadora de la Universidad de Murcia están disponibles en un grafo de conocimiento\">";
	ViewData["SecondaryMenu"] = "<ul class=\"nav nav-tabs container\" id=\"myTab\" role=\"tablist\"> <li class=\"nav-item active\" role=\"presentation\"> <a class=\"nav-link\" id=\"seccion1-tab\" data-toggle=\"tab\" href=\"#seccion-panel\" role=\"tab\" aria-controls=\"seccion\" aria-selected=\"true\">Estadísticas generales</a> </li> <li class=\"nav-item\" role=\"presentation\"> <a class=\"nav-link\" id=\"seccion2-tab\" data-toggle=\"tab\" href=\"#seccion2-panel\" role=\"tab\" aria-controls=\"seccion2\" aria-selected=\"false\">Resumen del Grafo ASIO-SGI</a> </li> <li class=\"nav-item\" role=\"presentation\">        <a class=\"nav-link\" id=\"seccion3-tab\" data-toggle=\"tab\" href=\"#seccion3-panel\" role=\"tab\" aria-controls=\"seccion3\" aria-selected=\"false\">Entidades del Grafo ASIO-SGI</a> </li> <li class=\"nav-item\" role=\"presentation\"> <a class=\"nav-link\" id=\"seccion4-tab\" data-toggle=\"tab\" href=\"#seccion4-panel\" role=\"tab\" aria-controls=\"seccion4\" aria-selected=\"false\">Datos del Grafo ASIO-SGI</a>    </li> </ul>";
	
	string result = Model.Results.FirstOrDefault();
    byte[] byteArray = Encoding.UTF8.GetBytes(result);
    MemoryStream stream = new MemoryStream(byteArray);
    var csvReader = new CsvReader(new StreamReader(stream), CultureInfo.InvariantCulture);
    var records = csvReader.GetRecords<PruebaSparql>();

	string result3 = Model.Results[1];
    byte[] byteArray3 = Encoding.UTF8.GetBytes(result3);
    MemoryStream stream3 = new MemoryStream(byteArray3);
    var csvReader3 = new CsvReader(new StreamReader(stream3), CultureInfo.InvariantCulture);
    var records3 = csvReader3.GetRecords<grafosASIO>();
		
	string result2 = Model.Results[2];
    byte[] byteArray2 = Encoding.UTF8.GetBytes(result2);
    MemoryStream stream2 = new MemoryStream(byteArray2);
    var csvReader2 = new CsvReader(new StreamReader(stream2), CultureInfo.InvariantCulture);
    var records2 = csvReader2.GetRecords<entidadesGrafo>();
}
@*<% sparql
    select count(*) as ?count
    where
    {
     ?s ?p ?o
     }
    /%>*@

@*<% sparql
    select  ?graph ?nameOrganizationGraph ?numTriples
where
{
    {
        select  ?graph count(*) as ?numTriples
        where
        {
            select *
            where
            {
                graph ?graph
                {
                    ?s ?p ?o
                }
                                FILTER(str(?graph) like''http://graph.um.es*'')
            }
                       
        }
               
    }
    OPTIONAL
    {
        ?graph <http://www.w3.org/ns/prov#wasAttributedTo> ?organizationGraph.
        ?organizationGraph <http://purl.org/roh/mirror/foaf#name> ?nameOrganizationGraph
    }
}order by desc(?numTriples)
    /%>*@

@*<% sparql
    select distinct ?entity ?type ?numTriples
	where
	{
		{
			select ?type count(?s) as ?numTriples
			from <http://graph.um.es/graph/sgi>
			where
			{
				?s rdf:type ?type.
			}
			group by ?type	 
		}
		OPTIONAL
		{
			?type <http://www.w3.org/2000/01/rdf-schema#label> ?entity.
			filter(lang(?entity)=''es'')
		}
	}order by desc(?numTriples)	
	
    /%>*@
        <div class="tab-content">
			<!-- Sección de Estadísticas generales, contiene información acerca de los triples y grafos (triples por grafo y PROV) \-->
            <div class="tab-pane active" id="seccion-panel" role="tabpanel" aria-labelledby="seccion-tab">
                <div class="row">
                    <div class="col col-12">
                        <h1>Datos disponibles en el grafo de conocimiento de Hércules ASIO-SGI</h1>
                        <p>Como resultado del proyecto Hércules ASIO, los datos generados por la actividad investigadora de la Universidad de Murcia están disponibles en un grafo de conocimiento. Los datos de este grafo pueden consultarse mediante un SPARQL Endpoint, mientras que sus entidades y relaciones están accesibles como datos enlazados y abiertos (Linked Open Data). El RDF Store cuenta actualmente con
                                @foreach (var fila in records)
								{
									<strong>@fila.count</strong>
								}						
								triples, repartidos en los siguientes grafos:
						</p>
						<table>
							<thead>
								<tr>
									<th>Grafo</th>
									<th>Origen</th>
									<th>Número de triples</th>
								</tr>
							</thead>
							<tbody>
								@foreach (var fila3 in records3)
								{
								<tr>
									<td><a href="@fila3.graph" target="_blank">
									@fila3.graph</a></td>
									<td>@fila3.nameOrganizationGraph</td>
									<td>@fila3.numTriples</td>
								</tr>
								}
							</tbody>
						</table>
                    </div>
                </div>
            </div>
            <!-- Sección de Resumen del Grafo SGI, entidades destacadas y alguna gráfica, por diseñar y decidir \-->
			<div class="tab-pane" id="seccion2-panel" role="tabpanel" aria-labelledby="seccion2-tab">
                <div class="row">
                    <div class="col col-12">
                        <h2>Resumen del Grafo de Conocimiento ASIO-SGI</h2>
                    </div>
					<div class="col col-12 col-md-6 col-texto">
						<p>Resumen del grafo ASIO-SGI, entidades destacadas y alguna gráfica.</p>
					</div>
                </div>
            </div>
            <!-- Sección de Entidades del Grafo SGI, listado completo de todas las entidades del grafo, indicando cuántas hay de cada una \-->
			<div class="tab-pane" id="seccion3-panel" role="tabpanel" aria-labelledby="seccion3-tab">
                <div class="row">
                    <div class="col col-12">
                        <h2>Entidades del Grafo de Conocimiento ASIO-SGI</h2>
						<p>Listado de entidades con total de entidades.</p>
						<table>
							<thead>
								<tr>
									<th>Entidad</th>
									<th>Tipo</th>
									<th>Número de triples</th>
								</tr>
							</thead>
							<tbody>
								@foreach (var fila2 in records2)
								{
								<tr>
									<td>@fila2.entity</td>
									<td>@fila2.type</td>
									<td>@fila2.numTriples</td>
								</tr>
								}
							</tbody>
						</table>
					</div>
                </div>
            </div>
			<!-- Sección de Datos del Grafo SGI, acceso a los volcados, ejemplos SPARQL, uso de Linked Data Server, acceso a FAIR \-->
            <div class="tab-pane" id="seccion4-panel" role="tabpanel" aria-labelledby="seccion4-tab">
                <div class="row">
                    <div class="col col-12 col-titulo">
                        <h2>Datos del Grafo de Conocimiento ASIO-SGI</h2>
                    <p>Los datos del grafo pueden consultarse mediante un SPARQL Endpoint, mientras que sus entidades y relaciones están accesibles como datos enlazados y abiertos (Linked Open Data). Esto convierte los datos de ASIO SGI en un conjunto de datos FAIR.</p>
                   </div>
				</div>
				<div class="row">
                    <div class="col col-12">
                        <h2>Consultas SPARQL</h2>
                    </div>
					<div class="col col-12 col-md-6 col-texto">
						<p><a href="https://www.w3.org/TR/sparql11-overview/" target="_blank">SPARQL (<strong>S</strong>PARQL <strong>P</strong>rotocol and <strong>R</strong>DF <strong>Q</strong>uery <strong>L</strong>anguage)</a> es el lenguaje de consulta para RDF y también el protocolo con el que se envían las consultas y se reciben los resultados.</p>
                        <p>Hércules ASIO dispone de un SPARQL Endpoint público, que permite realizar consultas SPARQL sobre los datos del grafo ASIO-SGI y que se encuentra disponible en la siguiente URL:</p>
						<p><a href="http://155.54.239.204:8890/sparql">http://155.54.239.204:8890/sparql</a></p>
                        <p>Se indican a continuación algunos ejemplos de consultas:
						<ul>
							<li><a href="http://155.54.239.204:8890/sparql?default-graph-uri=&amp;query=select+%3Fs+count%28%3Fo%29+as+%3Fatributos+from+%3Chttp%3A%2F%2Fgraph.um.es%2Fgraph%2Fum_sgi%3E%0D%0Awhere+%7B%3Fs+a+%3Chttp%3A%2F%2Fpurl.org%2Froh%2Fmirror%2Ffoaf%23Person%3E.%0D%0A%3Fs+%3Fp+%3Fo%0D%0A%7D%0D%0Aorder+by+DESC%28%3Fatributos%29%0D%0ALIMIT+100%0D%0A&amp;should-sponge=&amp;format=text%2Fhtml&amp;timeout=0&amp;debug=on&amp;run=+Run+Query+">Obtener los 100 investigadores con más atributos (más datos)</a>.
							</li>
                            <li><a href="http://155.54.239.204:8890/sparql?default-graph-uri=&amp;query=select+*+from+%3Chttp%3A%2F%2Fgraph.um.es%2Fgraph%2Fum_sgi%3E%0D%0Awhere+%7B+%3Chttp%3A%2F%2Fgraph.um.es%2Fres%2Fperson%2F1602%3E+%3Fp+%3Fo%7D%0D%0A%0D%0A&amp;should-sponge=&amp;format=text%2Fhtml&amp;timeout=0&amp;debug=on&amp;run=+Run+Query+">Obtener los atributos de un investigador</a>.
							</li>
						</ul>
						</p>						
                    </div>
					<div class="col col-12 col-md-6 col-imagen">
                        <div class="imagen"><img src="http://herc-as-front-desa.atica.um.es/docs/sparql.svg" alt="SPARQL" height="240" style="display:block;margin-left:auto;margin-right:auto;"></div>
                    </div>
                </div>
				<div class="row">
                    <div class="col col-12">
                        <h2>Linked Data Server</h2>
                    </div>
					<div class="col col-12 col-md-6 col-imagen">
                        <div class="imagen"><img src="http://herc-as-front-desa.atica.um.es/docs/knowledgegraph.png" alt="Linked Data Server" height="240" style="display:block;margin-left:auto;margin-right:auto;"></div>
                    </div>
					<div class="col col-12 col-md-6 col-texto">
                        <p>Linked Data Server es un <a href="https://github.com/HerculesCRUE/GnossDeustoBackend/tree/master/Linked_Data_Server" target="_blank">componente de ASIO desarrollado en .Net Core</a> que proporciona el servicio de datos enlazados de Hércules ASIO.</p>
						<p>Linked Data Server proporciona acceso a los datos de las entidades del grafo para personas y máquinas, que pueden obtener la representación RDF de la entidad si la petición solicita "application/rdf+xml", y cumple la especificación LDP: <a href="https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/Docs/20200415%20H%C3%A9rcules%20ASIO%20Evaluaci%C3%B3n%20de%20cumplimiento%20Linked%20Data%20Platform.md">Hércules Backend ASIO. Evaluación de cumplimiento Linked Data Platform (LDP)</a></p>
						<p>Se puede comprobar el funcionamiento del servidor mediante los siguientes ejemplos:</p>
						<ul>
							<li>Investigador con publicaciones e identificadores externos. <a href="http://graph.um.es/res/person/63ec870e-9908-49b9-9242-07a449bc562f" rel="nofollow">http://graph.um.es/res/person/63ec870e-9908-49b9-9242-07a449bc562f</a> </li>
							<li>Artículo con DOI y otros identificadores obtenidos desde fuentes externas. <a href="http://graph.um.es/res/article/a248813c-cfe9-4208-8009-87464d6cfade" rel="nofollow">http://graph.um.es/res/article/a248813c-cfe9-4208-8009-87464d6cfade</a></li>
							<li>Proyecto de investigación. <a href="http://graph.um.es/res/project/5d35f1da-6eee-49e3-a350-54447ab24344" rel="nofollow">http://graph.um.es/res/project/5d35f1da-6eee-49e3-a350-54447ab24344</a></li>
						</ul>
                    </div>
                </div>
				<div class="row">
                    <div class="col col-12">
                        <h2>Datos FAIR</h2>
                    </div>
					<div class="col col-12 col-md-6 col-texto">
                        <p>Los <a href="https://doi.org/10.25504/FAIRsharing.WWI10U" target="_blank">Principios FAIR</a> proporcionan directrices para la publicación de recursos digitales tales como conjuntos de datos, códigos, flujos de trabajo y objetos de investigación, de manera que sean localizables, accesibles, interoperables y reutilizables (FAIR).</p>
						<p>Hoy en día, la mejor manera de publicar datos FAIR es hacerlo mediante Linked Data, teniendo especial cuidado de generar datos y metadatos de alta calidad, mejorando así la reusabilidad de los datos para máquinas y, como consecuencia y en última instancia, para humanos.</p>
						<p>En el documento de <a href="https://github.com/HerculesCRUE/GnossDeustoBackend/blob/master/fair/Docs/Entregable%20EF2-1.6_%20documento%20de%20an%C3%A1lisis%20de%20m%C3%A9todos%20FAIR.md" target="_blank">Análisis de métodos FAIR</a> se puede consultar cómo se alinea Hércules ASIO con los principios FAIR.</p>
                    </div>
					<div class="col col-12 col-md-6 col-imagen">
                        <div class="imagen"><img src="http://herc-as-front-desa.atica.um.es/docs/FairData.png" alt="Go FAIR Data" height="240" style="display:block;margin-left:auto;margin-right:auto;"></div>
                    </div>
                </div>
            </div>
        </div>
@functions{
    public class PruebaSparql
    {
        public int count { get; set; }
    }

	public class grafosASIO
    {
        public string graph { get; set; }
		public string nameOrganizationGraph { get; set; }
        public int numTriples { get; set; }
    }
	
	public class entidadesGrafo
    {
		public string entity { get; set; }
        public string type { get; set; }
		public int numTriples { get; set; }
    }
}
', '2021-01-21 16:08:11.090799', '2021-02-17 10:38:31.735621'),
	('b1c2cbcf-22fb-4d84-bf88-aec8f4204854', '/public/gnossdeustobackend/TestGraficos', '@{
    Layout = "_Layout";
    ViewData["Title"] = "Test de Gráficos";
    ViewData["link_scripts"] = "<script src=\"https://cdnjs.cloudflare.com/ajax/libs/d3/4.7.2/d3.min.js\"></script>";
	ViewData["BodyClass"] = "dashboard";
}
        <div class="row">
            <div class="col col-12">
                <h2>
                    <a href="javascript: void(0);">Producción científica</a>
                </h2>
                <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam eleifend, ex elementum molestie varius, purus erat imperdiet nunc, in scelerisque ipsum ante ac risus. <a href="javascript: void(0);">Praesent et metus id eros rhoncus maximus nec</a> porta ante. In posuere nunc neque, non maximus ipsum placerat quis.</p>
                <div class="entidades">
                    <div>
                        <p>
                            <a href="javascript: void(0);">
                                <span class="numResultados">256</span>
                                Papers
                            </a>
                        </p>
                        <p>
                            <a href="javascript: void(0);">
                                <span class="numResultados">3.328</span>
                                Conferencias
                            </a>
                        </p>
                        <p>
                            <a href="javascript: void(0);">
                                <span class="numResultados">110</span>
                                Proyectos
                            </a>
                        </p>
                    </div>
                </div>
            </div>
        </div>
		<div class="row">
            <div class="col col-12">
                <h2>
                    <a href="javascript: void(0);">Proyectos por año</a>
                </h2>
				<div class="graficos">
					<img src="http://herc-as-front-desa.atica.um.es/imagenes/barchart-demo.svg" alt="">
				</div>               
            </div>
        </div>
        <div class="row">
            <div class="col col-12">
                <h2>
                    <a href="javascript: void(0);">Proyectos</a>
                </h2>
				<div class="graficos">
					<div id="pieChart"></div>
					<div id="pieChart2"></div>
				</div>               
            </div>
        </div>
        <div class="row">
            <div class="col col-12">
                <h2>
                    <a href="javascript: void(0);">Investigador</a>
                </h2>
                <p>Mapa y grafo de relaciones</p>
                <img src="http://herc-as-front-desa.atica.um.es/imagenes/canvas.png" alt="">
            </div>
        </div>

<script src="https://herc-as-front-desa.atica.um.es/carga-web/theme/static/d3pie.min.js"></script>
<script>
var pie = new d3pie("pieChart", {
	"header": {
		"title": {
			"text": "Por convocatoria",
			"fontSize": 20,
			"font": "open sans"
		},
		"subtitle": {
			"text": ".",
			"color": "#ffffff",
			"fontSize": 12,
			"font": "open sans"
		},
		"titleSubtitlePadding": 9
	},
	"footer": {
		"color": "#999999",
		"fontSize": 10,
		"font": "open sans",
		"location": "bottom-left"
	},
	"size": {
		"canvasHeight": 260,
		"canvasWidth": 380,
		"pieInnerRadius": "37%",
		"pieOuterRadius": "92%"
	},
	"data": {
		"sortOrder": "value-desc",
		"content": [
			{
				"label": "Europeo",
				"value": 45,
				"color": "#f30000"
			},
			{
				"label": "Nacional",
				"value": 29,
				"color": "#0600f3"
			},
			{
				"label": "Regional",
				"value": 8,
				"color": "#00b109"
			},
			{
				"label": "Convocatoria",
				"value": 8,
				"color": "#14e4b4"
			},
			{
				"label": "Otros",
				"value": 8,
				"color": "#0fe7fb"
			}
		]
	},
	"labels": {
		"outer": {
			"pieDistance": 20
		},
		"inner": {
			"hideWhenLessThanPercentage": 3
		},
		"mainLabel": {
			"fontSize": 11
		},
		"percentage": {
			"color": "#ffffff",
			"decimalPlaces": 0
		},
		"value": {
			"color": "#adadad",
			"fontSize": 11
		},
		"lines": {
			"enabled": true
		},
		"truncation": {
			"enabled": true,
			"truncateLength": 12
		}
	},
	"tooltips": {
		"enabled": true,
		"type": "placeholder",
		"string": "{label}: {value}, {percentage}%"
	},
	"effects": {
		"pullOutSegmentOnClick": {
			"effect": "none",
			"speed": 400,
			"size": 8
		}
	},
	"misc": {
		"gradient": {
			"enabled": true,
			"percentage": 100
		}
	},
	"callbacks": {
		onClickSegment: function(info) {
					console.log("click:", info);
		}
	}
});

var pie = new d3pie("pieChart2", {
	"header": {
		"title": {
			"text": "Por Área/Disciplina",
			"fontSize": 20,
			"font": "open sans"
		},
		"subtitle": {
			"text": ".",
			"color": "#ffffff",
			"fontSize": 12,
			"font": "open sans"
		},
		"titleSubtitlePadding": 9
	},
	"footer": {
		"color": "#999999",
		"fontSize": 10,
		"font": "open sans",
		"location": "bottom-left"
	},
	"size": {
		"canvasHeight": 260,
		"canvasWidth": 380,
		"pieInnerRadius": "37%",
		"pieOuterRadius": "92%"
	},
	"data": {
		"sortOrder": "value-desc",
		"content": [
			{
				"label": "Química",
				"value": 40,
				"color": "#f30000"
			},
			{
				"label": "Biología",
				"value": 34,
				"color": "#0600f3"
			},
			{
				"label": "Medio ambiente",
				"value": 12,
				"color": "#00b109"
			},
			{
				"label": "Economía circular",
				"value": 8,
				"color": "#14e4b4"
			},
			{
				"label": "Otros",
				"value": 4,
				"color": "#0fe7fb"
			}
		]
	},
	"labels": {
		"outer": {
			"pieDistance": 20
		},
		"inner": {
			"hideWhenLessThanPercentage": 3
		},
		"mainLabel": {
			"fontSize": 11
		},
		"percentage": {
			"color": "#ffffff",
			"decimalPlaces": 0
		},
		"value": {
			"color": "#adadad",
			"fontSize": 11
		},
		"lines": {
			"enabled": true
		},
		"truncation": {
			"enabled": true,
			"truncateLength": 12
		}
	},
	"tooltips": {
		"enabled": true,
		"type": "placeholder",
		"string": "{label}: {value}, {percentage}%"
	},
	"effects": {
		"pullOutSegmentOnClick": {
			"effect": "none",
			"speed": 400,
			"size": 8
		}
	},
	"misc": {
		"gradient": {
			"enabled": true,
			"percentage": 100
		}
	},
	"callbacks": {
		onClickSegment: function(info) {
					console.log("click:", info);
		}
	}
});
</script>
', '2021-01-26 14:18:47.203848', '2021-02-11 14:16:07.502761'),
	('51c981fc-28f8-40f7-8814-18428501dd7d', '/public/gnossdeustobackend/home', '@model ApiCargaWebInterface.ViewModels.CmsDataViewModel
@using Newtonsoft.Json
@using CsvHelper
@using System.Globalization
@using System.IO
@using System.Text
@{
    Layout = "_Layout";
    ViewData["BodyClass"] = "tipo-home";
	ViewData["Title"] = "Proyecto ASIO";
	ViewData["MetaData"] = "<meta name=\"description\" content=\"Web pública del Proyecto Hércules ASIO de la Universidad de Murcia\">";
	
	string result = Model.Results.FirstOrDefault();
    byte[] byteArray = Encoding.UTF8.GetBytes(result);
    MemoryStream stream = new MemoryStream(byteArray);
    var csvReader = new CsvReader(new StreamReader(stream), CultureInfo.InvariantCulture);
    var records = csvReader.GetRecords<TotalTriples>();
	
	string resultApi = Model.Results[1];
    List<JobPage> resultObject = JsonConvert.DeserializeObject<List<JobPage>>(resultApi);
    var job = resultObject.Where(item => item.ExecutedAt.HasValue).OrderByDescending(item => item.ExecutedAt).FirstOrDefault();
}

@*<% sparql
    select count(*) as ?count
    where
    {
     ?s ?p ?o
     }
    /%>*@
@*<% api http://herc-as-front-desa.atica.um.es/cron-config/Job?type=0&count=3&from=0 /%>*@
        <div class="row full-row">
            <div class="col col-12">
                <div class="banner">
					<img src="http://herc-as-front-desa.atica.um.es/docs/asio-titulos.jpg" class="img-fluid" alt="Hércules ASIO - Arquitectura Semántica Infraestructura Ontológica">
                </div>
            </div>
        </div>
        <div class="row">
            <h1>Hércules ASIO - Arquitectura Semántica Infraestructura Ontológica</h1>
            <div class="col col-12 col-md-6">
                <div class="bloque-texto">
                    <div class="imagen"><img src="http://herc-as-front-desa.atica.um.es/docs/hercules-crue.png" class="img-fluid" alt="Hércules CRUE"></div>
                    <div class="contenido">
                        <h2>Proyecto Hércules ASIO - Backend SGI</h2>
                        <p>El objetivo del Proyecto ASIO es adquirir un servicio de I+D que consiste en el desarrollo de soluciones innovadoras para la Universidad de Murcia en relación al reto de Arquitectura Semántica e Infraestructura Ontológica. En concreto, se pretende desarrollar e incorporar soluciones que superen las actualmente disponibles en el mercado ...<a href="https://herc-as-front-desa.atica.um.es/carga-web/public/gnossdeustobackend/informacion-hercules-asio">ver más</a></p>
                    </div>
                </div>
            </div>
            <div class="col col-12 col-md-3">
                <div class="bloque-texto">
                    <div class="imagen"><img src="http://herc-as-front-desa.atica.um.es/docs/knowledgegraph.png" class="img-fluid" alt="Grafo de conocimiento"></div>
                    <div class="contenido">
                        <h2>Grafo de ASIO</h2>
                        <p>El proyecto Hércules ASIO cuenta con un grafo de 
						@foreach (var fila in records)
							{
								<strong>@fila.count</strong>
							}
						triples, generados desde el SGI de la Universidad de Murcia...<a href="https://herc-as-front-desa.atica.um.es/carga-web/public/gnossdeustobackend/grafo-de-conocimiento">ver más</a></p>
                    </div>
                </div>
            </div>
            <div class="col col-12 col-md-3">
                <div class="bloque-texto">
                    <div class="imagen"><img src="http://herc-as-front-desa.atica.um.es/docs/hercules-status.png" class="img-fluid" alt="Estado de Hércules ASIO"></div>
                    <div class="contenido">
                        <h2>Estado de ASIO</h2>
                        <p>El último proceso de actualización de datos de ASIO desde el SGI se ha lanzado el @job.ExecutedAt.Value.ToString() ...<a href="https://herc-as-front-desa.atica.um.es/carga-web/public/gnossdeustobackend/grafo-de-conocimiento#seccion3">ver más</a></p>
                    </div>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col col-12 col-md-4">
                <div class="bloque-texto">
                    <div class="imagen"><img src="http://herc-as-front-desa.atica.um.es/docs/persistence.jpg" class="img-fluid" alt="URIs y persistencia"></div>
                    <div class="contenido">
                        <h2>URIs. Buenas prácticas</h2>
                        <p>Los URI (Uniform Resource Identifier) usados para referenciar la información del SGI mediante Hércules ASIO comparten un esquema común, aplican unas reglas de normalización similares y persisten ..."<a href="https://herc-as-front-desa.atica.um.es/carga-web/public/gnossdeustobackend/adhesion-buenas-practicas-uris">ver más</a></p>
                    </div>
                </div>
            </div>
            <div class="col col-12 col-md-4">
                <div class="bloque-texto">
                    <div class="imagen"><img src="http://herc-as-front-desa.atica.um.es/docs/fair.jpg" class="img-fluid" alt="FAIR Data"></div>
                    <div class="contenido">
                        <h2>FAIR. Linked Open Data</h2>
                        <p>Hércules ASIO genera un conjunto de datos FAIR mediante el servicio de un espacio de datos abiertos enlazados (y enlazables), lo que permite a personas y máquinas reutilizar los datos de las entidades del proceso de investigacón de la universidad, como investigadores, publicaciones o proyectos ...<a href="https://herc-as-front-desa.atica.um.es/carga-web/public/gnossdeustobackend/grafo-de-conocimiento#seccion4">ver más</a></p>
                    </div>
                </div>
            </div>
            <div class="col col-12 col-md-4">
                <div class="bloque-texto">
                    <div class="imagen"><img src="http://herc-as-front-desa.atica.um.es/docs/hercules-um.png" class="img-fluid" alt="Otros proyectos Hércules"></div>
                    <div class="contenido">
                        <h2>Otros proyectos Hércules</h2>
                        <p>El proyecto “Hércules - Semántica de Datos de Investigación de Universidades” es una iniciativa creada por la Universidad de Murcia con el objetivo de crear un Sistema de Gestión de Investigación (SGI) basado en datos abiertos semánticos que ofrezca una visión global de los datos de investigación del Sistema Universitario Español (SUE), para mejorar la gestión, el análisis y las posibles sinergias entre universidades y el gran público ...<a href="https://www.um.es/web/hercules/" target="_blank">ver más</a></p>
                    </div>
                </div>
            </div>
        </div>
@functions{
	public class JobPage
    {
        public string Job { get; set; }
        public string State { get; set; }
        public string Id { get; set; }
        public string ExceptionDetails { get; set; }
        public DateTime? ExecutedAt { get; set; }
    }
    public class TotalTriples
    {
        public int count { get; set; }
    }
}
', '2021-01-24 09:24:08.786548', '2021-02-22 12:11:22.449139'),
	('db93c342-93a4-4cbb-9d74-7efc048a9abf', '/Views/Shared/_menupersonalizado.cshtml', '
    <div class="container-fluid">
        <div class="navbar-header">
            <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#bs-example-navbar-collapse-1" aria-expanded="false">
                <span class="sr-only">Toggle navigation</span>
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
            </button>
            <a class="navbar-brand" href="https://herc-as-front-desa.atica.um.es/carga-web/public/gnossdeustobackend/home">Hércules</a>
        </div>
        <div class="collapse navbar-collapse" id="bs-example-navbar-collapse-1">
            <ul class="nav navbar-nav">
				<li>
					<a href="/carga-web/public/gnossdeustobackend/informacion-hercules-asio">Información sobre Hércules</a>
				</li>
				<li>
					<a href="/carga-web/public/gnossdeustobackend/grafo-de-conocimiento">Grafo de Conocimiento</a>
				</li>
				<li class="dropdown">
					<a class="dropdown-toggle" href="#" data-toggle="dropdown">
						Administración
						<b class="caret"></b>
					</a>
					<ul class="dropdown-menu" role="menu" aria-labelledby="dLabel">
						<li>
							<a href="/carga-web/RepositoryConfig">Repositorios</a>
						</li>
						<li>
							<a href="/carga-web/ShapeConfig">Shapes y Validación</a>
						</li>
						<li>
							<a href="/carga-web/UrisFactory">Factoría de URIs</a>
						</li>
						<li>
							<a href="/carga-web/Token">Obtener tokens</a>
						</li>
						<li>
							<a href="/carga-web/CheckSystem">Comprobación del sistema</a>
						</li>
						<li>
							<a href="/carga-web/CMS">Administración de páginas</a>
						</li>
					</ul>
				</li>
            </ul>
        </div>
    </div>

', '2021-01-11 15:03:37.734402', '2021-02-22 12:41:29.998739'),
	('04e786ac-506a-49dd-ba03-f84e7103d4b9', '/public/gnossdeustobackend/informacion-hercules-asio', '@model ApiCargaWebInterface.ViewModels.CmsDataViewModel
@using CsvHelper
@using System.Globalization
@using System.IO
@using System.Text
@{
    Layout = "_Layout";
    ViewData["BodyClass"] = "tipo-documento";
	ViewData["Title"] = "Información del proyecto ASIO";
	ViewData["MetaData"] = "<meta name=\"description\" content=\"El objetivo del Proyecto ASIO es adquirir un servicio de I+D consistente en el desarrollo de soluciones innovadoras para la Universidad de Murcia en relación al reto de Arquitectura Semántica e Infraestructura Ontológica\">";
}
        <div class="row">
            <div class="col col-12">
                <h1>Hércules ASIO. Arquitectura Semántica e Infraestructura Ontológica</h1>
                <p>El objetivo del Proyecto ASIO es adquirir un servicio de I+D consistente en el desarrollo de soluciones innovadoras para la Universidad de Murcia en relación al reto de Arquitectura Semántica e Infraestructura Ontológica. En concreto, se pretende desarrollar e incorporar soluciones que superen las actualmente disponibles en el mercado.</p>

				<p>La solución ASIO es susceptible de ser utilizada en el futuro de forma regular tanto por la Universidad de Murcia como por las restantes Universidades que forman parte de la CRUE con necesidades y competencias similares, ya que como proceso de compra pública precomercial, el objetivo ha sido alcanzar una solución innovadora dirigida específicamente a los retos y necesidades que afectan al sector público y que persiguen la dinamización de la I+D+i.</p>

				<p>El proyecto Infraestructura Ontológica de la información del Sistema Universitario Español consiste en crear una red de ontologías que describa con fidelidad y alta granularidad los datos del dominio de la Gestión de la Investigación.</p>

				<p>El proyecto Arquitectura Semántica de Datos del SUE ha consistido en desarrollar una plataforma eficiente para almacenar, gestionar y publicar los datos del SGI (Sistema de Gestión de la Investigación), basándose en la Infraestructura Ontológica, con la capacidad de sincronizar instancias instaladas en diferentes Universidades.</p>

				<p>Dado de los demás proyectos que componen HÉRCULES dependen tanto de la Infraestructura Ontológica, como de la Arquitectura Semántica de Datos, el proyecto ASIO interactúa con los desarrollos y resultados de los demás proyectos HÉRCULES: desarrollo de un Prototipo Innovador de Gestión de la Investigación para Universidades y Enriquecimiento de Datos y Métodos de Análisis.</p>
            </div>
        </div>
        <div class="row">
            <div class="col col-12 col-titulo">
                <h2>Desarrollo de Hércules ASIO</h2>
            </div>
            <div class="col col-12 col-md-6 col-imagen">
                <div class="imagen"><img src="http://herc-as-front-desa.atica.um.es/docs/roh-project.png" alt="Documentos de Hércules ASIO"></div>
            </div>
            <div class="col col-12 col-md-6 col-texto">
                <p>Hércules ASIO es un proyecto de software libre alojado en dos repositorios públicos de GitHub:</p>
				<ul>
                    <li>
                        <a href="https://github.com/HerculesCRUE/GnossDeustoOnto" target="_blank">GitHub de Infraestructura Ontológica</a>. Este repositorio aloja la Red de Ontologías Hércules - ROH y tiene los siguientes documentos principales: 
						<ul>
							<li>
							<a href="https://github.com/HerculesCRUE/GnossDeustoOnto/tree/master/Documentation" target="_blank">Tutorial de la Red de Ontologías Hércules (ROH)</a>. Se trata de una documentación explicativa, generada como primera lectura recomendada. El documento ilustra con diagramas como se relacionan entre sí las entidades principales de ROH. También incluye una tabla por cada entidad, en la que se indican las subclases y propiedades de tipo <i>object</i> y <i>datatype</i>.
							</li>
							<li>
							<a href="https://github.com/HerculesCRUE/GnossDeustoOnto/blob/master/Documentation/1-%20OntologyDocumentation.pdf"  target="_blank">ROH Reference Specification</a>. Este documento detalla, en formato tabular, las subclases y propiedades de tipo <i>object</i> y <i>datatype</i> de cada concepto de la ontología ROH.
							</li>						
						</ul>
                    </li>
					<li>
                        <a href="https://github.com/HerculesCRUE/GnossDeustoBackend" target="_blank">GitHub de Arquitectura Semántica</a>. Este repositorio contiene los componentes de software que, junto con el software base de sistemas y bases de datos, constituyen la arquitectura semántica del proyecto ASIO. 
					</li>
                </ul>				
            </div>
        </div>
', '2020-12-17 12:48:51.968072', '2021-02-16 08:45:34.914871');
/*!40000 ALTER TABLE "Page" ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
