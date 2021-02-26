using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RACSMicroservice.IntegrationEvents.Events
{
    public class RentCarEvent : IntegrationEvent
    {
        public RentCarEvent(int carId, DateTime takeOverDate, DateTime returnDate, string takeOverCity, string returnCity, string userId, List<int> ticketIds)
        {
            CarId = carId;
            TakeOverDate = takeOverDate;
            ReturnDate = returnDate;
            TakeOverCity = takeOverCity;
            ReturnCity = returnCity;
            UserId = userId;
            TicketIds = ticketIds;
        }

        public List<int> TicketIds { get; set; }
        public int CarId { get; set; }
        public DateTime TakeOverDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string TakeOverCity { get; set; }
        public string ReturnCity { get; set; }
        public string UserId { get; set; }
    }
}
