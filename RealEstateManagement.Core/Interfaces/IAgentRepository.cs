using RealEstateManagement.Core.Models;

namespace RealEstateManagement.Core.Interfaces
{
    public interface IAgentRepository
    {
        // 根據員工編號找人 (登入用)
        Task<Agent?> GetByEmployeeCodeAsync(string employeeCode);
    }
}
