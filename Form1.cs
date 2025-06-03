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
                var repository = new IndexFileRepository();
                await repository.OpenAsync(folderPath);

                threadManager = new ThreadManager(repository);

                EnableExportUI();
                RefreshGrid();
            }
            catch (DirectoryNotFoundException)
            {
                WriteFolderButtonAlert("파일 참조가 잘못되었습니다");
            }
            catch (FileNotFoundException)
            {
                WriteFolderButtonAlert("유효한 아카이빙 폴더가 아닙니다");
            }
            catch (JsonException)
            {
                WriteFolderButtonAlert("index.json 파일이 유효하지 않습니다");
            }
            catch (Exception ex)
            {
                WriteFolderButtonAlert($"예상치 못한 오류: {ex.Message}");
            }
        }

        private void WriteFolderButtonAlert(string message)
        {
            GetFolderButton.Text = $"폴더 가져오기\n({message})";
        }

        private void EnableExportUI()
        {
            ThreadListGrid.Visible = true;
            ReadyButtonPanel.Visible = false;
            RunningButtonPanel.Visible = true;
            GetFolderButton.Visible = false;
        }

        private void OnCheckBoxClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = ThreadListGrid.Rows[e.RowIndex];
            var item = (JsonIndexDocument)row.DataBoundItem;
            threadManager.Check(item, !(row.Cells[e.ColumnIndex]?.Value ?? false));
        }

        private void OnTitleFilterChanged(object sender, EventArgs e)
        {
            threadManager.TitleFilter = this.FilterTitleInputField.Text;
            RefreshGrid();
        }
        private void OnAuthorFilterChanged(object sender, EventArgs e)
        {
            threadManager.AuthorFilter = this.FilterAuthorInputField.Text;
            RefreshGrid();
        }

        private void SelectAllCheckBoxChanged(object sender, EventArgs e)
        {
            if (this.SelectAllCheckBox.Checked)
            {
                foreach(JsonIndexDocument doc in threadManager.Filtered)
                {
                    threadManager.Check(doc, true);
                }
            }
            else
            {
                foreach(JsonIndexDocument doc in threadManager.All)
                {
                    threadManager.Check(doc, false);
                }
            }
            RefreshGrid();
        }
        private void RefreshGrid()
        {
            ThreadListGrid.DataSource = threadManager.Filtered.ToList();
            ThreadListGrid.Refresh();
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
