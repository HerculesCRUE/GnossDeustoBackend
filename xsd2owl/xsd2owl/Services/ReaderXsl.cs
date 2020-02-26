using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;

namespace xsd2owl.Services
{
    public class ReaderXsl
    {
        const string stylesheetXSL = "C:\\Hercules\\xsd2owlApi\\xsd2owl\\xsd2owl\\xsl\\xsd2owl.xsl";
        public string performTask(string xsdUrl)
        {
            string response = "";
            //WebRequest req = WebRequest.Create(xsdUrl);
            //long length = req.ContentLength;
            //if(length > 200000)
            //{
            //    throw new Exception("longitud mayor a la permitida (200000)");
            //}
           // FileStream xsdFile = File.Open(xsdUrl,FileMode.Open);

            XslCompiledTransform transform = new XslCompiledTransform();
            StreamReader sr = new StreamReader(stylesheetXSL);
            XmlTextReader textReader = new XmlTextReader(sr);
            XsltSettings sets = new XsltSettings(false, true);
            var resolver = new XmlUrlResolver();
            transform.Load(textReader,sets, resolver);
            StringWriter writer = new StringWriter();
            //XmlReader reader = XmlReader.Create(xsdFile);

            transform.Transform(new XmlTextReader(new StreamReader(xsdUrl)), null, writer);
            //xsdFile.Dispose();
            response = writer.ToString();
            response = response.Replace("$amp;", "&");
            return response;
        }
    }
}
