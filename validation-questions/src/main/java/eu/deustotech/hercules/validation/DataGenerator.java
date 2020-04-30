package eu.deustotech.hercules.validation;

import java.io.FileWriter;
import java.io.IOException;

import org.apache.jena.query.Dataset;
import org.apache.jena.rdf.model.InfModel;
import org.apache.jena.rdf.model.Model;
import org.apache.jena.rdf.model.ModelFactory;
import org.apache.jena.rdf.model.Property;
import org.apache.jena.rdf.model.Resource;
import org.apache.jena.rdf.model.Statement;
import org.apache.jena.rdf.model.StmtIterator;
import org.apache.jena.reasoner.Reasoner;
import org.apache.jena.reasoner.ReasonerRegistry;
import org.apache.jena.tdb2.TDB2Factory;
import org.apache.jena.util.FileManager;
import org.apache.jena.util.PrintUtil;


public class DataGenerator {
	
		static String tdbDirectory = "/home/mikel/PROYECTOS/Hercules/validation-questions/dataset";
		static String inftdbDirectory = "/home/mikel/PROYECTOS/Hercules/validation-questions/inferred";

		static String ontFile = "file:/home/mikel/PROYECTOS/Hercules/roh/roh-v2.owl";
		static String ontPrefix = "http://purl.org/roh#";
		static String dataFile = "file:/home/mikel/PROYECTOS/Hercules/roh/examples/data.ttl";
		static String baseURI = "http://purl.org/roh/data#";
		static String inferredOutput = "/home/mikel/PROYECTOS/Hercules/roh/roh-v2-inferred.owl";
		static String[] researcherNames = {"Pedro", "Maria", "Juan", "Sofia", "Ana"};
	
		public static void printStatements(Model m, Resource s, Property p, Resource o) { 
			for (StmtIterator i = m.listStatements(s,p,o); i.hasNext(); ) { 
				Statement stmt = i.nextStatement(); 
				System.out.println(" - " + PrintUtil.print(stmt)); 
			} 
		} 
		
		public static void generateModel() {
	
			Dataset dataset = TDB2Factory.connectDataset(tdbDirectory) ;
			dataset.begin();
			Model tdbModel = dataset.getDefaultModel();
			
			// Model schema = FileManager.get().loadModel(ontFile);
			Model data = FileManager.get().loadModel(dataFile);
			
			// tdbModel.add(schema);
			// tdbModel.commit();
			
			
			Reasoner reasoner = ReasonerRegistry.getOWLMiniReasoner();
			reasoner = reasoner.bindSchema(tdbModel);

			InfModel infmodel = ModelFactory.createInfModel(reasoner, data);
			
			Dataset infDataset = TDB2Factory.connectDataset(inftdbDirectory) ;
			infDataset.begin();
			
			Model inftdbModel = infDataset.getDefaultModel();
			inftdbModel.add(infmodel);
			infDataset.commit();
			infDataset.end();
			
			dataset.end();
			

//			// Create Researchers
//			Map<String, Resource> researchers = new HashMap<String, Resource>();
//			for (int i = 0; i < 5; i++) {
//				Resource researcher = model.createResource(baseURI.concat("researcher-".concat(String.valueOf(i))));
//				researcher.addProperty(RDF.type, FOAF.Person);
//				researcher.addProperty(FOAF.name, researcherNames[i]);
//				researchers.put(researcher.getURI(), researcher);
//			}
//			
//			// Create research groups
//			Map<String, Resource> researchGroups = new HashMap<String, Resource>();
//			for (int i = 0; i < 3; i++) {
//				Resource researchGroup = model.createResource(baseURI.concat("research-group-".concat(String.valueOf(i))));
//				researchGroup.addProperty(RDF.type, ontology.getOntClass(ontPrefix.concat("ResearchGroup")));
//				// Associate researchers to research groups
//				if (i == 0) {
//					researchers.get(baseURI.concat("researcher-".concat(String.valueOf(0))));
//				} else if (i == 1) {
//					
//				} else if (i == 2) {
//					
//				}
//				researchGroups.put(researchGroup.getURI(), researchGroup);
//
//			}
		}
		
		public static Model getModel(String ontFile, String dataFile) {
			Model model = ModelFactory.createDefaultModel();
			
			model.read(ontFile, ontPrefix);
			model.read(dataFile, baseURI);
			return model;
		}
		
		public static Model getInferredModel() {			
			Dataset infDataset = TDB2Factory.connectDataset(inftdbDirectory) ;
			infDataset.begin();
			
			Model inftdbModel = infDataset.getDefaultModel();
			infDataset.end();
			
			return inftdbModel;
		}
		
		static public void main(String[] args) {
			
			generateModel();		
			// model.write(System.out, "TURTLE");
		}
}
