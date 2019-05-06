using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Traverse_Returns_Addon
{
    public partial class frmReports : Form
    {
        public frmReports()
        {
            InitializeComponent();
        }

        private void frmReports_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'PRPDataSet1.tblInPRPReturnsHist' table. You can move, or remove it, as needed.

           
            this.tblInPRPReturnsHistTableAdapter.Fill(this.PRPDataSet1.tblInPRPReturnsHist,Program.strReportRaNum);
            
            this.reportViewer1.RefreshReport();
        }
    }
}