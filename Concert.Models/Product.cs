using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Concert.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Required field.")]
        [MaxLength(100)]
        [DisplayName("Artist Name")]
        public string Artist { get; set; }

        [Required(ErrorMessage = "Required field.")]
        [MaxLength(100)]
        [DisplayName("Song Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Required field.")]
        [DisplayName("Song Length")]
        public TimeSpan Length { get; set; }

        [Required(ErrorMessage = "Required field.")]
        [Range(1900, 2100, ErrorMessage = "It must be between 1900 and 2100.")]
        [DisplayName("Release Year")]
        public int ReleaseYear { get; set; }
        
        /*
        // TODO because these properties are linked with other models
        [MaxLength(30)]
        [DisplayName("Music Genre")]
        public string Genre { get; set; }
        
        [MaxLength(30)]
        public string Language { get; set; }
        */
    }
}
