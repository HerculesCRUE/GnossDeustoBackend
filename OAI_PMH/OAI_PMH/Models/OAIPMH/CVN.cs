using OaiPmhNet.Models.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace OaiPmhNet.Models.OAIPMH
{
    public class CVN
    {

        public CVN(string pXML_CVN, string pId,ConfigJson pConfigJsonHandler)
        {
            string input = Path.GetTempPath() + Guid.NewGuid().ToString() + ".xml";
            string output = Path.GetTempPath() + Guid.NewGuid().ToString() + ".xml";

            File.WriteAllText(input, pXML_CVN);

            Process p = new Process(); // create process to run the python program
            p.StartInfo.FileName = pConfigJsonHandler.GetConfig().PythonExe; //Python.exe location
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false; // ensures you can read stdout
            p.StartInfo.Arguments = @$"{pConfigJsonHandler.GetConfig().PythonScript} {input} {output} {pId} --format pretty-xml"; // start the python program with two parameters
            p.Start(); // start the process (the python program)            
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

        public string rdf { get; }

        public string oai_dc
        {
            get
            {
                if (!string.IsNullOrEmpty(rdf))
                {
                    return rdf;
                }
                throw new Exception("Los datos RDF no existen");
            }
        }

        public string Id { get; }
        public DateTime Date { get; }
        public string Name { get; }
    }
}
