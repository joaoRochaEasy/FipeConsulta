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
                    message = "O corpo da requisi��o est� vazio ou cont�m dados inv�lidos.",
                    data = (object)null
                });
            }

            // Validar se todos os campos necess�rios est�o presentes
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Os dados fornecidos est�o incorretos. Verifique os campos e tente novamente.",
                    errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(k => k.Key, k => k.Value.Errors.Select(e => e.ErrorMessage)),
                    data = (object)null
                });
            }

            try
            {
                // Verificar se a consulta j� foi realizada
                var uniqueQueryRule = new UniqueQueryRule(_vehicleQueryProcessor);
                if (!uniqueQueryRule.IsNewQuery(query))
                {
                    // Se a consulta j� foi realizada, retorna o resultado existente
                    var result = await _vehicleQueryProcessor.GetSavedQueryResultAsync(query);
                    return Ok(new
                    {
                        status = "success",
                        message = "Consulta realizada anteriormente. Retornando resultado salvo.",
                        data = JsonSerializer.Deserialize<object>(result)
                    });
                }

                // Executar a consulta no servi�o
                var newResult = await _vehicleQueryService.QueryValueAsync(query);

                // Verificar se o resultado da API � v�lido (n�o cont�m erro)
                if (IsValidResult(newResult))
                {
                    // Salvar o resultado da nova consulta se for v�lido
                    await _vehicleQueryProcessor.SaveQueryResultAsync(query, newResult);
                    return Ok(new
                    {
                        status = "success",
                        message = "Consulta realizada com sucesso.",
                        data = JsonSerializer.Deserialize<object>(newResult)
                    });
                }

                // Se o resultado n�o for v�lido, retorna uma mensagem amig�vel de erro
                return BadRequest(new
                {
                    status = "error",
                    message = "Nenhum resultado foi encontrado para os par�metros fornecidos. Por favor, tente com outros valores.",
                    data = (object)null
                });
            }
            catch (JsonException jsonEx)
            {
                // Tratamento de erros de convers�o de JSON
                return BadRequest(new
                {
                    status = "error",
                    message = "Erro ao processar a requisi��o. Verifique os dados JSON fornecidos.",
                    details = jsonEx.Message,
                    data = (object)null
                });
            }
        }

        // Fun��o para verificar se o resultado da API FIPE � v�lido
        private bool IsValidResult(string result)
        {
            try
            {
                // Tenta deserializar o resultado para verificar se cont�m o campo "Valor" (ou outro campo necess�rio)
                var jsonDoc = JsonDocument.Parse(result);

                // Verifica se o campo "Valor" existe e n�o est� vazio
                return jsonDoc.RootElement.TryGetProperty("Valor", out var valor) && !string.IsNullOrEmpty(valor.GetString());
            }
            catch
            {
                // Se ocorrer qualquer exce��o ao processar o JSON, considera o resultado inv�lido
                return false;
            }
        }
    }
}
