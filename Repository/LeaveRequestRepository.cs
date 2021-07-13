using Leave_Management.Contracts;
using Leave_Management.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Management.Repository
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveRequestRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool Create(LeaveRequest entity)
        {
            _db.LeaveRequests.Add(entity);
            return Save();
        }

        public bool Delete(LeaveRequest entity)
        {
            _db.LeaveRequests.Remove(entity);
            return Save();
        }

        public ICollection<LeaveRequest> FindAll()
        {
            var leaveHistory = _db.LeaveRequests
                                .Include(q => q.RequestingEmployee)
                                .Include(q => q.LeaveTypes)
                                .Include(q => q.ApprovedBy).OrderByDescending(q => q.DateRequested).ToList();
            return leaveHistory;
        }

        public ICollection<LeaveRequest> FindByEmployeeId(string employeeId)
        {
            var leaverequests = FindAll().Where(q => q.RequestingEmployeeId == employeeId).ToList();
            return leaverequests;
        }

        public LeaveRequest FindByEmployeeIdAndLeaveType(string employeeId, int leaverequestId)
        {
            var leaveHistory = FindAll().FirstOrDefault(q => q.Id == leaverequestId && q.RequestingEmployeeId==employeeId);
            return leaveHistory;
        }

        public LeaveRequest FindById(int id)
        {
            var leaveHistory = _db.LeaveRequests
                                .Include(q => q.RequestingEmployee)
                                .Include(q => q.LeaveTypes)
                                .Include(q => q.ApprovedBy).FirstOrDefault(q => q.Id == id);
            return leaveHistory;
        }


        public bool IsExists(int id)
        {
            var exists = _db.LeaveRequests.Any(q => q.Id == id);
            return exists;
        }

        public bool Save()
        {
            var result = _db.SaveChanges();
            return result > 0;
        }

        public bool Update(LeaveRequest entity)
        {
            _db.LeaveRequests.Update(entity);
            return Save();
        }
    }
}
