﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Hercules_SAML
{
    [Authorize]
    public class ClaimsModel : PageModel
    {
        private readonly ILogger<ClaimsModel> _logger;

        public ClaimsModel(ILogger<ClaimsModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}