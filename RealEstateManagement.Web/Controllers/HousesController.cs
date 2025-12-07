using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstateManagement.Core.Interfaces;
using RealEstateManagement.Core.Models;
using System.Security.Claims;

namespace RealEstateManagement.Web.Controllers
{
    [Authorize]
    public class HousesController : Controller
    {
        private readonly IHouseService _houseService;

        public HousesController(IHouseService houseService)
        {
            _houseService = houseService;
        }

        // 取得當前登入的房仲 ID
        private int GetCurrentAgentId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int agentId))
            {
                return agentId;
            }
            throw new UnauthorizedAccessException("無法識別使用者身分，請重新登入");
        }

        // GET: Houses
        public async Task<IActionResult> Index()
        {
            var houses = await _houseService.GetAllHousesAsync();
            return View(houses);
        }

        // GET: Houses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var house = await _houseService.GetHouseByIdAsync(id.Value);
            if (house == null) return NotFound();
            return View(house);
        }

        // GET: Houses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Houses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(House house)
        {
            // 移除 ModelState 驗證 Agent 錯誤
            ModelState.Remove("Agent");
            // 自動綁定當前 Agent
            house.AgentId = GetCurrentAgentId();

            if (ModelState.IsValid)
            {
                await _houseService.AddHouseAsync(house);
                return RedirectToAction(nameof(Index));
            }
            return View(house);
        }

        // GET: Houses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var house = await _houseService.GetHouseByIdAsync(id.Value);
            if (house == null) return NotFound();

            // 只能編輯自己的房源
            if (house.AgentId != GetCurrentAgentId())
            {
                TempData["Error"] = "您無權編輯其他人的房源！";
                return RedirectToAction(nameof(Index));
            }

            return View(house);
        }

        // POST: Houses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, House house)
        {
            if (id != house.Id) return NotFound();

            // 檢查身分
            var existingHouse = await _houseService.GetHouseByIdAsync(id);
            if (existingHouse == null || existingHouse.AgentId != GetCurrentAgentId()) return Unauthorized();

            // 確保 AgentId 不被篡改
            house.AgentId = existingHouse.AgentId;
            // 移除 ModelState 驗證 Agent 錯誤
            ModelState.Remove("Agent");

            if (ModelState.IsValid)
            {
                try
                {
                    await _houseService.UpdateHouseAsync(house);
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exists = await _houseService.GetHouseByIdAsync(house.Id);
                    if (exists == null) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(house);
        }

        // GET: Houses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var house = await _houseService.GetHouseByIdAsync(id.Value);
            if (house == null) return NotFound();

            // 只能刪除自己的房源
            if (house.AgentId != GetCurrentAgentId())
            {
                TempData["Error"] = "您無權刪除其他人的房源！";
                return RedirectToAction(nameof(Index));
            }

            return View(house);
        }

        // POST: Houses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var house = await _houseService.GetHouseByIdAsync(id);
            // 檢查身分
            if (house == null || house.AgentId != GetCurrentAgentId()) return Unauthorized();

            await _houseService.DeleteHouseAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
