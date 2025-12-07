using RealEstateManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateManagement.Core.Interfaces
{
    public interface IAppointmentRepository
    {
        // 查詢
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<Appointment?> GetByIdAsync(int id);

        // 新增/修改/刪除
        Task AddAsync(Appointment appointment);
        Task UpdateAsync(Appointment appointment);
        Task DeleteAsync(int id);

        // 額外功能：取得某個房子的所有預約
        Task<IEnumerable<Appointment>> GetByHouseIdAsync(int houseId);
    }
}
