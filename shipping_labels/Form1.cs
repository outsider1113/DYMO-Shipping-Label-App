// Form1.cs
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
            try { txtProductCode.Focus(); txtProductCode.SelectAll(); } catch { }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            ApplyResponsiveSizing();
        }

        private void ConfigureFields()
        {
            txtNet.ReadOnly = true;
            txtNet.TabStop = false;
            txtNet.Cursor = Cursors.Default;
            txtNet.BorderStyle = BorderStyle.FixedSingle;
            txtNet.BackColor = SystemColors.Window;
            txtNet.TextAlign = HorizontalAlignment.Right;

            txtPrintProductId.ReadOnly = true;
            txtPrintProductId.TabStop = false;
            txtPrintProductId.Cursor = Cursors.Default;
            txtPrintProductId.BorderStyle = BorderStyle.FixedSingle;
            txtPrintProductId.BackColor = SystemColors.Window;

            txtDescription.ReadOnly = true;
            txtDescription.TabStop = false;
            txtDescription.Multiline = false;
            txtDescription.WordWrap = false;
            txtDescription.ScrollBars = ScrollBars.None;
            txtDescription.Cursor = Cursors.Default;
            txtDescription.BorderStyle = BorderStyle.FixedSingle;
            txtDescription.BackColor = Color.FromArgb(245, 245, 245);
            txtDescription.ForeColor = Color.Black;

            numCopies.Minimum = 1;
            numCopies.Maximum = 99;
            if (numCopies.Value < 1 || numCopies.Value > 99) numCopies.Value = 1;
            numCopies.TextAlign = HorizontalAlignment.Right;

            lblStatus.Text = "";
            lblStatus.Padding = new Padding(14, 0, 14, 0);
            lblStatus.BackColor = Color.Gainsboro;
            lblStatus.ForeColor = Color.Black;
        }

        private void ApplyModernUi()
        {
            DoubleBuffered = true;
            BackColor = Color.FromArgb(240, 243, 248);
            Font = new Font("Segoe UI", 12f, FontStyle.Regular);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(980, 700);

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

                Rectangle working = Screen.FromControl(this).WorkingArea;
                float dpiScale = Math.Max(1f, DeviceDpi / 96f);

                int workW = Math.Max(working.Width, 1100);
                int workH = Math.Max(working.Height, 760);

                int outerPadding = Clamp((int)Math.Round(workW * 0.012f), 12, 26);
                int cardPadding = Clamp((int)Math.Round(workW * 0.018f), 18, 34);
                int headerGap = Clamp((int)Math.Round(workH * 0.012f), 10, 18);

                tlpRoot.Padding = new Padding(outerPadding);
                tlpHeader.Margin = new Padding(0, 0, 0, headerGap);
                pnlCard.Padding = new Padding(cardPadding);

                float titleFontSize = ClampF(workH / 36f, 18f, 28f);
                float headerStatusFontSize = ClampF(workH / 62f, 11f, 16f);
                float labelFontSize = ClampF(workH / 58f, 12f, 18f);
                float inputFontSize = ClampF(workH / 44f, 15f, 24f);
                float buttonFontSize = ClampF(workH / 46f, 15f, 22f);
                float copiesFontSize = ClampF(workH / 54f, 13f, 19f);
                float statusFontSize = ClampF(workH / 64f, 12f, 17f);

                Font baseFont = new Font("Segoe UI", labelFontSize, FontStyle.Regular);
                Font labelFont = new Font("Segoe UI", labelFontSize, FontStyle.Bold);
                Font titleFont = new Font("Segoe UI", titleFontSize, FontStyle.Bold);
                Font printerFont = new Font("Segoe UI", headerStatusFontSize, FontStyle.Bold);
                Font inputFont = new Font("Segoe UI", inputFontSize, FontStyle.Regular);
                Font calcFont = new Font("Segoe UI", inputFontSize + 1f, FontStyle.Bold);
                Font descFont = new Font("Segoe UI", Math.Max(12f, inputFontSize - 2f), FontStyle.Italic);
                Font buttonFont = new Font("Segoe UI", buttonFontSize, FontStyle.Bold);
                Font statusFont = new Font("Segoe UI", statusFontSize, FontStyle.Bold);
                Font copiesFont = new Font("Segoe UI", copiesFontSize, FontStyle.Bold);
                Font copiesValueFont = new Font("Segoe UI", copiesFontSize, FontStyle.Regular);

                Font = baseFont;
                lblTitle.Font = titleFont;
                lblPrinterStatus.Font = printerFont;
                lblStatus.Font = statusFont;

                foreach (Label lbl in GetAllLabels(this))
                {
                    if (lbl == lblTitle || lbl == lblPrinterStatus || lbl == lblStatus) continue;
                    lbl.Font = labelFont;
                }

                foreach (TextBox tb in GetAllTextBoxes(this))
                {
                    tb.Font = inputFont;
                }

                txtNet.Font = calcFont;
                txtPrintProductId.Font = calcFont;
                txtDescription.Font = descFont;
                txtDescription.ForeColor = Color.FromArgb(60, 60, 60);

                btnPrint.Font = buttonFont;
                btnClear.Font = buttonFont;
                lblCopies.Font = copiesFont;
                numCopies.Font = copiesValueFont;

                int cardPreferredWidth = Clamp((int)Math.Round(workW * 0.82f), 930, 1450);
                int sidePercent = 8;
                if (workW < 1350) sidePercent = 4;
                if (workW < 1180) sidePercent = 2;

                tlpShell.ColumnStyles[0].Width = sidePercent;
                tlpShell.ColumnStyles[1].Width = 100 - (sidePercent * 2);
                tlpShell.ColumnStyles[2].Width = sidePercent;

                int labelWidth = Clamp((int)Math.Round(cardPreferredWidth * 0.28f), 220, 340);
                tlpForm.ColumnStyles[0].Width = labelWidth;

                int usableHeight = Math.Max(440, pnlCard.ClientSize.Height - 10);
                int actionsHeight = Clamp((int)Math.Round(usableHeight * 0.15f), 84, 140);
                int formHeight = Math.Max(300, usableHeight - actionsHeight - 12);

                int mainRow = Clamp(formHeight / 6, 58, 95);
                int descRow = Clamp(formHeight - (mainRow * 6), 42, 74);

                for (int i = 0; i < 6; i++)
                {
                    tlpForm.RowStyles[i].SizeType = SizeType.Absolute;
                    tlpForm.RowStyles[i].Height = mainRow;
                }

                tlpForm.RowStyles[6].SizeType = SizeType.Absolute;
                tlpForm.RowStyles[6].Height = descRow;

                tlpCard.RowStyles[0].SizeType = SizeType.Absolute;
                tlpCard.RowStyles[0].Height = mainRow * 6 + descRow + 12;
                tlpCard.RowStyles[1].SizeType = SizeType.Absolute;
                tlpCard.RowStyles[1].Height = actionsHeight;

                int textHeight = Clamp((int)Math.Round(mainRow * 0.58f), 36, 62);
                int descHeight = Clamp((int)Math.Round(descRow * 0.52f), 30, 50);
                int buttonHeight = Clamp((int)Math.Round(actionsHeight * 0.78f), 72, 118);
                int copiesWidth = Clamp((int)Math.Round(cardPreferredWidth * 0.16f), 120, 180);
                int printerWidth = Clamp((int)Math.Round(cardPreferredWidth * 0.22f), 165, 260);
                int fieldMarginV = Clamp((mainRow - textHeight) / 2, 8, 18);

                lblPrinterStatus.Width = printerWidth;

                foreach (TextBox tb in GetAllTextBoxes(this))
                {
                    int h = tb == txtDescription ? descHeight : textHeight;
                    tb.MinimumSize = new Size(0, h);
                    tb.Margin = new Padding(0, fieldMarginV, 0, fieldMarginV);
                }

                txtProductCode.MaxLength = 20;
                txtHeatId.MaxLength = 50;
                txtGross.MaxLength = 20;
                txtTare.MaxLength = 20;

                btnPrint.MinimumSize = new Size(190, buttonHeight);
                btnClear.MinimumSize = new Size(190, buttonHeight);

                numCopies.MinimumSize = new Size(copiesWidth, textHeight);
                numCopies.Width = copiesWidth;
                lblCopies.Margin = new Padding(0, 0, 16, 0);

                tlpActions.ColumnStyles[0].Width = 30F;
                tlpActions.ColumnStyles[1].Width = 30F;
                tlpActions.ColumnStyles[2].Width = 20F;
                tlpActions.ColumnStyles[4].Width = 20F;

                lblStatus.MinimumSize = new Size(0, Clamp((int)Math.Round(textHeight * 1.15f), 40, 62));
                lblStatus.Padding = new Padding(14, 0, 14, 0);

                tlpForm.Margin = new Padding(0, 0, 0, 12);
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
            txtGross.KeyPress += DecimalInput_KeyPress;
            txtTare.KeyPress += DecimalInput_KeyPress;
        }

        private void DigitsOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
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
                decimal value = copies;
                if (value < numCopies.Minimum) value = numCopies.Minimum;
                if (value > numCopies.Maximum) value = numCopies.Maximum;
                numCopies.Value = value;
                ShowStatus("Copies set to " + (int)numCopies.Value, false);
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
                    tb.Text = "";
                    tb.Focus();
                    return;
                }

                NumericUpDown nud = focused as NumericUpDown;
                if (nud != null)
                {
                    nud.Value = nud.Minimum;
                    nud.Focus();
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
                        if (ReferenceEquals(boxes[i], current))
                        {
                            currentIndex = i;
                            break;
                        }
                    }
                }

                int targetIndex;
                if (currentIndex < 0)
                {
                    targetIndex = previous ? boxes.Count - 1 : 0;
                }
                else
                {
                    targetIndex = previous
                        ? Math.Max(0, currentIndex - 1)
                        : Math.Min(boxes.Count - 1, currentIndex + 1);
                }

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
                    try
                    {
                        Application.Restart();
                    }
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
            catch
            {
                return false;
            }
            finally
            {
                ReleaseComObjectQuietly(addIn);
            }
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
            catch
            {
                return null;
            }
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
            string code = NormalizeCode(txtProductCode.Text);

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

        private void txtGross_TextChanged(object sender, EventArgs e)
        {
            RecalcNet();
        }

        private void txtTare_TextChanged(object sender, EventArgs e)
        {
            RecalcNet();
        }

        private void RecalcNet()
        {
            decimal gross;
            decimal tare;

            if (!TryParseDecimal(txtGross.Text, out gross) || !TryParseDecimal(txtTare.Text, out tare))
            {
                txtNet.Text = "";
                return;
            }

            decimal net = gross - tare;
            txtNet.Text = net.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintLabelsFromUi();
        }

        private void PrintLabelsFromUi()
        {
            object addIn = null;
            object labels = null;
            bool printJobStarted = false;

            try
            {
                int copies = (numCopies != null) ? (int)numCopies.Value : 1;
                if (copies < 1)
                {
                    ShowStatus("Copies must be at least 1.", true);
                    return;
                }

                string code = NormalizeCode(txtProductCode.Text);
                if (string.IsNullOrWhiteSpace(code))
                    throw new Exception("Enter a Product Code.");

                ProductInfo info;
                if (!_productsByCode.TryGetValue(code, out info))
                    throw new Exception("Product Code not found in productIDs.csv.");

                string heat = (txtHeatId.Text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(heat))
                    throw new Exception("Enter a Heat #.");

                decimal gross;
                decimal tare;

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

                txtGross.Text = "";
                txtTare.Text = "";
                txtNet.Text = "";
                txtGross.Focus();

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
                {
                    try { TryInvokeCom(addIn, "EndPrintJob"); } catch { }
                }

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

                if (numCopies != null) numCopies.Value = 1;

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

                    string code = NormalizeCode(cols[idxCode]);
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
            {
                if (string.Equals(headers[i], name, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

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
                    {
                        current += '"';
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current);
                    current = "";
                }
                else
                {
                    current += c;
                }
            }

            result.Add(current);
            return result;
        }

        private static string NormalizeCode(string input)
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
                methodName,
                BindingFlags.InvokeMethod,
                null,
                target,
                args);
        }

        private static bool TryInvokeCom(object target, string methodName, params object[] args)
        {
            try
            {
                InvokeCom(target, methodName, args);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}