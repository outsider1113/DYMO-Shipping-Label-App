using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace shipping_labels
{
    public partial class Form1 : Form
    {
        private sealed class ProductInfo
        {
            public string PrintedProductId;
            public string Description;
        }

        private readonly Dictionary<string, ProductInfo> _productsByCode =
            new Dictionary<string, ProductInfo>(StringComparer.OrdinalIgnoreCase);

        private readonly Timer _printerTimer = new Timer();
        private readonly ToolTip _printerToolTip = new ToolTip();

        private const int PrinterPollIntervalMs = 1500;
        private DateTime _lastHotkeyPrintUtc = DateTime.MinValue;
        private DateTime _lastHotkeyRestartUtc = DateTime.MinValue;
        private const int HotkeyDebouncePrintMs = 900;
        private const int HotkeyDebounceRestartMs = 1500;

        private bool _updatingProductCode;
        private bool _updatingHeatText;

        public Form1()
        {
            InitializeComponent();
            KeyPreview = true;

            Shown += Form1_Shown;
            Resize += Form1_Resize;

            ConfigureFields();
            ApplyModernUi();
            WireInputGuards();
            InitPrinterStatus();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            try { WindowState = FormWindowState.Maximized; } catch { }
            ApplyResponsiveSizing();

            try
            {
                txtProductCode.Focus();
                txtProductCode.SelectAll();
            }
            catch { }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            ApplyResponsiveSizing();
        }

        // ── Read copies value safely — defaults to 1 if blank/invalid ────────
        private int GetCopiesValue()
        {
            int val;
            if (int.TryParse((txtCopies.Text ?? "").Trim(), out val) && val >= 1 && val <= 99)
                return val;
            return 1;
        }

        private void ConfigureFields()
        {
            ConfigureSingleLineLook(txtProductCode);
            ConfigureSingleLineLook(txtHeatId);
            ConfigureSingleLineLook(txtGross);
            ConfigureSingleLineLook(txtTare);
            ConfigureSingleLineLook(txtNet);
            ConfigureSingleLineLook(txtPrintProductId);
            ConfigureSingleLineLook(txtDescription);

            txtNet.ReadOnly = true;
            txtNet.TabStop = false;
            txtNet.Cursor = Cursors.Default;
            txtNet.BackColor = SystemColors.Window;
            txtNet.TextAlign = HorizontalAlignment.Left;

            txtPrintProductId.ReadOnly = true;
            txtPrintProductId.TabStop = false;
            txtPrintProductId.Cursor = Cursors.Default;
            txtPrintProductId.BackColor = SystemColors.Window;
            txtPrintProductId.TextAlign = HorizontalAlignment.Left;

            txtDescription.ReadOnly = true;
            txtDescription.TabStop = false;
            txtDescription.Cursor = Cursors.Default;
            txtDescription.BackColor = Color.FromArgb(245, 245, 245);
            txtDescription.ForeColor = Color.FromArgb(35, 35, 35);
            txtDescription.TextAlign = HorizontalAlignment.Left;

            // txtCopies — digits only, 1-99
            txtCopies.Text = "1";
            txtCopies.TextAlign = HorizontalAlignment.Center;
            txtCopies.BorderStyle = BorderStyle.FixedSingle;

            txtProductCode.MaxLength = 20;
            txtHeatId.MaxLength = 16;
            txtGross.MaxLength = 20;
            txtTare.MaxLength = 20;

            lblStatus.Text = "";
            lblStatus.Padding = new Padding(14, 0, 14, 0);
            lblStatus.BackColor = Color.Gainsboro;
            lblStatus.ForeColor = Color.Black;
        }

        private static void ConfigureSingleLineLook(TextBox tb)
        {
            if (tb == null) return;
            tb.Multiline = true;
            tb.AcceptsReturn = false;
            tb.WordWrap = false;
            tb.ScrollBars = ScrollBars.None;
            tb.BorderStyle = BorderStyle.FixedSingle;
            tb.TextAlign = HorizontalAlignment.Left;
        }

        private void ApplyModernUi()
        {
            DoubleBuffered = true;
            BackColor = Color.FromArgb(238, 241, 246);
            Font = new Font("Segoe UI", 14f, FontStyle.Bold);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1180, 780);

            if (pnlCard != null)
                pnlCard.BackColor = Color.White;

            StyleButton(btnPrint, Color.FromArgb(30, 136, 229), Color.White);
            StyleButton(btnClear, Color.FromArgb(97, 97, 97), Color.White);
        }

        private void ApplyResponsiveSizing()
        {
            if (!IsHandleCreated) return;

            try
            {
                SuspendLayout();
                tlpRoot.SuspendLayout();
                tlpHeader.SuspendLayout();
                tlpShell.SuspendLayout();
                pnlCard.SuspendLayout();
                tlpCard.SuspendLayout();
                tlpForm.SuspendLayout();
                tlpActions.SuspendLayout();

                int clientW = Math.Max(ClientSize.Width, 1024);
                int clientH = Math.Max(ClientSize.Height, 700);

                int outerPadding = 12;
                int cardPadding = 14;

                tlpRoot.Padding = new Padding(outerPadding);
                pnlCard.Padding = new Padding(cardPadding);

                // ── Hard-reserved heights ─────────────────────────────────────────
                int statusBarH = 48;
                int headerH = 44;
                int actionBarH = 110;
                int cardVPad = cardPadding * 2;

                // Keep tlpCard row 1 in sync
                tlpCard.RowStyles[1].SizeType = SizeType.Absolute;
                tlpCard.RowStyles[1].Height = actionBarH;

                // ── Form rows get everything left ─────────────────────────────────
                int formAvail = clientH - statusBarH - headerH - actionBarH
                                - cardVPad - outerPadding * 2 - 24;
                formAvail = Math.Max(formAvail, 280);

                int rowH = formAvail / 7;
                int marginV = Math.Max(3, rowH / 18);
                int boxH = Math.Max(rowH - marginV * 2, 20);

                tlpForm.ColumnStyles[0].Width = 20f;
                tlpForm.ColumnStyles[1].Width = 80f;

                // ── Fonts ─────────────────────────────────────────────────────────
                float inputFontSize = ClampF(boxH * 0.42f, 12f, 9999f);
                float labelColPx = clientW * 0.20f - cardPadding - 8f;
                float labelByWidth = labelColPx / (11f * 0.60f);
                float labelFontSize = ClampF(Math.Min(labelByWidth, 18f), 10f, 18f);
                float btnFontSize = 15f;
                float copiesLblFont = 16f;
                float copiesTxtFont = ClampF(actionBarH * 0.45f, 14f, 32f);
                float titleFontSize = ClampF(headerH * 0.45f, 12f, 22f);
                float printerFontSize = ClampF(headerH * 0.25f, 9f, 14f);
                float statusFontSize = 11f;

                lblTitle.Font = new Font("Segoe UI", titleFontSize, FontStyle.Bold);
                lblPrinterStatus.Font = new Font("Segoe UI", printerFontSize, FontStyle.Bold);
                lblStatus.Font = new Font("Segoe UI", statusFontSize, FontStyle.Bold);

                Font labelFont = new Font("Segoe UI", labelFontSize, FontStyle.Bold);
                Font inputFont = new Font("Segoe UI", inputFontSize, FontStyle.Bold);
                Font btnFont = new Font("Segoe UI", btnFontSize, FontStyle.Bold);

                foreach (Label lbl in GetAllLabels(this))
                {
                    if (lbl == lblTitle || lbl == lblPrinterStatus ||
                        lbl == lblStatus || lbl == lblCopies) continue;
                    lbl.Font = labelFont;
                }

                // Apply inputFont to all textboxes EXCEPT txtCopies (handled separately)
                foreach (TextBox tb in GetAllTextBoxes(this))
                {
                    if (tb == txtCopies) continue;
                    tb.Font = inputFont;
                }

                btnPrint.Font = btnFont;
                btnClear.Font = btnFont;
                lblCopies.Font = new Font("Segoe UI", copiesLblFont, FontStyle.Bold);
                txtCopies.Font = new Font("Segoe UI", copiesTxtFont, FontStyle.Bold);

                // ── Textbox sizing ────────────────────────────────────────────────
                int fontLineH = (int)Math.Round(inputFontSize * 1.34f);
                int padTop = Math.Max(2, (boxH - fontLineH) / 2);

                foreach (TextBox tb in GetAllTextBoxes(this))
                {
                    if (tb == txtCopies) continue;
                    tb.Dock = DockStyle.None;
                    tb.Anchor = AnchorStyles.Left | AnchorStyles.Right
                                     | AnchorStyles.Top | AnchorStyles.Bottom;
                    tb.Margin = new Padding(0, marginV, 6, marginV);
                    tb.MinimumSize = new Size(0, boxH);
                    tb.Height = boxH;
                    tb.Padding = new Padding(4, padTop, 4, 0);
                }

                // ── Action bar ────────────────────────────────────────────────────
                int btnH = 80;
                int btnW = Math.Max(180, (int)Math.Round(clientW * 0.10f));
                int copiesW = Math.Max(300, (int)Math.Round(clientW * 0.15f));

                // Keep tlpActions column 4 Absolute width in sync
                tlpActions.ColumnStyles[4].SizeType = SizeType.Absolute;
                tlpActions.ColumnStyles[4].Width = copiesW;

                // Keep tlpActions row height in sync
                tlpActions.RowStyles[0].SizeType = SizeType.Absolute;
                tlpActions.RowStyles[0].Height = actionBarH;

                btnPrint.MinimumSize = new Size(btnW, btnH);
                btnPrint.Size = new Size(btnW, btnH);
                btnClear.MinimumSize = new Size(btnW, btnH);
                btnClear.Size = new Size(btnW, btnH);
                btnPrint.Margin = new Padding(0, 0, 14, 0);
                btnClear.Margin = new Padding(0);

                // txtCopies — sized explicitly, no layout engine games
                int copiesTxtH = btnH;   // same height as buttons
                int copiesPadTop = Math.Max(2, (copiesTxtH - (int)Math.Round(copiesTxtFont * 1.34f)) / 2);
                txtCopies.Dock = DockStyle.None;
                txtCopies.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                txtCopies.Width = copiesW;
                txtCopies.Height = copiesTxtH;
                txtCopies.MinimumSize = new Size(copiesW, copiesTxtH);
                txtCopies.Margin = new Padding(8, (actionBarH - copiesTxtH) / 2, 0, 0);
                txtCopies.Padding = new Padding(4, copiesPadTop, 4, 0);

                // Center "Copies" label vertically against the textbox
                lblCopies.Margin = new Padding(0, (actionBarH - 30) / 2, 12, 0);

                lblStatus.MinimumSize = new Size(0, statusBarH - 8);
                tlpHeader.Margin = new Padding(0, 0, 0, 6);
                tlpForm.Margin = new Padding(0, 0, 0, 6);
            }
            finally
            {
                tlpActions.ResumeLayout();
                tlpForm.ResumeLayout();
                tlpCard.ResumeLayout();
                pnlCard.ResumeLayout();
                tlpShell.ResumeLayout();
                tlpHeader.ResumeLayout();
                tlpRoot.ResumeLayout();
                ResumeLayout();
            }
        }

        private static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private static float ClampF(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private static IEnumerable<Label> GetAllLabels(Control root)
        {
            foreach (Control c in root.Controls)
            {
                Label lbl = c as Label;
                if (lbl != null) yield return lbl;
                foreach (Label child in GetAllLabels(c))
                    yield return child;
            }
        }

        private static IEnumerable<TextBox> GetAllTextBoxes(Control root)
        {
            foreach (Control c in root.Controls)
            {
                TextBox tb = c as TextBox;
                if (tb != null) yield return tb;
                foreach (TextBox child in GetAllTextBoxes(c))
                    yield return child;
            }
        }

        private void StyleButton(Button button, Color backColor, Color foreColor)
        {
            if (button == null) return;
            button.UseVisualStyleBackColor = false;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = backColor;
            button.ForeColor = foreColor;
            button.Cursor = Cursors.Hand;
        }

        private void WireInputGuards()
        {
            txtProductCode.KeyPress += DigitsOnly_KeyPress;
            txtHeatId.KeyPress += DigitsOnly_KeyPress;
            txtGross.KeyPress += DecimalInput_KeyPress;
            txtTare.KeyPress += DecimalInput_KeyPress;
            txtCopies.KeyPress += CopiesInput_KeyPress;

            txtProductCode.KeyDown += SingleLineInput_KeyDown;
            txtHeatId.KeyDown += SingleLineInput_KeyDown;
            txtGross.KeyDown += SingleLineInput_KeyDown;
            txtTare.KeyDown += SingleLineInput_KeyDown;
        }

        private void SingleLineInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            e.Handled = true;
            FocusAdjacentEditableTextBox(false);
        }

        private void DigitsOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;
            if (char.IsDigit(e.KeyChar)) return;
            e.Handled = true;
        }

        private void CopiesInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow digits only; block everything else except control chars
            if (char.IsControl(e.KeyChar)) return;
            if (char.IsDigit(e.KeyChar)) return;
            e.Handled = true;
        }

        private void DecimalInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox box = sender as TextBox;
            if (box == null) return;
            if (char.IsControl(e.KeyChar)) return;
            if (char.IsDigit(e.KeyChar)) return;
            if (e.KeyChar == '.')
            {
                if (box.Text.IndexOf('.') < 0) return;
            }
            e.Handled = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Keys keyCode = keyData & Keys.KeyCode;

            if (keyCode == Keys.F13) { ClearAllFields(); return true; }
            if (keyCode == Keys.F14) { HotkeyPrint(); return true; }
            if (keyCode == Keys.F15) { ClearFocusedEntry(); return true; }
            if (keyCode == Keys.F16) { FocusAdjacentEditableTextBox(true); return true; }
            if (keyCode == Keys.F17) { FocusAdjacentEditableTextBox(false); return true; }
            if (keyCode == Keys.F18) { HotkeyForceRestart(); return true; }
            if (keyCode == Keys.F19) { SetCopiesHotkey(6); return true; }
            if (keyCode == Keys.F20) { SetCopiesHotkey(1); return true; }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private bool IsDebounced(ref DateTime lastUtc, int debounceMs)
        {
            DateTime now = DateTime.UtcNow;
            if (lastUtc != DateTime.MinValue)
            {
                double ms = (now - lastUtc).TotalMilliseconds;
                if (ms >= 0 && ms < debounceMs) return true;
            }
            lastUtc = now;
            return false;
        }

        private void HotkeyPrint()
        {
            if (IsDebounced(ref _lastHotkeyPrintUtc, HotkeyDebouncePrintMs)) return;
            try
            {
                if (btnPrint != null && btnPrint.Enabled) btnPrint.PerformClick();
            }
            catch { }
        }

        private void SetCopiesHotkey(int copies)
        {
            try
            {
                txtCopies.Text = copies.ToString();
                ShowStatus("Copies set to " + copies, false);
            }
            catch { }
        }

        private void ClearFocusedEntry()
        {
            try
            {
                Control focused = GetFocusedControl(this);
                TextBoxBase tb = focused as TextBoxBase;
                if (tb != null)
                {
                    if (tb.ReadOnly) return;
                    if (tb == txtCopies)
                        tb.Text = "1";
                    else
                        tb.Text = "";
                    tb.Focus();
                }
            }
            catch { }
        }

        private void FocusAdjacentEditableTextBox(bool previous)
        {
            try
            {
                List<TextBox> boxes = GetEditableTextBoxesOrdered();
                if (boxes.Count == 0) return;

                Control focused = GetFocusedControl(this);
                TextBox current = focused as TextBox;
                int currentIndex = -1;

                if (current != null)
                {
                    for (int i = 0; i < boxes.Count; i++)
                    {
                        if (ReferenceEquals(boxes[i], current)) { currentIndex = i; break; }
                    }
                }

                int targetIndex;
                if (currentIndex < 0)
                    targetIndex = previous ? boxes.Count - 1 : 0;
                else
                    targetIndex = previous
                        ? Math.Max(0, currentIndex - 1)
                        : Math.Min(boxes.Count - 1, currentIndex + 1);

                boxes[targetIndex].Focus();
                boxes[targetIndex].SelectAll();
            }
            catch { }
        }

        private List<TextBox> GetEditableTextBoxesOrdered()
        {
            List<TextBox> list = new List<TextBox>();
            CollectTextBoxes(this, list);
            return list
                .Where(tb => tb != null && tb.Visible && tb.Enabled && !tb.ReadOnly)
                .OrderBy(tb => tb.TabIndex)
                .ThenBy(tb => tb.Top)
                .ThenBy(tb => tb.Left)
                .ToList();
        }

        private static void CollectTextBoxes(Control parent, List<TextBox> list)
        {
            foreach (Control c in parent.Controls)
            {
                TextBox tb = c as TextBox;
                if (tb != null) list.Add(tb);
                if (c.HasChildren) CollectTextBoxes(c, list);
            }
        }

        private static Control GetFocusedControl(Control control)
        {
            ContainerControl cc = control as ContainerControl;
            while (cc != null && cc.ActiveControl != null)
            {
                control = cc.ActiveControl;
                cc = control as ContainerControl;
            }
            return control;
        }

        private void HotkeyForceRestart()
        {
            if (IsDebounced(ref _lastHotkeyRestartUtc, HotkeyDebounceRestartMs)) return;
            try { ShowStatusPrinting("Restarting..."); } catch { }
            try
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    try { Application.Restart(); }
                    catch
                    {
                        try { Process.Start(Application.ExecutablePath); } catch { }
                        try { Environment.Exit(0); } catch { }
                    }
                });
            }
            catch
            {
                try { Process.Start(Application.ExecutablePath); } catch { }
                try { Environment.Exit(0); } catch { }
            }
        }

        private void InitPrinterStatus()
        {
            _printerTimer.Stop();
            _printerTimer.Interval = PrinterPollIntervalMs;
            _printerTimer.Tick -= PrinterTimer_Tick;
            _printerTimer.Tick += PrinterTimer_Tick;
            _printerTimer.Start();
            UpdatePrinterStatus();
        }

        private void PrinterTimer_Tick(object sender, EventArgs e)
        {
            UpdatePrinterStatus();
        }

        private void UpdatePrinterStatus()
        {
            string printerName;
            bool ok = TryGetAnyDymoPrinter(out printerName);
            if (lblPrinterStatus == null) return;

            if (ok)
            {
                string shown = string.IsNullOrWhiteSpace(printerName)
                    ? "OK"
                    : "OK (" + Ellipsize(printerName, 34) + ")";
                lblPrinterStatus.Text = "Printer: " + shown;
                lblPrinterStatus.ForeColor = Color.DarkGreen;
                try { _printerToolTip.SetToolTip(lblPrinterStatus, printerName ?? ""); } catch { }
            }
            else
            {
                lblPrinterStatus.Text = "Printer: NOT FOUND";
                lblPrinterStatus.ForeColor = Color.DarkRed;
                try { _printerToolTip.SetToolTip(lblPrinterStatus, "No DYMO printer detected"); } catch { }
            }
        }

        private static string Ellipsize(string value, int maxChars)
        {
            if (string.IsNullOrEmpty(value) || maxChars <= 0) return "";
            if (value.Length <= maxChars) return value;
            if (maxChars <= 3) return value.Substring(0, maxChars);
            return value.Substring(0, maxChars - 3) + "...";
        }

        private bool TryGetAnyDymoPrinter(out string printerName)
        {
            printerName = null;
            object addIn = null;
            try
            {
                addIn = CreateComFromProgIds(new[] { "Dymo.DymoAddIn", "DymoAddIn", "DYMO.DymoAddIn" });
                string printers = TryInvokeComString(addIn, "GetDymoPrinters");
                if (string.IsNullOrWhiteSpace(printers)) return false;

                List<string> parts = printers
                    .Split(new[] { '|', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => (s ?? "").Trim())
                    .Where(s => s.Length > 0)
                    .ToList();

                if (parts.Count == 0) return false;

                string preferred = parts.FirstOrDefault(p =>
                    p.IndexOf("4XL", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    p.IndexOf("LabelWriter 4XL", StringComparison.OrdinalIgnoreCase) >= 0);

                printerName = preferred ?? parts[0];
                return true;
            }
            catch { return false; }
            finally { ReleaseComObjectQuietly(addIn); }
        }

        private void ShowStatus(string message, bool isError)
        {
            if (lblStatus == null) return;
            lblStatus.Text = message ?? "";
            if (string.IsNullOrWhiteSpace(lblStatus.Text))
            {
                lblStatus.BackColor = Color.Gainsboro;
                lblStatus.ForeColor = Color.Black;
                return;
            }
            if (isError)
            {
                lblStatus.BackColor = Color.Firebrick;
                lblStatus.ForeColor = Color.White;
            }
            else
            {
                lblStatus.BackColor = Color.SeaGreen;
                lblStatus.ForeColor = Color.White;
            }
        }

        private void ShowStatusPrinting(string message)
        {
            if (lblStatus == null) return;
            lblStatus.Text = message ?? "";
            lblStatus.BackColor = Color.Goldenrod;
            lblStatus.ForeColor = Color.Black;
        }

        private static void ReleaseComObjectQuietly(object comObj)
        {
            try
            {
                if (comObj != null && Marshal.IsComObject(comObj))
                    Marshal.FinalReleaseComObject(comObj);
            }
            catch { }
        }

        private static string TryInvokeComString(object target, string methodName, params object[] args)
        {
            try
            {
                object result = InvokeCom(target, methodName, args);
                return result == null ? null : result.ToString();
            }
            catch { return null; }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                LoadProductsFromCsv();
                ApplyResponsiveSizing();
                ShowStatus("Loaded " + _productsByCode.Count + " products", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CSV Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ShowStatus("CSV load failed", true);
            }
        }

        private void txtProductCode_TextChanged(object sender, EventArgs e)
        {
            if (!_updatingProductCode)
            {
                string digits = NormalizeDigits(txtProductCode.Text);
                if (txtProductCode.Text != digits)
                {
                    _updatingProductCode = true;
                    int start = digits.Length;
                    txtProductCode.Text = digits;
                    txtProductCode.SelectionStart = Math.Min(start, txtProductCode.TextLength);
                    _updatingProductCode = false;
                }
            }

            string code = NormalizeDigits(txtProductCode.Text);
            if (string.IsNullOrWhiteSpace(code))
            {
                txtPrintProductId.Text = "";
                txtDescription.Text = "";
                ShowStatus("", false);
                return;
            }

            ProductInfo info;
            if (_productsByCode.TryGetValue(code, out info))
            {
                txtPrintProductId.Text = info.PrintedProductId ?? "";
                txtDescription.Text = (info.Description ?? "").Replace("\r", " ").Replace("\n", " ").Trim();
                ShowStatus("OK", false);
            }
            else
            {
                txtPrintProductId.Text = "";
                txtDescription.Text = "";
                ShowStatus("Product code not found", true);
            }
        }

        private void txtHeatId_TextChanged(object sender, EventArgs e)
        {
            ApplyHeatTextboxFormatting();
        }

        private void ApplyHeatTextboxFormatting()
        {
            if (_updatingHeatText) return;
            string digits = NormalizeDigits(txtHeatId.Text);
            string formatted = FormatHeatId(digits);
            if (txtHeatId.Text == formatted) return;
            _updatingHeatText = true;
            txtHeatId.Text = formatted;
            txtHeatId.SelectionStart = txtHeatId.TextLength;
            _updatingHeatText = false;
        }

        private static string FormatHeatId(string input)
        {
            string digits = NormalizeDigits(input);
            if (string.IsNullOrWhiteSpace(digits)) return "";
            if (digits.Length == 1) return digits;
            return digits.Substring(0, 1) + "-" + digits.Substring(1);
        }

        private string GetFormattedHeatForPrint()
        {
            return FormatHeatId(txtHeatId.Text);
        }

        private void txtGross_TextChanged(object sender, EventArgs e) { RecalcNet(); }
        private void txtTare_TextChanged(object sender, EventArgs e) { RecalcNet(); }

        private void RecalcNet()
        {
            decimal gross, tare;
            if (!TryParseDecimal(txtGross.Text, out gross) || !TryParseDecimal(txtTare.Text, out tare))
            {
                txtNet.Text = "";
                return;
            }
            decimal net = gross - tare;
            txtNet.Text = net.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private void btnClear_Click(object sender, EventArgs e) { ClearAllFields(); }
        private void btnPrint_Click(object sender, EventArgs e) { PrintLabelsFromUi(); }

        private void PrintLabelsFromUi()
        {
            object addIn = null;
            object labels = null;
            bool printJobStarted = false;

            try
            {
                int copies = GetCopiesValue();
                if (copies < 1) { ShowStatus("Copies must be at least 1.", true); return; }

                string code = NormalizeDigits(txtProductCode.Text);
                if (string.IsNullOrWhiteSpace(code))
                    throw new Exception("Enter a Product Code.");

                ProductInfo info;
                if (!_productsByCode.TryGetValue(code, out info))
                    throw new Exception("Product Code not found in productIDs.csv.");

                string heat = GetFormattedHeatForPrint();
                if (string.IsNullOrWhiteSpace(heat))
                    throw new Exception("Enter a Heat #.");

                decimal gross, tare;
                if (!TryParseDecimal(txtGross.Text, out gross))
                    throw new Exception("Gross must be a number.");
                if (!TryParseDecimal(txtTare.Text, out tare))
                    throw new Exception("Tare must be a number.");

                decimal net = gross - tare;

                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string labelPath = Path.Combine(baseDir, "template.label");
                if (!File.Exists(labelPath))
                    throw new FileNotFoundException("template.label not found next to EXE.", labelPath);

                addIn = CreateComFromProgIds(new[] { "Dymo.DymoAddIn", "DymoAddIn", "DYMO.DymoAddIn" });
                labels = CreateComFromProgIds(new[] { "Dymo.DymoLabels", "DymoLabels", "DYMO.DymoLabels" });

                string printerName;
                if (!TryGetAnyDymoPrinter(out printerName))
                    throw new Exception("No DYMO printer detected. Check USB/power and DYMO drivers.");

                if (!string.IsNullOrWhiteSpace(printerName))
                {
                    if (!TryInvokeCom(addIn, "SelectPrinter", printerName))
                        TryInvokeCom(addIn, "SelectPrinter2", printerName);
                }

                InvokeCom(addIn, "Open", labelPath);

                SetField(labels, "HEATID", heat);
                SetField(labels, "GROSS", gross.ToString("0.###", CultureInfo.InvariantCulture));
                SetField(labels, "TARE", tare.ToString("0.###", CultureInfo.InvariantCulture));
                SetField(labels, "NET", net.ToString("0.###", CultureInfo.InvariantCulture));
                SetField(labels, "PRODUCTID", (info.PrintedProductId ?? "").Trim());
                SetField(labels, "PRODUCTDESCRIPTION", (info.Description ?? "").Trim());

                ShowStatusPrinting("Printing " + copies + " label(s)...");

                TryInvokeCom(addIn, "StartPrintJob");
                printJobStarted = true;

                InvokeCom(addIn, "Print", copies, false);

                TryInvokeCom(addIn, "EndPrintJob");
                printJobStarted = false;

                ShowStatus("Printed " + copies + " label(s)", false);
            }
            catch (COMException ex)
            {
                ShowStatus("Printer error: check roll/feed and connection.", true);
                MessageBox.Show(ex.Message, "Printer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                ShowStatus(ex.Message, true);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (printJobStarted && addIn != null)
                    try { TryInvokeCom(addIn, "EndPrintJob"); } catch { }

                ReleaseComObjectQuietly(labels);
                ReleaseComObjectQuietly(addIn);
            }
        }

        private void ClearAllFields()
        {
            try
            {
                txtProductCode.Text = "";
                txtHeatId.Text = "";
                txtGross.Text = "";
                txtTare.Text = "";
                txtNet.Text = "";
                txtPrintProductId.Text = "";
                txtDescription.Text = "";
                txtCopies.Text = "1";
                ShowStatus("Cleared.", false);
                txtProductCode.Focus();
            }
            catch { }
        }

        private void LoadProductsFromCsv()
        {
            _productsByCode.Clear();
            string csvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "productIDs.csv");
            if (!File.Exists(csvPath))
                throw new FileNotFoundException("productIDs.csv not found next to EXE.", csvPath);

            using (StreamReader sr = new StreamReader(csvPath))
            {
                string headerLine = sr.ReadLine();
                if (headerLine == null) throw new Exception("productIDs.csv is empty.");

                List<string> headers = SplitCsvLine(headerLine).Select(h => (h ?? "").Trim()).ToList();
                int idxCode = 0;
                int idxName = IndexOfHeader(headers, "Product Name");
                int idxDesc = IndexOfHeader(headers, "Product Description");

                if (idxName < 0 || idxDesc < 0)
                    throw new Exception("CSV must contain headers: 'Product Name' and 'Product Description'.");

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    List<string> cols = SplitCsvLine(line);
                    if (cols.Count <= Math.Max(idxDesc, Math.Max(idxName, idxCode))) continue;
                    string code = NormalizeDigits(cols[idxCode]);
                    if (string.IsNullOrWhiteSpace(code)) continue;
                    _productsByCode[code] = new ProductInfo
                    {
                        PrintedProductId = (cols[idxName] ?? "").Trim(),
                        Description = (cols[idxDesc] ?? "").Trim()
                    };
                }
            }
        }

        private static int IndexOfHeader(List<string> headers, string name)
        {
            for (int i = 0; i < headers.Count; i++)
                if (string.Equals(headers[i], name, StringComparison.OrdinalIgnoreCase)) return i;
            return -1;
        }

        private static List<string> SplitCsvLine(string line)
        {
            List<string> result = new List<string>();
            bool inQuotes = false;
            string current = "";
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    { current += '"'; i++; }
                    else
                    { inQuotes = !inQuotes; }
                }
                else if (c == ',' && !inQuotes)
                { result.Add(current); current = ""; }
                else
                { current += c; }
            }
            result.Add(current);
            return result;
        }

        private static string NormalizeDigits(string input)
        {
            if (input == null) return "";
            return Regex.Replace(input.Trim(), @"\D", "");
        }

        private static bool TryParseDecimal(string value, out decimal parsed)
        {
            value = (value ?? "").Trim().Replace(",", "");
            return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out parsed);
        }

        private static object CreateComFromProgIds(string[] progIds)
        {
            foreach (string progId in progIds)
            {
                Type type = Type.GetTypeFromProgID(progId, false);
                if (type == null) continue;
                try
                {
                    object instance = Activator.CreateInstance(type);
                    if (instance != null) return instance;
                }
                catch { }
            }
            throw new Exception(
                "Could not create DYMO COM object. Tried:\n- " + string.Join("\n- ", progIds) +
                "\n\nDYMO Label Software components may not be registered on this PC.");
        }

        private static void SetField(object labels, string name, string value)
        {
            if (TryInvokeCom(labels, "SetField", name, value)) return;
            if (TryInvokeCom(labels, "SetObjectText", name, value)) return;
            if (TryInvokeCom(labels, "SetText", name, value)) return;
            throw new Exception("Could not set label field '" + name + "'.");
        }

        private static object InvokeCom(object target, string methodName, params object[] args)
        {
            return target.GetType().InvokeMember(
                methodName, BindingFlags.InvokeMethod, null, target, args);
        }

        private static bool TryInvokeCom(object target, string methodName, params object[] args)
        {
            try { InvokeCom(target, methodName, args); return true; }
            catch { return false; }
        }
    }
}