using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RACSMicroservice.IntegrationEvents.Events
{
    public class CreateRACSIntegrationEvent : IntegrationEvent
    {
        public CreateRACSIntegrationEvent(string name, string city, string state, double lon, double lat, string adminId)
        {
            Name = name;
            City = city;
            State = state;
            Lon = lon;
            Lat = lat;
            AdminId = adminId;
        }

        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public double Lon { get; set; }
        public double Lat { get; set; }
        public string AdminId { get; set; }

    }
}
