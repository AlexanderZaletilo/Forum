using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
namespace Forum.Models
{
    public class PrivateMessage
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Published { get; set; }
        public string SenderId { get; set; }
        [ForeignKey("SenderId")]
        [InverseProperty("SentMessages")]
        public User Sender { get; set; }
        public string ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        [InverseProperty("ReceivedMessages")]
        public User Receiver { get; set; }
    }
}
