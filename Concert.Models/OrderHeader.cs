using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Concert.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }

		[ValidateNever] // Needed for Model validation in SummaryPOST method
		public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        public DateTime OrderDate { get; set; }
		[DisplayName("Order total (€)")]
		public double OrderTotal { get; set; }
        public string? OrderStatus { get; set; }

        public string? PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateOnly PaymentDueDate { get; set; }

        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }
        [Required]
        [DisplayName("Street Address")]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Country { get; set; }
        [DisplayName("Postal Code")]
        public string PostalCode { get; set; }
        [Required]
        [DisplayName("Concert Date")]
        public DateTime ConcertDate { get; set; }
    }
}
