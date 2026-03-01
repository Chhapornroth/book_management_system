# Design Patterns Explanation - BookStore Management System

This document explains how all 5 design patterns work together in the application.

## 🎯 Overview of Patterns

1. **Singleton Pattern** - Database connection management
2. **Factory Pattern** - User creation
3. **Builder Pattern** - Sale object construction
4. **Command Pattern** - Sale processing operations
5. **Observer Pattern** - Sale notifications

---

## 📊 Complete Sale Processing Flow

Here's how all patterns work together when a cashier processes a sale:

```
┌─────────────────────────────────────────────────────────────────┐
│                    SALE PROCESSING FLOW                         │
└─────────────────────────────────────────────────────────────────┘

1. USER LOGIN (Factory Pattern)
   ┌─────────────────────────────────────┐
   │ LoginForm authenticates user         │
   │ ↓                                    │
   │ UserFactory.CreateUser(role, id, ...)│
   │ ↓                                    │
   │ Returns: AdminUser or CashierUser    │
   └─────────────────────────────────────┘

2. DATABASE ACCESS (Singleton Pattern)
   ┌─────────────────────────────────────┐
   │ All repositories use:                │
   │ DbConnectionManager.Instance         │
   │ ↓                                    │
   │ Creates single shared connection     │
   │ (Prevents multiple connections)       │
   └─────────────────────────────────────┘

3. CASHIER ADDS ITEMS TO CART
   ┌─────────────────────────────────────┐
   │ CashierForm collects:               │
   │ - Customer name                     │
   │ - Books with quantities             │
   │ - Discounts                         │
   └─────────────────────────────────────┘

4. PROCESS SALE BUTTON CLICKED (Command Pattern)
   ┌─────────────────────────────────────────────────────────────┐
   │ CashierForm.BtnProcessSale_Click()                           │
   │                                                               │
   │ Step 1: Create ProcessSaleCommand                           │
   │   ┌─────────────────────────────────────────────────────┐   │
   │   │ var command = new ProcessSaleCommand(                │   │
   │   │   customerName, cartItems, employeeId,              │   │
   │   │   bookRepo, saleRepo, saleNotifier)                 │   │
   │   └─────────────────────────────────────────────────────┘   │
   │                                                               │
   │ Step 2: Execute via CommandInvoker                         │
   │   ┌─────────────────────────────────────────────────────┐   │
   │   │ _commandInvoker.ExecuteCommand(command)              │   │
   │   │   ↓                                                  │   │
   │   │ command.Execute()                                    │   │
   │   └─────────────────────────────────────────────────────┘   │
   └─────────────────────────────────────────────────────────────┘

5. COMMAND EXECUTION (Inside ProcessSaleCommand.Execute())
   ┌─────────────────────────────────────────────────────────────┐
   │ For each cart item:                                         │
   │                                                               │
   │   A. Validate Stock                                          │
   │      ┌─────────────────────────────────────┐                │
   │      │ bookRepo.GetBookById(item.BookId)   │                │
   │      │ Check: book.Stock >= item.Quantity  │                │
   │      └─────────────────────────────────────┘                │
   │                                                               │
   │   B. Build Sale Object (Builder Pattern)                    │
   │      ┌──────────────────────────────────────────────────┐   │
   │      │ var sale = new SaleBuilder()                    │   │
   │      │     .SetCustomerName(customerName)              │   │
   │      │     .SetBookId(bookId)                          │   │
   │      │     .SetEmployeeId(employeeId)                  │   │
   │      │     .SetPrice(price)                            │   │
   │      │     .SetQuantity(quantity)                      │   │
   │      │     .SetDiscount(discount)                      │   │
   │      │     .SetSaleDate(DateTime.Now)                   │   │
   │      │     .Build();  // Calculates total automatically │   │
   │      └──────────────────────────────────────────────────┘   │
   │                                                               │
   │   C. Update Stock                                            │
   │      ┌─────────────────────────────────────┐                │
   │      │ bookRepo.UpdateStock(bookId, qty)   │                │
   │      │ (Uses Singleton for DB connection)   │                │
   │      └─────────────────────────────────────┘                │
   │                                                               │
   │   D. Save Sale to Database                                  │
   │      ┌─────────────────────────────────────┐                │
   │      │ saleRepo.AddSale(sale)              │                │
   │      │ (Uses Singleton for DB connection)   │                │
   │      └─────────────────────────────────────┘                │
   │                                                               │
   │   E. Notify Observers (Observer Pattern)                    │
   │      ┌──────────────────────────────────────────────────┐   │
   │      │ saleNotifier.Notify(sale)                       │   │
   │      │   ↓                                             │   │
   │      │ For each observer:                              │   │
   │      │   observer.OnSaleCreated(sale)                   │   │
   │      │   ↓                                             │   │
   │      │ LoggingSaleObserver logs the sale               │   │
   │      └──────────────────────────────────────────────────┘   │
   │                                                               │
   │   F. Store for Undo Support                                  │
   │      ┌─────────────────────────────────────┐                │
   │      │ _processedSales.Add(sale)           │                │
   │      │ (Stored in command for undo)        │                │
   │      └─────────────────────────────────────┘                │
   └─────────────────────────────────────────────────────────────┘

6. COMMAND HISTORY (CommandInvoker)
   ┌─────────────────────────────────────┐
   │ If execution successful:            │
   │   _commandHistory.Push(command)     │
   │   (Enables undo functionality)       │
   └─────────────────────────────────────┘

7. RESULT REPORTING
   ┌─────────────────────────────────────┐
   │ CashierForm displays:               │
   │ - Success message                   │
   │ - Number of items processed         │
   │ - Total amount                      │
   │ - Failed items (if any)             │
   └─────────────────────────────────────┘
```

