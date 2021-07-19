using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatMicroservice.DTOs
{
    public class SendMessageToNewConversationDto
    {
        [Required]

        public string ReceiverId { get; set; }
        [Required]

        public string Message { get; set; }
        [Required]

        public string RoomName { get; set; }

        [Required]

        public string ReceiverLastName { get; set; }
        [Required]
        public string ReceiverFirstName { get; set; }
    }
}
