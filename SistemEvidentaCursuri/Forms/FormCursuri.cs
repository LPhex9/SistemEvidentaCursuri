using SistemEvidentaCursuri.Data;
using SistemEvidentaCursuri.Models;

namespace SistemEvidentaCursuri.Forms
{
    public class FormCursuri : Form
    {
        private DataGridView dgv = new();
        private TextBox txtFormator = new();
        private NumericUpDown numDurata = new();
        private CheckBox chkDurata = new();

        public FormCursuri()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Gestionare Cursuri";
            this.Size = new Size(820, 560);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 250);

            // ── Top bar ──
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(39, 174, 96) };
            var lblTitle = new Label
            {
                Text = "📚 Cursuri",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Left,
                Width = 200,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            pnlTop.Controls.Add(lblTitle);
            this.Controls.Add(pnlTop);

            // ── Filter bar ──
            var pnlFilter = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 48,
                Padding = new Padding(10, 8, 10, 0),
                BackColor = Color.FromArgb(240, 248, 240)
            };

            pnlFilter.Controls.Add(new Label { Text = "Formator:", TextAlign = ContentAlignment.MiddleLeft, Width = 65, Height = 30 });
            txtFormator = new TextBox { Width = 160, Height = 30, Font = new Font("Segoe UI", 9.5F) };
            txtFormator.TextChanged += (s, e) => ApplyFilter();
            pnlFilter.Controls.Add(txtFormator);

            pnlFilter.Controls.Add(new Label { Text = "  Durată (zile):", TextAlign = ContentAlignment.MiddleLeft, Width = 100, Height = 30 });
            chkDurata = new CheckBox { Text = "", Width = 20, Height = 30, CheckAlign = ContentAlignment.MiddleCenter };
            chkDurata.CheckedChanged += (s, e) => { numDurata.Enabled = chkDurata.Checked; ApplyFilter(); };
            pnlFilter.Controls.Add(chkDurata);
            numDurata = new NumericUpDown { Width = 60, Minimum = 1, Maximum = 365, Value = 5, Enabled = false };
            numDurata.ValueChanged += (s, e) => { if (chkDurata.Checked) ApplyFilter(); };
            pnlFilter.Controls.Add(numDurata);

            var btnReset = new Button { Text = "✖ Resetează", Width = 100, Height = 30, Margin = new Padding(15, 0, 0, 0) };
            btnReset.Click += (s, e) => { txtFormator.Clear(); chkDurata.Checked = false; LoadData(); };
            pnlFilter.Controls.Add(btnReset);
            this.Controls.Add(pnlFilter);

            // ── DataGridView ──
            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9.5F),
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(39, 174, 96),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold)
                }
            };
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 35;
            this.Controls.Add(dgv);

            // ── Buttons ──
            var pnlBtn = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(10, 8, 10, 8),
                BackColor = Color.FromArgb(240, 248, 240)
            };
            pnlBtn.Controls.Add(MakeBtn("➕ Adaugă", Color.FromArgb(39, 174, 96), BtnAdauga_Click));
            pnlBtn.Controls.Add(MakeBtn("✏️ Modifică", Color.FromArgb(41, 128, 185), BtnModifica_Click));
            pnlBtn.Controls.Add(MakeBtn("🗑️ Șterge", Color.FromArgb(192, 57, 43), BtnSterge_Click));
            this.Controls.Add(pnlBtn);
        }

        private Button MakeBtn(string text, Color color, EventHandler onClick)
        {
            var btn = new Button
            {
                Text = text,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Height = 33,
                AutoSize = true,
                Padding = new Padding(10, 0, 10, 0),
                Margin = new Padding(3, 0, 3, 0),
                Font = new Font("Segoe UI", 9.5F)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += onClick;
            return btn;
        }

        private void LoadData()
        {
            dgv.DataSource = DatabaseHelper.GetAllCursuri();
            SetColumns();
        }

        private void ApplyFilter()
        {
            int? durata = chkDurata.Checked ? (int)numDurata.Value : null;
            dgv.DataSource = DatabaseHelper.FilterCursuri(txtFormator.Text, durata);
            SetColumns();
        }

        private void SetColumns()
        {
            if (dgv.Columns.Contains("IdCurs"))     dgv.Columns["IdCurs"].Visible = false;
            if (dgv.Columns.Contains("Denumire"))   dgv.Columns["Denumire"].HeaderText = "Denumire Curs";
            if (dgv.Columns.Contains("Formator"))   dgv.Columns["Formator"].HeaderText = "Formator";
            if (dgv.Columns.Contains("Pret"))       { dgv.Columns["Pret"].HeaderText = "Preț (MDL)"; dgv.Columns["Pret"].DefaultCellStyle.Format = "N2"; }
            if (dgv.Columns.Contains("DurataZile")) dgv.Columns["DurataZile"].HeaderText = "Durată (zile)";
        }

        private Curs? GetSelected()
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selectați un curs.", "Atenție", MessageBoxButtons.OK, MessageBoxIcon.Warning); return null; }
            return dgv.SelectedRows[0].DataBoundItem as Curs;
        }

        private void BtnAdauga_Click(object? sender, EventArgs e)
        {
            using var form = new FormCursEdit();
            if (form.ShowDialog() == DialogResult.OK) LoadData();
        }

        private void BtnModifica_Click(object? sender, EventArgs e)
        {
            var curs = GetSelected();
            if (curs == null) return;
            using var form = new FormCursEdit(curs);
            if (form.ShowDialog() == DialogResult.OK) LoadData();
        }

        private void BtnSterge_Click(object? sender, EventArgs e)
        {
            var curs = GetSelected();
            if (curs == null) return;

            if (DatabaseHelper.CursHasInscrieri(curs.IdCurs))
            {
                MessageBox.Show("Nu puteți șterge un curs cu înscrieri active!", "Restricție", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show($"Ștergeți cursul '{curs.Denumire}'?", "Confirmare",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DatabaseHelper.DeleteCurs(curs.IdCurs);
                LoadData();
            }
        }
    }
}
