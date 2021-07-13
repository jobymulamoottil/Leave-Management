using Leave_Management.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Management.Contracts
{
    public interface ILeaveRequestRepository : IRepositoryBase<LeaveRequest>
    {
        ICollection<LeaveRequest> FindByEmployeeId(string employeeId);

        LeaveRequest FindByEmployeeIdAndLeaveType(string employeeId, int leaverequestId);
    }
}
