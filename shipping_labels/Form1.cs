// Form1.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Diagnostics;
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
            public string PrintedProductId; // CSV "Product Name" (long ID)
            public string Description;      // CSV "Product Description"
        }

        private readonly Dictionary<string, ProductInfo> _productsByCode =
            new Dictionary<string, ProductInfo>(StringComparer.OrdinalIgnoreCase);

        private readonly System.Windows.Forms.Timer _printerTimer = new System.Windows.Forms.Timer();
        private const int PrinterPollIntervalMs = 1500;
        private readonly ToolTip _printerToolTip = new ToolTip();

        private DateTime _lastHotkeyPrintUtc = DateTime.MinValue;
        private DateTime _lastHotkeyRestartUtc = DateTime.MinValue;
        private const int HotkeyDebouncePrintMs = 900;
        private const int HotkeyDebounceRestartMs = 1500;

        // -------- Responsive layout baseline --------
        private bool _layoutBaselineCaptured = false;
        private bool _applyingResponsiveLayout = false;

        // DESIGN baseline (must match Designer ClientSize)
        private readonly Size _designClientSize = new Size(1000, 700);

        private readonly Dictionary<Control, Rectangle> _baseBounds = new Dictionary<Control, Rectangle>();
        private readonly Dictionary<Control, float> _baseFontSize = new Dictionary<Control, float>();
        private readonly Dictionary<Control, bool> _baseLabelAutoSize = new Dictionary<Control, bool>();

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;

            this.Shown += Form1_Shown;
            this.SizeChanged += Form1_SizeChanged;

            // Net styling
            txtNet.ReadOnly = true;
            txtNet.TabStop = false;
            txtNet.Cursor = Cursors.Default;
            txtNet.BorderStyle = BorderStyle.FixedSingle;
            txtNet.BackColor = SystemColors.Window;
            txtNet.TextAlign = HorizontalAlignment.Right;

            // Multiline so Height is respected
            txtNet.Multiline = true;
            txtNet.Width = 240;
            txtNet.Height = 32;
            txtNet.Font = new Font(txtNet.Font.FontFamily, 16f, FontStyle.Bold);

            txtPrintProductId.ReadOnly = true;

            // Description: simple 1-line grey read-only field
            if (txtDescription != null)
            {
                txtDescription.ReadOnly = true;
                txtDescription.TabStop = false;
                txtDescription.Multiline = false;
                txtDescription.WordWrap = false;
                txtDescription.ScrollBars = ScrollBars.None;
                txtDescription.BackColor = SystemColors.ControlLight;
                txtDescription.ForeColor = Color.Black;
                txtDescription.BorderStyle = BorderStyle.FixedSingle;
            }

            ApplyModernUi();

            if (numCopies != null)
            {
                numCopies.Minimum = 1;
                numCopies.Maximum = 99;
                if (numCopies.Value < 1 || numCopies.Value > 99) numCopies.Value = 1;
                numCopies.TextAlign = HorizontalAlignment.Right;
            }

            if (lblStatus != null)
            {
                lblStatus.AutoSize = false;
                lblStatus.Dock = DockStyle.Bottom;
                lblStatus.Height = 28;
                lblStatus.TextAlign = ContentAlignment.MiddleLeft;
                lblStatus.Padding = new Padding(10, 0, 10, 0);
                lblStatus.BackColor = Color.Gainsboro;
                lblStatus.ForeColor = Color.Black;
                lblStatus.Text = "";
            }

            InitPrinterStatus();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            CaptureResponsiveBaseline();

            try { this.WindowState = FormWindowState.Maximized; } catch { }

            // Apply after window state actually changes
            try { BeginInvoke((MethodInvoker)ApplyResponsiveLayout); }
            catch { ApplyResponsiveLayout(); }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            ApplyResponsiveLayout();
        }

        private void CaptureResponsiveBaseline()
        {
            if (_layoutBaselineCaptured) return;
            _layoutBaselineCaptured = true;

            _baseBounds.Clear();
            _baseFontSize.Clear();
            _baseLabelAutoSize.Clear();

            foreach (Control c in this.Controls)
            {
                if (c.Dock != DockStyle.None) continue;

                _baseBounds[c] = c.Bounds;
                if (c.Font != null) _baseFontSize[c] = c.Font.Size;

                var lbl = c as Label;
                if (lbl != null) _baseLabelAutoSize[c] = lbl.AutoSize;
            }
        }

        private void ApplyResponsiveLayout()
        {
            if (!_layoutBaselineCaptured) return;
            if (_applyingResponsiveLayout) return;
            if (this.ClientSize.Width <= 0 || this.ClientSize.Height <= 0) return;

            _applyingResponsiveLayout = true;

            try
            {
                float sx = (float)this.ClientSize.Width / Math.Max(1, _designClientSize.Width);
                float sy = (float)this.ClientSize.Height / Math.Max(1, _designClientSize.Height);
                float boundsScale = Math.Min(sx, sy);

                if (boundsScale > 1.60f) boundsScale = 1.60f;
                if (boundsScale < 0.85f) boundsScale = 0.85f;

                float dpi = 96f;
                try { dpi = this.DeviceDpi; }
                catch
                {
                    try { using (var g = this.CreateGraphics()) dpi = g.DpiX; } catch { }
                }
                float dpiFactor = dpi / 96f;

                float dpiBoost =
                    (dpiFactor <= 1.05f) ? 1.20f :
                    (dpiFactor <= 1.25f) ? 1.08f :
                    1.00f;

                float screenBoost = 1.00f;
                try
                {
                    var wa = Screen.FromControl(this).WorkingArea;
                    if (wa.Width <= 1600 || wa.Height <= 900) screenBoost = 1.12f;
                }
                catch { }

                float fontScale = boundsScale * dpiBoost * screenBoost;
                if (fontScale > 1.55f) fontScale = 1.55f;
                if (fontScale < 0.90f) fontScale = 0.90f;

                this.SuspendLayout();

                foreach (Control c in this.Controls)
                {
                    if (c.Dock != DockStyle.None) continue;

                    Rectangle baseRect;
                    if (!_baseBounds.TryGetValue(c, out baseRect)) continue;

                    int newX = (int)Math.Round(baseRect.X * boundsScale);
                    int newY = (int)Math.Round(baseRect.Y * boundsScale);
                    int newW = (int)Math.Round(baseRect.Width * boundsScale);
                    int newH = (int)Math.Round(baseRect.Height * boundsScale);

                    c.SetBounds(newX, newY, newW, newH);

                    float baseFont;
                    if (_baseFontSize.TryGetValue(c, out baseFont) && c.Font != null)
                    {
                        float newFontSize = baseFont * fontScale;
                        newFontSize = Math.Max(newFontSize, 11f);
                        newFontSize = Math.Min(newFontSize, 26f);
                        c.Font = new Font(c.Font.FontFamily, newFontSize, c.Font.Style);
                    }

                    var lbl = c as Label;
                    if (lbl != null)
                    {
                        lbl.AutoSize = false;
                        lbl.Height = lbl.PreferredHeight;
                    }

                    var tb = c as TextBox;
                    if (tb != null && !tb.Multiline)
                        tb.Height = tb.PreferredHeight;

                    var nud = c as NumericUpDown;
                    if (nud != null)
                        nud.Height = nud.PreferredHeight;

                    var btn = c as Button;
                    if (btn != null)
                    {
                        Size pref = btn.PreferredSize;
                        int padH = (int)Math.Round(10 * boundsScale);
                        int padW = (int)Math.Round(16 * boundsScale);

                        int wantH = Math.Max(pref.Height + padH, (int)Math.Round(44 * boundsScale));
                        int wantW = Math.Max(pref.Width + padW, (int)Math.Round(150 * boundsScale));

                        int maxW = Math.Max(50, this.ClientSize.Width - btn.Left - 10);
                        btn.Width = Math.Min(wantW, maxW);
                        btn.Height = wantH;
                    }
                }

                if (txtDescription != null)
                {
                    txtDescription.Multiline = false;
                    txtDescription.WordWrap = false;
                    txtDescription.ScrollBars = ScrollBars.None;
                    txtDescription.ReadOnly = true;
                    txtDescription.TabStop = false;
                    txtDescription.BackColor = SystemColors.ControlLight;
                    txtDescription.Height = txtDescription.PreferredHeight;
                }

                if (txtNet != null)
                    txtNet.Multiline = true;

                this.ResumeLayout(true);
            }
            catch
            {
                try { this.ResumeLayout(true); } catch { }
            }
            finally
            {
                _applyingResponsiveLayout = false;
            }
        }

        // ---------------- Hotkeys ----------------
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Keys keyCode = keyData & Keys.KeyCode;

            if (keyCode == Keys.F13) { ClearAllFields(); return true; }
            if (keyCode == Keys.F14) { HotkeyPrint(); return true; }
            if (keyCode == Keys.F15) { ClearFocusedEntry(); return true; }
            if (keyCode == Keys.F16) { FocusAdjacentEditableTextBox(previous: true); return true; }
            if (keyCode == Keys.F17) { FocusAdjacentEditableTextBox(previous: false); return true; }
            if (keyCode == Keys.F18) { HotkeyForceRestart(); return true; }
            if (keyCode == Keys.F19) { SetCopiesHotkey(6); return true; }
            if (keyCode == Keys.F20) { SetCopiesHotkey(1); return true; }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private bool IsDebounced(ref DateTime lastUtc, int debounceMs)
        {
            var now = DateTime.UtcNow;

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
            try { if (btnPrint != null && btnPrint.Enabled) btnPrint.PerformClick(); } catch { }
        }

        private void SetCopiesHotkey(int copies)
        {
            try
            {
                if (numCopies == null) return;
                decimal v = copies;
                if (v < numCopies.Minimum) v = numCopies.Minimum;
                if (v > numCopies.Maximum) v = numCopies.Maximum;
                numCopies.Value = v;
                ShowStatus("Copies set to " + (int)numCopies.Value, isError: false);
            }
            catch { }
        }

        private void ClearFocusedEntry()
        {
            try
            {
                Control focused = GetFocusedControl(this);

                var tb = focused as TextBoxBase;
                if (tb != null)
                {
                    if (tb.ReadOnly) return;
                    tb.Text = "";
                    tb.Focus();
                    return;
                }

                var nud = focused as NumericUpDown;
                if (nud != null)
                {
                    nud.Value = nud.Minimum;
                    nud.Focus();
                    return;
                }
            }
            catch { }
        }

        private void FocusAdjacentEditableTextBox(bool previous)
        {
            try
            {
                var boxes = GetEditableTextBoxesOrdered();
                if (boxes.Count == 0) return;

                Control focused = GetFocusedControl(this);
                TextBox current = focused as TextBox;

                int idx = -1;
                if (current != null)
                {
                    for (int i = 0; i < boxes.Count; i++)
                    {
                        if (ReferenceEquals(boxes[i], current)) { idx = i; break; }
                    }
                }

                int targetIdx = (idx < 0)
                    ? (previous ? (boxes.Count - 1) : 0)
                    : (previous ? Math.Max(0, idx - 1) : Math.Min(boxes.Count - 1, idx + 1));

                boxes[targetIdx].Focus();
                boxes[targetIdx].SelectAll();
            }
            catch { }
        }

        private List<TextBox> GetEditableTextBoxesOrdered()
        {
            var list = new List<TextBox>();
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
                var tb = c as TextBox;
                if (tb != null) list.Add(tb);
                if (c.HasChildren) CollectTextBoxes(c, list);
            }
        }

        private static Control GetFocusedControl(Control control)
        {
            var cc = control as ContainerControl;
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

        // ---------------- UI styling ----------------
        private void ApplyModernUi()
        {
            this.Font = new Font("Segoe UI", 12f, FontStyle.Regular);
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(720, 480);
            this.Size = new Size(1000, 700);

            foreach (var btn in this.Controls.OfType<Button>())
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Height = Math.Max(btn.Height, 44);
                btn.Cursor = Cursors.Hand;

                if (btn == btnPrint)
                {
                    btn.UseVisualStyleBackColor = false;
                    btn.BackColor = Color.FromArgb(30, 136, 229);
                    btn.ForeColor = Color.White;
                    btn.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
                }
                else if (btn.Name == "btnClear")
                {
                    btn.UseVisualStyleBackColor = false;
                    btn.BackColor = Color.FromArgb(97, 97, 97);
                    btn.ForeColor = Color.White;
                    btn.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
                }
            }

            foreach (var tb in this.Controls.OfType<TextBox>())
            {
                if (tb == txtNet) continue;
                tb.Font = new Font("Segoe UI", 12f, FontStyle.Regular);
            }

            if (numCopies != null)
                numCopies.Font = new Font("Segoe UI", 12f, FontStyle.Regular);

            if (lblPrinterStatus != null)
                lblPrinterStatus.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
        }

        // ---------------- Printer status ----------------
        private void InitPrinterStatus()
        {
            _printerTimer.Stop();
            _printerTimer.Interval = PrinterPollIntervalMs;
            _printerTimer.Tick -= PrinterTimer_Tick;
            _printerTimer.Tick += PrinterTimer_Tick;
            _printerTimer.Start();
            UpdatePrinterStatus();
        }

        private void PrinterTimer_Tick(object sender, EventArgs e) => UpdatePrinterStatus();

        private void UpdatePrinterStatus()
        {
            bool ok = TryGetAnyDymoPrinter(out string printerName);
            if (lblPrinterStatus == null) return;

            if (ok)
            {
                string shown = string.IsNullOrWhiteSpace(printerName) ? "OK" : "OK (" + Ellipsize(printerName, 28) + ")";
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

        private static string Ellipsize(string s, int maxChars)
        {
            if (string.IsNullOrEmpty(s) || maxChars <= 0) return "";
            if (s.Length <= maxChars) return s;
            if (maxChars <= 3) return s.Substring(0, maxChars);
            return s.Substring(0, maxChars - 3) + "...";
        }

        private bool TryGetAnyDymoPrinter(out string printerName)
        {
            printerName = null;
            object addIn = null;

            try
            {
                addIn = CreateComFromProgIds(new[] { "Dymo.DymoAddIn", "DymoAddIn", "DYMO.DymoAddIn" });
                var printers = TryInvokeComString(addIn, "GetDymoPrinters");
                if (string.IsNullOrWhiteSpace(printers)) return false;

                var parts = printers
                    .Split(new[] { '|', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => (s ?? "").Trim())
                    .Where(s => s.Length > 0)
                    .ToList();

                if (parts.Count == 0) return false;

                var preferred = parts.FirstOrDefault(p =>
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
                var result = InvokeCom(target, methodName, args);
                return result == null ? null : result.ToString();
            }
            catch { return null; }
        }

        // ---------------- Core behavior ----------------
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                LoadProductsFromCsv();
                ShowStatus("Loaded " + _productsByCode.Count + " products", isError: false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CSV Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ShowStatus("CSV load failed", isError: true);
            }
        }

        private void txtProductCode_TextChanged(object sender, EventArgs e)
        {
            string code = NormalizeCode(txtProductCode.Text);

            if (string.IsNullOrWhiteSpace(code))
            {
                txtPrintProductId.Text = "";
                txtDescription.Text = "";
                ShowStatus("", isError: false);
                return;
            }

            ProductInfo info;
            if (_productsByCode.TryGetValue(code, out info))
            {
                txtPrintProductId.Text = info.PrintedProductId ?? "";
                txtDescription.Text = (info.Description ?? "").Replace("\r", " ").Replace("\n", " ").Trim();
                ShowStatus("OK", isError: false);
            }
            else
            {
                txtPrintProductId.Text = "";
                txtDescription.Text = "";
                ShowStatus("Product code not found", isError: true);
            }
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

        private void btnClear_Click(object sender, EventArgs e) => ClearAllFields();

        // ✅ ADDED: this aligns with Designer wiring and preserves intended behavior
        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintLabelsFromUi();
        }

        // ✅ ADDED: actual print routine (DYMO COM, template.label, copies)
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
                    ShowStatus("Copies must be at least 1.", isError: true);
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

                // DYMO COM late-binding
                addIn = CreateComFromProgIds(new[] { "Dymo.DymoAddIn", "DymoAddIn", "DYMO.DymoAddIn" });
                labels = CreateComFromProgIds(new[] { "Dymo.DymoLabels", "DymoLabels", "DYMO.DymoLabels" });

                if (!TryGetAnyDymoPrinter(out string printerName))
                    throw new Exception("No DYMO printer detected. Check USB/power and DYMO drivers.");

                // Select printer if possible
                if (!string.IsNullOrWhiteSpace(printerName))
                {
                    if (!TryInvokeCom(addIn, "SelectPrinter", printerName))
                        TryInvokeCom(addIn, "SelectPrinter2", printerName);
                }

                // Open template
                InvokeCom(addIn, "Open", labelPath);

                // Fill label fields (these must match your template object IDs)
                SetField(labels, "HEATID", heat);
                SetField(labels, "GROSS", gross.ToString("0.###", CultureInfo.InvariantCulture));
                SetField(labels, "TARE", tare.ToString("0.###", CultureInfo.InvariantCulture));
                SetField(labels, "NET", net.ToString("0.###", CultureInfo.InvariantCulture));
                SetField(labels, "PRODUCTID", (info.PrintedProductId ?? "").Trim());
                SetField(labels, "PRODUCTDESCRIPTION", (info.Description ?? "").Trim());

                ShowStatusPrinting("Printing " + copies + " label(s)...");

                TryInvokeCom(addIn, "StartPrintJob");
                printJobStarted = true;

                // Print copies
                InvokeCom(addIn, "Print", copies, false);

                TryInvokeCom(addIn, "EndPrintJob");
                printJobStarted = false;

                // Clear weights after print for quick repeat
                txtGross.Text = "";
                txtTare.Text = "";
                txtNet.Text = "";
                txtGross.Focus();

                ShowStatus("Printed " + copies + " label(s)", isError: false);
            }
            catch (COMException ex)
            {
                ShowStatus("Printer error: check roll/feed and connection.", isError: true);
                MessageBox.Show(ex.Message, "Printer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                ShowStatus(ex.Message, isError: true);
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
                ShowStatus("Cleared.", isError: false);
                txtProductCode.Focus();
            }
            catch { }
        }

        // ---------- CSV ----------
        private void LoadProductsFromCsv()
        {
            _productsByCode.Clear();

            string csvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "productIDs.csv");
            if (!File.Exists(csvPath))
                throw new FileNotFoundException("productIDs.csv not found next to EXE.", csvPath);

            using (var sr = new StreamReader(csvPath))
            {
                string headerLine = sr.ReadLine();
                if (headerLine == null) throw new Exception("productIDs.csv is empty.");

                var headers = SplitCsvLine(headerLine).Select(h => (h ?? "").Trim()).ToList();

                int idxCode = 0;
                int idxName = IndexOfHeader(headers, "Product Name");
                int idxDesc = IndexOfHeader(headers, "Product Description");

                if (idxName < 0 || idxDesc < 0)
                    throw new Exception("CSV must contain headers: 'Product Name' and 'Product Description'.");

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var cols = SplitCsvLine(line);
                    if (cols.Count <= Math.Max(idxDesc, Math.Max(idxName, idxCode))) continue;

                    string code = NormalizeCode(cols[idxCode]);
                    if (string.IsNullOrWhiteSpace(code)) continue;

                    _productsByCode[code] = new ProductInfo
                    {
                        PrintedProductId = (cols[idxName] ?? "").Trim(),
                        Description = (cols[idxDesc] ?? "").Trim(),
                    };
                }
            }
        }

        private static int IndexOfHeader(List<string> headers, string name)
        {
            for (int i = 0; i < headers.Count; i++)
                if (string.Equals(headers[i], name, StringComparison.OrdinalIgnoreCase))
                    return i;
            return -1;
        }

        private static List<string> SplitCsvLine(string line)
        {
            var result = new List<string>();
            bool inQuotes = false;
            string cur = "";

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        cur += '"';
                        i++;
                    }
                    else inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(cur);
                    cur = "";
                }
                else cur += c;
            }

            result.Add(cur);
            return result;
        }

        private static string NormalizeCode(string input)
        {
            if (input == null) return "";
            return Regex.Replace(input.Trim(), @"\D", "");
        }

        private static bool TryParseDecimal(string s, out decimal value)
        {
            s = (s ?? "").Trim().Replace(",", "");
            return decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }

        // ---------- DYMO COM late-binding ----------
        private static object CreateComFromProgIds(string[] progIds)
        {
            foreach (var progId in progIds)
            {
                var t = Type.GetTypeFromProgID(progId, throwOnError: false);
                if (t == null) continue;

                try
                {
                    var obj = Activator.CreateInstance(t);
                    if (obj != null) return obj;
                }
                catch { }
            }

            throw new Exception(
                "Could not create DYMO COM object. Tried:\n- " + string.Join("\n- ", progIds) +
                "\n\nDYMO Label Software components may not be registered on this PC."
            );
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
                args
            );
        }

        private static bool TryInvokeCom(object target, string methodName, params object[] args)
        {
            try { InvokeCom(target, methodName, args); return true; }
            catch { return false; }
        }
    }
}