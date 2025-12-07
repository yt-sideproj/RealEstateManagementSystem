using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using RealEstateManagement.Core.Interfaces;
using RealEstateManagement.Core.Models;
using RealEstateManagement.Web.Models;
using System.Security.Claims;

namespace RealEstateManagement.Web.Controllers
{
    [Authorize]
    public class HousesController : Controller
    {
        private readonly IHouseService _houseService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public HousesController(IHouseService houseService, IWebHostEnvironment hostEnvironment)
        {
            _houseService = houseService;
            _hostEnvironment = hostEnvironment;
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
        // [修改] 參數改為 HouseViewModel
        public async Task<IActionResult> Create(HouseViewModel houseVM)
        {
            houseVM.AgentId = GetCurrentAgentId();
            houseVM.CreatedAt = DateTime.Now;
            houseVM.IsActive = true;

            // 移除 ModelState 驗證 Agent、ImageFile 錯誤
            ModelState.Remove("Agent");
            ModelState.Remove("ImageFile");

            if (ModelState.IsValid)
            {
                // 先存入資料庫以取得 ID
                await _houseService.AddHouseAsync(houseVM);

                // 處理圖片上傳
                if (houseVM.ImageFile != null)
                {
                    await SaveImage(houseVM.ImageFile, houseVM.Id);
                }

                return RedirectToAction(nameof(Index));
            }
            return View(houseVM);
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

            var viewModel = new HouseViewModel
            {
                Id = house.Id,
                Title = house.Title,
                Price = house.Price,
                SquareMeters = house.SquareMeters,
                City = house.City,
                District = house.District,
                DetailAddress = house.DetailAddress,
                Description = house.Description,
                IsActive = house.IsActive,
                AgentId = house.AgentId,
                CreatedAt = house.CreatedAt
            };

            return View(viewModel);
        }

        // POST: Houses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [修改] 參數改為 HouseViewModel
        public async Task<IActionResult> Edit(int id, HouseViewModel houseVM)
        {
            if (id != houseVM.Id) return NotFound();

            // 檢查身分
            var existingHouse = await _houseService.GetHouseByIdAsync(id);
            if (existingHouse == null || existingHouse.AgentId != GetCurrentAgentId()) return Unauthorized();

            // 確保 AgentId 不被篡改
            houseVM.AgentId = existingHouse.AgentId;

            // 移除 ModelState 驗證 Agent、ImageFile 錯誤
            ModelState.Remove("Agent");
            ModelState.Remove("ImageFile");

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. 更新資料庫
                    await _houseService.UpdateHouseAsync(houseVM);

                    // 2. 如果有上傳新照片，就覆蓋舊的
                    if (houseVM.ImageFile != null)
                    {
                        await SaveImage(houseVM.ImageFile, houseVM.Id);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _houseService.GetHouseByIdAsync(houseVM.Id) == null) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(houseVM);
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
            if (house == null || house.AgentId != GetCurrentAgentId()) return Unauthorized();

            await _houseService.DeleteHouseAsync(id);

            // 刪除對應的照片
            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "house_images", id + ".jpg");
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            return RedirectToAction(nameof(Index));
        }

        // 儲存圖片的共用方法
        private async Task SaveImage(IFormFile file, int houseId)
        {
            // 確保資料夾存在
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string path = Path.Combine(wwwRootPath, "house_images");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // 檔名規則：{HouseId}.jpg (統一存成 jpg 或保留原始副檔名)
            string fileName = houseId + ".jpg";
            string fullPath = Path.Combine(path, fileName);

            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
        }
    }
}
