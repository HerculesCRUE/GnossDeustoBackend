using Hercules_SAML.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hercules_SAML.Models.Services
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
        ///Añade un item token SAML
        ///</summary>
        ///<param name="id">Id</param>
        public void AddTokenSAML(Guid id)
        {
            TokenSAML tokenSAml = new TokenSAML();
            tokenSAml.Token = id;
            _context.TokenSAML.Add(tokenSAml);
            _context.SaveChanges();
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
