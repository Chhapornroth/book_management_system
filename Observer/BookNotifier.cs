using System.Collections.Generic;
using WindowsFormsApp.Models;

namespace WindowsFormsApp.Observer
{
    /// <summary>
    /// Observable subject that notifies observers when book operations occur.
    /// </summary>
    public class BookNotifier
    {
        private readonly List<IBookObserver> _observers = new();

        public void Attach(IBookObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        public void Detach(IBookObserver observer)
        {
            _observers.Remove(observer);
        }

        public void NotifyBookAdded(Book book)
        {
            foreach (var observer in _observers)
            {
                observer.OnBookAdded(book);
            }
        }

        public void NotifyBookUpdated(Book book)
        {
            foreach (var observer in _observers)
            {
                observer.OnBookUpdated(book);
            }
        }

        public void NotifyBookDeleted(int bookId)
        {
            foreach (var observer in _observers)
            {
                observer.OnBookDeleted(bookId);
            }
        }
    }
}

