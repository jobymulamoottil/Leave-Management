using Leave_Management.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Management.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IGenricRepository<LeaveTypes> LeaveTypes { get; }

        IGenricRepository<LeaveRequest> LeaveRequests { get; }

        IGenricRepository<LeaveAllocation> LeaveAllocations { get; }

        Task Save();
    }
}
