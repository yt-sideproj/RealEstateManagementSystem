using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstateManagement.Core.Interfaces;
using RealEstateManagement.Core.Models;

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
            return View(house);
        }

        // POST: Houses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, House house)
        {
            if (id != house.Id) return NotFound();

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
            return View(house);
        }

        // POST: Houses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _houseService.DeleteHouseAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
