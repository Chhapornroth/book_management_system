-- =====================================================
-- PostgreSQL CREATE TABLE Scripts
-- BookStore Management System
-- =====================================================
-- 
-- Run this script to create all required tables:
-- psql -U postgres -d bookstore_db -f create_tables.sql
-- 
-- Or execute in pgAdmin / DBeaver / any PostgreSQL client
-- =====================================================

-- =====================================================
-- Table: users
-- Purpose: User authentication and login credentials
-- =====================================================
CREATE TABLE users (
    user_id SERIAL PRIMARY KEY,
    username VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    role VARCHAR(20) NOT NULL CHECK (role IN ('Admin', 'Cashier')),
    employee_id INTEGER,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    last_login TIMESTAMP,
    is_active BOOLEAN DEFAULT TRUE
);

-- =====================================================
-- Table: employees
-- Purpose: Employee records and information
-- =====================================================
CREATE TABLE employees (
    employee_id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    gender VARCHAR(6) NOT NULL CHECK (gender IN ('Male', 'Female')),
    phone_number VARCHAR(15) NOT NULL UNIQUE,
    birthday DATE NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- =====================================================
-- Table: books
-- Purpose: Book inventory and records
-- =====================================================
CREATE TABLE books (
    book_id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    author_name VARCHAR(255) NOT NULL,
    stock INTEGER NOT NULL CHECK (stock >= 0),
    adding_date DATE NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- =====================================================
-- Table: sales
-- Purpose: Transaction records for book sales
-- =====================================================
CREATE TABLE sales (
    sale_id SERIAL PRIMARY KEY,
    customer_name VARCHAR(255) NOT NULL,
    book_id INTEGER NOT NULL,
    employee_id INTEGER NOT NULL,
    price NUMERIC(10, 2) NOT NULL CHECK (price >= 0),
    quantity INTEGER NOT NULL CHECK (quantity > 0),
    discount NUMERIC(5, 4) NOT NULL DEFAULT 0 CHECK (discount >= 0 AND discount <= 1),
    total NUMERIC(10, 2) GENERATED ALWAYS AS (
        CASE 
            WHEN discount <= 0 THEN (price * quantity)
            ELSE (price * quantity) - ((price * quantity) * discount)
        END
    ) STORED,
    sale_date DATE NOT NULL DEFAULT CURRENT_DATE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    -- Foreign Key Constraints
    CONSTRAINT fk_sales_book FOREIGN KEY (book_id) 
        REFERENCES books(book_id) ON DELETE RESTRICT,
    CONSTRAINT fk_sales_employee FOREIGN KEY (employee_id) 
        REFERENCES employees(employee_id) ON DELETE RESTRICT
);

-- =====================================================
-- Foreign Key: users -> employees
-- =====================================================
ALTER TABLE users 
ADD CONSTRAINT fk_users_employee 
FOREIGN KEY (employee_id) 
REFERENCES employees(employee_id) 
ON DELETE SET NULL;

-- =====================================================
-- Indexes for Performance
-- =====================================================
CREATE INDEX idx_books_title ON books(title);
CREATE INDEX idx_books_author ON books(author_name);
CREATE INDEX idx_employees_phone ON employees(phone_number);
CREATE INDEX idx_sales_date ON sales(sale_date);
CREATE INDEX idx_sales_employee ON sales(employee_id);
CREATE INDEX idx_sales_book ON sales(book_id);
CREATE INDEX idx_users_username ON users(username);

