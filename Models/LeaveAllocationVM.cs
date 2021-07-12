using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Management.Models
{
    public class LeaveAllocationVM
    {
        public int Id { get; set; }

        [Display(Name = "Number of Days")]
        public int NumberOfDays { get; set; }

        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        public int Period { get; set; }

        public EmployeeVM Employee { get; set; }

        public string EmployeeId { get; set; }

        public LeaveTypeVM LeaveTypes { get; set; }

        public int LeaveTypesId { get; set; }


    }

    public class CreateLeaveAllocationVM
    {
        public int NumberUpdated { get; set; }

        public List<LeaveTypeVM> LeaveTypes { get; set; }
    }

    public class ViewAllocationVM
    {
        public EmployeeVM Employee { get; set; }

        public string EmployeeId { get; set; }

        public List<LeaveAllocationVM> LeaveAllocations { get; set; }
    }

    public class EditLeaveAllocationVM
    {
        public int Id { get; set; }

        public LeaveTypeVM LeaveTypes { get; set; }

        public EmployeeVM Employee { get; set; }

        public string EmployeeId { get; set; }

        [Display(Name = "Number of Days")]
        public int NumberOfDays { get; set; }

    }
}
