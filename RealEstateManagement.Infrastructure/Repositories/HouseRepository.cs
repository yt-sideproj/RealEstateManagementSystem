using Microsoft.EntityFrameworkCore;
using RealEstateManagement.Core.Interfaces;
using RealEstateManagement.Core.Models;
using RealEstateManagement.Infrastructure.Data;

namespace RealEstateManagement.Infrastructure.Repositories
{
    public class HouseRepository : IHouseRepository
    {
        private readonly AppDbContext _context;

        public HouseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<House>> GetAllAsync()
        {
            // 使用 AsNoTracking 提升查詢效能 (Read-Only)
            return await _context.Houses
                .OrderByDescending(h => h.CreatedAt) // 最新的排前面
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<House?> GetByIdAsync(int id)
        {
            return await _context.Houses.FindAsync(id);
        }

        public async Task AddAsync(House house)
        {
            await _context.Houses.AddAsync(house);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(House house)
        {
            _context.Houses.Update(house);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var house = await _context.Houses.FindAsync(id);
            if (house != null)
            {
                // 軟刪除 (Soft Delete) 或是直接刪除
                house.IsDeleted = true;
                _context.Houses.Update(house);
                //_context.Houses.Remove(house);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Houses.AnyAsync(e => e.Id == id);
        }
    }
}
