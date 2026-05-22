using SistemEvidentaCursuri.Data;
using SistemEvidentaCursuri.Models;

namespace SistemEvidentaCursuri.Forms
{
    public class FormCursanti : Form
    {
        private DataGridView dgv = new();
        private TextBox txtSearch = new();

        public FormCursanti()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Gestionare Cursanți";
            this.Size = new Size(800, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 250);

            // ── Top bar ──
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(41, 128, 185) };
            var lblTitle = new Label
            {
                Text = "👤 Cursanți",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Left,
                Width = 200,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            pnlTop.Controls.Add(lblTitle);
            this.Controls.Add(pnlTop);

            // ── Search bar ──
            var pnlSearch = new Panel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(10, 8, 10, 0) };
            txtSearch = new TextBox
            {
                PlaceholderText = "🔍 Caută după Nume sau Email...",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F)
            };
            txtSearch.TextChanged += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text)) LoadData();
                else { dgv.DataSource = DatabaseHelper.SearchCursanti(txtSearch.Text); SetColumns(); }
            };
            pnlSearch.Controls.Add(txtSearch);
            this.Controls.Add(pnlSearch);

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
                    BackColor = Color.FromArgb(41, 128, 185),
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
                BackColor = Color.FromArgb(240, 240, 245)
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
            dgv.DataSource = DatabaseHelper.GetAllCursanti();
            SetColumns();
        }

        private void SetColumns()
        {
            if (dgv.Columns.Contains("IdCursant"))   dgv.Columns["IdCursant"].Visible = false;
            if (dgv.Columns.Contains("NumeComplet")) dgv.Columns["NumeComplet"].Visible = false;
            if (dgv.Columns.Contains("Nume"))        { dgv.Columns["Nume"].HeaderText = "Nume"; dgv.Columns["Nume"].Width = 150; }
            if (dgv.Columns.Contains("Prenume"))     { dgv.Columns["Prenume"].HeaderText = "Prenume"; dgv.Columns["Prenume"].Width = 150; }
            if (dgv.Columns.Contains("Telefon"))     dgv.Columns["Telefon"].HeaderText = "Telefon";
            if (dgv.Columns.Contains("Email"))       dgv.Columns["Email"].HeaderText = "Email";
        }

        private Cursant? GetSelected()
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selectați un cursant.", "Atenție", MessageBoxButtons.OK, MessageBoxIcon.Warning); return null; }
            return dgv.SelectedRows[0].DataBoundItem as Cursant;
        }

        private void BtnAdauga_Click(object? sender, EventArgs e)
        {
            using var form = new FormCursantEdit();
            if (form.ShowDialog() == DialogResult.OK) LoadData();
        }

        private void BtnModifica_Click(object? sender, EventArgs e)
        {
            var cursant = GetSelected();
            if (cursant == null) return;
            using var form = new FormCursantEdit(cursant);
            if (form.ShowDialog() == DialogResult.OK) LoadData();
        }

        private void BtnSterge_Click(object? sender, EventArgs e)
        {
            var cursant = GetSelected();
            if (cursant == null) return;

            if (DatabaseHelper.CursantHasInscrieri(cursant.IdCursant))
            {
                MessageBox.Show("Nu puteți șterge un cursant cu înscrieri active!", "Restricție", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show($"Ștergeți cursantul '{cursant.NumeComplet}'?", "Confirmare",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DatabaseHelper.DeleteCursant(cursant.IdCursant);
                LoadData();
            }
        }
    }
}
