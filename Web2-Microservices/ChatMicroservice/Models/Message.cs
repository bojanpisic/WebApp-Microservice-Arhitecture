using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatMicroservice.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }
        public bool Received { get; set; } = false;
        public string SenderId { get; set; }
        public int ConversationId {get; set;}
        public virtual Conversation Conversation { get; set; }
    }
}
