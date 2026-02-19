using System;
using WindowsFormsApp.Models;

namespace WindowsFormsApp.Observer
{
    /// <summary>
    /// Simple observer that logs when a sale is created.
    /// </summary>
    public class LoggingSaleObserver : ISaleObserver
    {
        public void OnSaleCreated(Sale sale)
        {
            Console.WriteLine($"[Observer] Sale created for {sale.CustomerName}, total={sale.Total}");
        }
    }
}


