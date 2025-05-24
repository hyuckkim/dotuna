using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using DoTuna.Export;
using DoTuna.Thread;
using System.Diagnostics;
using System.Collections.Generic;

namespace DoTuna
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // 이벤트 핸들러 바인딩 (디자이너가 생성하지 않는다면 수동 연결)
            GetFolderButton.Click += OnGetFolderClick;
            GetThreadSourceFileButton.Click += OnGetThreadSourceFileClick;
            ExportFileButton.Click += ExportButtonClick;
            SelectAllCheckBox.Click += OnSelectAllCheckBoxClick;
            ThreadListGrid.CellContentClick += OnCheckBoxClick;

            AllowDrop = true;
            DragEnter += Form1_DragEnter;
            DragDrop += Form1_DragDrop;
        }

        private void OnGetFolderClick(object sender, EventArgs e)
        {
            using var folderDialog = new FolderBrowserDialog
            {
                Description = "폴더 선택"
            };

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                var selectedPath = folderDialog.SelectedPath;
                HandleFolderPath(selectedPath);
            }
        }

        private void OnGetThreadSourceFileClick(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://mega.nz/folder/COpUVSxY#AEbhRcjb2lzLQ0K9t0n9ng/folder/Pb43BLDK",
                UseShellExecute = true
            });
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (droppedFiles.Length > 0 && Directory.Exists(droppedFiles[0]))
                {
                    HandleFolderPath(droppedFiles[0]);
                }
            }
        }

        private void HandleFolderPath(string folderPath)
        {
            try
            {
                ThreadManager.Open(folderPath);
                ThreadListGrid.Visible = true;
                ReadyButtonPanel.Visible = false;
                RunningButtonPanel.Visible = true;

                ThreadListGrid.DataSource = null;
                ThreadListGrid.DataSource = ThreadManager.Index;
            }
            catch (DirectoryNotFoundException)
            {
                GetFolderButton.Text = "폴더 가져오기\n(파일 참조가 잘못되었습니다)";
            }
            catch (FileNotFoundException)
            {
                GetFolderButton.Text = "폴더 가져오기\n(유효한 아카이빙 폴더가 아닙니다)";
            }
            catch (JsonException)
            {
                GetFolderButton.Text = "폴더 가져오기\n(index.json 파일이 유효하지 않습니다)";
            }
            catch (Exception ex)
            {
                GetFolderButton.Text = $"폴더 가져오기\n(예상치 못한 오류: {ex.Message})";
            }
        }

        private void OnCheckBoxClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 &&
                ThreadListGrid.Rows[e.RowIndex].DataBoundItem is JsonIndexDocument item)
            {
                var cell = ThreadListGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (cell is DataGridViewCheckBoxCell)
                {
                    item.IsCheck = !(item.IsCheck ?? false);
                }
            }
        }

        private void OnSelectAllCheckBoxClick(object sender, EventArgs e)
        {
            if (SelectAllCheckBox is CheckBox checkbox)
            {
                bool isChecked = checkbox.Checked;

                if (ThreadListGrid.DataSource is IEnumerable<JsonIndexDocument> items)
                {
                    foreach (var obj in items)
                    {
                        obj.IsCheck = isChecked;
                    }

                    ThreadListGrid.Refresh();
                }
            }
        }

        private async void ExportButtonClick(object sender, EventArgs e)
        {
            ExportFileButton.Enabled = false;

            var progress = new Progress<string>(message =>
            {
                ExportFileButton.Text = message;
            });

            await ExportManager.Build(progress);

            ExportFileButton.Enabled = true;
            ExportFileButton.Text = "내보내기";
        }
    }
}
