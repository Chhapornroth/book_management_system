using System.Collections.Generic;
using WindowsFormsApp.Models;

namespace WindowsFormsApp.Observer
{
    /// <summary>
    /// Observable subject that notifies observers when a sale is created.
    /// </summary>
    public class SaleNotifier
    {
        private readonly List<ISaleObserver> _observers = new();

        public void Attach(ISaleObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        public void Detach(ISaleObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify(Sale sale)
        {
            foreach (var observer in _observers)
            {
                observer.OnSaleCreated(sale);
            }
        }
    }
}


