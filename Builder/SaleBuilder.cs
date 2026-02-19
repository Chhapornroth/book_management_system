using System;
using WindowsFormsApp.Models;

namespace WindowsFormsApp.Builder
{
    /// <summary>
    /// Builder Pattern:
    /// Builds a Sale object step-by-step.
    /// </summary>
    public class SaleBuilder
    {
        private readonly Sale _sale = new();

        public SaleBuilder SetCustomerName(string customerName)
        {
            _sale.CustomerName = customerName;
            return this;
        }

        public SaleBuilder SetBookId(int bookId)
        {
            _sale.BookId = bookId;
            return this;
        }

        public SaleBuilder SetEmployeeId(int employeeId)
        {
            _sale.EmployeeId = employeeId;
            return this;
        }

        public SaleBuilder SetPrice(decimal price)
        {
            _sale.Price = price;
            return this;
        }

        public SaleBuilder SetQuantity(int quantity)
        {
            _sale.Quantity = quantity;
            return this;
        }

        public SaleBuilder SetDiscount(decimal discount)
        {
            _sale.Discount = discount;
            return this;
        }

        public SaleBuilder SetSaleDate(DateTime saleDate)
        {
            _sale.SaleDate = saleDate;
            _sale.CreatedAt = DateTime.Now;
            return this;
        }

        public Sale Build()
        {
            var gross = _sale.Price * _sale.Quantity;
            if (_sale.Discount > 0)
            {
                gross -= gross * _sale.Discount;
            }

            _sale.Total = decimal.Round(gross, 2);
            return _sale;
        }
    }
}


