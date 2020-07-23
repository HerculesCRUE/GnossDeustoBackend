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
    public class TokenController : Controller
    {
        CallTokenService _callTokenService;
        public TokenController(CallTokenService callTokenService)
        {
            _callTokenService = callTokenService;
        }
        public IActionResult Index()
        {
            GetTokenViewModel tokenViewModel = new GetTokenViewModel();
            tokenViewModel.TokenOptions = LoadTokenList();
            return View(tokenViewModel);
        }

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
            if (token != null)
            {
                tokenViewModel.Token = $"{token.token_type} {token.access_token}";
            }
            tokenViewModel.TokenOptions = LoadTokenList();
            return View("Index",tokenViewModel);
        }

        private Dictionary<TokensEnum, string> LoadTokenList()
        {
            Dictionary<TokensEnum, string> tokensList = new Dictionary<TokensEnum, string>();
            tokensList.Add(TokensEnum.TokenCarga, "token de api carga");
            tokensList.Add(TokensEnum.TokenUrisFactory, "token de uris factory");
            tokensList.Add(TokensEnum.TokenCron, "token de cron configure");
            tokensList.Add(TokensEnum.TokenOAIPMH, "token de api OAIPMH");
            return tokensList;
        }
    }
}