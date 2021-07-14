using AutoMapper;
using Leave_Management.Contracts;
using Leave_Management.Data;
using Leave_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Management.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {

        private readonly ILeaveAllocationRepository _leaveAllocationrepos;
        private readonly ILeaveTypeRepository _leaverepos;
        private readonly ILeaveRequestRepository _leaveRequestRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public LeaveRequestController(ILeaveRequestRepository leaveRequestRepo, ILeaveAllocationRepository leaveAllocationrepos,
            ILeaveTypeRepository leaverepo, IMapper mapper, UserManager<Employee> userManager)
        {
            _leaveAllocationrepos = leaveAllocationrepos;
            _leaverepos = leaverepo;
            _leaveRequestRepo = leaveRequestRepo;
            _mapper = mapper;
            _userManager = userManager;
        }

        [Authorize(Roles = "Administrator")]
        // GET: LeaveRequestController
        public async Task<ActionResult> Index()
        {
            var leaveRequests = await _leaveRequestRepo.FindAll();
            var leaveRequestModel = _mapper.Map<List<LeaveRequestVM>>(leaveRequests);
            var model = new AdminLeaveRequestViewVM
            {
                TotalRequests = leaveRequestModel.Count,
                ApprovedRequests = leaveRequestModel.Count(q => q.Approved == true),
                PendingRequests = leaveRequestModel.Count(q => q.Approved == null),
                RejectedRequests = leaveRequestModel.Count(q => q.Approved == false),
                LeaveRequests = leaveRequestModel
            };
            return View(model);
        }

        // GET: LeaveRequestController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var leaveRequest = await _leaveRequestRepo.FindById(id);
            var model = _mapper.Map<LeaveRequestVM>(leaveRequest);
            return View(model);
        }

        public async Task<ActionResult> ApproveRequest(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var leaveRequest = await _leaveRequestRepo.FindById(id);
                var employeeid = leaveRequest.RequestingEmployeeId;
                var leavetypeid = leaveRequest.LeaveTypesId;
                var allocation = await _leaveAllocationrepos.GetLeaveAllocationByEmployeeAndType(employeeid, leavetypeid);
                var startdate = Convert.ToDateTime(leaveRequest.StartDate);
                var enddate = Convert.ToDateTime(leaveRequest.EndDate);
                int dayesRequested = (int)(enddate.Date - startdate.Date).TotalDays;
                allocation.NumberOfDays = allocation.NumberOfDays - dayesRequested;
                leaveRequest.ApprovedBy = user;
                leaveRequest.Approved = true;
                leaveRequest.DateActioned = DateTime.Now;
                leaveRequest.ApprovedById = user.Id;
                await _leaveRequestRepo.Update(leaveRequest);
                await _leaveAllocationrepos.Update(allocation);

                return RedirectToAction(nameof(Index), "LeaveRequest");
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index), "LeaveRequest");
            }
        }

        public async Task<ActionResult> RejectRequest(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var leaveRequest = await _leaveRequestRepo.FindById(id);
                leaveRequest.Approved = false;
                leaveRequest.DateActioned = DateTime.Now;
                leaveRequest.ApprovedBy = user;
                leaveRequest.ApprovedById = user.Id;
                var IsSuccess = await _leaveRequestRepo.Update(leaveRequest);
                if (!IsSuccess)
                {
                    return RedirectToAction(nameof(Index), "LeaveRequest");
                }
                return RedirectToAction(nameof(Index), "LeaveRequest");
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index), "LeaveRequest");
            }
        }

        // GET: LeaveRequestController/Create
        public async Task<ActionResult> Create()
        {
            var leaveTypes = await _leaverepos.FindAll();
            var leaveTypeItems = leaveTypes.Select(q => new SelectListItem
            {
                Text = q.Name,
                Value = q.Id.ToString()
            });
            var model = new CreateLeaveRequestVM
            {
                LeaveTypes = leaveTypeItems
            };
            return View(model);
        }

        // POST: LeaveRequestController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateLeaveRequestVM model)
        {
            try
            {
                var startdate = Convert.ToDateTime(model.StartDate);
                var enddate = Convert.ToDateTime(model.EndDate);

                var leaveTypes = await _leaverepos.FindAll();
                var leaveTypeItems = leaveTypes.Select(q => new SelectListItem
                {
                    Text = q.Name,
                    Value = q.Id.ToString()
                });
                model.LeaveTypes = leaveTypeItems;
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                if (DateTime.Compare(startdate, enddate) > 1)
                {
                    ModelState.AddModelError("", "Start Date Cannot be Further in the future than the End Date...");
                    return View(model);
                }
                var employee = await _userManager.GetUserAsync(User);
                var allocation = await _leaveAllocationrepos.GetLeaveAllocationByEmployeeAndType(employee.Id, model.LeaveTypeId);
                int dayesRequested = (int)(enddate.Date - startdate.Date).TotalDays;

                if (dayesRequested > allocation.NumberOfDays)
                {
                    ModelState.AddModelError("", "You do not have sufficient Days...");
                    return View(model);
                }

                var leaveRequestModel = new LeaveRequestVM
                {
                    RequestingEmployeeId = employee.Id,
                    StartDate = startdate,
                    EndDate = enddate,
                    Approved = null,
                    DateRequested = DateTime.Now,
                    DateActioned = DateTime.Now,
                    LeaveTypesId = model.LeaveTypeId,
                    Cancelled = false,
                    RequestComments = model.RequestComments
                };
                var leaveRequest = _mapper.Map<LeaveRequest>(leaveRequestModel);
                var IsSuccess = await _leaveRequestRepo.Create(leaveRequest);
                if (!IsSuccess)
                {
                    ModelState.AddModelError("", "Something went wrong while submitting...");
                    return View(model);
                }

                return RedirectToAction("MyLeave");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Something went wrong...");
                return View(model);
            }
        }

        // GET: LeaveRequestController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: LeaveRequestController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Delete/5
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

        public async Task<IActionResult> MyLeave()
        {
            var employee = await _userManager.GetUserAsync(User);
            var employeeid = employee.Id;
            var employeeleavealloc = await _leaveAllocationrepos.GetLeaveAllocationByEmployee(employeeid);
            var employeeleaverequests = await _leaveRequestRepo.FindByEmployeeId(employeeid);

            var employeeleaveallocModel = _mapper.Map<List<LeaveAllocationVM>>(employeeleavealloc);
            var employeeleaverequestsModel = _mapper.Map<List<LeaveRequestVM>>(employeeleaverequests);
            var model = new EmployeeLeaveRequestVM
            {
                LeaveAllocations = employeeleaveallocModel,
                LeaveRequests = employeeleaverequestsModel
            };

            return View(model);
        }

        public async Task<ActionResult> CancelRequest(int id)
        {
            //var employee = _userManager.GetUserAsync(User).Result;
            //var employeeid = employee.Id;
            //var leaverequest = _leaveRequestRepo.FindByEmployeeIdAndLeaveType(employeeid, requestid);
            var leaveRequest = await _leaveRequestRepo.FindById(id);
            leaveRequest.Cancelled = true;
            await _leaveRequestRepo.Update(leaveRequest);
            return RedirectToAction(nameof(MyLeave));
        }
    }
}
