using System;
using System.Diagnostics;
using WindowsFormsApp.Models;

namespace WindowsFormsApp.Observer
{
    /// <summary>
    /// Observer that logs when book operations occur.
    /// </summary>
    public class LoggingBookObserver : IBookObserver
    {
        public void OnBookAdded(Book book)
        {
            Debug.WriteLine($"[Observer] Book added: ID={book.BookId}, Title='{book.Title}', Stock={book.Stock}");
        }

        public void OnBookUpdated(Book book)
        {
            Debug.WriteLine($"[Observer] Book updated: ID={book.BookId}, Title='{book.Title}', Stock={book.Stock}");
        }

        public void OnBookDeleted(int bookId)
        {
            Debug.WriteLine($"[Observer] Book deleted: ID={bookId}");
        }
    }
}

