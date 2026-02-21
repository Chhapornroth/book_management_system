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
        private Button btnLogin;
        private Label lblWelcome;
        private Label lblInstruction;
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
                Height = 150,
                BackColor = _role == "Admin" ? Color.FromArgb(52, 152, 219) : Color.FromArgb(46, 204, 113)
            };

            lblWelcome = new Label
            {
                Text = $"Welcome, {_role}",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 30)
            };

            lblInstruction = new Label
            {
                Text = "Please enter your credentials to continue",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(240, 240, 240),
                AutoSize = true,
                Location = new Point(20, 75)
            };

            headerPanel.Controls.Add(lblWelcome);
            headerPanel.Controls.Add(lblInstruction);

            // Content Panel
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(40, 30, 40, 30)
            };

            var lblFullName = new Label
            {
                Text = "Full Name",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(0, 20)
            };

            txtFullName = new TextBox
            {
                PlaceholderText = "Enter your full name",
                Size = new Size(350, 35),
                Location = new Point(0, 45),
                Font = new Font("Segoe UI", 11F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Padding = new Padding(10, 0, 0, 0)
            };

            var lblPhone = new Label
            {
                Text = "Phone Number",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(0, 100)
            };

            txtPassword = new TextBox
            {
                PlaceholderText = "Enter your phone number",
                Size = new Size(350, 35),
                Location = new Point(0, 125),
                Font = new Font("Segoe UI", 11F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                UseSystemPasswordChar = false,
                Padding = new Padding(10, 0, 0, 0)
            };

            btnLogin = new Button
            {
                Text = "Sign In",
                Size = new Size(350, 45),
                Location = new Point(0, 190),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = _role == "Admin" ? Color.FromArgb(52, 152, 219) : Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatAppearance.MouseOverBackColor = _role == "Admin" ? Color.FromArgb(41, 128, 185) : Color.FromArgb(39, 174, 96);
            btnLogin.Click += BtnLogin_Click;

            contentPanel.Controls.Add(lblFullName);
            contentPanel.Controls.Add(txtFullName);
            contentPanel.Controls.Add(lblPhone);
            contentPanel.Controls.Add(txtPassword);
            contentPanel.Controls.Add(btnLogin);

            this.Controls.Add(headerPanel);
            this.Controls.Add(contentPanel);
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
                    }
                    else
                    {
                        var cashierForm = new CashierForm(user);
                        cashierForm.ShowDialog();
                    }
                    
                    this.Close();
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
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

