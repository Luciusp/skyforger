using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using skyforger.models.classes;
using skyforger.models.common;
using skyforger.models.common.Feats;
using skyforger.models.creatures;

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