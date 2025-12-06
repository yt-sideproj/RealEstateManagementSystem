using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RealEstateManagement.Core.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; } // 帳號

        [Required]
        [JsonIgnore]
        public required string Password { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        [Phone]
        public required string Phone { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // 導覽屬性
        public virtual ICollection<Appointment>? Appointments { get; set; }
    }
}
