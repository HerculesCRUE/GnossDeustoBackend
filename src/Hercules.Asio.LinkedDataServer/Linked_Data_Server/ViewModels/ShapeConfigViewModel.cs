﻿using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Linked_Data_Server.ViewModels
{
    [ExcludeFromCodeCoverage]
    [Display(Name = "Shape config")]
    public class ShapeConfigViewModel
    {
        [Display(Name = "Identifier")]
        public Guid ShapeConfigID { get; set; }
        public string Name { get; set; }
        [Display(Name = "identificador del repositorio")]
        public Guid RepositoryID { get; set; }
        public string Shape { get; set; }
    }
}
