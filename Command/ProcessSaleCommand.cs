using System;
using System.Collections.Generic;
using System.Diagnostics;
using WindowsFormsApp.Builder;
using WindowsFormsApp.Data;
using WindowsFormsApp.Models;
using WindowsFormsApp.Observer;

namespace WindowsFormsApp.Command
{
    /// <summary>
    /// Command Pattern Implementation:
    /// Encapsulates the sale processing operation as a command.
    /// </summary>
    public class ProcessSaleCommand : ICommand
    {
        private readonly string _customerName;
        private readonly List<CartItemData> _cartItems;
        private readonly int _employeeId;
        private readonly BookRepository _bookRepo;
        private readonly SaleRepository _saleRepo;
        private readonly SaleNotifier _saleNotifier;
        private readonly List<Sale> _processedSales = new();
        private readonly List<CartItemData> _processedItems = new();

        public ProcessSaleCommand(
            string customerName,
            List<CartItemData> cartItems,
            int employeeId,
            BookRepository bookRepo,
            SaleRepository saleRepo,
            SaleNotifier saleNotifier)
        {
            _customerName = customerName;
            _cartItems = new List<CartItemData>(cartItems); // Create a copy
            _employeeId = employeeId;
            _bookRepo = bookRepo;
            _saleRepo = saleRepo;
            _saleNotifier = saleNotifier;
        }

        public bool Execute()
        {
            _processedSales.Clear();
            _processedItems.Clear();

            foreach (var item in _cartItems)
            {
                try
                {
                    // Validate stock availability
                    var book = _bookRepo.GetBookById(item.BookId);
                    if (book == null)
                    {
                        Debug.WriteLine($"Book ID {item.BookId} not found");
                        continue;
                    }

                    if (book.Stock < item.Quantity)
                    {
                        Debug.WriteLine($"Insufficient stock for '{book.Title}'. Available: {book.Stock}, Requested: {item.Quantity}");
                        continue;
                    }

                    // Build sale using Builder pattern
                    var sale = new SaleBuilder()
                        .SetCustomerName(_customerName)
                        .SetBookId(item.BookId)
                        .SetEmployeeId(_employeeId)
                        .SetPrice(item.Price)
                        .SetQuantity(item.Quantity)
                        .SetDiscount(item.Discount)
                        .SetSaleDate(DateTime.Now)
                        .Build();

                    // Update stock first (atomic operation)
                    if (!_bookRepo.UpdateStock(item.BookId, item.Quantity))
                    {
                        Debug.WriteLine($"Failed to update stock for book ID {item.BookId}");
                        continue;
                    }

                    // Add sale to repository and get the generated sale ID
                    var saleId = _saleRepo.AddSale(sale);
                    sale.SaleId = saleId; // Set the ID for undo functionality
                    _processedSales.Add(sale);
                    _processedItems.Add(item);

                    // Notify observers (Observer Pattern)
                    _saleNotifier.Notify(sale);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error processing item '{item.Title}': {ex.Message}");
                }
            }

            return _processedSales.Count > 0;
        }

        public bool Undo()
        {
            // Undo operation: restore stock and remove sales
            bool allUndone = true;

            foreach (var sale in _processedSales)
            {
                try
                {
                    // Restore stock using RestoreStock method (increases stock)
                    if (!_bookRepo.RestoreStock(sale.BookId, sale.Quantity))
                    {
                        Debug.WriteLine($"Failed to restore stock for book ID {sale.BookId}");
                        allUndone = false;
                        continue;
                    }

                    // Remove sale from repository
                    if (!_saleRepo.DeleteSale(sale.SaleId))
                    {
                        Debug.WriteLine($"Failed to delete sale {sale.SaleId}");
                        allUndone = false;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error undoing sale {sale.SaleId}: {ex.Message}");
                    allUndone = false;
                }
            }

            _processedSales.Clear();
            _processedItems.Clear();

            return allUndone;
        }

        public List<Sale> GetProcessedSales() => new List<Sale>(_processedSales);
        public List<CartItemData> GetProcessedItems() => new List<CartItemData>(_processedItems);
        public int ProcessedCount => _processedSales.Count;
        public int FailedCount => _cartItems.Count - _processedSales.Count;
    }

    /// <summary>
    /// Cart item data structure for command processing
    /// </summary>
    public class CartItemData
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
    }
}

