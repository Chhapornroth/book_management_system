using System;
using System.Collections.Generic;
using System.Linq;
using WindowsFormsApp.Command;
using WindowsFormsApp.Data;
using WindowsFormsApp.Models;
using WindowsFormsApp.Observer;

namespace WindowsFormsApp.Facade
{
    /// <summary>
    /// Facade Pattern:
    /// Provides a simplified interface to the complex subsystem of sale processing.
    /// Hides the complexity of validation, command creation, execution, and result handling.
    /// </summary>
    public class SaleProcessingFacade
    {
        private readonly BookRepository _bookRepo;
        private readonly SaleRepository _saleRepo;
        private readonly SaleNotifier _saleNotifier;
        private readonly CommandInvoker _commandInvoker;

        public SaleProcessingFacade(
            BookRepository bookRepo,
            SaleRepository saleRepo,
            SaleNotifier saleNotifier,
            CommandInvoker commandInvoker)
        {
            _bookRepo = bookRepo;
            _saleRepo = saleRepo;
            _saleNotifier = saleNotifier;
            _commandInvoker = commandInvoker;
        }

        /// <summary>
        /// Result of sale processing operation
        /// </summary>
        public class SaleProcessingResult
        {
            public bool Success { get; set; }
            public int ProcessedCount { get; set; }
            public int FailedCount { get; set; }
            public List<string> FailedItems { get; set; } = new();
            public List<CartItemData> ProcessedItems { get; set; } = new();
            public decimal TotalAmount { get; set; }
            public string Message { get; set; } = string.Empty;
        }

        /// <summary>
        /// Processes a sale with all validations and error handling.
        /// This is the simplified interface that hides all complexity.
        /// </summary>
        public SaleProcessingResult ProcessSale(
            string customerName,
            List<CartItem> cartItems,
            int employeeId)
        {
            var result = new SaleProcessingResult();

            // Validation: Check if cart is empty
            if (cartItems == null || cartItems.Count == 0)
            {
                result.Success = false;
                result.Message = "Cart is empty";
                return result;
            }

            // Validation: Check customer name
            if (string.IsNullOrWhiteSpace(customerName))
            {
                result.Success = false;
                result.Message = "Please enter customer name";
                return result;
            }

            // Validation: Check stock availability for all items
            var validationErrors = ValidateStockAvailability(cartItems);
            if (validationErrors.Count > 0)
            {
                result.Success = false;
                result.FailedItems = validationErrors;
                result.Message = $"Validation failed: {string.Join(", ", validationErrors)}";
                return result;
            }

            // Convert cart items to command data format
            var cartItemData = cartItems.Select(item => new CartItemData
            {
                BookId = item.BookId,
                Title = item.Title,
                Price = item.Price,
                Quantity = item.Quantity,
                Discount = item.Discount
            }).ToList();

            // Create command using Command Pattern
            var processSaleCommand = new ProcessSaleCommand(
                customerName,
                cartItemData,
                employeeId,
                _bookRepo,
                _saleRepo,
                _saleNotifier
            );

            // Execute command via CommandInvoker
            bool executionSuccess = _commandInvoker.ExecuteCommand(processSaleCommand);

            // Process results
            if (!executionSuccess || processSaleCommand.ProcessedCount == 0)
            {
                result.Success = false;
                result.Message = "No items were processed. Please check stock availability.";
                return result;
            }

            // Calculate total amount
            result.TotalAmount = cartItems
                .Where(item => processSaleCommand.GetProcessedItems()
                    .Any(pi => pi.BookId == item.BookId && pi.Quantity == item.Quantity))
                .Sum(item => (item.Price * item.Quantity) * (1 - item.Discount));

            // Get processed and failed items
            var processedItems = processSaleCommand.GetProcessedItems();
            result.ProcessedItems = processedItems;
            result.ProcessedCount = processSaleCommand.ProcessedCount;
            result.FailedCount = processSaleCommand.FailedCount;

            // Identify failed items
            if (result.FailedCount > 0)
            {
                result.FailedItems = cartItems
                    .Where(item => !processedItems.Any(pi => 
                        pi.BookId == item.BookId && 
                        pi.Quantity == item.Quantity && 
                        pi.Price == item.Price))
                    .Select(item => $"{item.Title} (Qty: {item.Quantity})")
                    .ToList();
            }

            result.Success = true;
            result.Message = $"Sale processed successfully! {result.ProcessedCount} item(s) processed. Total: {result.TotalAmount:C}";

            return result;
        }

        /// <summary>
        /// Validates stock availability for all cart items
        /// </summary>
        private List<string> ValidateStockAvailability(List<CartItem> cartItems)
        {
            var errors = new List<string>();

            foreach (var item in cartItems)
            {
                var book = _bookRepo.GetBookById(item.BookId);
                if (book == null)
                {
                    errors.Add($"Book ID {item.BookId} not found");
                    continue;
                }

                if (book.Stock < item.Quantity)
                {
                    errors.Add($"Insufficient stock for '{book.Title}'. Available: {book.Stock}, Requested: {item.Quantity}");
                }
            }

            return errors;
        }

        /// <summary>
        /// Gets the processed items from the last command (for cart cleanup)
        /// </summary>
        public List<CartItemData> GetProcessedItemsFromLastCommand()
        {
            // This would need access to the last command, but for simplicity,
            // we'll return empty list. In a real scenario, you might store the last command.
            return new List<CartItemData>();
        }
    }

    /// <summary>
    /// Cart item structure for facade (matches CashierForm's CartItem)
    /// </summary>
    public class CartItem
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
    }
}

