using Leave_Management.Contracts;
using Leave_Management.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Management.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IGenricRepository<LeaveTypes> _leaveTypes;
        private IGenricRepository<LeaveRequest> _leaveRequest;
        private IGenricRepository<LeaveAllocation> _leaveAllocations;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IGenricRepository<LeaveTypes> LeaveTypes => _leaveTypes ??= new GenericRepository<LeaveTypes>(_context);
        public IGenricRepository<LeaveRequest> LeaveRequests => _leaveRequest ??= new GenericRepository<LeaveRequest>(_context);
        public IGenricRepository<LeaveAllocation> LeaveAllocations => _leaveAllocations ??= new GenericRepository<LeaveAllocation>(_context);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool dispose)
        {
            if(dispose)
            {
                _context.Dispose();
            }
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
