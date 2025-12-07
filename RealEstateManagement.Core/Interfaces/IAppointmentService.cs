using RealEstateManagement.Core.Models;
using RealEstateManagement.Core.Enums;

namespace RealEstateManagement.Core.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();

        Task<Appointment?> GetAppointmentByIdAsync(int id);

        // 建立預約
        Task AddAppointmentAsync(Appointment appointment);

        // 更新預約狀態
        Task UpdateStatusAsync(int id, AppointmentStatus status);

        // 更新預約時間
        Task RescheduleAppointmentAsync(int id, DateTime newDate);

        // 取消預約 (真刪除)
        Task DeleteAppointmentAsync(int id);
    }
}
