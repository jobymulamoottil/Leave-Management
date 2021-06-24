﻿using AutoMapper;
using Leave_Management.Data;
using Leave_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Management.Mappings
{
    public class Maps : Profile
    {
        public Maps()
        {
            CreateMap<LeaveTypes, DetailsLeaveTypeVM>().ReverseMap();

            CreateMap<LeaveTypes, CreateLeaveTypeVM>().ReverseMap();

            CreateMap<Employee, EmployeeVM>().ReverseMap();

            CreateMap<LeaveHistory, LeaveHistoryVM>().ReverseMap();

            CreateMap<LeaveAllocation, LeaveAllocationVM>().ReverseMap();
        }
    }
}