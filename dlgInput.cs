using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UtilModel
{
    public partial class dlgInput : Form
    {
        public string m_Caption = "";
        public string m_Text = "";
        public dlgInput()
        {
            InitializeComponent();
        }

        private void dlgInput_Load(object sender, EventArgs e)
        {
            if (m_Caption.Length > 0)
            {
                this.Text = m_Caption;
            }
            txtInput.Text = m_Text;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            m_Text = txtInput.Text;
            this.DialogResult = DialogResult.OK;
        }
    }
}