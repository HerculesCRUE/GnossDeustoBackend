import static org.hamcrest.MatcherAssert.assertThat; 
import static org.hamcrest.Matchers.*;
import static org.junit.Assert.assertEquals;
import static org.junit.Assert.fail;

import java.io.FileWriter;
import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.hp.hpl.jena.query.Query;
import com.hp.hpl.jena.query.QueryExecution;
import com.hp.hpl.jena.query.QueryExecutionFactory;
import com.hp.hpl.jena.query.QueryFactory;
import com.hp.hpl.jena.query.QuerySolution;
import com.hp.hpl.jena.query.ResultSet;
import com.hp.hpl.jena.query.ResultSetFormatter;
import com.hp.hpl.jena.rdf.model.Literal;
import com.hp.hpl.jena.rdf.model.Model;
import com.hp.hpl.jena.rdf.model.ModelFactory;
import com.hp.hpl.jena.rdf.model.Resource;

import es.gnossdeusto.backend.validation.DataGenerator;

import org.junit.BeforeClass;
import org.junit.Ignore;
import org.junit.Test;

public class ValidationQuestionsTest {
	
	private static Model data;
	
	@BeforeClass
	public static void loadModel() {
		data = DataGenerator.getModel(System.getProperty("ontFile"), System.getProperty("dataFile"), System.getProperty("uneskos"));
		// data = DataGenerator.getInferredModel(System.getProperty("ontFile"), System.getProperty("dataFile"));
	}
	
	private static void writeSPARQLQuery(String method, String query) {
		FileWriter myWriter;
		try {
			myWriter = new FileWriter(String.format("sparql-query/%s.sparql", method));
		    myWriter.write(query);
		    myWriter.close();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	/**
	 * Centros de  investigación que trabajan en un área/disciplina específica.
	 */
	@Test
	public void Q1() {
		List<String> expectedResult = new ArrayList<String>();
		expectedResult.add("http://purl.org/roh/data#centro-investigacion-2");
		expectedResult.add("http://purl.org/roh/data#centro-investigacion-1");
		
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX uneskos: <http://purl.org/roh/unesco-individuals#>\n" +
				"\n" + 
				"SELECT ?centro WHERE {\n" + 
				"        ?centro a roh:ResearchGroup ;\n" + 
				"                          roh:hasKnowledgeArea uneskos:120304 .\n" + 
				"}";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String> result = new ArrayList<String>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
			      QuerySolution soln = results.nextSolution() ;
			      Resource centro = soln.getResource("centro");
			      result.add(centro.getURI());
			    }
		}
		assertThat(expectedResult, equalTo(result));
	}
	
	/**
	 * Listado de los investigadores de un centro/estructura de investigación de un área/disciplina específica y su posición.
	 */
	@Test
	public void Q2() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#investigador-2", "http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh#ResearcherPosition"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#investigador-1", "http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh#ResearcherPosition"});
		
		String queryString = "PREFIX foaf: <http://purl.org/roh/mirror/foaf#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX vivo:  <http://purl.org/roh/mirror/vivo#>\n" +
				"PREFIX unesco: <http://purl.org/roh/unesco-individuals#>\n" + 
				"PREFIX ro: <http://purl.org/obo/owl/ro#>\n" + 
				"PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>\n" + 
				"\n" + 
				"SELECT ?researcher ?center ?positionClass\n" + 
				"WHERE {\n" + 
				"  ?researcher	roh:hasKnowledgeArea unesco:120304 ;\n" + 
				"  				roh:hasPosition ?position .\n" + 
				"  ?position vivo:relates ?center ;\n" + 
				"  			a	?positionClass .\n" +
				"  ?center a roh:ResearchGroup .\n" + 
				"FILTER NOT EXISTS {\n" +
				"?position a ?otherClass .\n" + 
				"?otherClass rdfs:subClassOf ?positionClass .\n" +
				"FILTER (?otherClass != ?positionClass)\n" +
				"}\n"+
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			// ResultSetFormatter.out(System.out, results, query);
			 for ( ; results.hasNext() ; )
			    {
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource researcher = soln.getResource("researcher");
			      Resource center = soln.getResource("center");
			      Resource positionClass = soln.getResource("positionClass");
			      result.add(new String[]{researcher.getURI(), center.getURI(), positionClass.getURI()});
			    }
		}
		
		assertEquals(2, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
		assertThat(result, hasItem(expectedResult.get(1)));
	}
	
	@Ignore
	@Test
	public void Q3() {
		fail("Test not implemented");
	}
	
	/**
	 * Centros/estructuras de investigación que posean sellos de calidad asociados.
	 */
	@Test
	public void Q4() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", "Grupo reconocido por el Gobierno Vasco"});
		
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" +  
				"SELECT ?center ?accreditationTitle\n" + 
				"WHERE {\n" + 
				" ?center a roh:ResearchGroup ;\n" + 
				"         roh:hasAccreditation ?accreditation .\n" + 
				" ?accreditation roh:title ?accreditationTitle\n" + 
				"}";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource center = soln.getResource("center");
			      Literal accreditationTitle = soln.getLiteral("accreditationTitle");
			      
			      result.add(new String[]{center.getURI(), accreditationTitle.getString()});
			    }
		}
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
	}
	
	/**
	 * Listado de los centros/estructuras de investigación que hayan realizado proyectos y su respectiva convocatoria.
	 */
	@Test
	public void Q5() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/data#hazitek2020"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/data#european-funding-program"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-3", "http://purl.org/roh/data#european-funding-program"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", null});

		
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX ro: <http://purl.org/roh/mirror/obo/ro#>\n" + 
				"PREFIX bibo: <http://purl.org/ontology/bibo/>\n" + 
				"PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>\n" + 
				"PREFIX foaf: <http://purl.org/roh/mirror/foaf#>\n" + 
				"PREFIX unesco: <http://purl.org/roh/unesco-individuals#>\n" + 
				"PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>\n" + 
				"PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>\n" +
				"\n" + 
				"SELECT DISTINCT ?centro ?fundingProgram ?project\n" + 
				"WHERE {\n" + 
				"  ?fundingAmount roh:grants ?centro .\n" + 
				"  ?funding ro:hasPart ?fundingAmount ;\n" + 
				"  roh:fundedBy ?fundingProgram ;\n" + 
				"   roh:supports ?project .\n" +
				"OPTIONAL {\n" +
				"?project roh:projectStatus ?projectStatus .\n" +
				"}\n" +
				"FILTER (!BOUND(?projectStatus) || ?projectStatus != \"PROPOSAL_SUBMITTED\")\n" +
				"}";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			// ResultSetFormatter.out(System.out, results, query);
			 for ( ; results.hasNext() ; )
			    {
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource centro = soln.getResource("centro");
			      Resource fundingProgram = soln.getResource("fundingProgram");
			      
			      result.add(new String[]{centro.getURI(), fundingProgram.getURI()});
			    }
		}
			
		assertEquals(4, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
		assertThat(result, hasItem(expectedResult.get(1)));
		assertThat(result, hasItem(expectedResult.get(2)));
		assertThat(result, hasItem(expectedResult.get(3)));
	}
	
	/**
	 * Listado de la producción científica en un determinado rango de fechas de un centro/estructura de investigación en un área/disciplina.
	 */
	@Test
	public void Q6() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/data#journal-article-1", "http://purl.org/roh/unesco-individuals#1203"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/data#my-fabulous-patent", "http://purl.org/roh/unesco-individuals#1203"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/data#another-fabulous-patent", "http://purl.org/roh/unesco-individuals#1203"});
		
		String queryString =  "PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX bibo:  <http://purl.org/roh/mirror/bibo#>\n" +
				"PREFIX rdfs:  <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"PREFIX foaf: <http://purl.org/roh/mirror/foaf#>\n" + 
				"PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>\n" +
				"SELECT ?organization ?researchObject ?knowledgeArea \n" +
				"WHERE {\n" +
				"?researchObject a roh:ResearchObject ;\n" +
				"vivo:dateIssued ?dateIssued ;\n" +
				"roh:hasKnowledgeArea ?knowledgeArea ;\n" +
				"bibo:authorList ?authorList .\n" +
				"?authorList rdfs:member ?author .\n" +
				"?author roh:hasPosition ?position .\n" + 
				"?organization a foaf:Organization .\n" +
				"?position vivo:relates ?organization .\n" +
				"?dateIssued vivo:dateTime ?dateTime .\n" +
				"FILTER (YEAR(?dateTime) >= \"2010\"^^xsd:integer && YEAR(?dateTime) <= \"2020\"^^xsd:integer)\n" +
				"} GROUP BY ?organization ?researchObject ?knowledgeArea\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
