using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using WindowsFormsApp.Models;
using WindowsFormsApp.Singleton;

namespace WindowsFormsApp.Data
{
    /// <summary>
    /// Data Access Layer for Books
    /// Uses parameterized queries for security
    /// </summary>
    public class BookRepository
    {
        public List<Book> GetAllBooks()
        {
            var books = new List<Book>();
            using var conn = DbConnectionManager.Instance.CreateConnection();
            conn.Open();
            
            using var cmd = new NpgsqlCommand("SELECT book_id, title, author_name, stock, adding_date FROM books ORDER BY book_id", conn);
            using var reader = cmd.ExecuteReader();
            
            while (reader.Read())
            {
                books.Add(new Book
                {
                    BookId = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    AuthorName = reader.GetString(2),
                    Stock = reader.GetInt32(3),
                    AddingDate = reader.GetDateTime(4)
                });
            }
            
            return books;
        }

        public Book GetBookById(int bookId)
        {
            using var conn = DbConnectionManager.Instance.CreateConnection();
            conn.Open();
            
            using var cmd = new NpgsqlCommand("SELECT book_id, title, author_name, stock, adding_date FROM books WHERE book_id = @id", conn);
            cmd.Parameters.AddWithValue("id", bookId);
            using var reader = cmd.ExecuteReader();
            
            if (reader.Read())
            {
                return new Book
                {
                    BookId = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    AuthorName = reader.GetString(2),
                    Stock = reader.GetInt32(3),
                    AddingDate = reader.GetDateTime(4)
                };
            }
            
            return null;
        }

        public int AddBook(Book book)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));
            if (string.IsNullOrWhiteSpace(book.Title)) throw new ArgumentException("Book title cannot be empty", nameof(book));
            if (string.IsNullOrWhiteSpace(book.AuthorName)) throw new ArgumentException("Author name cannot be empty", nameof(book));
            if (book.Stock < 0) throw new ArgumentException("Stock cannot be negative", nameof(book));
            if (book.AddingDate > DateTime.Now.Date) throw new ArgumentException("Adding date cannot be in the future", nameof(book));
            if (book.AddingDate < DateTime.Now.AddYears(-50).Date) throw new ArgumentException("Adding date cannot be more than 50 years in the past", nameof(book));

            using var conn = DbConnectionManager.Instance.CreateConnection();
            conn.Open();
            
            using var cmd = new NpgsqlCommand(
                "INSERT INTO books (title, author_name, stock, adding_date) VALUES (@title, @author, @stock, @date) RETURNING book_id",
                conn);
            cmd.Parameters.AddWithValue("title", book.Title);
            cmd.Parameters.AddWithValue("author", book.AuthorName);
            cmd.Parameters.AddWithValue("stock", book.Stock);
            cmd.Parameters.AddWithValue("date", book.AddingDate);
            
            var result = cmd.ExecuteScalar();
            if (result == null || result == DBNull.Value)
                throw new InvalidOperationException("Failed to create book - no ID returned");
            
            return Convert.ToInt32(result);
        }

        public bool UpdateBook(Book book)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));
            if (string.IsNullOrWhiteSpace(book.Title)) throw new ArgumentException("Book title cannot be empty", nameof(book));
            if (string.IsNullOrWhiteSpace(book.AuthorName)) throw new ArgumentException("Author name cannot be empty", nameof(book));
            if (book.Stock < 0) throw new ArgumentException("Stock cannot be negative", nameof(book));
            if (book.BookId <= 0) throw new ArgumentException("Invalid book ID", nameof(book));
            if (book.AddingDate > DateTime.Now.Date) throw new ArgumentException("Adding date cannot be in the future", nameof(book));
            if (book.AddingDate < DateTime.Now.AddYears(-50).Date) throw new ArgumentException("Adding date cannot be more than 50 years in the past", nameof(book));

            using var conn = DbConnectionManager.Instance.CreateConnection();
            conn.Open();
            
            using var cmd = new NpgsqlCommand(
                "UPDATE books SET title = @title, author_name = @author, stock = @stock, adding_date = @date WHERE book_id = @id",
                conn);
            cmd.Parameters.AddWithValue("id", book.BookId);
            cmd.Parameters.AddWithValue("title", book.Title);
            cmd.Parameters.AddWithValue("author", book.AuthorName);
            cmd.Parameters.AddWithValue("stock", book.Stock);
            cmd.Parameters.AddWithValue("date", book.AddingDate);
            
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool DeleteBook(int bookId)
        {
            using var conn = DbConnectionManager.Instance.CreateConnection();
            conn.Open();
            
            using var cmd = new NpgsqlCommand("DELETE FROM books WHERE book_id = @id", conn);
            cmd.Parameters.AddWithValue("id", bookId);
            
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool UpdateStock(int bookId, int quantity)
        {
            if (bookId <= 0) throw new ArgumentException("Invalid book ID", nameof(bookId));
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive", nameof(quantity));

            using var conn = DbConnectionManager.Instance.CreateConnection();
            conn.Open();
            
            // Check if there's enough stock before updating
            using var checkCmd = new NpgsqlCommand("SELECT stock FROM books WHERE book_id = @id", conn);
            checkCmd.Parameters.AddWithValue("id", bookId);
            var result = checkCmd.ExecuteScalar();
            
            if (result == null || result == DBNull.Value)
            {
                return false; // Book not found
            }
            
            var currentStock = Convert.ToInt32(result);
            
            if (currentStock < quantity)
            {
                return false; // Not enough stock
            }
            
            using var cmd = new NpgsqlCommand("UPDATE books SET stock = stock - @qty WHERE book_id = @id AND stock >= @qty", conn);
            cmd.Parameters.AddWithValue("id", bookId);
            cmd.Parameters.AddWithValue("qty", quantity);
            
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}

