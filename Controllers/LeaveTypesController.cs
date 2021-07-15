using AutoMapper;
using Leave_Management.Contracts;
using Leave_Management.Data;
using Leave_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Management.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LeaveTypesController : Controller
    {
        private readonly ILeaveTypeRepository _repos;
        private readonly IUnitOfWork _unitofwork;
        private readonly IMapper _mapper;

        public LeaveTypesController(ILeaveTypeRepository repos, IUnitOfWork unitofwork, IMapper mapper)
        {
            _repos = repos;
            _unitofwork = unitofwork;
            _mapper = mapper;
        }


        public async Task<ActionResult> Index()
        {
            //var leaveTypes_temp = await _repos.FindAll();
            var leaveTypes_temp = await _unitofwork.LeaveTypes.FindAll();
            var leaveTypes = leaveTypes_temp.ToList();
            var model = _mapper.Map<List<LeaveTypes>, List<LeaveTypeVM>>(leaveTypes);
            return View(model);
        }

        // GET: LeaveTypesController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            //var IsExists = await _repos.IsExists(id);
            var IsExists = await _unitofwork.LeaveTypes.IsExists(q => q.Id == id);
            if (!IsExists)
            {
                return NotFound();
            }
            //var leaveType = await _repos.FindById(id);
            var leaveType = await _unitofwork.LeaveTypes.Find(q => q.Id == id);
            var model = _mapper.Map<LeaveTypeVM>(leaveType);
            return View(model);
        }

        // GET: LeaveTypesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveTypesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(LeaveTypeVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var leaveType = _mapper.Map<LeaveTypes>(model);
                leaveType.DateCreated = DateTime.Now;

                //var is_success = await _repos.Create(leaveType);
                await _unitofwork.LeaveTypes.Create(leaveType);
                await _unitofwork.Save();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong...");
                return View(model);
            }
        }

        // GET: LeaveTypesController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var IsExists = await _unitofwork.LeaveTypes.IsExists(q => q.Id == id);
            if (!IsExists)
            {
                return NotFound();
            }
            var leaveTypes = await _unitofwork.LeaveTypes.Find(q => q.Id == id);
            var model = _mapper.Map<LeaveTypeVM>(leaveTypes);
            return View(model);
        }

        // POST: LeaveTypesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(LeaveTypeVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var leaveType = _mapper.Map<LeaveTypes>(model);
                _unitofwork.LeaveTypes.Update(leaveType);
                await _unitofwork.Save();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong...");
                return View(model);
            }
        }

        // POST: LeaveTypesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var leaveTypes = await _unitofwork.LeaveTypes.Find(q => q.Id == id);
                if (leaveTypes == null)
                {
                    return NotFound();
                }
                _unitofwork.LeaveTypes.Delete(leaveTypes);
                await _unitofwork.Save();

            }
            catch
            {
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