//				 ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource organization = soln.getResource("organization");
			      Resource researchObject = soln.getResource("researchObject");
			      Resource knowledgeArea = soln.getResource("knowledgeArea");
			      
			      result.add(new String[]{organization.getURI(), researchObject.getURI(), knowledgeArea.getURI()});
			    }
		}
		assertEquals(3, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
		assertThat(result, hasItem(expectedResult.get(1)));
		assertThat(result, hasItem(expectedResult.get(2)));
	}
	
	/**
	 * Artículos publicados en revistas, según las comunidades autónomas. Por las limitaciones de Jena ARQ, la consulta para conocer la 
	 * comunidad autónoma se ha realizado de manera independiente.
	 */
	@Test
	public void Q7() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"1", "https://sws.geonames.org/3336903/"});
		
		String queryString = "PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX obo: <http://purl.obolibrary.org/obo/>\n" + 
				"PREFIX bibo: <http://purl.org/roh/mirror/bibo#>\n" + 
				"PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>\n" + 
				"PREFIX foaf: <http://purl.org/roh/mirror/foaf#>\n" + 
				"PREFIX unesco: <http://purl.org/roh/unesco-individuals#>\n" + 
				"PREFIX ro: <http://purl.org/obo/owl/ro#>\n" + 
				"PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>\n" + 
				"PREFIX ero: <http://purl.org/roh/mirror/obo/ero#>\n" + 
				"PREFIX gn: <http://purl.org/roh/mirror/geonames#>\n" + 
				"PREFIX iao: <http://purl.org/roh/mirror/obo/iao#>\n" + 
				"PREFIX dc: <http://purl.org/dc/elements/1.1/>\n" + 
				"\n" + 
				"SELECT\n" + 
				"(COUNT(DISTINCT ?publication) as ?publicationCount) ?location  \n" +
				"WHERE {\n" + 
				"        ?publication         a iao:IAO_0000013 ;\n" + 
				"                              vivo:hasPublicationVenue        ?journal ;\n" + 
				"                        bibo:authorList        ?authorList .\n" + 
				"          ?journal        a bibo:Journal        ;\n" + 
				"                    vivo:dateIssued        ?dateTimeValue .            \n" + 
				"          ?authorList        rdfs:member        ?author .\n" + 
				"          ?dateTimeValue        vivo:dateTime        ?dateTime .\n" + 
				"          ?author vivo:relatedBy        ?position ;\n" + 
				"                   a        foaf:Person .\n" + 
				"          ?position         a        ?positionClass        ;\n" + 
				"                                    vivo:relates        ?organization .\n" + 
//				"          ?positionClass        rdfs:subClassOf        vivo:Position        .\n" + 
				"          ?organization        a ?organizationClass ;\n" + 
				"                         gn:locatedIn        ?location .\n" + 
				"\n" + 
				"} \n" +
				"GROUP BY ?location";
		// writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		Map<Resource, Integer> locationPubs = new HashMap<Resource, Integer>();

		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			     
			      Literal publicationCount = soln.getLiteral("publicationCount");
			      Resource location = soln.getResource("location");
			      
		    	  String geonamesQueryString = "SELECT ?comunidadAutonoma \n" +
		    			  "WHERE {\n" +
		    			  "?s <http://www.geonames.org/ontology#parentADM1> ?comunidadAutonoma" +
		    			  "}\n";
		    	  
		    	  Query geonamesQuery = QueryFactory.create(geonamesQueryString) ;
		    	  
		    	  Model geonamesData = ModelFactory.createDefaultModel();
		    	  String datasetURL = String.format("%sabout.rdf", location.getURI());
		    	  geonamesData.read(datasetURL);
		    	  
		    	  try (QueryExecution qexecGeonames = QueryExecutionFactory.create(geonamesQuery, geonamesData)) {
		    		  ResultSet geonamesResults = qexecGeonames.execSelect();
		    		  for ( ; geonamesResults.hasNext() ; ) {
		    			  QuerySolution geonamesSoln = geonamesResults.nextSolution() ;
		    			  // ResultSetFormatter.out(System.out, geonamesResults, geonamesQuery);
		    			  
		    			  Resource comunidadAutonoma = geonamesSoln.getResource("comunidadAutonoma");
		    			  
		    			  if (!locationPubs.containsKey(comunidadAutonoma)) {
		    				  locationPubs.put(comunidadAutonoma, 0);
		    			  }
		    			  locationPubs.put(comunidadAutonoma, locationPubs.get(comunidadAutonoma) + publicationCount.getInt());
		    			  
		    		  }
		    		  
		    	  }		     
			    }
		}
		
		 for (Resource key : locationPubs.keySet()) {
			 result.add(new String[]{locationPubs.get(key).toString(), key.getURI()});
		 }
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
	}
	
	/**
	 * Producción científica (ResearchObjects) de un grupo de investigación agrupados por tipo, es decir, publicaciones, patentes.
	 */
	@Test
	public void Q8() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/data#my-fabulous-patent", "http://purl.org/roh/mirror/bibo#Patent"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/data#journal-article-1", "http://purl.org/roh/mirror/obo/iao#IAO_0000013"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/data#another-fabulous-patent", "http://purl.org/roh/mirror/bibo#Patent"});

		
		String queryString = "PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" +  
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX bibo: <http://purl.org/roh/mirror/bibo#>\n" + 
				"PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>\n" + 
				"SELECT DISTINCT ?researchGroup ?researchObject ?researchObjectClass \n" +
				"WHERE {\n" +
				"?researchObject a roh:ResearchObject ;\n" +
				"a ?researchObjectClass ;\n" +
				"bibo:authorList ?authorList .\n" +
				"?authorList ?order ?author .\n" +
				"?author roh:hasPosition ?position .\n" +
				"?position vivo:relates ?researchGroup .\n" +
				"?researchGroup a roh:ResearchGroup .\n" +
				"FILTER NOT EXISTS {\n" +
				"?researchObject a ?otherClass .\n" + 
				"?otherClass rdfs:subClassOf ?researchObjectClass .\n" +
				"FILTER (?otherClass != ?researchObjectClass)\n" +
				"}\n"+
				"FILTER (str(?researchObjectClass) != \"http://purl.org/roh#ResearchObject\")\n" +
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  //ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource researchGroup = soln.getResource("researchGroup");
			      Resource researchObject = soln.getResource("researchObject");
			      Resource researchObjectClass = soln.getResource("researchObjectClass");
			      
			      result.add(new String[]{researchGroup.getURI(), researchObject.getURI(), researchObjectClass.getURI()});
			    }
		}
		
		assertEquals(3, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
		assertThat(result, hasItem(expectedResult.get(1)));
		assertThat(result, hasItem(expectedResult.get(2)));
	}
	
	/**
	 * Listado de patentes, diseños industriales, etc. de un centro/estructura de investigación en un área/disciplina.
	 */
	@Test 
	public void Q9 () {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#my-fabulous-patent", "http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/unesco-individuals#1203"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#another-fabulous-patent", "http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/unesco-individuals#1203"});
		
		String queryString = "PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX bibo: <http://purl.org/roh/mirror/bibo#>\n" + 
				"PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>\n" + 
				"SELECT ?patent ?centre ?knowledgeArea \n" +
				"WHERE {\n" +
				"?patent a bibo:Patent ;\n" +
				"bibo:authorList ?authorList .\n" +
				"?patent roh:hasKnowledgeArea ?knowledgeArea .\n" +
				"?authorlist rdfs:member ?author .\n" +
				"?author roh:hasPosition ?position .\n" + 
				"?position vivo:relates ?centre .\n" +
				"?centre a roh:ResearchGroup .\n" +
				"} GROUP BY ?patent ?centre ?knowledgeArea";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				 // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource patent = soln.getResource("patent");
			      Resource centre = soln.getResource("centre");
			      Resource knowledgeArea = soln.getResource("knowledgeArea");
			      
			      result.add(new String[]{patent.getURI(), centre.getURI(), knowledgeArea.getURI()});
			    }
		}
		assertEquals(2, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
		assertThat(result, hasItem(expectedResult.get(1)));
	
	}
	
	/**
	 * Listado de los proyectos adjudicados/desarrollados, de un centro/estructura de investigación, de un área/disciplina, en un determinado año de búsqueda.
	 */
	@Test
	public void Q10() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#a-project", "http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/unesco-individuals#1203"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#a-project", "http://purl.org/roh/data#company-one", "http://purl.org/roh/unesco-individuals#1203"});
		
		String queryString = "PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>\n" +
				"PREFIX rdfs:  <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"PREFIX foaf: <http://purl.org/roh/mirror/foaf#>\n" + 
				"PREFIX bfo: <http://purl.org/roh/mirror/obo/bfo#>\n" +
				"SELECT DISTINCT ?project ?centre ?knowledgeArea \n" +
				"WHERE {\n" +
				"?project a vivo:Project ;\n" +
				"roh:hasKnowledgeArea ?knowledgeArea ;\n" +
				"vivo:relates ?role ;\n" +
				"vivo:dateTimeInterval ?dateTimeInterval .\n" +
				"?role a ?roleClass ;\n" + 
				"roh:roleOf ?centre .\n" +
				"?roleClass rdfs:subClassOf* bfo:BFO_0000023 .\n" +
				"?centre a ?organization .\n" +
				"?organization rdfs:subClassOf* foaf:Organization .\n" +
				"?dateTimeInterval vivo:start ?startDateTimeValue ;\n" +
				"vivo:end ?endDateTimeValue . \n" +
				"?startDateTimeValue vivo:dateTime ?start . \n" +
				"?endDateTimeValue vivo:dateTime ?end . \n" +
				"FILTER (YEAR(?start) <= \"2019\"^^xsd:integer && YEAR(?end) >= \"2019\"^^xsd:integer )\n" +
				"FILTER NOT EXISTS {\n" +
				"?role a ?otherClass .\n" + 
				"?otherClass rdfs:subClassOf ?roleClass .\n" +
				"FILTER (?otherClass != ?roleClass)\n" +
				"}\n"+
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource project = soln.getResource("project");
			      Resource centre = soln.getResource("centre");
			      Resource knowledgeArea = soln.getResource("knowledgeArea");
			      
			      result.add(new String[]{project.getURI(), centre.getURI(), knowledgeArea.getURI()});
			    }
		}
		
		assertEquals(2, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
		assertThat(result, hasItem(expectedResult.get(1)));
	}
	
	/**
	 * Encontrar el research object más antiguo organización. 
	 */
	@Test
	public void Q11A() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[] {"http://purl.org/roh/data#another-fabulous-patent", "2015-02-12T00:00:00"});
		
		String queryString = "PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX foaf: <http://purl.org/roh/mirror/foaf#>\n" + 
				"PREFIX bibo: <http://purl.org/roh/mirror/bibo#>\n" + 
				"SELECT DISTINCT ?researchObject ?date \n" +
				"WHERE {\n" +
				"?researchObject a roh:ResearchObject ;\n" +
				"vivo:dateIssued ?dateIssued ;\n" +
				"roh:hasKnowledgeArea ?knowledgeArea ;\n" +
				"bibo:authorList ?authorList .\n" +
				"?dateIssued vivo:dateTime ?date .\n" +
				"?authorList ?p ?author .\n" +
				"?author a foaf:Person ;\n" +
				"roh:hasPosition ?position .\n" + 
				"?position vivo:relates <http://purl.org/roh/data#centro-investigacion-1> .\n" +
				"} ORDER BY ASC(?date) LIMIT 1\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource researchObject = soln.getResource("researchObject");
			      Literal date = soln.getLiteral("date");
			      
			      result.add(new String[]{researchObject.getURI(), date.getString()});
			    }
		}
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
	}
	
	/**
	 * Encontrar el research object más reciente de una organización. 
	 */
	@Test
	public void Q11B() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[] {"http://purl.org/roh/data#journal-article-1", "2020-04-27T00:00:00"});
		
		String queryString = "PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX foaf: <http://purl.org/roh/mirror/foaf#>\n" + 
				"PREFIX bibo: <http://purl.org/roh/mirror/bibo#>\n" + 
				"SELECT DISTINCT ?researchObject ?date \n" +
				"WHERE {\n" +
				"?researchObject a roh:ResearchObject ;\n" +
				"vivo:dateIssued ?dateIssued ;\n" +
				"roh:hasKnowledgeArea ?knowledgeArea ;\n" +
				"bibo:authorList ?authorList .\n" +
				"?dateIssued vivo:dateTime ?date .\n" +
				"?authorList ?p ?author .\n" +
				"?author a foaf:Person ;\n" +
				"roh:hasPosition ?position .\n" + 
				"?position vivo:relates <http://purl.org/roh/data#centro-investigacion-1> .\n" +
				"} ORDER BY DESC(?date) LIMIT 1\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource researchObject = soln.getResource("researchObject");
			      Literal date = soln.getLiteral("date");
			      
			      result.add(new String[]{researchObject.getURI(), date.getString()});
			    }
		}
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
	}
	
	/**
	 * Listar proyectos agrupados por ámbito geográfico.
	 */
	@Test
	public void Q12() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#a-project", "http://purl.org/roh/data#centro-investigacion-1", "https://sws.geonames.org/3336903/"});
		
		String queryString = "PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" +
				"PREFIX gn:	<http://purl.org/roh/mirror/geonames#>\n" +
				"SELECT DISTINCT ?project ?centre ?location \n" +
				"WHERE {\n" +
				"?project a vivo:Project ; \n" +
				"gn:locatedIn ?location ;\n" +
				"vivo:relates ?role .\n" +
				"?role a vivo:MemberRole ;\n" + 
				"roh:roleOf ?centre .\n" +
				"?centre a roh:ResearchGroup .\n" +
				"OPTIONAL { ?project roh:projectStatus ?projectStatus .\n" +
				"FILTER (?projectStatus != \"PROPOSAL_SUBMITTED\")" +
				"}\n" +
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource project = soln.getResource("project");
			      Resource centre = soln.getResource("centre");
			      Resource location = soln.getResource("location");
    			  result.add(new String[] {project.getURI(), centre.getURI(), location.getURI()});

			    }
			      
		    }
		
