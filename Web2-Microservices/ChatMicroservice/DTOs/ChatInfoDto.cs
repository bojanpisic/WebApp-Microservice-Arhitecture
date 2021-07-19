using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatMicroservice.DTOs
{
    public class ChatInfoDto
    {
        [Required]
        public string connectionId { get; set; }
        [Required]

        public string roomName { get; set; }
    }
}
