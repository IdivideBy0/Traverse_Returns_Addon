using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.VisualBasic;

namespace Traverse_Returns_Addon
{
    public partial class frmReturns : Form
    {

        public string strUserID = System.Environment.UserName;
        public string strWrkStnID = System.Environment.MachineName;
        public bool UPDATE_MODE = false;
        public bool COMPLETED = false;
        public bool boolerr = false;
        public string errstr = String.Empty;
        public string strTabEntered = String.Empty;

        SqlConnection conn = new SqlConnection("Data Source = PRPSQL1; Initial Catalog = PRP; Integrated Security = SSPI");

        public frmReturns()
        {
            InitializeComponent();

        }

        private void frmReturns_Load(object sender, EventArgs e)
        {

            // TODO: This line of code loads data into the 'pRPDataSet1.tblInPRPReturnsDetCd' table. You can move, or remove it, as needed.
            this.tblInPRPReturnsDetCdTableAdapter.Fill(this.pRPDataSet1.tblInPRPReturnsDetCd);
            // TODO: This line of code loads data into the 'pRPDataSet1.lkpInItem' table. You can move, or remove it, as needed.
            this.lkpInItemTableAdapter.Fill(this.pRPDataSet1.lkpInItem);
            // TODO: This line of code loads data into the 'pRPDataSet1.lkpSmCompID' table. You can move, or remove it, as needed.
            this.lkpSmCompIDTableAdapter.Fill(this.pRPDataSet1.lkpSmCompID);
            // TODO: This line of code loads data into the 'pRPDataSet1.tblInPRPReturnsCodes' table. You can move, or remove it, as needed.
            this.tblInPRPReturnsCodesTableAdapter.Fill(this.pRPDataSet1.tblInPRPReturnsCodes);

            DateTime thisDate = DateTime.Today;
            string correctDate = thisDate.ToString("d");
            txtDate.Text = correctDate;

            txtUserID.Text = strUserID.ToUpper();
            txtWrkStnID.Text = strWrkStnID.ToUpper();

            lvwReturnCodes.View = View.Details;
            this.lstType.SelectedItem = "New";

            string strRcvItem = String.Empty;


            // Query Return details date received to display todays received items
            SqlDataReader reader = null;
            conn.Open();

            SqlCommand RcvToday = new SqlCommand("SELECT  h.ReturnAuthNum, h.CustId, d.ReturnedPartId, d.ReturnedQty FROM tblInPRPReturnsDetail d INNER JOIN tblInPRPReturnsHeader h ON d.ReturnAuthNum = h.ReturnAuthNum Where ReceivedDate = @ReceivedDate", conn);
            RcvToday.Parameters.Add("@ReceivedDate", SqlDbType.DateTime).Value = correctDate;

            reader = RcvToday.ExecuteReader();
            if (reader != null)
            {
                while (reader.Read())
                {
                    strRcvItem = (reader["ReturnedPartId"].ToString());
                    //strPriceId = (reader["PriceId"].ToString());
                    //strUom = (reader["Uomdflt"].ToString());
                }

            }

            conn.Close();

            if (strRcvItem == String.Empty)
            {
                btnRcv.Visible = false;
            }


        }




