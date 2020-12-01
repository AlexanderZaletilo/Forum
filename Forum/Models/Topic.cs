using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace Forum.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MessageCount { get; set; }
        public DateTime Opened { get; set; }
        public DateTime? Closed { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public List<Message> Messages { get; set; }
    }
}
