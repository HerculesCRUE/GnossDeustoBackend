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
        readonly private CallApiVirtualPath _callVirtualPath;
        public ApiFileProvider(CallApiVirtualPath callVirtualPath)
        {
            Log.Error($"ApiFileProvider constructor");
            _callVirtualPath = callVirtualPath;            
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return null;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var result = new ApiFileInfo(_callVirtualPath, subpath);
            return result.Exists ? result as IFileInfo : new NotFoundFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            var apiChangeToken = new ApiChangeToken(_callVirtualPath, filter);
            return apiChangeToken;
        }
    }
}
