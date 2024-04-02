using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KRiuPP_LB1
{
    public partial class MainForm : Form
    {
        private Form activeForm;
        public MainForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void OpenChildForm(Form childform, object btnSender)
        {
            if (activeForm != null)
            {
                activeForm.Close();
            }
            activeForm = childform;
            childform.TopLevel = false;
            childform.FormBorderStyle = FormBorderStyle.None;
            childform.Dock = DockStyle.Fill;
            this.panel1.Controls.Add(childform);
            this.panel1.Tag = childform;
            childform.BringToFront();
            childform.Show();
        }

        private void книгиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new BookForm(), sender);
        }

        private void посетителиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new VisitorForm(), sender);
        }

        private void выданныеКнигиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new LoanForm(), sender);
        }

        private void авторыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new AuthorForm(), sender);
        }
    }
}
