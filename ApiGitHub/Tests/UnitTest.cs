using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Xunit;

using ApiGitHub.Models;
using ApiGitHub.Services;
using ApiGitHub.Helpers;

namespace ApiGitHub.Tests
{
    public class UnitTest
    {
        private ApiGitHubService apiGitHubService = new ApiGitHubService();
        private RepositorioHelper repositorioHelper = new RepositorioHelper();
        private FileService fileService = new FileService("./../App_Data/test");

        // Teste geral do arquivo de histório e seus dados
        // Criar, Incluir, Excluir e Retorna Específico
        [Fact]
        public void criaIncluiRecebeRepositorioArquivo()
        {
            fileService.criaArquivoJsonHistorico(new HistoricoFavoritos());

            /* 
             * Caso 1 - De um arquivo em branco:
             * seleciona um usuário especifico que não deve existir;
             * insere o registro no arquivo;
             * agora que existe, seleciona novamente
            */
            var testeNull1 = fileService.retornaHistoricoUsuario("testeUsuario");
            Assert.True(testeNull1 == null);

            fileService.incluiUsuarioRepositorioFavorito("testeUsuario", "testeRepositorio");

            var testeNotNull1 = fileService.retornaHistoricoUsuario("testeUsuario");
            Assert.True(testeNotNull1[0].listRepositorios.Count >= 1);

            /* 
             * Caso 2 - De um arquivo já existente:
             * seleciona um usuário especifico;
             * exclui o registro no arquivo;
             * agora que não existe, seleciona novamente
            */
            var testeNotNull2 = fileService.retornaHistoricoUsuario("testeUsuario");
            Assert.False(testeNotNull2 == null);

            fileService.excluiUsuarioRepositorioFavorito("testeUsuario", "testeRepositorio");

            var testeNull2 = fileService.retornaHistoricoUsuario("testeUsuario");
            Assert.True(testeNull2[0].listRepositorios.Count < 1);
        }

        // Teste parcial do arquivo de histório e seus dados
        // Criar, Incluir, Retorna Geral e Formata
        [Fact]
        public void formataRepositoriosFavoritosArquivo()
        {
            fileService.criaArquivoJsonHistorico(new HistoricoFavoritos());
            List<Repositorio> repositorios = new List<Repositorio>()
            {
                { new Repositorio{ name = "repos1"} },
                { new Repositorio{ name = "repos2"} },
                { new Repositorio{ name = "repos3"} },
            };

            /* 
             * Caso 1 - De um arquivo em branco:
             * seleciona um usuário especifico;
             * insere o registro no arquivo com algum repositorio favorito presente na lista de teste
             * formata repositorios favoritos;
            */
            var testeNull1 = fileService.retornaHistoricoUsuario("testeUsuario");
            Assert.True(testeNull1 == null);

            fileService.incluiUsuarioRepositorioFavorito("testeUsuario", "repos2");

            var testeNotNull1 = fileService.retornaHistoricoUsuario("testeUsuario");
            Assert.True(testeNotNull1[0].listRepositorios.Count >= 1);

            var testeFormatado = repositorioHelper.formataRepositorios(repositorios, testeNotNull1);
            Assert.True(testeFormatado["comuns"].Count == 2);
            Assert.True(testeFormatado["favoritos"].Count == 1);
        }

        // Teste de filtro de repositorios
        // Filtra
        [Fact]
        public void filtraRepositoriosArquivo()
        {
            fileService.criaArquivoJsonHistorico(new HistoricoFavoritos());
            List<Repositorio> repositorios = new List<Repositorio>()
            {
                { new Repositorio{ name = "reposabc"} },
                { new Repositorio{ name = "reposbcd"} },
            };

            /* 
             * Caso 1 - De uma lista de repositorios:
             * filtra com pattern presente no primeiro;
             * filtra com pattern presente no segundo;
             * filtra com pattern não presente;
            */
            var testeFiltrado1 = repositorioHelper.filtraListaRepositorios("abc", repositorios);
            Assert.True(testeFiltrado1.Count == 1);

            var testeFiltrado2 = repositorioHelper.filtraListaRepositorios("bcd", repositorios);
            Assert.True(testeFiltrado2.Count == 1);

            var testeFiltrado3 = repositorioHelper.filtraListaRepositorios("cde", repositorios);
            Assert.True(testeFiltrado3.Count == 0);
        }
    }
}