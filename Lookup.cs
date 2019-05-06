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
    public partial class frmLookup : Form
        
    {
        public event System.EventHandler RaNumHandler;

        SqlConnection conn = new SqlConnection("Data Source = PRPSQL1; Initial Catalog = PRP; Integrated Security = SSPI");

        public frmLookup()
        {
            InitializeComponent();
        }

        private void frmLookup_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'pRPDataSet1.stpSmLookup' table. You can move, or remove it, as needed.
            
            this.stpSmLookupTableAdapter.FillBy(this.pRPDataSet1.stpSmLookup);
            cboCriteria.SelectedItem = "Any Part of Field";

        } 


        private void btnFind_Click(object sender, EventArgs e)
        {
            string strSQL = String.Empty;
            BuildQuery(ref strSQL);

            
            SqlDataAdapter dataAdapter = new SqlDataAdapter(strSQL, conn);
            BindingSource bindingSource1 = new BindingSource();

            DataTable table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;
            dataAdapter.Fill(table);
            bindingSource1.DataSource = table;

            dataGridView1.DataSource = bindingSource1;
            bindingNavigator1.BindingSource = bindingSource1;

        }
        private void BuildQuery(ref string strSQL)
        {
            string strLookBy = String.Empty;

            conn.Open();

            SqlCommand GetLookBy = new SqlCommand("Select FieldName from stpSmLookup where Description = '" + cboLookBy.Text + "' and SrchID = 'frmInPRPReturns'", conn);
            
            SqlDataReader reader = null;
            reader = GetLookBy.ExecuteReader();
            
            if (reader != null)
            {
                while (reader.Read())
                {
                    strLookBy = (reader["FieldName"].ToString());
                }

            }
            conn.Close();


            string strCriteria = cboCriteria.SelectedItem.ToString();
            string strSearch = txtSearch.Text;
            string strSQLCriteria = String.Empty;
            string strTable = String.Empty;
            

            switch (strCriteria)
            {
                
                case "Any Part of Field":
                    strSQLCriteria = " Like '%"+ strSearch +"%'"; 
                    break;
                case "Whole Field":
                    strSQLCriteria = " = '"+ strSearch +"'";
                    break;
                case "Start of Field":
                    strSQLCriteria = " Like '" + strSearch + "%'";
                    break;

            }
            if (cboLookBy.Text != "Returned Item")
            {
                strSQL = "Select * from tblInPRPReturnsHeader where " + strLookBy + strSQLCriteria + "";
            }
            else
            {
                strSQL = "Select * from tblInPRPReturnsDetail where " + strLookBy + strSQLCriteria + "";
            }
           
        }

        private void cboLookBy_SelectionChangeCommitted(object sender, EventArgs e)
        {
            switch (cboLookBy.Text)
            {
                case "Customer ID":
                    dataGridView1.DataSource = this.lkpInPRPReturnsHeaderBindingSource;
                    break;
                case "RA Number":
                    dataGridView1.DataSource = this.lkpInPRPReturnsHeaderBindingSource;
                    break;
                case "Invoice Number":
                    dataGridView1.DataSource = this.lkpInPRPReturnsHeaderBindingSource;
                    break;
                case "Packing List":
                    dataGridView1.DataSource = this.lkpInPRPReturnsHeaderBindingSource;
                    break;
                case "Returned Item":
                    dataGridView1.DataSource = this.lkpInPRPReturnsDetailBindingSource;
                    break;

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return; 
           
            DataGridViewCell cell = dataGridView1.CurrentCell;
            if (cell.Visible)
            {
                dataGridView1.CurrentCell = cell;
                Program.strRaNum = dataGridView1.CurrentCell.Value.ToString();

                if (RaNumHandler != null)
                {
                    RaNumHandler(sender, e);

                    //firing if any change made 
                    this.Close();
                }

            }
        }


       
    }
}