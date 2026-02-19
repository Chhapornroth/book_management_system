using System.Diagnostics;
using WindowsFormsApp.Models;

namespace WindowsFormsApp.Strategy
{
    public class CashPaymentStrategy : IPaymentStrategy
    {
        public string Name => "Cash";

        public void ProcessPayment(Sale sale)
        {
            // In a real app this might open a cash drawer.
            Debug.WriteLine($"Processing CASH payment for sale #{sale.SaleId}, amount {sale.Total:C}.");
        }
    }
}


