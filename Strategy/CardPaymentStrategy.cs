using System.Diagnostics;
using WindowsFormsApp.Models;

namespace WindowsFormsApp.Strategy
{
    public class CardPaymentStrategy : IPaymentStrategy
    {
        public string Name => "Card";

        public void ProcessPayment(Sale sale)
        {
            // In a real app this would integrate with a card terminal / gateway.
            Debug.WriteLine($"Processing CARD payment for sale #{sale.SaleId}, amount {sale.Total:C}.");
        }
    }
}


