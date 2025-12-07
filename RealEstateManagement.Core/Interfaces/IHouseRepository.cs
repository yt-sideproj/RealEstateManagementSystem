using RealEstateManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateManagement.Core.Interfaces
{
    public interface IHouseRepository
    {
        // 查詢
        Task<IEnumerable<House>> GetAllAsync();
        Task<House?> GetByIdAsync(int id);

        // 新增/修改/刪除
        Task AddAsync(House house);
        Task UpdateAsync(House house);
        Task DeleteAsync(int id);

        // 判斷是否存在
        Task<bool> ExistsAsync(int id);
    }
}
