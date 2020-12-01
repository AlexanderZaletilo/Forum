using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace Forum.Models
{
    public class User: IdentityUser
    {
        [Required(ErrorMessage = "Choose a file")]
        public string ProfileImage { get; set; }
        public int MessageCount { get; set; } = 0;

        public DateTime Registered { get; set; }
        public List<Topic> Topics { get; set; }
        public List<Message> Messages { get; set; }
        public List<PrivateMessage> SentMessages { get; set; }
        public List<PrivateMessage> ReceivedMessages { get; set; }
        [NotMapped]
        public List<PrivateMessage> DialogMessages { get; set; }

    }
}
