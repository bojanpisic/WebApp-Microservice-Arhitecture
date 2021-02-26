using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserMicroservice.IntegrationEvents.Events
{
    public class SendConfirmationEmailEvent : IntegrationEvent
    {
        public SendConfirmationEmailEvent(string receiverEmail, string takeOverCity, string returnCity,
            float totalPrice, DateTime rentDate,
            DateTime takeOverDate, DateTime returnDate, string brand,
            string model, string type, float pricePerDay)
        {
            ReceiverEmail = receiverEmail;
            TakeOverCity = takeOverCity;
            ReturnCity = returnCity;
            TotalPrice = totalPrice;
            RentDate = rentDate;
            TakeOverDate = takeOverDate;
            ReturnDate = returnDate;
            Brand = brand;
            Model = model;
            Type = type;
            PricePerDay = pricePerDay;
        }

        public string ReceiverEmail { get; set; }
        public string TakeOverCity { get; set; }
        public string ReturnCity { get; set; }
        public float TotalPrice { get; set; }
        public DateTime RentDate { get; set; }
        public DateTime TakeOverDate { get; set; }
        public DateTime ReturnDate { get; set; }

        public string Brand { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }

        public float PricePerDay { get; set; }
    }
}
