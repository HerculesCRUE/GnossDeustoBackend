// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using System;
using System.Collections.Generic;
using System.Text;
using VDS.Common.Collections;

namespace VDS.RDF.Query.Inference
{
    /// <summary>
    /// An Inference Engine which uses RDFS reasoning.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Does basic RDFS inferencing using the schema taken from the Graph(s) which are provided in calls to the reasoners <see cref="StaticRdfsReasoner.Initialise">Initialise()</see> method.
    /// </para>
    /// <para>
    /// Types of inference performed are as follows:.
    /// </para>
    /// <ul>
    ///     <li>Class hierarchy reasoning - asserts additional types triples for anything that is typed as the subclass of a class.</li>
    ///     <li>Property hierarchy reasoning - asserts additional property triples for anything where the predicate is a subproperty of a defined property</li>   
    /// </ul>
    /// </remarks>
    public class RohRdfsReasoner : IInferenceEngine
    {
        private readonly Dictionary<INode, INode> _classMappings = new Dictionary<INode, INode>();
        private readonly Dictionary<INode, INode> _propertyMappings = new Dictionary<INode, INode>();
        private readonly IUriNode _rdfType, _rdfsClass, _rdfsSubClass, _rdfProperty, _rdfsSubProperty;

        /// <summary>
        /// Creates a new instance of the Static RdfsReasoner.
        /// </summary>
        public RohRdfsReasoner()
        {
            Graph g = new Graph();
            _rdfType = g.CreateUriNode("rdf:type");
            _rdfsClass = g.CreateUriNode("rdfs:Class");
            _rdfsSubClass = g.CreateUriNode("rdfs:subClassOf");
            _rdfProperty = g.CreateUriNode("rdf:Property");
            _rdfsSubProperty = g.CreateUriNode("rdfs:subPropertyOf");
        }

        /// <summary>
        /// Applies inference to the given Graph and outputs the inferred information to that Graph.
        /// </summary>
        /// <param name="g">Graph.</param>
        public virtual void Apply(IGraph g)
        {
            Apply(g, g);
        }

        /// <summary>
        /// Applies inference to the Input Graph and outputs the inferred information to the Output Graph.
        /// </summary>
        /// <param name="input">Graph to apply inference to.</param>
        /// <param name="output">Graph inferred information is output to.</param>
        public virtual void Apply(IGraph input, IGraph output)
        {
            // Infer information
            List<Triple> inferences = new List<Triple>();
            foreach (Triple t in input.Triples)
            {
                // Apply class/property hierarchy inferencing
                if (t.Predicate.Equals(_rdfType))
                {
                    if (!t.Object.Equals(_rdfsClass) && !t.Object.Equals(_rdfProperty))
                    {
                        InferClasses(t, output, inferences);
                    }
                }
                else if (t.Predicate.Equals(_rdfsSubClass))
                {
                    // Assert that this thing is a Class
                    inferences.Add(new Triple(t.Subject.CopyNode(output), _rdfType.CopyNode(output), _rdfsClass.CopyNode(output)));
                }
                else if (t.Predicate.Equals(_rdfsSubProperty))
                {
                    // Assert that this thing is a Property
                    inferences.Add(new Triple(t.Subject.CopyNode(output), _rdfType.CopyNode(output), _rdfProperty.CopyNode(output)));
                }
                else if (_propertyMappings.ContainsKey(t.Predicate))
                {
                    INode property = t.Predicate;

                    // Navigate up the property hierarchy asserting additional properties if able
                    while (_propertyMappings.ContainsKey(property))
                    {
                        if (_propertyMappings[property] != null)
                        {
                            // Assert additional properties
                            inferences.Add(new Triple(t.Subject.CopyNode(output), _propertyMappings[property].CopyNode(output), t.Object.CopyNode(output)));
                            property = _propertyMappings[property];
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            // Assert the inferred information
            inferences.RemoveAll(t => t.Subject.NodeType == NodeType.Literal);
            if (inferences.Count > 0)
            {
                output.Assert(inferences);
            }
        }

        /// <summary>
        /// Imports any Class heirarchy information from the given Graph into the Reasoners Knowledge Base in order to initialise the Reasoner.
        /// </summary>
        /// <param name="g">Graph to import from.</param>
        /// <remarks>
        /// Looks for Triples defining things to be classes and those defining that something is a subClass of something.
        /// </remarks>
        public void Initialise(IGraph g)
        {
            foreach (Triple t in g.Triples)
            {
                if (t.Predicate.Equals(_rdfType))
                {
                    if (t.Object.Equals(_rdfsClass))
                    {
                        // The Triple defines a Class
                        if (!_classMappings.ContainsKey(t.Subject))
                        {
                            _classMappings.Add(t.Subject, null);
                        }
                    }
                    else if (t.Object.Equals(_rdfProperty) && !_propertyMappings.ContainsKey(t.Subject))
                    {
                        _propertyMappings.Add(t.Subject, null);
                    }
                }
                else if (t.Predicate.Equals(_rdfsSubClass))
                {
                    // The Triple defines a Sub Class
                    if (!_classMappings.ContainsKey(t.Subject))
                    {
                        _classMappings.Add(t.Subject, t.Object);
                    }
                    else if (_classMappings[t.Subject] == null)
                    {
                        _classMappings[t.Subject] = t.Object;
                    }
                }
                else if (t.Predicate.Equals(_rdfsSubProperty))
                {
                    // The Triple defines a Sub property
                    if (!_propertyMappings.ContainsKey(t.Subject))
                    {
                        _propertyMappings.Add(t.Subject, t.Object);
                    }
                    else if (_propertyMappings[t.Subject] == null)
                    {
                        _propertyMappings[t.Subject] = t.Object;
                    }
                }
                else
                {
                    // Just add the property as a predicate
                    if (!_propertyMappings.ContainsKey(t.Predicate))
                    {
                        _propertyMappings.Add(t.Predicate, null);
                    }
                }
            }
        }

        /// <summary>
        /// Helper method which applies Class hierarchy inferencing.
        /// </summary>
        /// <param name="t">Triple defining the type for something.</param>
        /// <param name="output">Output Graph.</param>
        /// <param name="inferences">List of Inferences.</param>
        private void InferClasses(Triple t, IGraph output, List<Triple> inferences)
        {
            INode type = t.Object;

            // Navigate up the class hierarchy asserting additional types if able
            while (_classMappings.ContainsKey(type))
            {
                if (_classMappings[type] != null)
                {
                    // Assert additional type information
                    inferences.Add(new Triple(t.Subject.CopyNode(output), t.Predicate.CopyNode(output), _classMappings[type].CopyNode(output)));
                    type = _classMappings[type];
                }
                else
                {
                    break;
                }
            }
        }
    }
}
