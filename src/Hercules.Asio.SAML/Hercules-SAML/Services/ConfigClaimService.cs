using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hercules_SAML.Services
{
    public class ConfigClaimService
    {
        private string claim { get; set; }
        private string value { get; set; }
        private IConfiguration _configuration { get; set; }

        public ConfigClaimService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetClaim()
        {
            if (string.IsNullOrEmpty(claim))
            {
                string connectionString = string.Empty;
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("Claim"))
                {
                    connectionString = environmentVariables["Claim"] as string;
                }
                else
                {
                    connectionString = _configuration["Claim"];
                }

                claim = connectionString;
            }
            return claim;
        }

        public string GetValue()
        {
            if (string.IsNullOrEmpty(value))
            {
                string connectionString = string.Empty;
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("Value"))
                {
                    connectionString = environmentVariables["Value"] as string;
                }
                else
                {
                    connectionString = _configuration["Value"];
                }

                value = connectionString;
            }
            return value;
        }
    }
}
