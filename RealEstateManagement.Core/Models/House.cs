using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateManagement.Core.Models
{
    public class House
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "標題必填")]
        [StringLength(100)]
        public required string Title { get; set; }

        [Required]
        [Range(1, 1000000000, ErrorMessage = "價格必須大於 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } // 單位：萬

        [Required]
        public required string Address { get; set; }

        public int SquareMeters { get; set; }

        public string? Description { get; set; } // 可空

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // 導覽屬性
        public virtual ICollection<Appointment>? Appointments { get; set; }
    }
}
