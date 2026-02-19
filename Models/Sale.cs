using System;

namespace WindowsFormsApp.Models
{
    // Built via Builder pattern
    public class Sale
    {
        public int SaleId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int BookId { get; set; }
        public int EmployeeId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; } // 0.0 - 1.0
        public decimal Total { get; set; }
        public DateTime SaleDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}


