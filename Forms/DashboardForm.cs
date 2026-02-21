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
            this.Text = "BookStore Management System";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Padding = new Padding(20);

            // Header Panel
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.FromArgb(52, 73, 94)
            };

            var titleLabel = new Label
            {
                Text = "📚 BookStore Management System",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            var subtitleLabel = new Label
            {
                Text = "Select your role to continue",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(200, 200, 200),
                AutoSize = true,
                Location = new Point(20, 60)
            };

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subtitleLabel);

            // Content Panel
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(40)
            };

            var roleLabel = new Label
            {
                Text = "Select Your Role",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(0, 30)
            };

            var adminButton = new Button
            {
                Text = "👤 Admin",
                Size = new Size(200, 80),
                Location = new Point(0, 80),
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.None
            };
            adminButton.FlatAppearance.BorderSize = 0;
            adminButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 128, 185);
            adminButton.Click += AdminButton_Click;

            var cashierButton = new Button
            {
                Text = "💰 Cashier",
                Size = new Size(200, 80),
                Location = new Point(0, 80),
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.None
            };
            cashierButton.FlatAppearance.BorderSize = 0;
            cashierButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 174, 96);
            cashierButton.Click += CashierButton_Click;

            // Center buttons on resize and initial load
            void CenterControls()
            {
                var centerX = contentPanel.Width / 2;
                adminButton.Left = centerX - 225;
                cashierButton.Left = centerX + 25;
                roleLabel.Left = (contentPanel.Width - roleLabel.Width) / 2;
            }

            contentPanel.Resize += (s, e) => CenterControls();
            contentPanel.Layout += (s, e) => CenterControls();

            contentPanel.Controls.Add(roleLabel);
            contentPanel.Controls.Add(adminButton);
            contentPanel.Controls.Add(cashierButton);

            this.Controls.Add(headerPanel);
            this.Controls.Add(contentPanel);
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

