using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services.VirtualPathProvider
{
    public class ApiFileProvider : IFileProvider
    {
        private CallApiVirtualPath _callVirtualPath;
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
            var result = new ApiFileInfo(_callVirtualPath, subpath);
            return result.Exists ? result as IFileInfo : new NotFoundFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            return new ApiChangeToken(_callVirtualPath, filter);
        }
    }
}
