using RealEstateManagement.Core.DTOs;
using RealEstateManagement.Core.Interfaces;
using RealEstateManagement.Core.Models;
using RealEstateManagement.Core.Enums;
using System.Transactions;

namespace RealEstateManagement.Infrastructure.Services
{
    public class HouseService : IHouseService
    {
        private readonly IHouseRepository _houseRepo;
        private readonly IAppointmentRepository _apptRepo;

        public HouseService(IHouseRepository houseRepo, IAppointmentRepository apptRepo)
        {
            _houseRepo = houseRepo;
            _apptRepo = apptRepo;
        }

        public async Task<IEnumerable<House>> GetAllHousesAsync()
        {
            return await _houseRepo.GetAllAsync();
        }

        public async Task<House?> GetHouseByIdAsync(int id)
        {
            return await _houseRepo.GetByIdAsync(id);
        }

        public async Task AddHouseAsync(House house)
        {
            house.CreatedAt = DateTime.Now;
            await _houseRepo.AddAsync(house);
        }

        public async Task UpdateHouseAsync(House house)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var existingHouse = await _houseRepo.GetByIdAsync(house.Id);
                    if (existingHouse == null) return;

                    // 邏輯：如果原本是上架，現在變成下架
                    if (existingHouse.IsActive == true && house.IsActive == false)
                    {
                        // 取消相關預約
                        var appointments = await _apptRepo.GetByHouseIdAsync(house.Id);
                        var activeAppointments = appointments.Where(a =>
                            a.Status == AppointmentStatus.Pending ||
                            a.Status == AppointmentStatus.Confirmed);

                        foreach (var appt in activeAppointments)
                        {
                            appt.Status = AppointmentStatus.Cancelled;
                            await _apptRepo.UpdateAsync(appt);
                        }
                    }

                    // 更新房源
                    await _houseRepo.UpdateAsync(house);

                    scope.Complete();
                }
                catch (Exception)
                {
                    throw; // 繼續把錯誤往外拋，讓 Controller 知道
                }
            }
        }

        public async Task DeleteHouseAsync(int id)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // 1. 刪除房源
                await _houseRepo.DeleteAsync(id);

                // 2. 取消相關預約
                var appointments = await _apptRepo.GetByHouseIdAsync(id);
                var activeAppointments = appointments.Where(a =>
                        a.Status == AppointmentStatus.Pending ||
                        a.Status == AppointmentStatus.Confirmed);

                foreach (var appt in activeAppointments)
                {
                    appt.Status = AppointmentStatus.Cancelled;
                    await _apptRepo.UpdateAsync(appt);
                }

                scope.Complete();
            }
        }

        // API 接取資料
        public async Task<IEnumerable<House>> GetActiveHousesAsync(HouseSearchDto searchDto)
        {
            var allHouses = await _houseRepo.GetAllAsync();

            var query = allHouses.Where(h => h.IsActive);

            // 1. 價格區間搜尋
            if (searchDto.MinPrice.HasValue)
                query = query.Where(h => h.Price >= searchDto.MinPrice.Value);
            if (searchDto.MaxPrice.HasValue)
                query = query.Where(h => h.Price <= searchDto.MaxPrice.Value);

            // 2. 縣市搜尋
            if (!string.IsNullOrEmpty(searchDto.City))
                query = query.Where(h => h.City == searchDto.City);

            // 3. 行政區搜尋
            if (!string.IsNullOrEmpty(searchDto.District))
                query = query.Where(h => h.District == searchDto.District);

            return query;
        }
    }
}
