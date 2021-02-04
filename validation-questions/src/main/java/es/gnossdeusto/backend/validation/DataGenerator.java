package es.gnossdeusto.backend.validation;

import java.io.FileNotFoundException;
import java.io.FileReader;
import java.util.Iterator;

import org.mindswap.pellet.jena.PelletReasonerFactory;

import com.hp.hpl.jena.rdf.model.InfModel;
import com.hp.hpl.jena.rdf.model.Model;
import com.hp.hpl.jena.rdf.model.ModelFactory;
import com.hp.hpl.jena.rdf.model.Property;
import com.hp.hpl.jena.rdf.model.Resource;
import com.hp.hpl.jena.rdf.model.Statement;
import com.hp.hpl.jena.rdf.model.StmtIterator;
import com.hp.hpl.jena.reasoner.Reasoner;
import com.hp.hpl.jena.reasoner.ReasonerRegistry;
import com.hp.hpl.jena.reasoner.ValidityReport;
import com.hp.hpl.jena.reasoner.ValidityReport.Report;
import com.hp.hpl.jena.util.FileManager;
import com.hp.hpl.jena.util.PrintUtil;

public class DataGenerator {

	static String tdbDirectory = "/home/mikel/PROYECTOS/Hercules/validation-questions/dataset";
	static String inftdbDirectory = "/home/mikel/PROYECTOS/Hercules/validation-questions/inferred";

	static String ontFile = "file:/home/mikel/PROYECTOS/Hercules/roh/roh-v2.owl";
	static String ontPrefix = "http://purl.org/roh#";
	static String dataFile = "file:/home/mikel/PROYECTOS/Hercules/roh/examples/data.ttl";
	static String baseURI = "http://purl.org/roh/data#";
	static String inferredOutput = "/home/mikel/PROYECTOS/Hercules/roh/roh-v2-inferred.owl";
	static String[] researcherNames = { "Pedro", "Maria", "Juan", "Sofia", "Ana" };

	public static void printStatements(Model m, Resource s, Property p, Resource o) {
		for (StmtIterator i = m.listStatements(s, p, o); i.hasNext();) {
			Statement stmt = i.nextStatement();
			System.out.println(" - " + PrintUtil.print(stmt));
		}
	}

	public static void printIterator(Iterator<Report> i, String header) throws InvalidOntologyException {
		System.out.println(header);
		for (int c = 0; c < header.length(); c++)
			System.out.print("=");
		System.out.println();

		if (i.hasNext()) {
			while (i.hasNext()) {
				Report report = i.next();
				System.out.println(report.toString());
			}
			throw new InvalidOntologyException("Invalid ontology, see logs files...");
		} else
			System.out.println("<EMPTY>");

		System.out.println();
	}

	public static InfModel generateModel(String ontFile, String dataFile) {

		System.out.println("Loading ontology...");
		Model rohOntology = FileManager.get().loadModel(ontFile);
		System.out.println("Loading example data...");
		Model data = FileManager.get().loadModel(dataFile);

		System.out.println("Loading OWL reasoner...");
		Reasoner reasoner = ReasonerRegistry.getOWLMiniReasoner();
		System.out.println("Binding ontology to OWL reasoner...");
		reasoner = reasoner.bindSchema(rohOntology);

		System.out.println("Creating inference model...");
		InfModel infmodel = ModelFactory.createInfModel(reasoner, data);
		// try {
		// System.out.println("Writing inference model...");
		// infmodel.write(new FileWriter(output), "TURTLE");
		// } catch (IOException e) {
		// // TODO Auto-generated catch block
		// e.printStackTrace();
		// }
		return infmodel;
		// Dataset dataset = TDB2Factory.connectDataset(tdbDirectory) ;
		// dataset.begin();
		// Model tdbModel = dataset.getDefaultModel();
		//
		// // Model schema = FileManager.get().loadModel(ontFile);
		// Model data = FileManager.get().loadModel(dataFile);
		//
		// // tdbModel.add(schema);
		// // tdbModel.commit();
		//
		//
		// Reasoner reasoner = ReasonerRegistry.getOWLMiniReasoner();
		// reasoner = reasoner.bindSchema(tdbModel);
		//
		// InfModel infmodel = ModelFactory.createInfModel(reasoner, data);
		//
		// Dataset infDataset = TDB2Factory.connectDataset(inftdbDirectory) ;
		// infDataset.begin();
		//
		// Model inftdbModel = infDataset.getDefaultModel();
		// inftdbModel.add(infmodel);
		// infDataset.commit();
		// infDataset.end();
		//
		// dataset.end();

		// // Create Researchers
		// Map<String, Resource> researchers = new HashMap<String, Resource>();
		// for (int i = 0; i < 5; i++) {
		// Resource researcher =
		// model.createResource(baseURI.concat("researcher-".concat(String.valueOf(i))));
		// researcher.addProperty(RDF.type, FOAF.Person);
		// researcher.addProperty(FOAF.name, researcherNames[i]);
		// researchers.put(researcher.getURI(), researcher);
		// }
		//
		// // Create research groups
		// Map<String, Resource> researchGroups = new HashMap<String, Resource>();
		// for (int i = 0; i < 3; i++) {
		// Resource researchGroup =
		// model.createResource(baseURI.concat("research-group-".concat(String.valueOf(i))));
		// researchGroup.addProperty(RDF.type,
		// ontology.getOntClass(ontPrefix.concat("ResearchGroup")));
		// // Associate researchers to research groups
		// if (i == 0) {
		// researchers.get(baseURI.concat("researcher-".concat(String.valueOf(0))));
		// } else if (i == 1) {
		//
		// } else if (i == 2) {
		//
		// }
		// researchGroups.put(researchGroup.getURI(), researchGroup);
		//
		// }
	}

	public static Model getModel(String ontFile, String dataFile, String uneskos) {

		Reasoner reasoner = PelletReasonerFactory.theInstance().create();

		// create an empty model
		Model emptyModel = ModelFactory.createDefaultModel();

		// create an inferencing model using Pellet reasoner
		InfModel model = ModelFactory.createInfModel(reasoner, emptyModel);

		// read the file
		try {
			model.read(new FileReader(ontFile), ontPrefix);
			model.read(new FileReader(dataFile), baseURI, "TURTLE");
			model.read(new FileReader(uneskos), "TURTLE");
		} catch (FileNotFoundException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		// print validation report
		ValidityReport report = model.validate();
		try {
			printIterator(report.getReports(), "Validation Results");
		} catch (InvalidOntologyException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
			
	        return model;
		}
		
		public static Model getInferredModel(String ontFile, String dataFile) {			
			
			
			return generateModel(ontFile, dataFile);
		}
		
		static public void main(String[] args) {
			
			generateModel(args[0], args[1]);		
			// model.write(System.out, "TURTLE");
		}
}
