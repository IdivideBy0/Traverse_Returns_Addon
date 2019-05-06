namespace Traverse_Returns_Addon
{
    partial class frmReports
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            this.tblInPRPReturnsHistBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.PRPDataSet1 = new Traverse_Returns_Addon.PRPDataSet1();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.tblInPRPReturnsHistTableAdapter = new Traverse_Returns_Addon.PRPDataSet1TableAdapters.tblInPRPReturnsHistTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.tblInPRPReturnsHistBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PRPDataSet1)).BeginInit();
            this.SuspendLayout();
            // 
            // tblInPRPReturnsHistBindingSource
            // 
            this.tblInPRPReturnsHistBindingSource.DataMember = "tblInPRPReturnsHist";
            this.tblInPRPReturnsHistBindingSource.DataSource = this.PRPDataSet1;
            // 
            // PRPDataSet1
            // 
            this.PRPDataSet1.DataSetName = "PRPDataSet1";
            this.PRPDataSet1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "PRPDataSet1_tblInPRPReturnsHist";
            reportDataSource1.Value = this.tblInPRPReturnsHistBindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "Traverse_Returns_Addon.Report1.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(860, 648);
            this.reportViewer1.TabIndex = 0;
            // 
            // tblInPRPReturnsHistTableAdapter
            // 
            this.tblInPRPReturnsHistTableAdapter.ClearBeforeFill = true;
            // 
            // frmReports
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 648);
            this.Controls.Add(this.reportViewer1);
            this.Name = "frmReports";
            this.Text = "frmReports";
            this.Load += new System.EventHandler(this.frmReports_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tblInPRPReturnsHistBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PRPDataSet1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.BindingSource tblInPRPReturnsHistBindingSource;
        private PRPDataSet1 PRPDataSet1;
        private Traverse_Returns_Addon.PRPDataSet1TableAdapters.tblInPRPReturnsHistTableAdapter tblInPRPReturnsHistTableAdapter;
    }
}