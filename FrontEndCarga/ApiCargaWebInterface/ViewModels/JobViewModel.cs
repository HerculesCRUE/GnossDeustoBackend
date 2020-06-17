﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.ViewModels
{
    public class JobViewModel
    {
        public string Job { get; set; }
        public string State { get; set; }
        public string Id { get; set; }

        public string ExceptionDetails { get; set; }
        public DateTime? ExecutedAt { get; set; }
    }
}