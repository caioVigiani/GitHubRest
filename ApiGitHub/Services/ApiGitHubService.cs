using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using RestSharp;

namespace ApiGitHub.Services
{
    public class ApiGitHubService
    {
        private string urlApi = "https://api.github.com/";

        public IRestResponse getUsuario(string usuario)
        {
            var client = new RestClient(urlApi + "users/" + usuario);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            return client.Execute(request);
        }

        public IRestResponse getRepositorios(string usuario)
        {
            var client = new RestClient(urlApi + "users/" + usuario + "/repos");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            return client.Execute(request);
        }

        public IRestResponse getRepositorio(string usuario, string repositorio)
        {
            var client = new RestClient(urlApi + "repos/" + usuario + "/" + repositorio);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            return client.Execute(request);
        }

        public IRestResponse getContribuidores(string usuario, string repositorio)
        {
            var client = new RestClient(urlApi + "repos/" + usuario + "/" + repositorio + "/collaborators");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            return client.Execute(request);
        }
    }
}