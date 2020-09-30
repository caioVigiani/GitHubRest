using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;

using RestSharp;

using Newtonsoft.Json;

using ApiGitHub.Models;
using ApiGitHub.Services;
using ApiGitHub.Helpers;

namespace ApiGitHub.Controllers
{
    public class HomeController : Controller
    {
        private ApiGitHubService apiGitHubService = new ApiGitHubService();
        private RepositorioHelper repositorioHelper = new RepositorioHelper();
        private FileService fileService = new FileService(HttpRuntime.AppDomainAppPath + "/App_Data/");

        public ActionResult MeusRepositorios(string usuarioDaBusca, string filtroRepositorio = "")
        {
            Usuario usuario = new Usuario();

            if (usuarioDaBusca != "" && usuarioDaBusca != null)
            {
                IRestResponse respostaUsuario = apiGitHubService.getUsuario(usuarioDaBusca);

                if ((int)respostaUsuario.StatusCode != 200)
                {
                    ViewBag.error = "Usuário " + usuarioDaBusca + " não encontrado!";
                }
                else
                {
                    usuario = JsonConvert.DeserializeObject<Usuario>(respostaUsuario.Content);

                    IRestResponse respostaRepositorio = apiGitHubService.getRepositorios(usuarioDaBusca);

                    if ((int)respostaRepositorio.StatusCode != 200)
                    {
                        ViewBag.error = "Não foram encontrados repositórios para o usuário " + usuarioDaBusca;
                    }
                    else
                    {
                        List<Repositorio> repositorios = JsonConvert.DeserializeObject<List<Repositorio>>(respostaRepositorio.Content);

                        repositorios = repositorioHelper.filtraListaRepositorios(filtroRepositorio, repositorios);
                        Dictionary<string, List<Repositorio>> repositoriosFormatados = formataRepositorios(usuario, repositorios);

                        ViewBag.repositorios = repositoriosFormatados;
                        ViewBag.filtro = filtroRepositorio;
                    }
                }
            }

            ViewBag.usuario = usuario;

            // mantendo o login GitHub do author quando busca vazia
            ViewBag.usuarioBuscado = (usuario.login != null ? usuario.login : "caioVigiani");

            return View();
        }

        public ActionResult MeusRepositoriosFavoritos()
        {
            // Inviavel filtro de favoritos, muitas requisições na API do GitHub bloqueia acesso momentaneamente

            HistoricoFavoritos historicoFavoritos = fileService.retornaHistoricoCompleto();

            List<Repositorio> repositorios = new List<Repositorio>();
            foreach (UsuarioHistoricoFavorito usuario in historicoFavoritos.listaUsuarios)
            {
                foreach(string repositorio in usuario.listRepositorios)
                {
                    IRestResponse respostaRepositorio = apiGitHubService.getRepositorio(usuario.loginUsuario, repositorio);
                    if ((int)respostaRepositorio.StatusCode == 200)
                    {
                        Repositorio repositorioDoHistorico = JsonConvert.DeserializeObject<Repositorio>(respostaRepositorio.Content);
                        repositorios.Add(repositorioDoHistorico);
                    }
                }
            }

            ViewBag.repositorios = repositorios;

            return View();
        }

        public ActionResult Usuario(string loginUsuario)
        {
            IRestResponse respostaUsuario = apiGitHubService.getUsuario(loginUsuario);
            if ((int)respostaUsuario.StatusCode == 200)
            {
                Usuario usuario = JsonConvert.DeserializeObject<Usuario>(respostaUsuario.Content);
                ViewBag.usuario = usuario;
            }
            else
            {
                ViewBag.error = "Usuário não encontrado";
            }

            return View();
        }

        public ActionResult Repositorio(string loginUsuario, string nomeRepositorio)
        {
            IRestResponse respostaRepositorio = apiGitHubService.getRepositorio(loginUsuario, nomeRepositorio);
            if ((int)respostaRepositorio.StatusCode == 200)
            {
                Repositorio repositorio = JsonConvert.DeserializeObject<Repositorio>(respostaRepositorio.Content);

                IRestResponse respostaColaboradores = apiGitHubService.getContribuidores(loginUsuario, nomeRepositorio);
                if ((int)respostaColaboradores.StatusCode == 200)
                {
                    List<Contribuidor> contribuidores = JsonConvert.DeserializeObject<List<Contribuidor>>(respostaColaboradores.Content);
                    repositorio.contribuidores = contribuidores;
                }

                ViewBag.repositorio = repositorio;
            }
            else
            {
                ViewBag.error = "Repositório não encontrado";
            }

            return View();
        }

        public Dictionary<string, List<Repositorio>> formataRepositorios(Usuario usuario, List<Repositorio> repositorios)
        {
            IEnumerable<UsuarioHistoricoFavorito> usuarioRepositoriosFavoritos = fileService.retornaHistoricoUsuario(usuario.login);

            Dictionary<string, List<Repositorio>> repositoriosFormatados = repositorioHelper.formataRepositorios(repositorios, usuarioRepositoriosFavoritos);

            return repositoriosFormatados;
        }

        [HttpGet]
        public void alterarHistoricoRepositorios(string usuario, string repositorio, bool incluir)
        {
            if (incluir)
            {
                fileService.incluiUsuarioRepositorioFavorito(usuario, repositorio);
            }
            else
            {
                fileService.excluiUsuarioRepositorioFavorito(usuario, repositorio);
            }
        }
    }
}