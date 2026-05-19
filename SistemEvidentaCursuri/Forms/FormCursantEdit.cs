using SistemEvidentaCursuri.Data;
using SistemEvidentaCursuri.Models;
using SistemEvidentaCursuri.Helpers;

namespace SistemEvidentaCursuri.Forms
{
    public class FormCursantEdit : Form
    {
        private readonly Cursant? _cursant;
        private TextBox txtNume = new(), txtPrenume = new(), txtTelefon = new(), txtEmail = new();

        public FormCursantEdit(Cursant? cursant = null)
        {
            _cursant = cursant;
            InitializeComponent();
            if (cursant != null)
            {
                txtNume.Text    = cursant.Nume;
                txtPrenume.Text = cursant.Prenume;
                txtTelefon.Text = cursant.Telefon;
                txtEmail.Text   = cursant.Email;
            }
        }

        private void InitializeComponent()
        {
            this.Text = _cursant == null ? "Adaugă Cursant" : "Modifică Cursant";
            this.Size = new Size(380, 320);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5,
                Padding = new Padding(15),
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int i = 0; i < 5; i++)
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));

            AddRow(layout, 0, "Nume *", txtNume);
            AddRow(layout, 1, "Prenume *", txtPrenume);
            AddRow(layout, 2, "Telefon *", txtTelefon);
            AddRow(layout, 3, "Email *", txtEmail);

            var pnlBtn = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Fill
            };
            var btnSave = new Button { Text = "💾 Salvează", BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 110, Height = 30 };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            var btnCancel = new Button { Text = "Anulează", FlatStyle = FlatStyle.Flat, Width = 90, Height = 30 };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            pnlBtn.Controls.AddRange(new Control[] { btnSave, btnCancel });
            layout.Controls.Add(pnlBtn);
            layout.SetColumn(pnlBtn, 0);
            layout.SetRow(pnlBtn, 4);
            layout.SetColumnSpan(pnlBtn, 2);

            this.Controls.Add(layout);
        }

        private static void AddRow(TableLayoutPanel layout, int row, string label, TextBox txt)
        {
            layout.Controls.Add(new Label { Text = label, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 9.5F) }, 0, row);
            txt.Dock = DockStyle.Fill;
            txt.Font = new Font("Segoe UI", 9.5F);
            layout.Controls.Add(txt, 1, row);
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNume.Text) || string.IsNullOrWhiteSpace(txtPrenume.Text) ||
                string.IsNullOrWhiteSpace(txtTelefon.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Toate câmpurile sunt obligatorii!", "Validare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!Validator.IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Formatul emailului este invalid!", "Validare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!Validator.IsValidPhone(txtTelefon.Text))
            {
                MessageBox.Show("Formatul telefonului este invalid! (ex: 069123456)", "Validare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int excludeId = _cursant?.IdCursant ?? 0;
            if (DatabaseHelper.EmailExistsCursant(txtEmail.Text.Trim(), excludeId))
            {
                MessageBox.Show("Există deja un cursant cu acest email!", "Validare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var cursant = new Cursant
            {
                IdCursant = _cursant?.IdCursant ?? 0,
                Nume      = txtNume.Text.Trim(),
                Prenume   = txtPrenume.Text.Trim(),
                Telefon   = txtTelefon.Text.Trim(),
                Email     = txtEmail.Text.Trim()
            };

            if (_cursant == null) DatabaseHelper.AddCursant(cursant);
            else                  DatabaseHelper.UpdateCursant(cursant);

            this.DialogResult = DialogResult.OK;
        }
    }
}
