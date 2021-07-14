using AutoMapper;
using Leave_Management.Contracts;
using Leave_Management.Data;
using Leave_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Management.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LeaveAllocationController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public LeaveAllocationController(IUnitOfWork unitofwork, IMapper mapper, UserManager<Employee> userManager)
        {
            _unitofwork = unitofwork;
            _mapper = mapper;
            _userManager = userManager;
        }

        // GET: LeaveAllocationController
        public async Task<ActionResult> Index()
        {
            var leaveTypes = await _unitofwork.LeaveTypes.FindAll();
            var mappedLeaveTypes = _mapper.Map<List<LeaveTypes>, List<LeaveTypeVM>>(leaveTypes.ToList());
            var model = new CreateLeaveAllocationVM
            {
                LeaveTypes = mappedLeaveTypes,
                NumberUpdated = 0
            };
            return View(model);
        }

        public async Task<ActionResult> SetLeave(int id)
        {
            var leaveTypes = await _unitofwork.LeaveTypes.Find(q => q.Id == id);
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            foreach (var emp in employees)
            {
                //if (await _leaveallocationrepos.CheckAllocation(id, emp.Id))
                //    continue;
                var period = DateTime.Now.Year;
                if (await _unitofwork.LeaveAllocations.IsExists(q => q.Id == id && q.EmployeeId == emp.Id && q.Period == period))
                    continue;

                var allocation = new LeaveAllocationVM
                {
                    DateCreated = DateTime.Now,
                    EmployeeId = emp.Id,
                    LeaveTypesId = id,
                    NumberOfDays = leaveTypes.DefaultDays,
                    Period = DateTime.Now.Year

                };
                var leaveAllocation = _mapper.Map<LeaveAllocation>(allocation);
                //await _leaveallocationrepos.Create(leaveAllocation);
                await _unitofwork.LeaveAllocations.Create(leaveAllocation);
                await _unitofwork.Save();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> ListEmployees()
        {
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            var model = _mapper.Map<List<EmployeeVM>>(employees);
            return View(model);
        }

        // GET: LeaveAllocationController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var employee = _mapper.Map<EmployeeVM>(user);
            //var leavealloc = await _leaveallocationrepos.GetLeaveAllocationByEmployee(id);
            var period = DateTime.Now.Year;
            var records = await _unitofwork.LeaveAllocations.FindAll(expression: q => q.EmployeeId == id && q.Period == period,
                                                                     includes: q => q.Include(x => x.LeaveTypes));
            var allocations = _mapper.Map<List<LeaveAllocationVM>>(records);
            var model = new ViewAllocationVM
            {
                Employee = employee,
                LeaveAllocations = allocations
            };
            return View(model);
        }

        // GET: LeaveAllocationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveAllocationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveAllocationController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            //var model = _mapper.Map<EditLeaveAllocationVM>(await _leaveallocationrepos.FindById(id));
            var leavealloc = await _unitofwork.LeaveAllocations.Find(q => q.Id == id,
                                    includes: q => q.Include(x => x.Employee).Include(x => x.LeaveTypes));
            var model = _mapper.Map<EditLeaveAllocationVM>(leavealloc);
            return View(model);
        }

        // POST: LeaveAllocationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditLeaveAllocationVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                //var record = await _leaveallocationrepos.FindById(model.Id);
                var record = await _unitofwork.LeaveAllocations.Find(q => q.Id == model.Id);
                record.NumberOfDays = model.NumberOfDays;
                _unitofwork.LeaveAllocations.Update(record);
                await _unitofwork.Save();
                return RedirectToAction(nameof(Details), new { id = model.EmployeeId });
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveAllocationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveAllocationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
