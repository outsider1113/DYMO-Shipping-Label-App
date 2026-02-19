using System;
using System.Collections.Generic;
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
            public string PrintedProductId; // CSV "Product Name" (long ID)
            public string Description;      // CSV "Product Description"
        }

        // key = 4-digit code in first CSV column (header is blank)
        private readonly Dictionary<string, ProductInfo> _productsByCode =
            new Dictionary<string, ProductInfo>(StringComparer.OrdinalIgnoreCase);

        // Force WinForms timer to avoid ambiguity
        private readonly System.Windows.Forms.Timer _printerTimer = new System.Windows.Forms.Timer();
        private const int PrinterPollIntervalMs = 1500;

        // Reuse one tooltip (avoid allocating a new one every tick)
        private readonly ToolTip _printerToolTip = new ToolTip();

        public Form1()
        {
            InitializeComponent();

            // UX tweaks
            txtNet.ReadOnly = true;

            // Make Net big + bold, and clearly non-editable
            txtNet.TabStop = false;                 // skip when tabbing through fields
            txtNet.Cursor = Cursors.Default;        // don't show I-beam cursor
            txtNet.BorderStyle = BorderStyle.FixedSingle;
            txtNet.BackColor = SystemColors.Window; // keep it clean/white
            txtNet.TextAlign = HorizontalAlignment.Right;

            // TextBox needs Multiline=true to honor custom Height in WinForms
            txtNet.Multiline = true;
            txtNet.Width = 240;
            txtNet.Height = 32;

            // Larger, bold font for visual clarity
            txtNet.Font = new Font(txtNet.Font.FontFamily, 16f, FontStyle.Bold);

            txtPrintProductId.ReadOnly = true;
            txtDescription.ReadOnly = true;

            ApplyModernUi();

            // Copies control (1-99)
            if (numCopies != null)
            {
                numCopies.Minimum = 1;
                numCopies.Maximum = 99;
                if (numCopies.Value < 1 || numCopies.Value > 99) numCopies.Value = 1;
                numCopies.TextAlign = HorizontalAlignment.Right;
            }

            // Status bar styling (reuses existing lblStatus)
            if (lblStatus != null)
            {
                lblStatus.AutoSize = false;
                lblStatus.Dock = DockStyle.Bottom;
                lblStatus.Height = 26;
                lblStatus.TextAlign = ContentAlignment.MiddleLeft;
                lblStatus.Padding = new Padding(8, 0, 8, 0);
                lblStatus.BackColor = Color.Gainsboro;
                lblStatus.ForeColor = Color.Black;
                lblStatus.Text = "";
            }

            // Printer status indicator (top-right)
            InitPrinterStatus();
        }

        private void ApplyModernUi()
        {
            // Form base styling
            this.Font = new Font("Segoe UI", 10f, FontStyle.Regular);
            this.BackColor = Color.White;

            // Allow fullscreen/maximize and a larger starting size
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(720, 480);
            this.Size = new Size(900, 600);

            // Buttons: flatter + consistent size
            foreach (var btn in this.Controls.OfType<Button>())
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Height = Math.Max(btn.Height, 40);
                btn.Cursor = Cursors.Hand;

                // Make the primary button pop a bit, but still simple
                if (btn == btnPrint)
                {
                    btn.UseVisualStyleBackColor = false; // required for BackColor to show
                    btn.BackColor = Color.FromArgb(30, 136, 229);
                    btn.ForeColor = Color.White;
                    btn.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
                }
                else if (btn.Name == "btnClear")
                {
                    btn.UseVisualStyleBackColor = false;
                    btn.BackColor = Color.FromArgb(97, 97, 97);
                    btn.ForeColor = Color.White;
                    btn.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
                }
            }

            // Inputs: slightly larger for workers
            foreach (var tb in this.Controls.OfType<TextBox>())
            {
                if (tb == txtNet) continue; // already styled
                tb.Font = new Font("Segoe UI", 10f, FontStyle.Regular);
            }

            if (numCopies != null)
            {
                numCopies.Font = new Font("Segoe UI", 10f, FontStyle.Regular);
            }

            if (lblPrinterStatus != null)
            {
                lblPrinterStatus.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
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
            bool ok = TryGetAnyDymoPrinter(out string printerName);

            if (lblPrinterStatus == null) return;

            if (ok)
            {
                string shown = string.IsNullOrWhiteSpace(printerName)
                    ? "OK"
                    : "OK (" + Ellipsize(printerName, 28) + ")";

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

                // Most DYMO COM installs support GetDymoPrinters(), returning a string list
                var printers = TryInvokeComString(addIn, "GetDymoPrinters");
                if (string.IsNullOrWhiteSpace(printers)) return false;

                // Common formats: "Printer1|Printer2|..." OR "Printer1;Printer2;..." (sometimes newline-separated)
                var parts = printers
                    .Split(new[] { '|', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => (s ?? "").Trim())
                    .Where(s => s.Length > 0)
                    .ToList();

                if (parts.Count == 0) return false;

                printerName = parts[0];
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
                txtDescription.Text = info.Description ?? "";
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

        private void btnPrint_Click(object sender, EventArgs e)
        {
            object addIn = null;
            object labels = null;
            bool printJobStarted = false;

            try
            {
                // Copies (1-99)
                int copies = 1;
                if (numCopies != null) copies = (int)numCopies.Value;
                if (copies < 1)
                {
                    ShowStatus("Copies must be at least 1.", isError: true);
                    return;
                }

                // Validate
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

                // Files (must be copied next to EXE)
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string labelPath = System.IO.Path.Combine(baseDir, "template.label");
                if (!File.Exists(labelPath))
                    throw new FileNotFoundException("template.label not found next to EXE.", labelPath);

                // DYMO COM late-binding (works with your setup)
                addIn = CreateComFromProgIds(new[] { "Dymo.DymoAddIn", "DymoAddIn", "DYMO.DymoAddIn" });
                labels = CreateComFromProgIds(new[] { "Dymo.DymoLabels", "DymoLabels", "DYMO.DymoLabels" });

                // Quick "is a printer there?" check (best-effort)
                if (!TryGetAnyDymoPrinter(out string printerName))
                    throw new Exception("No DYMO printer detected. Check USB/power and DYMO drivers.");

                // Show the printer name (if available)
                if (!string.IsNullOrWhiteSpace(printerName) && lblPrinterStatus != null)
                {
                    try { lblPrinterStatus.Text = "Printer: OK (" + Ellipsize(printerName, 28) + ")"; } catch { }
                }

                InvokeCom(addIn, "Open", labelPath);

                // Fill template objects (these must match your Object IDs)
                SetField(labels, "HEATID", heat);
                SetField(labels, "GROSS", gross.ToString("0.###", CultureInfo.InvariantCulture));
                SetField(labels, "TARE", tare.ToString("0.###", CultureInfo.InvariantCulture));
                SetField(labels, "NET", net.ToString("0.###", CultureInfo.InvariantCulture));

                // CSV conventions:
                // Product Name = long-form ID → print into PRODUCTID
                // Product Description = item name → print into PRODUCTDESCRIPTION
                SetField(labels, "PRODUCTID", (info.PrintedProductId ?? "").Trim());
                SetField(labels, "PRODUCTDESCRIPTION", (info.Description ?? "").Trim());

                // Yellow status while printing
                ShowStatusPrinting("Printing " + copies + " label(s)...");
                if (lblStatus != null) lblStatus.Refresh();

                TryInvokeCom(addIn, "StartPrintJob");
                printJobStarted = true;

                // Print N copies
                InvokeCom(addIn, "Print", copies, false);

                TryInvokeCom(addIn, "EndPrintJob");
                printJobStarted = false;

                // After print: clear just weights for fast repeat
                txtGross.Text = "";
                txtTare.Text = "";
                txtNet.Text = "";
                txtGross.Focus();

                ShowStatus("Printed " + copies + " label(s)", isError: false);
            }
            catch (COMException ex)
            {
                ShowStatus("Printer error: check roll/feed and printer connection.", isError: true);

                MessageBox.Show(
                    "Printer error occurred.\r\n\r\n" +
                    "Check:\r\n" +
                    "- Label roll loaded\r\n" +
                    "- Feed path not jammed\r\n" +
                    "- Printer connected & powered\r\n" +
                    "- DYMO software can see the printer\r\n\r\n" +
                    "Details:\r\n" + ex.Message,
                    "Printer Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
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
                    // Try to end print job to avoid getting "stuck"
                    try { TryInvokeCom(addIn, "EndPrintJob"); } catch { }
                }

                ReleaseComObjectQuietly(labels);
                ReleaseComObjectQuietly(addIn);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAllFields();
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
            catch
            {
                // Don't crash on clear
            }
        }

        // ---------- CSV ----------
        private void LoadProductsFromCsv()
        {
            _productsByCode.Clear();

            string csvPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "productIDs.csv");
            if (!File.Exists(csvPath))
                throw new FileNotFoundException("productIDs.csv not found next to EXE.", csvPath);

            using (var sr = new StreamReader(csvPath))
            {
                string headerLine = sr.ReadLine();
                if (headerLine == null) throw new Exception("productIDs.csv is empty.");

                var headers = SplitCsvLine(headerLine).Select(h => (h ?? "").Trim()).ToList();

                int idxCode = 0; // first column is blank header
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
            // keep digits only (handles weird "1002.0" etc)
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
            try
            {
                InvokeCom(target, methodName, args);
                return true;
            }
            catch { return false; }
        }
    }
}
