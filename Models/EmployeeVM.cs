using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Management.Models
{
    public class EmployeeVM
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        [Display(Name = "Email ID")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string TaxId { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime DateofBirth { get; set; }

        [Display(Name = "Date Joined")]
        public DateTime DateJoined { get; set; }
    }
}
