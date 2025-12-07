using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstateManagement.Core.DTOs;
using RealEstateManagement.Core.Interfaces;
using RealEstateManagement.Core.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RealEstateManagement.Web.Controllers.Api
{
    [Route("api/appointments")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // 使用Jwt
    public class AppointmentsApiController : ControllerBase
    {
        private readonly IAppointmentService _apptService;
        private readonly IHouseService _houseService;

        public AppointmentsApiController(IAppointmentService apptService, IHouseService houseService)
        {
            _apptService = apptService;
            _houseService = houseService;
        }

        private int? GetCurrentCustomerId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int customerId))
            {
                return customerId;
            }
            return null;
        }

        // POST: api/appointments
        // 預約看房
        [HttpPost]
        public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentDto request)
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == null || !customerId.HasValue) 
                return Unauthorized(new { message = "無效的使用者身分" });

            // 檢查房源是否存在且上架中
            var house = await _houseService.GetHouseByIdAsync(request.HouseId);
            if (house == null || !house.IsActive)
                return BadRequest(new { message = "該房源不存在或已下架，無法預約" });

            // 檢查預約時間
            // 簡單驗證：不能預約過去的時間
            if (request.VisitDate < DateTime.Now)
                return BadRequest(new { message = "預約時間必須是未來時間" });

            // 4. 建立預約物件
            var appointment = new Appointment
            {
                HouseId = request.HouseId,
                CustomerId = customerId.Value, // 來自 Token
                VisitDate = request.VisitDate,
                // Status 會在 Service 裡預設為 Pending
            };

            // 存入資料庫
            await _apptService.AddAppointmentAsync(appointment);

            return Ok(new { message = "預約成功，請靜待房仲聯繫確認", appointmentId = appointment.Id });
        }

        // GET: api/appointments/my
        // 讓客戶查詢自己的預約紀錄
        [HttpGet("my")]
        public async Task<IActionResult> GetMyAppointments()
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == null || !customerId.HasValue)
                return Unauthorized(new { message = "無效的使用者身分" });

            // 取得所有預約並篩選 (如果 Service 有 GetByCustomerId 會更好，這裡先用 LINQ 過濾)
            var allAppts = await _apptService.GetAllAppointmentsAsync();
            var myAppts = allAppts
                .Where(a => a.CustomerId == customerId.Value)
                .Select(a => new
                {
                    a.Id,
                    a.VisitDate,
                    Status = a.Status.ToString(), // 轉成字串顯示 (Pending/Confirmed...)
                    HouseTitle = a.House?.Title,
                    Address = a.House?.FullAddress // 記得 House Model 有加這個 Helper 屬性嗎？
                })
                .OrderByDescending(x => x.VisitDate);

            return Ok(myAppts);
        }
    }
}
