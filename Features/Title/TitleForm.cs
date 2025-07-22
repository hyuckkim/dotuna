using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace DoTuna.Features.Title
{
    public partial class TitleForm : Form
    {
        public TitleForm()
        {
            InitializeComponent();

            DragDropHelper.RegisterDragDropEvents(this, HandleFolderPath);
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

        private async Task HandleFolderPath(string folderPath)
        {
            try
            {
                var repository = new IndexFileRepository();
                await repository.Open(folderPath);

                var exportForm = new ExportForm(repository, folderPath);
                exportForm.Show();
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

        private void OnGetThreadSourceFileClick(object sender, EventArgs e)
        {
            var url = "https://mega.nz/folder/COpUVSxY#AEbhRcjb2lzLQ0K9t0n9ng/folder/Pb43BLDK";
            try
            {
                Clipboard.SetText(url);
            }
            catch (Exception) { }
        }
    }
}
