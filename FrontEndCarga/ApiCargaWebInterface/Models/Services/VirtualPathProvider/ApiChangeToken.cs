using ApiCargaWebInterface.Models.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services.VirtualPathProvider
{
    /// <summary>
    /// Comprueba si la página ha sido cambiada para reemplazarla por la versión de caché
    /// </summary>
    public class ApiChangeToken : IChangeToken
    {
        private CallApiVirtualPath _apiVirtualPath;
        private string _viewPath;

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
                    PageInfo page = _apiVirtualPath.GetPage(_viewPath);
                    if (page != null)
                    {
                        if (!page.LastRequested.HasValue)
                        {
                            return false;
                        }
                        else
                        {
                            return page.LastModified > page.LastRequested.Value;
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
        public void Dispose() { }
    }
}
