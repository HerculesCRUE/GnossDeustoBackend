using ApiCargaWebInterface.Models.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services.VirtualPathProvider
{
    /// <summary>
    /// Comprueba si la página ha sido cambiada para reemplazarla por la versión de caché
    /// </summary>
    public class ApiChangeToken : IChangeToken
    {
        readonly private CallApiVirtualPath _apiVirtualPath;
        readonly private string _viewPath;
        private static Dictionary<string, DateTime?> _pageLastRequested = new Dictionary<string, DateTime?>();

        public ApiChangeToken(CallApiVirtualPath apiVirtualPath, string viewPath)
        {
            _apiVirtualPath = apiVirtualPath;
            _viewPath = viewPath;
        }
        public bool ActiveChangeCallbacks => false;

        public bool HasChanged
        {
            get
            {
                try
                {

                    if (!_viewPath.EndsWith(".cshtml") || _viewPath.Contains("Views/Shared/_menupersonalizado.cshtml"))
                    {
                        PageInfo page = _apiVirtualPath.GetPage(_viewPath);
                        if (page != null)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            return EmptyDisposable.Instance;
        }
    }
    internal class EmptyDisposable : IDisposable
    {
        public static EmptyDisposable Instance { get; } = new EmptyDisposable();
        private EmptyDisposable() { }
        public void Dispose() 
        {
            // No hace nada.
        }
    }
}
