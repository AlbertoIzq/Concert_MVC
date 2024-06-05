using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Concert.Models
{
    public class Language
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Required field.")]
        [MaxLength(30)]
        [DisplayName("Language")]
        public string Name { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "It must be between 1 and 100.")]
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
    }
}
