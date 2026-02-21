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
            _saleNotifier.Attach(new LoggingSaleObserver());
            InitializeComponent();
            LoadBooks();
        }

        private void InitializeComponent()
        {
            this.Text = $"Cashier POS - Welcome {_currentUser.FullName}";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // Header Panel
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(46, 204, 113)
            };

            var welcomeLabel = new Label
            {
                Text = $"💰 Welcome, {_currentUser.FullName} - Point of Sale",
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

            tabControl = new TabControl 
            { 
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                Appearance = TabAppearance.FlatButtons
            };

            // POS Tab
            var tabPOS = new TabPage("💰 Point of Sale");
            CreatePOSTab(tabPOS);

            // Books Tab
            var tabBooks = new TabPage("📚 Book Inventory");
            CreateBooksTab(tabBooks);

            // Sales History Tab
            var tabSales = new TabPage("📊 Sales History");
            CreateSalesTab(tabSales);

            tabControl.TabPages.Add(tabPOS);
            tabControl.TabPages.Add(tabBooks);
            tabControl.TabPages.Add(tabSales);

            this.Controls.Add(tabControl);
            this.Controls.Add(headerPanel);
        }

        private void CreatePOSTab(TabPage tab)
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
                Text = "🛒 New Sale",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(15, 15)
            };

            var lblCustomer = new Label
            {
                Text = "Customer Name",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true,
                Location = new Point(15, 45)
            };
            txtCustomerName = new TextBox 
            { 
                Location = new Point(15, 65), 
                Size = new Size(200, 32), 
                PlaceholderText = "Enter customer name",
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblBook = new Label
            {
                Text = "Book",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true,
                Location = new Point(230, 45)
            };
            cmbBookId = new ComboBox 
            { 
                Location = new Point(230, 65), 
                Size = new Size(200, 32), 
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
                FlatStyle = FlatStyle.Flat
            };

            var lblPrice = new Label
            {
                Text = "Price",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true,
                Location = new Point(445, 45)
            };
            txtPrice = new TextBox 
            { 
                Location = new Point(445, 65), 
                Size = new Size(100, 32), 
                PlaceholderText = "0.00",
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblQuantity = new Label
            {
                Text = "Quantity",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true,
                Location = new Point(445, 45)
            };
            txtQuantity = new TextBox 
            { 
                Location = new Point(445, 65), 
                Size = new Size(100, 32), 
                PlaceholderText = "1",
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblDiscount = new Label
            {
                Text = "Discount",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true,
                Location = new Point(445, 110)
            };
            chk5Percent = new CheckBox 
            { 
                Text = "5%", 
                Location = new Point(445, 130), 
                Size = new Size(60, 28),
                Font = new Font("Segoe UI", 10F),
                Appearance = Appearance.Button,
                FlatStyle = FlatStyle.Flat
            };
            chk5Percent.FlatAppearance.CheckedBackColor = Color.FromArgb(52, 152, 219);
            chk10Percent = new CheckBox 
            { 
                Text = "10%", 
                Location = new Point(515, 130), 
                Size = new Size(60, 28),
                Font = new Font("Segoe UI", 10F),
                Appearance = Appearance.Button,
                FlatStyle = FlatStyle.Flat
            };
            chk10Percent.FlatAppearance.CheckedBackColor = Color.FromArgb(52, 152, 219);
            chk20Percent = new CheckBox 
            { 
                Text = "20%", 
                Location = new Point(585, 130), 
                Size = new Size(60, 28),
                Font = new Font("Segoe UI", 10F),
                Appearance = Appearance.Button,
                FlatStyle = FlatStyle.Flat
            };
            chk20Percent.FlatAppearance.CheckedBackColor = Color.FromArgb(52, 152, 219);

            chk5Percent.CheckedChanged += (s, e) => { if (chk5Percent.Checked) { chk10Percent.Checked = false; chk20Percent.Checked = false; } };
            chk10Percent.CheckedChanged += (s, e) => { if (chk10Percent.Checked) { chk5Percent.Checked = false; chk20Percent.Checked = false; } };
            chk20Percent.CheckedChanged += (s, e) => { if (chk20Percent.Checked) { chk5Percent.Checked = false; chk10Percent.Checked = false; } };

            btnAddToCart = new Button 
            { 
                Text = "➕ Add to Cart", 
                Location = new Point(15, 130), 
                Size = new Size(120, 38),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAddToCart.FlatAppearance.BorderSize = 0;
            btnAddToCart.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 174, 96);
            btnAddToCart.Click += BtnAddToCart_Click;

            btnProcessSale = new Button 
            { 
                Text = "💳 Process Sale", 
                Location = new Point(145, 130), 
                Size = new Size(120, 38),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnProcessSale.FlatAppearance.BorderSize = 0;
            btnProcessSale.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 128, 185);
            btnProcessSale.Click += BtnProcessSale_Click;

            btnClearCart = new Button 
            { 
                Text = "🗑️ Clear Cart", 
                Location = new Point(275, 130), 
                Size = new Size(120, 38),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClearCart.FlatAppearance.BorderSize = 0;
            btnClearCart.FlatAppearance.MouseOverBackColor = Color.FromArgb(243, 156, 18);
            btnClearCart.Click += BtnClearCart_Click;

            var totalLabel = new Label
            {
                Text = "Total Amount:",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(15, 180)
            };

            lblTotal = new Label
            {
                Text = "$0.00",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                Location = new Point(150, 175),
                Size = new Size(300, 35),
                ForeColor = Color.FromArgb(46, 204, 113)
            };

            inputPanel.Controls.AddRange(new Control[] {
                inputTitle,
                lblCustomer, txtCustomerName,
                lblBook, cmbBookId,
                lblPrice, txtPrice,
                lblQuantity, txtQuantity,
                lblDiscount, chk5Percent, chk10Percent, chk20Percent,
                btnAddToCart, btnProcessSale, btnClearCart,
                totalLabel, lblTotal
            });
            
            // Adjust input panel height to fit all controls
            inputPanel.Height = 250;

            // Cart DataGridView
            dgvCart = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
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
                    SelectionBackColor = Color.FromArgb(46, 204, 113),
                    SelectionForeColor = Color.White,
                    Padding = new Padding(5)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(46, 204, 113),
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
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(245, 247, 250), Padding = new Padding(15) };
            
            dgvBooks = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
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
                    SelectionBackColor = Color.FromArgb(46, 204, 113),
                    SelectionForeColor = Color.White,
                    Padding = new Padding(5)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(46, 204, 113),
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
            
            panel.Controls.Add(dgvBooks);
            tab.Controls.Add(panel);
        }

        private void CreateSalesTab(TabPage tab)
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(245, 247, 250), Padding = new Padding(15) };
            
            dgvSales = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
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
                    SelectionBackColor = Color.FromArgb(46, 204, 113),
                    SelectionForeColor = Color.White,
                    Padding = new Padding(5)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(46, 204, 113),
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
            tab.Controls.Add(panel);
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
                    AddingDate = b.AddingDate.ToShortDateString()
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
                SaleDate = s.SaleDate.ToShortDateString()
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

            // Validate price and quantity
            if (price <= 0)
            {
                MessageBox.Show("Price must be greater than zero", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (qty <= 0)
            {
                MessageBox.Show("Quantity must be greater than zero", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Safe null check for SelectedItem
            if (cmbBookId.SelectedItem == null)
            {
                MessageBox.Show("Please select a book", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var bookIdStr = cmbBookId.SelectedItem.ToString()?.Split('-')[0].Trim();
            if (string.IsNullOrEmpty(bookIdStr) || !int.TryParse(bookIdStr, out var bookId))
            {
                MessageBox.Show("Invalid book selection", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var discount = 0m;
            if (chk5Percent.Checked) discount = 0.05m;
            else if (chk10Percent.Checked) discount = 0.10m;
            else if (chk20Percent.Checked) discount = 0.20m;

            // Validate discount range
            if (discount < 0 || discount > 1)
            {
                MessageBox.Show("Invalid discount value", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var book = _bookRepo.GetBookById(bookId);
            if (book == null)
            {
                MessageBox.Show("Book not found. Please refresh the book list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadBooks(); // Refresh book list
                return;
            }

            // Check stock availability
            if (book.Stock < qty)
            {
                MessageBox.Show($"Insufficient stock! Available: {book.Stock}, Requested: {qty}", "Stock Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if adding this item would exceed available stock (considering items already in cart)
            var cartQuantity = _cartItems.Where(i => i.BookId == bookId).Sum(i => i.Quantity);
            if (book.Stock < cartQuantity + qty)
            {
                MessageBox.Show($"Insufficient stock! Available: {book.Stock}, Already in cart: {cartQuantity}, Requested: {qty}", "Stock Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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

            lblTotal.Text = $"{_currentTotal:C}";
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

            // Validate stock availability for all items before processing
            foreach (var item in _cartItems)
            {
                var book = _bookRepo.GetBookById(item.BookId);
                if (book == null)
                {
                    MessageBox.Show($"Book ID {item.BookId} not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (book.Stock < item.Quantity)
                {
                    MessageBox.Show($"Insufficient stock for '{book.Title}'! Available: {book.Stock}, Requested: {item.Quantity}", "Stock Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Process each item in cart with transaction-like behavior
            var processedSales = new List<Sale>();
            var failedItems = new List<CartItem>();

            foreach (var item in _cartItems)
            {
                try
                {
                    // Re-validate stock before processing (stock may have changed)
                    var book = _bookRepo.GetBookById(item.BookId);
                    if (book == null)
                    {
                        failedItems.Add(item);
                        continue;
                    }

                    if (book.Stock < item.Quantity)
                    {
                        failedItems.Add(item);
                        continue;
                    }

                    var sale = new SaleBuilder()
                        .SetCustomerName(txtCustomerName.Text)
                        .SetBookId(item.BookId)
                        .SetEmployeeId(_currentUser.Id)
                        .SetPrice(item.Price)
                        .SetQuantity(item.Quantity)
                        .SetDiscount(item.Discount)
                        .SetSaleDate(DateTime.Now)
                        .Build();

                    // Update stock first (atomic operation with WHERE clause)
                    if (!_bookRepo.UpdateStock(item.BookId, item.Quantity))
                    {
                        failedItems.Add(item);
                        continue;
                    }

                    // Then add sale
                    _saleRepo.AddSale(sale);
                    processedSales.Add(sale);

                    // Notify observers (Observer Pattern)
                    _saleNotifier.Notify(sale);
                }
                catch (Exception ex)
                {
                    failedItems.Add(item);
                    MessageBox.Show($"Error processing item '{item.Title}': {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Remove failed items from cart
            foreach (var item in failedItems)
            {
                _cartItems.Remove(item);
            }

            // Report results
            if (failedItems.Count > 0)
            {
                var message = $"Some items could not be processed:\n";
                foreach (var item in failedItems)
                {
                    message += $"- {item.Title} (Qty: {item.Quantity})\n";
                }
                MessageBox.Show(message, "Partial Success", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (processedSales.Count == 0)
            {
                MessageBox.Show("No items were processed. Please check stock availability.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
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

