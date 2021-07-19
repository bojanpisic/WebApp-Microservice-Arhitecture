using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatMicroservice.DTOs
{
    public class SendMessageDto
    {
        [Required]
        public int ConversationId { get; set; }
        [Required]

        public string SenderId { get; set; }
        [Required]

        public string Message { get; set; }
        [Required]

        public string RoomName { get; set; }
    }
}