//			for (Resource key : locationPubs.keySet()) {
//				 result.add(new String[]{locationPubs.get(key).toString(), key.getURI()});
//			 }

		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));

	}
	
	/**
	 * Dado un proyecto listar los documentos de su dossier
	 */
	@Test
	public void Q13() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#a-project-justification", "http://purl.org/roh#Justification"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#a-project-research-proposal", "http://purl.org/roh/mirror/vivo#ResearchProposal"});


		String queryString = "PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX bibo:  <http://purl.org/roh/mirror/bibo#>\n" +
				"PREFIX roh: <http://purl.org/roh#>\n" +
				"PREFIX rdfs:  <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"SELECT  ?document ?documentType\n" +
				"WHERE {\n" +
				"?dossier a roh:Dossier .\n" +
				"?dossier vivo:relates <http://purl.org/roh/data#a-project>, ?document .\n" +
				"?document a ?documentType .\n" +
				"?documentType rdfs:subClassOf bibo:Report ." +
				"FILTER NOT EXISTS {\n" +
				"?document a ?otherClass .\n" + 
				"?otherClass rdfs:subClassOf ?documentType .\n" +
				"FILTER (?otherClass != ?documentType)\n" +
				"}\n"+
				"} \n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
//				  ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource document = soln.getResource("document");
			      Resource documentType = soln.getResource("documentType");
			      
			      result.add(new String[]{document.getURI(), documentType.getURI()});
			    }
		}
		
		assertEquals(2, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
		assertThat(result, hasItem(expectedResult.get(1)));
	}
	
	/**
	 * Diferentes consultas sobre proyectos.
	 */
	@Test
	public void Q14() {
		Q5();
		Q10();
		Q12();
	}
	
	/*
	 * Listar proyectos con el mismo subject area o con subject areas relacionadas por parentesco, mirando en el árbol UNESKOS.
	 */
	@Test
	public void Q15() {
		List<String> expectedResult = new ArrayList<String>();
		expectedResult.add("http://purl.org/roh/data#a-project");
		expectedResult.add("http://purl.org/roh/data#another-project");
		expectedResult.add("http://purl.org/roh/data#another-great-project");
		
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" +
				"PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX uneskos: <http://purl.org/roh/unesco-individuals#>\n" +
				"PREFIX skos: <http://www.w3.org/2004/02/skos/core#>\n" +
				"SELECT DISTINCT ?project \n" +
				"WHERE {\n" +
				"{\n" +
				"?project a vivo:Project ;\n" +
				"roh:hasKnowledgeArea ?knowledgeArea .\n" +
				"?knowledgeArea skos:broader+|skos:narrower+|skos:related+ uneskos:1203 .\n" +
				"} UNION {\n" +
					"uneskos:1203 skos:broader+ ?topKnowledgeArea .\n" +
					"?topKnowledgeArea skos:narrower+ ?otherKnowledgeArea .\n" +
					"?project roh:hasKnowledgeArea ?otherKnowledgeArea ;\n" +
					"a vivo:Project .\n" +
				"}\n" +
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String> result = new ArrayList<String>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource project = soln.getResource("project");
			      
			      result.add(project.getURI());
			    }
		}
		
		assertThat(result, containsInAnyOrder(expectedResult.toArray()));
	}
	
	/**
	 * Dada una persona listar proyectos en los que ha intervenido filtrados por periodo y/o organización
	 */
	@Test
	public void Q16A() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#investigador-1", "http://purl.org/roh/data#a-project", "http://purl.org/roh/data#centro-investigacion-1"});
		
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" +
				"PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX foaf: <http://purl.org/roh/mirror/foaf#>\n" + 
				"PREFIX rdfs:  <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>\n" +
				"SELECT DISTINCT ?researcher ?project ?organization \n" +
				"WHERE {\n" +
				"?project a vivo:Project ;\n" +
				"vivo:relates ?role ;\n" +
				"vivo:dateTimeInterval ?dateTimeInterval .\n" +
				"?dateTimeInterval vivo:end ?endDateTimeValue ;\n" +
				"vivo:start ?startDateTimeValue .\n" +
				"?endDateTimeValue vivo:dateTime ?end .\n" +
				"?startDateTimeValue vivo:dateTime ?start .\n" +
				"?role roh:roleOf ?researcher .\n" +
				"?researcher roh:hasPosition ?position .\n" +
				"?position vivo:relates ?organization .\n" +
				"?organization a ?organizationClass .\n" +
				"?organizationClass rdfs:subClassOf foaf:Organization .\n" +
				"FILTER (YEAR(?start) <= \"2019\"^^xsd:integer && YEAR(?end) >= \"2019\"^^xsd:integer )\n" +
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource researcher = soln.getResource("researcher");
			      Resource project = soln.getResource("project");
			      Resource organization = soln.getResource("organization");
			      
			      result.add(new String[]{researcher.getURI(), project.getURI(), organization.getURI()});
			    }
		}
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
		
	}
	
	/**
	 * 	Dada una persona listar research objects a los que ha contribuido, filtrados por periodo y/o organización.
	 */
	@Test
	public void Q16B() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#investigador-1", "http://purl.org/roh/data#journal-article-1", "http://purl.org/roh/data#centro-investigacion-1"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#investigador-1", "http://purl.org/roh/data#my-fabulous-patent", "http://purl.org/roh/data#centro-investigacion-1"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#investigador-3", "http://purl.org/roh/data#journal-article-1", "http://purl.org/roh/data#centro-investigacion-1"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#investigador-3", "http://purl.org/roh/data#my-fabulous-patent", "http://purl.org/roh/data#centro-investigacion-1"});

		
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" +
				"PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX foaf: <http://purl.org/roh/mirror/foaf#>\n" + 
				"PREFIX rdfs:  <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>\n" +
				"PREFIX bibo: <http://purl.org/roh/mirror/bibo#>\n" + 
				"SELECT DISTINCT ?researcher ?researchObject ?organization \n" +
				"WHERE {\n" +
				"?researchObject a roh:ResearchObject ;\n" +
				"bibo:authorList ?authorList ;\n" +
				"vivo:dateIssued ?dateTimeValue .\n" +
				"?dateTimeValue vivo:dateTime ?date .\n" +
				"?authorList rdfs:member ?researcher .\n" +
				"?researcher roh:hasPosition ?position .\n" +
				"?position vivo:relates ?organization .\n" +
				"?organization a ?organizationClass .\n" +
				"?organizationClass rdfs:subClassOf foaf:Organization .\n" +
				"FILTER (YEAR(?date) >= \"2019\"^^xsd:integer && YEAR(?date) <= \"2020\"^^xsd:integer )\n" +
				"}\n ORDER BY ?researcher";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource researcher = soln.getResource("researcher");
			      Resource researchObject = soln.getResource("researchObject");
			      Resource organization = soln.getResource("organization");
			      
			      result.add(new String[]{researcher.getURI(), researchObject.getURI(), organization.getURI()});
			    }
		}
		
		assertEquals(4, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
		
	}
	
	/**
	 *  Dada una organización, en un periodo de tiempo, listar research projects o proyectos.
	 */
	@Test
	public void Q17() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/data#a-project"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#company-one", "http://purl.org/roh/data#a-project"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/data#another-collaborative-project"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-3", "http://purl.org/roh/data#another-collaborative-project"});

		
		String queryString = "PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>\n" +
				"PREFIX rdfs:  <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"PREFIX foaf: <http://purl.org/roh/mirror/foaf#>\n" + 
				"SELECT  DISTINCT ?organization ?project \n" +
				"WHERE {\n" +
				"?organization a ?organizationClass .\n" +
				"?organizationClass rdfs:subClassOf* foaf:Organization .\n" +
				"?role roh:roleOf ?organization ;\n" + 
				"vivo:relatedBy ?project .\n" + 
				"?project a vivo:Project ;\n" +
				"vivo:dateTimeInterval ?dateTimeInterval .\n" +
				"?dateTimeInterval vivo:start ?startDateTimeValue ;\n" +
				"vivo:end ?endDateTimeValue . \n" +
				"?startDateTimeValue vivo:dateTime ?start . \n" +
				"?endDateTimeValue vivo:dateTime ?end . \n" +
				"FILTER (YEAR(?start) <= \"2019\"^^xsd:integer && YEAR(?end) >= \"2019\"^^xsd:integer )\n" +
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource project = soln.getResource("project");
			      Resource organization = soln.getResource("organization");
			      
			      result.add(new String[]{organization.getURI(), project.getURI()});
			    }
		}
		
		assertEquals(4, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
		assertThat(result, hasItem(expectedResult.get(1)));
		assertThat(result, hasItem(expectedResult.get(2)));
		assertThat(result, hasItem(expectedResult.get(3)));

	}
	
	/**
	 * Contar las publicaciones con índice de impacto de un usuario en un periodo de tiempo.
	 */
	@Test
	public void Q18() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#investigador-1", "1"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#investigador-3", "1"});
		
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" +
				"PREFIX bibo: <http://purl.org/roh/mirror/bibo#>\n" + 
				"PREFIX rdfs:  <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"SELECT ?researcher (COUNT(?metric) as ?count)\n" +
				"WHERE {\n" +
				"?researchObject a roh:ResearchObject ;\n" +
				"bibo:authorList ?authorList ;\n" +
				"roh:hasMetric ?metric .\n" +
				"?metric roh:impactFactor ?impactFactor .\n" +
				"?authorList rdfs:member ?researcher .\n" +
				"} GROUP BY ?researcher \n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource researcher = soln.getResource("researcher");
			      Literal count = soln.getLiteral("count");
			      
			      result.add(new String[]{researcher.getURI(), count.getString()});
			    }
		}
		
		assertEquals(2, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
		assertThat(result, hasItem(expectedResult.get(1)));
	}
	
	@Ignore
	@Test
	public void Q19() {
		fail("Test not implemented");
	}
	
	/**
	 * Listar las áreas de conocimiento más abordadas por una organización en proyectos o publicaciones en un periodo de tiempo.
	 */
	@Test
	public void Q20() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/unesco-individuals#1203", "4"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#company-one", "http://purl.org/roh/unesco-individuals#1203", "1"});
		
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" +
				"PREFIX foaf: <http://purl.org/roh/mirror/foaf#>\n" + 
				"PREFIX rdfs:  <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"PREFIX bfo: <http://purl.org/roh/mirror/obo/bfo#>\n" +
				"PREFIX bibo:  <http://purl.org/roh/mirror/bibo#>\n" +
				"PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>\n" +
				"SELECT ?organization ?knowledgeArea (count(DISTINCT ?item) as ?count) \n" +
				"WHERE {\n" +
				"SELECT ?date ?item ?organization ?knowledgeArea \n" +
				"WHERE{\n" +
				"{\n" +
				"?item a vivo:Project ;\n" + 
				"vivo:dateTimeInterval ?dateTimeInterval ;\n" +
				"roh:hasKnowledgeArea ?knowledgeArea ;\n" +
				"vivo:relates ?role .\n" +
				"?dateTimeInterval vivo:end ?endDateTimeValue ;\n" +
				"vivo:start ?startDateTimeValue .\n" +
				"?endDateTimeValue vivo:dateTime ?end .\n" +
				"?startDateTimeValue vivo:dateTime ?start .\n" +
				"?role a ?roleClass ;\n" +
				"roh:roleOf ?organization .\n" +
				"?roleClass rdfs:subClassOf* bfo:BFO_0000023 .\n" + 
				"?organization a ?organizationClass .\n" +
				"?organizationClass rdfs:subClassOf* foaf:Organization .\n" +
				"FILTER (YEAR(?start) <= \"2019\"^^xsd:integer && YEAR(?end) >= \"2019\"^^xsd:integer )\n" +
				"}\n" + 
				"UNION {\n" +
				"?item a ?publicationClass ;\n" +
				"vivo:dateIssued ?dateTimeValue ;\n" +
				"roh:hasKnowledgeArea ?knowledgeArea ;\n" +
				"bibo:authorList ?authorList .\n" +
				"?publicationClass rdfs:subClassOf* bibo:Document .\n" +
				"?authorList ?order ?author .\n" +
				"?author roh:hasPosition ?position .\n" +
				"?position vivo:relates ?organization .\n" +
				"?organization a ?organizationClass .\n" +
				"?organizationClass rdfs:subClassOf* foaf:Organization .\n" +
				"?dateTimeValue vivo:dateTime ?date .\n" +
				"FILTER (YEAR(?date) <= \"2019\"^^xsd:integer || YEAR(?date) >= \"2020\"^^xsd:integer )\n" +
				"}\n" +
				"}\n" +
				"} GROUP BY ?organization ?knowledgeArea ORDER BY ?count\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource knowledgeArea = soln.getResource("knowledgeArea");
			      Resource organization = soln.getResource("organization");
			      Literal count = soln.getLiteral("count");
			      
			      result.add(new String[]{organization.getURI(), knowledgeArea.getURI(), count.getString()});
			    }
		}
		
		assertEquals(2, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
		assertThat(result, hasItem(expectedResult.get(1)));
	}
	
	/**
	 * Contar research objects de diferentes tipos o proyectos a nivel persona, línea, área de conocimiento u organización.
	 */
	@Test
	public void Q21() {
		Q6();
		Q8();
		Q11A();
		Q11B();
		Q16B();
	}
	
	@Ignore
	@Test
	public void Q22() {
		fail("Test not implemented");
	}
	
	/**
	 * Ordenar por contador de pubicaciones de impacto o proyectos europeos línea de investigación asociados a diferentes líneas o áreas de conocimiento.
	 */
	@Test
	public void Q23() {
		Q20();
	}
	
	/**
	 * Dada una empresa encontrar los proyectos en los que ha colaborado con los grupos de una universidad.
	 */
	@Test
	public void Q24() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#a-private-project", "http://purl.org/roh/data#centro-investigacion-1"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#a-project", "http://purl.org/roh/data#centro-investigacion-1"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#a-project-in-negotiation", "http://purl.org/roh/data#centro-investigacion-1"});

		String queryString = "PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"SELECT DISTINCT ?project ?centre \n" +
				"WHERE {\n" +
				"?project a vivo:Project ;\n" +
				"vivo:relates ?role ;\n" + 
				"vivo:relates ?centreRole .\n" +
				"?role roh:roleOf <http://purl.org/roh/data#company-one> .\n" +
				"?centreRole roh:roleOf ?centre .\n" +
				"?centre a roh:ResearchGroup .\n" +
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource project = soln.getResource("project");
			      Resource centre = soln.getResource("centre");
			      
			      result.add(new String[]{project.getURI(), centre.getURI()});
			    }
		}
		
		assertEquals(3, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
		assertThat(result, hasItem(expectedResult.get(1)));
		assertThat(result, hasItem(expectedResult.get(2)));
	}
	/**
	* Obtener el listado de las tesis doctorales que he dirigido.
	*/
	@Test
	public void Q25() {
		List<String> expectedResult = new ArrayList<String>();
		expectedResult.add("http://purl.org/roh/data#investigador-3-phd-thesis");
		
		String queryString = "PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX ro: <http://purl.org/roh/mirror/obo/ro#>\n" + 
				"\n" + 
				"SELECT ?thesis \n" + 
				"WHERE {\n" + 
				"  ?supervisorRole a roh:SupervisorRole ;\n" + 
				"  ro:RO_0000052 <http://purl.org/roh/data#investigador-1> ;\n" + 
				"        vivo:relatedBy ?supervisingRelationship .\n" + 
				"  ?supervisingRelationship a roh:PhDSupervisingRelationship ;\n" +
				" roh:produces ?thesis" +
				"}";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String> result = new ArrayList<String>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource thesis = soln.getResource("thesis");
			      
			      result.add(thesis.getURI());
			    }
		}
		assertThat(expectedResult, equalTo(result));	
	}
	
	/**
	* Obtener el listado de congresos/workshops y eventos de divulgación científica en los que haya participado indicando el rol que he tenido: organizador, expositor, etc.
	*/
	@Test
	public void Q26() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#investigador-2", "http://purl.org/roh/data#a-great-conference", "http://purl.org/roh/mirror/vivo#AttendeeRole"});
		
		String queryString = "PREFIX ro: <http://purl.org/roh/mirror/obo/ro#>\n" +
				"PREFIX bibo:  <http://purl.org/roh/mirror/bibo#>\n" +
				"PREFIX rdfs:  <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"PREFIX bfo: <http://purl.org/roh/mirror/obo/bfo#>\n" +
				"SELECT ?researcher ?conference ?roleClass \n" +
				"WHERE {\n" +
				"?conference a bibo:Conference ;\n" +
				"bfo:BFO_0000055 ?role .\n" + 
				"?role a ?roleClass ;\n" +
				"ro:RO_0000052 ?researcher .\n" +
				"FILTER NOT EXISTS {\n" +
				"?role a ?otherClass .\n" + 
				"?otherClass rdfs:subClassOf ?roleClass .\n" +
				"FILTER (?otherClass != ?roleClass)\n" +
				"}\n"+
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource researcher = soln.getResource("researcher");
			      Resource conference = soln.getResource("conference");
			      Resource roleClass = soln.getResource("roleClass");
			      
			      result.add(new String[]{researcher.getURI(), conference.getURI(), roleClass.getURI()});
			    }
		}
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
	}
	
	/**
	* Obtener el listado de patentes, diseños industriales, etc. que haya registrado como titular o cotitular X o Y persona, Z o K institución
	*/
	@Test
	public void Q27() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#another-fabulous-patent", "http://purl.org/roh/data#investigador-3", "http://purl.org/roh/data#centro-investigacion-1"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#my-fabulous-patent", "http://purl.org/roh/data#investigador-1", "http://purl.org/roh/data#centro-investigacion-1"});

		String queryString = "PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX bibo:  <http://purl.org/roh/mirror/bibo#>\n" +
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"SELECT ?patent ?researcher ?center \n" +
				"WHERE {\n" +
				"?patent a bibo:Patent ;\n" +
				"roh:correspondingAuthor ?researcher .\n" +
				"?researcher roh:hasPosition ?position .\n" + 
				"?position vivo:relates ?center .\n" + 
				"?center a roh:ResearchGroup .\n" + 
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource researcher = soln.getResource("researcher");
			      Resource patent = soln.getResource("patent");
			      Resource center = soln.getResource("center");
			      
			      result.add(new String[]{patent.getURI(), researcher.getURI(), center.getURI()});
			    }
		}
		assertEquals(2, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
		assertThat(result, hasItem(expectedResult.get(1)));
	}
	
	/**
	* Obtener el listado de proyectos en los que he participado incluyendo el rol que he desempeñado, por ejemplo, investigador principal. 
	*/
	@Test
	public void Q28() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#investigador-1", "http://purl.org/roh/data#a-project", "http://purl.org/roh/mirror/vivo#MemberRole"});
		
		
		String queryString = "PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX foaf: <http://purl.org/roh/mirror/foaf#>\n" + 
				"PREFIX rdfs:  <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"SELECT ?researcher ?project ?roleClass \n" +
				"WHERE {\n" +
				"?project a vivo:Project ;\n" + 
				"vivo:relates ?role .\n" + 
				"?role a ?roleClass ;\n" +
				"roh:roleOf ?researcher .\n" + 
				"?researcher a foaf:Person .\n" +
				"FILTER NOT EXISTS {\n" +
				"?role a ?otherClass .\n" + 
				"?otherClass rdfs:subClassOf ?roleClass .\n" +
				"FILTER (?otherClass != ?roleClass)\n" +
				"}\n"+
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
//				  ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource researcher = soln.getResource("researcher");
			      Resource project = soln.getResource("project");
			      Resource roleClass = soln.getResource("roleClass");
			      
			      result.add(new String[]{researcher.getURI(), project.getURI(), roleClass.getURI()});
			    }
		}
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
	}
	
	/**
	* Obtener el listado de mi producción científica
	*/
	@Test
	public void Q29() {
		List<String> expectedResult = new ArrayList<String>();
		expectedResult.add("http://purl.org/roh/data#my-fabulous-patent");
		expectedResult.add("http://purl.org/roh/data#another-fabulous-patent");
		expectedResult.add("http://purl.org/roh/data#journal-article-1");
		
		String queryString = "PREFIX bibo:  <http://purl.org/roh/mirror/bibo#>\n" +
				"PREFIX rdfs:  <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"SELECT DISTINCT ?item \n" +
				"WHERE {\n" +
				"?item a roh:ResearchObject ;\n" + 
				"bibo:authorList ?authorList .\n" +
				"?authorList rdfs:member <http://purl.org/roh/data#investigador-1> . \n" +
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String> result = new ArrayList<String>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource item = soln.getResource("item");
			      
			      
			      result.add(item.getURI());
			    }
		}
		
		assertThat(expectedResult, containsInAnyOrder(result.toArray()));
	}
	
	@Ignore
	@Test
	public void Q30() {

	}
	
	/**
	* Obtener los indicadores de mi producción científica como, por ejemplo, total de citas, h-index, etc.
	*/
	@Test
	public void Q31() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#investigador-2", "5", "53"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#investigador-3", "1", "5"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#investigador-1", "3", "21"});
		
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX foaf: <http://purl.org/roh/mirror/foaf#>\n" + 
				"SELECT ?researcher ?factorH ?cites \n" +
				"WHERE {\n" +
				"?researcher a foaf:Person ;\n" + 
				"roh:hasCV ?cv .\n" + 
				"?cv roh:cites ?cites ;\n" + 
				"roh:factorH ?factorH .\n" + 
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource researcher = soln.getResource("researcher");
			      Literal factorH = soln.getLiteral("factorH");
			      Literal cites = soln.getLiteral("cites");
			      
			      result.add(new String[]{researcher.getURI(), factorH.getString(), cites.getString()});
			    }
		}
		
		assertEquals(3, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
		assertThat(result, hasItem(expectedResult.get(1)));
		assertThat(result, hasItem(expectedResult.get(2)));
	}
	
	/**
	* Listar proyectos en los que una persona ha participado en un periodo de tiempo. 
	*/
	@Test
	public void Q32() {
		Q16A();
	}
	
	/**
	* Dado un periodo de 6 años devuelveme el número de academic articles con factor de impacto y determina si es mayor 5, pudiendo filtrar por cuartil, considerando de Q3 para arriba.
	*/
	@Test
	public void Q33() {		
		String queryString = "PREFIX iao: <http://purl.org/roh/mirror/obo/iao#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX bibo:  <http://purl.org/roh/mirror/bibo#>\n" +
				"PREFIX rdfs:  <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>\n" +
				"PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"SELECT ?researcher (COUNT(?journal) as ?count)  " +
				"WHERE {\n" +
				"?journal a iao:IAO_0000013 ;\n" +
				"vivo:dateIssued ?dateTimeValue ;\n" +
				"bibo:authorList ?authorList ;\n" +
				"roh:hasMetric ?metric .\n" +
				"?dateTimeValue vivo:dateTime ?date .\n" +
				"?authorList rdfs:member ?researcher . \n" +
				"?metric roh:impactFactor ?impactFactor ;\n" +
				"roh:quartile ?quartile .\n" +
				"FILTER (str(?quartile) = \"Q1\"^^xsd:string || str(?quartile) = \"Q2\"^^xsd:string) \n" +
				"FILTER (YEAR(?date) >= \"2015\"^^xsd:integer && YEAR(?date) <= \"2020\"^^xsd:integer )\n" +
				"} GROUP BY ?researcher HAVING (?count > 5) \n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource researcher = soln.getResource("researcher");
			      Literal count = soln.getLiteral("count");
			      
			      result.add(new String[]{researcher.getURI(), count.getString()});
			    }
		}
		
		assertEquals(0, result.size());
	}
	
	/**
	* Proyecto en estado PROPOSAL_SUBMITTED dirigidas a una empresa e incluso detalles económicos de la misma, el Funding propuesto y los Funding Amounts associados.
	*/
	@Test
	public void Q34() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#a-project-in-negotiation", "http://purl.org/roh/data#company-one", "5000"});
		
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" +
				"PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX ro: <http://purl.org/roh/mirror/obo/ro#>\n" + 
				"SELECT ?project ?company ?fundingAmounts \n" + 
				"WHERE {\n" +
				"?project a vivo:Project ;\n" +
				"roh:projectStatus \"PROPOSAL_SUBMITTED\" ;\n" +
				"roh:isSupportedBy ?funding .\n" +
				"?funding ro:hasPart ?fundingAmount ;\n" +
				"roh:fundedBy ?fundingProgram .\n" +
				"?fundingProgram roh:promotedBy ?company .\n" +
				"?fundingAmount roh:monetaryAmount ?fundingAmounts .\n" +
				"?company a vivo:Company .\n" +
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource project = soln.getResource("project");
			      Resource company = soln.getResource("company");
			      Literal fundingAmounts = soln.getLiteral("fundingAmounts");
			      
			      result.add(new String[]{project.getURI(), company.getURI(), fundingAmounts.getString()});
			    }
		}
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
	}
	
	/**
	* Listar los documentos de justificación asociados a un proyecto.
	*/
	@Test
	public void Q35A() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#a-project", "http://purl.org/roh/data#a-project-research-proposal"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#a-project", "http://purl.org/roh/data#a-project-justification"});
		
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX bibo:  <http://purl.org/roh/mirror/bibo#>\n" +
				"PREFIX rdfs:  <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"SELECT DISTINCT ?project ?report \n" + 
				"WHERE {\n" +
				"?project a vivo:Project .\n" +
				"?dossier vivo:relates ?project .\n" +
				"{\n" +
				"SELECT ?report \n" +
				"WHERE{\n" +
				"?dossier vivo:relates ?report .\n" +
				"?report a ?reportClass .\n" +
				"?reportClass rdfs:subClassOf* bibo:Report .\n" +
				"}\n" +
				"}\n" +
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
//				  ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource project = soln.getResource("project");
			      Resource report = soln.getResource("report");
			      
			      result.add(new String[]{project.getURI(), report.getURI()});
			    }
		}
		
		assertEquals(2, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
		assertThat(result, hasItem(expectedResult.get(1)));
	}
	
	/**
	* Listar las fechas de justificación de un proyecto
	*/
	@Test
	public void Q35B() {
		List<String> expectedResult = new ArrayList<String>();
		expectedResult.add("2019-03-31T00:00:00");
		expectedResult.add("2020-03-31T00:00:00");
		
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"SELECT ?foreseenJustificationDate \n" +
				"WHERE {\n" +
				"?project a vivo:Project ;\n" +
				"roh:foreseenJustificationDate ?foreseenJustificationDate .\n" +
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String> result = new ArrayList<String>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Literal foreseenJustificationDate = soln.getLiteral("foreseenJustificationDate");
			      
			      result.add(foreseenJustificationDate.getString());
			    }
		}
		assertThat(expectedResult, containsInAnyOrder(result.toArray()));
	}
	
	/**
	* Listar los gastos de un proyecto.
	*/
	@Test
	public void Q35C() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#an-intern-expense", "The salary of an intern working for the project"});
		
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"SELECT ?expense ?description \n" +
				"WHERE {\n" +
				"?project a vivo:Project ;\n" +
				"roh:spends ?expense .\n" +
				"?expense vivo:description ?description .\n" +
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource expense = soln.getResource("expense");
			      Literal description = soln.getLiteral("description");
			      
			      result.add(new String[]{expense.getURI(), description.getString()});
			    }
		}
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
	}
	
	/**
	* Listar los grupos ordenados por financiación recibida.
	*/ 
	@Test
	public void Q36() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/data#european-funding-program", "25000"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/data#hazitek2020", "10000"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-3", "http://purl.org/roh/data#european-funding-program", "35000"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", null, "5000"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", null, "50000"});

		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX ro: <http://purl.org/roh/mirror/obo/ro#>\n" + 
				"SELECT ?organization ?fundingProgram (SUM(?monetaryAmount) as ?totalFunding) \n" +
				"WHERE {\n" +
				"?fundingProgram a roh:FundingProgram ;\n" +
				"roh:funds ?funding .\n" +
				"?funding ro:hasPart ?fundingAmount .\n" +
				"?fundingAmount roh:grants ?organization ;\n" +
				"roh:monetaryAmount ?monetaryAmount .\n" +
				"} GROUP BY ?organization ?fundingProgram \n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource organization = soln.getResource("organization");
			      Resource fundingProgram = soln.getResource("fundingProgram");
			      Literal totalFunding = soln.getLiteral("totalFunding");
			      
			      result.add(new String[]{organization.getURI(), fundingProgram.getURI(), totalFunding.getString()});
			    }
		}
				
		assertEquals(5, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
		assertThat(result, hasItem(expectedResult.get(1)));
		assertThat(result, hasItem(expectedResult.get(2)));
		assertThat(result, hasItem(expectedResult.get(3)));
		assertThat(result, hasItem(expectedResult.get(4)));

	}
	
	/**
	* Listar número de proyectos y/o publicaciones compartidas entre 2 o más organizaciones.
	*/
	@Test
	public void Q37() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#a-project", "http://purl.org/roh/data#company-one", "http://purl.org/roh/data#centro-investigacion-1"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#a-project", "http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/data#company-one"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#another-collaborative-project", "http://purl.org/roh/data#centro-investigacion-3", "http://purl.org/roh/data#centro-investigacion-1"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#another-collaborative-project", "http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/data#centro-investigacion-3"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#a-project-in-negotiation", "http://purl.org/roh/data#company-one", "http://purl.org/roh/data#centro-investigacion-1"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#a-project-in-negotiation", "http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/data#company-one"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#a-private-project", "http://purl.org/roh/data#company-one", "http://purl.org/roh/data#centro-investigacion-1"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#a-private-project", "http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/data#company-one"});

		
		
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX foaf: <http://purl.org/roh/mirror/foaf#>\n" + 
				"SELECT DISTINCT ?project ?organization ?otherOrganization \n" +
				"WHERE {\n" +
				"?project a vivo:Project ;\n" +
				"vivo:relates ?role ;\n" +
				"vivo:relates ?otherRole .\n" +
				"?role roh:roleOf ?organization .\n" +
				"?organization a foaf:Organization .\n" +
				"?otherRole roh:roleOf ?otherOrganization .\n" +
				"?otherOrganization a foaf:Organization .\n" +
				"FILTER (?organization != ?otherOrganization) \n" + 
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource project = soln.getResource("project");
			      Resource organization = soln.getResource("organization");
			      Resource otherOrganization = soln.getResource("otherOrganization");
			      
			      result.add(new String[]{project.getURI(), organization.getURI(), otherOrganization.getURI()});
			    }
		}
		
		assertEquals(8, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
		assertThat(result, hasItem(expectedResult.get(1)));
		assertThat(result, hasItem(expectedResult.get(2)));
		assertThat(result, hasItem(expectedResult.get(3)));
		assertThat(result, hasItem(expectedResult.get(4)));
		assertThat(result, hasItem(expectedResult.get(5)));
		assertThat(result, hasItem(expectedResult.get(6)));
		assertThat(result, hasItem(expectedResult.get(7)));
	}
	
	@Ignore
	@Test
	public void Q38() {
		fail("Test not implemented");
	}
	
	/**
	* Financiación atraída en unos años por todos los investigadores del área de conocimiento X.
	*/
	@Test
	public void Q39() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/unesco-individuals#1203", "10000"});
		
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" +
				"PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX ro: <http://purl.org/roh/mirror/obo/ro#>\n" + 
				"PREFIX foaf: <http://purl.org/roh/mirror/foaf#>\n" + 
				"SELECT ?organization ?knowledgeArea (SUM(?monetaryAmount) as ?totalFunding) \n" + 
				"WHERE {\n" +
				"?project a vivo:Project ;\n" +
				"roh:hasKnowledgeArea ?knowledgeArea ;\n" +
				"roh:isSupportedBy ?funding .\n" +
				"?funding ro:hasPart ?fundingAmount .\n" +
				"?fundingAmount roh:monetaryAmount ?monetaryAmount ;\n" +
				"roh:grants ?organization .\n" +
				"?organization a foaf:Organization .\n" +
				"} GROUP BY ?organization ?knowledgeArea\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource organization = soln.getResource("organization");
			      Resource knowledgeArea = soln.getResource("knowledgeArea");
			      Literal totalFunding = soln.getLiteral("totalFunding");
			      
			      result.add(new String[]{organization.getURI(), knowledgeArea.getURI(), totalFunding.getString()});
			    }
		}
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
	}
	
	@Ignore
	@Test
	public void Q40() {
		fail("Test not implemented");
	}
	
	@Ignore
	@Test
	public void Q41() {
		fail("Test not implemented");
	}
	
	/**
	* Investigadores que tienen ERCs, Marie Curie, etc.
	*/
	@Test
	public void Q42() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#investigador-1", "http://purl.org/roh/data#investigador-1-juan-de-la-cierva"});
		
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX foaf: <http://purl.org/roh/mirror/foaf#>\n" + 
				"SELECT ?researcher ?grant \n" +
				"WHERE {\n" +
				"?contract a roh:PersonContract ; \n" +
				"vivo:relates ?researcher .\n" + 
				"?researcher a foaf:Person .\n" +
				"{\n" +
				"SELECT ?grant \n" +
				"WHERE {\n" +
				"?grant a roh:Grant .\n" +
				"?contract vivo:relates ?grant .\n" +
				"}\n" +
				"}\n" +
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource researcher = soln.getResource("researcher");
			      Resource grant = soln.getResource("grant");
			      
			      result.add(new String[]{researcher.getURI(), grant.getURI()});
			    }
		}
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
	}
	
	/**
	* Identificar qué universidades de la red cuentan con la distinción de excelencia Severo Ochoa, María de Maeztu o las equivalentes.
	*/
	@Test
	public void Q43() {
		Q4();
	}
	
	/**
	* Cuantificar los proyectos en convocatorias competitivas de un grupo de investigación en un rango de años con grupos de investigación de otras Universidades.
	*/
	@Test
	public void Q44() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{
			"http://purl.org/roh/data#another-collaborative-project",
			"http://purl.org/roh/data#european-funding-program",
			"http://purl.org/roh/data#centro-investigacion-3",
			"http://purl.org/roh/data#universidad-2"
		});
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX ro: <http://purl.org/roh/mirror/obo/ro#>\n" + 
				"PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>\n" +
				"SELECT DISTINCT ?project ?fundingProgram ?otherOrganization ?university \n" +
				"WHERE{\n" +
				"?project a vivo:Project ;\n" +
				"vivo:dateTimeInterval ?dateTimeInterval ;\n" +
				"vivo:relates ?role ;\n" +
				"vivo:relates ?otherRole ;\n" +
				"roh:isSupportedBy ?funding .\n" +
				"?funding roh:fundedBy ?fundingProgram .\n" +
				"?role roh:roleOf <http://purl.org/roh/data#centro-investigacion-1> .\n" +
				"?otherRole roh:roleOf ?otherOrganization .\n" +
				"?university ro:BFO_0000051 ?otherOrganization .\n" +
				"?university a vivo:University .\n" +
				"?dateTimeInterval vivo:start ?start ;\n" +
				"vivo:end ?end .\n" +
				"?start vivo:dateTime ?startTime .\n" +
				"?end vivo:dateTime ?endTime .\n" +
				"FILTER (?otherOrganization != <http://purl.org/roh/data#centro-investigacion-1>)" +
				"FILTER (YEAR(?startTime) <= \"2019\"^^xsd:integer && YEAR(?endTime) >= \"2019\"^^xsd:integer)" +
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
//				  ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource project = soln.getResource("project");
			      Resource fundingProgram = soln.getResource("fundingProgram");
			      Resource otherOrganization = soln.getResource("otherOrganization");
			      Resource university = soln.getResource("university");
			      
			      result.add(new String[]{project.getURI(), fundingProgram.getURI(), otherOrganization.getURI(), university.getURI()});
			    }
		}
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
	}
	
	/**
	* Obtener el total de publicaciones de impacto por mujeres y hombres.
	*/
	@Test
	public void Q45() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"female", "1"});
		expectedResult.add(new String[]{"male", "1"});

		
		String queryString = "PREFIX iao: <http://purl.org/roh/mirror/obo/iao#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX bibo:  <http://purl.org/roh/mirror/bibo#>\n" +
				"PREFIX rdfs:  <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>\n" +
				"PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX foaf: <http://purl.org/roh/mirror/foaf#>\n" + 
				"SELECT ?gender (COUNT(?journal) as ?count)  " +
				"WHERE {\n" +
				"?journal a iao:IAO_0000013 ;\n" +
				"vivo:dateIssued ?dateTimeValue ;\n" +
				"bibo:authorList ?authorList ;\n" +
				"roh:hasMetric ?metric .\n" +
				"?dateTimeValue vivo:dateTime ?date .\n" +
				"?authorList rdfs:member ?researcher . \n" +
				"?researcher foaf:gender ?gender .\n" +
				"?metric roh:impactFactor ?impactFactor .\n" +
				"} GROUP BY ?gender \n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Literal gender = soln.getLiteral("gender");
			      Literal count = soln.getLiteral("count");
			      
			      result.add(new String[]{gender.getString(), count.getString()});
			    }
		}
		
		assertEquals(2, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
		assertThat(result, hasItem(expectedResult.get(1)));

	}
	
	@Ignore
	@Test
	public void Q46() {
	}
	
	@Ignore
	@Test
	public void Q47() {
		
	}

	/**
	* Obtener los indicadores de la producción científica como, por ejemplo, total de citas, h-index, JCR, etc de un grupo de investigación o instituto de investigación.
	*/
	@Test
	public void Q48() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", "79"});
		
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX foaf: <http://purl.org/roh/mirror/foaf#>\n" + 
				"PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"SELECT ?researchGroup (SUM(?cites) as ?totalCites) \n" +
				"WHERE {\n" +
				"?researcher a foaf:Person ;\n" + 
				"roh:hasPosition ?position ;\n" +
				"roh:hasCV ?cv .\n" + 
				"?cv roh:cites ?cites ;\n" + 
				"roh:factorH ?factorH .\n" + 
				"?position vivo:relates ?researchGroup .\n" +
				"?researchGroup a roh:ResearchGroup .\n" +
				"} GROUP BY ?researchGroup \n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
