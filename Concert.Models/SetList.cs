using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Concert.Models
{
    public class SetList
    {
        [Key]
        public int Id { get; set; }

        public int SongId { get; set; }
        [ForeignKey("SongId")]
        [ValidateNever]
        public Song Song { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "It must be between 1 and 100.")]
        public int Order { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
