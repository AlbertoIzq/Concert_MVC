using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Concert.Models
{
    public class SongImage
    {
        public int Id { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        public int SongId { get; set; }
        [ForeignKey("SongId")]
        public Song Song { get; set; }
    }
}
