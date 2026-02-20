using System;
using System.Collections.Generic;
using Npgsql;
using WindowsFormsApp.Models;
using WindowsFormsApp.Singleton;

namespace WindowsFormsApp.Data
{
    /// <summary>
    /// Data Access Layer for Sales
    /// </summary>
    public class SaleRepository
    {
        public List<Sale> GetAllSales()
        {
            var sales = new List<Sale>();
            using var conn = DbConnectionManager.Instance.CreateConnection();
            conn.Open();
            
            using var cmd = new NpgsqlCommand(
                "SELECT sale_id, customer_name, book_id, employee_id, price, quantity, discount, total, sale_date FROM sales ORDER BY sale_date DESC, sale_id DESC",
                conn);
            using var reader = cmd.ExecuteReader();
            
            while (reader.Read())
            {
                sales.Add(new Sale
                {
                    SaleId = reader.GetInt32(0),
                    CustomerName = reader.GetString(1),
                    BookId = reader.GetInt32(2),
                    EmployeeId = reader.GetInt32(3),
                    Price = reader.GetDecimal(4),
                    Quantity = reader.GetInt32(5),
                    Discount = reader.GetDecimal(6),
                    Total = reader.GetDecimal(7),
                    SaleDate = reader.GetDateTime(8)
                });
            }
            
            return sales;
        }

        public int AddSale(Sale sale)
        {
            using var conn = DbConnectionManager.Instance.CreateConnection();
            conn.Open();
            
            using var cmd = new NpgsqlCommand(
                "INSERT INTO sales (customer_name, book_id, employee_id, price, quantity, discount, sale_date) VALUES (@customer, @bookId, @empId, @price, @qty, @discount, @date) RETURNING sale_id",
                conn);
            cmd.Parameters.AddWithValue("customer", sale.CustomerName);
            cmd.Parameters.AddWithValue("bookId", sale.BookId);
            cmd.Parameters.AddWithValue("empId", sale.EmployeeId);
            cmd.Parameters.AddWithValue("price", sale.Price);
            cmd.Parameters.AddWithValue("qty", sale.Quantity);
            cmd.Parameters.AddWithValue("discount", sale.Discount);
            cmd.Parameters.AddWithValue("date", sale.SaleDate);
            
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool DeleteSale(int saleId)
        {
            using var conn = DbConnectionManager.Instance.CreateConnection();
            conn.Open();
            
            using var cmd = new NpgsqlCommand("DELETE FROM sales WHERE sale_id = @id", conn);
            cmd.Parameters.AddWithValue("id", saleId);
            
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}

