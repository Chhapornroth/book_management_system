using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public partial class Form1 : Form
    {
        private Button button1;
        private Label label1;
        private int clickCount = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.button1 = new Button();
            this.label1 = new Label();
            this.SuspendLayout();
            
            // Form1
            this.Text = "My Windows Forms Application";
            this.Size = new Size(500, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            // button1
            this.button1.Location = new Point(200, 150);
            this.button1.Size = new Size(100, 40);
            this.button1.Text = "Click Me!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.button1_Click);
            
            // label1
            this.label1.AutoSize = true;
            this.label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            this.label1.Location = new Point(150, 80);
            this.label1.Size = new Size(200, 21);
            this.label1.Text = "Welcome to Windows Forms!";
            this.label1.TextAlign = ContentAlignment.MiddleCenter;
            
            // Form1
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clickCount++;
            label1.Text = $"Button clicked {clickCount} time(s)!";
        }
    }
}

