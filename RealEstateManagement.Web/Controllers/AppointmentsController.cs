using Microsoft.AspNetCore.Mvc;
using RealEstateManagement.Core.Interfaces;
using RealEstateManagement.Core.Enums;
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

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            // Repository 已經寫好 Include(House) 和 Include(Customer)
            // 所以這裡拿到的資料，可以直接讀到 House.Title 和 Customer.Name
            var appointments = await _apptService.GetAllAppointmentsAsync();
            return View(appointments);
        }

        // POST: Appointments/Confirm/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id)
        {
            await _apptService.UpdateStatusAsync(id, AppointmentStatus.Confirmed);
            return RedirectToAction(nameof(Index));
        }

        // POST: Appointments/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            // 呼叫 Service 的取消邏輯 (如果有實作通知信會在這裡觸發)
            await _apptService.UpdateStatusAsync(id, AppointmentStatus.Cancelled);
            return RedirectToAction(nameof(Index));
        }

        // POST: Appointments/Complete/5 (選用：標記帶看完成)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            await _apptService.UpdateStatusAsync(id, AppointmentStatus.Completed);
            return RedirectToAction(nameof(Index));
        }
    }
}
