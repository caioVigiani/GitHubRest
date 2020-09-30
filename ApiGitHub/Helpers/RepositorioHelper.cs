using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ApiGitHub.Models;

using System.Text.RegularExpressions;

namespace ApiGitHub.Helpers
{
    public class RepositorioHelper
    {
        public List<Repositorio> filtraListaRepositorios(string filtroRepositorio, List<Repositorio> repositorios)
        {
            if (filtroRepositorio != "")
            {
                string pattern = string.Concat(@"(.*?)", filtroRepositorio, @"(.*?)");
                repositorios = repositorios.Where(repositorio => Regex.Match(repositorio.name, pattern, RegexOptions.IgnoreCase).Success).ToList();
            }

            return repositorios;
        }

        public Dictionary<string, List<Repositorio>> formataRepositorios(List<Repositorio> repositorios, IEnumerable<UsuarioHistoricoFavorito> usuarioRepositoriosFavoritos)
        {
            Dictionary<string, List<Repositorio>> repositoriosFormatados = new Dictionary<string, List<Repositorio>>()
            {
                { "comuns", new List<Repositorio>() },
                { "favoritos", new List<Repositorio>() },
            };

            if (usuarioRepositoriosFavoritos.Count() == 0)
            {
                repositoriosFormatados["comuns"] = repositorios;
            }
            else
            {
                foreach (var repositorio in repositorios)
                {
                    foreach (var usuarioRepositorio in usuarioRepositoriosFavoritos)
                    {
                        if (usuarioRepositorio.listRepositorios.Contains(repositorio.name))
                        {
                            repositoriosFormatados["favoritos"].Add(repositorio);
                        }
                        else
                        {
                            repositoriosFormatados["comuns"].Add(repositorio);
                        }
                    }
                }
            }

            return repositoriosFormatados;
        }
    }
}