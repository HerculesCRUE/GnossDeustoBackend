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
        private CallApiVirtualPath _callVirtualPath;
        public ApiFileProvider(CallApiVirtualPath callVirtualPath)
        {
            //Stopwatch sw = new Stopwatch(); // Creación del Stopwatch.
            //sw.Start(); // Iniciar la medición.
            _callVirtualPath = callVirtualPath;
            
            //sw.Stop();
            //Log.Information($"ApiFileProvider : {sw.Elapsed.ToString("hh\\:mm\\:ss\\.fff")}\n");
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return null;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            //Stopwatch sw = new Stopwatch(); // Creación del Stopwatch.
            //sw.Start(); // Iniciar la medición.
            var result = new ApiFileInfo(_callVirtualPath, subpath);
            //sw.Stop();
            //Log.Information($"ApiFileProvider -> GetFileInfo {subpath} : {sw.Elapsed.ToString("hh\\:mm\\:ss\\.fff")}\n");
            return result.Exists ? result as IFileInfo : new NotFoundFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            //Stopwatch sw = new Stopwatch(); // Creación del Stopwatch.
            //sw.Start(); // Iniciar la medición.
            var apiChangeToken = new ApiChangeToken(_callVirtualPath, filter);
            //Log.Information($"ApiFileProvider -> Watch filter: {filter} : {sw.Elapsed.ToString("hh\\:mm\\:ss\\.fff")}\n");
            return apiChangeToken;

        }
    }
}
