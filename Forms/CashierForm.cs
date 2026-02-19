using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp.Builder;
using WindowsFormsApp.Data;
using WindowsFormsApp.Models;
using WindowsFormsApp.Observer;
using WindowsFormsApp.Strategy;

namespace WindowsFormsApp.Forms
{
    /// <summary>
    /// Cashier Form - Point of Sale System
    /// </summary>
    public partial class CashierForm : Form
    {
        private TextBox txtCustomerName, txtPrice, txtQuantity;
        private ComboBox cmbBookId;
        private CheckBox chk5Percent, chk10Percent, chk20Percent;
        private DataGridView dgvCart, dgvBooks, dgvSales;
        private Label lblTotal;
        private Button btnAddToCart, btnProcessSale, btnClearCart, btnLogout;
        private TabControl tabControl;
        private decimal _currentTotal = 0;
        private readonly BookRepository _bookRepo = new();
        private readonly SaleRepository _saleRepo = new();
        private readonly SaleNotifier _saleNotifier = new();
        private readonly User _currentUser;
        private List<CartItem> _cartItems = new();

        public CashierForm(User user)
        {
            _currentUser = user;
            _saleNotifier.AddObserver(new LoggingSaleObserver());
            InitializeComponent();
            LoadBooks();
        }

        private void InitializeComponent()
        {
            this.Text = $"Cashier POS - Welcome {_currentUser.FullName}";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            tabControl = new TabControl { Dock = DockStyle.Fill };

            // POS Tab
            var tabPOS = new TabPage("Point of Sale");
            CreatePOSTab(tabPOS);

            // Books Tab
            var tabBooks = new TabPage("Book Inventory");
            CreateBooksTab(tabBooks);

            // Sales History Tab
            var tabSales = new TabPage("Sales History");
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

            tabControl.TabPages.Add(tabPOS);
            tabControl.TabPages.Add(tabBooks);
            tabControl.TabPages.Add(tabSales);

            this.Controls.Add(tabControl);
            this.Controls.Add(btnLogout);
        }

        private void CreatePOSTab(TabPage tab)
        {
            var panel = new Panel { Dock = DockStyle.Fill };

            // Input Panel
            var inputPanel = new Panel { Height = 200, Dock = DockStyle.Top, BorderStyle = BorderStyle.FixedSingle };

            txtCustomerName = new TextBox { Location = new Point(10, 30), Size = new Size(200, 25), PlaceholderText = "Customer Name" };
            cmbBookId = new ComboBox { Location = new Point(220, 30), Size = new Size(150, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            txtPrice = new TextBox { Location = new Point(380, 30), Size = new Size(100, 25), PlaceholderText = "Price" };
            txtQuantity = new TextBox { Location = new Point(490, 30), Size = new Size(100, 25), PlaceholderText = "Quantity" };

            chk5Percent = new CheckBox { Text = "5%", Location = new Point(10, 70), Size = new Size(60, 25) };
            chk10Percent = new CheckBox { Text = "10%", Location = new Point(80, 70), Size = new Size(60, 25) };
            chk20Percent = new CheckBox { Text = "20%", Location = new Point(150, 70), Size = new Size(60, 25) };

            chk5Percent.CheckedChanged += (s, e) => { if (chk5Percent.Checked) { chk10Percent.Checked = false; chk20Percent.Checked = false; } };
            chk10Percent.CheckedChanged += (s, e) => { if (chk10Percent.Checked) { chk5Percent.Checked = false; chk20Percent.Checked = false; } };
            chk20Percent.CheckedChanged += (s, e) => { if (chk20Percent.Checked) { chk5Percent.Checked = false; chk10Percent.Checked = false; } };

            btnAddToCart = new Button { Text = "Add to Cart", Location = new Point(220, 70), Size = new Size(120, 30), BackColor = Color.Green, ForeColor = Color.White };
            btnAddToCart.Click += BtnAddToCart_Click;

            btnProcessSale = new Button { Text = "Process Sale", Location = new Point(350, 70), Size = new Size(120, 30), BackColor = Color.Blue, ForeColor = Color.White };
            btnProcessSale.Click += BtnProcessSale_Click;

            btnClearCart = new Button { Text = "Clear Cart", Location = new Point(480, 70), Size = new Size(100, 30), BackColor = Color.Orange, ForeColor = Color.White };
            btnClearCart.Click += BtnClearCart_Click;

            lblTotal = new Label
            {
                Text = "Total: $0.00",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(10, 110),
                Size = new Size(300, 30),
                ForeColor = Color.DarkGreen
            };

            inputPanel.Controls.AddRange(new Control[] {
                new Label { Text = "Customer Name:", Location = new Point(10, 10) },
                txtCustomerName,
                new Label { Text = "Book ID:", Location = new Point(220, 10) },
                cmbBookId,
                new Label { Text = "Price:", Location = new Point(380, 10) },
                txtPrice,
                new Label { Text = "Quantity:", Location = new Point(490, 10) },
                txtQuantity,
                new Label { Text = "Discount:", Location = new Point(10, 50) },
                chk5Percent, chk10Percent, chk20Percent,
                btnAddToCart, btnProcessSale, btnClearCart,
                lblTotal
            });

            // Cart DataGridView
            dgvCart = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };
            dgvCart.Columns.Add("BookId", "Book ID");
            dgvCart.Columns.Add("Title", "Title");
            dgvCart.Columns.Add("Price", "Price");
            dgvCart.Columns.Add("Quantity", "Qty");
            dgvCart.Columns.Add("Discount", "Discount");
            dgvCart.Columns.Add("Subtotal", "Subtotal");

            panel.Controls.Add(dgvCart);
            panel.Controls.Add(inputPanel);
            tab.Controls.Add(panel);
        }

        private void CreateBooksTab(TabPage tab)
        {
            dgvBooks = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };
            tab.Controls.Add(dgvBooks);
        }

