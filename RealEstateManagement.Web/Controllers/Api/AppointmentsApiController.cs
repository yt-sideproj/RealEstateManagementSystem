using Azure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstateManagement.Core.DTOs;
using RealEstateManagement.Core.Enums;
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

            // 取得所有預約並篩選
            var allAppts = await _apptService.GetAllAppointmentsAsync();

            // 取得目前的 Base URL
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var myAppts = allAppts
                .Where(a => a.CustomerId == customerId.Value)
                .Select(a => new
                {
                    a.Id,
                    a.VisitDate,
                    Status = a.Status.ToString(),
                    HouseTitle = a.House?.Title,
                    Address = a.House?.FullAddress,
                    // [新增] 圖片完整路徑
                    ImageUrl = a.House != null
                        ? $"{baseUrl}/house_images/{a.House.Id}.jpg"
                        : null
                })
                .OrderByDescending(x => x.VisitDate);

            return Ok(myAppts);
        }

        // PATCH: api/appointments/{id}
        // 變更預約
        [HttpPatch("{id}")]
        public async Task<IActionResult> Reschedule (int id, [FromBody] RescheduleAppointmentDto request)
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == null || !customerId.HasValue)
                return Unauthorized(new { message = "無效的使用者身分" });

            // 1. 檢查預約是否存在
            var appointment = await _apptService.GetAppointmentByIdAsync(id);
            if (appointment == null) return NotFound(new { message = "找不到此預約" });

            // 2. 檢查是否為該客戶的預約
            if (appointment.CustomerId != customerId.Value) return Forbid(); // 403 Forbidden

            // 3. 檢查狀態 (已完成或已取消的不能改)
            if (appointment.Status == AppointmentStatus.Completed || appointment.Status == AppointmentStatus.Cancelled)
                return BadRequest(new { message = "此預約已結案或取消，無法變更時間" });

            // 4. 檢查新時間是否合理 (例如不能是過去)
            if (request.NewVisitDate < DateTime.Now)
                return BadRequest(new { message = "預約時間必須是未來時間" });

            // 5. 執行改期
            await _apptService.RescheduleAppointmentAsync(id, request.NewVisitDate);

            return Ok(new { message = "預約時間已變更，請靜待房仲確認" });
        }

        // DELETE: api/appointments/{id}
        // 取消預約
        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == null || !customerId.HasValue)
                return Unauthorized(new { message = "無效的使用者身分" });

            // 1. 檢查預約是否存在
            var appointment = await _apptService.GetAppointmentByIdAsync(id);
            if (appointment == null) return NotFound();

            // 2. 檢查是否為該客戶的預約
            if (appointment.CustomerId != customerId.Value) return Forbid();

            // 3. 檢查狀態 (已完成的不能取消)
            if (appointment.Status == AppointmentStatus.Completed) 
                return BadRequest(new { message = "已完成的預約無法取消" });

            // 4. 執行取消 (變更狀態為 Cancelled)
            await _apptService.UpdateStatusAsync(id, AppointmentStatus.Cancelled);

            return Ok(new { message = "預約已取消" });
        }
    }
}
