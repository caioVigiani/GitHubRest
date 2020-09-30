using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiGitHub.Models
{
    public class HistoricoFavoritos
    {
        // id do usuário com list de id dos repositorios favoritados
        public List<UsuarioHistoricoFavorito> listaUsuarios { get; set; }
    }
}