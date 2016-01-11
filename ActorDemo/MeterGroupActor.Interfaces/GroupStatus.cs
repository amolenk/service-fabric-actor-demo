using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MeterGroupActor.Interfaces
{
    [DataContract]
    public class GroupStatus
    {
        [DataMember]
        public int DeviceCount { get; set; }

        [DataMember]
        public decimal AverageReading { get; set; }
    }
}
