using FipeConsulta.Domain.Entities;
using FipeConsulta.Domain.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace FipeConsulta.Infrastructure.Repositories
{
    public class VehicleQueryRepository : IVehicleQueryProcessor
    {
        private readonly string _path = @"C:\workspace\FipeConsulta\QueryResults";

        // Verifica se a consulta já foi realizada (verifica se o arquivo existe)
        public bool IsQueryAlreadyExecuted(VehicleQuery query)
        {
            var filePath = Path.Combine(_path, query.GenerateFileName());
            return File.Exists(filePath);
        }

        // Salva o resultado da consulta em um arquivo
        public async Task SaveQueryResultAsync(VehicleQuery query, string result)
        {
            var filePath = Path.Combine(_path, query.GenerateFileName());
            Directory.CreateDirectory(_path);
            await File.WriteAllTextAsync(filePath, result);
        }

        // Retorna o resultado salvo de uma consulta já realizada
        public async Task<string> GetSavedQueryResultAsync(VehicleQuery query)
        {
            var filePath = Path.Combine(_path, query.GenerateFileName());
            return await File.ReadAllTextAsync(filePath);
        }
    }
}
