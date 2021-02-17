using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineMicroservice.IntegrationEvents.Events
{
    public class CreateAirlineIntegrationEvent : IntegrationEvent
    {
        public CreateAirlineIntegrationEvent(string name, string city, string state, double lat, double lon, string adminId)
        {
            Name = name;
            City = city;
            State = state;
            Lat = lat;
            Lon = lon;
            AdminId = adminId;
        }

        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string AdminId { get; set; }

    }
}
