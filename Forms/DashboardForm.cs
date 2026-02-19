using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp.Forms
{
    /// <summary>
    /// Dashboard Form - Entry point for role selection
    /// </summary>
    public partial class DashboardForm : Form
    {
        public DashboardForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "BookStore Management System - Select Role";
            this.Size = new Size(500, 250);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            var label = new Label
            {
                Text = "Determine User Identity:",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(150, 30)
            };

            var adminButton = new Button
            {
                Text = "Admin",
                Size = new Size(100, 50),
                Location = new Point(150, 80),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(50, 194, 214)
            };
            adminButton.Click += AdminButton_Click;

            var cashierButton = new Button
            {
                Text = "Cashier",
                Size = new Size(100, 50),
                Location = new Point(250, 80),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(181, 211, 145)
            };
            cashierButton.Click += CashierButton_Click;

            this.Controls.Add(label);
            this.Controls.Add(adminButton);
            this.Controls.Add(cashierButton);
        }

        private void AdminButton_Click(object sender, EventArgs e)
        {
            var loginForm = new LoginForm("Admin");
            this.Hide();
            loginForm.ShowDialog();
            this.Close();
        }

        private void CashierButton_Click(object sender, EventArgs e)
        {
            var loginForm = new LoginForm("Cashier");
            this.Hide();
            loginForm.ShowDialog();
            this.Close();
        }
    }
}

