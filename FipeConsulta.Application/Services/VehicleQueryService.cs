using FipeConsulta.Domain.Entities;
using FipeConsulta.Domain.Interfaces;
using System.Text.Json;
using System.Text;

namespace FipeConsulta.Application.Services
{
    public class VehicleQueryService : IVehicleQueryService
    {
        private readonly HttpClient _httpClient;

        public VehicleQueryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> QueryValueAsync(VehicleQuery query)
        {
            var jsonContent = JsonSerializer.Serialize(query);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://veiculos.fipe.org.br/api/veiculos/ConsultarValorComTodosParametros", content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
