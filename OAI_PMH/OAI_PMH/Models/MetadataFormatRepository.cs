using System.Collections.Generic;
using System.Linq;
using OaiPmhNet.Models;

namespace OaiPmhNet.Test
{
    public class MetadataFormatRepository : IMetadataFormatRepository
    {
        private readonly Dictionary<string, MetadataFormat> _dictionary;

        public MetadataFormatRepository(IList<MetadataFormat> metadataFormats)
        {
            _dictionary = metadataFormats.ToDictionary(f => f.Prefix, f => f);
        }

        public MetadataFormat GetMetadataFormat(string prefix)
        {
            if (_dictionary.TryGetValue(prefix, out MetadataFormat format))
                return format;
            else
                return null;
        }

        public IEnumerable<MetadataFormat> GetMetadataFormats()
        {
            return _dictionary.Select(o => o.Value);
        }
    }
}
