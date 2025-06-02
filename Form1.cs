using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoTuna
{
    public partial class Form1 : Form
    {
        public string SourcePath { get; private set; } = string.Empty;
        public ThreadManager threadManager = null!;
        public Form1()
        {
            InitializeComponent();

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
                _ = HandleFolderPath(selectedPath);
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
                    _ = HandleFolderPath(droppedFiles[0]);
                }
            }
        }

        private async Task HandleFolderPath(string folderPath)
        {
            try
            {
                SourcePath = folderPath;
                var repository = new IndexFileRepository();
                await repository.OpenAsync(SourcePath);

                threadManager = new ThreadManager(repository);

                ThreadListGrid.Visible = true;
                ReadyButtonPanel.Visible = false;
                RunningButtonPanel.Visible = true;
                GetFolderButton.Visible = false;

                ThreadListGrid.DataSource = null;
                ThreadListGrid.DataSource = FilteredDoc.ToList();
                ThreadListGrid.Refresh();
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
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || !(ThreadListGrid.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn))
                return;

            var row = ThreadListGrid.Rows[e.RowIndex];
            if (!(row.DataBoundItem is JsonIndexDocument item))
                return;

            item.IsCheck = !item.IsCheck;
        }

        private void OnFilterChanged(object sender, EventArgs e)
        {
            ThreadListGrid.DataSource = FilteredDoc.ToList();
            ThreadListGrid.Refresh();
        }

        private void SelectAllCheckBoxChanged(object sender, EventArgs e)
        {
            if (this.SelectAllCheckBox.Checked)
            {
                foreach(JsonIndexDocument doc in FilteredDoc)
                {
                    doc.IsCheck = true;
                }
            }
            else
            {
                foreach(JsonIndexDocument doc in threadManager.All)
                {
                    doc.IsCheck = false;
                }
            }
            ThreadListGrid.Refresh();
        }
        private IEnumerable<JsonIndexDocument> FilteredDoc
        {
            get
            {
                return threadManager.Filtered(
                    this.FilterTitleInputField.Text,
                    this.FilterAuthorInputField.Text
                );
            }
        }
        private async void ExportButtonClick(object sender, EventArgs e)
        {
            ExportFileButton.Enabled = false;

            var progress = new Progress<string>(message =>
            {
                ExportFileButton.Text = message;
            });

            await new Exporter(this.DocumentPatternInputField.Text).Build(
                SourcePath,
                threadManager.Checked.ToList(),
                progress
            );

            ExportFileButton.Enabled = true;
            ExportFileButton.Text = "내보내기";
        }
    }
}
