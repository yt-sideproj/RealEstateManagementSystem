using RealEstateManagement.Core.Models;

namespace RealEstateManagement.Core.Interfaces
{
    public interface ICustomerRepository
    {
        // 根據 Email 找人 (登入用)
        Task<Customer?> GetByEmailAsync(string email);
    }
}
