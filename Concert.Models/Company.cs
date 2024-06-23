using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Concert.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Required field.")]
        [MaxLength(100)]
        [DisplayName("Company Name")]
        public string Name { get; set; }

        [DisplayName("Street Address")]
        public string? StreetAddress { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [DisplayName("Postal Code")]
        public string? PostalCode { get; set; }

        [DisplayName("Phone Number")]
        public string? PhoneNumber { get; set; }
    }
}
