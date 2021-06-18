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
                //ClientId es el parámetro del servicio que solicita acceso a través de 'client_id'
                //ClientSecrets es el parámetro del servicio que solicita acceso a través de 'client_secret'
                //AllowedScopes son los scopes a los que se le da acceso al servicio
            new Client
            {
                ClientId = "carga",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                AllowedScopes = {"apiCarga","apiConversor","apiOAIPMH"},
                AccessTokenLifetime = 86400
            },
            new Client
            {
                ClientId = "cron",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("secretCron".Sha256())
                },
                AllowedScopes = {"apiCron","apiCarga"},
                AccessTokenLifetime = 86400
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
                AllowedScopes = {"apiCron", "apiCarga", "apiUrisFactory", "apiGestorDocumentacion","apiConversor","apiOAIPMH"},
                AccessTokenLifetime = 86400
            },
            new Client
            {
                ClientId = "urisFactory",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("secretUris".Sha256())
                },
                AccessTokenLifetime = 86400
            },
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
            },
            new Client
            {
                ClientId = "Discover",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("secretDiscover".Sha256())
                },
                AllowedScopes = {"apiCron", "apiCarga","apiUrisFactory"},
                AccessTokenLifetime = 86400
            },
            new Client
            {
                ClientId = "conversor",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("secretConversor".Sha256())
                },
                AllowedScopes = { "apiUrisFactory"},
                AccessTokenLifetime = 86400,
            }
        };
        }

        // APIs allowed to access the Auth server
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
        {
            new ApiResource("apiCarga", "API de Carga"),new ApiResource("apiCron", "API Cron configure"),new ApiResource("apiUrisFactory", "API UrisFactory"),new ApiResource("apiOAIPMH", "API OAIPMH cvn"),new ApiResource("apiGestorDocumentacion", "API GEstor de documentación"),new ApiResource("apiConversor", "API conversor XML RDF")
        };
        }
    }
}
