using WindowsFormsApp.Models;

namespace WindowsFormsApp.Strategy
{
    /// <summary>
    /// Strategy Pattern:
    /// Different payment strategies (cash, card, etc.).
    /// </summary>
    public interface IPaymentStrategy
    {
        string Name { get; }
        void ProcessPayment(Sale sale);
    }
}


