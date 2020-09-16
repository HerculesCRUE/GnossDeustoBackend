// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace API_DISCOVER.Models.Entities
{
    [DataContract]
    public class ORCIDExpandedSearch
    {
        public ORCIDExpandedSearch() { 

        }

        [DataMember(Name = "expanded-result")]
        public List<Result> expanded_result { get; set; }

        [DataContract]
        public class Result
        {
            public Result() { 
            }
            [DataMember(Name = "orcid-id")]
            public string orcid_id { get; set; }
            [DataMember(Name = "given-names")]
            public string given_names { get; set; }
            [DataMember(Name = "family-names")]
            public string family_names { get; set; }
        }
    }

    [DataContract]
    public class ORCIDPerson
    {
        public ORCIDPerson()
        {

        }

        [DataMember(Name = "external-identifiers")]
        public ExternalIdentifiers external_identifiers { get; set; }

        [DataContract]
        public class ExternalIdentifiers
        {
            public ExternalIdentifiers()
            {
            }
            [DataMember(Name = "external-identifier")]
            public List<ExternalIdentifier> external_identifier { get; set; }

            [DataContract]
            public class ExternalIdentifier
            {
                public ExternalIdentifier()
                {
                }
                [DataMember(Name = "external-id-type")]
                public string external_id_type { get; set; }
                [DataMember(Name = "external-id-value")]
                public string external_id_value { get; set; }
            }
        }
    }

    [DataContract]
    public class ORCIDWorks
    {
        public ORCIDWorks()
        {

        }
        [DataMember(Name = "group")]
        public List<Group> group { get; set; }

        [DataContract]
        public class Group
        {
            public Group()
            {
            }
            [DataMember(Name = "work-summary")]
            public List<WorkSummary> work_summary { get; set; }

            [DataContract]
            public class WorkSummary
            {
                public WorkSummary()
                {
                }
                [DataMember(Name = "title")]
                public Title title { get; set; }

                [DataContract]
                public class Title
                {
                    public Title()
                    {
                    }
                    [DataMember(Name = "title")]
                    public Title2 title2 { get; set; }

                    [DataContract]
                    public class Title2
                    {
                        public Title2()
                        {
                        }
                        [DataMember(Name = "value")]
                        public string value { get; set; }
                    }
                }
            }
        }
    }
}
