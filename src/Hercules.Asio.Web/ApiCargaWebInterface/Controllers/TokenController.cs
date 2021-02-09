// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Controlador para obtener los tokens de acceso a los diferentes apis
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiCargaWebInterface.Models.Entities;
using ApiCargaWebInterface.Models.Services;
using ApiCargaWebInterface.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ApiCargaWebInterface.Controllers
{
    /// <summary>
    /// Controlador para obtener los tokens de acceso a los diferentes apis
    /// </summary>
    public class TokenController : Controller
    {
        CallTokenService _callTokenService;
        public TokenController(CallTokenService callTokenService)
        {
            _callTokenService = callTokenService;
        }
        /// <summary>
        /// devuelve la página principal con la lista de apis disponibles
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            GetTokenViewModel tokenViewModel = new GetTokenViewModel();
            tokenViewModel.TokenOptions = LoadTokenList();
            return View(tokenViewModel);
        }
        public IActionResult Prueba()
        {
            return View();
        }
        public IActionResult Graficas()
        {
            return View();
        }
        /// <summary>
        /// Obtiene un token para el api pasado
        /// </summary>
        /// <param name="token_Type">Api seleccionada para generar el token</param>
        /// <returns></returns>
        [HttpGet]
        [Route("[Controller]/get")]
        public IActionResult GetToken(int token_Type)
        {
            GetTokenViewModel tokenViewModel = new GetTokenViewModel();
            tokenViewModel.Token = "Token no disponible";
            TokenBearer token = null;
            if (token_Type.Equals((int)TokensEnum.TokenCarga))
            {
                token = _callTokenService.CallTokenCarga();
            }
            else if (token_Type.Equals((int)TokensEnum.TokenCron))
            {
                token = _callTokenService.CallTokenCron();
            }
            else if (token_Type.Equals((int)TokensEnum.TokenUrisFactory))
            {
                token = _callTokenService.CallTokenUrisFactory();
            }
            else if (token_Type.Equals((int)TokensEnum.TokenOAIPMH))
            {
                token = _callTokenService.CallTokenOAIPMH();
            }
            else if (token_Type.Equals((int)TokensEnum.TokenDocumentacion))
            {
                token = _callTokenService.CallTokenApiDocumentacion();
            }
            if (token != null)
            {
                tokenViewModel.Token = $"{token.token_type} {token.access_token}";
            }
            tokenViewModel.TokenOptions = LoadTokenList();
            return View("Index",tokenViewModel);
        }

        /// <summary>
        /// Carga la lista de apis disponibles
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, string> LoadTokenList()
        {
            Dictionary<int, string> tokensList = new Dictionary<int, string>();
            tokensList.Add((int)TokensEnum.TokenCarga, "token de api carga");
            tokensList.Add((int)TokensEnum.TokenUrisFactory, "token de uris factory");
            tokensList.Add((int)TokensEnum.TokenCron, "token de cron configure");
            tokensList.Add((int)TokensEnum.TokenOAIPMH, "token de api OAIPMH");
            tokensList.Add((int)TokensEnum.TokenDocumentacion, "token de api documentacion");
            return tokensList;
        }
    }
}