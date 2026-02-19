using WindowsFormsApp.Models;

namespace WindowsFormsApp.Observer
{
    /// <summary>
    /// Observer Pattern:
    /// Observers are notified when a Sale is created.
    /// </summary>
    public interface ISaleObserver
    {
        void OnSaleCreated(Sale sale);
    }
}


