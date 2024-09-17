using FipeConsulta.Application.Services;
using FipeConsulta.Domain.Entities;
using FipeConsulta.Domain.Interfaces;
using FipeConsulta.Domain.Rules;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;

namespace FipeConsulta.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FipeConsultaController : ControllerBase
    {
        private readonly IVehicleQueryService _vehicleQueryService;
        private readonly IVehicleQueryProcessor _vehicleQueryProcessor;

        public FipeConsultaController(
            IVehicleQueryService vehicleQueryService,
            IVehicleQueryProcessor vehicleQueryProcessor)
        {
            _vehicleQueryService = vehicleQueryService;
            _vehicleQueryProcessor = vehicleQueryProcessor;
        }

        [HttpPost("consultar")]
        public async Task<IActionResult> Consultar([FromBody] VehicleQuery query)
        {
            if (query == null)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "O corpo da requisição está vazio ou contém dados inválidos.",
                    data = (object)null
                });
            }

            // Validar se todos os campos necessários estão presentes
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Os dados fornecidos estão incorretos. Verifique os campos e tente novamente.",
                    errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(k => k.Key, k => k.Value.Errors.Select(e => e.ErrorMessage)),
                    data = (object)null
                });
            }

            try
            {
                // Verificar se a consulta já foi realizada
                var uniqueQueryRule = new UniqueQueryRule(_vehicleQueryProcessor);
                if (!uniqueQueryRule.IsNewQuery(query))
                {
                    // Se a consulta já foi realizada, retorna o resultado existente
                    var result = await _vehicleQueryProcessor.GetSavedQueryResultAsync(query);
                    return Ok(new
                    {
                        status = "success",
                        message = "Consulta realizada anteriormente. Retornando resultado salvo.",
                        data = JsonSerializer.Deserialize<object>(result)
                    });
                }

                // Executar a consulta no serviço
                var newResult = await _vehicleQueryService.QueryValueAsync(query);

                // Verificar se o resultado da API é válido (não contém erro)
                if (IsValidResult(newResult))
                {
                    // Salvar o resultado da nova consulta se for válido
                    await _vehicleQueryProcessor.SaveQueryResultAsync(query, newResult);
                    return Ok(new
                    {
                        status = "success",
                        message = "Consulta realizada com sucesso.",
                        data = JsonSerializer.Deserialize<object>(newResult)
                    });
                }

                // Se o resultado não for válido, retorna uma mensagem amigável de erro
                return BadRequest(new
                {
                    status = "error",
                    message = "Nenhum resultado foi encontrado para os parâmetros fornecidos. Por favor, tente com outros valores.",
                    data = (object)null
                });
            }
            catch (JsonException jsonEx)
            {
                // Tratamento de erros de conversão de JSON
                return BadRequest(new
                {
                    status = "error",
                    message = "Erro ao processar a requisição. Verifique os dados JSON fornecidos.",
                    details = jsonEx.Message,
                    data = (object)null
                });
            }
        }

        // Função para verificar se o resultado da API FIPE é válido
        private bool IsValidResult(string result)
        {
            try
            {
                // Tenta deserializar o resultado para verificar se contém o campo "Valor" (ou outro campo necessário)
                var jsonDoc = JsonDocument.Parse(result);

                // Verifica se o campo "Valor" existe e não está vazio
                return jsonDoc.RootElement.TryGetProperty("Valor", out var valor) && !string.IsNullOrEmpty(valor.GetString());
            }
            catch
            {
                // Se ocorrer qualquer exceção ao processar o JSON, considera o resultado inválido
                return false;
            }
        }
    }
}
