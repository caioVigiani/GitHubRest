using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiGitHub.Models
{
    public class UsuarioHistoricoFavorito
    {
        public string loginUsuario { get; set; }
        public List<string> listRepositorios { get; set; }
    }
}