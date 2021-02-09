// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para la configuración de las apis securizadas
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerHecules
{
    /// <summary>
    /// Clase para la configuración de las apis securizadas
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Obtiene los clientes configurados
        /// </summary>
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
                AllowedScopes = {"apiCron", "apiCarga","apiUrisFactory","apiGestorDocumentacion"},
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
            ,
            new Client
            {
                ClientId = "unidata",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("secretUnidata".Sha256())
                },
                AllowedScopes = { "apiUnidata"},
                AccessTokenLifetime = 86400
                //AccessTokenType = AccessTokenType.Reference
            }
             ,
            new Client
            {
                ClientId = "GestorDocumentacion",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("secretGestorDocumentacion".Sha256())
                },
                AllowedScopes = { "apiGestorDocumentacion"},
                AccessTokenLifetime = 86400
                //AccessTokenType = AccessTokenType.Reference
            },
            new Client
            {
                ClientId = "Discover",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("secretDiscover".Sha256())
                },
                AllowedScopes = {"apiCron", "apiCarga"},
                AccessTokenLifetime = 86400
                //AccessTokenType = AccessTokenType.Reference
            },
            new Client
            {
                ClientId = "LinkedDataServer",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("secretLinkedDataServer".Sha256())
                },
                AllowedScopes = {"apiCarga"},
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
            new ApiResource("api1", "My Api"),new ApiResource("apiCarga", "My ApiCarga"),new ApiResource("apiCron", "My ApiCron"),new ApiResource("apiUrisFactory", "My ApiUrisFactory"),new ApiResource("apiOAIPMH", "My apiOAIPMH"),new ApiResource("apiUnidata", "My apiUnidata"),new ApiResource("apiGestorDocumentacion", "My apiDocumentacion")
        };
        }
    }
}
