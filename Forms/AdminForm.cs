using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp.Data;
using WindowsFormsApp.Models;

namespace WindowsFormsApp.Forms
{
    /// <summary>
    /// Admin Form - CRUD operations for Books, Employees, and Sales
    /// </summary>
    public partial class AdminForm : Form
    {
        private TabControl tabControl;
        private DataGridView dgvBooks, dgvEmployees, dgvSales;
        private TextBox txtBookTitle, txtBookAuthor, txtBookStock, txtBookId;
        private TextBox txtEmployeeName, txtEmployeePhone, txtEmployeeId;
        private DateTimePicker dtpBookDate, dtpEmployeeBirthday;
        private ComboBox cmbEmployeeGender;
        private Button btnAddBook, btnUpdateBook, btnDeleteBook;
        private Button btnAddEmployee, btnUpdateEmployee, btnDeleteEmployee;
        private Button btnDeleteSale, btnLogout;
        private TextBox txtSearchBooks, txtSearchEmployees, txtSearchSales;
        private readonly BookRepository _bookRepo = new();
        private readonly EmployeeRepository _employeeRepo = new();
        private readonly SaleRepository _saleRepo = new();
        private readonly User _currentUser;

        public AdminForm(User user)
        {
            _currentUser = user;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = $"Admin Dashboard - Welcome {_currentUser.FullName}";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // Header Panel
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(52, 152, 219)
            };

            var welcomeLabel = new Label
            {
                Text = $"👤 Welcome, {_currentUser.FullName}",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            btnLogout = new Button
            {
                Text = "🚪 Logout",
                Size = new Size(120, 40),
                Location = new Point(this.Width - 140, 15),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 57, 43);
            btnLogout.Click += (s, e) => { this.Close(); new DashboardForm().Show(); };

            headerPanel.Controls.Add(welcomeLabel);
            headerPanel.Controls.Add(btnLogout);

            // Tab Control with modern styling
            tabControl = new TabControl 
            { 
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                Appearance = TabAppearance.FlatButtons
            };
            
            // Books Tab
            var tabBooks = new TabPage("📚 Books");
            CreateBooksTab(tabBooks);
            
            // Employees Tab
            var tabEmployees = new TabPage("👥 Employees");
            CreateEmployeesTab(tabEmployees);
            
            // Sales Tab
            var tabSales = new TabPage("💰 Sales");
            CreateSalesTab(tabSales);

            tabControl.TabPages.Add(tabBooks);
            tabControl.TabPages.Add(tabEmployees);
            tabControl.TabPages.Add(tabSales);

            this.Controls.Add(tabControl);
            this.Controls.Add(headerPanel);
        }

        private void CreateBooksTab(TabPage tab)
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(245, 247, 250), Padding = new Padding(15) };
            
