using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    public class ConfigTokenService
    {
        public IConfigurationRoot Configuration { get; set; }
        public string Authority { get; set; }
        public string GrantType { get; set; }
        public string Scope { get; set; }
        public string ScopeOAIPMH { get; set; }
        public string ScopeCron { get; set; }
        public string ClientId { get; set; }
        public string ClientIdOAIPMH { get; set; }
        public string ClientSecret { get; set; }
        public string ClientSecretOAIPMH { get; set; }
        
        public ConfigTokenService()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }
        public string GetAuthorityGetToken()
        {
            if (string.IsNullOrEmpty(Authority))
            {
                string authority = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("AuthorityGetToken"))
                {
                    authority = environmentVariables["AuthorityGetToken"] as string;
                }
                else
                {
                    authority = Configuration["AuthorityGetToken"];
                }

                Authority = authority;
            }
            return Authority;
        }

        public string GetGrantType()
        {
            if (string.IsNullOrEmpty(GrantType))
            {
                string grantType = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("GrantType"))
                {
                    grantType = environmentVariables["GrantType"] as string;
                }
                else
                {
                    grantType = Configuration["GrantType"];
                }

                GrantType = grantType;
            }
            return GrantType;
        }

        public string GetScopeCarga()
        {
            if (string.IsNullOrEmpty(Scope))
            {
                string scope = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ScopeCarga"))
                {
                    scope = environmentVariables["ScopeCarga"] as string;
                }
                else
                {
                    scope = Configuration["ScopeCarga"];
                }

                Scope = scope;
            }
            return Scope;
        }

        public string GetClientIdCarga()
        {
            if (string.IsNullOrEmpty(ClientId))
            {
                string clientId = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ClientId"))
                {
                    clientId = environmentVariables["ClientId"] as string;
                }
                else
                {
                    clientId = Configuration["ClientId"];
                }

                ClientId = clientId;
            }
            return ClientId;
        }

        public string GetClientSecretCarga()
        {
            if (string.IsNullOrEmpty(ClientSecret))
            {
                string clientSecret = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ClientSecret"))
                {
                    clientSecret = environmentVariables["ClientSecret"] as string;
                }
                else
                {
                    clientSecret = Configuration["ClientSecret"];
                }

                ClientSecret = clientSecret;
            }
            return ClientSecret;
        }

        public string GetScopeOAIPMH()
        {
            if (string.IsNullOrEmpty(ScopeOAIPMH))
            {
                string scope = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ScopeOAIPMH"))
                {
                    scope = environmentVariables["ScopeOAIPMH"] as string;
                }
                else
                {
                    scope = Configuration["ScopeOAIPMH"];
                }

                ScopeOAIPMH = scope;
            }
            return ScopeOAIPMH;
        }

        public string GetClientSecretOAIPMH()
        {
            if (string.IsNullOrEmpty(ClientSecretOAIPMH))
            {
                string clientSecret = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ClientSecretOAIPMH"))
                {
                    clientSecret = environmentVariables["ClientSecretOAIPMH"] as string;
                }
                else
                {
                    clientSecret = Configuration["ClientSecretOAIPMH"];
                }

                ClientSecretOAIPMH = clientSecret;
            }
            return ClientSecretOAIPMH;
        }

        public string GetClientIdOAIPMH()
        {
            if (string.IsNullOrEmpty(ClientIdOAIPMH))
            {
                string clientId = "";
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ClientIdOAIPMH"))
                {
                    clientId = environmentVariables["ClientIdOAIPMH"] as string;
                }
                else
                {
                    clientId = Configuration["ClientIdOAIPMH"];
                }

                ClientIdOAIPMH = clientId;
            }
            return ClientIdOAIPMH;
        }
    }
}
