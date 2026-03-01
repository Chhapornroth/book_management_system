using System.Diagnostics;
using WindowsFormsApp.Models;

namespace WindowsFormsApp.Observer
{
    /// <summary>
    /// Observer that logs when employee operations occur.
    /// </summary>
    public class LoggingEmployeeObserver : IEmployeeObserver
    {
        public void OnEmployeeAdded(Employee employee)
        {
            Debug.WriteLine($"[Observer] Employee added: ID={employee.EmployeeId}, Name='{employee.Name}', Role={employee.Role}");
        }

        public void OnEmployeeUpdated(Employee employee)
        {
            Debug.WriteLine($"[Observer] Employee updated: ID={employee.EmployeeId}, Name='{employee.Name}', Role={employee.Role}");
        }

        public void OnEmployeeDeleted(int employeeId)
        {
            Debug.WriteLine($"[Observer] Employee deleted: ID={employeeId}");
        }
    }
}

