import static org.hamcrest.MatcherAssert.assertThat; 
import static org.hamcrest.Matchers.*;
import static org.junit.Assert.assertEquals;
import static org.junit.Assert.fail;

import java.util.ArrayList;
import java.util.List;

import org.apache.jena.query.Query;
import org.apache.jena.query.QueryExecution;
import org.apache.jena.query.QueryExecutionFactory;
import org.apache.jena.query.QueryFactory;
import org.apache.jena.query.QuerySolution;
import org.apache.jena.query.ResultSet;
import org.apache.jena.query.ResultSetFormatter;
import org.apache.jena.rdf.model.Literal;
import org.apache.jena.rdf.model.Model;
import org.apache.jena.rdf.model.Resource;
import org.junit.Test;

import eu.deustotech.hercules.validation.DataGenerator;

public class ValidationQuestionsTest {

	Model data = DataGenerator.getModel(System.getProperty("ontFile"), System.getProperty("dataFile"));
	
	@Test
	public void Q1() {
		List<String> expectedResult = new ArrayList<String>();
		expectedResult.add("http://purl.org/roh/data#centro-investigacion-2");
		expectedResult.add("http://purl.org/roh/data#centro-investigacion-1");
		
		String queryString = "PREFIX vivo: <http://vivoweb.org/ontology/core#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX obo: <http://purl.obolibrary.org/obo/>\n" + 
				"PREFIX bibo: <http://purl.org/ontology/bibo/>\n" + 
				"PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>\n" + 
				"PREFIX foaf: <http://xmlns.com/foaf/0.1/>\n" + 
				"PREFIX unesco: <http://purl.org/roh/unesco-individuals#>\n" + 
				"PREFIX ro: <http://purl.org/obo/owl/ro#>\n" + 
				"\n" + 
				"SELECT ?centro WHERE {\n" + 
				"        ?centro a roh:ResearchGroup ;\n" + 
				"                          roh:hasKnowledgeArea unesco:C00261 .\n" + 
				"}";
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
				"  ?researcher	roh:hasKnowledgeArea unesco:C00261 ;\n" + 
				"  				roh:hasPosition ?position .\n" + 
				"  ?position vivo:relates ?center ;\n" + 
				"  			a	?positionClass .\n" + 
				"  ?center a roh:ResearchGroup .\n" + 
				"}";
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
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
	
	@Test
	public void Q3() {
		fail("Test not implemented");
	}
	
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
	
	@Test
	public void Q5() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/data#hazitek2020"});
		
		String queryString = "PREFIX vivo: <http://vivoweb.org/ontology/core#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX obo: <http://purl.obolibrary.org/obo/>\n" + 
				"PREFIX bibo: <http://purl.org/ontology/bibo/>\n" + 
				"PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>\n" + 
				"PREFIX foaf: <http://purl.org/roh/mirror/foaf#>\n" + 
				"PREFIX unesco: <http://purl.org/roh/unesco-individuals#>\n" + 
				"PREFIX ro: <http://purl.org/obo/owl/ro#>\n" + 
				"PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>\n" + 
				"\n" + 
				"SELECT ?centro ?fundingProgram\n" + 
				"WHERE {\n" + 
				"  ?fundingAmount roh:grants ?centro .\n" + 
				"  ?funding obo:hasPart ?fundingAmount .\n" + 
				"  ?funding roh:fundedBy ?fundingProgram .\n" + 
				"}";
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource centro = soln.getResource("centro");
			      Resource fundingProgram = soln.getResource("fundingProgram");
			      
			      result.add(new String[]{centro.getURI(), fundingProgram.getURI()});
			    }
		}
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
	}
	
	@Test
	public void Q6() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/data#journal-article-1", "http://purl.org/roh/unesco-individuals#C00750"});
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/data#my-fabulous-patent", "http://purl.org/roh/unesco-individuals#C00750"});

		// TODO: Fix ResearchObject inference
		String queryString =  "PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX bibo:  <http://purl.org/roh/mirror/bibo#>\n" +
				"PREFIX rdfs:  <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"PREFIX foaf: <http://purl.org/roh/mirror/foaf#>\n" + 
				"SELECT ?organization ?researchObject ?knowledgeArea \n" +
				"WHERE {\n" +
				"?researchObject a ?document ;\n" +
				"roh:hasKnowledgeArea ?knowledgeArea ;\n" +
				"bibo:authorList ?authorList .\n" +
				"?document rdfs:subClassOf* bibo:Document .\n" +
				"?authorList ?order ?author .\n" +
				"?author roh:hasPosition ?position .\n" + 
				"?organization a ?organizationClass . \n" + 
				"?organizationClass rdfs:subClassOf* foaf:Organization .\n" +
				"?position vivo:relates ?organization .\n" +
				"} GROUP BY ?organization ?researchObject ?knowledgeArea\n";
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
		assertEquals(2, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
		assertThat(result, hasItem(expectedResult.get(1)));
	}
	
	@Test
	public void Q7() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"1", "https://sws.geonames.org/3128026/"});
		
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
				"          ?authorList        ?order        ?author .\n" + 
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
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
//				 ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Literal publicationCount = soln.getLiteral("publicationCount");
			      Resource location = soln.getResource("location");
			      
			      result.add(new String[]{publicationCount.getString(), location.getURI()});
			    }
		}
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
	}
	
	@Test
	public void Q8() {
		fail("Test not implemented");
	}
	
	@Test 
	public void Q9 () {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#my-fabulous-patent", "http://purl.org/roh/data#centro-investigacion-1"});
		
		// TODO: missing dateTime restrictions and Organization inference
		String queryString = "PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX bibo: <http://purl.org/roh/mirror/bibo#>\n" + 
				"SELECT ?patent ?centre \n" +
				"WHERE {\n" +
				"?patent a bibo:Patent ;\n" +
				"bibo:authorList ?authorList .\n" +
				"?authorlist ?order ?author .\n" +
				"?author roh:hasPosition ?position .\n" + 
				"?position vivo:relates ?centre .\n" +
				"?centre a roh:ResearchGroup .\n" +
				"} GROUP BY ?patent ?centre";
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
			      
			      result.add(new String[]{patent.getURI(), centre.getURI()});
			    }
		}
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
	}
	
	@Test
	public void Q10() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#a-project", "http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/unesco-individuals#C00750"});
		
		// TODO: missing Role inference
		String queryString = "PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>\n" +
				"PREFIX rdfs:  <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"PREFIX foaf: <http://purl.org/roh/mirror/foaf#>\n" + 
				"SELECT ?project ?centre ?knowledgeArea \n" +
				"WHERE {\n" +
				"?project a roh:Project ;\n" +
				"roh:hasKnowledgeArea ?knowledgeArea ;\n" +
				"vivo:relates ?role ;\n" +
				"vivo:dateTimeInterval ?dateTimeInterval .\n" +
				"?role a vivo:MemberRole ;\n" + 
				"roh:roleOf ?centre .\n" +
				"?centre a ?organization .\n" +
				"?organization rdfs:subClassOf* foaf:Organization .\n" +
				"?dateTimeInterval vivo:start ?startDateTimeValue ;\n" +
				"vivo:end ?endDateTimeValue . \n" +
				"?startDateTimeValue vivo:dateTime ?start . \n" +
				"?endDateTimeValue vivo:dateTime ?end . \n" +
				"FILTER (YEAR(?start) <= \"2019\"^^xsd:integer && YEAR(?end) >= \"2019\"^^xsd:integer )\n" +
				"}\n";
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
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
		
	}
	
	@Test
	public void Q11() {
		fail("Test not implemented");
	}
	
	@Test
	public void Q12() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#a-project", "http://purl.org/roh/data#centro-investigacion-1", "https://www.geonames.org/3336903"});
		
		String queryString = "PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" +
				"PREFIX gn:	<http://purl.org/roh/mirror/geonames#>\n" +
				"SELECT ?project ?centre ?location \n" +
				"WHERE {\n" +
				"?project a roh:Project ; \n" +
				"gn:locatedIn ?location ;\n" + 
				"vivo:relates ?role .\n" +
				"?role a vivo:MemberRole ;\n" + 
				"roh:roleOf ?centre .\n" +
				"?centre a roh:ResearchGroup .\n" +
				"}\n";
		
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
//				  ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource project = soln.getResource("project");
			      Resource centre = soln.getResource("centre");
			      Resource location = soln.getResource("location");
			      
			      result.add(new String[]{project.getURI(), centre.getURI(), location.getURI()});
			    }
		}
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));

	}
	
	@Test
	public void Q13() {
		fail("Test not implemented");
	}
	
	@Test
	public void Q14() {
		fail("Test not implemented");
	}
	
	@Test
	public void Q15() {
		List<String> expectedResult = new ArrayList<String>();
		expectedResult.add("http://purl.org/roh/data#a-project");
		
		//TODO: missing uneskos inference
		String queryString = "PREFIX roh: <http://purl.org/roh#>\n" +
				"PREFIX uneskos: <http://purl.org/roh/unesco-individuals#>\n" +
				"SELECT ?project \n" +
				"WHERE {\n" +
				"?project a roh:Project ;\n" +
				"roh:hasKnowledgeArea uneskos:C00750 .\n" +
				"}\n";
		
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
		
		assertThat(result, equalTo(expectedResult));
	}
	
	@Test
	public void Q16() {
		fail("Test not implemented");
	}
	
	@Test
	public void Q17() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#centro-investigacion-1", "http://purl.org/roh/data#a-project"});
		
		//TODO: missing organization inference and research objects
		String queryString = "PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>\n" +
				"SELECT ?organization ?project \n" +
				"WHERE {\n" +
				"?organization a roh:ResearchGroup .\n" + 
				"?role roh:roleOf ?organization ;\n" + 
				"vivo:relatedBy ?project .\n" + 
				"?project a roh:Project ;\n" +
				"vivo:dateTimeInterval ?dateTimeInterval .\n" +
				"?dateTimeInterval vivo:start ?startDateTimeValue ;\n" +
				"vivo:end ?endDateTimeValue . \n" +
				"?startDateTimeValue vivo:dateTime ?start . \n" +
				"?endDateTimeValue vivo:dateTime ?end . \n" +
				"FILTER (YEAR(?start) <= \"2019\"^^xsd:integer && YEAR(?end) >= \"2019\"^^xsd:integer )\n" +
				"}\n";
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
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
	}
	
	@Test
	public void Q18() {
		fail("Test not implemented");
	}
	
	@Test
	public void Q19() {
		fail("Test not implemented");
	}
	
	@Test
	public void Q20() {
		fail("Test not implemented");
	}
	
	@Test
	public void Q21() {
		fail("Test not implemented");
	}
	
	@Test
	public void Q22() {
		fail("Test not implemented");
	}
	
	@Test
	public void Q23() {
		fail("Test not implemented");
	}
	
	@Test
	public void Q24() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#a-project", "http://purl.org/roh/data#centro-investigacion-1"});
		
		
		// TODO: missing organization inference
		String queryString = "PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"SELECT ?project ?centre \n" +
				"WHERE {\n" +
				"?project a roh:Project ;\n" +
				"vivo:relates ?role .\n" + 
				"?role roh:roleOf <http://purl.org/roh/data#company-one> .\n" +
				"{\n" +
				"SELECT ?centre \n" +
				"WHERE {\n" +
				"?project vivo:relates ?centreRole .\n" +
				"?centreRole roh:roleOf ?centre .\n" + 
				"?centre a roh:ResearchGroup .\n" +
				"}\n" +
				"}\n" +
				"}\n";
		Query query = QueryFactory.create(queryString) ;
		List<String[]> result = new ArrayList<String[]>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
