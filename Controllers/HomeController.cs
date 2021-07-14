using AutoMapper;
using Leave_Management.Contracts;
using Leave_Management.Data;
using Leave_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IUnitOfWork _unitofwork;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitofwork, IMapper mapper, UserManager<Employee> userManager)
        {
            _logger = logger;
            _unitofwork = unitofwork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var leaveRequests = await _unitofwork.LeaveRequests.FindAll(includes: q => q
                                                                .Include(x => x.LeaveTypes)
                                                                .Include(x => x.RequestingEmployee));
            var leaveRequestModel = _mapper.Map<List<LeaveRequestVM>>(leaveRequests);
            var model = new HomeVM
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
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
