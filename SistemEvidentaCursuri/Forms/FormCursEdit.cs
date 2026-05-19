using SistemEvidentaCursuri.Data;
using SistemEvidentaCursuri.Models;

namespace SistemEvidentaCursuri.Forms
{
    public class FormCursEdit : Form
    {
        private readonly Curs? _curs;
        private TextBox txtDenumire = new(), txtFormator = new();
        private NumericUpDown numPret = new(), numDurata = new();

        public FormCursEdit(Curs? curs = null)
        {
            _curs = curs;
            InitializeComponent();
            if (curs != null)
            {
                txtDenumire.Text  = curs.Denumire;
                txtFormator.Text  = curs.Formator;
                numPret.Value     = curs.Pret;
                numDurata.Value   = curs.DurataZile;
            }
        }

        private void InitializeComponent()
        {
            this.Text = _curs == null ? "Adaugă Curs" : "Modifică Curs";
            this.Size = new Size(380, 310);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5,
                Padding = new Padding(15)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int i = 0; i < 5; i++)
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));

            txtDenumire = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9.5F) };
            txtFormator = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9.5F) };
            numPret     = new NumericUpDown { Dock = DockStyle.Fill, Minimum = 1, Maximum = 999999, DecimalPlaces = 2, Value = 100 };
            numDurata   = new NumericUpDown { Dock = DockStyle.Fill, Minimum = 1, Maximum = 365, Value = 5 };

            AddRow(layout, 0, "Denumire *", txtDenumire);
            AddRow(layout, 1, "Formator *", txtFormator);
            AddRow(layout, 2, "Preț (MDL) *", numPret);
            AddRow(layout, 3, "Durată (zile) *", numDurata);

            var pnlBtn = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill };
            var btnSave = new Button { Text = "💾 Salvează", BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 110, Height = 30 };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            var btnCancel = new Button { Text = "Anulează", FlatStyle = FlatStyle.Flat, Width = 90, Height = 30 };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            pnlBtn.Controls.AddRange(new Control[] { btnSave, btnCancel });
            layout.Controls.Add(pnlBtn);
            layout.SetColumn(pnlBtn, 0); layout.SetRow(pnlBtn, 4); layout.SetColumnSpan(pnlBtn, 2);

            this.Controls.Add(layout);
        }

        private static void AddRow(TableLayoutPanel layout, int row, string label, Control ctrl)
        {
            layout.Controls.Add(new Label { Text = label, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 9.5F) }, 0, row);
            ctrl.Dock = DockStyle.Fill;
            layout.Controls.Add(ctrl, 1, row);
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDenumire.Text) || string.IsNullOrWhiteSpace(txtFormator.Text))
            {
                MessageBox.Show("Toate câmpurile sunt obligatorii!", "Validare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (numPret.Value <= 0)
            {
                MessageBox.Show("Prețul trebuie să fie mai mare decât 0!", "Validare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var curs = new Curs
            {
                IdCurs     = _curs?.IdCurs ?? 0,
                Denumire   = txtDenumire.Text.Trim(),
                Formator   = txtFormator.Text.Trim(),
                Pret       = numPret.Value,
                DurataZile = (int)numDurata.Value
            };

            if (_curs == null) DatabaseHelper.AddCurs(curs);
            else               DatabaseHelper.UpdateCurs(curs);

            this.DialogResult = DialogResult.OK;
        }
    }
}
