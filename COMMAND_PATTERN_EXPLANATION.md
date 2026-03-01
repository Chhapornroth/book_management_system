# Command Pattern - Simple Explanation

## 🎯 What is the Command Pattern?

Think of the Command Pattern like a **remote control** for your TV. Instead of directly changing channels, you press a button that "remembers" what to do. Later, you can press "undo" to go back.

In programming, the Command Pattern **encapsulates a request as an object**. This means:
- Instead of calling methods directly, you create a "command object" that knows what to do
- You can execute it now, later, or undo it
- You can queue multiple commands
- You can log what commands were executed

---

## 📦 The 3 Main Components

### 1. **ICommand Interface** (The Contract)
```csharp
public interface ICommand
{
    bool Execute();  // Do the action
    bool Undo();     // Reverse the action
}
```
**Think of it as:** A blueprint that says "every command must be able to Execute and Undo"

### 2. **ProcessSaleCommand** (The Concrete Command)
```csharp
public class ProcessSaleCommand : ICommand
{
    // Stores all the data needed to process a sale
    private readonly string _customerName;
    private readonly List<CartItemData> _cartItems;
    // ... etc
    
    public bool Execute() { /* Process the sale */ }
    public bool Undo() { /* Reverse the sale */ }
}
```
**Think of it as:** A specific command that knows how to process a sale

### 3. **CommandInvoker** (The Manager)
```csharp
public class CommandInvoker
{
    private Stack<ICommand> _commandHistory = new();
    
    public bool ExecuteCommand(ICommand command) { /* Execute and save */ }
    public bool UndoLastCommand() { /* Undo the last one */ }
}
```
**Think of it as:** The manager that executes commands and remembers them for undo

---

## 🔄 How It Works - Step by Step

### **Scenario: Cashier processes a sale**

```
┌─────────────────────────────────────────────────────────────┐
│  STEP 1: Cashier clicks "Process Sale" button              │
└─────────────────────────────────────────────────────────────┘
```

**In CashierForm.cs (line 975-982):**
```csharp
// Create a command object with all the sale information
var processSaleCommand = new ProcessSaleCommand(
    txtCustomerName.Text,      // "John Doe"
    cartItemData,              // [Book1, Book2, Book3]
    _currentUser.Id,            // Employee ID: 5
    _bookRepo,                 // Database access
    _saleRepo,                 // Database access
    _saleNotifier              // Observer for notifications
);
```

**What happens:**
- A `ProcessSaleCommand` object is created
- It stores all the information needed (customer name, items, etc.)
- **Nothing is executed yet!** The command is just "prepared"

---

```
┌─────────────────────────────────────────────────────────────┐
│  STEP 2: Execute the command                                │
└─────────────────────────────────────────────────────────────┘
```

**In CashierForm.cs (line 984):**
```csharp
bool success = _commandInvoker.ExecuteCommand(processSaleCommand);
```

**What happens inside CommandInvoker (line 18-30):**
```csharp
public bool ExecuteCommand(ICommand command)
{
    // 1. Call the command's Execute() method
    bool result = command.Execute();
    
    // 2. If successful, save it to history (for undo)
    if (result)
    {
        _commandHistory.Push(command);  // Save to stack
    }
    
    return result;
}
```

**What happens inside ProcessSaleCommand.Execute() (line 42-98):**
```csharp
public bool Execute()
{
    foreach (var item in _cartItems)
    {
        // 1. Validate stock
        var book = _bookRepo.GetBookById(item.BookId);
        
        // 2. Build sale object (using Builder pattern)
        var sale = new SaleBuilder()
            .SetCustomerName(_customerName)
            .SetBookId(item.BookId)
            // ... more setters
            .Build();
        
        // 3. Update stock in database
        _bookRepo.UpdateStock(item.BookId, item.Quantity);
        
        // 4. Save sale to database
        var saleId = _saleRepo.AddSale(sale);
        sale.SaleId = saleId;
        
        // 5. Remember this sale (for undo later)
        _processedSales.Add(sale);
        
        // 6. Notify observers
        _saleNotifier.Notify(sale);
    }
    
    return _processedSales.Count > 0;  // Success if any sales processed
}
```

**Result:**
- ✅ Sales are saved to database
- ✅ Stock is updated
- ✅ Command is saved in history stack
- ✅ Can be undone later

---

```
┌─────────────────────────────────────────────────────────────┐
│  STEP 3: Cashier clicks "Undo Last Sale" button            │
└─────────────────────────────────────────────────────────────┘
```

**In CashierForm.cs (line 1008-1020):**
```csharp
bool success = _commandInvoker.UndoLastCommand();
```

**What happens inside CommandInvoker (line 37-43):**
```csharp
public bool UndoLastCommand()
{
    // 1. Get the last command from history (LIFO - Last In, First Out)
    var lastCommand = _commandHistory.Pop();
    
    // 2. Call its Undo() method
    return lastCommand.Undo();
}
```

**What happens inside ProcessSaleCommand.Undo() (line 101-135):**
```csharp
public bool Undo()
{
    foreach (var sale in _processedSales)
    {
        // 1. Restore stock (add back what was taken)
        _bookRepo.RestoreStock(sale.BookId, sale.Quantity);
        
        // 2. Delete the sale from database
        _saleRepo.DeleteSale(sale.SaleId);
    }
    
    // 3. Clear the processed sales list
    _processedSales.Clear();
    
    return true;  // Success
}
```

**Result:**
- ✅ Stock is restored
- ✅ Sale is removed from database
- ✅ Everything is back to how it was before

---

## 🎨 Visual Flow Diagram