//				  ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource researchGroup = soln.getResource("researchGroup");
			      Literal totalCites = soln.getLiteral("totalCites");
			      
			      result.add(new String[]{researchGroup.getURI(), totalCites.getString()});
			    }
		}
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
	}
	
	/**
	* Obtener las publicaciones de un investigador o grupo de investigación en una revista científica indicada.
	*/
	@Test
	public void Q49() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/data#journal-article-1"});
		
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX bibo:  <http://purl.org/roh/mirror/bibo#>\n" +
				"PREFIX rdfs:  <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"SELECT DISTINCT ?researchGroup ?journalArticle \n" +
				"WHERE {\n" +
				"?researcher roh:hasPosition ?position .\n" +
				"?position vivo:relates ?researchGroup .\n" +
				"?researchGroup a roh:ResearchGroup .\n" +
				"?journalArticle bibo:authorList ?authorList ;\n" +
				"vivo:hasPublicationVenue <http://purl.org/roh/data#excelent-journal> .\n" +
				"?authorList rdfs:member ?researcher .\n" +
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
//				  ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource researchGroup = soln.getResource("researchGroup");
			      Resource journalArticle = soln.getResource("journalArticle");
			      
			      result.add(new String[]{researchGroup.getURI(), journalArticle.getURI()});
			    }
		}
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
	}
	
	/**
	* Dados unos años listar los JRCs en los mismos para poder visualizar evolución.
	*/
	@Test
	public void Q50() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#journal-article-1", "2.5", "2020"});
		
		String queryString = "PREFIX iao: <http://purl.org/roh/mirror/obo/iao#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX bibo:  <http://purl.org/roh/mirror/bibo#>\n" +
				"PREFIX rdfs:  <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>\n" +
				"PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"SELECT ?journal ?impactFactor (YEAR(?date) as ?year)  " +
				"WHERE {\n" +
				"?journal a iao:IAO_0000013 ;\n" +
				"vivo:dateIssued ?dateTimeValue ;\n" +
				"bibo:authorList ?authorList ;\n" +
				"roh:hasMetric ?metric .\n" +
				"?dateTimeValue vivo:dateTime ?date .\n" +
				"?authorList rdfs:member <http://purl.org/roh/data#investigador-1> . \n" +
				"?metric roh:impactFactor ?impactFactor ;\n" +
				"FILTER (YEAR(?date) >= \"2020\"^^xsd:integer && YEAR(?date) <= \"2020\"^^xsd:integer )\n" +
				"} \n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