            // Input Panel
            var inputPanel = new Panel 
            { 
                Height = 180, 
                Dock = DockStyle.Top, 
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            var inputTitle = new Label
            {
                Text = "📚 Book Information",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(15, 15)
            };
            
            txtBookId = new TextBox 
            { 
                Location = new Point(15, 45), 
                Size = new Size(80, 32), 
                Enabled = false, 
                Text = "Auto",
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(236, 240, 241),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtBookTitle = new TextBox 
            { 
                Location = new Point(110, 45), 
                Size = new Size(220, 32), 
                PlaceholderText = "Book Title",
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtBookAuthor = new TextBox 
            { 
                Location = new Point(340, 45), 
                Size = new Size(220, 32), 
                PlaceholderText = "Author Name",
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtBookStock = new TextBox 
            { 
                Location = new Point(570, 45), 
                Size = new Size(100, 32), 
                PlaceholderText = "Stock",
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle
            };
            dtpBookDate = new DateTimePicker 
            { 
                Location = new Point(680, 45), 
                Size = new Size(150, 32),
                Value = DateTime.Now.Date,
                MaxDate = DateTime.Now.Date,
                MinDate = DateTime.Now.AddYears(-50).Date,
                Font = new Font("Segoe UI", 10F)
            };
            
            btnAddBook = new Button 
            { 
                Text = "➕ Add", 
                Location = new Point(15, 95), 
                Size = new Size(100, 38),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAddBook.FlatAppearance.BorderSize = 0;
            btnAddBook.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 174, 96);
            btnUpdateBook = new Button 
            { 
                Text = "✏️ Update", 
                Location = new Point(125, 95), 
                Size = new Size(100, 38),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnUpdateBook.FlatAppearance.BorderSize = 0;
            btnUpdateBook.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 128, 185);
            btnDeleteBook = new Button 
            { 
                Text = "🗑️ Delete", 
                Location = new Point(235, 95), 
                Size = new Size(100, 38),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDeleteBook.FlatAppearance.BorderSize = 0;
            btnDeleteBook.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 57, 43);
            
            btnAddBook.Click += BtnAddBook_Click;
            btnUpdateBook.Click += BtnUpdateBook_Click;
            btnDeleteBook.Click += BtnDeleteBook_Click;
            
            var searchLabel = new Label
            {
                Text = "🔍 Search:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(15, 145)
            };
            
            txtSearchBooks = new TextBox 
            { 
                Location = new Point(90, 142), 
                Size = new Size(350, 32), 
                PlaceholderText = "Search by title, author...",
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearchBooks.TextChanged += (s, e) => FilterBooks();
            
            inputPanel.Controls.AddRange(new Control[] { 
                inputTitle,
                new Label { Text = "ID", Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(127, 140, 141), Location = new Point(15, 25), AutoSize = true },
                txtBookId,
                new Label { Text = "Title", Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(127, 140, 141), Location = new Point(110, 25), AutoSize = true },
                txtBookTitle,
                new Label { Text = "Author", Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(127, 140, 141), Location = new Point(340, 25), AutoSize = true },
                txtBookAuthor,
                new Label { Text = "Stock", Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(127, 140, 141), Location = new Point(570, 25), AutoSize = true },
                txtBookStock,
                new Label { Text = "Date", Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(127, 140, 141), Location = new Point(680, 25), AutoSize = true },
                dtpBookDate,
                btnAddBook, btnUpdateBook, btnDeleteBook,
                searchLabel,
                txtSearchBooks
            });
            
            // DataGridView with modern styling
            dgvBooks = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(236, 240, 241),
                Font = new Font("Segoe UI", 10F),
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.White,
                    ForeColor = Color.FromArgb(52, 73, 94),
                    SelectionBackColor = Color.FromArgb(52, 152, 219),
                    SelectionForeColor = Color.White,
                    Padding = new Padding(5)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(52, 73, 94),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(5)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(250, 250, 250)
                }
            };
            dgvBooks.CellClick += (s, e) => { if (e.RowIndex >= 0) LoadBookToForm(dgvBooks.Rows[e.RowIndex]); };
            
            panel.Controls.Add(dgvBooks);
            panel.Controls.Add(inputPanel);
            tab.Controls.Add(panel);
        }

        private void CreateEmployeesTab(TabPage tab)
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(245, 247, 250), Padding = new Padding(15) };
            
            var inputPanel = new Panel 
            { 
                Height = 180, 
                Dock = DockStyle.Top, 
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            var inputTitle = new Label
            {
                Text = "👥 Employee Information",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(15, 15)
            };
            
            txtEmployeeId = new TextBox 
            { 
                Location = new Point(15, 45), 
                Size = new Size(80, 32), 
                Enabled = false, 
                Text = "Auto",
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(236, 240, 241),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtEmployeeName = new TextBox 
            { 
                Location = new Point(110, 45), 
                Size = new Size(220, 32), 
                PlaceholderText = "Full Name",
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle
            };
            cmbEmployeeGender = new ComboBox 
            { 
                Location = new Point(340, 45), 
                Size = new Size(120, 32),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            cmbEmployeeGender.Items.AddRange(new[] { "Male", "Female" });
            txtEmployeePhone = new TextBox 
            { 
                Location = new Point(470, 45), 
                Size = new Size(150, 32), 
                PlaceholderText = "Phone Number",
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle
            };
            dtpEmployeeBirthday = new DateTimePicker 
            { 
                Location = new Point(630, 45), 
                Size = new Size(150, 32),
                Value = DateTime.Now.AddYears(-25).Date,
                MaxDate = DateTime.Now.Date,
                MinDate = DateTime.Now.AddYears(-100).Date,
                Font = new Font("Segoe UI", 10F)
            };
            
            btnAddEmployee = new Button 
            { 
                Text = "➕ Add", 
                Location = new Point(15, 95), 
                Size = new Size(100, 38),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAddEmployee.FlatAppearance.BorderSize = 0;
            btnAddEmployee.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 174, 96);
            btnUpdateEmployee = new Button 
            { 
                Text = "✏️ Update", 
                Location = new Point(125, 95), 
                Size = new Size(100, 38),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnUpdateEmployee.FlatAppearance.BorderSize = 0;
            btnUpdateEmployee.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 128, 185);
            btnDeleteEmployee = new Button 
            { 
                Text = "🗑️ Delete", 
                Location = new Point(235, 95), 
                Size = new Size(100, 38),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDeleteEmployee.FlatAppearance.BorderSize = 0;
            btnDeleteEmployee.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 57, 43);
            
            btnAddEmployee.Click += BtnAddEmployee_Click;
            btnUpdateEmployee.Click += BtnUpdateEmployee_Click;
            btnDeleteEmployee.Click += BtnDeleteEmployee_Click;
            
            var searchLabel = new Label
            {
                Text = "🔍 Search:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(15, 145)
            };
            
            txtSearchEmployees = new TextBox 
            { 
                Location = new Point(90, 142), 
                Size = new Size(350, 32), 
                PlaceholderText = "Search by name, phone...",
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearchEmployees.TextChanged += (s, e) => FilterEmployees();
            
            inputPanel.Controls.AddRange(new Control[] {
                inputTitle,
                new Label { Text = "ID", Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(127, 140, 141), Location = new Point(15, 25), AutoSize = true },
                txtEmployeeId,
                new Label { Text = "Name", Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(127, 140, 141), Location = new Point(110, 25), AutoSize = true },
                txtEmployeeName,
                new Label { Text = "Gender", Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(127, 140, 141), Location = new Point(340, 25), AutoSize = true },
                cmbEmployeeGender,
                new Label { Text = "Phone", Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(127, 140, 141), Location = new Point(470, 25), AutoSize = true },
                txtEmployeePhone,
                new Label { Text = "Birthday", Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(127, 140, 141), Location = new Point(630, 25), AutoSize = true },
                dtpEmployeeBirthday,
                btnAddEmployee, btnUpdateEmployee, btnDeleteEmployee,
                searchLabel,
                txtSearchEmployees
            });
            
            dgvEmployees = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(236, 240, 241),
                Font = new Font("Segoe UI", 10F),
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.White,
                    ForeColor = Color.FromArgb(52, 73, 94),
                    SelectionBackColor = Color.FromArgb(52, 152, 219),
                    SelectionForeColor = Color.White,
                    Padding = new Padding(5)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(52, 73, 94),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(5)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(250, 250, 250)
                }
            };
            dgvEmployees.CellClick += (s, e) => { if (e.RowIndex >= 0) LoadEmployeeToForm(dgvEmployees.Rows[e.RowIndex]); };
            
            panel.Controls.Add(dgvEmployees);
            panel.Controls.Add(inputPanel);
            tab.Controls.Add(panel);
        }

        private void CreateSalesTab(TabPage tab)
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(245, 247, 250), Padding = new Padding(15) };
            
            var inputPanel = new Panel 
            { 
                Height = 100, 
                Dock = DockStyle.Top, 
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            var inputTitle = new Label
            {
                Text = "💰 Sales Transactions",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(15, 15)
            };
            
            btnDeleteSale = new Button 
            { 
                Text = "🗑️ Delete Selected", 
                Location = new Point(15, 50), 
                Size = new Size(140, 38),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDeleteSale.FlatAppearance.BorderSize = 0;
            btnDeleteSale.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 57, 43);
            btnDeleteSale.Click += BtnDeleteSale_Click;
            
            var searchLabel = new Label
            {
                Text = "🔍 Search:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(170, 58)
            };
            
            txtSearchSales = new TextBox 
            { 
                Location = new Point(240, 55), 
                Size = new Size(350, 32), 
                PlaceholderText = "Search by customer, date...",
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearchSales.TextChanged += (s, e) => FilterSales();
            
            inputPanel.Controls.Add(inputTitle);
            inputPanel.Controls.Add(btnDeleteSale);
            inputPanel.Controls.Add(searchLabel);
            inputPanel.Controls.Add(txtSearchSales);
            
            dgvSales = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(236, 240, 241),
                Font = new Font("Segoe UI", 10F),
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.White,
                    ForeColor = Color.FromArgb(52, 73, 94),
                    SelectionBackColor = Color.FromArgb(52, 152, 219),
                    SelectionForeColor = Color.White,
                    Padding = new Padding(5)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(52, 73, 94),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(5)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(250, 250, 250)
                }
            };
            
            panel.Controls.Add(dgvSales);
            panel.Controls.Add(inputPanel);
            tab.Controls.Add(panel);
        }

