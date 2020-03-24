using OaiPmhNet.Models.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace OaiPmhNet.Models.OAIPMH
{
    /// <summary>
    /// Objeto con la información correspondiente al CVN
    /// </summary>
    public class CVN
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pXML_CVN">XML del CVN</param>
        /// <param name="pId">Identificador del CVN</param>
        /// <param name="pRutaEjecutablePyhton">Ruta del ejecutable Pyhton</param>
        /// <param name="pRutaScriptPython">Ruta de script de Python</param>
        public CVN(string pXML_CVN, string pId,string pRutaEjecutablePyhton,string pRutaScriptPython)
        {
            string input = Path.GetTempPath() + Guid.NewGuid().ToString() + ".xml";
            string output = Path.GetTempPath() + Guid.NewGuid().ToString() + ".xml";

            File.WriteAllText(input, pXML_CVN);

            Process p = new Process(); 
            p.StartInfo.FileName = pRutaEjecutablePyhton;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false; 
            p.StartInfo.Arguments = @$"{pRutaScriptPython} {input} {output} {pId} --format pretty-xml";
            p.Start();         
            p.WaitForExit();
            StreamReader sOutput = p.StandardOutput;
            string standardOutput = sOutput.ReadToEnd();
            StreamReader sError = p.StandardError;
            string standardError = sError.ReadToEnd();

            Id = pId;
            Date = DateTime.Now;
            if (string.IsNullOrEmpty(standardError))
            {
                rdf = File.ReadAllText(output);
            }
            else
            {
                throw new Exception("Error al convertir a RDF: Executable:"+ p.StartInfo.FileName + ". Args:"+ p.StartInfo.Arguments+ ". Error:"+standardError);
            }
        }

        /// <summary>
        /// RDF del CVN
        /// </summary>
        public string rdf { get; }
        
        /// <summary>
        /// Identificador
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Fecha del CVN
        /// </summary>
        public DateTime Date { get; }

        /// <summary>
        /// Nombre del CVN
        /// </summary>
        public string Name { get; }
    }
}
