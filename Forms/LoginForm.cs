using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsApp.Data;
using WindowsFormsApp.Factory;

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
            this.Text = "Login - " + _role;
            this.Size = new Size(500, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(223, 165, 113);

            lblWelcome = new Label
            {
                Text = "Welcome",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = Color.FromArgb(223, 165, 113),
                AutoSize = true,
                Location = new Point(200, 30)
            };

            lblInstruction = new Label
            {
                Text = "Login with Your FullName",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(160, 70)
            };

            txtFullName = new TextBox
            {
                PlaceholderText = "Enter Your FullName",
                Size = new Size(260, 30),
                Location = new Point(120, 120),
                Font = new Font("Segoe UI", 11F)
            };

            txtPassword = new TextBox
            {
                PlaceholderText = "Enter Your Phone Number",
                Size = new Size(260, 30),
                Location = new Point(120, 170),
                Font = new Font("Segoe UI", 11F),
                UseSystemPasswordChar = false
            };

            btnLogin = new Button
            {
                Text = "Login",
                Size = new Size(260, 40),
                Location = new Point(120, 230),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(4, 153, 216),
                ForeColor = Color.White
            };
            btnLogin.Click += BtnLogin_Click;

            this.Controls.Add(lblWelcome);
            this.Controls.Add(lblInstruction);
            this.Controls.Add(txtFullName);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please complete all required fields", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var employee = _employeeRepo.GetEmployeeByPhone(txtPassword.Text);
            
            if (employee != null && employee.Name.Equals(txtFullName.Text, StringComparison.OrdinalIgnoreCase))
            {
                var user = UserFactory.CreateUser(_role, employee.EmployeeId, employee.Name);
                
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
    }
}

