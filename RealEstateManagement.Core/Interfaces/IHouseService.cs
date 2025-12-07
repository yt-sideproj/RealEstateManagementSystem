using RealEstateManagement.Core.DTOs;
using RealEstateManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateManagement.Core.Interfaces
{
    public interface IHouseService
    {
        // 後台操作
        Task<IEnumerable<House>> GetAllHousesAsync();
        Task<House?> GetHouseByIdAsync(int id);
        Task AddHouseAsync(House house);
        Task UpdateHouseAsync(House house);
        Task DeleteHouseAsync(int id);

        // API 接取資料
        Task<IEnumerable<House>> GetActiveHousesAsync(HouseSearchDto searchDto);
    }
}
