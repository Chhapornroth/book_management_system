using System.Collections.Generic;
using WindowsFormsApp.Models;

namespace WindowsFormsApp.Observer
{
    /// <summary>
    /// Observable subject that notifies observers when employee operations occur.
    /// </summary>
    public class EmployeeNotifier
    {
        private readonly List<IEmployeeObserver> _observers = new();

        public void Attach(IEmployeeObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        public void Detach(IEmployeeObserver observer)
        {
            _observers.Remove(observer);
        }

        public void NotifyEmployeeAdded(Employee employee)
        {
            foreach (var observer in _observers)
            {
                observer.OnEmployeeAdded(employee);
            }
        }

        public void NotifyEmployeeUpdated(Employee employee)
        {
            foreach (var observer in _observers)
            {
                observer.OnEmployeeUpdated(employee);
            }
        }

        public void NotifyEmployeeDeleted(int employeeId)
        {
            foreach (var observer in _observers)
            {
                observer.OnEmployeeDeleted(employeeId);
            }
        }
    }
}

