using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;

using Newtonsoft.Json;

using ApiGitHub.Models;


namespace ApiGitHub.Services
{
    public class FileService
    {
        private string filePath;

        public FileService(string serverPath)
        {
            filePath = serverPath + "/historico.json";
            if (!System.IO.File.Exists(filePath)) {
                HistoricoFavoritos conteudo = new HistoricoFavoritos()
                {
                    listaUsuarios = new List<UsuarioHistoricoFavorito>()
                };

                criaArquivoJsonHistorico(conteudo);
            }
        }

        public void criaArquivoJsonHistorico(HistoricoFavoritos conteudo)
        {
            using (StreamWriter file = System.IO.File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, conteudo);
            }
        }

        public void incluiUsuarioRepositorioFavorito(string loginUsuario, string repositorioFavorito)
        {
            HistoricoFavoritos novoConteudo = retornaHistoricoCompleto();

            if (novoConteudo != null && novoConteudo.listaUsuarios != null)
            {
                UsuarioHistoricoFavorito usarioHistoricoExistente = novoConteudo.listaUsuarios.Where(usuario => usuario.loginUsuario == loginUsuario).FirstOrDefault<UsuarioHistoricoFavorito>();

                if (usarioHistoricoExistente != null)
                {
                    // já existe histórico do usuário no json, só altera lista de repositorios
                    if (!usarioHistoricoExistente.listRepositorios.Contains(repositorioFavorito))
                        usarioHistoricoExistente.listRepositorios.Add(repositorioFavorito);
                }
                else
                {
                    // não existe histórico do usuário no json, cria novo usuário
                    UsuarioHistoricoFavorito novoUsuario = new UsuarioHistoricoFavorito()
                    {
                        loginUsuario = loginUsuario,
                        listRepositorios = new List<string>() { { repositorioFavorito } }
                    };

                    novoConteudo.listaUsuarios.Add(novoUsuario);
                }
            }
            else
            {
                novoConteudo = new HistoricoFavoritos();
                novoConteudo.listaUsuarios = new List<UsuarioHistoricoFavorito>();
                // não existe arquivo json
                UsuarioHistoricoFavorito novoUsuario = new UsuarioHistoricoFavorito()
                {
                    loginUsuario = loginUsuario,
                    listRepositorios = new List<string>() { { repositorioFavorito } }
                };

                novoConteudo.listaUsuarios.Add(novoUsuario);
            }

            criaArquivoJsonHistorico(novoConteudo);
        }

        public void excluiUsuarioRepositorioFavorito(string loginUsuario, string repositorioFavorito)
        {
            HistoricoFavoritos novoConteudo = retornaHistoricoCompleto();

            if (novoConteudo != null)
            {
                UsuarioHistoricoFavorito usuarioHistoricoExistente = novoConteudo.listaUsuarios.Where(usuario => usuario.loginUsuario == loginUsuario).FirstOrDefault<UsuarioHistoricoFavorito>();

                if (usuarioHistoricoExistente != null)
                {
                    // já existe histórico do usuário no json, só altera lista de repositorios
                    usuarioHistoricoExistente.listRepositorios.Remove(repositorioFavorito);
                }
            }

            criaArquivoJsonHistorico(novoConteudo);
        }

        public HistoricoFavoritos retornaHistoricoCompleto()
        {
            HistoricoFavoritos historicoFavoritos = new HistoricoFavoritos();

            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                historicoFavoritos = JsonConvert.DeserializeObject<HistoricoFavoritos>(json);
            }

            if (historicoFavoritos == null)
            {
                historicoFavoritos = new HistoricoFavoritos();
                historicoFavoritos.listaUsuarios = new List<UsuarioHistoricoFavorito>();
            }

            return historicoFavoritos;
        }

        public UsuarioHistoricoFavorito[] retornaHistoricoUsuario(string loginUsuario)
        {
            HistoricoFavoritos historicoFavoritos = retornaHistoricoCompleto();

            if (historicoFavoritos.listaUsuarios == null)
                return null;

            return historicoFavoritos.listaUsuarios.Where(usuario => usuario.loginUsuario == loginUsuario).ToArray();
        }
    }
}