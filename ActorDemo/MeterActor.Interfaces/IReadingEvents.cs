using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterActor.Interfaces
{
    public interface IReadingEvents : IActorEvents
    {
        void ReadingAvailable(long deviceId, int reading);
    }
}