//				  ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource journal = soln.getResource("journal");
			      Literal impactFactor = soln.getLiteral("impactFactor");
			      Literal year = soln.getLiteral("year");
			      
			      result.add(new String[]{journal.getURI(), impactFactor.getString(), year.getString()});
			    }
		}
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
	}
	
	@Ignore
	@Test
	public void Q51() {
		
	}
	
	/**
	* Buscar las publicaciones de un grupo o instituto de investigación, o una universidad en las que aparezca en el título de la publicación los tokens que indique como entrada a la consulta.
	*/
	@Test
	public void Q52() {
		List<String> expectedResult = new ArrayList<String>();
		expectedResult.add("http://purl.org/roh/data#journal-article-1");
		
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX bibo:  <http://purl.org/roh/mirror/bibo#>\n" +
				"PREFIX rdfs:  <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"PREFIX dc:    <http://purl.org/dc/elements/1.1/>\n" +
				"SELECT DISTINCT ?publication \n" +
				"WHERE {\n" +
				"?publication bibo:authorList ?authorList  ;" +
				"dc:title ?title ;" +
				"a bibo:Article ." + 
				"?authorList rdfs:member ?author .\n" +
				"?author roh:hasPosition ?position .\n" +
				"?position vivo:relates <http://purl.org/roh/data#centro-investigacion-1> .\n" +
				"FILTER regex(?title, \"great\", \"i\")" +
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String> result = new ArrayList<String>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
//				  ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource publication = soln.getResource("publication");
			      
			      result.add(publication.getURI());
			    }
		}
		
		assertThat(expectedResult, equalTo(result));
	}
	
	/**
	* Listar la revistas científicas con un JCR Q2, en un área determinada, donde ha publicado un grupo o instituto de investigación.
	*/
	@Test
	public void Q53() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#excelent-journal", "http://purl.org/roh/unesco-individuals#1203"});
		
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX bibo:  <http://purl.org/roh/mirror/bibo#>\n" +
				"PREFIX rdfs:  <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"SELECT DISTINCT ?journal ?knowledgeArea \n" +
				"WHERE {\n" +
				"?journalArticle vivo:hasPublicationVenue ?journal ;\n" +
				"bibo:authorList ?authorList ;\n" +
				"roh:hasMetric ?metric ;\n" +
				"roh:hasKnowledgeArea ?knowledgeArea .\n" +
				"?authorList rdfs:member ?author .\n" +
				"?author roh:hasPosition ?position .\n" +
				"?position vivo:relates <http://purl.org/roh/data#centro-investigacion-1> .\n" +
				"?metric roh:quartile \"Q2\"" +
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
//				  ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource journal = soln.getResource("journal");
			      Resource knowledgeArea = soln.getResource("knowledgeArea");
			      
			      result.add(new String[] {journal.getURI(), knowledgeArea.getURI()});
			    }
		}
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
	}
	
	/**
	* Listar los impactos JCR de los investigadores de un grupo en un año.
	*/
	@Test
	public void Q54() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#investigador-1", "http://purl.org/roh/data#journal-article-1", "2.5"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#investigador-3", "http://purl.org/roh/data#journal-article-1", "2.5"});

		String queryString = "PREFIX iao: <http://purl.org/roh/mirror/obo/iao#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX bibo:  <http://purl.org/roh/mirror/bibo#>\n" +
				"PREFIX rdfs:  <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>\n" +
				"PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"SELECT ?researcher ?journal ?impactFactor  " +
				"WHERE {\n" +
				"?journal a iao:IAO_0000013 ;\n" +
				"vivo:dateIssued ?dateTimeValue ;\n" +
				"bibo:authorList ?authorList ;\n" +
				"roh:hasMetric ?metric .\n" +
				"?dateTimeValue vivo:dateTime ?date .\n" +
				"?authorList rdfs:member ?researcher . \n" +
				"?metric roh:impactFactor ?impactFactor ;\n" +
				"FILTER (YEAR(?date) >= \"2020\"^^xsd:integer && YEAR(?date) <= \"2020\"^^xsd:integer )\n" +
				"} \n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
