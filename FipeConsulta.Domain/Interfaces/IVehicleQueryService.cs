using FipeConsulta.Domain.Entities;  // Adicionar o "using" para referenciar a classe VehicleQuery
using System.Threading.Tasks;

namespace FipeConsulta.Domain.Interfaces
{
    public interface IVehicleQueryService
    {
        Task<string> QueryValueAsync(VehicleQuery query);  // Utiliza a classe VehicleQuery
    }
}
