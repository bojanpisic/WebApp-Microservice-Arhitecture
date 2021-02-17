using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.DTOs
{
    public class TripInfo
    {
        public IEnumerable<FlightInfo> FlightInfo { get; set; }
        public float TotalPrice { get; set; }

        public TripInfo()
        {
            FlightInfo = new List<FlightInfo>();
        }
    }
}
