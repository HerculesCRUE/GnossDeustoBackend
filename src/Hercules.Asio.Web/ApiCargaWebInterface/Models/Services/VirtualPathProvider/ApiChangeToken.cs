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
                        if (!LastRequested(_viewPath).HasValue)
                        {
                            return false;
                        }
                        else
                        {
                            DateTime lastRequested = LastRequested(_viewPath).Value;
                            DateTime now = DateTime.Now;
                            var segundos = (now - lastRequested).TotalSeconds;
                            if (segundos>5)
                            {
                                PageInfo page = _apiVirtualPath.GetPage(_viewPath);
                                if (page != null)
                                {
                                    DateTime lastRequest = LastRequested(_viewPath, true).Value;
                                    bool changed = page.LastModified > lastRequest;
                                    return changed;
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

        private DateTime? LastRequested(string path, bool changeResquested = false)
        {
            DateTime? lastRequested = null;
            if (_pageLastRequested.ContainsKey(path))
            {
                lastRequested = _pageLastRequested[path];
                if (changeResquested)
                {
                    _pageLastRequested[path] = DateTime.Now;
                }
            }
            else
            {
                _pageLastRequested.Add(path, DateTime.Now);
            }
            return lastRequested;
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
