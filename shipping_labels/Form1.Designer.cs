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
            this.tlpRoot = new System.Windows.Forms.TableLayoutPanel();
            this.tlpHeader = new System.Windows.Forms.TableLayoutPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblPrinterStatus = new System.Windows.Forms.Label();
            this.tlpShell = new System.Windows.Forms.TableLayoutPanel();
            this.pnlCard = new System.Windows.Forms.Panel();
            this.tlpCard = new System.Windows.Forms.TableLayoutPanel();
            this.tlpForm = new System.Windows.Forms.TableLayoutPanel();
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
            this.tlpActions = new System.Windows.Forms.TableLayoutPanel();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblCopies = new System.Windows.Forms.Label();
            this.numCopies = new System.Windows.Forms.NumericUpDown();
            this.lblStatus = new System.Windows.Forms.Label();
            this.tlpRoot.SuspendLayout();
            this.tlpHeader.SuspendLayout();
            this.tlpShell.SuspendLayout();
            this.pnlCard.SuspendLayout();
            this.tlpCard.SuspendLayout();
            this.tlpForm.SuspendLayout();
            this.tlpActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCopies)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpRoot
            // 
            this.tlpRoot.ColumnCount = 1;
            this.tlpRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRoot.Controls.Add(this.tlpHeader, 0, 0);
            this.tlpRoot.Controls.Add(this.tlpShell, 0, 1);
            this.tlpRoot.Controls.Add(this.lblStatus, 0, 2);
            this.tlpRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpRoot.Location = new System.Drawing.Point(0, 0);
            this.tlpRoot.Margin = new System.Windows.Forms.Padding(0);
            this.tlpRoot.Name = "tlpRoot";
            this.tlpRoot.Padding = new System.Windows.Forms.Padding(16);
            this.tlpRoot.RowCount = 3;
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpRoot.Size = new System.Drawing.Size(1280, 820);
            this.tlpRoot.TabIndex = 0;
            // 
            // tlpHeader
            // 
            this.tlpHeader.AutoSize = true;
            this.tlpHeader.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpHeader.ColumnCount = 2;
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpHeader.Controls.Add(this.lblTitle, 0, 0);
            this.tlpHeader.Controls.Add(this.lblPrinterStatus, 1, 0);
            this.tlpHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpHeader.Location = new System.Drawing.Point(16, 16);
            this.tlpHeader.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.tlpHeader.Name = "tlpHeader";
            this.tlpHeader.RowCount = 1;
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpHeader.Size = new System.Drawing.Size(1248, 48);
            this.tlpHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoEllipsis = true;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(1067, 48);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "CMX SHIPPING LABEL KIOSK";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPrinterStatus
            // 
            this.lblPrinterStatus.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblPrinterStatus.AutoEllipsis = true;
            this.lblPrinterStatus.Location = new System.Drawing.Point(1083, 8);
            this.lblPrinterStatus.Margin = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.lblPrinterStatus.Name = "lblPrinterStatus";
            this.lblPrinterStatus.Size = new System.Drawing.Size(165, 32);
            this.lblPrinterStatus.TabIndex = 1;
            this.lblPrinterStatus.Text = "Printer: NOT FOUND";
            this.lblPrinterStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tlpShell
            // 
            this.tlpShell.ColumnCount = 3;
            this.tlpShell.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8F));
            this.tlpShell.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 84F));
            this.tlpShell.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8F));
            this.tlpShell.Controls.Add(this.pnlCard, 1, 0);
            this.tlpShell.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpShell.Location = new System.Drawing.Point(16, 76);
            this.tlpShell.Margin = new System.Windows.Forms.Padding(0);
            this.tlpShell.Name = "tlpShell";
            this.tlpShell.RowCount = 1;
            this.tlpShell.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpShell.Size = new System.Drawing.Size(1248, 676);
            this.tlpShell.TabIndex = 1;
            // 
            // pnlCard
            // 
            this.pnlCard.BackColor = System.Drawing.Color.White;
            this.pnlCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCard.Controls.Add(this.tlpCard);
            this.pnlCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCard.Location = new System.Drawing.Point(99, 0);
            this.pnlCard.Margin = new System.Windows.Forms.Padding(0);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Padding = new System.Windows.Forms.Padding(24);
            this.pnlCard.Size = new System.Drawing.Size(1048, 676);
            this.pnlCard.TabIndex = 0;
            // 
            // tlpCard
            // 
            this.tlpCard.ColumnCount = 1;
            this.tlpCard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCard.Controls.Add(this.tlpForm, 0, 0);
            this.tlpCard.Controls.Add(this.tlpActions, 0, 1);
            this.tlpCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpCard.Location = new System.Drawing.Point(24, 24);
            this.tlpCard.Margin = new System.Windows.Forms.Padding(0);
            this.tlpCard.Name = "tlpCard";
            this.tlpCard.RowCount = 2;
            this.tlpCard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpCard.Size = new System.Drawing.Size(998, 626);
            this.tlpCard.TabIndex = 0;
            // 
            // tlpForm
            // 
            this.tlpForm.ColumnCount = 2;
            this.tlpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tlpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpForm.Controls.Add(this.lblProductCode, 0, 0);
            this.tlpForm.Controls.Add(this.txtProductCode, 1, 0);
            this.tlpForm.Controls.Add(this.lblHeatId, 0, 1);
            this.tlpForm.Controls.Add(this.txtHeatId, 1, 1);
            this.tlpForm.Controls.Add(this.lblGross, 0, 2);
            this.tlpForm.Controls.Add(this.txtGross, 1, 2);
            this.tlpForm.Controls.Add(this.lblTare, 0, 3);
            this.tlpForm.Controls.Add(this.txtTare, 1, 3);
            this.tlpForm.Controls.Add(this.lblNet, 0, 4);
            this.tlpForm.Controls.Add(this.txtNet, 1, 4);
            this.tlpForm.Controls.Add(this.lblPrintProductId, 0, 5);
            this.tlpForm.Controls.Add(this.txtPrintProductId, 1, 5);
            this.tlpForm.Controls.Add(this.lblDescription, 0, 6);
            this.tlpForm.Controls.Add(this.txtDescription, 1, 6);
            this.tlpForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpForm.Location = new System.Drawing.Point(0, 0);
            this.tlpForm.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.tlpForm.Name = "tlpForm";
            this.tlpForm.RowCount = 7;
            this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tlpForm.Size = new System.Drawing.Size(998, 530);
            this.tlpForm.TabIndex = 0;
            // 
            // lblProductCode
            // 
            this.lblProductCode.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblProductCode.AutoSize = true;
            this.lblProductCode.Location = new System.Drawing.Point(0, 32);
            this.lblProductCode.Margin = new System.Windows.Forms.Padding(0);
            this.lblProductCode.Name = "lblProductCode";
            this.lblProductCode.Size = new System.Drawing.Size(75, 13);
            this.lblProductCode.TabIndex = 0;
            this.lblProductCode.Text = "Product Code";
            // 
            // txtProductCode
            // 
            this.txtProductCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProductCode.Location = new System.Drawing.Point(250, 12);
            this.txtProductCode.Margin = new System.Windows.Forms.Padding(0, 12, 0, 12);
            this.txtProductCode.Name = "txtProductCode";
            this.txtProductCode.Size = new System.Drawing.Size(748, 20);
            this.txtProductCode.TabIndex = 1;
            this.txtProductCode.TextChanged += new System.EventHandler(this.txtProductCode_TextChanged);
            // 
            // lblHeatId
            // 
            this.lblHeatId.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblHeatId.AutoSize = true;
            this.lblHeatId.Location = new System.Drawing.Point(0, 111);
            this.lblHeatId.Margin = new System.Windows.Forms.Padding(0);
            this.lblHeatId.Name = "lblHeatId";
            this.lblHeatId.Size = new System.Drawing.Size(40, 13);
            this.lblHeatId.TabIndex = 2;
            this.lblHeatId.Text = "Heat #";
            // 
            // txtHeatId
            // 
            this.txtHeatId.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtHeatId.Location = new System.Drawing.Point(250, 91);
            this.txtHeatId.Margin = new System.Windows.Forms.Padding(0, 12, 0, 12);
            this.txtHeatId.Name = "txtHeatId";
            this.txtHeatId.Size = new System.Drawing.Size(748, 20);
            this.txtHeatId.TabIndex = 3;
            // 
            // lblGross
            // 
            this.lblGross.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblGross.AutoSize = true;
            this.lblGross.Location = new System.Drawing.Point(0, 190);
            this.lblGross.Margin = new System.Windows.Forms.Padding(0);
            this.lblGross.Name = "lblGross";
            this.lblGross.Size = new System.Drawing.Size(34, 13);
            this.lblGross.TabIndex = 4;
            this.lblGross.Text = "Gross";
            // 
            // txtGross
            // 
            this.txtGross.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtGross.Location = new System.Drawing.Point(250, 170);
            this.txtGross.Margin = new System.Windows.Forms.Padding(0, 12, 0, 12);
            this.txtGross.Name = "txtGross";
            this.txtGross.Size = new System.Drawing.Size(748, 20);
            this.txtGross.TabIndex = 5;
            this.txtGross.TextChanged += new System.EventHandler(this.txtGross_TextChanged);
            // 
            // lblTare
            // 
            this.lblTare.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTare.AutoSize = true;
            this.lblTare.Location = new System.Drawing.Point(0, 270);
            this.lblTare.Margin = new System.Windows.Forms.Padding(0);
            this.lblTare.Name = "lblTare";
            this.lblTare.Size = new System.Drawing.Size(28, 13);
            this.lblTare.TabIndex = 6;
            this.lblTare.Text = "Tare";
            // 
            // txtTare
            // 
            this.txtTare.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTare.Location = new System.Drawing.Point(250, 250);
            this.txtTare.Margin = new System.Windows.Forms.Padding(0, 12, 0, 12);
            this.txtTare.Name = "txtTare";
            this.txtTare.Size = new System.Drawing.Size(748, 20);
            this.txtTare.TabIndex = 7;
            this.txtTare.TextChanged += new System.EventHandler(this.txtTare_TextChanged);
            // 
            // lblNet
            // 
            this.lblNet.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblNet.AutoSize = true;
            this.lblNet.Location = new System.Drawing.Point(0, 349);
            this.lblNet.Margin = new System.Windows.Forms.Padding(0);
            this.lblNet.Name = "lblNet";
            this.lblNet.Size = new System.Drawing.Size(24, 13);
            this.lblNet.TabIndex = 8;
            this.lblNet.Text = "Net";
            // 
            // txtNet
            // 
            this.txtNet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNet.Location = new System.Drawing.Point(250, 329);
            this.txtNet.Margin = new System.Windows.Forms.Padding(0, 12, 0, 12);
            this.txtNet.Name = "txtNet";
            this.txtNet.ReadOnly = true;
            this.txtNet.Size = new System.Drawing.Size(748, 20);
            this.txtNet.TabIndex = 9;
            this.txtNet.TabStop = false;
            // 
            // lblPrintProductId
            // 
            this.lblPrintProductId.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblPrintProductId.AutoSize = true;
            this.lblPrintProductId.Location = new System.Drawing.Point(0, 429);
            this.lblPrintProductId.Margin = new System.Windows.Forms.Padding(0);
            this.lblPrintProductId.Name = "lblPrintProductId";
            this.lblPrintProductId.Size = new System.Drawing.Size(81, 13);
            this.lblPrintProductId.TabIndex = 10;
            this.lblPrintProductId.Text = "Printed Product";
            // 
            // txtPrintProductId
            // 
            this.txtPrintProductId.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPrintProductId.Location = new System.Drawing.Point(250, 409);
            this.txtPrintProductId.Margin = new System.Windows.Forms.Padding(0, 12, 0, 12);
            this.txtPrintProductId.Name = "txtPrintProductId";
            this.txtPrintProductId.ReadOnly = true;
            this.txtPrintProductId.Size = new System.Drawing.Size(748, 20);
            this.txtPrintProductId.TabIndex = 11;
            this.txtPrintProductId.TabStop = false;
            // 
            // lblDescription
            // 
            this.lblDescription.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(0, 503);
            this.lblDescription.Margin = new System.Windows.Forms.Padding(0);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(60, 13);
            this.lblDescription.TabIndex = 12;
            this.lblDescription.Text = "Description";
            // 
            // txtDescription
            // 
            this.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDescription.Location = new System.Drawing.Point(250, 484);
            this.txtDescription.Margin = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ReadOnly = true;
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtDescription.Size = new System.Drawing.Size(748, 20);
            this.txtDescription.TabIndex = 13;
            this.txtDescription.TabStop = false;
            // 
            // tlpActions
            // 
            this.tlpActions.AutoSize = true;
            this.tlpActions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpActions.ColumnCount = 5;
            this.tlpActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tlpActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tlpActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpActions.Controls.Add(this.btnPrint, 0, 0);
            this.tlpActions.Controls.Add(this.btnClear, 1, 0);
            this.tlpActions.Controls.Add(this.lblCopies, 3, 0);
            this.tlpActions.Controls.Add(this.numCopies, 4, 0);
            this.tlpActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpActions.Location = new System.Drawing.Point(0, 542);
            this.tlpActions.Margin = new System.Windows.Forms.Padding(0);
            this.tlpActions.Name = "tlpActions";
            this.tlpActions.RowCount = 1;
            this.tlpActions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpActions.Size = new System.Drawing.Size(998, 84);
            this.tlpActions.TabIndex = 1;
            // 
            // btnPrint
            // 
            this.btnPrint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPrint.Location = new System.Drawing.Point(0, 0);
            this.btnPrint.Margin = new System.Windows.Forms.Padding(0, 0, 16, 0);
            this.btnPrint.MinimumSize = new System.Drawing.Size(180, 72);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(277, 84);
            this.btnPrint.TabIndex = 0;
            this.btnPrint.Text = "PRINT";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnClear
            // 
            this.btnClear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClear.Location = new System.Drawing.Point(293, 0);
            this.btnClear.Margin = new System.Windows.Forms.Padding(0, 0, 16, 0);
            this.btnClear.MinimumSize = new System.Drawing.Size(180, 72);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(277, 84);
            this.btnClear.TabIndex = 1;
            this.btnClear.Text = "CLEAR";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblCopies
            // 
            this.lblCopies.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblCopies.AutoSize = true;
            this.lblCopies.Location = new System.Drawing.Point(780, 35);
            this.lblCopies.Margin = new System.Windows.Forms.Padding(0, 0, 16, 0);
            this.lblCopies.Name = "lblCopies";
            this.lblCopies.Size = new System.Drawing.Size(39, 13);
            this.lblCopies.TabIndex = 2;
            this.lblCopies.Text = "Copies";
            this.lblCopies.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numCopies
            // 
            this.numCopies.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numCopies.Location = new System.Drawing.Point(835, 32);
            this.numCopies.Margin = new System.Windows.Forms.Padding(0);
            this.numCopies.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numCopies.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCopies.Name = "numCopies";
            this.numCopies.Size = new System.Drawing.Size(130, 20);
            this.numCopies.TabIndex = 3;
            this.numCopies.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numCopies.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatus.Location = new System.Drawing.Point(16, 764);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.lblStatus.MinimumSize = new System.Drawing.Size(0, 40);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(1248, 40);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1280, 820);
            this.Controls.Add(this.tlpRoot);
            this.MinimumSize = new System.Drawing.Size(980, 700);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CMX Shipping Label Kiosk";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tlpRoot.ResumeLayout(false);
            this.tlpRoot.PerformLayout();
            this.tlpHeader.ResumeLayout(false);
            this.tlpShell.ResumeLayout(false);
            this.pnlCard.ResumeLayout(false);
            this.tlpCard.ResumeLayout(false);
            this.tlpCard.PerformLayout();
            this.tlpForm.ResumeLayout(false);
            this.tlpForm.PerformLayout();
            this.tlpActions.ResumeLayout(false);
            this.tlpActions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCopies)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpRoot;
        private System.Windows.Forms.TableLayoutPanel tlpHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblPrinterStatus;
        private System.Windows.Forms.TableLayoutPanel tlpShell;
        private System.Windows.Forms.Panel pnlCard;
        private System.Windows.Forms.TableLayoutPanel tlpCard;
        private System.Windows.Forms.TableLayoutPanel tlpForm;
        private System.Windows.Forms.TableLayoutPanel tlpActions;
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
        private System.Windows.Forms.Label lblCopies;
        private System.Windows.Forms.NumericUpDown numCopies;
        private System.Windows.Forms.Label lblStatus;
    }
}