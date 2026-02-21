using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsApp.Data;
using WindowsFormsApp.Factory;
using WindowsFormsApp.Models;

namespace WindowsFormsApp.Forms
{
    /// <summary>
    /// Login Form - User authentication
    /// </summary>
    public partial class LoginForm : Form
    {
        private TextBox txtFullName;
        private TextBox txtPassword;
        private Button btnLogin, btnBack;
        private Label lblWelcome;
        private Label lblInstruction;
        private Label lblFullName;
        private Label lblPhone;
        private string _role;
        private int _errorCount = 0;
        private readonly EmployeeRepository _employeeRepo = new();

        public LoginForm(string role)
        {
            _role = role;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = $"Login - {_role}";
            this.Size = new Size(450, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Padding = new Padding(0);

            // Header Panel
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 160,
                BackColor = _role == "Admin" ? Color.FromArgb(52, 152, 219) : Color.FromArgb(46, 204, 113)
            };

            btnBack = new Button
            {
                Text = "⬅️ Back",
                Size = new Size(100, 35),
                Location = new Point(20, 15),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.FlatAppearance.MouseOverBackColor = Color.FromArgb(127, 140, 141);
            btnBack.Click += (s, e) => 
            { 
                var dashboard = new DashboardForm();
                this.Hide();
                dashboard.ShowDialog();
                this.Close();
            };

            lblWelcome = new Label
            {
                Text = $"Welcome, {_role}",
                Font = new Font("Segoe UI", 26F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 50)
            };

            lblInstruction = new Label
            {
                Text = "Please enter your credentials to continue",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(240, 240, 240),
                AutoSize = true,
                Location = new Point(20, 95)
            };

            headerPanel.Controls.Add(btnBack);
            headerPanel.Controls.Add(lblWelcome);
            headerPanel.Controls.Add(lblInstruction);

            // Content Panel
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(40, 30, 40, 30)
            };

            var inputWidth = 350;

            lblFullName = new Label
            {
                Text = "Full Name",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(50, 10),
                BackColor = Color.FromArgb(245, 247, 250),
                Visible = true,
                Size = new Size(100, 20)
            };

            txtFullName = new TextBox
            {
                PlaceholderText = "Enter your full name",
                Size = new Size(inputWidth, 38),
                Location = new Point(50, 38),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Padding = new Padding(10, 0, 0, 0),
                Anchor = AnchorStyles.None
            };

            lblPhone = new Label
            {
                Text = "Phone Number",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(50, 90),
                BackColor = Color.FromArgb(245, 247, 250),
                Visible = true,
                Size = new Size(120, 20)
            };

            txtPassword = new TextBox
            {
                PlaceholderText = "Enter your phone number",
                Size = new Size(inputWidth, 38),
                Location = new Point(50, 118),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                UseSystemPasswordChar = false,
                Padding = new Padding(10, 0, 0, 0),
                Anchor = AnchorStyles.None
            };

            btnLogin = new Button
            {
                Text = "Sign In",
                Size = new Size(inputWidth, 48),
                Location = new Point(50, 170),
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                BackColor = _role == "Admin" ? Color.FromArgb(52, 152, 219) : Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.None
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatAppearance.MouseOverBackColor = _role == "Admin" ? Color.FromArgb(41, 128, 185) : Color.FromArgb(39, 174, 96);
            btnLogin.Click += BtnLogin_Click;

            // Add labels first so they appear on top
            contentPanel.Controls.Add(lblFullName);
            contentPanel.Controls.Add(lblPhone);
            // Then add input controls
            contentPanel.Controls.Add(txtFullName);
            contentPanel.Controls.Add(txtPassword);
            contentPanel.Controls.Add(btnLogin);
            
            // Bring labels to front to ensure visibility
            lblFullName.BringToFront();
            lblPhone.BringToFront();

            // Center all controls on resize
            void CenterControls()
            {
                if (contentPanel.Width > 0)
                {
                    var newCenterX = (contentPanel.Width - inputWidth) / 2;
                    lblFullName.Left = newCenterX;
                    txtFullName.Left = newCenterX;
                    lblPhone.Left = newCenterX;
                    txtPassword.Left = newCenterX;
                    btnLogin.Left = newCenterX;
                }
            }

            contentPanel.Resize += (s, e) => CenterControls();

            this.Controls.Add(headerPanel);
            this.Controls.Add(contentPanel);
            
            // Initial centering after form is shown
            this.Shown += (s, e) => 
            {
                CenterControls();
                lblFullName.BringToFront();
                lblPhone.BringToFront();
            };
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please complete all required fields", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var employee = _employeeRepo.GetEmployeeByPhone(txtPassword.Text.Trim());
                
                if (employee != null && employee.Name.Equals(txtFullName.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    var roleEnum = _role == "Admin" ? UserRole.Admin : UserRole.Cashier;
                    var user = UserFactory.CreateUser(roleEnum, employee.EmployeeId, employee.Name);
                    
                    this.Hide();
                    
                    if (_role == "Admin")
                    {
                        var adminForm = new AdminForm(user);
                        adminForm.ShowDialog();
                        // When child form closes, close login and show dashboard
                        var dashboard = new DashboardForm();
                        this.Close();
                        dashboard.ShowDialog();
                        return;
                    }
                    else
                    {
                        var cashierForm = new CashierForm(user);
                        cashierForm.ShowDialog();
                        // When child form closes, close login and show dashboard
                        var dashboard = new DashboardForm();
                        this.Close();
                        dashboard.ShowDialog();
                        return;
                    }
                }
                else
                {
                    _errorCount++;
                    if (_errorCount >= 3)
                    {
                        MessageBox.Show("Maximum login attempts reached. Please contact administrator.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtPassword.Enabled = false;
                        btnLogin.Enabled = false;
                    }
                    else
                    {
                        MessageBox.Show("Invalid credentials. Please try again.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Database error: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\n\nDetails: {ex.InnerException.Message}";
                }
                errorMessage += "\n\nPlease check:\n1. Database connection settings\n2. PostgreSQL is running\n3. Database 'bookstore_db' exists";
                MessageBox.Show(errorMessage, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

