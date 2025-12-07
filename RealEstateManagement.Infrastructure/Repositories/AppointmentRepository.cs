using Microsoft.EntityFrameworkCore;
using RealEstateManagement.Core.Interfaces;
using RealEstateManagement.Core.Models;
using RealEstateManagement.Infrastructure.Data;
using RealEstateManagement.Core.Enums;

namespace RealEstateManagement.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            // Eager Loading: 一次把關聯的 House 和 Customer 資料抓進來
            return await _context.Appointments
                .Include(a => a.House)
                .Include(a => a.Customer)
                .OrderByDescending(a => a.VisitDate)
                .ToListAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.House)
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment != null)
            {
                // 執行軟刪除
                appointment.Status = AppointmentStatus.Cancelled;
                _context.Appointments.Update(appointment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Appointment>> GetByHouseIdAsync(int houseId)
        {
            return await _context.Appointments
                .Where(a => a.HouseId == houseId)
                .Include(a => a.Customer)
                .ToListAsync();
        }
    }
}
