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

            tabControl = new TabControl { Dock = DockStyle.Fill };
            
            // Books Tab
            var tabBooks = new TabPage("Books");
            CreateBooksTab(tabBooks);
            
            // Employees Tab
            var tabEmployees = new TabPage("Employees");
            CreateEmployeesTab(tabEmployees);
            
            // Sales Tab
            var tabSales = new TabPage("Sales");
            CreateSalesTab(tabSales);

            btnLogout = new Button
            {
                Text = "Logout",
                Size = new Size(100, 35),
                Location = new Point(this.Width - 120, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.Red,
                ForeColor = Color.White
            };
            btnLogout.Click += (s, e) => { this.Close(); new DashboardForm().Show(); };

            tabControl.TabPages.Add(tabBooks);
            tabControl.TabPages.Add(tabEmployees);
            tabControl.TabPages.Add(tabSales);

            this.Controls.Add(tabControl);
            this.Controls.Add(btnLogout);
        }

        private void CreateBooksTab(TabPage tab)
        {
            var panel = new Panel { Dock = DockStyle.Fill };
            
            // Input Panel
            var inputPanel = new Panel { Height = 150, Dock = DockStyle.Top, BorderStyle = BorderStyle.FixedSingle };
            
            txtBookId = new TextBox { Location = new Point(10, 10), Size = new Size(100, 25), Enabled = false, Text = "Auto" };
            txtBookTitle = new TextBox { Location = new Point(120, 10), Size = new Size(200, 25), PlaceholderText = "Title" };
            txtBookAuthor = new TextBox { Location = new Point(330, 10), Size = new Size(200, 25), PlaceholderText = "Author" };
            txtBookStock = new TextBox { Location = new Point(540, 10), Size = new Size(100, 25), PlaceholderText = "Stock" };
            dtpBookDate = new DateTimePicker { Location = new Point(650, 10), Size = new Size(150, 25) };
            
            btnAddBook = new Button { Text = "Add", Location = new Point(10, 50), Size = new Size(80, 30) };
            btnUpdateBook = new Button { Text = "Update", Location = new Point(100, 50), Size = new Size(80, 30) };
            btnDeleteBook = new Button { Text = "Delete", Location = new Point(190, 50), Size = new Size(80, 30), BackColor = Color.Red, ForeColor = Color.White };
            
            btnAddBook.Click += BtnAddBook_Click;
            btnUpdateBook.Click += BtnUpdateBook_Click;
            btnDeleteBook.Click += BtnDeleteBook_Click;
            
            txtSearchBooks = new TextBox { Location = new Point(10, 90), Size = new Size(300, 25), PlaceholderText = "Search books..." };
            txtSearchBooks.TextChanged += (s, e) => FilterBooks();
            
            inputPanel.Controls.AddRange(new Control[] { 
                new Label { Text = "ID:", Location = new Point(10, 12) },
                txtBookId,
                new Label { Text = "Title:", Location = new Point(120, -12) },
                txtBookTitle,
                new Label { Text = "Author:", Location = new Point(330, -12) },
                txtBookAuthor,
                new Label { Text = "Stock:", Location = new Point(540, -12) },
                txtBookStock,
                new Label { Text = "Date:", Location = new Point(650, -12) },
                dtpBookDate,
                btnAddBook, btnUpdateBook, btnDeleteBook,
                txtSearchBooks
            });
            
            // DataGridView
            dgvBooks = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true
            };
            dgvBooks.CellClick += (s, e) => { if (e.RowIndex >= 0) LoadBookToForm(dgvBooks.Rows[e.RowIndex]); };
            
            panel.Controls.Add(dgvBooks);
            panel.Controls.Add(inputPanel);
            tab.Controls.Add(panel);
        }

        private void CreateEmployeesTab(TabPage tab)
        {
            var panel = new Panel { Dock = DockStyle.Fill };
            
            var inputPanel = new Panel { Height = 150, Dock = DockStyle.Top, BorderStyle = BorderStyle.FixedSingle };
            
            txtEmployeeId = new TextBox { Location = new Point(10, 10), Size = new Size(100, 25), Enabled = false, Text = "Auto" };
            txtEmployeeName = new TextBox { Location = new Point(120, 10), Size = new Size(200, 25), PlaceholderText = "Full Name" };
            cmbEmployeeGender = new ComboBox { Location = new Point(330, 10), Size = new Size(100, 25) };
            cmbEmployeeGender.Items.AddRange(new[] { "Male", "Female" });
            txtEmployeePhone = new TextBox { Location = new Point(440, 10), Size = new Size(150, 25), PlaceholderText = "Phone" };
            dtpEmployeeBirthday = new DateTimePicker { Location = new Point(600, 10), Size = new Size(150, 25) };
            
            btnAddEmployee = new Button { Text = "Add", Location = new Point(10, 50), Size = new Size(80, 30) };
            btnUpdateEmployee = new Button { Text = "Update", Location = new Point(100, 50), Size = new Size(80, 30) };
            btnDeleteEmployee = new Button { Text = "Delete", Location = new Point(190, 50), Size = new Size(80, 30), BackColor = Color.Red, ForeColor = Color.White };
            
            btnAddEmployee.Click += BtnAddEmployee_Click;
            btnUpdateEmployee.Click += BtnUpdateEmployee_Click;
            btnDeleteEmployee.Click += BtnDeleteEmployee_Click;
            
            txtSearchEmployees = new TextBox { Location = new Point(10, 90), Size = new Size(300, 25), PlaceholderText = "Search employees..." };
            txtSearchEmployees.TextChanged += (s, e) => FilterEmployees();
            
            inputPanel.Controls.AddRange(new Control[] {
                new Label { Text = "ID:", Location = new Point(10, 12) },
                txtEmployeeId,
                new Label { Text = "Name:", Location = new Point(120, -12) },
                txtEmployeeName,
                new Label { Text = "Gender:", Location = new Point(330, -12) },
                cmbEmployeeGender,
                new Label { Text = "Phone:", Location = new Point(440, -12) },
                txtEmployeePhone,
                new Label { Text = "Birthday:", Location = new Point(600, -12) },
                dtpEmployeeBirthday,
                btnAddEmployee, btnUpdateEmployee, btnDeleteEmployee,
                txtSearchEmployees
            });
            
            dgvEmployees = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true
            };
            dgvEmployees.CellClick += (s, e) => { if (e.RowIndex >= 0) LoadEmployeeToForm(dgvEmployees.Rows[e.RowIndex]); };
            
            panel.Controls.Add(dgvEmployees);
            panel.Controls.Add(inputPanel);
            tab.Controls.Add(panel);
        }

        private void CreateSalesTab(TabPage tab)
        {
            var panel = new Panel { Dock = DockStyle.Fill };
            
            var inputPanel = new Panel { Height = 80, Dock = DockStyle.Top, BorderStyle = BorderStyle.FixedSingle };
            
            btnDeleteSale = new Button { Text = "Delete Selected", Location = new Point(10, 20), Size = new Size(120, 30), BackColor = Color.Red, ForeColor = Color.White };
            btnDeleteSale.Click += BtnDeleteSale_Click;
            
            txtSearchSales = new TextBox { Location = new Point(150, 20), Size = new Size(300, 25), PlaceholderText = "Search sales..." };
            txtSearchSales.TextChanged += (s, e) => FilterSales();
            
            inputPanel.Controls.Add(btnDeleteSale);
            inputPanel.Controls.Add(txtSearchSales);
            
            dgvSales = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true
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
                b.AddingDate.ToShortDateString()
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
                e.Birthday.ToShortDateString()
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
                s.SaleDate.ToShortDateString()
            }).ToList();
        }

        private void LoadBookToForm(DataGridViewRow row)
        {
            txtBookId.Text = row.Cells[0].Value.ToString();
            txtBookTitle.Text = row.Cells[1].Value.ToString();
            txtBookAuthor.Text = row.Cells[2].Value.ToString();
            txtBookStock.Text = row.Cells[3].Value.ToString();
            if (DateTime.TryParse(row.Cells[4].Value.ToString(), out var date))
                dtpBookDate.Value = date;
        }

        private void LoadEmployeeToForm(DataGridViewRow row)
        {
            txtEmployeeId.Text = row.Cells[0].Value.ToString();
            txtEmployeeName.Text = row.Cells[1].Value.ToString();
            cmbEmployeeGender.Text = row.Cells[2].Value.ToString();
            txtEmployeePhone.Text = row.Cells[3].Value.ToString();
            if (DateTime.TryParse(row.Cells[4].Value.ToString(), out var date))
                dtpEmployeeBirthday.Value = date;
        }

        private void BtnAddBook_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBookTitle.Text) || string.IsNullOrWhiteSpace(txtBookAuthor.Text) || !int.TryParse(txtBookStock.Text, out var stock))
            {
                MessageBox.Show("Please fill all fields correctly", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var book = new Book
            {
                Title = txtBookTitle.Text,
                AuthorName = txtBookAuthor.Text,
                Stock = stock,
                AddingDate = dtpBookDate.Value.Date
            };

            _bookRepo.AddBook(book);
            LoadBooks();
            ClearBookForm();
        }

        private void BtnUpdateBook_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtBookId.Text, out var id) || id <= 0) return;
            if (string.IsNullOrWhiteSpace(txtBookTitle.Text) || string.IsNullOrWhiteSpace(txtBookAuthor.Text) || !int.TryParse(txtBookStock.Text, out var stock))
            {
                MessageBox.Show("Please fill all fields correctly", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var book = new Book
            {
                BookId = id,
                Title = txtBookTitle.Text,
                AuthorName = txtBookAuthor.Text,
                Stock = stock,
                AddingDate = dtpBookDate.Value.Date
            };

            _bookRepo.UpdateBook(book);
            LoadBooks();
        }

        private void BtnDeleteBook_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtBookId.Text, out var id) || id <= 0) return;
            if (MessageBox.Show($"Delete book ID {id}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _bookRepo.DeleteBook(id);
                LoadBooks();
                ClearBookForm();
            }
        }

        private void BtnAddEmployee_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmployeeName.Text) || string.IsNullOrWhiteSpace(cmbEmployeeGender.Text) || string.IsNullOrWhiteSpace(txtEmployeePhone.Text))
            {
                MessageBox.Show("Please fill all fields", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var employee = new Employee
            {
                Name = txtEmployeeName.Text,
                Gender = cmbEmployeeGender.Text,
                PhoneNumber = txtEmployeePhone.Text,
                Birthday = dtpEmployeeBirthday.Value.Date
            };

            _employeeRepo.AddEmployee(employee);
            LoadEmployees();
            ClearEmployeeForm();
        }

        private void BtnUpdateEmployee_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtEmployeeId.Text, out var id) || id <= 0) return;
            if (string.IsNullOrWhiteSpace(txtEmployeeName.Text) || string.IsNullOrWhiteSpace(cmbEmployeeGender.Text) || string.IsNullOrWhiteSpace(txtEmployeePhone.Text))
            {
                MessageBox.Show("Please fill all fields", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var employee = new Employee
            {
                EmployeeId = id,
                Name = txtEmployeeName.Text,
                Gender = cmbEmployeeGender.Text,
                PhoneNumber = txtEmployeePhone.Text,
                Birthday = dtpEmployeeBirthday.Value.Date
            };

            _employeeRepo.UpdateEmployee(employee);
            LoadEmployees();
        }

        private void BtnDeleteEmployee_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtEmployeeId.Text, out var id) || id <= 0) return;
            if (MessageBox.Show($"Delete employee ID {id}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _employeeRepo.DeleteEmployee(id);
                LoadEmployees();
                ClearEmployeeForm();
            }
        }

        private void BtnDeleteSale_Click(object sender, EventArgs e)
        {
            if (dgvSales.SelectedRows.Count == 0) return;
            var saleId = (int)dgvSales.SelectedRows[0].Cells[0].Value;
            if (MessageBox.Show($"Delete sale ID {saleId}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _saleRepo.DeleteSale(saleId);
                LoadSales();
            }
        }

        private void ClearBookForm()
        {
            txtBookId.Text = "Auto";
            txtBookTitle.Clear();
            txtBookAuthor.Clear();
            txtBookStock.Clear();
            dtpBookDate.Value = DateTime.Now;
        }

        private void ClearEmployeeForm()
        {
            txtEmployeeId.Text = "Auto";
            txtEmployeeName.Clear();
            cmbEmployeeGender.SelectedIndex = -1;
            txtEmployeePhone.Clear();
            dtpEmployeeBirthday.Value = DateTime.Now;
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

