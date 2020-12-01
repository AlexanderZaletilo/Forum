using System.ComponentModel.DataAnnotations;
using Forum.Models;

namespace Forum.ViewModels
{
    public class TopicShowViewModel
    {
        [Required(ErrorMessage = "Empty message")]
        [StringLength(512, ErrorMessage = "Maximum length ")]
        [Display(Name = "Add a message")]
        public string Text { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public Topic Topic { get; set; }
        [Required]
        public int Id { get; set; }
    }
}
