using RealEstateManagement.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace RealEstateManagement.Web.Models
{
    public class HouseViewModel : House
    {
        [Display(Name = "房源照片")]
        public IFormFile? ImageFile { get; set; }
    }
}