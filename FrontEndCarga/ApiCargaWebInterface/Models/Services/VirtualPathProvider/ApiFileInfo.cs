using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
 
namespace ApiCargaWebInterface.Models.Services.VirtualPathProvider
{
    /// <summary>
    /// Clase para obtener un fichero de un api mediante su ruta virtual
    /// </summary>
    public class ApiFileInfo : IFileInfo
    {
        private string _viewPath;
        private byte[] _viewContent;
        private DateTimeOffset _lastModified;
        private bool _exists;
        private CallApiVirtualPath _apiVirtualPath;

        public ApiFileInfo(CallApiVirtualPath apiVirtualPath, string viewPath)
        {
            _viewPath = viewPath;
            _apiVirtualPath = apiVirtualPath;
            GetView(viewPath);
        }
        /// <summary>
        ///  True si existe el fichero en el api
        /// </summary>
        public bool Exists => _exists;
        /// <summary>
        /// True para el caso TryGetDirectoryContents tiene un sub-directory
        /// </summary>    
        public bool IsDirectory => false;
        /// <summary>
        /// La ultima vez que el fichero ha sido modificado
        /// </summary>
        public DateTimeOffset LastModified => _lastModified;
        /// <summary>
        /// El tamaño del fichero en bytes, -1 si es un directorio o no existe el fichero
        /// </summary>
        public long Length
        {
            get
            {
                using (var stream = new MemoryStream(_viewContent))
                {
                    return stream.Length;
                }
            }
        }
        /// <summary>
        /// El nombre del fichero
        /// </summary>
        public string Name => Path.GetFileName(_viewPath);
        /// <summary>
        /// El path del archivo, incluyendo el nombre. Devuelve null si no es accesible
        /// </summary>
        public string PhysicalPath => null;
        /// <summary>
        /// Devuelve el fichero como un stream de lectura. Se debería cerrar el stream una vez leído
        /// </summary>
        /// <returns>El stream del fichero</returns>
        public Stream CreateReadStream()
        {
            return new MemoryStream(_viewContent);
        }
        /// <summary>
        /// Obtiene la información del fichero
        /// </summary>
        /// <param name="viewPath">ruta virtual del fichero</param>
        private void GetView(string viewPath)
        {
            try
            {
                var page = _apiVirtualPath.GetPage(viewPath);
                
                if (page != null)
                {
                    _exists = true;
                    
                    _viewContent = Encoding.UTF8.GetBytes(page.Content);
                    _lastModified = page.LastModified;
                }
            }
            catch (Exception ex)
            {
                // if something went wrong, Exists will be false
            }
        }
    }
}
