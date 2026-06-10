using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SistemEvidentaCursuri.Data;
=======
﻿using SistemEvidentaCursuri.Data;
>>>>>>> Stashed changes

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

            // ── Main layout ──
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(0)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));   // header
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));   // stats
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // content
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));   // buttons

            mainLayout.Controls.Add(pnlTop, 0, 0);
            mainLayout.Controls.Add(pnlStats, 0, 1);
            mainLayout.Controls.Add(dgv, 0, 2);
            mainLayout.Controls.Add(pnlBtn, 0, 3);

            this.Controls.Add(mainLayout);
            this.PerformLayout();
        }

        private void LoadData()
        {
            var raport = DatabaseHelper.GetRaportSumar();
            var table = new System.Data.DataTable();
            table.Columns.Add("Nume Complet", typeof(string));
            table.Columns.Add("Nr. Înscrieri", typeof(int));
            table.Columns.Add("Sumă Achitată (MDL)", typeof(decimal));

            foreach (var (nume, nr, suma) in raport)
                table.Rows.Add(nume, nr, suma);

            dgv.DataSource = table;

            var (totalCursanti, sumaTotala, medie, cursTop) = DatabaseHelper.GetStatistici();
            lblStats.Text = $"📋 Total cursanți: {totalCursanti}   |   " +
                            $"💰 Sumă totală încasată: {sumaTotala:N2} MDL   |   " +
                            $"📈 Medie/cursant: {medie:N2} MDL   |   " +
                            $"🏆 Cursul top: {cursTop}";
            // column formatting done in DataBindingComplete
        }

        private void ConfigureColumns()
        {
            if (dgv == null || dgv.Columns == null || dgv.Columns.Count == 0) return;
            if (dgv.Columns.Contains("Sumă Achitată (MDL)"))
                dgv.Columns["Sumă Achitată (MDL)"].DefaultCellStyle.Format = "N2";
        }

        private void ConfigureColumns()
        {
            if (dgv == null || dgv.Columns == null || dgv.Columns.Count == 0) return;
            if (dgv.Columns.Contains("Sumă Achitată (MDL)"))
                dgv.Columns["Sumă Achitată (MDL)"].DefaultCellStyle.Format = "N2";
        }

        private void BtnExport_Click(object? sender, EventArgs e)
        {
            using var dlg = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt",
                FileName = $"Raport_Cursanti_{DateTime.Today:yyyyMMdd}.txt"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            var raport = DatabaseHelper.GetRaportSumar();
            var (totalCursanti, sumaTotala, medie, cursTop) = DatabaseHelper.GetStatistici();

            using var sw = new StreamWriter(dlg.FileName, false, System.Text.Encoding.UTF8);
            sw.WriteLine("=================================================");
            sw.WriteLine("       RAPORT SUMAR — CENTRU DE INSTRUIRE        ");
            sw.WriteLine($"       Data: {DateTime.Now:dd.MM.yyyy HH:mm}   ");
            sw.WriteLine("=================================================");
            sw.WriteLine();
            sw.WriteLine($"{"Nume Complet",-30} {"Nr. Înscrieri",12} {"Sumă Achitată",18}");
            sw.WriteLine(new string('-', 65));
            foreach (var (nume, nr, suma) in raport)
                sw.WriteLine($"{nume,-30} {nr,12} {suma,17:N2} MDL");
            sw.WriteLine(new string('=', 65));
            sw.WriteLine();
            sw.WriteLine("STATISTICI GENERALE:");
            sw.WriteLine($"  Total cursanți      : {totalCursanti}");
            sw.WriteLine($"  Sumă totală încasată: {sumaTotala:N2} MDL");
            sw.WriteLine($"  Medie per cursant   : {medie:N2} MDL");
            sw.WriteLine($"  Cursul cel mai popular: {cursTop}");

            MessageBox.Show($"Raport exportat cu succes!\n{dlg.FileName}", "Export reușit", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
