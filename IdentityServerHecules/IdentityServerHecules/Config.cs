using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerHecules
{
    public class Config
    {
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
        {
            new Client
            {
                ClientId = "client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                AllowedScopes = {"api1"},
                AccessTokenLifetime = 86400
                //AccessTokenType = AccessTokenType.Reference
            },
            new Client
            {
                ClientId = "carga",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                AllowedScopes = {"apiCarga"},
                AccessTokenLifetime = 86400
                //AccessTokenType = AccessTokenType.Reference
            },
            new Client
            {
                ClientId = "cron",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("secretCron".Sha256())
                },
                AllowedScopes = {"apiCron"},
                AccessTokenLifetime = 86400
                //AccessTokenType = AccessTokenType.Reference
            }
            ,
            new Client
            {
                ClientId = "Web",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("master".Sha256())
                },
                AllowedScopes = {"apiCron", "apiCarga","apiUrisFactory"},
                AccessTokenLifetime = 86400
                //AccessTokenType = AccessTokenType.Reference
            },
            new Client
            {
                ClientId = "urisFactory",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("secretUris".Sha256())
                },
                AllowedScopes = { "apiUrisFactory"},
                AccessTokenLifetime = 86400
                //AccessTokenType = AccessTokenType.Reference
            }
            ,
            new Client
            {
                ClientId = "OAIPMH",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("secretOAIPMH".Sha256())
                },
                AllowedScopes = { "apiOAIPMH"},
                AccessTokenLifetime = 86400
                //AccessTokenType = AccessTokenType.Reference
            }
        };
        }

        // APIs allowed to access the Auth server
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
        {
            new ApiResource("api1", "My Api"),new ApiResource("apiCarga", "My ApiCarga"),new ApiResource("apiCron", "My ApiCron"),new ApiResource("apiUrisFactory", "My ApiUrisFactory"),new ApiResource("apiOAIPMH", "My apiOAIPMH")
        };
        }
    }
}
