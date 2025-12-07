using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateManagement.Core.DTOs
{
    public class BookAppointmentDto
    {
        [Required]
        public int HouseId { get; set; }

        [Required]
        public DateTime VisitDate { get; set; }
    }
}
