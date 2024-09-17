using FipeConsulta.Domain.Entities;
using FipeConsulta.Domain.Interfaces;

namespace FipeConsulta.Domain.Rules
{
    public class UniqueQueryRule
    {
        private readonly IVehicleQueryProcessor _vehicleQueryProcessor;

        public UniqueQueryRule(IVehicleQueryProcessor vehicleQueryProcessor)
        {
            _vehicleQueryProcessor = vehicleQueryProcessor;
        }

        // Verifica se a consulta é nova ou já foi realizada
        public bool IsNewQuery(VehicleQuery query)
        {
            return !_vehicleQueryProcessor.IsQueryAlreadyExecuted(query);
        }
    }
}
