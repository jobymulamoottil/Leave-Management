using AutoMapper;
using Leave_Management.Contracts;
using Leave_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Management.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILeaveTypeRepository _leavetyperepos;
        private readonly ILeaveRequestRepository _leaveRequestRepo;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, ILeaveTypeRepository leavetyperepos, 
                              IMapper mapper, ILeaveRequestRepository leaveRequestRepo)
        {
            _logger = logger;
            _leavetyperepos = leavetyperepos;
            _leaveRequestRepo = leaveRequestRepo;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var leaveRequests = _leaveRequestRepo.FindAll();
            var leaveRequestModel = _mapper.Map<List<LeaveRequestVM>>(leaveRequests);
            var model = new HomeVM
            {
                TotalRequests = leaveRequestModel.Count,
                ApprovedRequests = leaveRequestModel.Count(q => q.Approved == true),
                PendingRequests = leaveRequestModel.Count(q => q.Approved == null),
                RejectedRequests = leaveRequestModel.Count(q => q.Approved == false),
            };
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
