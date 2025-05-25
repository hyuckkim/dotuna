using System;
using System.Drawing;
using System.Windows.Forms;

namespace DoTuna
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private Label TitleLabel;
        private Button GetFolderButton;
        private DataGridView ThreadListGrid;
        private Button GetThreadSourceFileButton;
        private Button ExportFileButton;
        private TextBox FilterAuthorInputField;
        private TextBox FilterTitleInputField;
        private CheckBox SelectAllCheckBox;
        private Panel ReadyButtonPanel;
        private Panel RunningButtonPanel;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.TitleLabel = new Label
            {
                Text = "Dotuna: 스레드 추출하기",
                Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point),
                Location = new Point(0, 10),
                AutoSize = true
            };

            this.GetFolderButton = new Button
            {
                Name = "GetFolderButton",
                Text = "폴더 가져오기",
                Dock = DockStyle.Fill
            };
            this.GetFolderButton.Click += new EventHandler(this.OnGetFolderClick);

            this.ThreadListGrid = new DataGridView
            {
                Name = "ThreadListGrid",
                AllowUserToAddRows = false,
                Visible = false,
                Dock = DockStyle.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false
            };

            var colThreadName = new DataGridViewTextBoxColumn
            {
                HeaderText = "스레드 이름",
                DataPropertyName = "title",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };

            var colUserName = new DataGridViewTextBoxColumn
            {
                HeaderText = "유저 이름",
                DataPropertyName = "username",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };

            var colCheck = new DataGridViewCheckBoxColumn
            {
                HeaderText = "",
                Width = 30,
                DataPropertyName = "IsCheck",
                Name = "IsCheck"
            };

            this.ThreadListGrid.Columns.AddRange(new DataGridViewColumn[]
            {
                colThreadName,
                colUserName,
                colCheck
            });

            this.ReadyButtonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40
            };

            this.GetThreadSourceFileButton = new Button
            {
                Text = "스레드 원본 파일 받기",
                Width = 240,
                Margin = new Padding(0, 0, 5, 0),
                Padding = new Padding(10, 0, 10, 0)
            };
            this.GetThreadSourceFileButton.Click += new EventHandler(this.OnGetThreadSourceFileClick);
            this.ReadyButtonPanel.Controls.Add(this.GetThreadSourceFileButton);

            this.RunningButtonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                Visible = false
            };

            this.ExportFileButton = new Button
            {
                Text = "내보내기",
                Width = 120,
                Margin = new Padding(0, 0, 5, 0),
                Padding = new Padding(10, 0, 10, 0)
            };
            this.ExportFileButton.Click += new EventHandler(this.ExportButtonClick);
            this.FilterAuthorInputField = new TextBox
            {
                Width = 200,
            };
            this.FilterAuthorInputField.TextChanged += new EventHandler(this.OnFilterChanged);
            this.FilterTitleInputField = new TextBox
            {
                Width = 200,
            };
            this.FilterTitleInputField.TextChanged += new EventHandler(this.OnFilterChanged);
            this.SelectAllCheckBox = new CheckBox
            {
                Text = "전체 선택",
            };
            this.SelectAllCheckBox.CheckedChanged += new EventHandler(this.SelectAllCheckBoxChanged);
    
            var flowLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(5),
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
            };

            flowLayout.Controls.Add(this.ExportFileButton);
            flowLayout.Controls.Add(this.FilterAuthorInputField);
            flowLayout.Controls.Add(this.FilterTitleInputField);
            flowLayout.Controls.Add(this.SelectAllCheckBox);

            this.RunningButtonPanel.Controls.Add(flowLayout);
        
            this.Text = "DoTuna";
            this.MinimumSize = new Size(450, 450);
            this.ClientSize = new Size(800, 450);

            this.Controls.Add(this.TitleLabel);

            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5, 50, 5, 50),
            };

            mainPanel.Controls.Add(this.GetFolderButton);
            mainPanel.Controls.Add(this.ThreadListGrid);

            this.Controls.Add(mainPanel);
            this.Controls.Add(this.ReadyButtonPanel);
            this.Controls.Add(this.RunningButtonPanel);

            this.TitleLabel.SendToBack();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