        private void VerifyForm(ref Boolean formerror, ref string err)
        {

            if (strTabEntered != "tabPage2") //Avoid this check when clearing form
            {

                string errFld = System.String.Empty;

                if (cboCustID.Text == System.String.Empty)
                {
                    errFld = "cboCustID";
                    formerror = true;
                }

                if (txtCustName.Text == System.String.Empty)
                {
                    errFld = "txtCustName";
                    formerror = true;
                }

                if (txtContact.Text == System.String.Empty)
                {
                    errFld = "txtContact";
                    formerror = true;
                }

                if (txtPhone.Text == System.String.Empty)
                {
                    errFld = "txtPhone";
                    formerror = true;
                }

                if (txtAddress.Text == System.String.Empty)
                {
                    errFld = "txtAddress";
                    formerror = true;
                }

                if (txtCity.Text == System.String.Empty)
                {
                    errFld = "txtCity";
                    formerror = true;
                }

                if (txtState.Text == System.String.Empty)
                {
                    errFld = "txtState";
                    formerror = true;
                }

                if (txtCountry.Text == System.String.Empty)
                {
                    errFld = "txtCountry";
                    formerror = true;
                }

                if (txtPostalCode.Text == System.String.Empty)
                {
                    errFld = "txtPostalCode";
                    formerror = true;
                }

                if (txtReplOrderNum.Text == System.String.Empty)
                {
                    errFld = "txtReplOrderNum";
                    formerror = true;
                }

                if (cboInvcNum.Text == System.String.Empty)
                {
                    errFld = "cboInvcNum";
                    formerror = true;
                }

                if (cboRetPackListNum.Text == System.String.Empty)
                {
                    errFld = "cboRetPackListNum";
                    formerror = true;
                }

                if (formerror)
                {
                    MessageBox.Show("Error field = " + errFld, "Traverse");

                    formerror = true;
                    errstr = errFld;
                }

            }
        }
        private void CreateRANumber()
        {
            //Code to generate RA #

            string TransID = String.Empty;


            conn.Open();


            SqlCommand cmd = new SqlCommand("dbo.trvsp_NextTransID", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@FunctionID", "RM"));
            cmd.Parameters.Add(new SqlParameter("@IsInt", 1));
            cmd.Parameters.Add(new SqlParameter("@TransID", 1));
            cmd.Parameters["@TransID"].Direction = ParameterDirection.Output;

            cmd.ExecuteNonQuery();
            TransID = cmd.Parameters["@TransID"].Value.ToString();


            cboReturnAuthNum.Text = TransID;

            conn.Close();

        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {

            VerifyForm(ref boolerr, ref errstr);

            if (boolerr == true)
            {
                this.tabControl1.SelectedTab = this.tabPage1;

                Controls.Find(errstr, true)[0].Focus();

                return;

            }

            LoadList();


            if ((lstType.SelectedItem.ToString() == "New") && (cboReturnAuthNum.Text == String.Empty) && (strTabEntered != "tabPage2"))
            {

                CreateRANumber();

                InsertNewRecord();

            }


            if (lstType.SelectedItem.ToString() != "New")
            {
                lblqryArHistoryMsg.Text = "Items purchased by " + cboCustID.Text + "\n" + " having Invoice number " + cboInvcNum.Text;

                this.tblInPRPReturnsDetailTableAdapter.FillBy(this.pRPDataSet1.tblInPRPReturnsDetail, cboReturnAuthNum.Text);
                this.qryArPRPHistoryTableAdapter.Fill(this.pRPDataSet1.qryArPRPHistory, cboCustID.Text, cboInvcNum.Text);

                bindingNavigatorArHistDetail.BindingSource = qryArPRPHistoryBindingSource;
            }
            else
            {
                //Use qryArPRPHistory to locate the items purchased from the specified Invoice Number and Customer.

                // Populate the text fields for entry number and ItemID.
                lblqryArHistoryMsg.Text = "Items purchased by " + cboCustID.Text + "\n" + " having Invoice number " + cboInvcNum.Text;


                this.qryArPRPHistoryTableAdapter.Fill(this.pRPDataSet1.qryArPRPHistory, cboCustID.Text, cboInvcNum.Text);

                bindingNavigatorArHistDetail.BindingSource = qryArPRPHistoryBindingSource;

                //end else 
            }

        }
        string CleanSQL(string strSQL)
        {
            strSQL = strSQL.Replace("'", "''");
            return strSQL;
        }

        private void InsertNewRecord()
        {
            try
            {
                conn.Open();
                SqlCommand InsertNewRec = new SqlCommand("INSERT INTO tblInPRPReturnsHeader (ReturnAuthNum, CustID, CustName, Contact, Phone, Address1, Address2, City, Region, Country, Zip, InvoiceNum, PackingListNum, ReplOrder, Memo, CompletedYn, CreationDate, WrkStnID, UserID, Rep1Id) " +
                "Values (@ReturnAuthNum, @CustID, @CustName, @Contact, @Phone, @Address1, @Address2, @City, @Region, @Country, @Zip, @InvoiceNum, @PackingListNum, @ReplOrder, @Memo, @CompletedYn, @CreationDate, @WrkStnID, @UserID, @Rep1Id)", conn);

                InsertNewRec.Parameters.Add("@ReturnAuthNum", SqlDbType.VarChar, 8).Value = cboReturnAuthNum.Text;
                InsertNewRec.Parameters.Add("@CustID", SqlDbType.VarChar, 10).Value = cboCustID.Text;
                InsertNewRec.Parameters.Add("@CustName", SqlDbType.VarChar, 30).Value = CleanSQL(txtCustName.Text);
                InsertNewRec.Parameters.Add("@Contact", SqlDbType.VarChar, 30).Value = CleanSQL(txtContact.Text);
                InsertNewRec.Parameters.Add("@Phone", SqlDbType.Char, 15).Value = CleanSQL(txtPhone.Text);
                InsertNewRec.Parameters.Add("@Address1", SqlDbType.VarChar, 30).Value = CleanSQL(txtAddress.Text);
                InsertNewRec.Parameters.Add("@Address2", SqlDbType.VarChar, 30).Value = String.Empty;
                InsertNewRec.Parameters.Add("@City", SqlDbType.VarChar, 30).Value = CleanSQL(txtCity.Text);
                InsertNewRec.Parameters.Add("@Region", SqlDbType.VarChar, 10).Value = CleanSQL(txtState.Text);
                InsertNewRec.Parameters.Add("@Country", SqlDbType.VarChar, 30).Value = CleanSQL(txtCountry.Text);
                InsertNewRec.Parameters.Add("@Zip", SqlDbType.VarChar, 10).Value = CleanSQL(txtPostalCode.Text);
                InsertNewRec.Parameters.Add("@InvoiceNum", SqlDbType.VarChar, 15).Value = cboInvcNum.Text;
                InsertNewRec.Parameters.Add("@PackingListNum", SqlDbType.VarChar, 15).Value = cboRetPackListNum.Text;
                InsertNewRec.Parameters.Add("@ReplOrder", SqlDbType.VarChar, 50).Value = CleanSQL(txtReplOrderNum.Text);
                InsertNewRec.Parameters.Add("@Memo", SqlDbType.VarChar, 500).Value = CleanSQL(txtCustMemo.Text);
                InsertNewRec.Parameters.Add("@CompletedYn", SqlDbType.Bit).Value = 0;
                InsertNewRec.Parameters.Add("@CreationDate", SqlDbType.DateTime).Value = txtDate.Text;
                InsertNewRec.Parameters.Add("@WrkStnID", SqlDbType.VarChar, 20).Value = txtWrkStnID.Text;
                InsertNewRec.Parameters.Add("@UserID", SqlDbType.VarChar, 20).Value = txtUserID.Text;
                InsertNewRec.Parameters.Add("@Rep1Id", SqlDbType.VarChar, 3).Value = cboRep1Id.Text;

                InsertNewRec.ExecuteNonQuery();
                conn.Close();
            }

            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }



        private void AllowEdits()
        {
            COMPLETED = false;
            UPDATE_MODE = true;
            txtCustName.ReadOnly = false;
            txtContact.ReadOnly = false;
            txtPhone.ReadOnly = false;
            txtAddress.ReadOnly = false;
            txtCity.ReadOnly = false;
            txtState.ReadOnly = false;
            txtCountry.ReadOnly = false;
            txtPostalCode.ReadOnly = false;
            txtReplOrderNum.ReadOnly = false;
            //cboInvcNum
            //cboRetPackListNum.ReadOnly = false;
            txtCustMemo.ReadOnly = false;
            //btnUpdate.Visible = true;
            bindingNavigatorAddNewItem.Visible = true;
            bindingNavigatorDeleteItem.Visible = true;
            toolStripButtonSaveUpdate.Visible = true;
            btnClear.Visible = true;
            btnCompleted.Visible = true;

        }

        private void lstType_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboReturnAuthNum.Items.Clear();
            if (lstType.SelectedItem != null)
            {
                if (lstType.SelectedItem.ToString() == "New")
                {

                    cboReturnAuthNum.Enabled = false;
                    AllowEdits();
                    pnlUserId.Visible = false;
                    btnDelete.Visible = false;
                    cboReturnAuthNum.Text = String.Empty;
                    //cboCustID.Text = String.Empty;
                    cboCustID.Refresh();
                    txtCustName.Text = String.Empty;
                    txtContact.Text = String.Empty;
                    txtPhone.Text = String.Empty;
                    txtAddress.Text = String.Empty;
                    txtCity.Text = String.Empty;
                    txtState.Text = String.Empty;
                    txtCountry.Text = String.Empty;
                    txtPostalCode.Text = String.Empty;
                    txtReplOrderNum.Text = String.Empty;
                    cboInvcNum.Text = String.Empty;
                    cboRetPackListNum.Text = String.Empty;
                    txtCustMemo.Text = String.Empty;
                    txtDate.BackColor = System.Drawing.Color.Silver;

                    bindingNavigatorAddNewItem.Visible = true;
                    bindingNavigatorDeleteItem.Visible = true;

                    DateTime thisDate = DateTime.Today;
                    string correctDate = thisDate.ToString("d");
                    txtDate.Text = correctDate;

                    txtLookup.Visible = false;

                }

                if (lstType.SelectedItem.ToString() == "Pending")
                {

                    cboReturnAuthNum.Enabled = true;
                    AllowEdits();
                    btnDelete.Visible = true;
                    txtLookup.Visible = true;
                    cboReturnAuthNum.Focus();
                }

                if (lstType.SelectedItem.ToString() == "Completed")
                {

                    cboReturnAuthNum.Enabled = true;
                    NoEdits();
                    btnDelete.Visible = false;
                    txtLookup.Visible = true;
                    cboReturnAuthNum.Focus();
                }

            }
        }

        private void cboReturnAuthNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //Populate tab1 with data matching the return auth #

                conn.Open();
                SqlCommand PopulateTab1 = new SqlCommand("Select * from dbo.tblInPRPReturnsHeader where ReturnAuthNum = @ReturnAuthNum", conn);
                PopulateTab1.Parameters.Add("@ReturnAuthNum", SqlDbType.VarChar, 8).Value = cboReturnAuthNum.Text;

                SqlDataReader reader = null;
                reader = PopulateTab1.ExecuteReader();

                while (reader.Read())
                {
                    cboCustID.Text = reader["CustID"].ToString();
                    txtCustName.Text = reader["CustName"].ToString();
                    txtContact.Text = reader["Contact"].ToString();
                    txtPhone.Text = reader["Phone"].ToString();
                    txtAddress.Text = reader["Address1"].ToString();
                    txtCity.Text = reader["City"].ToString();
                    txtState.Text = reader["Region"].ToString();
                    txtCountry.Text = reader["Country"].ToString();
                    txtPostalCode.Text = reader["Zip"].ToString();
                    txtReplOrderNum.Text = reader["ReplOrder"].ToString();
                    cboInvcNum.Text = reader["InvoiceNum"].ToString();
                    cboRetPackListNum.Text = reader["packingListNum"].ToString();
                    txtCustMemo.Text = reader["Memo"].ToString();
                    txtDate.BackColor = System.Drawing.Color.LightSkyBlue;
                    txtDate.Text = reader["CreationDate"].ToString();
                    userIDTextBox.Text = reader["UserId"].ToString();
                    cboRep1Id.Text = reader["Rep1Id"].ToString();
                    cboRep1Id.Enabled = false;

                }

                conn.Close();
                if (userIDTextBox.Text != String.Empty)
                {
                    pnlUserId.Visible = true;
                }
                txtLookup.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }



