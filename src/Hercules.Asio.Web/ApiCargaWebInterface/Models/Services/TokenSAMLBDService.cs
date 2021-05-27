// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Clase para gestionar las operaciones en base de datos de los repositorios 
using ApiCargaWebInterface.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ApiCargaWebInterface.Models.Services
{
    ///<summary>
    ///Clase para gestionar los token SAML
    ///</summary>
    public class TokenSAMLBDService
    {
        private readonly EntityContext _context;
        public TokenSAMLBDService(EntityContext context)
        {
            _context = context;
        }


        ///<summary>
        ///Obtiene un token
        ///</summary>
        ///<param name="id">Identificador del token</param>
        public TokenSAML GetTokenSAML(Guid id)
        {
            return _context.TokenSAML.FirstOrDefault(item => item.Token.Equals(id));
        }

        ///<summary>
        ///Elimina un token
        ///</summary>
        ///<param name="tokenSAML">Token SAML</param>
        public void RemoveTokenSAML(TokenSAML tokenSAML)
        {
            _context.Entry(tokenSAML).State = EntityState.Deleted;
            _context.SaveChanges();
        }
    }
}
