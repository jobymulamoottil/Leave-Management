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

        public async Task<bool> Create(LeaveRequest entity)
        {
            await _db.LeaveRequests.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveRequest entity)
        {
            _db.LeaveRequests.Remove(entity);
            return await Save();
        }

        public async Task<ICollection<LeaveRequest>> FindAll()
        {
            var leaveHistory = await _db.LeaveRequests
                                .Include(q => q.RequestingEmployee)
                                .Include(q => q.LeaveTypes)
                                .Include(q => q.ApprovedBy).OrderByDescending(q => q.DateRequested).ToListAsync();
            return leaveHistory;
        }

        public async Task<ICollection<LeaveRequest>> FindByEmployeeId(string employeeId)
        {
            var leaverequests = await FindAll();
            return leaverequests.Where(q => q.RequestingEmployeeId == employeeId).ToList();
        }

        public async Task<LeaveRequest> FindByEmployeeIdAndLeaveType(string employeeId, int leaverequestId)
        {
            var leaveHistory = await FindAll();
            return leaveHistory.FirstOrDefault(q => q.Id == leaverequestId && q.RequestingEmployeeId == employeeId);
        }

        public async Task<LeaveRequest> FindById(int id)
        {
            var leaveHistory = await _db.LeaveRequests
                                .Include(q => q.RequestingEmployee)
                                .Include(q => q.LeaveTypes)
                                .Include(q => q.ApprovedBy).FirstOrDefaultAsync(q => q.Id == id);
            return leaveHistory;
        }


        public async Task<bool> IsExists(int id)
        {
            var exists = await _db.LeaveRequests.AnyAsync(q => q.Id == id);
            return exists;
        }

        public async Task<bool> Save()
        {
            var result = await _db.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Update(LeaveRequest entity)
        {
            _db.LeaveRequests.Update(entity);
            return await Save();
        }
    }
}
