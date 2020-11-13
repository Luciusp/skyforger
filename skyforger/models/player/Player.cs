using System.ComponentModel.DataAnnotations;

namespace skyforger.models.player
{
    public class Player
    {
        public int Id { get; set; }
        
        public string Auth0Id { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }
        
        [Required]
        [Display(Name = "Character Name")]
        public string CharacterName { get; set; }
        
        [Required]
        [Display(Name = "Profile Picture URL")]
        public string ProfilePictureUri { get; set; }

    }
}