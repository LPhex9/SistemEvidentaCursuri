using SistemEvidentaCursuri.Data;

namespace SistemEvidentaCursuri.Forms
{
    public class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Sistem Evidență Cursuri și Înscrieri";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 500);
            this.BackColor = Color.FromArgb(245, 245, 250);

            // ── Menu Strip ──
            var menuStrip = new MenuStrip();
            menuStrip.BackColor = Color.FromArgb(30, 60, 120);
            menuStrip.ForeColor = Color.White;
            menuStrip.Font = new Font("Segoe UI", 10F);

            var mnuCursanti = new ToolStripMenuItem("👤 Cursanți");
            mnuCursanti.ForeColor = Color.White;
            mnuCursanti.Click += (s, e) => { new FormCursanti().ShowDialog(); };

            var mnuCursuri = new ToolStripMenuItem("📚 Cursuri");
            mnuCursuri.ForeColor = Color.White;
            mnuCursuri.Click += (s, e) => { new FormCursuri().ShowDialog(); };

            var mnuInscrieri = new ToolStripMenuItem("📝 Înscrieri");
            mnuInscrieri.ForeColor = Color.White;
            mnuInscrieri.Click += (s, e) => { new FormInscrieri().ShowDialog(); };

            var mnuRaport = new ToolStripMenuItem("📊 Raport");
            mnuRaport.ForeColor = Color.White;
            mnuRaport.Click += (s, e) => { new FormRaport().ShowDialog(); };

            menuStrip.Items.AddRange(new ToolStripItem[] { mnuCursanti, mnuCursuri, mnuInscrieri, mnuRaport });
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // ── Header Panel ──
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(30, 60, 120)
            };
            var lblTitle = new Label
            {
                Text = "Centru de Instruire",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlHeader.Controls.Add(lblTitle);
            this.Controls.Add(pnlHeader);

            // ── Dashboard Panel ──
            var pnlDashboard = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(30),
                BackColor = Color.FromArgb(245, 245, 250)
            };
            pnlDashboard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            pnlDashboard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            pnlDashboard.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            pnlDashboard.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            pnlDashboard.Controls.Add(CreateDashboardButton("👤 Cursanți", "Gestionare cursanți", Color.FromArgb(41, 128, 185), () => new FormCursanti().ShowDialog()), 0, 0);
            pnlDashboard.Controls.Add(CreateDashboardButton("📚 Cursuri", "Gestionare cursuri", Color.FromArgb(39, 174, 96), () => new FormCursuri().ShowDialog()), 1, 0);
            pnlDashboard.Controls.Add(CreateDashboardButton("📝 Înscrieri", "Gestionare înscrieri", Color.FromArgb(230, 126, 34), () => new FormInscrieri().ShowDialog()), 0, 1);
            pnlDashboard.Controls.Add(CreateDashboardButton("📊 Raport Sumar", "Vizualizare rapoarte", Color.FromArgb(142, 68, 173), () => new FormRaport().ShowDialog()), 1, 1);

            this.Controls.Add(pnlDashboard);
        }

        private Panel CreateDashboardButton(string title, string subtitle, Color color, Action onClick)
        {
            var pnl = new Panel
            {
                Margin = new Padding(10),
                BackColor = color,
                Cursor = Cursors.Hand,
                Dock = DockStyle.Fill
            };
            pnl.Click += (s, e) => onClick();

            var lblIcon = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Cursor = Cursors.Hand
            };
            lblIcon.Click += (s, e) => onClick();

            pnl.Controls.Add(lblIcon);
            return pnl;
        }
    }
}