---

## 🔍 Detailed Pattern Explanations

### 1. **Singleton Pattern** - `DbConnectionManager`

**Purpose**: Ensure only ONE database connection instance exists

**How it works**:
```csharp
// Instead of creating new connections everywhere:
var conn = new NpgsqlConnection(connectionString); // ❌ Bad

// Use Singleton:
var conn = DbConnectionManager.Instance.CreateConnection(); // ✅ Good
```

**Benefits**:
- Prevents connection pool exhaustion
- Centralized connection management
- Single source of truth for connection string

**Usage in code**:
- All repositories (`BookRepository`, `SaleRepository`, `EmployeeRepository`) use this
- Accessed via: `DbConnectionManager.Instance.CreateConnection()`

---

### 2. **Factory Pattern** - `UserFactory`

**Purpose**: Create different user types without exposing creation logic

**How it works**:
```csharp
// Instead of:
if (role == UserRole.Admin)
    user = new AdminUser();
else
    user = new CashierUser(); // ❌ Scattered logic

// Use Factory:
var user = UserFactory.CreateUser(role, id, fullName); // ✅ Centralized
```

**Benefits**:
- Encapsulates object creation
- Easy to add new user types
- Single place to modify creation logic

**Usage in code**:
- Used in `LoginForm.cs` after authentication
- Returns `AdminUser` or `CashierUser` based on role

---

### 3. **Builder Pattern** - `SaleBuilder`

**Purpose**: Construct complex `Sale` objects step-by-step

**How it works**:
```csharp
// Instead of:
var sale = new Sale();
sale.CustomerName = "John";
sale.BookId = 1;
sale.Price = 10.00m;
sale.Quantity = 2;
sale.Discount = 0.1m;
sale.Total = (10.00m * 2) * (1 - 0.1m); // Manual calculation ❌

// Use Builder:
var sale = new SaleBuilder()
    .SetCustomerName("John")
    .SetBookId(1)
    .SetPrice(10.00m)
    .SetQuantity(2)
    .SetDiscount(0.1m)
    .SetSaleDate(DateTime.Now)
    .Build(); // Automatically calculates total ✅
```

**Benefits**:
- Fluent, readable API
- Automatic total calculation
- Prevents incomplete objects
- Easy to add new properties

**Usage in code**:
- Used inside `ProcessSaleCommand.Execute()`
- Builds each sale object before saving

---

### 4. **Command Pattern** - `ICommand`, `ProcessSaleCommand`, `CommandInvoker`

**Purpose**: Encapsulate operations as objects, enabling undo/redo and queuing

**How it works**:
```csharp
// Step 1: Create command with all necessary data
var command = new ProcessSaleCommand(
    customerName, cartItems, employeeId,
    bookRepo, saleRepo, saleNotifier
);

// Step 2: Execute via invoker
_commandInvoker.ExecuteCommand(command);
//   ↓
// command.Execute() is called
//   ↓
// Processes all items, saves to DB, notifies observers
//   ↓
// Command is stored in history for undo

// Step 3: Undo if needed (future feature)
_commandInvoker.UndoLastCommand();
//   ↓
// command.Undo() is called
//   ↓
// Restores stock, removes sales from DB
```

