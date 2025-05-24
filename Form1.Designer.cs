using System;
using System.Drawing;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace DoTuna
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private Label TitleLabel;
        private Button GetFolderButton;
        private ObjectListView ThreadListGrid;  // 변경: ObjectListView로 교체
        private CheckBox SelectAllCheckBox;
        private Button GetThreadSourceFileButton;
        private Button ExportFileButton;
        private Panel ReadyButtonPanel;
        private Panel RunningButtonPanel;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.TitleLabel = new Label();
            this.GetFolderButton = new Button();
            this.ThreadListGrid = new ObjectListView();  // 변경: ObjectListView로 교체
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
            // ThreadListGrid (ObjectListView)
            // 
            this.ThreadListGrid.Name = "ThreadListGrid";
            this.ThreadListGrid.View = View.Details;
            this.ThreadListGrid.FullRowSelect = true;
            this.ThreadListGrid.MultiSelect = false;
            this.ThreadListGrid.HideSelection = false;
            this.ThreadListGrid.UseCompatibleStateImageBehavior = false;
            this.ThreadListGrid.Dock = DockStyle.Fill;
            this.ThreadListGrid.Visible = false;
            this.ThreadListGrid.CheckBoxes = true;
            this.ThreadListGrid.CellEditUseWholeCell = false;

            // Define columns
            var colThreadName = new OLVColumn("스레드 이름", "title")
            {
                Width = 200,
                FillsFreeSpace = true,
                IsEditable = false
            };
            var colUserName = new OLVColumn("유저 이름", "username")
            {
                Width = 120,
                FillsFreeSpace = true,
                IsEditable = false
            };
            var colCheck = new OLVColumn("", "IsCheck")
            {
                Width = 30,
                CheckBoxes = true,
                TextAlign = HorizontalAlignment.Center
            };
            this.ThreadListGrid.Columns.AddRange(new ColumnHeader[] {
                colThreadName,
                colUserName,
                colCheck
            });
            this.ThreadListGrid.AllColumns.AddRange(new OLVColumn[] {
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

            // 
            // SelectAllCheckBox
            // 
            this.SelectAllCheckBox.Text = "전체 선택";
            this.SelectAllCheckBox.AutoSize = true;
            this.SelectAllCheckBox.CheckedChanged += new EventHandler(this.OnSelectAllCheckBoxClick);
            this.SelectAllCheckBox.Dock = DockStyle.Top;

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
