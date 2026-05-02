using System.Net;
using System.Text.Json;

using Xunit;
using ContaGotas;
using Xunit.Abstractions;

namespace ContaGotas.Tests
{
    public class CombustivelApiServiceTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public CombustivelApiServiceTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public class FakeDgegHandler : HttpMessageHandler
        {
            private readonly string _json;
            public FakeDgegHandler(string json) => _json = json;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
            {
                return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(_json)
                });
            }
        }
        
        [Fact]
        public async Task ChamarDGEG_DeveRetornarLista_QuandoJsonForValidoESucessoTrue()
        {
            // Arrange
            var service = new CombustivelApiService();

            // Act 
            // Nota: Como o teu código atual aponta para um URL fixo da Microsoft no GetAsync,
            // este teste num cenário real tentaria aceder à net. 
            // Abaixo mostro como testar a lógica de parsing isolada se o método fosse protegido.
            
            List<TipoCombustivel>? resultado = await service.chamarDGEG<List<TipoCombustivel>>("https://precoscombustiveis.dgeg.gov.pt/api/PrecoComb/GetTiposCombustiveis");
            foreach (var item in resultado)
            {
                _testOutputHelper.WriteLine(item.Nome);
                _testOutputHelper.WriteLine(item.Id.ToString());
                
            }
                
            // Assert
            Assert.NotNull(resultado);
            Assert.NotEmpty(resultado);
        
        }

        [Fact]
        public async Task ChamarDGEG_DeveRetornarListaT_QuandoSemErros()
        {
            // Arrange
            var service = new CombustivelApiService();
            var jsonErro = "{\"status\":true,\"mensagem\":\"sucesso\",\"resultado\":[{" +
                           "\"Id\":65553,\"Nome\":\"J. Pimenta & Filhos, Lda.\",\"TipoPosto\":\"Outro\",\"Municipio\":\"Ponte de Lima\",\"Preco\":\"0,929 €\"," +
                           "\"Marca\":\"GALP\",\"Combustivel\":\"GPL Auto\",\"DataAtualizacao\":\"2026-01-19 02:25\",\"Distrito\":\"Viana do Castelo\"," +
                           "\"Morada\":\"EN 201 Km 36,46 - Sernados\",\"Localidade\":\"Feitosa\",\"CodPostal\":\"4990-351\",\"Latitude\":41.75301," +
                           "\"Longitude\":-8.58063,\"Quantidade\":84}]}";

        var handler = new FakeDgegHandler(jsonErro);
            service._client = new HttpClient(handler);
            
            // Act
            List<Posto> resultado = await service.chamarDGEG<List<Posto>>("https://precoscombustiveis.dgeg.gov.pt/api/PrecoComb/PesquisarPostos?idsTiposComb=3201%2C3205&idMarca=29&idTipoPosto=3&idDistrito=13&idsMunicipios=198,194");
            

            // Assert
        
            Assert.NotEmpty(resultado);

            foreach (var VARIABLE in resultado)
            {
                _testOutputHelper.WriteLine(VARIABLE.PrecoString.ToString());
            }
        }
        
        [Fact]
        public async Task ChamarDGEG_DeveRetornarListaT_CampoObrigatorioEmFalta()
        {
            // Arrange
            var service = new CombustivelApiService();
            var jsonErro = "{\"status\":true,\"mensagem\":\"sucesso\",\"resultado\":[{" +
                           "\"Id\":65553,\"Nome\":\"J. Pimenta & Filhos, Lda.\",\"TipoPosto\":\"Outro\",\"Municipio\":\"Ponte de Lima\"," +
                           "\"Marca\":\"GALP\",\"Combustivel\":\"GPL Auto\",\"DataAtualizacao\":\"2026-01-19 02:25\",\"Distrito\":\"Viana do Castelo\"," +
                           "\"Morada\":\"EN 201 Km 36,46 - Sernados\",\"Localidade\":\"Feitosa\",\"CodPostal\":\"4990-351\",\"Latitude\":41.75301," +
                           "\"Longitude\":-8.58063,\"Quantidade\":84}]}";

            var handler = new FakeDgegHandler(jsonErro);
            service._client = new HttpClient(handler);
            
            // Act
            var ex = await Assert.ThrowsAsync<Exception>(() => 
                service.chamarDGEG<List<Posto>>("https://precoscombustiveis.dgeg.gov.pt/api/PrecoComb/PesquisarPostos?idsTiposComb=3201%2C3205&idMarca=29&idTipoPosto=3&idDistrito=13&idsMunicipios=198,194"));
            

            // Assert
            
            Assert.Contains("Algum campo critico no Json esta null ou em falta",ex.Message);
            
        }
        
        
        [Fact]
        public async Task ChamarDGEG_DeveRetornar_resultadoVazio()
        {
            // Arrange
            var service = new CombustivelApiService();
            var jsonErro = "{\"status\":true,\"mensagem\":\"sucesso\",\"resultado\":[]}";

            var handler = new FakeDgegHandler(jsonErro);
            service._client = new HttpClient(handler);
            
            // Act
            List<Posto> resultado = await service.chamarDGEG<List<Posto>>("https://precoscombustiveis.dgeg.gov.pt/");
            

            // Assert
        
            Assert.Null(resultado);
            
        }
        
        
        

        [Fact]
        public async Task ChamarDGEG_DeveLancarExcecao_AposMaxTentativasEmCasoDeErroHttp()
        {
            // Este teste validaria se o loop de 'tentativas' funciona.
            // Para isto ser testável, o HttpClient _client teria de ser Mockado.
            
            var service = new CombustivelApiService();

            // Esperamos que o erro customizado que escreveste no 'catch' seja lançado
            var ex = await Assert.ThrowsAsync<Exception>(() => 
                service.chamarDGEG<List<PrecoMedioModel>>("https://url-invalida-que-gera-erro.com"));
            
            Assert.Contains("Servidor da DGEG não encontrado", ex.Message);
        }
    }
}