        private void cboCustID_Enter(object sender, EventArgs e)
        {
            if (boolerr != true)
            {
                this.Cursor = Cursors.WaitCursor;
                this.lkpArCustTableAdapter.Fill(this.pRPDataSet1.lkpArCust);
                this.Cursor = Cursors.Default;
            }

            boolerr = false;
        }


        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            if (UPDATE_MODE == true)
            {
                this.tblInPRPReturnsDetailBindingSource.EndEdit();
                this.tblInPRPReturnsDetailTableAdapter.Update(this.pRPDataSet1.tblInPRPReturnsDetail);
            }

        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            bool adderror = false;

            if ((returningQtyTextBox.Text == String.Empty) || (Information.IsNumeric(returningQtyTextBox.Text) == false))
            {
                MessageBox.Show("You must supply a quantity for this return");
                //return;
                adderror = true;
            }

            if (lvwReturnCodes.SelectedItems.Count == 0)
            {
                MessageBox.Show("You must supply a return code for this return");
                //lvwReturnCodes.Focus();
                errorProvider1.SetError(lvwReturnCodes, "Select a return code from the list.");
                // return;
                adderror = true;
            }

            if (returningItemIDTextBox.Text == String.Empty)
            {
                MessageBox.Show("You must supply an item number for this return");
                adderror = true;
            }

            if (txtItemMemo.Text != String.Empty)
            {
                int txtcnt = 0;
                foreach (char c in txtItemMemo.Text)
                {
                    txtcnt++;
                }
                if (txtcnt > 199)
                {
                    MessageBox.Show("Item memo text is too large (max 200 characters), please edit and try again");
                    // return;
                    adderror = true;
                }
            }

            // select count(EntryNum) where ReturnAuthNum = cboReturnAuthNum
            //If no previous records exist create the first one for this table

            if (!adderror)
            {
                int cnt = 0;
                cnt = System.Convert.ToInt16(this.tblInPRPReturnsDetailTableAdapter.CountEntryNumbers(cboReturnAuthNum.Text).ToString());
                if (cnt == 0)
                {
                    conn.Open();
                    SqlCommand InsertNewRec = new SqlCommand("INSERT INTO tblInPRPReturnsDetail (ReturnAuthNum, EntryNum, ReturningItemID, ReturningQty, ReturningCode, ExplanationText) " +
                    "Values (@ReturnAuthNum, @EntryNum, @ReturningItemID, @ReturningQty, @ReturningCode, @ExplanationText)", conn);

                    InsertNewRec.Parameters.Add("@ReturnAuthNum", SqlDbType.VarChar, 8).Value = cboReturnAuthNum.Text;
                    InsertNewRec.Parameters.Add("@EntryNum", SqlDbType.SmallInt).Value = 1;
                    InsertNewRec.Parameters.Add("@ReturningItemID", SqlDbType.VarChar, 24).Value = returningItemIDTextBox.Text;
                    InsertNewRec.Parameters.Add("@ReturningQty", SqlDbType.Decimal).Value = returningQtyTextBox.Text;
                    InsertNewRec.Parameters.Add("@ReturningCode", SqlDbType.SmallInt).Value = lblCodeID.Text;
                    InsertNewRec.Parameters.Add("@ExplanationText", SqlDbType.VarChar, 250).Value = lblDesc.Text + " - " + CleanSQL(txtItemMemo.Text);

                    InsertNewRec.ExecuteNonQuery();
                    conn.Close();

                    this.tblInPRPReturnsDetailTableAdapter.FillBy(this.pRPDataSet1.tblInPRPReturnsDetail, cboReturnAuthNum.Text);

                }
                //Else insert new EntryNum based on the count of any previous records 
                else if (cnt > 0)
                {
                    conn.Open();

                    SqlCommand GetLastEntryNum = new SqlCommand("select top 1 EntryNum from tblInPRPReturnsDetail where ReturnAuthNum = @ReturnAuthNum order by EntryNum desc", conn);
                    GetLastEntryNum.Parameters.Add("@ReturnAuthNum", SqlDbType.VarChar, 8).Value = cboReturnAuthNum.Text;

                    SqlDataReader readnum = null;
                    readnum = GetLastEntryNum.ExecuteReader();

                    if (readnum != null)
                    {
                        while (readnum.Read())
                        {
                            cnt = Convert.ToInt16((readnum["EntryNum"].ToString()));
                        }

                    }
                    conn.Close();

                    conn.Open();

                    SqlCommand InsertNewRec = new SqlCommand("INSERT INTO tblInPRPReturnsDetail (ReturnAuthNum, EntryNum, ReturningItemID, ReturningQty, ReturningCode, ExplanationText) " +
                    "Values (@ReturnAuthNum , @EntryNum, @ReturningItemID, @ReturningQty, @ReturningCode, @ExplanationText )", conn);

                    InsertNewRec.Parameters.Add("@ReturnAuthNum", SqlDbType.VarChar, 8).Value = cboReturnAuthNum.Text;
                    InsertNewRec.Parameters.Add("@EntryNum", SqlDbType.SmallInt).Value = 1 + cnt;
                    InsertNewRec.Parameters.Add("@ReturningItemID", SqlDbType.VarChar, 24).Value = returningItemIDTextBox.Text;
                    InsertNewRec.Parameters.Add("@ReturningQty", SqlDbType.Decimal).Value = returningQtyTextBox.Text;
                    InsertNewRec.Parameters.Add("@ReturningCode", SqlDbType.SmallInt).Value = lblCodeID.Text;
                    InsertNewRec.Parameters.Add("@ExplanationText", SqlDbType.VarChar, 250).Value = lblDesc.Text + " - " + CleanSQL(txtItemMemo.Text);


                    InsertNewRec.ExecuteNonQuery();
                    conn.Close();

                    this.tblInPRPReturnsDetailTableAdapter.FillBy(this.pRPDataSet1.tblInPRPReturnsDetail, cboReturnAuthNum.Text);
                }
            }
            // Return type changes to Pending

            conn.Open();
            SqlCommand GetStatus = new SqlCommand("Select CompletedYn, CreationDate from tblInPRPReturnsHeader where ReturnAuthNum = @ReturnAuthNum", conn);
            GetStatus.Parameters.Add("@ReturnAuthNum", SqlDbType.VarChar, 8).Value = cboReturnAuthNum.Text;

            SqlDataReader reader = null;
            reader = GetStatus.ExecuteReader();

            string strCompletedYn = String.Empty;
            string strEntryDate = String.Empty;

            while (reader.Read())
            {
                strCompletedYn = reader["CompletedYn"].ToString();
                strEntryDate = reader["CreationDate"].ToString();
            }

            conn.Close();

            if ((strEntryDate != null) & (strCompletedYn == "False"))
            {
                this.lstType.ClearSelected();
                this.lstType.SelectedItem = "Pending";
                this.lstType.Refresh();

                AllowEdits();

            }
            if (adderror)
            {
                dataGridView1.Rows[0].Selected = false;
                return;
            }
            else
            {
                txtItemMemo.Text = String.Empty;
                returningItemIDTextBox.Text = String.Empty;
                returningQtyTextBox.Text = String.Empty;
                lvwReturnCodes.SelectedItems.Clear();
            }

        }

        private void cboInvcNum_Enter(object sender, EventArgs e)
        {
            this.lkpArHistHeaderTableAdapter.FillBy(this.pRPDataSet1.lkpArHistHeader, cboCustID.Text);
        }

