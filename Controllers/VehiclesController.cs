using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoStats.Data;
using AutoStats.Models;

namespace AutoStats.Controllers
{
    [Authorize]
    public class VehiclesController : Controller
    {
        private readonly AutoStatsContext _context;
        private readonly UserManager<AutoStatsUser> _userManager;

        public VehiclesController(AutoStatsContext context, UserManager<AutoStatsUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //  Metoda GET: Vozila
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var vehicles = await _context.Vehicles
                .Where(v => v.UserId == userId)
                .ToListAsync();

            return View(vehicles);
        }

        // Metoda GET: Detalji o vozilu
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == id && v.UserId == userId);

            if (vehicle == null) return NotFound();

            return View(vehicle);
        }

        // Metoda GET: Dodaj vozilo
        public IActionResult Create()
        {
            return View();
        }

        // Metoda POST: Dodaj vozilo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vehicle vehicle)
        {
            ModelState.Remove("UserId");

            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            if (ModelState.IsValid)
            {
                vehicle.UserId = userId;
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(vehicle);
        }

        // Metoda GET: Edituj vozilo
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == id && v.UserId == userId);

            if (vehicle == null) return NotFound();

            return View(vehicle);
        }

        // Metoda POST: Edituj vozilo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Make,Model,RegistrationNumber,Year,ChassisNumber,EngineDisplacement,PowerKW")] Vehicle vehicle)
        {
            if (id != vehicle.Id) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var vehicleToUpdate = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == id && v.UserId == userId);

            if (vehicleToUpdate == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    vehicleToUpdate.Make = vehicle.Make;
                    vehicleToUpdate.Model = vehicle.Model;
                    vehicleToUpdate.RegistrationNumber = vehicle.RegistrationNumber;
                    vehicleToUpdate.Year = vehicle.Year;
                    vehicleToUpdate.ChassisNumber = vehicle.ChassisNumber;
                    vehicleToUpdate.EngineDisplacement = vehicle.EngineDisplacement;
                    vehicleToUpdate.PowerKW = vehicle.PowerKW;

                    _context.Update(vehicleToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(vehicle);
        }

        // Metoda GET: Obriši vozilo
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == id && v.UserId == userId);

            if (vehicle == null) return NotFound();

            return View(vehicle);
        }

        // Metoda POST: Obriši vozilo
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == id && v.UserId == userId);

            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicles.Any(e => e.Id == id);
        }
    }
}
