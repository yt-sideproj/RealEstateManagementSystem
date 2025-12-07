using RealEstateManagement.Core.Interfaces;
using RealEstateManagement.Core.Models;
using RealEstateProject.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateManagement.Infrastructure.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepo;

        public AppointmentService(IAppointmentRepository appointmentRepo)
        {
            _appointmentRepo = appointmentRepo;
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            return await _appointmentRepo.GetAllAsync();
        }

        public async Task AddAppointmentAsync(Appointment appointment)
        {
            // 預約建立時預設為 Pending
            appointment.Status = AppointmentStatus.Pending;
            appointment.CreatedAt = DateTime.Now;

            await _appointmentRepo.AddAsync(appointment);
        }

        public async Task UpdateStatusAsync(int id, AppointmentStatus status)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment != null)
            {
                appointment.Status = status;
                await _appointmentRepo.UpdateAsync(appointment);
            }
        }

        public async Task DeleteAppointmentAsync(int id)
        {
            // 這裡直接呼叫 Repository 的刪除
            // 如果未來有邏輯 (例如：發送 Email 通知客戶已被取消)，也可以寫在這裡
            await _appointmentRepo.DeleteAsync(id);
        }
    }
}