//				  ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource project = soln.getResource("project");
			      Resource centre = soln.getResource("centre");
			      
			      result.add(new String[]{project.getURI(), centre.getURI()});
			    }
		}
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
	}
	
	@Test
	public void Q25() {
		List<String> expectedResult = new ArrayList<String>();
		expectedResult.add("http://purl.org/roh/data#investigador-3");
		
		String queryString = "PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX obo: <http://purl.obolibrary.org/obo/>\n" + 
				"\n" + 
				"SELECT ?supervisee \n" + 
				"WHERE {\n" + 
				"  ?supervisorRole a roh:SupervisorRole ;\n" + 
				"  obo:RO_0000052 <http://purl.org/roh/data#investigador-1> ;\n" + 
				"        vivo:relatedBy ?supervisingRelationship .\n" + 
				"  ?supervisingRelationship a roh:PhDSupervisingRelationship ;\n" + 
				"                            vivo:relates ?superviseeRole .\n" + 
				"  ?superviseeRole a roh:SuperviseeRole ;\n" + 
				"    obo:RO_0000052 ?supervisee\n" + 
				"}";
		Query query = QueryFactory.create(queryString) ;
		List<String> result = new ArrayList<String>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  // ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource supervisee = soln.getResource("supervisee");
			      
			      result.add(supervisee.getURI());
			    }
		}
		assertThat(expectedResult, equalTo(result));
	}
	
	@Test
	public void Q26() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#investigador-2", "http://purl.org/roh/data#a-great-conference", "http://purl.org/roh/mirror/vivo#AttendeeRole"});
		
		// TODO: missing activity inference
		String queryString = "PREFIX obo: <http://purl.obolibrary.org/obo/>\n" +
				"PREFIX bibo:  <http://purl.org/roh/mirror/bibo#>\n" +
				"SELECT ?researcher ?conference ?roleClass \n" +
				"WHERE {\n" +
				"?conference a bibo:Conference ;\n" +
				"obo:BFO_0000055 ?role .\n" + 
				"?role a ?roleClass ;\n" +
				"obo:RO_0000052 ?researcher .\n" +
				"}\n";
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
	
	@Test
	public void Q27() {
		fail("Test not implemented");
	}
	
	@Test
	public void Q28() {
		List<String[]> expectedResult = new ArrayList<String[]>();
		expectedResult.add(new String[]{"http://purl.org/roh/data#investigador-1", "http://purl.org/roh/data#a-project", "http://purl.org/roh/mirror/vivo#MemberRole"});
		
		
		String queryString = "PREFIX vivo: <http://purl.org/roh/mirror/vivo#>\n" + 
				"PREFIX roh: <http://purl.org/roh#>\n" + 
				"PREFIX foaf: <http://purl.org/roh/mirror/foaf#>\n" + 
				"SELECT ?researcher ?project ?roleClass \n" +
				"WHERE {\n" +
				"?project a roh:Project ;\n" + 
				"vivo:relates ?role .\n" + 
				"?role a ?roleClass ;\n" +
				"roh:roleOf ?researcher .\n" + 
				"?researcher a foaf:Person .\n" +
				"}\n";
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
			      Resource roleClass = soln.getResource("roleClass");
			      
			      result.add(new String[]{researcher.getURI(), project.getURI(), roleClass.getURI()});
			    }
		}
		
		assertEquals(1, result.size());
		assertThat(result, hasItem(expectedResult.get(0)));
	}
	
	@Test
	public void Q29() {
		List<String> expectedResult = new ArrayList<String>();
		expectedResult.add("http://purl.org/roh/data#my-fabulous-patent");
		expectedResult.add("http://purl.org/roh/data#journal-article-1");
		
		// TODO: improve query inference
		String queryString = "PREFIX bibo:  <http://purl.org/roh/mirror/bibo#>\n" +
				"PREFIX rdfs:  <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"SELECT ?item \n" +
				"WHERE {\n" +
				"?item a ?document ;\n" + 
				"bibo:authorList ?authorList .\n" +
				"?document rdfs:subClassOf* bibo:Document .\n" +
				"?authorList ?position <http://purl.org/roh/data#investigador-1> . \n" +
				"}\n";
		
		Query query = QueryFactory.create(queryString) ;
		List<String> result = new ArrayList<String>();
		try (QueryExecution qexec = QueryExecutionFactory.create(query, data)) {
			ResultSet results = qexec.execSelect();
			 for ( ; results.hasNext() ; )
			    {
				  //ResultSetFormatter.out(System.out, results, query);
			      QuerySolution soln = results.nextSolution() ;
			      
			      Resource item = soln.getResource("item");
			      
			      
			      result.add(item.getURI());
			    }
		}
		
		assertThat(expectedResult, equalTo(result));
	}
	
	@Test
	public void Q30() {
		fail("Test not implemented");
	}
	
	@Test
	public void Q31() {
		fail("Test not implemented");
	}
	
	@Test
	public void Q32() {
		fail("Test not implemented");
	}
}