```
┌──────────────────────────────────────────────────────────────┐
│                    COMMAND PATTERN FLOW                      │
└──────────────────────────────────────────────────────────────┘

1. USER ACTION (Cashier clicks "Process Sale")
   │
   ▼
2. CREATE COMMAND
   ProcessSaleCommand command = new ProcessSaleCommand(...)
   │
   │  [Command object is created but NOT executed yet]
   │
   ▼
3. EXECUTE VIA INVOKER
   _commandInvoker.ExecuteCommand(command)
   │
   ├─► command.Execute()
   │   │
   │   ├─► Validate stock
   │   ├─► Build sale object (Builder pattern)
   │   ├─► Update stock in database
   │   ├─► Save sale to database
   │   └─► Notify observers (Observer pattern)
   │
   └─► Save command to history stack
       │
       [Command is now in _commandHistory stack]
       │
       ▼
4. UNDO (if needed)
   _commandInvoker.UndoLastCommand()
   │
   ├─► Pop command from history
   │
   └─► command.Undo()
       │
       ├─► Restore stock
       └─► Delete sale from database
```

---

## 💡 Why Use Command Pattern?

### **Without Command Pattern (Traditional Way):**
```csharp
// Direct method calls - can't undo!
private void BtnProcessSale_Click(...)
{
    foreach (var item in _cartItems)
    {
        _bookRepo.UpdateStock(...);
        _saleRepo.AddSale(...);
        // Oops! Can't undo this!
    }
}
```

**Problems:**
- ❌ Can't undo if you make a mistake
- ❌ Can't queue commands for later
- ❌ Can't log what was done
- ❌ Hard to test

### **With Command Pattern:**
```csharp
// Create command object
var command = new ProcessSaleCommand(...);

// Execute via invoker
_commandInvoker.ExecuteCommand(command);
// ✅ Can undo later!
// ✅ Can queue for batch processing
// ✅ Can log commands
// ✅ Easy to test
```

**Benefits:**
- ✅ **Undo/Redo** - Can reverse actions
- ✅ **Queue** - Can save commands and execute later
- ✅ **Logging** - Can track what commands were executed
- ✅ **Testing** - Can test commands independently
- ✅ **Separation** - UI doesn't need to know HOW to process sales

---

## 🔍 Real Example from Your Code

### **When you process a sale:**

1. **CashierForm creates the command:**
   ```csharp
   var processSaleCommand = new ProcessSaleCommand(
       "John Doe",           // Customer
       [Book1, Book2],       // Items
       5,                    // Employee ID
       _bookRepo,            // Database
       _saleRepo,            // Database
       _saleNotifier         // Notifications
   );
   ```

2. **CommandInvoker executes it:**
   ```csharp
   _commandInvoker.ExecuteCommand(processSaleCommand);
   ```
   - Calls `processSaleCommand.Execute()`
   - Saves command to `_commandHistory` stack

3. **If you need to undo:**
   ```csharp
   _commandInvoker.UndoLastCommand();
   ```
   - Gets last command from stack
   - Calls `processSaleCommand.Undo()`
   - Restores everything

---

## 🎓 Key Concepts

### **1. Encapsulation**
The command object **encapsulates** (wraps up) all the information and logic needed to perform an action.

**Like a package:**
```
┌─────────────────────────┐
│  ProcessSaleCommand     │
│  ─────────────────────  │
│  • Customer name        │
│  • Cart items           │
│  • Employee ID          │
│  • Database access      │
│  • Execute() method     │
│  • Undo() method        │
└─────────────────────────┘
```

### **2. History Stack (LIFO)**
Commands are stored in a **Stack** (Last In, First Out).

**Like a stack of plates:**
```
    [Command 3] ← Last added (will be undone first)
    [Command 2]
    [Command 1] ← First added (will be undone last)
```

When you undo, you get the **most recent** command first.

### **3. Separation of Concerns**
- **CashierForm** (UI) - Doesn't know HOW to process sales
- **ProcessSaleCommand** - Knows HOW to process sales
- **CommandInvoker** - Manages execution and history

**UI just says:** "Execute this command"
**Command says:** "I know how to do it"
**Invoker says:** "I'll manage it and remember it"

---

## 🧪 Simple Analogy

**Think of a restaurant:**

1. **Customer (UI)** says: "I want a pizza"
2. **Order Ticket (Command)** is created with:
   - What: Pizza
   - Size: Large
   - Toppings: Pepperoni
   - Customer: Table 5
3. **Waiter (Invoker)** takes the order ticket to kitchen
4. **Chef (Execute)** makes the pizza
5. **If customer changes mind:** Waiter can cancel (Undo)

**In your code:**
- **CashierForm** = Customer
- **ProcessSaleCommand** = Order Ticket
- **CommandInvoker** = Waiter
- **Execute()** = Chef making the pizza
- **Undo()** = Canceling the order

---

## 📝 Summary

**Command Pattern = "Do it later, undo it if needed"**

1. **Create** a command object with all needed info
2. **Execute** it when ready (via invoker)
3. **Save** it to history (for undo)
4. **Undo** it if needed (reverses all actions)

**In your project:**
- ✅ Process sales as commands
- ✅ Undo sales if needed
- ✅ Keep history of all sales
- ✅ Clean separation of UI and business logic

---

## 🚀 Try It Yourself!

1. **Process a sale** → Command is created and executed
2. **Check undo button** → Should be enabled
3. **Click undo** → Sale is reversed, stock restored
4. **Check database** → Sale is removed

The Command Pattern makes your code more flexible and maintainable! 🎉

