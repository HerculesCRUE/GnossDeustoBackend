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
            _callVirtualPath = callVirtualPath;            
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return null;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            Log.Error($"GetFileInfo PRE");
            var result = new ApiFileInfo(_callVirtualPath, subpath);
            Log.Error($"GetFileInfo POST");
            return result.Exists ? result as IFileInfo : new NotFoundFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            Log.Error($"Watch PRE");
            var apiChangeToken = new ApiChangeToken(_callVirtualPath, filter);
            Log.Error($"Watch POST");
            return apiChangeToken;
        }
    }
}
