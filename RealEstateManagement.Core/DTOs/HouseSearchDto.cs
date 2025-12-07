using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateManagement.Core.DTOs
{
    public class HouseSearchDto
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }

        // 未來擴充容易，例如：
        // public string? Keyword { get; set; }
        // public int? MinSquareMeters { get; set; }
    }
}
