using System;
using System.Threading.Tasks;

namespace FipeConsulta.Application.Handlers
{
    public class DelayHandler
    {
        private readonly TimeSpan _delay;

        public DelayHandler(TimeSpan delay)
        {
            _delay = delay;
        }

        public async Task ApplyDelayAsync()
        {
            // Aplica o delay conforme o tempo configurado (por exemplo, 1 minuto)
            await Task.Delay(_delay);
        }
    }
}
