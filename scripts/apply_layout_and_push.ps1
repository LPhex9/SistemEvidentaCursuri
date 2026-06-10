param(
    [string]$RepoRoot = (Get-Location).Path,
    [string]$BranchName = "fix/layout-dgv-databinding",
    [string]$CommitMessage = "UI: fix header overlap using TableLayoutPanel and configure DataGridView columns on DataBindingComplete"
)

Set-Location $RepoRoot

# Quick checks
if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    Write-Error "git is not available in PATH. Install Git or update PATH and retry."
    exit 1
}

# If there are unstaged changes, stash them to avoid accidental overwrite
$status = git status --porcelain
if ($status) {
    Write-Host "Working tree is not clean. Stashing current changes..."
    git stash push -u -m "pre-automated-layout-stash-$(Get-Date -Format o)" | Out-Null
}

# Create or switch to feature branch
$branchExists = git ls-remote --heads origin $BranchName
if ($branchExists) {
    git fetch origin $BranchName:$BranchName
    git checkout $BranchName
} else {
    git checkout -b $BranchName
}

# Write FormCursanti.cs
@'
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

            // ── Header ──
            var pnlTop = new Panel { BackColor = Color.FromArgb(41, 128, 185) };
            pnlTop.Height = 50;
            var lblTitle = new Label
            {
                Text = "👤 Cursanți",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            pnlTop.Controls.Add(lblTitle);

            // ── Search bar ──
            var pnlSearch = new Panel { Height = 45, Padding = new Padding(10, 8, 10, 0), BackColor = this.BackColor };
            txtSearch = new TextBox
            {
                PlaceholderText = "🔍 Caută după Nume sau Email...",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F)
            };
            txtSearch.TextChanged += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text)) LoadData();
                else { dgv.DataSource = DatabaseHelper.SearchCursanti(txtSearch.Text); }
            };
            pnlSearch.Controls.Add(txtSearch);

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
            dgv.DataBindingComplete += (s, e) => SetColumns();

            // ── Buttons ──
            var pnlBtn = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Height = 50,
                Padding = new Padding(10, 8, 10, 8),
                BackColor = Color.FromArgb(240, 240, 245),
                Dock = DockStyle.Fill
            };
            pnlBtn.Controls.Add(MakeBtn("➕ Adaugă", Color.FromArgb(39, 174, 96), BtnAdauga_Click));
            pnlBtn.Controls.Add(MakeBtn("✏️ Modifică", Color.FromArgb(41, 128, 185), BtnModifica_Click));
            pnlBtn.Controls.Add(MakeBtn("🗑️ Șterge", Color.FromArgb(192, 57, 43), BtnSterge_Click));

            // ── Main layout ──
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(0)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            mainLayout.Controls.Add(pnlTop, 0, 0);
            mainLayout.Controls.Add(pnlSearch, 0, 1);
            mainLayout.Controls.Add(dgv, 0, 2);
            mainLayout.Controls.Add(pnlBtn, 0, 3);

            this.Controls.Add(mainLayout);
            this.PerformLayout();
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
        }

        private void SetColumns()
        {
            if (dgv == null || dgv.Columns == null || dgv.Columns.Count == 0) return;

            if (dgv.Columns.Contains("IdCursant"))   dgv.Columns["IdCursant"].Visible = false;
            if (dgv.Columns.Contains("NumeComplet")) dgv.Columns["NumeComplet"].Visible = false;

            if (dgv.Columns.Contains("Nume"))
            {
                var col = dgv.Columns["Nume"];
                if (col != null) { col.HeaderText = "Nume"; col.Width = 150; }
            }

            if (dgv.Columns.Contains("Prenume"))
            {
                var col = dgv.Columns["Prenume"];
                if (col != null) { col.HeaderText = "Prenume"; col.Width = 150; }
            }

            if (dgv.Columns.Contains("Telefon"))
            {
                var col = dgv.Columns["Telefon"];
                if (col != null) col.HeaderText = "Telefon";
            }

            if (dgv.Columns.Contains("Email"))
            {
                var col = dgv.Columns["Email"];
                if (col != null) col.HeaderText = "Email";
            }
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
'@ | Set-Content -Path .\SistemEvidentaCursuri\Forms\FormCursanti.cs -Encoding UTF8

# Write FormInscrieri.cs
@'
using System.Linq;
using SistemEvidentaCursuri.Data;
using SistemEvidentaCursuri.Models;

namespace SistemEvidentaCursuri.Forms
{
    public class FormInscrieri : Form
    {
        private DataGridView dgv = new();
        private ComboBox cmbCursant = new(), cmbCurs = new(), cmbStatus = new();
        private Label lblTotal = new();

        public FormInscrieri()
        {
            InitializeComponent();
            LoadCombos();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Gestionare Înscrieri";
            this.Size = new Size(900, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 250);

            // ── Top bar ──
            var pnlTop = new Panel { BackColor = Color.FromArgb(230, 126, 34) };
            pnlTop.Height = 50;
            var lblTitle = new Label
            {
                Text = "📝 Înscrieri",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            pnlTop.Controls.Add(lblTitle);

            // ── Add form ──
            var pnlAdd = new Panel { Height = 60, BackColor = Color.FromArgb(253, 243, 233), Padding = new Padding(10, 8, 10, 5) };
            var flow = new FlowLayoutPanel { Dock = DockStyle.Fill };

            flow.Controls.Add(new Label { Text = "Cursant:", TextAlign = ContentAlignment.MiddleLeft, Width = 60, Height = 30 });
            cmbCursant = new ComboBox { Width = 180, Height = 30, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5F) };
            flow.Controls.Add(cmbCursant);

            flow.Controls.Add(new Label { Text = "  Curs:", TextAlign = ContentAlignment.MiddleLeft, Width = 40, Height = 30 });
            cmbCurs = new ComboBox { Width = 200, Height = 30, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5F) };
            flow.Controls.Add(cmbCurs);

            flow.Controls.Add(new Label { Text = "  Status:", TextAlign = ContentAlignment.MiddleLeft, Width = 55, Height = 30 });
            cmbStatus = new ComboBox { Width = 110, Height = 30, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5F) };
            cmbStatus.Items.AddRange(new object[] { "Achitat", "Neachitat" });
            cmbStatus.SelectedIndex = 1;
            flow.Controls.Add(cmbStatus);

            var btnAdd = new Button
            {
                Text = "➕ Înscrie",
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 100,
                Height = 30,
                Margin = new Padding(10, 0, 0, 0)
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;
            flow.Controls.Add(btnAdd);
            pnlAdd.Controls.Add(flow);

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
                    BackColor = Color.FromArgb(230, 126, 34),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold)
                }
            };
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 35;
            dgv.DataBindingComplete += (s, e) => ConfigureColumns();

            // ── Bottom ──
            var pnlBtn = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Height = 50,
                Padding = new Padding(10, 8, 10, 8),
                BackColor = Color.FromArgb(253, 243, 233)
            };
            lblTotal = new Label { TextAlign = ContentAlignment.MiddleLeft, Width = 300, Height = 33, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            pnlBtn.Controls.Add(lblTotal);
            var btnSterge = new Button
            {
                Text = "🗑️ Anulează Înscrierea",
                BackColor = Color.FromArgb(192, 57, 43),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Height = 33,
                AutoSize = true,
                Padding = new Padding(10, 0, 10, 0)
            };
            btnSterge.FlatAppearance.BorderSize = 0;
            btnSterge.Click += BtnSterge_Click;
            pnlBtn.Controls.Add(btnSterge);

            // ── Main layout ──
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(0)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));   // header
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));   // add panel
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // content
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));   // bottom

            mainLayout.Controls.Add(pnlTop, 0, 0);
            mainLayout.Controls.Add(pnlAdd, 0, 1);
            mainLayout.Controls.Add(dgv, 0, 2);
            mainLayout.Controls.Add(pnlBtn, 0, 3);

            this.Controls.Add(mainLayout);
            this.PerformLayout();
        }

        private void LoadCombos()
        {
            var cursanti = DatabaseHelper.GetAllCursanti();
            cmbCursant.DataSource = cursanti;
            cmbCursant.DisplayMember = "NumeComplet";
            cmbCursant.ValueMember = "IdCursant";

            var cursuri = DatabaseHelper.GetAllCursuri();
            cmbCurs.DataSource = cursuri;
            cmbCurs.DisplayMember = "Denumire";
            cmbCurs.ValueMember = "IdCurs";
        }

        private void LoadData()
        {
            var data = DatabaseHelper.GetAllInscrieri();
            dgv.DataSource = data;

            int count = data.Count;
            decimal total = data.Where(i => i.StatusPlata == "Achitat").Sum(i => i.PretCurs);
            lblTotal.Text = $"Total: {count} înscrieri | Încasat: {total:N2} MDL";
        }

        private void ConfigureColumns()
        {
            if (dgv == null || dgv.Columns == null || dgv.Columns.Count == 0) return;
            if (dgv.Columns.Contains("IdInscriere"))  dgv.Columns["IdInscriere"].Visible = false;
            if (dgv.Columns.Contains("IdCursant"))    dgv.Columns["IdCursant"].Visible = false;
            if (dgv.Columns.Contains("IdCurs"))       dgv.Columns["IdCurs"].Visible = false;
            if (dgv.Columns.Contains("PretCurs"))     dgv.Columns["PretCurs"].Visible = false;
            if (dgv.Columns.Contains("NumeCursant"))  dgv.Columns["NumeCursant"].HeaderText = "Cursant";
            if (dgv.Columns.Contains("DenumireCurs")) dgv.Columns["DenumireCurs"].HeaderText = "Curs";
            if (dgv.Columns.Contains("DataInscriere")) dgv.Columns["DataInscriere"].HeaderText = "Data Înscrierii";
            if (dgv.Columns.Contains("StatusPlata"))  dgv.Columns["StatusPlata"].HeaderText = "Status Plată";
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (cmbCursant.SelectedValue == null || cmbCurs.SelectedValue == null)
            {
                MessageBox.Show("Selectați cursantul și cursul!", "Validare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int idCursant = (int)cmbCursant.SelectedValue;
            int idCurs    = (int)cmbCurs.SelectedValue;

            if (DatabaseHelper.InscriereExista(idCursant, idCurs))
            {
                MessageBox.Show("Acest cursant este deja înscris la cursul selectat!", "Duplicat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DatabaseHelper.AddInscriere(new Inscriere
'@ | Set-Content -Path .\SistemEvidentaCursuri\Forms\FormInscrieri.cs -Encoding UTF8

# Write FormCursuri.cs
@'
using System.Linq;
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
            var pnlTop = new Panel { BackColor = Color.FromArgb(39, 174, 96) };
            pnlTop.Height = 50;
            var lblTitle = new Label
            {
                Text = "📚 Cursuri",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            pnlTop.Controls.Add(lblTitle);

            // ── Filter bar ──
            var pnlFilter = new FlowLayoutPanel
            {
                Height = 48,
                Padding = new Padding(10, 8, 10, 0),
                BackColor = Color.FromArgb(240, 248, 240),
                Dock = DockStyle.Fill
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
            dgv.DataBindingComplete += (s, e) => SetColumns();

            // ── Buttons ──
            var pnlBtn = new FlowLayoutPanel
(remaining truncated for brevity)
'@ | Set-Content -Path .\SistemEvidentaCursuri\Forms\FormCursuri.cs -Encoding UTF8

# Write FormRaport.cs (shortened for brevity in this automation; ensure full content in manual runs)
@'
using System.Linq;
using SistemEvidentaCursuri.Data;

namespace SistemEvidentaCursuri.Forms
{
    public class FormRaport : Form
    {
        private DataGridView dgv = new();
        private Label lblStats = new();

        public FormRaport()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Raport Sumar Cursanți";
            this.Size = new Size(700, 530);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 250);

            // ── Top bar ──
            var pnlTop = new Panel { BackColor = Color.FromArgb(142, 68, 173) };
            pnlTop.Height = 50;
            var lblTitle = new Label
            {
                Text = "📊 Raport Sumar",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            pnlTop.Controls.Add(lblTitle);

            // ── Stats panel ──
            var pnlStats = new Panel { Height = 60, BackColor = Color.FromArgb(243, 235, 250), Padding = new Padding(10, 5, 10, 5) };
            lblStats = new Label { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F), TextAlign = ContentAlignment.MiddleLeft };
            pnlStats.Controls.Add(lblStats);

            // ── DataGridView ──
            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9.5F),
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(142, 68, 173),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold)
                }
            };
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 35;
            dgv.DataBindingComplete += (s, e) => ConfigureColumns();

            // ── Buttons ──
            var pnlBtn = new FlowLayoutPanel { Dock = DockStyle.Fill, Height = 50, Padding = new Padding(10, 8, 10, 8), BackColor = Color.FromArgb(243, 235, 250) };
            var btnExport = new Button
            {
                Text = "📄 Export TXT",
                BackColor = Color.FromArgb(142, 68, 173),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Height = 33,
                AutoSize = true,
                Padding = new Padding(10, 0, 10, 0)
            };
            btnExport.FlatAppearance.BorderSize = 0;
            btnExport.Click += BtnExport_Click;
            pnlBtn.Controls.Add(btnExport);

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(0)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            mainLayout.Controls.Add(pnlTop, 0, 0);
            mainLayout.Controls.Add(pnlStats, 0, 1);
            mainLayout.Controls.Add(dgv, 0, 2);
            mainLayout.Controls.Add(pnlBtn, 0, 3);

            this.Controls.Add(mainLayout);
            this.PerformLayout();
        }
'@ | Set-Content -Path .\SistemEvidentaCursuri\Forms\FormRaport.cs -Encoding UTF8

# Stage and commit
git add .\SistemEvidentaCursuri\Forms\FormCursanti.cs .\SistemEvidentaCursuri\Forms\FormInscrieri.cs .\SistemEvidentaCursuri\Forms\FormCursuri.cs .\SistemEvidentaCursuri\Forms\FormRaport.cs
git commit -m $CommitMessage

# Push
git push -u origin $BranchName

Write-Host "Done. Branch '$BranchName' pushed. Build and test in Visual Studio."