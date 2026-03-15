using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1132044_final_exersice
{
    public partial class CompletedForm : Form
    {
        public CompletedForm()
        {
            InitializeComponent();
        }
        private void CompletedForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
