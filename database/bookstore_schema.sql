-- =====================================================
-- PostgreSQL Database Schema for BookStore Management System
-- =====================================================
-- Database: bookstore_db
-- 
-- Connection String Template:
-- Host=localhost;Port=5432;Database=bookstore_db;Username=postgres;Password=your_password;
-- =====================================================

-- Drop existing tables if they exist (in reverse order due to foreign keys)
DROP TABLE IF EXISTS sales CASCADE;
DROP TABLE IF EXISTS books CASCADE;
DROP TABLE IF EXISTS employees CASCADE;
DROP TABLE IF EXISTS users CASCADE;

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

-- =====================================================
-- Sample Data Insertion
-- =====================================================

-- Insert Sample Employees
INSERT INTO employees (name, gender, phone_number, birthday) VALUES
('YIN CHHAPORNROTH', 'Male', '069305880', '2004-09-15'),
('John Doe', 'Male', '1234567890', '1990-01-15'),
('Jane Smith', 'Female', '0987654321', '1992-05-20');

-- Insert Sample Books
INSERT INTO books (title, author_name, stock, adding_date) VALUES
('Think and Grow Rich', 'Napoleon Hill', 4, '2024-03-03'),
('Everything Is F***ed', 'Mark Manson', 5, '2024-05-15'),
('What You Think of Me is None of My Business', 'Terry Cole-Whittaker', 5, '2024-12-24'),
('The 7 Habits of Highly Effective People', 'Stephen R. Covey', 10, '2024-01-10'),
('Atomic Habits', 'James Clear', 8, '2024-02-20');

-- Insert Sample Users (password_hash is placeholder - use proper hashing in production)
-- Default passwords: Admin123! and Cashier123!
INSERT INTO users (username, password_hash, role, employee_id) VALUES
('admin', '$2a$10$placeholder_hash_for_admin', 'Admin', 1),
('cashier1', '$2a$10$placeholder_hash_for_cashier', 'Cashier', 2),
('cashier2', '$2a$10$placeholder_hash_for_cashier2', 'Cashier', 3);

-- Insert Sample Sales
INSERT INTO sales (customer_name, book_id, employee_id, price, quantity, discount, sale_date) VALUES
('Sebastian', 3, 1, 15.00, 2, 0.05, '2024-08-18'),
('Tom', 2, 2, 10.00, 2, 0.00, '2024-08-18'),
('Alfia', 1, 2, 20.00, 3, 0.20, '2024-08-18'),
('Sarah', 4, 1, 25.00, 1, 0.10, '2024-08-19'),
('Mike', 5, 3, 18.00, 2, 0.00, '2024-08-19');

-- =====================================================
-- Views for Common Queries
-- =====================================================

-- View: Sales Summary
CREATE OR REPLACE VIEW sales_summary AS
SELECT 
    s.sale_id,
    s.customer_name,
    b.title AS book_title,
    e.name AS employee_name,
    s.price,
    s.quantity,
    s.discount,
    s.total,
    s.sale_date
FROM sales s
JOIN books b ON s.book_id = b.book_id
JOIN employees e ON s.employee_id = e.employee_id
ORDER BY s.sale_date DESC, s.sale_id DESC;

-- View: Book Inventory Status
CREATE OR REPLACE VIEW book_inventory_status AS
SELECT 
    b.book_id,
    b.title,
    b.author_name,
    b.stock,
    COALESCE(SUM(s.quantity), 0) AS total_sold,
    b.adding_date
FROM books b
LEFT JOIN sales s ON b.book_id = s.book_id
GROUP BY b.book_id, b.title, b.author_name, b.stock, b.adding_date
ORDER BY b.title;

-- =====================================================
-- Stored Procedures / Functions
-- =====================================================

-- Function: Get Total Sales for a Date Range
CREATE OR REPLACE FUNCTION get_total_sales(start_date DATE, end_date DATE)
RETURNS NUMERIC(10, 2) AS $$
BEGIN
    RETURN COALESCE(
        (SELECT SUM(total) FROM sales 
         WHERE sale_date BETWEEN start_date AND end_date),
        0
    );
END;
$$ LANGUAGE plpgsql;

-- Function: Get Low Stock Books (stock < threshold)
CREATE OR REPLACE FUNCTION get_low_stock_books(threshold INTEGER DEFAULT 5)
RETURNS TABLE (
    book_id INTEGER,
    title VARCHAR(255),
    author_name VARCHAR(255),
    stock INTEGER
) AS $$
BEGIN
    RETURN QUERY
    SELECT b.book_id, b.title, b.author_name, b.stock
    FROM books b
    WHERE b.stock < threshold
    ORDER BY b.stock ASC;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- Comments on Tables
-- =====================================================
COMMENT ON TABLE users IS 'User accounts for login authentication';
COMMENT ON TABLE employees IS 'Employee records and personal information';
COMMENT ON TABLE books IS 'Book inventory and catalog';
COMMENT ON TABLE sales IS 'Sales transactions and records';

COMMENT ON COLUMN sales.discount IS 'Discount as decimal (0.05 = 5%, 0.20 = 20%)';
COMMENT ON COLUMN sales.total IS 'Calculated total: (price * quantity) - (price * quantity * discount)';

-- =====================================================
-- End of Schema
-- =====================================================

