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
        public void ReadingAvailable(long deviceId, int reading)
        {
            Console.WriteLine("{2:HH:mm:ss} Reading available for device {0}: {1}", deviceId, reading, DateTime.Now);
        }
    }
}
