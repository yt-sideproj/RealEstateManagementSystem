using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RealEstateManagement.Core.Models
{
    public class Agent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; } // 房仲姓名 (e.g. 王小明)

        [Required]
        public required string EmployeeCode { get; set; } // 員工編號/帳號 (e.g. A001)

        [Required]
        [JsonIgnore]
        public required string Password { get; set; } // 密碼

        // 導覽屬性：一個房仲可以上架多間房子
        public virtual ICollection<House>? Houses { get; set; }
    }
}
