using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.ViewModels
{
    public class UriStructureViewModel
    {
        public static string GetUriStrcuture()
        {
            string uriStructure;
            StringBuilder structure = new StringBuilder();
            structure.AppendLine("{");
            structure.AppendLine("\"uriStructure\": {");
            structure.AppendLine("\"name\": \"uriExampleStructure\", ");
            structure.AppendLine("\"components\": [");
            structure.AppendLine("{");
            structure.AppendLine("\"uriComponent\": \"base\", ");
            structure.AppendLine("\"uriComponentValue\": \"base\", ");
            structure.AppendLine("\"uriComponentOrder\": 1, ");
            structure.AppendLine("\"mandatory\": true, ");
            structure.AppendLine("\"finalCharacter\": \"/\"");
            structure.AppendLine("},");
            structure.AppendLine("{");
            structure.AppendLine("\"uriComponent\": \"character\", ");
            structure.AppendLine("\"uriComponentValue\": \"character@RESOURCE\", ");
            structure.AppendLine("\"uriComponentOrder\": 2, ");
            structure.AppendLine("\"mandatory\": true, ");
            structure.AppendLine("\"finalCharacter\": \"/\"");
            structure.AppendLine("},");
            structure.AppendLine("{");
            structure.AppendLine("\"uriComponent\": \"resourceClass\", ");
            structure.AppendLine("\"uriComponentValue\": \"resourceClass@RESOURCECLASS\", ");
            structure.AppendLine("\"uriComponentOrder\": 3, ");
            structure.AppendLine("\"mandatory\": true, ");
            structure.AppendLine("\"finalCharacter\": \"/\" ");
            structure.AppendLine("}, ");
            structure.AppendLine("{");
            structure.AppendLine("\"uriComponent\": \"identifier\", ");
            structure.AppendLine("\"uriComponentValue\": \"@ID\", ");
            structure.AppendLine("\"uriComponentOrder\": 4, ");
            structure.AppendLine("\"mandatory\": true, ");
            structure.AppendLine("\"finalCharacter\": \"\"");
            structure.AppendLine("}");
            structure.AppendLine("]");
            structure.AppendLine("},");
            structure.AppendLine("\"resourcesClass\": [");
            structure.AppendLine("{");
            structure.AppendLine("\"resourceClass\": \"Example\", ");
            structure.AppendLine("\"rdfType\": \"ejemplo\", ");
            structure.AppendLine("\"labelResourceClass\": \"example\", ");
            structure.AppendLine("\"resourceURI\": \"uriExampleStructure\"");
            structure.AppendLine("}");
            structure.AppendLine("]");
            structure.AppendLine("}");
            uriStructure = structure.ToString();
            return uriStructure;
        }
    }
}
