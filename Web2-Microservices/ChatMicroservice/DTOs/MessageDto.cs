using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatMicroservice.DTOs
{
    public class MessageDto
    {
        [Required]

        public string Text { get; set; }
    }
}