        private void LoadData()
        {
            LoadBooks();
            LoadEmployees();
            LoadSales();
        }

        private void LoadBooks()
        {
            var books = _bookRepo.GetAllBooks();
            dgvBooks.DataSource = books.Select(b => new
            {
                b.BookId,
                b.Title,
                b.AuthorName,
                b.Stock,
                AddingDate = b.AddingDate.ToShortDateString()
            }).ToList();
        }

        private void LoadEmployees()
        {
            var employees = _employeeRepo.GetAllEmployees();
            dgvEmployees.DataSource = employees.Select(e => new
            {
                e.EmployeeId,
                e.Name,
                e.Gender,
                e.PhoneNumber,
                Birthday = e.Birthday.ToShortDateString()
            }).ToList();
        }

        private void LoadSales()
        {
            var sales = _saleRepo.GetAllSales();
            dgvSales.DataSource = sales.Select(s => new
            {
                s.SaleId,
                s.CustomerName,
                s.BookId,
                s.EmployeeId,
                Price = s.Price.ToString("C"),
                s.Quantity,
                Discount = (s.Discount * 100).ToString("F0") + "%",
                Total = s.Total.ToString("C"),
                SaleDate = s.SaleDate.ToShortDateString()
            }).ToList();
        }

        private void LoadBookToForm(DataGridViewRow row)
        {
            if (row == null || row.Cells.Count < 5) return;

            txtBookId.Text = row.Cells[0].Value?.ToString() ?? "";
            txtBookTitle.Text = row.Cells[1].Value?.ToString() ?? "";
            txtBookAuthor.Text = row.Cells[2].Value?.ToString() ?? "";
            txtBookStock.Text = row.Cells[3].Value?.ToString() ?? "";
            if (row.Cells[4].Value != null && DateTime.TryParse(row.Cells[4].Value.ToString(), out var date))
                dtpBookDate.Value = date;
        }

