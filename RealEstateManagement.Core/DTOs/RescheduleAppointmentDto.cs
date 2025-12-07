using System.ComponentModel.DataAnnotations;

namespace RealEstateManagement.Core.DTOs
{
    public class RescheduleAppointmentDto
    {
        [Required]
        public DateTime NewVisitDate { get; set; }
    }
}   