//				  ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource researcher = soln.getResource("researcher");
			      Resource journal = soln.getResource("journal");
			      Literal impactFactor = soln.getLiteral("impactFactor");
			      
			      result.add(new String[] {researcher.getURI(), journal.getURI(), impactFactor.getString()});
			    }
		}
		

		assertEquals(2, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
		assertThat(result, hasItem(expectedResult.get(1)));
	}
	
	/**
	* Listar los proyectos privados obtenidos por un grupo de investigación contratados por una organización privada.
	*/
	@Test
	public void Q55() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/data#a-private-project", "http://purl.org/roh/data#company-one"});
		
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" +
				"PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX ro: <http://purl.org/roh/mirror/obo/ro#>\n" + 
				"SELECT ?researchGroup ?project ?company \n" + 
				"WHERE {\n" +
				"?project a vivo:Project ;\n" +
				"vivo:relates ?role ;\n" +
				"roh:isSupportedBy ?funding .\n" +
				"?funding ro:hasPart ?fundingAmount ;\n" +
				"roh:fundedBy ?fundingProgram .\n" +
				"?fundingProgram roh:promotedBy ?company .\n" +
				"?fundingAmount roh:monetaryAmount ?fundingAmounts .\n" +
				"?company a vivo:Company .\n" +
				"?role roh:roleOf ?researchGroup .\n" +
				"?researchGroup a roh:ResearchGroup .\n" +
				"OPTIONAL {\n" +
				"?project roh:projectStatus ?projectStatus .\n" +
				"}\n" +
				"FILTER (!BOUND(?projectStatus) || ?projectStatus != \"PROPOSAL_SUBMITTED\")\n" +
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource researchGroup = soln.getResource("researchGroup");
			      Resource project = soln.getResource("project");
			      Resource company = soln.getResource("company");
			      
			      result.add(new String[] {researchGroup.getURI(), project.getURI(), company.getURI()});
			    }
		}
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
	}
	
	/**
	* Listar las tesis de una universidad en las que aparezca en el título los tokens que indique como entrada a la consulta.
	*/
	@Test 
	public void Q56() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#universidad-1", "http://purl.org/roh/data#investigador-3-phd-thesis"});
		
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX bibo:  <http://purl.org/roh/mirror/bibo#>\n" +
				"PREFIX rdfs:  <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"PREFIX dc:    <http://purl.org/dc/elements/1.1/>\n" +
				"PREFIX ro: <http://purl.org/roh/mirror/obo/ro#>\n" + 
				"SELECT DISTINCT ?university ?phdThesis \n" +
				"WHERE {\n" +
				"?phdThesis a roh:PhDThesis \n;" +
				"bibo:authorList ?authorList ;\n" +
				"dc:title ?title .\n" +
				"?authorList rdfs:member ?author .\n" +
				"?author roh:hasPosition ?position .\n" +
				"?position vivo:relates ?researchGroup .\n" +
				"?researchGroup a roh:ResearchGroup .\n" +
				"?university ro:BFO_0000051 ?researchGroup .\n" +
				"FILTER regex(?title, \"fabulous\", \"i\")" +
				"}\n";
		writeSPARQLQuery(Thread.currentThread().getStackTrace()[1].getMethodName(), queryString);
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
//				  ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource university = soln.getResource("university");
			      Resource phdThesis = soln.getResource("phdThesis");
			      
			      result.add(new String[] {university.getURI(), phdThesis.getURI()});
			    }
		}
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
	}
 	
	/**
	* Indicar el porcentaje de mujeres frente a hombres como autores de publicaciones con impacto de un grupo de investigación.
	*/ 
	@Test
	public void Q57() {
		Q45();
	}
	
}

