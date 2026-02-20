# Project Setup Guide

This guide will walk you through setting up the BookStore Management System from scratch.

## Prerequisites

Before you begin, ensure you have the following installed on your system:

1. **.NET 8.0 SDK** or later
   - Download from: https://dotnet.microsoft.com/download/dotnet/8.0
   - Verify installation: `dotnet --version` (should show 8.0.x or later)

2. **PostgreSQL 12** or later
   - Download from: https://www.postgresql.org/download/windows/
   - During installation, remember your PostgreSQL password (default user: `postgres`)
   - Verify installation: `psql --version`

3. **Windows OS** (Windows 10/11 recommended)

4. **Code Editor** (optional but recommended)
   - Visual Studio 2022 or later
   - Visual Studio Code with C# extension
   - JetBrains Rider

## Step-by-Step Setup

### Step 1: Clone or Download the Project

If using Git:
```bash
git clone <repository-url>
cd CSharpe
```

Or simply navigate to the project directory if you already have it.

### Step 2: Install .NET Dependencies

Restore NuGet packages:
```bash
dotnet restore
```

This will automatically download the required packages (Npgsql 8.0.3) as specified in `WindowsFormsApp.csproj`.

### Step 3: Set Up PostgreSQL Database

#### 3.1 Create the Database

Open PostgreSQL command line (psql) or pgAdmin, and run:

```sql
CREATE DATABASE bookstore_db;
```

#### 3.2 Run the Schema Script

Execute the database schema script to create all tables, views, and sample data:

**Using psql command line:**
```bash
psql -U postgres -d bookstore_db -f database/bookstore_schema.sql
```

**Using pgAdmin:**
1. Open pgAdmin
2. Connect to your PostgreSQL server
3. Right-click on `bookstore_db` → Query Tool
4. Open `database/bookstore_schema.sql`
5. Execute the script (F5)

**Using psql with password prompt:**
```bash
psql -U postgres -d bookstore_db -f database/bookstore_schema.sql
```

The script will:
- Create all necessary tables (users, employees, books, sales)
- Set up foreign key relationships
- Create indexes for performance
- Insert sample data
- Create views and stored procedures

### Step 4: Configure Database Connection

Update the connection string in `Singleton/DbConnectionManager.cs`:

1. Open `Singleton/DbConnectionManager.cs`
2. Find the `_connectionString` field (around line 16)
3. Update it with your PostgreSQL credentials:

```csharp
private readonly string _connectionString =
    "Host=localhost;Port=5432;Database=bookstore_db;Username=postgres;Password=YOUR_PASSWORD;";
```

Replace:
- `YOUR_PASSWORD` with your actual PostgreSQL password
- `localhost` if your PostgreSQL server is on a different host
- `5432` if PostgreSQL is running on a different port
- `postgres` if you're using a different username

**Example:**
```csharp
private readonly string _connectionString =
    "Host=localhost;Port=5432;Database=bookstore_db;Username=postgres;Password=mypassword123;";
```

### Step 5: Verify the Setup

#### 5.1 Test Database Connection

You can verify the database connection by building the project:

```bash
dotnet build
```

If there are no errors, the project is configured correctly.

#### 5.2 Verify Database Tables

Connect to PostgreSQL and verify tables exist:

```sql
\c bookstore_db
\dt
```

You should see tables: `books`, `employees`, `sales`, `users`

### Step 6: Run the Application

#### Option 1: Using dotnet CLI
```bash
dotnet run
```

#### Option 2: Using Visual Studio
1. Open `WindowsFormsApp.csproj` or the solution file in Visual Studio
2. Press F5 or click "Start Debugging"

#### Option 3: Build and Run Executable
```bash
dotnet build -c Release
```

The executable will be in: `bin/Release/net8.0-windows/WindowsFormsApp.exe`

### Step 7: Login to the Application

The database schema includes sample users. Use these credentials to log in:

**Admin Account:**
- Username: `admin`
- Password: Check the database schema for the actual password hash (you may need to update this)