        private void CreateSalesTab(TabPage tab)
        {
            dgvSales = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };
            tab.Controls.Add(dgvSales);
            LoadSales();
        }

        private void LoadBooks()
        {
            var books = _bookRepo.GetAllBooks();
            cmbBookId.Items.Clear();
            foreach (var book in books)
            {
                cmbBookId.Items.Add($"{book.BookId} - {book.Title}");
            }

            if (dgvBooks != null)
            {
                dgvBooks.DataSource = books.Select(b => new
                {
                    b.BookId,
                    b.Title,
                    b.AuthorName,
                    b.Stock,
                    b.AddingDate.ToShortDateString()
                }).ToList();
            }
        }

        private void LoadSales()
        {
            var sales = _saleRepo.GetAllSales().Where(s => s.EmployeeId == _currentUser.Id).ToList();
            dgvSales.DataSource = sales.Select(s => new
            {
                s.SaleId,
                s.CustomerName,
                s.BookId,
                Price = s.Price.ToString("C"),
                s.Quantity,
                Discount = (s.Discount * 100).ToString("F0") + "%",
                Total = s.Total.ToString("C"),
                s.SaleDate.ToShortDateString()
            }).ToList();
        }

        private void BtnAddToCart_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text) || cmbBookId.SelectedIndex < 0 ||
                !decimal.TryParse(txtPrice.Text, out var price) || !int.TryParse(txtQuantity.Text, out var qty))
            {
                MessageBox.Show("Please fill all fields correctly", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var bookIdStr = cmbBookId.SelectedItem.ToString().Split('-')[0].Trim();
            if (!int.TryParse(bookIdStr, out var bookId)) return;

            var discount = 0m;
            if (chk5Percent.Checked) discount = 0.05m;
            else if (chk10Percent.Checked) discount = 0.10m;
            else if (chk20Percent.Checked) discount = 0.20m;

            var book = _bookRepo.GetBookById(bookId);
            if (book == null) return;

            var subtotal = (price * qty) - ((price * qty) * discount);

            var item = new CartItem
            {
                BookId = bookId,
                Title = book.Title,
                Price = price,
                Quantity = qty,
                Discount = discount
            };

            _cartItems.Add(item);
            UpdateCartDisplay();
        }

        private void UpdateCartDisplay()
        {
            dgvCart.Rows.Clear();
            _currentTotal = 0;

            foreach (var item in _cartItems)
            {
                var subtotal = (item.Price * item.Quantity) - ((item.Price * item.Quantity) * item.Discount);
                _currentTotal += subtotal;
                dgvCart.Rows.Add(item.BookId, item.Title, item.Price.ToString("C"), item.Quantity,
                    (item.Discount * 100).ToString("F0") + "%", subtotal.ToString("C"));
            }

            lblTotal.Text = $"Total: {_currentTotal:C}";
        }

        private void BtnProcessSale_Click(object sender, EventArgs e)
        {
            if (_cartItems.Count == 0)
            {
                MessageBox.Show("Cart is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("Please enter customer name", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Process each item in cart
            foreach (var item in _cartItems)
            {
                var sale = new SaleBuilder()
                    .SetCustomerName(txtCustomerName.Text)
                    .SetBookId(item.BookId)
                    .SetEmployeeId(_currentUser.Id)
                    .SetPrice(item.Price)
                    .SetQuantity(item.Quantity)
                    .SetDiscount(item.Discount)
                    .SetSaleDate(DateTime.Now)
                    .Build();

                _saleRepo.AddSale(sale);
                _bookRepo.UpdateStock(item.BookId, item.Quantity);

                // Notify observers (Observer Pattern)
                _saleNotifier.NotifySaleCreated(sale);
            }

            MessageBox.Show($"Sale processed successfully! Total: {_currentTotal:C}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            _cartItems.Clear();
            UpdateCartDisplay();
            LoadBooks();
            LoadSales();
            ClearForm();
        }

        private void BtnClearCart_Click(object sender, EventArgs e)
        {
            _cartItems.Clear();
            UpdateCartDisplay();
            ClearForm();
        }

        private void ClearForm()
        {
            txtCustomerName.Clear();
            cmbBookId.SelectedIndex = -1;
            txtPrice.Clear();
            txtQuantity.Clear();
            chk5Percent.Checked = false;
            chk10Percent.Checked = false;
            chk20Percent.Checked = false;
        }

        private class CartItem
        {
            public int BookId { get; set; }
            public string Title { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public decimal Discount { get; set; }
        }
    }
}

