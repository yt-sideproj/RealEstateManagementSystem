using Microsoft.AspNetCore.Mvc;
using RealEstateManagement.Core.DTOs;
using RealEstateManagement.Core.Interfaces;
using RealEstateManagement.Infrastructure.Helpers;

namespace RealEstateManagement.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly JwtHelper _jwtHelper;

        public AuthController(ICustomerRepository customerRepo, JwtHelper jwtHelper)
        {
            _customerRepo = customerRepo;
            _jwtHelper = jwtHelper;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] CustomerLoginDto request)
        {
            // 1. 查無此人
            var customer = await _customerRepo.GetByEmailAsync(request.Email);
            if (customer == null) return Unauthorized(new { message = "帳號或密碼錯誤" });

            // 2. 驗證密碼
            if (!BCrypt.Net.BCrypt.Verify(request.Password, customer.Password))
            {
                return Unauthorized(new { message = "帳號或密碼錯誤" });
            }

            // 3. 發放 Token
            var token = _jwtHelper.GenerateToken(customer);
            return Ok(new { token });
        }
    }
}
