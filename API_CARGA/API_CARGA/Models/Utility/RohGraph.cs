using System;
using System.Collections.Generic;
using System.Text;
using VDS.Common.Collections;
using VDS.RDF;
using VDS.RDF.Query.Inference;

namespace VDS.RDF
{
    public class RohGraph : Graph
    {
        /// <summary>
        /// Creates a new unused Blank Node ID and returns it.
        /// </summary>
        /// <returns></returns>
        public override String GetNextBlankNodeID()
        {
            String id = Guid.NewGuid().ToString();
            id="N" + id.Replace("-", "");
            return id;
        }
    }
}
