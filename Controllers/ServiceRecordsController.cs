using AutoStats.Data;
using AutoStats.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Security.Claims;
using System.IO;
using System.Linq;

namespace AutoStats.Controllers
{
    [Authorize]
    public class ServiceRecordsController : Controller
    {
        private readonly AutoStatsContext _context;
        private readonly UserManager<AutoStatsUser> _userManager;

        public ServiceRecordsController(AutoStatsContext context, UserManager<AutoStatsUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Create(int vehicleId)
        {
            var record = new ServiceRecord
            {
                VehicleId = vehicleId,
                ServiceDate = DateTime.Today
            };

            ViewBag.VehicleId = vehicleId;
            ViewBag.ServiceTypes = new SelectList(new[]
            {
                "Mali servis", "Veliki servis", "Zamjena guma", "Punjenje klime", "Ostalo"
            });

            return View(record);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRecord serviceRecord)
        {
            if (serviceRecord.VehicleId == 0)
            {
                ModelState.AddModelError("VehicleId", "Vozilo nije odabrano.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(serviceRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction("ByVehicle", new { vehicleId = serviceRecord.VehicleId });
            }

            ViewBag.VehicleId = serviceRecord.VehicleId;
            ViewBag.ServiceTypes = new SelectList(new[]
            {
                "Mali servis", "Veliki servis", "Zamjena guma", "Punjenje klime", "Ostalo"
            });

            return View(serviceRecord);
        }

        public async Task<IActionResult> ByVehicle(int vehicleId, string? filter)
        {
            var userId = _userManager.GetUserId(User);
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == vehicleId && v.UserId == userId);

            if (vehicle == null)
            {
                return NotFound();
            }

            var query = _context.ServiceRecords.Where(r => r.VehicleId == vehicleId);

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(r => r.ServiceType == filter);
            }

            var records = await query
                .OrderByDescending(r => r.ServiceDate)
                .ToListAsync();

            var serviceTypes = await _context.ServiceRecords
                .Where(r => r.VehicleId == vehicleId)
                .Select(r => r.ServiceType)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            ViewBag.VehicleId = vehicleId;
            ViewBag.VehicleInfo = $"{vehicle.Make} {vehicle.Model} ({vehicle.RegistrationNumber})";
            ViewBag.ServiceTypes = serviceTypes;
            ViewBag.SelectedFilter = filter;

            return View(records);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var serviceRecord = await _context.ServiceRecords.FindAsync(id);
            if (serviceRecord == null)
            {
                return NotFound();
            }

            ViewBag.ServiceTypes = new SelectList(new[]
            {
                "Mali servis", "Veliki servis", "Zamjena guma", "Punjenje klime", "Ostalo"
            });

            return View(serviceRecord);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceRecord serviceRecord)
        {
            if (id != serviceRecord.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ServiceRecords.Any(e => e.Id == serviceRecord.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("ByVehicle", new { vehicleId = serviceRecord.VehicleId });
            }

            ViewBag.ServiceTypes = new SelectList(new[]
            {
                "Mali servis", "Veliki servis", "Zamjena guma", "Punjenje klime", "Ostalo"
            });

            return View(serviceRecord);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var serviceRecord = await _context.ServiceRecords
                .Include(s => s.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (serviceRecord == null)
            {
                return NotFound();
            }

            return View(serviceRecord);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceRecord = await _context.ServiceRecords.FindAsync(id);
            int vehicleId = serviceRecord.VehicleId;

            _context.ServiceRecords.Remove(serviceRecord);
            await _context.SaveChangesAsync();

            return RedirectToAction("ByVehicle", new { vehicleId });
        }

        [HttpGet]
        public async Task<IActionResult> ExportToPdf(int vehicleId)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var userId = _userManager.GetUserId(User);

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == vehicleId && v.UserId == userId);

            if (vehicle == null) return NotFound();

            var services = await _context.ServiceRecords
                .Where(s => s.VehicleId == vehicleId)
                .OrderBy(s => s.ServiceDate) // Najstariji → najnoviji
                .ToListAsync();

            var totalCost = services.Sum(s => s.Cost);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Column(col =>
                        {
                            col.Item().AlignCenter().Text($"Evidencija servisa za {vehicle.Make} {vehicle.Model} ({vehicle.RegistrationNumber})")
                                .SemiBold().FontSize(16).FontColor(Colors.Blue.Medium);

                            col.Item().Height(25); // Razmak ispod naslova
                        });

                    page.Content().Column(col =>
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Datum");
                                header.Cell().Element(CellStyle).Text("Tip");
                                header.Cell().Element(CellStyle).Text("Kilometraža");
                                header.Cell().Element(CellStyle).Text("Cijena (KM)");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container
                                        .DefaultTextStyle(x => x.SemiBold())
                                        .PaddingVertical(5)
                                        .BorderBottom(1)
                                        .BorderColor(Colors.Grey.Lighten2);
                                }
                            });

                            foreach (var s in services)
                            {
                                table.Cell().Element(CellContent).Text(s.ServiceDate.ToShortDateString());
                                table.Cell().Element(CellContent).Text(s.ServiceType);
                                table.Cell().Element(CellContent).Text(s.Mileage.ToString());
                                table.Cell().Element(CellContent).Text(s.Cost.ToString("0.00"));
                            }

                            static IContainer CellContent(IContainer container)
                            {
                                return container
                                    .PaddingVertical(5)
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten3);
                            }
                        });

                        col.Item().Height(20); // ✅ razmak prije ukupno

                        col.Item().Row(row =>
                        {
                            row.RelativeColumn().AlignRight().Text(txt =>
                            {
                                txt.Span("Ukupno:")
                                    .SemiBold().FontColor(Colors.Blue.Medium);
                            });

                            row.ConstantColumn(100).Text(txt =>
                            {
                                txt.Span($"{totalCost:0.00} KM")
                                    .SemiBold().FontColor(Colors.Blue.Medium);
                            });
                        });
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(txt =>
                        {
                            txt.Span("AutoStats aplikacija • ").FontSize(10);
                            txt.Span(DateTime.Now.ToString("dd.MM.yyyy")).FontSize(10);
                        });
                });
            });

            var stream = new MemoryStream();
            document.GeneratePdf(stream);
            stream.Position = 0;

            string filename = $"servisi_{vehicle.RegistrationNumber}_{DateTime.Now:yyyyMMdd}.pdf";

            return File(stream, "application/pdf", filename);
        }



    }
}
