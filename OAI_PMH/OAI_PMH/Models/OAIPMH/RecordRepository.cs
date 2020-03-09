using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OaiPmhNet.Converters;
using OaiPmhNet.Models;

namespace OaiPmhNet.Models.OAIPMH
{
    public class RecordRepository : IRecordRepository
    {
        private readonly IOaiConfiguration _configurationOAI;

        public RecordRepository(IOaiConfiguration configurationOAI)
        {
            _configurationOAI = configurationOAI;
        }

        public Record GetRecord(string identifier, string metadataPrefix)
        {
            throw new Exception("TODO");
        }

        public RecordContainer GetRecords(ArgumentContainer arguments, IResumptionToken resumptionToken = null)
        {
            throw new Exception("TODO");
        }

        public RecordContainer GetIdentifiers(ArgumentContainer arguments, IResumptionToken resumptionToken = null)
        {
            throw new Exception("TODO");
        }

    }
}
