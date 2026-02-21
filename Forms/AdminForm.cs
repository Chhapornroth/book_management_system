using System;
using System.Collections.Generic;
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
        private Button btnAddBook, btnUpdateBook, btnDeleteBook, btnClearBook;
        private Button btnAddEmployee, btnUpdateEmployee, btnDeleteEmployee, btnClearEmployee;
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
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 57, 43);
            btnLogout.Click += (s, e) => 
            { 
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            
            // Position logout button on the right side
            void PositionLogoutButton()
            {
                btnLogout.Location = new Point(headerPanel.Width - btnLogout.Width - 15, 15);
            }
            
            this.Shown += (s, e) => PositionLogoutButton();
            headerPanel.Resize += (s, e) => PositionLogoutButton();

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
                Height = 240, 
                Dock = DockStyle.Top, 
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            var inputTitle = new Label
            {
                Text = "📚 Book Information",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(15, 15)
            };
            
            var lblId = new Label { Text = "ID", Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(127, 140, 141), Location = new Point(15, 50), AutoSize = true };
            var lblTitle = new Label { Text = "Title", Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(127, 140, 141), Location = new Point(110, 50), AutoSize = true };
            var lblAuthor = new Label { Text = "Author", Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(127, 140, 141), Location = new Point(340, 50), AutoSize = true };
            var lblStock = new Label { Text = "Stock", Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(127, 140, 141), Location = new Point(570, 50), AutoSize = true };
            var lblDate = new Label { Text = "Date", Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(127, 140, 141), Location = new Point(680, 50), AutoSize = true };
            
            txtBookId = new TextBox 
            { 
                Location = new Point(15, 75), 
                Size = new Size(80, 35), 
                Enabled = false, 
                Text = "Auto",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(236, 240, 241),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtBookTitle = new TextBox 
            { 
                Location = new Point(110, 75), 
                Size = new Size(220, 35), 
                PlaceholderText = "Book Title",
                Font = new Font("Segoe UI", 12F),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtBookAuthor = new TextBox 
            { 
                Location = new Point(340, 75), 
                Size = new Size(220, 35), 
                PlaceholderText = "Author Name",
                Font = new Font("Segoe UI", 12F),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtBookStock = new TextBox 
            { 
                Location = new Point(570, 75), 
                Size = new Size(100, 35), 
                PlaceholderText = "Stock",
                Font = new Font("Segoe UI", 12F),
                BorderStyle = BorderStyle.FixedSingle
            };
            dtpBookDate = new DateTimePicker 
            { 
                Location = new Point(680, 75), 
                Size = new Size(220, 35),
                Value = DateTime.Now.Date,
                MaxDate = DateTime.Now.Date,
                MinDate = DateTime.Now.AddYears(-50).Date,
                Font = new Font("Segoe UI", 12F),
                Format = DateTimePickerFormat.Long
            };
            
            btnAddBook = new Button 
            { 
                Text = "➕ Add", 
                Location = new Point(15, 125), 
                Size = new Size(100, 40),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
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
                Location = new Point(125, 125), 
                Size = new Size(100, 40),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
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
                Location = new Point(235, 125), 
                Size = new Size(100, 40),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDeleteBook.FlatAppearance.BorderSize = 0;
            btnDeleteBook.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 57, 43);
            
            btnClearBook = new Button 
            { 
                Text = "🧹 Clear", 
                Location = new Point(345, 125), 
                Size = new Size(100, 40),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClearBook.FlatAppearance.BorderSize = 0;
            btnClearBook.FlatAppearance.MouseOverBackColor = Color.FromArgb(142, 68, 173);
            btnClearBook.Click += (s, e) => ClearBookForm();
            
            btnAddBook.Click += BtnAddBook_Click;
            btnUpdateBook.Click += BtnUpdateBook_Click;
            btnDeleteBook.Click += BtnDeleteBook_Click;
            
            var searchLabel = new Label
            {
                Text = "🔍 Search:",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(15, 180)
            };
            
            txtSearchBooks = new TextBox 
            { 
                Location = new Point(120, 177), 
                Size = new Size(350, 35), 
                PlaceholderText = "Search by title, author...",
                Font = new Font("Segoe UI", 12F),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearchBooks.TextChanged += (s, e) => FilterBooks();

            inputPanel.Controls.AddRange(new Control[] { 
                inputTitle,
                lblId, txtBookId,
                lblTitle, txtBookTitle,
                lblAuthor, txtBookAuthor,
                lblStock, txtBookStock,
                lblDate, dtpBookDate,
                btnAddBook, btnUpdateBook, btnDeleteBook, btnClearBook,
                searchLabel,
                txtSearchBooks
            });
            
            // DataGridView with modern styling
            dgvBooks = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(236, 240, 241),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                ColumnHeadersHeight = 50,
                RowTemplate = { Height = 45 },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.White,
                    ForeColor = Color.FromArgb(52, 73, 94),
                    SelectionBackColor = Color.FromArgb(52, 152, 219),
                    SelectionForeColor = Color.White,
                    Padding = new Padding(8),
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(52, 73, 94),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(8),
                    WrapMode = DataGridViewTriState.True
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(250, 250, 250),
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold)
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
                Height = 240, 
                Dock = DockStyle.Top, 
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            var inputTitle = new Label
            {
                Text = "👥 Employee Information",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(15, 15)
            };
            
            var lblEmpId = new Label { Text = "ID", Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(127, 140, 141), Location = new Point(15, 50), AutoSize = true };
            var lblEmpName = new Label { Text = "Name", Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(127, 140, 141), Location = new Point(110, 50), AutoSize = true };
            var lblEmpGender = new Label { Text = "Gender", Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(127, 140, 141), Location = new Point(340, 50), AutoSize = true };
            var lblEmpPhone = new Label { Text = "Phone", Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(127, 140, 141), Location = new Point(470, 50), AutoSize = true };
            var lblEmpBirthday = new Label { Text = "Birthday", Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(127, 140, 141), Location = new Point(630, 50), AutoSize = true };
            
            txtEmployeeId = new TextBox 
            { 
                Location = new Point(15, 75), 
                Size = new Size(80, 35), 
                Enabled = false, 
                Text = "Auto",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(236, 240, 241),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtEmployeeName = new TextBox 
            { 
                Location = new Point(110, 75), 
                Size = new Size(220, 35), 
                PlaceholderText = "Full Name",
                Font = new Font("Segoe UI", 12F),
                BorderStyle = BorderStyle.FixedSingle
            };
            cmbEmployeeGender = new ComboBox 
            { 
                Location = new Point(340, 75), 
                Size = new Size(120, 35),
                Font = new Font("Segoe UI", 12F),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            cmbEmployeeGender.Items.AddRange(new[] { "Male", "Female" });
            txtEmployeePhone = new TextBox 
            { 
                Location = new Point(470, 75), 
                Size = new Size(150, 35), 
                PlaceholderText = "Phone Number",
                Font = new Font("Segoe UI", 12F),
                BorderStyle = BorderStyle.FixedSingle
            };
            dtpEmployeeBirthday = new DateTimePicker 
            { 
                Location = new Point(630, 75), 
                Size = new Size(220, 35),
                Value = DateTime.Now.AddYears(-25).Date,
                MaxDate = DateTime.Now.Date,
                MinDate = DateTime.Now.AddYears(-100).Date,
                Font = new Font("Segoe UI", 12F),
                Format = DateTimePickerFormat.Long
            };
            
            btnAddEmployee = new Button 
            { 
                Text = "➕ Add", 
                Location = new Point(15, 125), 
                Size = new Size(100, 40),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
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
                Location = new Point(125, 125), 
                Size = new Size(100, 40),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
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
                Location = new Point(235, 125), 
                Size = new Size(100, 40),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDeleteEmployee.FlatAppearance.BorderSize = 0;
            btnDeleteEmployee.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 57, 43);
            
            btnClearEmployee = new Button 
            { 
                Text = "🧹 Clear", 
                Location = new Point(345, 125), 
                Size = new Size(100, 40),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClearEmployee.FlatAppearance.BorderSize = 0;
            btnClearEmployee.FlatAppearance.MouseOverBackColor = Color.FromArgb(142, 68, 173);
            btnClearEmployee.Click += (s, e) => ClearEmployeeForm();
            
            btnAddEmployee.Click += BtnAddEmployee_Click;
            btnUpdateEmployee.Click += BtnUpdateEmployee_Click;
            btnDeleteEmployee.Click += BtnDeleteEmployee_Click;
            
            var searchLabel = new Label
            {
                Text = "🔍 Search:",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(15, 180)
            };
            
            txtSearchEmployees = new TextBox 
            { 
                Location = new Point(120, 177), 
                Size = new Size(350, 35), 
                PlaceholderText = "Search by name, phone...",
                Font = new Font("Segoe UI", 12F),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearchEmployees.TextChanged += (s, e) => FilterEmployees();

            inputPanel.Controls.AddRange(new Control[] {
                inputTitle,
                lblEmpId, txtEmployeeId,
                lblEmpName, txtEmployeeName,
                lblEmpGender, cmbEmployeeGender,
                lblEmpPhone, txtEmployeePhone,
                lblEmpBirthday, dtpEmployeeBirthday,
                btnAddEmployee, btnUpdateEmployee, btnDeleteEmployee, btnClearEmployee,
                searchLabel,
                txtSearchEmployees
            });
            
            dgvEmployees = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(236, 240, 241),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                ColumnHeadersHeight = 50,
                RowTemplate = { Height = 45 },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.White,
                    ForeColor = Color.FromArgb(52, 73, 94),
                    SelectionBackColor = Color.FromArgb(52, 152, 219),
                    SelectionForeColor = Color.White,
                    Padding = new Padding(8),
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(52, 73, 94),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(8),
                    WrapMode = DataGridViewTriState.True
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(250, 250, 250),
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold)
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
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(15, 15)
            };
            
            btnDeleteSale = new Button 
            { 
                Text = "🗑️ Delete Selected", 
                Location = new Point(15, 50), 
                Size = new Size(160, 40),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
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
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(170, 58)
            };
            
            txtSearchSales = new TextBox 
            { 
                Location = new Point(270, 50), 
                Size = new Size(350, 35), 
                PlaceholderText = "Search by customer, date...",
                Font = new Font("Segoe UI", 12F),
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
                MultiSelect = true,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(236, 240, 241),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                ColumnHeadersHeight = 50,
                RowTemplate = { Height = 45 },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.White,
                    ForeColor = Color.FromArgb(52, 73, 94),
                    SelectionBackColor = Color.FromArgb(52, 152, 219),
                    SelectionForeColor = Color.White,
                    Padding = new Padding(8),
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(52, 73, 94),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(8),
                    WrapMode = DataGridViewTriState.True
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(250, 250, 250),
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold)
                }
            };
            
            panel.Controls.Add(dgvSales);
            panel.Controls.Add(inputPanel);
            tab.Controls.Add(panel);
        }

        private void LoadData()
        {
            try
            {
                LoadBooks();
                LoadEmployees();
                LoadSales();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}\n\nPlease check your database connection.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadBooks()
        {
            try
            {
                var books = _bookRepo.GetAllBooks();
                if (dgvBooks != null)
                {
                    dgvBooks.DataSource = books.Select(b => new
                    {
                        b.BookId,
                        b.Title,
                        b.AuthorName,
                        b.Stock,
                        AddingDate = b.AddingDate.ToShortDateString()
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading books: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadEmployees()
        {
            try
            {
                var employees = _employeeRepo.GetAllEmployees();
                if (dgvEmployees != null)
                {
                    dgvEmployees.DataSource = employees.Select(e => new
                    {
                        e.EmployeeId,
                        e.Name,
                        e.Gender,
                        e.PhoneNumber,
                        Birthday = e.Birthday.ToShortDateString()
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employees: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSales()
        {
            try
            {
                var sales = _saleRepo.GetAllSales();
                if (dgvSales != null)
                {
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sales: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            // Check if rows are selected in DataGridView
            if (dgvBooks.SelectedRows.Count == 0)
            {
                // Fallback to form field if no rows selected
                if (!int.TryParse(txtBookId.Text, out var id) || id <= 0)
                {
                    MessageBox.Show("Please select one or more books to delete", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Single delete from form field
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
                return;
            }

            // Multi-select deletion
            var selectedRows = dgvBooks.SelectedRows;
            var bookIds = new List<int>();
            
            foreach (DataGridViewRow row in selectedRows)
            {
                if (row.Cells[0].Value != null && int.TryParse(row.Cells[0].Value.ToString(), out var bookId))
                {
                    bookIds.Add(bookId);
                }
            }

            if (bookIds.Count == 0)
            {
                MessageBox.Show("No valid books selected", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var message = bookIds.Count == 1 
                ? $"Delete book ID {bookIds[0]}? This action cannot be undone."
                : $"Delete {bookIds.Count} selected books? This action cannot be undone.";

            if (MessageBox.Show(message, "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    var deletedCount = 0;
                    var failedCount = 0;
                    var failedIds = new List<int>();

                    foreach (var bookId in bookIds)
                    {
                        try
                        {
                            if (_bookRepo.DeleteBook(bookId))
                            {
                                deletedCount++;
                            }
                            else
                            {
                                failedCount++;
                                failedIds.Add(bookId);
                            }
                        }
                        catch (Exception ex)
                        {
                            failedCount++;
                            failedIds.Add(bookId);
                            System.Diagnostics.Debug.WriteLine($"Error deleting book {bookId}: {ex.Message}");
                        }
                    }

                    LoadBooks();
                    ClearBookForm();

                    if (failedCount == 0)
                    {
                        MessageBox.Show($"{deletedCount} book(s) deleted successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        var failedMsg = failedIds.Count <= 5 
                            ? $"Failed to delete: {string.Join(", ", failedIds)}"
                            : $"Failed to delete {failedIds.Count} book(s)";
                        MessageBox.Show($"{deletedCount} book(s) deleted successfully.\n{failedMsg}\n\nSome books may have associated sales.", "Partial Success", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting books: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            // Check if rows are selected in DataGridView
            if (dgvEmployees.SelectedRows.Count == 0)
            {
                // Fallback to form field if no rows selected
                if (!int.TryParse(txtEmployeeId.Text, out var id) || id <= 0)
                {
                    MessageBox.Show("Please select one or more employees to delete", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Single delete from form field
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
                return;
            }

            // Multi-select deletion
            var selectedRows = dgvEmployees.SelectedRows;
            var employeeIds = new List<int>();
            
            foreach (DataGridViewRow row in selectedRows)
            {
                if (row.Cells[0].Value != null && int.TryParse(row.Cells[0].Value.ToString(), out var employeeId))
                {
                    employeeIds.Add(employeeId);
                }
            }

            if (employeeIds.Count == 0)
            {
                MessageBox.Show("No valid employees selected", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var message = employeeIds.Count == 1 
                ? $"Delete employee ID {employeeIds[0]}? This action cannot be undone."
                : $"Delete {employeeIds.Count} selected employees? This action cannot be undone.";

            if (MessageBox.Show(message, "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    var deletedCount = 0;
                    var failedCount = 0;
                    var failedIds = new List<int>();

                    foreach (var employeeId in employeeIds)
                    {
                        try
                        {
                            if (_employeeRepo.DeleteEmployee(employeeId))
                            {
                                deletedCount++;
                            }
                            else
                            {
                                failedCount++;
                                failedIds.Add(employeeId);
                            }
                        }
                        catch (Exception ex)
                        {
                            failedCount++;
                            failedIds.Add(employeeId);
                            System.Diagnostics.Debug.WriteLine($"Error deleting employee {employeeId}: {ex.Message}");
                        }
                    }

                    LoadEmployees();
                    ClearEmployeeForm();

                    if (failedCount == 0)
                    {
                        MessageBox.Show($"{deletedCount} employee(s) deleted successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        var failedMsg = failedIds.Count <= 5 
                            ? $"Failed to delete: {string.Join(", ", failedIds)}"
                            : $"Failed to delete {failedIds.Count} employee(s)";
                        MessageBox.Show($"{deletedCount} employee(s) deleted successfully.\n{failedMsg}\n\nSome employees may have associated sales or users.", "Partial Success", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting employees: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnDeleteSale_Click(object sender, EventArgs e)
        {
            if (dgvSales.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select one or more sales to delete", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Multi-select deletion
            var selectedRows = dgvSales.SelectedRows;
            var saleIds = new List<int>();
            
            foreach (DataGridViewRow row in selectedRows)
            {
                if (row.Cells[0].Value != null && int.TryParse(row.Cells[0].Value.ToString(), out var saleId))
                {
                    saleIds.Add(saleId);
                }
            }

            if (saleIds.Count == 0)
            {
                MessageBox.Show("No valid sales selected", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var message = saleIds.Count == 1 
                ? $"Delete sale ID {saleIds[0]}? This action cannot be undone."
                : $"Delete {saleIds.Count} selected sales? This action cannot be undone.";

            if (MessageBox.Show(message, "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    var deletedCount = 0;
                    var failedCount = 0;
                    var failedIds = new List<int>();

                    foreach (var saleId in saleIds)
                    {
                        try
                        {
                            if (_saleRepo.DeleteSale(saleId))
                            {
                                deletedCount++;
                            }
                            else
                            {
                                failedCount++;
                                failedIds.Add(saleId);
                            }
                        }
                        catch (Exception ex)
                        {
                            failedCount++;
                            failedIds.Add(saleId);
                            System.Diagnostics.Debug.WriteLine($"Error deleting sale {saleId}: {ex.Message}");
                        }
                    }

                    LoadSales();

                    if (failedCount == 0)
                    {
                        MessageBox.Show($"{deletedCount} sale(s) deleted successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        var failedMsg = failedIds.Count <= 5 
                            ? $"Failed to delete: {string.Join(", ", failedIds)}"
                            : $"Failed to delete {failedIds.Count} sale(s)";
                        MessageBox.Show($"{deletedCount} sale(s) deleted successfully.\n{failedMsg}", "Partial Success", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting sales: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            try
            {
                if (string.IsNullOrWhiteSpace(txtSearchBooks.Text))
                {
                    LoadBooks();
                    return;
                }

                var searchText = txtSearchBooks.Text.ToLower();
                var books = _bookRepo.GetAllBooks();
                
                var filtered = books.Where(b => 
                    b.Title.ToLower().Contains(searchText) ||
                    b.AuthorName.ToLower().Contains(searchText) ||
                    b.BookId.ToString().Contains(searchText) ||
                    b.Stock.ToString().Contains(searchText) ||
                    b.AddingDate.ToShortDateString().ToLower().Contains(searchText)
                ).ToList();

                if (dgvBooks != null)
                {
                    dgvBooks.DataSource = filtered.Select(b => new
                    {
                        b.BookId,
                        b.Title,
                        b.AuthorName,
                        b.Stock,
                        AddingDate = b.AddingDate.ToShortDateString()
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering books: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilterEmployees()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSearchEmployees.Text))
                {
                    LoadEmployees();
                    return;
                }

                var searchText = txtSearchEmployees.Text.ToLower();
                var employees = _employeeRepo.GetAllEmployees();
                
                var filtered = employees.Where(e => 
                    e.Name.ToLower().Contains(searchText) ||
                    e.PhoneNumber.Contains(searchText) ||
                    e.EmployeeId.ToString().Contains(searchText) ||
                    e.Gender.ToLower().Contains(searchText) ||
                    e.Birthday.ToShortDateString().ToLower().Contains(searchText)
                ).ToList();

                if (dgvEmployees != null)
                {
                    dgvEmployees.DataSource = filtered.Select(e => new
                    {
                        e.EmployeeId,
                        e.Name,
                        e.Gender,
                        e.PhoneNumber,
                        Birthday = e.Birthday.ToShortDateString()
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering employees: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilterSales()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSearchSales.Text))
                {
                    LoadSales();
                    return;
                }

                var searchText = txtSearchSales.Text.ToLower();
                var sales = _saleRepo.GetAllSales();
                
                var filtered = sales.Where(s => 
                    s.CustomerName.ToLower().Contains(searchText) ||
                    s.SaleId.ToString().Contains(searchText) ||
                    s.BookId.ToString().Contains(searchText) ||
                    s.EmployeeId.ToString().Contains(searchText) ||
                    s.Price.ToString().Contains(searchText) ||
                    s.Quantity.ToString().Contains(searchText) ||
                    s.Total.ToString().Contains(searchText) ||
                    s.SaleDate.ToShortDateString().ToLower().Contains(searchText)
                ).ToList();

                if (dgvSales != null)
                {
                    dgvSales.DataSource = filtered.Select(s => new
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering sales: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

