using Microsoft.EntityFrameworkCore;
using RealEstateManagement.Core.Interfaces;
using RealEstateManagement.Core.Models;
using RealEstateManagement.Infrastructure.Data;

namespace RealEstateManagement.Infrastructure.Repositories
{
    public class AgentRepository : IAgentRepository
    {
        private readonly AppDbContext _context;

        public AgentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Agent?> GetByEmployeeCodeAsync(string employeeCode)
        {
            return await _context.Agents
                .FirstOrDefaultAsync(a => a.EmployeeCode == employeeCode);
        }
    }
}
