using System.ComponentModel.DataAnnotations;

namespace Forum.ViewModels
{
    public class TopicCreationViewModel
    {
        [Required(ErrorMessage = "Empty topic name")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Empty openning message")]
        [StringLength(512, ErrorMessage = "Maximum length is 512")]
        [Display(Name = "Openning message")]
        public string InitialMessage { get; set; }

    }
}
