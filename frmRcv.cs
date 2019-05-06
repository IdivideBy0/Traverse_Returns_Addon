using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace Traverse_Returns_Addon
{
    public partial class frmRcv : Form
    {

        public event System.EventHandler RaNumEvent;

        public frmRcv()
        {
            InitializeComponent();
            InitializeDataGridView();

        }

        private void InitializeDataGridView()
        {
            //Set up the DataGridView.
                dataGridView1.Dock = DockStyle.Fill;

                // Automatically generate the DataGridView columns.
                dataGridView1.AutoGenerateColumns = true;

                // Set up the data source.
                bindingSource1.DataSource = GetData();
                dataGridView1.DataSource = bindingSource1;

                // Automatically resize the visible rows.
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedHeaders;

                // Set the DataGridView control's border.
                dataGridView1.BorderStyle = BorderStyle.Fixed3D;

        }

        private static DataTable GetData()
        {
            DateTime thisDate = DateTime.Today;
            string correctDate = thisDate.ToString("d");
            SqlConnection conn = new SqlConnection("Data Source = PRPSQL1; Initial Catalog = PRP; Integrated Security = SSPI");

            // Query Return details date received to display todays received items


            SqlCommand RcvToday = new SqlCommand("SELECT  h.ReturnAuthNum, h.CustId, d.ReturnedPartId, d.ReturnedQty FROM tblInPRPReturnsDetail d INNER JOIN tblInPRPReturnsHeader h ON d.ReturnAuthNum = h.ReturnAuthNum Where ReceivedDate = @ReceivedDate", conn);

            RcvToday.Parameters.Add("@ReceivedDate", SqlDbType.DateTime).Value = correctDate;

            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = RcvToday;
            DataTable table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;
            adapter.Fill(table);

            return table;
        }
        private void frmRcv_Load(object sender, EventArgs e)
        {
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
 
                if (dataGridView1.CurrentRow == null) return;

                DataGridViewCell cell = dataGridView1.CurrentCell;
                if (cell.Visible)
                {
                    dataGridView1.CurrentCell = cell;
                    Program.strRaNum = dataGridView1.CurrentCell.Value.ToString();

                    if (RaNumEvent != null)
                    {
                        RaNumEvent(sender, e);

                        //firing if any change made 
                        this.Close();
                    }

                }
        }

    }

}