using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OptovayaBazaMDK
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        public void SetListBoxResult(ListBox listBox)
        {
            foreach (var item in listBox.Items)
            {
                ListBoxResultForm4.Items.Add(item);
            }
            ListBoxResultForm4 = listBox;
        }

        public void SetTextSum(string text)
        {
            lblSum.Text = text;
        }

        public void SetTextFIO(string text)
        {
            lblFIO.Text = text;
        }

        public void SetTextAdress(string text)
        {
            lblAdress.Text = text;
        }

        public void SetTextNumber(string text)
        {
            lblNumber.Text = text;
        }

        private void Form4_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
