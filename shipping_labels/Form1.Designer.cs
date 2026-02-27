// Form1.Designer.cs
namespace shipping_labels
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblProductCode = new System.Windows.Forms.Label();
            this.txtProductCode = new System.Windows.Forms.TextBox();
            this.lblHeatId = new System.Windows.Forms.Label();
            this.txtHeatId = new System.Windows.Forms.TextBox();
            this.lblGross = new System.Windows.Forms.Label();
            this.txtGross = new System.Windows.Forms.TextBox();
            this.lblTare = new System.Windows.Forms.Label();
            this.txtTare = new System.Windows.Forms.TextBox();
            this.lblNet = new System.Windows.Forms.Label();
            this.txtNet = new System.Windows.Forms.TextBox();
            this.lblPrintProductId = new System.Windows.Forms.Label();
            this.txtPrintProductId = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();

            this.lblCopies = new System.Windows.Forms.Label();
            this.numCopies = new System.Windows.Forms.NumericUpDown();
            this.lblPrinterStatus = new System.Windows.Forms.Label();

            ((System.ComponentModel.ISupportInitialize)(this.numCopies)).BeginInit();
            this.SuspendLayout();
            // 
            // lblProductCode
            // 
            this.lblProductCode.AutoSize = true;
            this.lblProductCode.Location = new System.Drawing.Point(16, 16);
            this.lblProductCode.Name = "lblProductCode";
            this.lblProductCode.Size = new System.Drawing.Size(92, 13);
            this.lblProductCode.TabIndex = 0;
            this.lblProductCode.Text = "Product Code:";
            // 
            // txtProductCode
            // 
            this.txtProductCode.Location = new System.Drawing.Point(128, 13);
            this.txtProductCode.Name = "txtProductCode";
            this.txtProductCode.Size = new System.Drawing.Size(120, 20);
            this.txtProductCode.TabIndex = 1;
            this.txtProductCode.TextChanged += new System.EventHandler(this.txtProductCode_TextChanged);
            // 
            // lblHeatId
            // 
            this.lblHeatId.AutoSize = true;
            this.lblHeatId.Location = new System.Drawing.Point(16, 46);
            this.lblHeatId.Name = "lblHeatId";
            this.lblHeatId.Size = new System.Drawing.Size(42, 13);
            this.lblHeatId.TabIndex = 2;
            this.lblHeatId.Text = "Heat #:";
            // 
            // txtHeatId
            // 
            this.txtHeatId.Location = new System.Drawing.Point(128, 43);
            this.txtHeatId.Name = "txtHeatId";
            this.txtHeatId.Size = new System.Drawing.Size(240, 20);
            this.txtHeatId.TabIndex = 3;
            // 
            // lblGross
            // 
            this.lblGross.AutoSize = true;
            this.lblGross.Location = new System.Drawing.Point(16, 76);
            this.lblGross.Name = "lblGross";
            this.lblGross.Size = new System.Drawing.Size(39, 13);
            this.lblGross.TabIndex = 4;
            this.lblGross.Text = "Gross:";
            // 
            // txtGross
            // 
            this.txtGross.Location = new System.Drawing.Point(128, 73);
            this.txtGross.Name = "txtGross";
            this.txtGross.Size = new System.Drawing.Size(120, 20);
            this.txtGross.TabIndex = 5;
            this.txtGross.TextChanged += new System.EventHandler(this.txtGross_TextChanged);
            // 
            // lblTare
            // 
            this.lblTare.AutoSize = true;
            this.lblTare.Location = new System.Drawing.Point(16, 106);
            this.lblTare.Name = "lblTare";
            this.lblTare.Size = new System.Drawing.Size(32, 13);
            this.lblTare.TabIndex = 6;
            this.lblTare.Text = "Tare:";
            // 
            // txtTare
            // 
            this.txtTare.Location = new System.Drawing.Point(128, 103);
            this.txtTare.Name = "txtTare";
            this.txtTare.Size = new System.Drawing.Size(120, 20);
            this.txtTare.TabIndex = 7;
            this.txtTare.TextChanged += new System.EventHandler(this.txtTare_TextChanged);
            // 
            // lblNet
            // 
            this.lblNet.AutoSize = true;
            this.lblNet.Location = new System.Drawing.Point(16, 136);
            this.lblNet.Name = "lblNet";
            this.lblNet.Size = new System.Drawing.Size(27, 13);
            this.lblNet.TabIndex = 8;
            this.lblNet.Text = "Net:";
            // 
            // txtNet
            // 
            this.txtNet.Location = new System.Drawing.Point(128, 133);
            this.txtNet.Name = "txtNet";
            this.txtNet.Size = new System.Drawing.Size(120, 20);
            this.txtNet.TabIndex = 9;
            // 
            // lblPrintProductId
            // 
            this.lblPrintProductId.AutoSize = true;
            this.lblPrintProductId.Location = new System.Drawing.Point(16, 166);
            this.lblPrintProductId.Name = "lblPrintProductId";
            this.lblPrintProductId.Size = new System.Drawing.Size(86, 13);
            this.lblPrintProductId.TabIndex = 10;
            this.lblPrintProductId.Text = "Printed Product:";
            // 
            // txtPrintProductId
            // 
            this.txtPrintProductId.Location = new System.Drawing.Point(128, 163);
            this.txtPrintProductId.Name = "txtPrintProductId";
            this.txtPrintProductId.Size = new System.Drawing.Size(240, 20);
            this.txtPrintProductId.TabIndex = 11;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(16, 196);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(63, 13);
            this.lblDescription.TabIndex = 12;
            this.lblDescription.Text = "Description:";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(128, 193);
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(740, 290);
            this.txtDescription.TabIndex = 13;
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrint.Location = new System.Drawing.Point(128, 505);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(160, 44);
            this.btnPrint.TabIndex = 14;
            this.btnPrint.Text = "PRINT";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClear.Location = new System.Drawing.Point(300, 505);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(160, 44);
            this.btnClear.TabIndex = 15;
            this.btnClear.Text = "CLEAR";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(264, 16);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 200;
            // 
            // lblCopies
            // 
            this.lblCopies.AutoSize = true;
            this.lblCopies.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCopies.Location = new System.Drawing.Point(480, 518);
            this.lblCopies.Name = "lblCopies";
            this.lblCopies.Size = new System.Drawing.Size(47, 13);
            this.lblCopies.TabIndex = 16;
            this.lblCopies.Text = "Copies:";
            // 
            // numCopies
            // 
            this.numCopies.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numCopies.Location = new System.Drawing.Point(540, 515);
            this.numCopies.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            this.numCopies.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numCopies.Name = "numCopies";
            this.numCopies.Size = new System.Drawing.Size(60, 20);
            this.numCopies.TabIndex = 17;
            this.numCopies.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numCopies.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblPrinterStatus
            // 
            this.lblPrinterStatus.AutoSize = true;
            this.lblPrinterStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPrinterStatus.Location = new System.Drawing.Point(650, 16);
            this.lblPrinterStatus.Name = "lblPrinterStatus";
            this.lblPrinterStatus.Size = new System.Drawing.Size(123, 13);
            this.lblPrinterStatus.TabIndex = 18;
            this.lblPrinterStatus.Text = "Printer: NOT FOUND";
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.lblPrinterStatus);
            this.Controls.Add(this.numCopies);
            this.Controls.Add(this.lblCopies);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.txtPrintProductId);
            this.Controls.Add(this.lblPrintProductId);
            this.Controls.Add(this.txtNet);
            this.Controls.Add(this.lblNet);
            this.Controls.Add(this.txtTare);
            this.Controls.Add(this.lblTare);
            this.Controls.Add(this.txtGross);
            this.Controls.Add(this.lblGross);
            this.Controls.Add(this.txtHeatId);
            this.Controls.Add(this.lblHeatId);
            this.Controls.Add(this.txtProductCode);
            this.Controls.Add(this.lblProductCode);
            this.Name = "Form1";
            this.Text = "CMX Shipping Label Kiosk";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numCopies)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblProductCode;
        private System.Windows.Forms.TextBox txtProductCode;
        private System.Windows.Forms.Label lblHeatId;
        private System.Windows.Forms.TextBox txtHeatId;
        private System.Windows.Forms.Label lblGross;
        private System.Windows.Forms.TextBox txtGross;
        private System.Windows.Forms.Label lblTare;
        private System.Windows.Forms.TextBox txtTare;
        private System.Windows.Forms.Label lblNet;
        private System.Windows.Forms.TextBox txtNet;
        private System.Windows.Forms.Label lblPrintProductId;
        private System.Windows.Forms.TextBox txtPrintProductId;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblStatus;

        private System.Windows.Forms.Label lblCopies;
        private System.Windows.Forms.NumericUpDown numCopies;
        private System.Windows.Forms.Label lblPrinterStatus;
    }
}