**Cashier Accounts:**
- Username: `cashier1` or `cashier2`
- Password: Check the database schema

**⚠️ Important:** The sample users have placeholder password hashes. For production use, you should:
1. Implement proper password hashing (e.g., BCrypt)
2. Create new users with secure passwords
3. Update the authentication logic in the login form

## Troubleshooting

### Issue: "Could not connect to database"

**Solutions:**
1. Verify PostgreSQL is running:
   ```bash
   # Check PostgreSQL service status
   # Windows: Services → PostgreSQL
   ```

2. Verify connection string in `DbConnectionManager.cs`:
   - Check host, port, database name, username, and password
   - Ensure no extra spaces or typos

3. Test connection manually:
   ```bash
   psql -U postgres -d bookstore_db
   ```

### Issue: "Package restore failed"

**Solutions:**
1. Check internet connection
2. Clear NuGet cache:
   ```bash
   dotnet nuget locals all --clear
   ```
3. Restore again:
   ```bash
   dotnet restore
   ```

### Issue: "Database does not exist"

**Solutions:**
1. Create the database:
   ```sql
   CREATE DATABASE bookstore_db;
   ```
2. Run the schema script again

### Issue: "Table already exists" errors

**Solutions:**
The schema script includes `DROP TABLE IF EXISTS` statements, so this shouldn't happen. If it does:
1. Drop the database and recreate it:
   ```sql
   DROP DATABASE bookstore_db;
   CREATE DATABASE bookstore_db;
   ```
2. Run the schema script again

### Issue: Application crashes on startup

**Solutions:**
1. Check the connection string is correct
2. Verify PostgreSQL is running
3. Check Windows Event Viewer for detailed error messages
4. Run from command line to see console errors:
   ```bash
   dotnet run
   ```

## Project Structure

```
CSharpe/
├── Data/                    # Data Access Layer (Repositories)
│   ├── BookRepository.cs
│   ├── EmployeeRepository.cs
│   └── SaleRepository.cs
├── Models/                  # Domain Models
│   ├── Book.cs
│   ├── Employee.cs
│   ├── Sale.cs
│   └── User.cs
├── Forms/                   # Windows Forms UI
│   ├── AdminForm.cs
│   ├── CashierForm.cs
│   ├── DashboardForm.cs
│   └── LoginForm.cs
├── Singleton/               # Singleton Pattern
│   └── DbConnectionManager.cs
├── Factory/                 # Factory Pattern
│   └── UserFactory.cs
├── Builder/                 # Builder Pattern
│   └── SaleBuilder.cs
├── Strategy/                # Strategy Pattern
│   ├── IPaymentStrategy.cs
│   ├── CashPaymentStrategy.cs
│   └── CardPaymentStrategy.cs
├── Observer/                # Observer Pattern
│   ├── ISaleObserver.cs
│   ├── SaleNotifier.cs
│   └── LoggingSaleObserver.cs
├── database/                # Database Scripts
│   ├── bookstore_schema.sql
│   └── create_tables.sql
├── Program.cs               # Application Entry Point
├── WindowsFormsApp.csproj   # Project File
└── SETUP.md                 # This file
```

## Next Steps

After successful setup:

1. **Explore the Application:**
   - Log in as admin to manage books and employees
   - Log in as cashier to process sales

2. **Customize:**
   - Update UI colors and styles
   - Add new features
   - Modify business logic

3. **Production Deployment:**
   - Implement proper password hashing
   - Set up connection string from configuration file (appsettings.json)
   - Add error logging
   - Set up database backups

## Additional Resources

- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Npgsql Documentation](https://www.npgsql.org/doc/)
- [Windows Forms Documentation](https://docs.microsoft.com/dotnet/desktop/winforms/)

## Support

If you encounter issues during setup:
1. Check the troubleshooting section above
2. Review error messages carefully
3. Verify all prerequisites are installed correctly
4. Check the `PROJECT_STATUS.md` file for project status

---

**Last Updated:** 2024
**Project Version:** 1.0

