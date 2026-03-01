using WindowsFormsApp.Models;

namespace WindowsFormsApp.Observer
{
    /// <summary>
    /// Observer Pattern:
    /// Observers are notified when Employee operations occur (add, update, delete).
    /// </summary>
    public interface IEmployeeObserver
    {
        void OnEmployeeAdded(Employee employee);
        void OnEmployeeUpdated(Employee employee);
        void OnEmployeeDeleted(int employeeId);
    }
}

