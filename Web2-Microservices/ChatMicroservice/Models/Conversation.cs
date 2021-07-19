using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatMicroservice.Models
{
    public class Conversation
    {
        public Conversation()
        {
            Messages = new HashSet<Message>();
        }
        public int ConversationId { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        [Required]
        public string SenderId { get; set; }
        [Required]
        public string ReceiverId { get; set; }
        [Required]
        public string ReceiverFirstname { get; set; }
        [Required]
        public string ReceiverLastName { get; set; }
        [Required]
        public string SenderFirstname { get; set; }
        [Required]
        public string SenderLastName { get; set; }
        [Required]
        public string RoomName { get; set; }
        public DateTime ReceiverDeleteTime { get; set; } = DateTime.MinValue;
        public DateTime SenderDeleteTime { get; set; } = DateTime.MinValue;
    }
}
