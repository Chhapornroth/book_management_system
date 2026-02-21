using System;
using System.Collections.Generic;
using Npgsql;
using WindowsFormsApp.Models;
using WindowsFormsApp.Singleton;

namespace WindowsFormsApp.Data
{
    /// <summary>
    /// Data Access Layer for Employees
    /// </summary>
    public class EmployeeRepository
    {
        public List<Employee> GetAllEmployees()
        {
            var employees = new List<Employee>();
            using var conn = DbConnectionManager.Instance.CreateConnection();
            conn.Open();
            
            using var cmd = new NpgsqlCommand("SELECT employee_id, name, gender, phone_number, birthday FROM employees ORDER BY employee_id", conn);
            using var reader = cmd.ExecuteReader();
            
            while (reader.Read())
            {
                employees.Add(new Employee
                {
                    EmployeeId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Gender = reader.GetString(2),
                    PhoneNumber = reader.GetString(3),
                    Birthday = reader.GetDateTime(4)
                });
            }
            
            return employees;
        }

        public Employee GetEmployeeById(int employeeId)
        {
            using var conn = DbConnectionManager.Instance.CreateConnection();
            conn.Open();
            
            using var cmd = new NpgsqlCommand("SELECT employee_id, name, gender, phone_number, birthday FROM employees WHERE employee_id = @id", conn);
            cmd.Parameters.AddWithValue("id", employeeId);
            using var reader = cmd.ExecuteReader();
            
            if (reader.Read())
            {
                return new Employee
                {
                    EmployeeId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Gender = reader.GetString(2),
                    PhoneNumber = reader.GetString(3),
                    Birthday = reader.GetDateTime(4)
                };
            }
            
            return null;
        }

        public Employee GetEmployeeByPhone(string phoneNumber)
        {
            using var conn = DbConnectionManager.Instance.CreateConnection();
            conn.Open();
            
            using var cmd = new NpgsqlCommand("SELECT employee_id, name, gender, phone_number, birthday FROM employees WHERE phone_number = @phone", conn);
            cmd.Parameters.AddWithValue("phone", phoneNumber);
            using var reader = cmd.ExecuteReader();
            
            if (reader.Read())
            {
                return new Employee
                {
                    EmployeeId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Gender = reader.GetString(2),
                    PhoneNumber = reader.GetString(3),
                    Birthday = reader.GetDateTime(4)
                };
            }
            
            return null;
        }

        public int AddEmployee(Employee employee)
        {
            if (employee == null) throw new ArgumentNullException(nameof(employee));
            if (string.IsNullOrWhiteSpace(employee.Name)) throw new ArgumentException("Employee name cannot be empty", nameof(employee));
            if (string.IsNullOrWhiteSpace(employee.Gender)) throw new ArgumentException("Gender cannot be empty", nameof(employee));
            if (string.IsNullOrWhiteSpace(employee.PhoneNumber)) throw new ArgumentException("Phone number cannot be empty", nameof(employee));
            if (employee.Birthday > DateTime.Now.Date) throw new ArgumentException("Birthday cannot be in the future", nameof(employee));
            if (employee.Birthday < DateTime.Now.AddYears(-100).Date) throw new ArgumentException("Birthday cannot be more than 100 years in the past", nameof(employee));
            // Check minimum working age (16 years)
            if (employee.Birthday > DateTime.Now.AddYears(-16).Date) throw new ArgumentException("Employee must be at least 16 years old", nameof(employee));

            using var conn = DbConnectionManager.Instance.CreateConnection();
            conn.Open();
            
            // Check for duplicate phone number
            using var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM employees WHERE phone_number = @phone", conn);
            checkCmd.Parameters.AddWithValue("phone", employee.PhoneNumber);
            var count = Convert.ToInt32(checkCmd.ExecuteScalar());
            if (count > 0)
            {
                throw new InvalidOperationException("Phone number already exists");
            }
            
            using var cmd = new NpgsqlCommand(
                "INSERT INTO employees (name, gender, phone_number, birthday) VALUES (@name, @gender, @phone, @birthday) RETURNING employee_id",
                conn);
            cmd.Parameters.AddWithValue("name", employee.Name);
            cmd.Parameters.AddWithValue("gender", employee.Gender);
            cmd.Parameters.AddWithValue("phone", employee.PhoneNumber);
            cmd.Parameters.AddWithValue("birthday", employee.Birthday);
            
            var result = cmd.ExecuteScalar();
            if (result == null || result == DBNull.Value)
                throw new InvalidOperationException("Failed to create employee - no ID returned");
            
            return Convert.ToInt32(result);
        }

        public bool UpdateEmployee(Employee employee)
        {
            if (employee == null) throw new ArgumentNullException(nameof(employee));
            if (string.IsNullOrWhiteSpace(employee.Name)) throw new ArgumentException("Employee name cannot be empty", nameof(employee));
            if (string.IsNullOrWhiteSpace(employee.Gender)) throw new ArgumentException("Gender cannot be empty", nameof(employee));
            if (string.IsNullOrWhiteSpace(employee.PhoneNumber)) throw new ArgumentException("Phone number cannot be empty", nameof(employee));
            if (employee.Birthday > DateTime.Now.Date) throw new ArgumentException("Birthday cannot be in the future", nameof(employee));
            if (employee.Birthday < DateTime.Now.AddYears(-100).Date) throw new ArgumentException("Birthday cannot be more than 100 years in the past", nameof(employee));
            if (employee.EmployeeId <= 0) throw new ArgumentException("Invalid employee ID", nameof(employee));
            // Check minimum working age (16 years)
            if (employee.Birthday > DateTime.Now.AddYears(-16).Date) throw new ArgumentException("Employee must be at least 16 years old", nameof(employee));

            using var conn = DbConnectionManager.Instance.CreateConnection();
            conn.Open();
            
            // Check for duplicate phone number (excluding current employee)
            using var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM employees WHERE phone_number = @phone AND employee_id != @id", conn);
            checkCmd.Parameters.AddWithValue("phone", employee.PhoneNumber);
            checkCmd.Parameters.AddWithValue("id", employee.EmployeeId);
            var count = Convert.ToInt32(checkCmd.ExecuteScalar());
            if (count > 0)
            {
                throw new InvalidOperationException("Phone number already exists");
            }
            
            using var cmd = new NpgsqlCommand(
                "UPDATE employees SET name = @name, gender = @gender, phone_number = @phone, birthday = @birthday WHERE employee_id = @id",
                conn);
            cmd.Parameters.AddWithValue("id", employee.EmployeeId);
            cmd.Parameters.AddWithValue("name", employee.Name);
            cmd.Parameters.AddWithValue("gender", employee.Gender);
            cmd.Parameters.AddWithValue("phone", employee.PhoneNumber);
            cmd.Parameters.AddWithValue("birthday", employee.Birthday);
            
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool DeleteEmployee(int employeeId)
        {
            using var conn = DbConnectionManager.Instance.CreateConnection();
            conn.Open();
            
            using var cmd = new NpgsqlCommand("DELETE FROM employees WHERE employee_id = @id", conn);
            cmd.Parameters.AddWithValue("id", employeeId);
            
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}

