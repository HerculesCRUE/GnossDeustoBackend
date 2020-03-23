using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OaiPmhNet;
using OaiPmhNet.Converters;
using OaiPmhNet.Models;
using OaiPmhNet.Models.OAIPMH;
using OaiPmhNet.Models.Services;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;


namespace OAI_PMH.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class OAI_PMHController : Controller
    {
        private ConfigJson _configJsonHandler;
        private IOaiConfiguration _configOAI;

        public OAI_PMHController(ConfigJson configJsonHandler)
        {
            _configJsonHandler = configJsonHandler;
            _configOAI = OaiConfiguration.Instance;
            _configOAI.SupportSets = _configJsonHandler.GetConfig().SupportSets;
            _configOAI.RepositoryName = _configJsonHandler.GetConfig().RepositoryName;
            _configOAI.AdminEmails = _configJsonHandler.GetConfig().AdminEmails;
            _configOAI.DeletedRecord = _configJsonHandler.GetConfig().DeletedRecord;
            _configOAI.Granularity = _configJsonHandler.GetConfig().Granularity;
            _configOAI.EarliestDatestamp = _configJsonHandler.GetConfig().EarliestDatestamp;
        }

        /// <summary>
        /// <p>Verbs, one of the defined OAI-PMH requests. <a href="https://www.openarchives.org/OAI/openarchivesprotocol.html" target="_blank">More information</a></p>
        /// <ul>
        ///     <li>
        ///			<h2><a name="GetRecord">GetRecord</a></h2>
        ///			<h3>Summary and Usage Notes</h3>
        ///			<p>This verb is used to retrieve an individual metadata record from a repository. 
        ///			Required arguments specify the identifier of the item from which the record 
        ///			is requested and the format of the metadata that should be included in the record. 
        ///			Depending on the level at which a repository tracks <a href="#deletion">deletions</a>, 
        ///			a header with a "deleted" value for the <code>status</code> 
        ///			attribute <b>may</b> be returned, in case the metadata format specified by the 
        ///			<code>metadataPrefix</code> is no longer 
        ///			available from the repository or from the specified item.</p>
        ///			<h3>Arguments</h3>
        ///			<ul>
        ///			  <li><code><b>identifier</b></code> a <i>required</i> argument that specifies 
        ///			    the <a href="#UniqueIdentifier">unique identifier</a>  of the item in the 
        ///			    <a href="#Repository">repository</a> from which the 
        ///			    <a href="#Record">record</a> must be disseminated.</li> 
        ///			  <li><code><b>metadataPrefix</b></code> a <i>required</i> argument that 
        ///			    specifies the <a href="#metadataPrefix"><code>metadataPrefix</code></a> of the 
        ///			    format that should be included in the <a href="#Record">metadata part of 
        ///			    the returned record</a> . A record should only be returned if the format 
        ///			    specified by the <a href="#metadataPrefix"><code>metadataPrefix</code></a> can be 
        ///			    disseminated from the item identified by the value of the identifier argument. 
        ///			    The metadata formats supported by a repository and for a particular record can 
        ///			    be retrieved using the 
        ///			    <code><a href="#ListMetadataFormats">ListMetadataFormats</a></code> request.</li>
        ///			</ul>
        ///			<h3>Error and Exception Conditions</h3>
        ///			<ul>
        ///			  <li><code><b>badArgument</b></code> - 
        ///			    The request includes illegal arguments or is missing required arguments.</li> 
        ///			  <li><code><b>cannotDisseminateFormat</b></code> - The value of the <code>metadataPrefix</code> argument is not 
        ///			    supported by the item identified by the value of the <code>identifier</code> argument.</li>
        ///			  <li><code><b>idDoesNotExist</b></code> - The value of the <code>identifier</code> argument is unknown 
        ///			    or illegal in this repository.</li>
        ///			</ul>
        ///		</li>
        ///		<li>
        ///			<h2><a name="Identify">Identify</a></h2>
        ///			<h3>Summary and Usage Notes</h3>
        ///			<p>This verb is used to retrieve information about a repository. Some of 
        ///			the information returned is required as part of the OAI-PMH. Repositories 
        ///			<b>may</b> also employ the Identify verb to return additional descriptive 
        ///			information.</p>
        ///			<h3>Arguments</h3>
        ///			<p>None</p>
        ///			<h3>Error and Exception Conditions</h3>
        ///			<ul>
        ///			  <li><code><b>badArgument</b></code> - 
        ///			    The request includes illegal arguments.</li>
        ///			</ul>
        ///			<h3>Response Format</h3>
        ///			<p>The response <b>must</b> include one instance of the following elements:</p>
        ///			<ul>
        ///			  <li><code>repositoryName</code> : 
        ///			    a human readable name for the repository;</li>
        ///			  <li><code>baseURL</code> : 
        ///			    the <a href="#HTTPRequestFormat">base URL</a> of the repository;</li>
        ///			  <li><code>protocolVersion</code> : 
        ///			    the version of the OAI-PMH supported by the repository;</li> 
        ///			  <li><code>earliestDatestamp</code> : 
        ///			    a <a href="#Dates">UTCdatetime</a> that is the guaranteed lower limit of all datestamps 
        ///			    recording changes, modifications, or deletions in the repository. A repository 
        ///			    <b>must not</b> use <a href="#Datestamp">datestamps</a> lower than the one specified 
        ///			    by the content of the <code>earliestDatestamp</code> element.
        ///			    <code>earliestDatestamp</code> must be expressed at the finest 
        ///			    <a href="#Datestamp">granularity</a> supported by the repository.</li>
        ///			  <li><code>deletedRecord</code> : 
        ///			    the manner in which the repository supports the notion of 
        ///			    <a href="#DeletedRecords">deleted records</a>. Legitimate values are <code>no</code> ; 
        ///			    <code>transient</code> ; <code>persistent</code> with meanings defined in the section on 
        ///			    <a href="#DeletedRecords">deletion</a>.</li> 
        ///			  <li><code>granularity:</code> 
        ///			    the finest <a href="#Datestamp">harvesting granularity</a> supported by the repository. 
        ///			    The legitimate values are <code>YYYY-MM-DD</code> and <code>YYYY-MM-DDThh:mm:ssZ</code> 
        ///			    with meanings as defined in <a href="http://www.w3.org/TR/NOTE-datetime">ISO8601</a>.</li>
        ///			</ul>
        ///			<p>The response <b>must</b> include one or more instances of the following 
        ///			element:</p>
        ///			<ul>
        ///			  <li><code>adminEmail</code> : 
        ///			    the e-mail address of an administrator of the repository.</li>
        ///			</ul>
        ///			<p>The response <b>may</b> include multiple instances of the following 
        ///			<b>optional</b> elements:</p>
        ///			<ul>
        ///			  <li><code>compression</code> : a compression encoding supported by the 
        ///			    repository. The <b>recommended</b> values are those defined for the 
        ///			    <code>Content-Encoding</code> header in Section 14.11 of 
        ///			    <a href="http://www.ietf.org/rfc/rfc2616.txt">RFC 2616</a> describing 
        ///			    HTTP 1.1. A <code>compression</code> element <b>should not</b> be 
        ///			    included for the <code>identity</code> encoding, which is implied.</li> 
        ///			  <li><code>description</code> : an extensible mechanism for communities 
        ///			    to describe their repositories. For example, the <code>description</code> 
        ///			    container could be used to include collection-level metadata in the response 
        ///			    to the Identify request. 
        ///			    <a href="http://www.openarchives.org/OAI/2.0/guidelines.htm">Implementation 
        ///			    Guidelines</a> are available to give directions with this respect. Each <code>description</code> container 
        ///			    <b>must</b> be accompanied by the URL of an XML schema describing the 
        ///			    structure of the description container.</li>
        ///			</ul>
        ///		</li>
        ///		<li>
        ///			<h2><a name="ListIdentifiers">ListIdentifiers</a></h2>
        ///			<h3>Summary and Usage Notes</h3>
        ///			<p>This verb is an abbreviated form of 
        ///			<a href="#ListRecords"><code>ListRecords</code></a>, retrieving only 
        ///			<a href="#header">headers</a> rather than <a href="#Record">records</a>. 
        ///			Optional arguments permit selective harvesting of 
        ///			<a href="#header">headers</a> based on <a href="#Set">set</a> membership 
        ///			and/or datestamp. Depending on the repository's support for 
        ///			<a href="#deletion">deletions</a>, a returned <a href="#header">header</a> 
        ///			<b>may</b> have a <code>status</code> attribute of "deleted" if a record 
        ///			matching the arguments specified in the request has been deleted.</p>
        ///			<h3>Arguments</h3>
        ///			<ul>
        ///			  <li><code><b>from</b></code> an <i>optional</i> 
        ///			    argument with a <a href="#Dates">UTCdatetime value</a>, 
        ///			    which specifies a lower bound for datestamp-based 
        ///			    <a href="#Datestamp">selective harvesting</a>.</li>
        ///			  <li><code><b>until</b></code> an <i>optional</i> 
        ///			    argument with a <a href="#Dates">UTCdatetime value</a>, 
        ///			    which specifies a upper bound for datestamp-based 
        ///			    <a href="#Datestamp">selective harvesting</a>.</li>
        ///			  <li><code><b>metadataPrefix</b> </code>a <i>required</i> argument, which specifies 
        ///			    that <a href="#header">headers</a> should be returned only if the metadata format matching 
        ///			    the supplied <a href="#metadataPrefix"><code>metadataPrefix</code></a> 
        ///			    is available or, depending on the repository's support for 
        ///			    <a href="#deletion">deletions</a>, has been deleted. The metadata formats supported 
        ///			    by a repository and for a particular item can be retrieved using the 
        ///			    <a href="#ListMetadataFormats"><code>ListMetadataFormats</code></a> 
        ///			    request.</li> 
        ///			  <li><code><b>set</b></code> an <i>optional</i> argument with a 
        ///			    <a href="#Set"><code>setSpec</code> value</a> , which specifies 
        ///			    <a href="#Set">set</a> criteria for 
        ///			    <a href="#SelectiveHarvestingandSets">selective harvesting</a>.</li>
        ///			  <li><code><b>resumptionToken</b></code> an <i>exclusive </i>argument with a value 
        ///			    that is the <a href="#FlowControl">flow control</a> token returned by a 
        ///			    previous <code><a href="#ListIdentifiers">ListIdentifiers</a></code> 
        ///			    request that issued an incomplete list.</li>
        ///			</ul>
        ///			<h3>Error and Exception Conditions</h3>
        ///			<ul>
        ///			  <li><code><b>badArgument</b></code> - 
        ///			    The request includes illegal arguments or is missing required arguments.</li> 
        ///			  <li><code><b>badResumptionToken</b></code> - 
        ///			    The value of the <code>resumptionToken</code> 
        ///			    argument is invalid or expired.</li> 
        ///			  <li><code><b>cannotDisseminateFormat</b></code> - 
        ///			    The value of the <code>metadataPrefix</code> argument is not supported 
        ///			    by the repository.</li> 
        ///			  <li><code><b>noRecordsMatch</b></code>- 
        ///			    The combination of the values of the <code>from</code>, <code>until</code>, 
        ///			    and <code>set</code> arguments results in an empty list.</li>
        ///			  <li><code><b>noSetHierarchy</b></code> - The repository does not support sets.</li>
        ///			</ul>
        ///		</li>
        ///		<li>
        ///			<h2><a name="ListMetadataFormats">ListMetadataFormats</a></h2>
        ///			<h3>Summary and Usage Notes</h3>
        ///			<p>This verb is used to retrieve the metadata formats available from a 
        ///			repository. An optional argument restricts the request to the formats 
        ///			available for a specific item.</p>
        ///			<h3>Arguments</h3>
        ///			<ul>
        ///			  <li><code><b>identifier</b></code> an 
        ///			  <i>optional</i> argument that specifies the unique identifier of the item for 
        ///			  which available metadata formats are being requested. If this argument 
        ///			  is omitted, then the response includes all metadata formats supported by this 
        ///			  repository. Note that the fact that a metadata format is supported by a 
        ///			  repository does <i>not</i> mean that it can be disseminated from all items in 
        ///			  the repository.</li>
        ///			</ul>
        ///			<h3>Error and Exception Conditions</h3>
        ///			<ul>
        ///			  <li><code><b>badArgument</b></code> - 
        ///			    The request includes illegal arguments or is missing required arguments.</li>
        ///			  <li><code><b>idDoesNotExist</b></code> - The value of the 
        ///			    <code>identifier</code> argument is unknown 
        ///			    or illegal in this repository.</li> 
        ///			  <li><code><b>noMetadataFormats</b></code> - There are no metadata formats 
        ///			    available for the specified item. </li>
        ///			</ul>
        ///		</li>
        ///		<li>
        ///			<h2><a name="ListRecords">ListRecords</a></h2>
        ///			<h3>Summary and Usage Notes</h3>
        ///			<p>This verb is used to harvest records from a repository. Optional arguments 
        ///			permit <a href="#Datestamp">selective harvesting</a> of 
        ///			<a href="#header">records</a> based on 
        ///			<a href="#Set">set</a> membership 
        ///			and/or datestamp. Depending on the repository's support for 
        ///			<a href="#deletion">deletions</a>, a returned 
        ///			<a href="#header">header</a> <b>may</b> have a <code>status</code> attribute of "deleted" 
        ///			if a record matching the arguments specified in the request has been deleted. 
        ///			No metadata will be present for records with deleted status.</p>
        ///			<h3>Arguments</h3>
        ///			<ul>
        ///			  <li><code><b>from</b></code> an <i>optional</i> 
        ///			    argument with a <a href="#Dates">UTCdatetime value</a>, which 
        ///			    specifies a lower bound for datestamp-based 
        ///			    <a href="#Datestamp">selective harvesting</a>.</li>
        ///			  <li><code><b>until</b></code> an <i>optional</i> argument with a 
        ///			    <a href="#Dates">UTCdatetime value</a>, which specifies a upper bound for datestamp-based 
        ///			    <a href="#Datestamp">selective harvesting</a>.</li>
        ///			  <li><code><b>set</b></code> an <i>optional</i> argument with a 
        ///			    <a href="#Set"><code>setSpec</code> value</a> , which specifies 
        ///			    <a href="#Set">set</a> criteria for <a href="#SelectiveHarvestingandSets">selective harvesting</a>.</li>
        ///			  <li><code><b>resumptionToken</b></code> an <i>exclusive</i> argument with 
        ///			    a value that is the <a href="#FlowControl">flow control</a> token returned by 
        ///			    a previous <code>ListRecords</code> request that 
        ///			    issued an incomplete list.</li> 
        ///			  <li><code><b>metadataPrefix</b></code> a 
        ///			    <i>required</i> argument (unless the exclusive argument <code>resumptionToken</code> is used) that 
        ///			    specifies the <a href="#metadataPrefix"><code>metadataPrefix</code></a> of the format 
        ///			    that should be included in the <a href="#Record">metadata part of the returned records</a>.   Records 
        ///			    should be included only for items from which the metadata format<br />
        ///			    matching the <a href="#metadataPrefix"><code>metadataPrefix</code></a> can be 
        ///			    disseminated. The metadata formats supported by a repository and for a 
        ///			    particular item can be retrieved using the 
        ///			    <code><a href="#ListMetadataFormats">ListMetadataFormats</a></code> request.</li>
        ///			</ul>
        ///			<h3>Error and Exception Conditions</h3>
        ///			<ul>
        ///			  <li><code><b>badArgument</b></code> - 
        ///			    The request includes illegal arguments or is missing required arguments.</li> 
        ///			  <li><code><b>badResumptionToken</b></code> - 
        ///			    The value of the <code>resumptionToken</code> argument is 
        ///			    invalid or expired.</li> 
        ///			  <li><code><b>cannotDisseminateFormat</b></code> - 
        ///			    The value of the <code>metadataPrefix</code> argument is not 
        ///			    supported by the repository.</li>
        ///			  <li><code><b>noRecordsMatch</b></code> - 
        ///			    The combination of the values of the <code>from</code>, <code>until</code>, 
        ///			    <code>set</code> and <code>metadataPrefix</code> arguments results in an empty list.</li>
        ///			  <li><code><b>noSetHierarchy</b></code> - 
        ///			    The repository does not support sets.</li>
        ///			</ul>
        ///		</li>
        ///		<li>
        ///			<h2><a name="ListSets">ListSets</a></h2>
        ///			<h3>Summary and Usage Notes</h3>
        ///			<p>This verb is used to retrieve the set structure of a repository, useful for 
        ///			<a href="#SelectiveHarvestingandSets">selective harvesting</a>.</p>
        ///			<h3>Arguments</h3>
        ///			<ul>
        ///			  <li><b><code>resumptionToken</code></b> 
        ///			    an <i>exclusive</i> argument with a value that is the 
        ///			    <a href="#FlowControl">flow control</a> token returned by a 
        ///			    previous <code>ListSets</code> request that issued 
        ///			    an incomplete list.</li>
        ///			</ul>
        ///			<h3>Error and Exception Conditions</h3>
        ///			<ul>
        ///			  <li><code><b>badArgument</b></code> - 
        ///			    The request includes illegal arguments or is missing required arguments.</li> 
        ///			  <li><code><b>badResumptionToken</b></code> - 
        ///			    The value of the <code>resumptionToken</code> argument is 
        ///			    invalid or expired.</li> 
        ///			  <li><code><b>noSetHierarchy</b></code> - 
        ///			    The repository does not support sets.</li>
        ///			</ul>
        ///		</li>
        ///	</ul>
        /// </summary>    
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public FileResult Get(OaiVerb verb, string identifier = "", string metadataPrefix = "", string from = "", string until = "", string set = "", string resumptionToken = "")
        {

            //CONFIG OAI-PMH
            _configOAI.BaseUrl = () =>
            {
                Uri baseUri = new Uri(string.Concat(this.Request.Scheme, "://", this.Request.Host, this.Request.Path));
                return baseUri.AbsoluteUri;
            };


            //MetadataFormatRepository
            MetadataFormatRepository metadataFormatRepository = new MetadataFormatRepository(_configJsonHandler.GetConfig().MetadataFormats);

            RecordRepository recordRepository = new RecordRepository(_configOAI, _configJsonHandler);

            //SetRepository
            SetRepository setRepository = new SetRepository(_configOAI, _configJsonHandler.GetConfig().Sets);

            DataProvider provider = new DataProvider(_configOAI, metadataFormatRepository, recordRepository, setRepository);

            ArgumentContainer arguments = new ArgumentContainer(verb.ToString(), metadataPrefix, resumptionToken, identifier, from, until, set);
            XDocument document = provider.ToXDocument(DateTime.Now.AddMinutes(100), arguments);

            var memoryStream = new MemoryStream();
            var xmlWriter = XmlWriter.Create(memoryStream);
            document.WriteTo(xmlWriter);
            xmlWriter.Flush();
            byte[] array = memoryStream.ToArray();
            return File(array, "application/xml");
        }
    }
}