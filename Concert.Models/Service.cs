using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Concert.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Required field.")]
        [MaxLength(100)]
        [DisplayName("Service Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Required field.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Required field.")]
        [Display(Name = "Fixed price")]
        [Range(1, 100000)]
        public double PriceFixed { get; set; }

        [Required(ErrorMessage = "Required field.")]
        [Display(Name = "Price per song")]
        [Range(1, 100000)]
        public double PricePerSong { get; set; }
    }
}
