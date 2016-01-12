using MeterActor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    class ReadingEventHandler : IReadingEvents
    {
        public void ReadingAvailable(long deviceId, int reading, Guid partitionId)
        {
            Console.WriteLine("{0:HH:mm:ss} Reading available for device {1}: {2} ({3})",
                DateTime.Now, deviceId, reading, partitionId);
        }
    }
}
