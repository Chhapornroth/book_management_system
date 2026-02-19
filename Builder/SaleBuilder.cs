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

        public SaleBuilder WithCustomer(string customerName)
        {
            _sale.CustomerName = customerName;
            return this;
        }

        public SaleBuilder WithBook(int bookId, decimal price, int quantity)
        {
            _sale.BookId = bookId;
            _sale.Price = price;
            _sale.Quantity = quantity;
            return this;
        }

        public SaleBuilder WithEmployee(int employeeId)
        {
            _sale.EmployeeId = employeeId;
            return this;
        }

        public SaleBuilder WithDiscount(decimal discount)
        {
            _sale.Discount = discount;
            return this;
        }

        public SaleBuilder WithTimestamp(DateTime createdAt)
        {
            _sale.CreatedAt = createdAt;
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


