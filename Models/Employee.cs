using System;

namespace WindowsFormsApp.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public string Role { get; set; } = "Cashier";
    }
}


