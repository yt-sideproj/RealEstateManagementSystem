using Microsoft.AspNetCore.Mvc;
using RealEstateManagement.Core.Interfaces;
using RealEstateManagement.Core.Enums;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace RealEstateManagement.Web.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _apptService;

        // 注入 Service
        public AppointmentsController(IAppointmentService apptService)
        {
            _apptService = apptService;
        }

        // 取得當前登入的房仲 ID
        private int GetCurrentAgentId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int agentId))
            {
                return agentId;
            }
            throw new UnauthorizedAccessException();
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var currentAgentId = GetCurrentAgentId();

            // 1. 取得所有預約
            var allAppointments = await _apptService.GetAllAppointmentsAsync();

            // 2. 只篩選出當前房仲上架房源的預約
            var myAppointments = allAppointments
                .Where(a => a.House != null && a.House.AgentId == currentAgentId)
                .OrderByDescending(a => a.VisitDate)
                .ToList();

            return View(myAppointments);
        }

        // POST: Appointments/Confirm/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id)
        {
            // 操作前身分驗證
            if (!await IsMyAppointment(id)) return Unauthorized();

            await _apptService.UpdateStatusAsync(id, AppointmentStatus.Confirmed);
            return RedirectToAction(nameof(Index));
        }

        // POST: Appointments/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            // 操作前身分驗證
            if (!await IsMyAppointment(id)) return Unauthorized();

            // 呼叫 Service 的取消邏輯 (如果有實作通知信會在這裡觸發)
            await _apptService.UpdateStatusAsync(id, AppointmentStatus.Cancelled);
            return RedirectToAction(nameof(Index));
        }

        // POST: Appointments/Complete/5 (選用：標記帶看完成)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            // 操作前身分驗證
            if (!await IsMyAppointment(id)) return Unauthorized();

            await _apptService.UpdateStatusAsync(id, AppointmentStatus.Completed);
            return RedirectToAction(nameof(Index));
        }

        // 檢查預約是否屬於自己
        private async Task<bool> IsMyAppointment(int appointmentId)
        {
            var currentAgentId = GetCurrentAgentId();
            var appointment = await _apptService.GetAppointmentByIdAsync(appointmentId);

            // 檢查：
            // 1. 預約是否存在
            // 2. 房源是否存在
            // 3. 房源的負責人是否為當前登入者
            return appointment != null &&
                   appointment.House != null &&
                   appointment.House.AgentId == currentAgentId;
        }
    }
}
