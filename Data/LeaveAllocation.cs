using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Management.Data
{
    public class LeaveAllocation
    {
        [Key]
        public int Id { get; set; }

        public int NumberOfDays { get; set; }

        public DateTime DateCreated { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }

        public string EmployeeId { get; set; }

        [ForeignKey("LeaveTypesId")]
        public LeaveTypes LeaveTypes { get; set; }

        public int LeaveTypesId { get; set; }

    }
}
