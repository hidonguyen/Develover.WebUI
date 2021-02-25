using Develover.WebUI.Areas.Vehicle.Models;
using Develover.WebUI.Controllers;
using Develover.WebUI.DbContexts;
using Develover.WebUI.Entities;
using Develover.WebUI.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Areas.Vehicle.Controllers
{
    [Area("Vehicle")]
    [Route("vehicle/{controller=home}/{action=index}")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;

        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            VehicleMapViewModel model = new VehicleMapViewModel
            {
                Vehicles = _context.Vehicles.AsNoTracking().Select(x => new VehicleViewModel
                {
                    Id = x.Id,
                    RegistrationNo = x.RegistrationNo,
                    EngineNo = x.EngineNo,
                    DriverId = x.DefaultDriverId,
                    IsRepairing = x.IsRepairing,
                    IsDriving = x.IsDriving,
                    VehicleRepairId = x.CurrentVehicleRepairId,
                    VehicleScheduleId = x.CurrentVehicleScheduleId,
                }).ToList()
            };

            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            var modelSchedules = _context.VehicleSchedules.AsNoTracking().Where(s => !s.Complete && s.DepartureDate >= date).Select(s => new { s.VehicleId, s.Id });

            foreach (var modelSchedule in modelSchedules)
            {
                var vehicle = model.Vehicles.Where(v => v.Id == modelSchedule.VehicleId && v.VehicleScheduleId != modelSchedule.Id).FirstOrDefault();
                if (vehicle != null)
                    vehicle.IsScheduled = true;
            }

            ViewData["VehicleUsePurposes"] = _context.VehicleUsePurposes.OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
            ViewData["Employees"] = _context.Employees.OrderBy(c => c.FirstName).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.FirstName }).ToList();
            ViewData["Departments"] = _context.Departments.OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.Name }).ToList();
            ViewData["Scheduleds"] = new List<SelectListItem>() { new SelectListItem { Value = Guid.Empty.ToString(), Text = "No Schedule" } };

            return View(model);
        }

        public IActionResult GetInfoVehicle(string id)
        {
            _ = Guid.TryParse(id, out Guid entityId);
            var model = _context.Vehicles.Where(v => v.Id == entityId).AsNoTracking().FirstOrDefault();
            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"Vehicle not found");
            }
            VehicleInfoViewModel vehicleInfo = new VehicleInfoViewModel()
            {
                Id = model.Id,
                RegistrationNo = model.RegistrationNo,
                RegistrationDate = model.RegistrationDate,
                ManufactureDate = model.ManufactureDate,
                PurchaseDate = model.PurchaseDate,
                EngineNo = model.EngineNo,
                ChassisNo = model.ChassisNo,
                Brand = model.Brand,
                ModelNo = model.ModelNo,
                Color = model.Color,
                Capacity = model.Capacity,
                DefaultDriverId = model.DefaultDriverId,
                DefaultDriver = _context.Employees.Find(model.DefaultDriverId)?.FullName,
                DefaultDepartmentId = model.DefaultDepartmentId,
                DefaultDepartment = _context.Departments.Find(model.DefaultDepartmentId)?.Name,
                CurrentVehicleRepairId = model.CurrentVehicleRepairId,
                IsRepairing = model.IsRepairing,
                CurrentVehicleScheduleId = model.CurrentVehicleScheduleId,
                IsScheduled = false,
                IsDriving = model.IsDriving,
                IsScheduledText = "No",
                IsRepairingText = model.IsRepairing ? "Yes" : "No",
                IsDrivingText = model.IsDriving ? "Yes" : "No",
                Note = model.Note,
            };

            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            var modelSchedules = _context.VehicleSchedules.AsNoTracking().Where(s => s.DepartureDate >= date && s.VehicleId == entityId && !s.Complete && s.Id != model.CurrentVehicleScheduleId).FirstOrDefault();
            if (modelSchedules != null)
            {
                vehicleInfo.IsScheduled = true;
                vehicleInfo.IsScheduledText = "Yes";
            }

            return Json(new { vehicleInfo });
        }
        public IActionResult GetStatusAll()
        {
            var model = _context.Vehicles.AsNoTracking();

            if (model == null)
            {
                Response.StatusCode = 404;
                return Json($"Vehicle not found");
            }
            List<VehicleStatus> list = model.Select(x => new VehicleStatus
            {
                Id = x.Id,
                IsRepairing = x.IsRepairing,
                IsDriving = x.IsDriving,
                CurrentVehicleScheduleId = x.CurrentVehicleScheduleId
            }).ToList();

            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            var modelSchedules = _context.VehicleSchedules.AsNoTracking().Where(s => s.DepartureDate >= date && !s.Complete).Select(s => new { s.VehicleId, s.Id });

            foreach (var modelSchedule in modelSchedules)
            {
                var vehicle = list.Where(v => v.Id == modelSchedule.VehicleId && v.CurrentVehicleScheduleId != modelSchedule.Id).FirstOrDefault();
                if (vehicle != null)
                    vehicle.IsScheduled = true;
            }

            return Json(new { list });
        }
        public IActionResult UpdateStatusDriving(string idVehicle, string idScheduled, string idDriver, string idDepartment, string note)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _ = Guid.TryParse(idVehicle, out Guid vehicleId);
                var model = _context.Vehicles.Where(v => v.Id == vehicleId).FirstOrDefault();

                if (model == null)
                {
                    Response.StatusCode = 404;
                    return Json($"Vehicle not found");
                }

                if (model.IsRepairing)
                {
                    Response.StatusCode = 404;
                    return Json($"The vehicle is repairing, you cannot use it");
                }
                if (!model.IsDriving)
                {
                    _ = Guid.TryParse(idDriver, out Guid driverId);
                    var modelDriver = _context.Employees.AsNoTracking().Where(x => x.Id == driverId).FirstOrDefault();

                    _ = Guid.TryParse(idDepartment, out Guid departmentId);
                    var modelDepartment = _context.Departments.AsNoTracking().Where(x => x.Id == departmentId).FirstOrDefault();

                    _ = Guid.TryParse(idScheduled, out Guid scheduledId);
                    if (scheduledId != Guid.Empty)
                    {
                        var scheduled = _context.VehicleSchedules.Where(v => v.Id == scheduledId).FirstOrDefault();
                        if (scheduled == null)
                        {
                            Response.StatusCode = 404;
                            return Json($"Scheduled not found");
                        }
                        model.CurrentVehicleScheduleId = scheduled.Id;
                    }
                    model.DefaultDriverId = modelDriver?.Id;
                    model.DefaultDepartmentId = modelDepartment?.Id;
                    model.Note = note;
                }
                else
                {
                    CompeleteScheduled(idVehicle);
                    model.DefaultDriverId = null;
                    model.DefaultDepartmentId = null;
                    model.Note = "";
                }

                model.IsDriving = !model.IsDriving;
                _context.Vehicles.Update(model);
                _context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.RollbackAsync();
                Response.StatusCode = 500;
                return Json($"{ex.Message}");
            }

            return GetInfoVehicle(idVehicle);

        }
        public IActionResult GetScheduledInfo(string id)
        {
            _ = Guid.TryParse(id, out Guid scheduledId);
            if (scheduledId != Guid.Empty)
            {
                var scheduled = _context.VehicleSchedules.Where(v => v.Id == scheduledId).FirstOrDefault();
                if (scheduled == null)
                {
                    return Json(new { driverId = "", departmentId = "" });
                }
                return Json(new { driverId = scheduled.DriverId, departmentId = scheduled.DepartmentId });
            }
            return Json(new { driverId = "", departmentId = "" });

        }
        public IActionResult GetListScheduled(string idVehicle)
        {
            _ = Guid.TryParse(idVehicle, out Guid vehicleId);
            var model = _context.Vehicles.Where(v => v.Id == vehicleId).FirstOrDefault();
            var formatdate = _context.DeveloverSettings.Select(d => d.FormatDateCsharp).FirstOrDefault();

            List<SelectListItem> list = _context.VehicleSchedules.OrderBy(c => c.DepartureDate).Where(c => c.VehicleId == vehicleId && !c.Complete && c.DepartureDate.Year == DateTime.Now.Year && c.DepartureDate.Month == DateTime.Now.Month && c.DepartureDate.Day == DateTime.Now.Day)
                .Select(c => new SelectListItem { Value = c.Id.ToLowerString(), Text = c.DepartureDate.ToString(formatdate + " HH:mm:ss") + " /" + c.Origin + " /" + c.Destination + " /" + c.Petitioner.FullName }).ToList()
                .Prepend(new SelectListItem { Value = Guid.Empty.ToString(), Text = "No Schedule" }).ToList();

            return Json(new { list });
        }
        public IActionResult UpdateStatusScheduled(string idVehicle, DateTime departureDate, DateTime returnDate, string idDriver, string idPetitioner, string idDepartment, string destination, string idVehicleUsePurpose)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _ = Guid.TryParse(idVehicle, out Guid vehicleId);
                var vehicle = _context.Vehicles.AsNoTracking().Where(v => v.Id == vehicleId).FirstOrDefault();

                _ = Guid.TryParse(idDriver, out Guid driverId);
                var driver = _context.Employees.AsNoTracking().Where(v => v.Id == driverId).FirstOrDefault();

                _ = Guid.TryParse(idPetitioner, out Guid petitionerId);
                var petitioner = _context.Employees.AsNoTracking().Where(v => v.Id == petitionerId).FirstOrDefault();

                _ = Guid.TryParse(idDepartment, out Guid departmentId);
                var department = _context.Departments.AsNoTracking().Where(v => v.Id == departmentId).FirstOrDefault();

                _ = Guid.TryParse(idVehicleUsePurpose, out Guid vehicleUsePurposeId);
                var vehicleUsePurpose = _context.VehicleUsePurposes.AsNoTracking().Where(v => v.Id == vehicleUsePurposeId).FirstOrDefault();

                if (vehicle == null || vehicleUsePurpose == null)
                {
                    Response.StatusCode = 404;
                    return Json($"Please choose enough information");
                }
                if (departureDate < DateTime.Now || returnDate < DateTime.Now)
                {
                    Response.StatusCode = 404;
                    return Json($"Please select a date greater than or equal to the current date");
                }
                var modelSchedules = _context.VehicleSchedules.AsNoTracking().Where(s => s.VehicleId == vehicleId && ((s.DepartureDate >= departureDate && s.ReturnDate <= returnDate) || (s.DepartureDate <= departureDate && s.ReturnDate >= returnDate) || (s.DepartureDate <= departureDate && s.ReturnDate >= departureDate) || (s.DepartureDate <= returnDate && s.ReturnDate >= returnDate))).FirstOrDefault();
                if (modelSchedules != null)
                {
                    Response.StatusCode = 404;
                    return Json($"The schedule has been coincided, please check again?");
                }

                var vehicleSchedule = new VehicleSchedule
                {
                    Id = Guid.NewGuid(),
                    IssueDate = DateTime.Now,
                    No = new VehicleScheduleController(null, _context).GetNoVoucher(Guid.NewGuid()),
                    SequenceNo = _context.VehicleSchedules.OrderByDescending(d => d.SequenceNo).FirstOrDefault()?.SequenceNo + 1 ?? 1,
                    DepartureDate = departureDate,
                    ReturnDate = returnDate,
                    VehicleId = vehicle.Id,
                    DriverId = driver.Id,
                    PetitionerId = petitioner.Id,
                    DepartmentId = department.Id,
                    VehicleUsePurposeId = vehicleUsePurpose.Id,
                    Destination = destination
                };

                _context.VehicleSchedules.Add(vehicleSchedule);

                _context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.RollbackAsync();
                Response.StatusCode = 500;
                return Json($"{ex.Message}");
            }

            return GetInfoVehicle(idVehicle);
        }
        public IActionResult CompeleteScheduled(string idVehicle)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _ = Guid.TryParse(idVehicle, out Guid vehicleId);
                var vehicle = _context.Vehicles.Where(v => v.Id == vehicleId).FirstOrDefault();

                if (vehicle == null)
                {
                    Response.StatusCode = 404;
                    return Json($"Vehicle is not found");
                }

                var scheduled = _context.VehicleSchedules.Where(v => v.Id == vehicle.CurrentVehicleScheduleId).FirstOrDefault();
                if (scheduled != null)
                {
                    scheduled.Complete = true;
                    _context.VehicleSchedules.Update(scheduled);
                }

                vehicle.CurrentVehicleScheduleId = null;
                vehicle.IsDriving = false;

                _context.Vehicles.Update(vehicle);
                _context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.RollbackAsync();
                Response.StatusCode = 500;
                return Json($"{ex.Message}");
            }

            return GetInfoVehicle(idVehicle);
        }
        public IActionResult UpdateStatusRepairing(string idVehicle, string idDriver, string repairShop, DateTime estimatedCompletionDate, double estimatedRepairCost)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _ = Guid.TryParse(idVehicle, out Guid vehicleId);
                var vehicle = _context.Vehicles.Where(v => v.Id == vehicleId).FirstOrDefault();

                _ = Guid.TryParse(idDriver, out Guid driverId);
                var driver = _context.Employees.AsNoTracking().Where(v => v.Id == driverId).FirstOrDefault();

                if (vehicle == null)
                {
                    Response.StatusCode = 404;
                    return Json($"Please choose enough information");
                }
                if (vehicle.IsDriving)
                {
                    Response.StatusCode = 404;
                    return Json($"The vehicle is driving, you cannot take the car for repair");
                }

                if (_context.VehicleRepairs.AsNoTracking().Where(x => x.VehicleId == vehicleId && x.Complete == false).Any())
                {
                    Response.StatusCode = 404;
                    return Json($"The vehicle is repairing, you cannot take the car for repair");
                }

                var vehicleRepair = new VehicleRepair
                {
                    Id = Guid.NewGuid(),
                    IssueDate = DateTime.Now,
                    VehicleId = vehicleId,
                    No = new VehicleRepairController(null, _context).GetNoVoucher(Guid.NewGuid()),
                    SequenceNo = _context.VehicleSchedules.OrderByDescending(d => d.SequenceNo).FirstOrDefault()?.SequenceNo + 1 ?? 1,
                    DriverId = driver.Id,
                    RepairShop = repairShop,
                    EstimatedCompletionDate = estimatedCompletionDate,
                    EstimatedRepairCost = estimatedRepairCost
                };


                vehicle.IsRepairing = true;
                vehicle.CurrentVehicleRepairId = vehicleRepair.Id;
                _context.VehicleRepairs.Add(vehicleRepair);

                _context.SaveChanges();
                transaction.Commit();

            }
            catch (Exception ex)
            {
                transaction.RollbackAsync();
                Response.StatusCode = 500;
                return Json($"{ex.Message}");
            }

            return GetInfoVehicle(idVehicle);
        }
        public IActionResult CompeleteRepairing(string idVehicle)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _ = Guid.TryParse(idVehicle, out Guid vehicleId);
                var vehicle = _context.Vehicles.Where(v => v.Id == vehicleId).FirstOrDefault();


                if (vehicle == null)
                {
                    Response.StatusCode = 404;
                    return Json($"Vehicle is not found");
                }
                if (!vehicle.IsRepairing)
                {
                    Response.StatusCode = 404;
                    return Json($"Vehicle is not Repairing");

                }

                var vehicleRepair = _context.VehicleRepairs.Where(v => v.Id == vehicle.CurrentVehicleRepairId).FirstOrDefault();
                if (vehicleRepair == null)
                {
                    Response.StatusCode = 404;
                    return Json($"Vehicle Repair is not found");
                }
                vehicle.IsRepairing = false;
                vehicle.CurrentVehicleRepairId = null;
                vehicleRepair.Complete = true;

                _context.VehicleRepairs.Update(vehicleRepair);
                _context.Vehicles.Update(vehicle);

                _context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.RollbackAsync();
                Response.StatusCode = 500;
                return Json($"{ex.Message}");
            }

            return GetInfoVehicle(idVehicle);
        }
    }
}
