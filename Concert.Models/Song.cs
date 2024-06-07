using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Concert.Models
{
    public class Song
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
        [DisplayName("Cover Length")]
        public TimeSpan Length { get; set; }

        [Required(ErrorMessage = "Required field.")]
        [Range(1900, 2100, ErrorMessage = "It must be between 1900 and 2100.")]
        [DisplayName("Release Year")]
        public int ReleaseYear { get; set; }

        [DisplayName("Genre")]
        public int GenreId { get; set; }

        [ForeignKey("GenreId")]
        [ValidateNever]
        public Genre Genre { get; set; }

        [DisplayName("Language")]
        public int LanguageId { get; set; }

        [ForeignKey("LanguageId")]
        [ValidateNever]
        public Language Language { get; set; }

        [DisplayName("Image Url")]
        [ValidateNever]
        public string ImageUrl { get; set; }
    }
}
