using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.ViewModels
{
    public class UriStructureViewModel
    {
        public string GetUriStrcuture()
        {
            StringBuilder structure = new StringBuilder();
            structure.Append("{");
            structure.Append("\"uriStructure\": {");
            structure.Append("\"name\": \"uriExampleStructure\", ");
            structure.Append("\"components\": [");
            structure.Append("{");
            structure.Append("\"uriComponent\": \"base\", ");
            structure.Append("\"uriComponentValue\": \"base\", ");
            structure.Append("\"uriComponentOrder\": 1, ");
            structure.Append("\"mandatory\": true, ");
            structure.Append("\"finalCharacter\": \"/\"");
            structure.Append("},");
            structure.Append("{");
            structure.Append("\"uriComponent\": \"character\", ");
            structure.Append("\"uriComponentValue\": \"character@RESOURCE\", ");
            structure.Append("\"uriComponentOrder\": 2, ");
            structure.Append("\"mandatory\": true, ");
            structure.Append("\"finalCharacter\": \"/\"");
            structure.Append("},");
            structure.Append("{");
            structure.Append("\"uriComponent\": \"resourceClass\", ");
            structure.Append("\"uriComponentValue\": \"resourceClass@RESOURCECLASS\", ");
            structure.Append("\"uriComponentOrder\": 3, ");
            structure.Append("\"mandatory\": true, ");
            structure.Append("\"finalCharacter\": \"/\" ");
            structure.Append("}, ");
            structure.Append("{");
            structure.Append("\"uriComponent\": \"identifier\", ");
            structure.Append("\"uriComponentValue\": \"@ID\", ");
            structure.Append("\"uriComponentOrder\": 4, ");
            structure.Append("\"mandatory\": true, ");
            structure.Append("\"finalCharacter\": \"\"");
            structure.Append("\"resourcesClass\": [");
            structure.Append("{");
            structure.Append("\"resourceClass\": \"Example\", ");
            structure.Append("\"labelResourceClass\": \"example\", ");
            structure.Append("\"resourceURI\": \"uriExampleStructure\"");
            structure.Append("}]}");
            return structure.ToString();
        }
    }
}
