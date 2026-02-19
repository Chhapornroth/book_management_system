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
                    BookId = reader.GetInt32("book_id"),
                    Title = reader.GetString("title"),
                    AuthorName = reader.GetString("author_name"),
                    Stock = reader.GetInt32("stock"),
                    AddingDate = reader.GetDateTime("adding_date")
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
                    BookId = reader.GetInt32("book_id"),
                    Title = reader.GetString("title"),
                    AuthorName = reader.GetString("author_name"),
                    Stock = reader.GetInt32("stock"),
                    AddingDate = reader.GetDateTime("adding_date")
                };
            }
            
            return null;
        }

        public int AddBook(Book book)
        {
            using var conn = DbConnectionManager.Instance.CreateConnection();
            conn.Open();
            
            using var cmd = new NpgsqlCommand(
                "INSERT INTO books (title, author_name, stock, adding_date) VALUES (@title, @author, @stock, @date) RETURNING book_id",
                conn);
            cmd.Parameters.AddWithValue("title", book.Title);
            cmd.Parameters.AddWithValue("author", book.AuthorName);
            cmd.Parameters.AddWithValue("stock", book.Stock);
            cmd.Parameters.AddWithValue("date", book.AddingDate);
            
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool UpdateBook(Book book)
        {
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
            using var conn = DbConnectionManager.Instance.CreateConnection();
            conn.Open();
            
            using var cmd = new NpgsqlCommand("UPDATE books SET stock = stock - @qty WHERE book_id = @id", conn);
            cmd.Parameters.AddWithValue("id", bookId);
            cmd.Parameters.AddWithValue("qty", quantity);
            
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}

