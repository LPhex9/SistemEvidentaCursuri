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
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(230, 126, 34) };
            var lblTitle = new Label
            {
                Text = "📝 Înscrieri",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Left,
                Width = 200,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            pnlTop.Controls.Add(lblTitle);
            this.Controls.Add(pnlTop);

            // ── Add form ──
            var pnlAdd = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(253, 243, 233), Padding = new Padding(10, 8, 10, 5) };
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
            this.Controls.Add(pnlAdd);

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
            this.Controls.Add(dgv);

            // ── Bottom ──
            var pnlBtn = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
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
            this.Controls.Add(pnlBtn);
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
            if (dgv.Columns.Contains("IdInscriere"))  dgv.Columns["IdInscriere"].Visible = false;
            if (dgv.Columns.Contains("IdCursant"))    dgv.Columns["IdCursant"].Visible = false;
            if (dgv.Columns.Contains("IdCurs"))       dgv.Columns["IdCurs"].Visible = false;
            if (dgv.Columns.Contains("PretCurs"))     dgv.Columns["PretCurs"].Visible = false;
            if (dgv.Columns.Contains("NumeCursant"))  dgv.Columns["NumeCursant"].HeaderText = "Cursant";
            if (dgv.Columns.Contains("DenumireCurs")) dgv.Columns["DenumireCurs"].HeaderText = "Curs";
            if (dgv.Columns.Contains("DataInscriere")) dgv.Columns["DataInscriere"].HeaderText = "Data Înscrierii";
            if (dgv.Columns.Contains("StatusPlata"))  dgv.Columns["StatusPlata"].HeaderText = "Status Plată";

            int count = data.Count;
            decimal total = data.Where(i => i.StatusPlata == "Achitat").Sum(i => i.PretCurs);
            lblTotal.Text = $"Total: {count} înscrieri | Încasat: {total:N2} MDL";
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
            {
                IdCursant     = idCursant,
                IdCurs        = idCurs,
                DataInscriere = DateTime.Today,
                StatusPlata   = cmbStatus.SelectedItem?.ToString() ?? "Neachitat"
            });
            LoadData();
        }

        private void BtnSterge_Click(object? sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selectați o înscriere.", "Atenție", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            var ins = dgv.SelectedRows[0].DataBoundItem as Inscriere;
            if (ins == null) return;

            if (MessageBox.Show($"Anulați înscrierea lui '{ins.NumeCursant}' la '{ins.DenumireCurs}'?",
                "Confirmare", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DatabaseHelper.DeleteInscriere(ins.IdInscriere);
                LoadData();
            }
        }
    }
}
