using System;
using System.Drawing;
using System.Windows.Forms;
using Zuby.ADGV;

namespace DoTuna
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private Label TitleLabel;
        private Button GetFolderButton;
        private AdvancedDataGridView ThreadListGrid;  // 변경
        private Button SelectAllButton;
        private Button GetThreadSourceFileButton;
        private Button ExportFileButton;
        private Panel ReadyButtonPanel;
        private Panel RunningButtonPanel;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.TitleLabel = new Label();
            this.GetFolderButton = new Button();
            this.ThreadListGrid = new AdvancedDataGridView();  // 변경
            this.SelectAllCheckBox = new CheckBox();
            this.GetThreadSourceFileButton = new Button();
            this.ExportFileButton = new Button();
            this.ReadyButtonPanel = new Panel();
            this.RunningButtonPanel = new Panel();

            // 
            // TitleLabel
            // 
            this.TitleLabel.Text = "Dotuna: 스레드 추출하기";
            this.TitleLabel.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point);
            this.TitleLabel.Location = new Point(0, 10);
            this.TitleLabel.AutoSize = true;

            // 
            // GetFolderButton
            // 
            this.GetFolderButton.Name = "GetFolderButton";
            this.GetFolderButton.Text = "폴더 가져오기";
            this.GetFolderButton.Dock = DockStyle.Fill;
            this.GetFolderButton.Click += new EventHandler(this.OnGetFolderClick);

            // 
            // ThreadListGrid (AdvancedDataGridView)
            // 
            this.ThreadListGrid.Name = "ThreadListGrid";
            this.ThreadListGrid.AllowUserToAddRows = false;
            this.ThreadListGrid.Visible = false;
            this.ThreadListGrid.Dock = DockStyle.Fill;
            this.ThreadListGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.ThreadListGrid.MultiSelect = false;
            this.ThreadListGrid.AutoGenerateColumns = false;
            this.ThreadListGrid.FilterAndSortEnabled = true;
            
            // Define columns
            var colThreadName = new DataGridViewTextBoxColumn()
            {
                HeaderText = "스레드 이름",
                DataPropertyName = "title",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            var colUserName = new DataGridViewTextBoxColumn()
            {
                HeaderText = "유저 이름",
                DataPropertyName = "username",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            var colCheck = new DataGridViewCheckBoxColumn()
            {
                HeaderText = "",
                Width = 30,
                DataPropertyName = "IsCheck"
            };
            this.ThreadListGrid.Columns.AddRange(new DataGridViewColumn[] {
                colThreadName,
                colUserName,
                colCheck
            });

            // 
            // ReadyButtonPanel
            // 
            this.ReadyButtonPanel.Dock = DockStyle.Bottom;
            this.ReadyButtonPanel.Height = 40;
            this.ReadyButtonPanel.Controls.Add(this.GetThreadSourceFileButton);

            // 
            // RunningButtonPanel
            // 
            this.RunningButtonPanel.Dock = DockStyle.Bottom;
            this.RunningButtonPanel.Height = 40;
            this.RunningButtonPanel.Visible = false;
            this.RunningButtonPanel.Controls.Add(this.ExportFileButton);

            // 
            // GetThreadSourceFileButton
            // 
            this.GetThreadSourceFileButton.Text = "스레드 원본 파일 받기";
            this.GetThreadSourceFileButton.Width = 240;
            this.GetThreadSourceFileButton.Margin = new Padding(0, 0, 5, 0);
            this.GetThreadSourceFileButton.Padding = new Padding(10, 0, 10, 0);
            this.GetThreadSourceFileButton.Click += new EventHandler(this.OnGetThreadSourceFileClick);

            // 
            // ExportFileButton
            // 
            this.ExportFileButton.Text = "내보내기";
            this.ExportFileButton.Width = 120; 
            this.ExportFileButton.Margin = new Padding(0, 0, 5, 0);
            this.ExportFileButton.Padding = new Padding(10, 0, 10, 0);
            this.ExportFileButton.Click += new EventHandler(this.ExportButtonClick);

            // SelectAllButton
            var SelectAllButton = new Button();
            SelectAllButton.Name = "SelectAllButton";
            SelectAllButton.Text = "전체 선택";
            SelectAllButton.AutoSize = true;
            SelectAllButton.Margin = new Padding(0, 0, 5, 0);
            SelectAllButton.Padding = new Padding(10, 0, 10, 0);
            SelectAllButton.Visible = false;
            SelectAllButton.Click += new EventHandler(this.OnSelectAllButtonClick);

            // ReadyButtonPanel에 추가하지 않고 RunningButtonPanel에 내보내기 오른쪽에 추가
            this.RunningButtonPanel.Controls.Add(SelectAllButton);
            this.RunningButtonPanel.Controls.SetChildIndex(SelectAllButton, 0); // 오른쪽에 오도록 순서 조정

            // 
            // Form1 (this)
            // 
            this.Text = "DoTuna";
            this.MinimumSize = new Size(450, 450);
            this.ClientSize = new Size(800, 450);

            // Layout controls
            this.Controls.Add(this.TitleLabel);

            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5, 50, 5, 50),
                AllowDrop = true
            };
            mainPanel.DragEnter += (s, e) => {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.None;
            };

            mainPanel.Controls.Add(this.GetFolderButton);
            mainPanel.Controls.Add(this.ThreadListGrid);
            mainPanel.Controls.Add(this.SelectAllCheckBox);

            this.Controls.Add(mainPanel);
            this.Controls.Add(this.ReadyButtonPanel);
            this.Controls.Add(this.RunningButtonPanel);
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
