using FipeConsulta.Domain.Entities;
using System.Threading.Tasks;

namespace FipeConsulta.Domain.Interfaces
{
    public interface IVehicleQueryProcessor
    {
        bool IsQueryAlreadyExecuted(VehicleQuery query);
        Task SaveQueryResultAsync(VehicleQuery query, string result);
        Task<string> GetSavedQueryResultAsync(VehicleQuery query);  // Retorna o resultado salvo, se existir
    }
}
