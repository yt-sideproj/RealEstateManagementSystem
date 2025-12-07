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
        [StringLength(20)]
        public required string City { get; set; } // 縣市 (e.g. 台北市)

        [Required]
        [StringLength(20)]
        public required string District { get; set; } // 行政區 (e.g. 大安區)

        [Required]
        [StringLength(100)]
        public required string DetailAddress { get; set; }// 詳細地址 (e.g. 信義路三段100號)

        public int SquareMeters { get; set; }

        public string? Description { get; set; } // 可空

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // 導覽屬性
        public virtual ICollection<Appointment>? Appointments { get; set; }

        // [Helper] 方便之後顯示完整地址 (唯讀屬性)
        [NotMapped] // 不要存到資料庫
        public string FullAddress => $"{City}{District}{DetailAddress}";
    }
}
