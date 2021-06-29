using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
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
    public class ApiFileProvider : IFileProvider
    {
        public ApiFileProvider()
        {
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return null;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            Log.Error($"GetFileInfo PRE");
            CallApiVirtualPath _callVirtualPath = (CallApiVirtualPath)(new HttpContextAccessor().HttpContext.RequestServices.GetService(typeof(CallApiVirtualPath)));
            Log.Error($"GetFileInfo POST");
            var result = new ApiFileInfo(_callVirtualPath, subpath);
            return result.Exists ? result as IFileInfo : new NotFoundFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            Log.Error($"Watch PRE");
            CallApiVirtualPath _callVirtualPath = (CallApiVirtualPath)(new HttpContextAccessor().HttpContext.RequestServices.GetService(typeof(CallApiVirtualPath)));
            Log.Error($"Watch POST");
            var apiChangeToken = new ApiChangeToken(_callVirtualPath, filter);
            return apiChangeToken;
        }
    }
}
