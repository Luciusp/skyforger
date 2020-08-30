using System.ComponentModel.DataAnnotations;

namespace skyforger.models.backpacks
{
    public class Backpack
    {
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }
        
        [Display(Name = "Notes/Effect")]
        public string NotesEffect { get; set; }
        
        [Display(Name = "GP Value")]
        public float GpValue { get; set; }

        [Display(Name = "Quantity")]
        public float Quantity { get; set; }

        [Display(Name = "Carried By")]
        public string OwnerId { get; set; }
    }
}