**Components**:
- **ICommand**: Interface defining `Execute()` and `Undo()`
- **ProcessSaleCommand**: Concrete command that processes sales
- **CommandInvoker**: Manages execution and maintains history

**Benefits**:
- Encapsulates operations
- Enables undo/redo functionality
- Can queue commands for later execution
- Separates request from execution
- Easy to add logging, validation, etc.

**Usage in code**:
- `CashierForm` creates and executes `ProcessSaleCommand`
- Command history stored in `CommandInvoker` for future undo support

---

### 5. **Observer Pattern** - `ISaleObserver`, `SaleNotifier`

**Purpose**: Notify multiple objects when a sale is created

**How it works**:
```csharp
// Step 1: Attach observers
var notifier = new SaleNotifier();
notifier.Attach(new LoggingSaleObserver());
// Could attach more: EmailObserver, AnalyticsObserver, etc.

// Step 2: When sale is created
notifier.Notify(sale);
//   ↓
// All observers are notified
//   ↓
// LoggingSaleObserver.OnSaleCreated(sale)
//   ↓
// Logs the sale to console/debug output
```

**Components**:
- **ISaleObserver**: Interface for observers
- **SaleNotifier**: Subject that notifies observers
- **LoggingSaleObserver**: Concrete observer that logs sales

**Benefits**:
- Loose coupling between subject and observers
- Easy to add new observers (email, analytics, etc.)
- Open/Closed Principle: Open for extension, closed for modification

**Usage in code**:
- `ProcessSaleCommand` notifies observers after each sale
- `LoggingSaleObserver` logs sales to debug output
- Can easily add more observers (email notifications, reports, etc.)

---

## 🔄 Pattern Interactions

### When Processing a Sale:

1. **Singleton** provides database connection to repositories
2. **Command** encapsulates the entire sale processing operation
3. **Builder** constructs each sale object within the command
4. **Observer** notifies listeners when sales are created
5. **Factory** was used earlier to create the cashier user

### Example: Complete Interaction

```csharp
// In CashierForm.BtnProcessSale_Click():

// 1. Create command (encapsulates operation)
var command = new ProcessSaleCommand(...);

// 2. Execute via invoker
_commandInvoker.ExecuteCommand(command);
    ↓
// 3. Inside command.Execute():
    // 3a. Validate using repositories (which use Singleton)
    var book = _bookRepo.GetBookById(item.BookId);
    
    // 3b. Build sale using Builder
    var sale = new SaleBuilder()
        .SetCustomerName(...)
        .SetBookId(...)
        // ... more setters
        .Build(); // Calculates total
    
    // 3c. Save using repository (uses Singleton)
    _saleRepo.AddSale(sale);
    
    // 3d. Notify observers
    _saleNotifier.Notify(sale);
        ↓
    // Observer logs the sale
    LoggingSaleObserver.OnSaleCreated(sale);
```

---

## 💡 Key Benefits of This Architecture

1. **Separation of Concerns**: Each pattern handles a specific responsibility
2. **Maintainability**: Easy to modify individual components
3. **Extensibility**: Easy to add new features (new commands, observers, etc.)
4. **Testability**: Each pattern can be tested independently
5. **Reusability**: Patterns can be reused in other parts of the application

---

## 🚀 Future Enhancements Possible

### Command Pattern:
- Add undo/redo UI buttons
- Queue commands for batch processing
- Add logging commands
- Add validation commands

### Observer Pattern:
- Add email notification observer
- Add analytics observer
- Add report generation observer

### Builder Pattern:
- Add validation in Build()
- Add different sale types (wholesale, retail)
- Add optional fields (tax, shipping)

---

## 📝 Summary

All 5 patterns work together seamlessly:

- **Singleton** ensures efficient database access
- **Factory** creates appropriate user objects
- **Builder** constructs complex sale objects
- **Command** encapsulates and manages sale operations
- **Observer** notifies interested parties about sales

This architecture makes the code:
- ✅ More maintainable
- ✅ More testable
- ✅ More extensible
- ✅ Following SOLID principles

