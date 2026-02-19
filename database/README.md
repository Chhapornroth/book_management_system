# PostgreSQL Database Setup for BookStore Management System

## Prerequisites

- PostgreSQL 12 or later installed
- PostgreSQL client tools (psql) or GUI tool (pgAdmin, DBeaver)

## Database Setup Steps

### 1. Create Database

```sql
CREATE DATABASE bookstore_db;
```

Or via command line:
```bash
createdb -U postgres bookstore_db
```

### 2. Run Schema Script

**Option A: Using psql command line**
```bash
psql -U postgres -d bookstore_db -f bookstore_schema.sql
```

**Option B: Using pgAdmin**
1. Open pgAdmin
2. Connect to your PostgreSQL server
3. Right-click on `bookstore_db` → Query Tool
4. Open and execute `bookstore_schema.sql`

**Option C: Using DBeaver**
1. Connect to PostgreSQL
2. Select `bookstore_db` database
3. Right-click → SQL Editor → New SQL Script
4. Open and execute `bookstore_schema.sql`

### 3. Verify Tables Created

```sql
\dt
```

Should show:
- users
- employees
- books
- sales

## Connection String

Update your `appsettings.json` or configuration file with:

```
Host=localhost;Port=5432;Database=bookstore_db;Username=postgres;Password=your_password;
```

## Table Structure

### users
- `user_id` (SERIAL PRIMARY KEY)
- `username` (VARCHAR, UNIQUE)
- `password_hash` (VARCHAR)
- `role` (VARCHAR: 'Admin' or 'Cashier')
- `employee_id` (INTEGER, FK to employees)
- `created_at` (TIMESTAMP)
- `last_login` (TIMESTAMP)
- `is_active` (BOOLEAN)

### employees
- `employee_id` (SERIAL PRIMARY KEY)
- `name` (VARCHAR)
- `gender` (VARCHAR: 'Male' or 'Female')
- `phone_number` (VARCHAR, UNIQUE)
- `birthday` (DATE)
- `created_at` (TIMESTAMP)

### books
- `book_id` (SERIAL PRIMARY KEY)
- `title` (VARCHAR)
- `author_name` (VARCHAR)
- `stock` (INTEGER, >= 0)
- `adding_date` (DATE)
- `created_at` (TIMESTAMP)

### sales
- `sale_id` (SERIAL PRIMARY KEY)
- `customer_name` (VARCHAR)
- `book_id` (INTEGER, FK to books)
- `employee_id` (INTEGER, FK to employees)
- `price` (NUMERIC(10,2))
- `quantity` (INTEGER)
- `discount` (NUMERIC(5,4), 0-1 range)
- `total` (NUMERIC(10,2), GENERATED)
- `sale_date` (DATE)
- `created_at` (TIMESTAMP)

## Sample Data

The `bookstore_schema.sql` includes sample data:
- 3 employees
- 5 books
- 3 users (admin and cashiers)
- 5 sales transactions

## Notes

- The `total` column in `sales` is a **generated column** that automatically calculates: `(price * quantity) - (price * quantity * discount)`
- Foreign keys ensure referential integrity
- Indexes are created for performance on frequently queried columns
- All tables include `created_at` timestamps for auditing

## Troubleshooting

**Error: "relation already exists"**
- Drop existing tables first: `DROP TABLE IF EXISTS sales, books, employees, users CASCADE;`

**Error: "permission denied"**
- Ensure you're connected as a user with CREATE privileges
- Or run as postgres superuser

**Error: "database does not exist"**
- Create the database first: `CREATE DATABASE bookstore_db;`

