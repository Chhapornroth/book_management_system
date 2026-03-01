using WindowsFormsApp.Models;

namespace WindowsFormsApp.Observer
{
    /// <summary>
    /// Observer Pattern:
    /// Observers are notified when Book operations occur (add, update, delete).
    /// </summary>
    public interface IBookObserver
    {
        void OnBookAdded(Book book);
        void OnBookUpdated(Book book);
        void OnBookDeleted(int bookId);
    }
}

