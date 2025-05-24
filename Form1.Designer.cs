namespace DoTuna
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Button GetFolderButton;
        private System.Windows.Forms.DataGridView ThreadListGrid;
        private System.Windows.Forms.CheckBox SelectAllCheckBox;
        private System.Windows.Forms.Button GetThreadSourceFileButton;
        private System.Windows.Forms.Button ExportFileButton;
        private System.Windows.Forms.Panel ReadyButtonPanel;
        private System.Windows.Forms.Panel RunningButtonPanel;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.TitleLabel = new System.Windows.Forms.Label();
            this.GetFolderButton = new System.Windows.Forms.Button();
            this.ThreadListGrid = new System.Windows.Forms.DataGridView();
            this.SelectAllCheckBox = new System.Windows.Forms.CheckBox();
            this.GetThreadSourceFileButton = new System.Windows.Forms.Button();
            this.ExportFileButton = new System.Windows.Forms.Button();
            this.ReadyButtonPanel = new System.Windows.Forms.Panel();
            this.RunningButtonPanel = new System.Windows.Forms.Panel();

            // 
            // TitleLabel
            // 
            this.TitleLabel.Text = "Dotuna: 스레드 추출하기";
            this.TitleLabel.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TitleLabel.Location = new System.Drawing.Point(0, 10);
            this.TitleLabel.AutoSize = true;

            // 
            // GetFolderButton
            // 
            this.GetFolderButton.Name = "GetFolderButton";
            this.GetFolderButton.Text = "폴더 가져오기";
            this.GetFolderButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GetFolderButton.Click += new System.EventHandler(this.OnGetFolderClick);

            // 
            // ThreadListGrid (DataGridView)
            // 
            this.ThreadListGrid.Name = "ThreadListGrid";
            this.ThreadListGrid.AllowUserToAddRows = false;
            this.ThreadListGrid.Visible = false;
            this.ThreadListGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ThreadListGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ThreadListGrid.MultiSelect = false;
            this.ThreadListGrid.AutoGenerateColumns = false;

            // Define columns like WPF DataGrid
            var colThreadName = new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                HeaderText = "스레드 이름",
                DataPropertyName = "title",
                ReadOnly = true,
                AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
            };
            var colUserName = new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                HeaderText = "유저 이름",
                DataPropertyName = "username",
                ReadOnly = true,
                AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
            };
            var colCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn
            {
                HeaderText = "",
                Width = 30,
                DataPropertyName = "IsCheck"
            };
            this.ThreadListGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                colThreadName,
                colUserName,
                colCheck
            });

            // SelectAllCheckBox 위치는 별도 컨트롤로 구현해야 하므로 아래 패널에 배치 예정

            // 
            // ReadyButtonPanel
            // 
            this.ReadyButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ReadyButtonPanel.Height = 40;
            this.ReadyButtonPanel.Controls.Add(this.GetThreadSourceFileButton);

            // 
            // RunningButtonPanel
            // 
            this.RunningButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.RunningButtonPanel.Height = 40;
            this.RunningButtonPanel.Visible = false;
            this.RunningButtonPanel.Controls.Add(this.ExportFileButton);

            // 
            // GetThreadSourceFileButton
            // 
            this.GetThreadSourceFileButton.Text = "스레드 원본 파일 받기";
            this.GetThreadSourceFileButton.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.GetThreadSourceFileButton.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.GetThreadSourceFileButton.Click += new System.EventHandler(this.OnGetThreadSourceFileClick);

            // 
            // ExportFileButton
            // 
            this.ExportFileButton.Text = "내보내기";
            this.ExportFileButton.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.ExportFileButton.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.ExportFileButton.Click += new System.EventHandler(this.ExportButtonClick);

            // 
            // SelectAllCheckBox
            // 
            this.SelectAllCheckBox.Text = "전체 선택";
            this.SelectAllCheckBox.AutoSize = true;
            this.SelectAllCheckBox.CheckedChanged += new System.EventHandler(this.OnSelectAllCheckBoxClick);
            this.SelectAllCheckBox.Dock = System.Windows.Forms.DockStyle.Top;

            // 
            // Form1 (this)
            // 
            this.Text = "DoTuna";
            this.MinimumSize = new System.Drawing.Size(450, 450);
            this.ClientSize = new System.Drawing.Size(800, 450);

            // Layout controls
            // TitleLabel top-left
            this.Controls.Add(this.TitleLabel);

            // Create a panel to hold GetFolderButton and ThreadListGrid to mimic Grid with Margin
            var mainPanel = new System.Windows.Forms.Panel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                Padding = new System.Windows.Forms.Padding(5, 50, 5, 50),
                AllowDrop = true
            };
            mainPanel.DragEnter += (s, e) => { if (e.Data.GetDataPresent(System.Windows.Forms.DataFormats.FileDrop)) e.Effect = System.Windows.Forms.DragDropEffects.Copy; else e.Effect = System.Windows.Forms.DragDropEffects.None; };

            // Inside mainPanel: 
            // Place GetFolderButton filling area initially
            mainPanel.Controls.Add(this.GetFolderButton);
            // Place ThreadListGrid (hidden initially)
            mainPanel.Controls.Add(this.ThreadListGrid);
            // Place SelectAllCheckBox on top of ThreadListGrid
            this.ThreadListGrid.Controls.Add(this.SelectAllCheckBox);

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