        private void LoadList()
        {
            // Get the table from the data set

            DataTable dtable = pRPDataSet1.Tables["tblInPRPReturnsCodes"];

            // Clear the ListView control
            // lvwReturnCodes.Items.Clear();

            // Display items in the ListView control
            for (int i = 0; i < dtable.Rows.Count; i++)
            {
                DataRow drow = dtable.Rows[i];

                // Only row that have not been deleted
                if (drow.RowState != DataRowState.Deleted)
                {
                    // Define the list items
                    ListViewItem lvi = new ListViewItem(drow["CodeID"].ToString());
                    lvi.SubItems.Add(drow["ReturnCodes"].ToString());

                    // Add the list items to the ListView
                    lvwReturnCodes.Items.Add(lvi);
                }
            }
        }

        private void lvwReturnCodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strTmp = String.Empty;
            string strID = String.Empty;
            string strDesc = String.Empty;

            ListView.SelectedListViewItemCollection SelectedItems = this.lvwReturnCodes.SelectedItems;
            if ((SelectedItems.Count > 0))
            {
                strTmp = (SelectedItems[0].SubItems[0].Text + "|" + SelectedItems[0].SubItems[1].Text);

                strID = strTmp.Substring(0, strTmp.IndexOf("|"));
                strDesc = strTmp.Substring(strTmp.IndexOf("|") + 1);

                lblCodeID.Text = strID;
                lblDesc.Text = strDesc;
            }
            errorProvider1.SetError(lvwReturnCodes, "");
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            //delete the row
            this.tblInPRPReturnsDetailTableAdapter.Update(this.pRPDataSet1.tblInPRPReturnsDetail);
            txtItemMemo.Text = String.Empty;
            //entryNumTextBox.Text = String.Empty;
            returningItemIDTextBox.Text = String.Empty;
            returningQtyTextBox.Text = String.Empty;
            lvwReturnCodes.SelectedItems.Clear();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            AllowEdits();
            UPDATE_MODE = true;
            COMPLETED = false;
        }

        private void NoEdits()
        {
            txtCustName.ReadOnly = true;
            txtContact.ReadOnly = true;
            txtPhone.ReadOnly = true;
            txtAddress.ReadOnly = true;
            txtCity.ReadOnly = true;
            txtState.ReadOnly = true;
            txtCountry.ReadOnly = true;
            txtPostalCode.ReadOnly = true;
            txtReplOrderNum.ReadOnly = true;
            // cboInvcNum
            //cboRetPackListNum.ReadOnly = true;
            txtCustMemo.ReadOnly = true;

            bindingNavigatorAddNewItem.Visible = false;
            bindingNavigatorDeleteItem.Visible = false;

            //btnUpdate.Visible = false;

            toolStripButtonSaveUpdate.Visible = false;
            btnClear.Visible = false;
            btnCompleted.Visible = false;
            UPDATE_MODE = false;
            COMPLETED = true;
        }

        private void txtCustMemo_Leave(object sender, EventArgs e)
        {
            if (UPDATE_MODE)
            {
                conn.Open();
                SqlCommand UpdateCustMemo = new SqlCommand("UPDATE tblInPRPReturnsHeader SET Memo = @Memo WHERE ReturnAuthNum = @ReturnAuthNum", conn);

                UpdateCustMemo.Parameters.Add("@Memo", SqlDbType.VarChar, 500).Value = CleanSQL(txtCustMemo.Text);
                UpdateCustMemo.Parameters.Add("@ReturnAuthNum", SqlDbType.VarChar, 8).Value = cboReturnAuthNum.Text;

                UpdateCustMemo.ExecuteNonQuery();

                conn.Close();

            }
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (UPDATE_MODE)
            {
                this.tblInPRPReturnsDetailBindingSource.EndEdit();
                this.tblInPRPReturnsDetailTableAdapter.Update(this.pRPDataSet1.tblInPRPReturnsDetail);
            }
        }

        private void tabPage3_Enter(object sender, EventArgs e)
        {

            cboEntryNum.Text = Program.sglbEntryNum;
            if (cboEntryNum.Text == String.Empty)
            {

                // Restore the cursor position and populate the entrynum textbox
                this.dataGridView2.CurrentCell = this.dataGridView2.Rows[0].Cells[1];

                //store entrynum for later use
                Program.sglbEntryNum = this.dataGridView2.CurrentCell.Value.ToString();
                cboEntryNum.Text = Program.sglbEntryNum;
            }

            DateTime thisDate = DateTime.Today;
            string rcvDate = thisDate.ToString("d");
            txtRcvDate.Text = rcvDate;
            txtRcvdBy.Text = strUserID.ToUpper();


            if (cboReturnAuthNum.Text != String.Empty)
            {
                //long strRaNum = Convert.ToInt64(cboReturnAuthNum.Text);
                txtFinalComments.Text = this.tblInPRPReturnsHeaderTableAdapter.FillMemo(cboReturnAuthNum.Text);
                bindingNavigatorCompleted.BindingSource = tblInPRPReturnsDetailBindingSource;
                this.tblInPRPReturnsDetailTableAdapter.FillBy(this.pRPDataSet1.tblInPRPReturnsDetail, cboReturnAuthNum.Text);
                // Get the table from the data set

                DataTable dtable = pRPDataSet1.Tables["tblInPRPReturnsDetCd"];

                // Clear the ListView control
                // lvwReturnCodes.Items.Clear();

                // Display items in the ListView control
                for (int i = 0; i < dtable.Rows.Count; i++)
                {
                    DataRow drow = dtable.Rows[i];

                    // Only row that have not been deleted
                    if (drow.RowState != DataRowState.Deleted)
                    {
                        // Define the list items
                        ListViewItem lvi = new ListViewItem(drow["DetID"].ToString());
                        lvi.SubItems.Add(drow["DetDescr"].ToString());

                        // Add the list items to the ListView
                        lvwDeterminationCodes.Items.Add(lvi);
                    }
                }
            }
        }

        private void lvwDeterminationCodes_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            string strTmp = String.Empty;
            string strID = String.Empty;
            string strDesc = String.Empty;

            ListView.SelectedListViewItemCollection SelectedItems = this.lvwDeterminationCodes.SelectedItems;
            if ((SelectedItems.Count > 0))
            {
                strTmp = (SelectedItems[0].SubItems[0].Text + "|" + SelectedItems[0].SubItems[1].Text);

                strID = strTmp.Substring(0, strTmp.IndexOf("|"));
                strDesc = strTmp.Substring(strTmp.IndexOf("|") + 1);

                lblDeterminationCodeNum.Text = strID;
                lblDeterminationCodeDescr.Text = strDesc;

            }
        }



        private void toolStripButtonSaveUpdate_Click(object sender, EventArgs e)
        {

            // Check to see if this itemid is in the returning datagridview - if not warn item is different than expected
            conn.Open();
            SqlCommand GetItemIds = new SqlCommand("Select ReturningItemId, ReturningQty from tblInPRPReturnsDetail WHERE ReturningItemId = @ReturningItemId", conn);

            GetItemIds.Parameters.Add("@ReturningItemId", SqlDbType.VarChar, 24).Value = cboReturnedItemID.Text;

            SqlDataReader reader = null;
            reader = GetItemIds.ExecuteReader();

            string strItemFound = String.Empty;
            string strItemQty = String.Empty;

            if (reader != null)
            {
                while (reader.Read())
                {
                    strItemFound = reader["ReturningItemId"].ToString();
                    strItemQty = reader["ReturningQty"].ToString();
                }

            }

            conn.Close();

            if (strItemFound == String.Empty)
            {
                DialogResult dr1 = MessageBox.Show("Warning, this item differs from items expected, are you sure?", "Attention!", MessageBoxButtons.YesNo);
                if (dr1 == DialogResult.No)
                {
                    return;
                }
            }
            else if (strItemFound == cboReturnedItemID.Text)
            {

                if (Convert.ToDouble(txtReturnedQty.Text) != Convert.ToDouble(strItemQty))
                {
                    DialogResult dr2 = MessageBox.Show("Quantity differs than exepected, are you sure?", "Attention!", MessageBoxButtons.YesNo);
                    if (dr2 == DialogResult.No)
                    {
                        return;
                    }

                }
            }


            if (cboReturnedItemID.Text == String.Empty)
            {
                MessageBox.Show("ItemId cannot be blank.");
                return;
            }
            if (txtReturnedQty.Text == String.Empty)
            {
                MessageBox.Show("Quantity cannot be blank.");
                return;
            }
            if (lblDeterminationCodeNum.Text == String.Empty)
            {
                MessageBox.Show("Must supply a determination code.");
                return;
            }
            if (txtRestockFee.Text == String.Empty)
            {
                MessageBox.Show("Restocking fee cannot be blank.");
                return;
            }

            string strEntryNum = cboEntryNum.Text;

            double dblUnitPrice = Convert.ToDouble(txtUnitPrice.Text);
            double dblTotalPrice = Convert.ToDouble(txtTotalPrice.Text);
            double dblRestockFee = Convert.ToDouble(txtRestockFee.Text);
            string strSolution = String.Empty;


            // Ask if the user would like to include a solution with this item.
            DialogResult dr = MessageBox.Show("Would you like to include a solution for this item at this time?", "Attention!", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                //string x = Interaction.InputBox("hi", "hello", "nothing", 10, 10);
                strSolution = Interaction.InputBox("Enter the solution below.", "Item Solution", "", 10, 10);
            }

            conn.Open();

            string strSql = "UPDATE tblInPRPReturnsDetail Set ReturnedPartId = @ReturnedPartId " +
",ReturnedQty = @ReturnedQty, ReceivedDate = @ReceivedDate, ReceivedBy = @ReceivedBy, UnitPrice = @UnitPrice, TotalPrice = @TotalPrice " +
",DeterminationCode = @DeterminationCode, RestockingFee = @RestockingFee, Solution = @Solution " +
"WHERE EntryNum = '" + @EntryNum + "' AND ReturnAuthNum = @ReturnAuthNum";

            SqlCommand UpdateRec = new SqlCommand(strSql, conn);

            UpdateRec.Parameters.Add("@ReturnedPartId", SqlDbType.VarChar, 24).Value = cboReturnedItemID.Text;
            UpdateRec.Parameters.Add("@ReturnedQty", SqlDbType.Decimal).Value = txtReturnedQty.Text;
            UpdateRec.Parameters.Add("@ReceivedDate", SqlDbType.DateTime, 24).Value = txtRcvDate.Text;
            UpdateRec.Parameters.Add("@ReceivedBy", SqlDbType.VarChar, 50).Value = txtRcvdBy.Text;
            UpdateRec.Parameters.Add("@UnitPrice", SqlDbType.Decimal).Value = dblUnitPrice;
            UpdateRec.Parameters.Add("@TotalPrice", SqlDbType.Decimal).Value = dblTotalPrice;
            UpdateRec.Parameters.Add("@DeterminationCode", SqlDbType.SmallInt).Value = lblDeterminationCodeNum.Text;
            UpdateRec.Parameters.Add("@RestockingFee", SqlDbType.Decimal).Value = dblRestockFee;
            UpdateRec.Parameters.Add("@Solution", SqlDbType.VarChar, 1000).Value = CleanSQL(strSolution);
            UpdateRec.Parameters.Add("@EntryNum", SqlDbType.SmallInt).Value = strEntryNum;
            UpdateRec.Parameters.Add("@ReturnAuthNum", SqlDbType.VarChar, 8).Value = cboReturnAuthNum.Text;
            
            UpdateRec.ExecuteNonQuery();
            conn.Close();

            this.tblInPRPReturnsDetailTableAdapter.FillBy(this.pRPDataSet1.tblInPRPReturnsDetail, cboReturnAuthNum.Text);

            txtSubTotal.Text = this.tblInPRPReturnsDetailTableAdapter.SubTotal(cboReturnAuthNum.Text).ToString();

            conn.Open();

            strSql = String.Empty;

            strSql = "Update tblInPRPReturnsDetail Set SubTotal = @SubTotal where ReturnAuthNum = @ReturnAuthNum";
            SqlCommand UpdateSubTotal = new SqlCommand(strSql, conn);

            UpdateSubTotal.Parameters.Add("@SubTotal", SqlDbType.Decimal).Value = txtSubTotal.Text;
            UpdateSubTotal.Parameters.Add("@ReturnAuthNum", SqlDbType.VarChar, 8).Value = cboReturnAuthNum.Text;

            UpdateSubTotal.ExecuteNonQuery();
            conn.Close();

            this.tblInPRPReturnsDetailTableAdapter.FillBy(this.pRPDataSet1.tblInPRPReturnsDetail, cboReturnAuthNum.Text);

            // Clean up tab contents after table updates
            cboReturnedItemID.Text = String.Empty;
            txtDescr.Text = String.Empty;
            txtUnitPrice.Text = String.Empty;
            txtReturnedQty.Text = String.Empty;
            txtTotalPrice.Text = String.Empty;

            // find the next row
            int gridrowindex = this.tblInPRPReturnsDetailBindingSource.Find("EntryNum", strEntryNum) + 1;
            // Restore the cursor position
            this.dataGridView2.CurrentCell = this.dataGridView2.Rows[gridrowindex].Cells[1];
            //store entrynum for later use
            Program.sglbEntryNum = this.dataGridView2.CurrentCell.Value.ToString();
            cboEntryNum.Text = Program.sglbEntryNum;
            txtRestockFee.Text = String.Empty;


        }

        private void btnClearForm_Click(object sender, EventArgs e)
        {

            ClearForm(sender, e);

        }

        private void ClearForm(object sender, EventArgs e)
        {
            this.errorProvider1.Clear();
            this.Cursor = Cursors.WaitCursor;
            frmReturns_Load(this, e);
            this.tabControl1.SelectedTab = this.tabPage1;
            cboReturnAuthNum.Text = String.Empty;
            cboCustID.Text = String.Empty;
            txtCustName.Text = String.Empty;
            txtContact.Text = String.Empty;
            txtPhone.Text = String.Empty;
            txtAddress.Text = String.Empty;
            txtCity.Text = String.Empty;
            txtState.Text = String.Empty;
            txtCountry.Text = String.Empty;
            txtPostalCode.Text = String.Empty;
            txtReplOrderNum.Text = String.Empty;
            cboInvcNum.Text = String.Empty;
            cboRetPackListNum.Text = String.Empty;
            returningQtyTextBox.Text = String.Empty;
            cboRep1Id.Text = String.Empty;
            txtCustMemo.Text = String.Empty;
            txtSubTotal.Text = String.Empty;
            txtTotalPrice.Text = String.Empty;
            txtUnitPrice.Text = String.Empty;
            cboEntryNum.Text = String.Empty;
            pRPDataSet1.Tables["tblInPRPReturnsDetail"].Rows.Clear();
            this.Cursor = Cursors.Default;
            Program.sglbEntryNum = String.Empty;
            Program.strRaNum = String.Empty;
            Program.strReportRaNum = String.Empty;
            lstType.Focus();

        }

        private void txtFinalComments_Leave(object sender, EventArgs e)
        {
            if (UPDATE_MODE)
            {
                conn.Open();
                SqlCommand UpdateCustMemo = new SqlCommand("UPDATE tblInPRPReturnsHeader SET Memo = @Memo WHERE ReturnAuthNum = @ReturnAuthNum", conn);

                UpdateCustMemo.Parameters.Add("@Memo", SqlDbType.VarChar, 500).Value = CleanSQL(txtFinalComments.Text);
                UpdateCustMemo.Parameters.Add("@ReturnAuthNum", SqlDbType.VarChar, 8).Value = cboReturnAuthNum.Text;

                UpdateCustMemo.ExecuteNonQuery();

                conn.Close();

            }

        }


        private void cboReturnAuthNum_Enter(object sender, EventArgs e)
        {
            try
            {
                cboReturnAuthNum.Items.Clear();
                if ((lstType.SelectedItem.ToString() == "Pending") && (cboReturnAuthNum.Text == String.Empty))
                {
                    conn.Open();
                    SqlCommand GetRaNumbers = new SqlCommand("Select ReturnAuthNum from tblInPRPReturnsHeader WHERE CompletedYn = '0' order by CAST(ReturnAuthNum as int) Desc", conn);
                    SqlDataReader reader = null;
                    reader = GetRaNumbers.ExecuteReader();

                    if (reader != null)
                    {

                        while (reader.Read())
                        {

                            cboReturnAuthNum.Items.Add(reader["ReturnAuthNum"]);

                        }

                    }

                    conn.Close();
                }

                if ((lstType.SelectedItem.ToString() == "Completed") && (cboReturnAuthNum.Text == String.Empty))
                {
                    conn.Open();
                    SqlCommand GetRaNumbers = new SqlCommand("Select ReturnAuthNum from tblInPRPReturnsHeader WHERE CompletedYn = '1' order by CAST(ReturnAuthNum as int) Desc", conn);
                    SqlDataReader reader = null;
                    reader = GetRaNumbers.ExecuteReader();

                    if (reader != null)
                    {

                        while (reader.Read())
                        {

                            cboReturnAuthNum.Items.Add(reader["ReturnAuthNum"]);

                        }

                    }

                    conn.Close();
                    NoEdits();

                }

                if ((lstType.SelectedItem.ToString() == "New") && (cboReturnAuthNum.Text == String.Empty))
                {
                    cboCustID.Focus();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

            if (cboReturnAuthNum.Text != null)
            {
                conn.Open();
                SqlCommand DeleteHeader = new SqlCommand("Delete from tblInPRPReturnsHeader where ReturnAuthNum = @ReturnAuthNum", conn);

                DeleteHeader.Parameters.Add("@ReturnAuthNum", SqlDbType.VarChar, 8).Value = cboReturnAuthNum.Text;

                DeleteHeader.ExecuteNonQuery();
                conn.Close();

                conn.Open();
                SqlCommand DeleteDetail = new SqlCommand("Delete from tblInPRPReturnsDetail where ReturnAuthNum = @ReturnAuthNum", conn);

                DeleteDetail.Parameters.Add("@ReturnAuthNum", SqlDbType.VarChar, 8).Value = cboReturnAuthNum.Text;
                
                DeleteDetail.ExecuteNonQuery();
                conn.Close();
                ClearForm(sender, e);
            }
        }

        private void cboReturnAuthNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (lstType.SelectedItem.ToString() != "New" && cboReturnAuthNum.Focused == true && cboReturnAuthNum.Text == String.Empty)
            {

                KeyPreview = true;

                if (e.KeyCode == Keys.F2)
                {
                    //Clear contents of cboReturnAuthNum
                    cboReturnAuthNum.Items.Clear();

                    frmLookup f = new frmLookup();
                    f.RaNumHandler += new EventHandler(RaNumEvent);
                    f.Show();
                }
            }
        }

        private void RaNumEvent(object sender, EventArgs e)
        {
            cboReturnAuthNum.Text = Program.strRaNum;
            cboReturnAuthNum_TextUpdate(sender, e);
        }
        /*
         * 
         * MJM/PRP 7/16/2014 debugging this 
         * */
        private void cboReturnAuthNum_TextUpdate(object sender, EventArgs e)
        {
            try
            {
                //Populate tab1 with data matching the return auth #
                string strCompletedYn = String.Empty;

                conn.Open();
                SqlCommand PopulateTab1 = new SqlCommand("Select * from tblInPRPReturnsHeader where ReturnAuthNum = @ReturnAuthNum", conn);

                PopulateTab1.Parameters.Add("@ReturnAuthNum", SqlDbType.VarChar, 8).Value = cboReturnAuthNum.Text;

                SqlDataReader reader = null;
                reader = PopulateTab1.ExecuteReader();

                while (reader.Read())
                {
                    cboCustID.Text = reader["CustID"].ToString();
                    txtCustName.Text = reader["CustName"].ToString();
                    txtContact.Text = reader["Contact"].ToString();
                    txtPhone.Text = reader["Phone"].ToString();
                    txtAddress.Text = reader["Address1"].ToString();
                    txtCity.Text = reader["City"].ToString();
                    txtState.Text = reader["Region"].ToString();
                    txtCountry.Text = reader["Country"].ToString();
                    txtPostalCode.Text = reader["Zip"].ToString();
                    txtReplOrderNum.Text = reader["ReplOrder"].ToString();
                    cboInvcNum.Text = reader["InvoiceNum"].ToString();
                    cboRetPackListNum.Text = reader["packingListNum"].ToString();
                    txtCustMemo.Text = reader["Memo"].ToString();
                    strCompletedYn = reader["CompletedYn"].ToString();
                    txtDate.BackColor = System.Drawing.Color.LightSkyBlue;
                    txtDate.Text = reader["CreationDate"].ToString();
                    userIDTextBox.Text = reader["UserId"].ToString();
                    cboRep1Id.Text = reader["Rep1Id"].ToString();
                }

                conn.Close();
                if ((txtDate.Text != null) & (strCompletedYn == "False"))
                {
                    this.lstType.ClearSelected();
                    this.lstType.SelectedItem = "Pending";
                    this.lstType.Refresh();
                }

                else if ((txtDate.Text != null) & (strCompletedYn == "True"))
                {
                    this.lstType.ClearSelected();
                    this.lstType.SelectedItem = "Completed";
                    this.lstType.Refresh();
                }

                if (userIDTextBox.Text != String.Empty)
                {
                    pnlUserId.Visible = true;
                }
                txtLookup.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void tabPage2_Leave(object sender, EventArgs e)
        {
            strTabEntered = "tabPage2";

        }

        private void tabPage1_Enter(object sender, EventArgs e)
        {
            strTabEntered = "tabPage1";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmRcv f = new frmRcv();
            f.RaNumEvent += new EventHandler(RaNumEvent);
            f.Show();
        }


        private void txtFinalComments_Leave_1(object sender, EventArgs e)
        {
            if (UPDATE_MODE)
            {
                conn.Open();
                SqlCommand UpdateCustMemo = new SqlCommand("UPDATE tblInPRPReturnsHeader SET Memo = @Memo WHERE ReturnAuthNum = @ReturnAuthNum", conn);

                UpdateCustMemo.Parameters.Add("@Memo", SqlDbType.VarChar, 500).Value = CleanSQL(txtCustMemo.Text);
                UpdateCustMemo.Parameters.Add("@ReturnAuthNum", SqlDbType.VarChar, 8).Value = cboReturnAuthNum.Text;

                UpdateCustMemo.ExecuteNonQuery();

                conn.Close();

            }
        }

        private void btnCompleted_Click(object sender, EventArgs e)
        {

            DialogResult dr = MessageBox.Show("OK to complete this return for import to credit memo?", "Attention!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dr == DialogResult.Yes)
            {
                SqlCommand Complete;
                conn.Open();
                Complete = new SqlCommand("UPDATE tblInPRPReturnsDetail SET SubTotal = @SubTotal WHERE ReturnAuthNum = @ReturnAuthNum", conn);

                Complete.Parameters.Add("@SubTotal", SqlDbType.Decimal).Value = txtSubTotal.Text;
                Complete.Parameters.Add("@ReturnAuthNum", SqlDbType.VarChar, 8).Value = cboReturnAuthNum.Text;
                
                Complete.ExecuteNonQuery();
                conn.Close();

                conn.Open();
                Complete = new SqlCommand("UPDATE tblInPRPReturnsHeader SET CompletedYn = '1' WHERE ReturnAuthNum = @ReturnAuthNum", conn);

                Complete.Parameters.Add("@ReturnAuthNum", SqlDbType.VarChar, 8).Value = cboReturnAuthNum.Text;

                Complete.ExecuteNonQuery();
                conn.Close();

                this.lstType.SelectedItem = "Completed";
                NoEdits();
            }
            else
            {
                cboReturnedItemID.Focus();
            }
        }






        private void cboReturnedItemID_Validating(object sender, CancelEventArgs e)
        {
            // Validate ItemId here
            conn.Open();

            SqlCommand ValidItem = new SqlCommand("Select ItemId from lkpInItem where ItemId = @ItemId", conn);
            ValidItem.Parameters.Add("@ItemId", SqlDbType.VarChar, 24).Value = cboReturnedItemID.Text;

            SqlDataReader readitem = null;
            readitem = ValidItem.ExecuteReader();

            string strValidItemId = String.Empty;
            if (readitem != null)
            {
                while (readitem.Read())
                {
                    strValidItemId = readitem["ItemId"].ToString();
                }
            }
            conn.Close();

            if (strValidItemId == String.Empty)
            {

                //MessageBox.Show("The ItemId entered is invalid!", "Attention!");
                //cboReturnedItemID.Focus();

                e.Cancel = true;
                //return;
                // Set the ErrorProvider error with the text to display. 
                cboReturnedItemID.Focus();
                this.errorProvider1.SetError(cboReturnedItemID, "The ItemId entered is invalid!");

            }

        }

        private void cboReturnedItemID_Validated(object sender, EventArgs e)
        {
            //// If the ItemId is valid

            errorProvider1.SetError(cboReturnedItemID, "");

            txtReturnedQty.Text = String.Empty;
            txtTotalPrice.Text = String.Empty;
            txtUnitPrice.Text = String.Empty;
            string strCustLevel = String.Empty;
            string strItemId = String.Empty;
            string strPriceId = String.Empty;
            string strUom = String.Empty;
            string glbLocID = String.Empty;
            string glbPrecUCost = String.Empty;
            string glbPrecUPrice = String.Empty;
            string strArHistPrice = "0";

            // get the last sale price from ArHist. If the item differs from what was returned, lookup current pricing for that item.

            conn.Open();

            SqlCommand GetArHistPrice = new SqlCommand("Select UnitPriceSell from lkpArHistDetail where" +
                " TransId = @TransId and PartId = @PartId", conn);

            GetArHistPrice.Parameters.Add("@TransId", SqlDbType.VarChar, 8).Value = cboRetPackListNum.Text;
            GetArHistPrice.Parameters.Add("@PartId", SqlDbType.VarChar, 24).Value = cboReturnedItemID.Text;

            SqlDataReader readprice = null;
            readprice = GetArHistPrice.ExecuteReader();

            while (readprice.Read())
            {
                strArHistPrice = (readprice["UnitPriceSell"].ToString());
            }

            if (strArHistPrice != "0")
            {

                Decimal CalcPrice = Convert.ToDecimal(strArHistPrice);
                txtUnitPrice.Text = String.Format("{0:N}", CalcPrice);
                conn.Close();
            }
            else
            {

                conn.Close();

                conn.Open();
                SqlCommand GetCustLevel = new SqlCommand("Select CustLevel from lkpArCust where CustID = @CustID", conn);

                GetCustLevel.Parameters.Add("@CustID", SqlDbType.VarChar, 10).Value = cboCustID.Text;

                SqlDataReader reader = null;
                reader = GetCustLevel.ExecuteReader();
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        strCustLevel = (reader["CustLevel"].ToString());
                    }

                }
                conn.Close();

                conn.Open();
                SqlCommand GetInItems = new SqlCommand("Select * from lkpInItem where ItemId = @ItemId", conn);

                GetInItems.Parameters.Add("@ItemId", SqlDbType.VarChar, 24).Value = cboReturnedItemID.Text;

                reader = GetInItems.ExecuteReader();
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        strItemId = (reader["ItemId"].ToString());
                        strPriceId = (reader["PriceId"].ToString());
                        strUom = (reader["Uomdflt"].ToString());
                    }

                }

                conn.Close();

                //strItemId = pRPDataSet1.Tables["lkpInItem"].Rows[lkpInItemBindingSource.Position].ItemArray[0].ToString();
                //strPriceId = pRPDataSet1.Tables["lkpInItem"].Rows[lkpInItemBindingSource.Position].ItemArray[2].ToString();
                //strUom = pRPDataSet1.Tables["lkpInItem"].Rows[lkpInItemBindingSource.Position].ItemArray[3].ToString();
                glbLocID = pRPDataSet1.Tables["lkpSmCompID"].Rows[0].ItemArray[1].ToString();
                glbPrecUCost = pRPDataSet1.Tables["lkpSmCompID"].Rows[0].ItemArray[2].ToString();
                glbPrecUPrice = pRPDataSet1.Tables["lkpSmCompID"].Rows[0].ItemArray[3].ToString();

                conn.Open();


                SqlCommand cmd = new SqlCommand("dbo.qrySoPriceCalc", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter("@ItemID", strItemId));
                cmd.Parameters.Add(new SqlParameter("@LocID", glbLocID));
                cmd.Parameters.Add(new SqlParameter("@Uom", strUom));
                cmd.Parameters.Add(new SqlParameter("@Qty", 1));
                cmd.Parameters.Add(new SqlParameter("@CustLevel", strCustLevel));
                cmd.Parameters.Add(new SqlParameter("@PriceID", strPriceId));
                cmd.Parameters.Add(new SqlParameter("@Date", DateTime.Today));
                cmd.Parameters.Add(new SqlParameter("@SerNum", ""));
                cmd.Parameters.Add(new SqlParameter("@gPrecUCost", Convert.ToDouble(glbPrecUCost)));
                cmd.Parameters.Add(new SqlParameter("@gPrecUPrice", Convert.ToDouble(glbPrecUPrice)));
                cmd.Parameters.Add(new SqlParameter("@DiscountsAllowed", true));
                cmd.Parameters.Add(new SqlParameter("@CalcPrice", SqlDbType.Decimal));
                cmd.Parameters["@CalcPrice"].Precision = 8;
                cmd.Parameters["@CalcPrice"].Scale = 2;
                cmd.Parameters["@CalcPrice"].Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                Decimal CalcPrice = Convert.ToDecimal(cmd.Parameters["@CalcPrice"].Value);
                txtUnitPrice.Text = String.Format("{0:N}", CalcPrice);
                conn.Close();
            }
        }

        private void txtReturnedQty_Validating(object sender, CancelEventArgs e)
        {
            string Str = txtReturnedQty.Text.Trim();
            double Num;
            bool isNum = double.TryParse(Str, out Num);



            ////

            if (Str.StartsWith("-"))
            {
                e.Cancel = true;
                //MessageBox.Show("Negative quantities are not allowed for this trasaction type", "Attention!");
                txtReturnedQty.Select(0, txtReturnedQty.Text.Length);
                this.errorProvider1.SetError(txtReturnedQty, "Negative quantities are not allowed for this trasaction type");
            }

            if (!isNum)
            {
                e.Cancel = true;
                //MessageBox.Show("Please enter a numeric quantity", "Attention!");
                txtReturnedQty.Select(0, txtReturnedQty.Text.Length);
                this.errorProvider1.SetError(txtReturnedQty, "Please enter a numeric quantity");

            }

            ////

        }

        private void txtReturnedQty_Validated(object sender, EventArgs e)
        {
            this.errorProvider1.SetError(txtReturnedQty, "");
            Double i = (Convert.ToDouble(txtUnitPrice.Text)) * (Convert.ToDouble(txtReturnedQty.Text));
            txtTotalPrice.Text = String.Format("{0:N}", i);
        }

        private void txtRestockFee_Validating(object sender, CancelEventArgs e)
        {
            string Str = txtRestockFee.Text;
            double Num;
            bool isNum = double.TryParse(Str, out Num);

            if (Str.StartsWith("-"))
            {
                //MessageBox.Show("Negative values are not allowed for restocking fees", "Attention!");
                e.Cancel = true;

                txtRestockFee.Select(0, txtRestockFee.Text.Length);
                this.errorProvider1.SetError(txtRestockFee, "Negative values are not allowed for restocking fees");

            }

            if (!isNum)
            {
                e.Cancel = true;
                //MessageBox.Show("Please enter a numeric quantity", "Attention!");
                txtRestockFee.Select(0, txtRestockFee.Text.Length);
                this.errorProvider1.SetError(txtRestockFee, "Please enter a numeric quantity");

            }
            if (Num > Convert.ToDouble(txtUnitPrice.Text))
            {
                DialogResult dr = MessageBox.Show("Restocking fee is greater than the unit price for this item, are you sure?", "Attention!", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    txtRestockFee.Text = String.Format("{0:N}", Num);
                    return;
                }
                else
                {
                    e.Cancel = true;
                    return;
                }

            }
            else
            {
                txtRestockFee.Text = String.Format("{0:N}", Num);
            }
        }

        private void txtRestockFee_Validated(object sender, EventArgs e)
        {
            this.errorProvider1.SetError(txtRestockFee, "");
            if (Convert.ToDecimal(txtRestockFee.Text) > 0)
            {
                SqlCommand RestockChk;
                conn.Open();
                RestockChk = new SqlCommand("UPDATE tblInPRPReturnsDetail SET RestockingFeeYn = '1' WHERE ReturnAuthNum = @ReturnAuthNum AND ReturningItemId = @ReturningItemId", conn);

                RestockChk.Parameters.Add("@ReturnAuthNum", SqlDbType.VarChar, 8).Value = cboReturnAuthNum.Text;
                RestockChk.Parameters.Add("@ReturningItemId", SqlDbType.VarChar, 24).Value = cboReturnedItemID.Text;

                RestockChk.ExecuteNonQuery();
                conn.Close();
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (Program.strReportRaNum != String.Empty)
            {
                frmReports f = new frmReports();
                f.Show();
            }
            else
            {
                MessageBox.Show("Please select an RA number from the list.");
            }

        }

        private void returningItemIDTextBox_Validating(object sender, CancelEventArgs e)
        {
            // Validate ItemId here
            conn.Open();

            SqlCommand ValidItem = new SqlCommand("Select ItemId from lkpInItem where ItemId = @ItemId", conn);

            ValidItem.Parameters.Add("@ItemId", SqlDbType.VarChar, 24).Value = returningItemIDTextBox.Text;

            SqlDataReader readitem = null;
            readitem = ValidItem.ExecuteReader();

            string strValidItemId = String.Empty;
            if (readitem != null)
            {
                while (readitem.Read())
                {
                    strValidItemId = readitem["ItemId"].ToString();
                }
            }
            conn.Close();

            if (strValidItemId == String.Empty)
            {

                MessageBox.Show("The ItemId entered is invalid!", "Attention!");
                returningItemIDTextBox.Focus();

            }
        }

        private void returningItemIDTextBox_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(returningItemIDTextBox, "");
        }

        private void returningQtyTextBox_Validating(object sender, CancelEventArgs e)
        {
            string Str = returningQtyTextBox.Text.Trim();
            double Num;
            bool isNum = double.TryParse(Str, out Num);

            if (Str.StartsWith("-"))
            {
                e.Cancel = true;
                //MessageBox.Show("Negative quantities are not allowed for this trasaction type", "Attention!");
                returningQtyTextBox.Select(0, returningQtyTextBox.Text.Length);
                this.errorProvider1.SetError(returningQtyTextBox, "Negative quantities are not allowed for this trasaction type");
            }

            if (!isNum)
            {
                e.Cancel = true;
                //MessageBox.Show("Please enter a numeric quantity", "Attention!");
                returningQtyTextBox.Select(0, returningQtyTextBox.Text.Length);
                this.errorProvider1.SetError(returningQtyTextBox, "Please enter a numeric quantity");

            }
        }
        private void returningQtyTextBox_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(returningQtyTextBox, "");
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

            //Clear values 
            int rownum = dataGridView2.CurrentCell.RowIndex;

            //string strItem = dataGridView2.Rows[rownum].Cells[2].Value.ToString();

            dataGridView2.Rows[rownum].Cells[2].Value = String.Empty;
            dataGridView2.Rows[rownum].Cells[3].Value = 0;
            dataGridView2.Rows[rownum].Cells[4].Value = 0;
            dataGridView2.Rows[rownum].Cells[5].Value = String.Empty;
            dataGridView2.Rows[rownum].Cells[6].Value = 0;
            dataGridView2.Rows[rownum].Cells[7].Value = 0;
            dataGridView2.Rows[rownum].Cells[8].Value = 0;
            dataGridView2.Rows[rownum].Cells[9].Value = 0;
            dataGridView2.Rows[rownum].Cells[11].Value = String.Empty;


            //re-calculate subtotals
            string strSql = String.Empty;
            txtSubTotal.Text = this.tblInPRPReturnsDetailTableAdapter.SubTotal(cboReturnAuthNum.Text).ToString();

            conn.Open();

            strSql = String.Empty;

            strSql = "Update tblInPRPReturnsDetail Set SubTotal = @SubTotal where ReturnAuthNum = @ReturnAuthNum";
            SqlCommand UpdateSubTotal = new SqlCommand(strSql, conn);

            UpdateSubTotal.Parameters.Add("@SubTotal", SqlDbType.Decimal).Value = txtSubTotal.Text;
            UpdateSubTotal.Parameters.Add("@ReturnAuthNum", SqlDbType.VarChar, 8).Value = cboReturnAuthNum.Text;

            UpdateSubTotal.ExecuteNonQuery();
            conn.Close();

            this.tblInPRPReturnsDetailTableAdapter.FillBy(this.pRPDataSet1.tblInPRPReturnsDetail, cboReturnAuthNum.Text);


        }

        private void cboReturnAuthNum_Validated(object sender, EventArgs e)
        {
            Program.strReportRaNum = cboReturnAuthNum.Text;

        }

        private void txtRestockFee_Leave(object sender, EventArgs e)
        {
            if (txtRestockFee.Text == String.Empty)
            {
                DialogResult dr = MessageBox.Show("Restocking fee cannot be blank. Yes to calculate restocking fee to 30% of this items Total Price", "Attention!", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    double rstk = 0;

                    rstk = Convert.ToDouble(txtTotalPrice.Text) * .30;

                    txtRestockFee.Text = String.Format("{0:N}", rstk);
                    return;
                }
                else
                {
                    txtRestockFee.Focus();
                }
            }
        }
        /*
                private void tabPage2_Validating(object sender, CancelEventArgs e)
                {
                int cnt = 0;
                ListView.SelectedListViewItemCollection SelectedItems = this.lvwReturnCodes.SelectedItems;
                cnt = System.Convert.ToInt16(this.tblInPRPReturnsDetailTableAdapter.CountEntryNumbers(cboReturnAuthNum.Text).ToString());
       
                        if (cnt == 0) 
                        {
                        e.Cancel = true;
                        this.errorProvider1.SetError(returningItemIDTextBox, "Please add items to this return using the + sign.");
                        }
                        if (SelectedItems.Count == 0)
                        {
                            this.errorProvider1.SetError(lvwReturnCodes, "Please select a return code from the list.");
                        }
                }

                private void tabPage2_Validated(object sender, EventArgs e)
                {
            
                    this.errorProvider1.Clear();
                }
         */

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabControl1.TabPages[tabControl1.SelectedIndex].Focus();
            tabControl1.TabPages[tabControl1.SelectedIndex].CausesValidation = true;
        }

    }
}
