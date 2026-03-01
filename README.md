# BookStore Management System

A professional C# Windows Forms application for managing a bookstore with PostgreSQL database backend.

## 🚀 Features

- **User Authentication** - Secure login system with role-based access
- **Admin Dashboard** - Complete CRUD operations for books and employees
- **Cashier POS System** - Point of sale with discount calculation
- **Sales Management** - Track all transactions and sales history
- **PostgreSQL Database** - Robust database backend with proper relationships
- **Design Patterns** - Implements 6 design patterns (Singleton, Factory, Builder, Command, Observer, Facade)

## 📋 Requirements

- .NET 8.0 SDK or later
- PostgreSQL 12 or later
- Windows OS
- Npgsql package (included in project)

## 🗄️ Database Setup

1. Create PostgreSQL database:
```sql
CREATE DATABASE bookstore_db;
```

2. Run the schema script:
```bash
psql -U postgres -d bookstore_db -f database/bookstore_schema.sql
```

3. Update connection string in `Singleton/DbConnectionManager.cs`:
```
Host=localhost;Port=5432;Database=bookstore_db;Username=postgres;Password=your_password;
```

See `database/README.md` for detailed setup instructions.

## 🏗️ Project Structure

```
├── Models/          # Domain models (Book, Employee, Sale, User)
├── Data/            # Data access layer
├── Factory/         # Factory pattern (UserFactory)
├── Builder/         # Builder pattern (SaleBuilder)
├── Command/         # Command pattern (Sale processing commands)
├── Observer/        # Observer pattern (Sale notifications)
├── Facade/          # Facade pattern (Sale processing facade)
├── Singleton/       # Singleton pattern (Database connection)
├── Forms/           # Windows Forms UI
└── database/        # PostgreSQL schema and scripts
```

## 🎨 Design Patterns Implemented

1. **Singleton Pattern** - `DbConnectionManager` for database connection management
2. **Factory Pattern** - `UserFactory` for creating Admin/Cashier users
3. **Builder Pattern** - `SaleBuilder` for constructing sale objects step-by-step
4. **Command Pattern** - `ICommand` and `ProcessSaleCommand` for encapsulating sale operations with undo support
5. **Observer Pattern** - `ISaleObserver` for notifying when sales are created
6. **Facade Pattern** - `SaleProcessingFacade` for simplifying complex sale processing operations

## 🚀 How to Run

1. Clone the repository:
```bash
git clone https://github.com/Chhapornroth/book_management_system.git
cd book_management_system
```

2. Restore NuGet packages:
```bash
dotnet restore
```

3. Set up PostgreSQL database (see Database Setup above)

4. Update connection string in `Singleton/DbConnectionManager.cs`

5. Run the application:
```bash
dotnet run
```

## 🏗️ Building

To build the application:
```bash
dotnet build
```

To build a release version:
```bash
dotnet build -c Release
```

## 📝 Default Credentials

The database schema includes sample users. Update passwords in production!

- **Admin**: username: `admin`
- **Cashier**: username: `cashier1` or `cashier2`

## 🛠️ Technologies Used

- **C#** - Programming language
- **Windows Forms** - UI framework
- **PostgreSQL** - Database
- **Npgsql** - PostgreSQL .NET driver
- **.NET 8.0** - Framework

## 📄 License

This project is open source and available for educational purposes.

## 👤 Author

YIN CHHAPORNROTH

## 📧 Support

For issues or questions, please open an issue on GitHub.
