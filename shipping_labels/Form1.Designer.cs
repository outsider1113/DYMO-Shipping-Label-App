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
            this.txtCopies = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.tlpRoot.SuspendLayout();
            this.tlpHeader.SuspendLayout();
            this.tlpShell.SuspendLayout();
            this.pnlCard.SuspendLayout();
            this.tlpCard.SuspendLayout();
            this.tlpForm.SuspendLayout();
            this.tlpActions.SuspendLayout();
            this.SuspendLayout();

            // ── tlpRoot ──────────────────────────────────────────────────
            this.tlpRoot.ColumnCount = 1;
            this.tlpRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRoot.Controls.Add(this.tlpHeader, 0, 0);
            this.tlpRoot.Controls.Add(this.tlpShell, 0, 1);
            this.tlpRoot.Controls.Add(this.lblStatus, 0, 2);
            this.tlpRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpRoot.Location = new System.Drawing.Point(0, 0);
            this.tlpRoot.Margin = new System.Windows.Forms.Padding(0);
            this.tlpRoot.Name = "tlpRoot";
            this.tlpRoot.Padding = new System.Windows.Forms.Padding(12);
            this.tlpRoot.RowCount = 3;
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tlpRoot.Size = new System.Drawing.Size(1440, 900);
            this.tlpRoot.TabIndex = 0;

            // ── tlpHeader ────────────────────────────────────────────────
            this.tlpHeader.AutoSize = true;
            this.tlpHeader.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpHeader.ColumnCount = 2;
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpHeader.Controls.Add(this.lblTitle, 0, 0);
            this.tlpHeader.Controls.Add(this.lblPrinterStatus, 1, 0);
            this.tlpHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpHeader.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.tlpHeader.Name = "tlpHeader";
            this.tlpHeader.RowCount = 1;
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpHeader.Size = new System.Drawing.Size(1416, 40);
            this.tlpHeader.TabIndex = 0;

            // ── lblTitle ─────────────────────────────────────────────────
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Text = "CMX SHIPPING LABEL KIOSK";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ── lblPrinterStatus ─────────────────────────────────────────
            this.lblPrinterStatus.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblPrinterStatus.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.lblPrinterStatus.Name = "lblPrinterStatus";
            this.lblPrinterStatus.Size = new System.Drawing.Size(260, 30);
            this.lblPrinterStatus.Text = "Printer: NOT FOUND";
            this.lblPrinterStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // ── tlpShell ─────────────────────────────────────────────────
            this.tlpShell.ColumnCount = 1;
            this.tlpShell.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpShell.Controls.Add(this.pnlCard, 0, 0);
            this.tlpShell.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpShell.Margin = new System.Windows.Forms.Padding(0);
            this.tlpShell.Name = "tlpShell";
            this.tlpShell.RowCount = 1;
            this.tlpShell.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpShell.TabIndex = 1;

            // ── pnlCard ──────────────────────────────────────────────────
            this.pnlCard.BackColor = System.Drawing.Color.White;
            this.pnlCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCard.Controls.Add(this.tlpCard);
            this.pnlCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCard.Margin = new System.Windows.Forms.Padding(0);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Padding = new System.Windows.Forms.Padding(16);
            this.pnlCard.TabIndex = 0;

            // ── tlpCard ──────────────────────────────────────────────────
            // Row 1 (actions) = Absolute 110 — never negotiated down by AutoSize
            this.tlpCard.ColumnCount = 1;
            this.tlpCard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCard.Controls.Add(this.tlpForm, 0, 0);
            this.tlpCard.Controls.Add(this.tlpActions, 0, 1);
            this.tlpCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpCard.Margin = new System.Windows.Forms.Padding(0);
            this.tlpCard.Name = "tlpCard";
            this.tlpCard.RowCount = 2;
            this.tlpCard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tlpCard.TabIndex = 0;

            // ── tlpForm ──────────────────────────────────────────────────
            this.tlpForm.ColumnCount = 2;
            this.tlpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18F));
            this.tlpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 82F));
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
            this.tlpForm.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.tlpForm.Name = "tlpForm";
            this.tlpForm.RowCount = 7;
            this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28F));
            this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28F));
            this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28F));
            this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28F));
            this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28F));
            this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28F));
            this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.32F));
            this.tlpForm.TabIndex = 0;

            // ── Labels ───────────────────────────────────────────────────
            this.lblProductCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblProductCode.Margin = new System.Windows.Forms.Padding(4, 0, 8, 0);
            this.lblProductCode.Name = "lblProductCode";
            this.lblProductCode.Text = "Product";
            this.lblProductCode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.lblHeatId.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeatId.Margin = new System.Windows.Forms.Padding(4, 0, 8, 0);
            this.lblHeatId.Name = "lblHeatId";
            this.lblHeatId.Text = "Heat #";
            this.lblHeatId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.lblGross.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblGross.Margin = new System.Windows.Forms.Padding(4, 0, 8, 0);
            this.lblGross.Name = "lblGross";
            this.lblGross.Text = "Gross";
            this.lblGross.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.lblTare.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTare.Margin = new System.Windows.Forms.Padding(4, 0, 8, 0);
            this.lblTare.Name = "lblTare";
            this.lblTare.Text = "Tare";
            this.lblTare.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.lblNet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNet.Margin = new System.Windows.Forms.Padding(4, 0, 8, 0);
            this.lblNet.Name = "lblNet";
            this.lblNet.Text = "Net";
            this.lblNet.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.lblPrintProductId.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPrintProductId.Margin = new System.Windows.Forms.Padding(4, 0, 8, 0);
            this.lblPrintProductId.Name = "lblPrintProductId";
            this.lblPrintProductId.Text = "Printed";
            this.lblPrintProductId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.lblDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDescription.Margin = new System.Windows.Forms.Padding(4, 0, 8, 0);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Text = "Description";
            this.lblDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ── TextBoxes ─────────────────────────────────────────────────
            this.txtProductCode.AcceptsReturn = false;
            this.txtProductCode.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.txtProductCode.Margin = new System.Windows.Forms.Padding(0, 8, 4, 8);
            this.txtProductCode.Multiline = true;
            this.txtProductCode.Name = "txtProductCode";
            this.txtProductCode.TabIndex = 1;
            this.txtProductCode.WordWrap = false;
            this.txtProductCode.TextChanged += new System.EventHandler(this.txtProductCode_TextChanged);

            this.txtHeatId.AcceptsReturn = false;
            this.txtHeatId.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.txtHeatId.Margin = new System.Windows.Forms.Padding(0, 8, 4, 8);
            this.txtHeatId.Multiline = true;
            this.txtHeatId.Name = "txtHeatId";
            this.txtHeatId.TabIndex = 3;
            this.txtHeatId.WordWrap = false;
            this.txtHeatId.TextChanged += new System.EventHandler(this.txtHeatId_TextChanged);

            this.txtGross.AcceptsReturn = false;
            this.txtGross.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.txtGross.Margin = new System.Windows.Forms.Padding(0, 8, 4, 8);
            this.txtGross.Multiline = true;
            this.txtGross.Name = "txtGross";
            this.txtGross.TabIndex = 5;
            this.txtGross.WordWrap = false;
            this.txtGross.TextChanged += new System.EventHandler(this.txtGross_TextChanged);

            this.txtTare.AcceptsReturn = false;
            this.txtTare.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.txtTare.Margin = new System.Windows.Forms.Padding(0, 8, 4, 8);
            this.txtTare.Multiline = true;
            this.txtTare.Name = "txtTare";
            this.txtTare.TabIndex = 7;
            this.txtTare.WordWrap = false;
            this.txtTare.TextChanged += new System.EventHandler(this.txtTare_TextChanged);

            this.txtNet.AcceptsReturn = false;
            this.txtNet.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.txtNet.Margin = new System.Windows.Forms.Padding(0, 8, 4, 8);
            this.txtNet.Multiline = true;
            this.txtNet.Name = "txtNet";
            this.txtNet.ReadOnly = true;
            this.txtNet.TabIndex = 9;
            this.txtNet.TabStop = false;
            this.txtNet.WordWrap = false;

            this.txtPrintProductId.AcceptsReturn = false;
            this.txtPrintProductId.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.txtPrintProductId.Margin = new System.Windows.Forms.Padding(0, 8, 4, 8);
            this.txtPrintProductId.Multiline = true;
            this.txtPrintProductId.Name = "txtPrintProductId";
            this.txtPrintProductId.ReadOnly = true;
            this.txtPrintProductId.TabIndex = 11;
            this.txtPrintProductId.TabStop = false;
            this.txtPrintProductId.WordWrap = false;

            this.txtDescription.AcceptsReturn = false;
            this.txtDescription.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.txtDescription.Margin = new System.Windows.Forms.Padding(0, 8, 4, 8);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ReadOnly = true;
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtDescription.TabIndex = 13;
            this.txtDescription.TabStop = false;
            this.txtDescription.WordWrap = false;

            // ── tlpActions ───────────────────────────────────────────────
            this.tlpActions.AutoSize = false;
            this.tlpActions.ColumnCount = 5;
            this.tlpActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 300F));
            this.tlpActions.Controls.Add(this.btnPrint, 0, 0);
            this.tlpActions.Controls.Add(this.btnClear, 1, 0);
            this.tlpActions.Controls.Add(this.lblCopies, 3, 0);
            this.tlpActions.Controls.Add(this.txtCopies, 4, 0);
            this.tlpActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpActions.Margin = new System.Windows.Forms.Padding(0);
            this.tlpActions.Name = "tlpActions";
            this.tlpActions.RowCount = 1;
            this.tlpActions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tlpActions.TabIndex = 1;

            // ── btnPrint ─────────────────────────────────────────────────
            this.btnPrint.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Top;
            this.btnPrint.Margin = new System.Windows.Forms.Padding(0, 0, 14, 0);
            this.btnPrint.MinimumSize = new System.Drawing.Size(180, 80);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(180, 80);
            this.btnPrint.TabIndex = 0;
            this.btnPrint.Text = "PRINT";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);

            // ── btnClear ─────────────────────────────────────────────────
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Top;
            this.btnClear.Margin = new System.Windows.Forms.Padding(0);
            this.btnClear.MinimumSize = new System.Drawing.Size(180, 80);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(180, 80);
            this.btnClear.TabIndex = 1;
            this.btnClear.Text = "CLEAR";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);

            // ── lblCopies ────────────────────────────────────────────────
            this.lblCopies.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblCopies.AutoSize = true;
            this.lblCopies.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.lblCopies.Name = "lblCopies";
            this.lblCopies.Text = "Copies";
            this.lblCopies.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // ── txtCopies ─────────────────────────────────────────────────
            this.txtCopies.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Top;
            this.txtCopies.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCopies.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.txtCopies.MaxLength = 2;
            this.txtCopies.Multiline = true;
            this.txtCopies.Name = "txtCopies";
            this.txtCopies.Size = new System.Drawing.Size(300, 80);
            this.txtCopies.TabIndex = 2;
            this.txtCopies.Text = "1";
            this.txtCopies.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtCopies.WordWrap = false;

            // ── lblStatus ────────────────────────────────────────────────
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatus.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ── Form1 ────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1440, 900);
            this.Controls.Add(this.tlpRoot);
            this.MinimumSize = new System.Drawing.Size(1024, 700);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CMX Shipping Label Kiosk";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tlpRoot.ResumeLayout(false);
            this.tlpHeader.ResumeLayout(false);
            this.tlpShell.ResumeLayout(false);
            this.pnlCard.ResumeLayout(false);
            this.tlpCard.ResumeLayout(false);
            this.tlpCard.PerformLayout();
            this.tlpForm.ResumeLayout(false);
            this.tlpActions.ResumeLayout(false);
            this.tlpActions.PerformLayout();
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
        private System.Windows.Forms.TableLayoutPanel tlpActions;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblCopies;
        private System.Windows.Forms.TextBox txtCopies;
        private System.Windows.Forms.Label lblStatus;
    }
}