        private void LoadEmployeeToForm(DataGridViewRow row)
        {
            if (row == null || row.Cells.Count < 5) return;

            txtEmployeeId.Text = row.Cells[0].Value?.ToString() ?? "";
            txtEmployeeName.Text = row.Cells[1].Value?.ToString() ?? "";
            cmbEmployeeGender.Text = row.Cells[2].Value?.ToString() ?? "";
            txtEmployeePhone.Text = row.Cells[3].Value?.ToString() ?? "";
            if (row.Cells[4].Value != null && DateTime.TryParse(row.Cells[4].Value.ToString(), out var date))
                dtpEmployeeBirthday.Value = date;
        }

        private void BtnAddBook_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBookTitle.Text) || string.IsNullOrWhiteSpace(txtBookAuthor.Text) || !int.TryParse(txtBookStock.Text, out var stock))
            {
                MessageBox.Show("Please fill all fields correctly", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (stock < 0)
            {
                MessageBox.Show("Stock cannot be negative", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtpBookDate.Value.Date > DateTime.Now.Date)
            {
                MessageBox.Show("Adding date cannot be in the future", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtpBookDate.Value.Date < DateTime.Now.AddYears(-50).Date)
            {
                MessageBox.Show("Adding date cannot be more than 50 years in the past", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var book = new Book
                {
                    Title = txtBookTitle.Text.Trim(),
                    AuthorName = txtBookAuthor.Text.Trim(),
                    Stock = stock,
                    AddingDate = dtpBookDate.Value.Date
                };

                _bookRepo.AddBook(book);
                LoadBooks();
                ClearBookForm();
                MessageBox.Show("Book added successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding book: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdateBook_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtBookId.Text, out var id) || id <= 0)
            {
                MessageBox.Show("Please select a book to update", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtBookTitle.Text) || string.IsNullOrWhiteSpace(txtBookAuthor.Text) || !int.TryParse(txtBookStock.Text, out var stock))
            {
                MessageBox.Show("Please fill all fields correctly", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (stock < 0)
            {
                MessageBox.Show("Stock cannot be negative", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtpBookDate.Value.Date > DateTime.Now.Date)
            {
                MessageBox.Show("Adding date cannot be in the future", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtpBookDate.Value.Date < DateTime.Now.AddYears(-50).Date)
            {
                MessageBox.Show("Adding date cannot be more than 50 years in the past", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var book = new Book
                {
                    BookId = id,
                    Title = txtBookTitle.Text.Trim(),
                    AuthorName = txtBookAuthor.Text.Trim(),
                    Stock = stock,
                    AddingDate = dtpBookDate.Value.Date
                };

                if (_bookRepo.UpdateBook(book))
                {
                    LoadBooks();
                    MessageBox.Show("Book updated successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Book not found or update failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating book: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDeleteBook_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtBookId.Text, out var id) || id <= 0)
            {
                MessageBox.Show("Please select a book to delete", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Delete book ID {id}? This action cannot be undone.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (_bookRepo.DeleteBook(id))
                    {
                        LoadBooks();
                        ClearBookForm();
                        MessageBox.Show("Book deleted successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Book not found or cannot be deleted (may have associated sales)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting book: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnAddEmployee_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmployeeName.Text) || string.IsNullOrWhiteSpace(cmbEmployeeGender.Text) || string.IsNullOrWhiteSpace(txtEmployeePhone.Text))
            {
                MessageBox.Show("Please fill all fields", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtpEmployeeBirthday.Value.Date > DateTime.Now.Date)
            {
                MessageBox.Show("Birthday cannot be in the future", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtpEmployeeBirthday.Value.Date < DateTime.Now.AddYears(-100).Date)
            {
                MessageBox.Show("Birthday cannot be more than 100 years in the past", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if employee is at least 16 years old (minimum working age)
            if (dtpEmployeeBirthday.Value.Date > DateTime.Now.AddYears(-16).Date)
            {
                MessageBox.Show("Employee must be at least 16 years old", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var employee = new Employee
                {
                    Name = txtEmployeeName.Text.Trim(),
                    Gender = cmbEmployeeGender.Text.Trim(),
                    PhoneNumber = txtEmployeePhone.Text.Trim(),
                    Birthday = dtpEmployeeBirthday.Value.Date
                };

                _employeeRepo.AddEmployee(employee);
                LoadEmployees();
                ClearEmployeeForm();
                MessageBox.Show("Employee added successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding employee: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdateEmployee_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtEmployeeId.Text, out var id) || id <= 0)
            {
                MessageBox.Show("Please select an employee to update", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmployeeName.Text) || string.IsNullOrWhiteSpace(cmbEmployeeGender.Text) || string.IsNullOrWhiteSpace(txtEmployeePhone.Text))
            {
                MessageBox.Show("Please fill all fields", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtpEmployeeBirthday.Value.Date > DateTime.Now.Date)
            {
                MessageBox.Show("Birthday cannot be in the future", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtpEmployeeBirthday.Value.Date < DateTime.Now.AddYears(-100).Date)
            {
                MessageBox.Show("Birthday cannot be more than 100 years in the past", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if employee is at least 16 years old (minimum working age)
            if (dtpEmployeeBirthday.Value.Date > DateTime.Now.AddYears(-16).Date)
            {
                MessageBox.Show("Employee must be at least 16 years old", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var employee = new Employee
                {
                    EmployeeId = id,
                    Name = txtEmployeeName.Text.Trim(),
                    Gender = cmbEmployeeGender.Text.Trim(),
                    PhoneNumber = txtEmployeePhone.Text.Trim(),
                    Birthday = dtpEmployeeBirthday.Value.Date
                };

                if (_employeeRepo.UpdateEmployee(employee))
                {
                    LoadEmployees();
                    MessageBox.Show("Employee updated successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Employee not found or update failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating employee: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDeleteEmployee_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtEmployeeId.Text, out var id) || id <= 0)
            {
                MessageBox.Show("Please select an employee to delete", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Delete employee ID {id}? This action cannot be undone.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (_employeeRepo.DeleteEmployee(id))
                    {
                        LoadEmployees();
                        ClearEmployeeForm();
                        MessageBox.Show("Employee deleted successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Employee not found or cannot be deleted (may have associated sales or users)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting employee: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnDeleteSale_Click(object sender, EventArgs e)
        {
            if (dgvSales.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a sale to delete", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var firstCell = dgvSales.SelectedRows[0].Cells[0]?.Value;
            if (firstCell == null || !int.TryParse(firstCell.ToString(), out var saleId))
            {
                MessageBox.Show("Invalid sale selection", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show($"Delete sale ID {saleId}? This action cannot be undone.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (_saleRepo.DeleteSale(saleId))
                    {
                        LoadSales();
                        MessageBox.Show("Sale deleted successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Sale not found or delete failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting sale: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ClearBookForm()
        {
            txtBookId.Text = "Auto";
            txtBookTitle.Clear();
            txtBookAuthor.Clear();
            txtBookStock.Clear();
            dtpBookDate.Value = DateTime.Now.Date;
        }

        private void ClearEmployeeForm()
        {
            txtEmployeeId.Text = "Auto";
            txtEmployeeName.Clear();
            cmbEmployeeGender.SelectedIndex = -1;
            txtEmployeePhone.Clear();
            dtpEmployeeBirthday.Value = DateTime.Now.AddYears(-25).Date; // Default to 25 years ago
        }

        private void FilterBooks()
        {
            // Simple filtering - can be enhanced
            if (string.IsNullOrWhiteSpace(txtSearchBooks.Text))
            {
                LoadBooks();
                return;
            }
            // For now, reload all and filter in memory
            LoadBooks();
        }

        private void FilterEmployees()
        {
            if (string.IsNullOrWhiteSpace(txtSearchEmployees.Text))
            {
                LoadEmployees();
                return;
            }
            LoadEmployees();
        }

        private void FilterSales()
        {
            if (string.IsNullOrWhiteSpace(txtSearchSales.Text))
            {
                LoadSales();
                return;
            }
            LoadSales();
        }
    }
}

