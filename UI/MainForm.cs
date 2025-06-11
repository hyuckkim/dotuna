using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoTuna
{
    public partial class MainForm : Form
    {
        public ThreadManager threadManager = null!;
        public Exporter exporter = null!;

        public MainForm()
        {
            InitializeComponent();

            // 폴더 선택을 위한 드래그앤드롭 이벤트 등록
            AllowDrop = true;
            DragEnter += MainForm_DragEnter;
            DragDrop += MainForm_DragDrop;
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

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            var droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            var folderPath = droppedFiles.FirstOrDefault(Directory.Exists);
            if (folderPath == null)
                return;

            _ = HandleFolderPath(folderPath);
        }

        private async Task HandleFolderPath(string folderPath)
        {
            try
            {
                // 인덱스 파일 로드 및 ThreadManager, Exporter 생성
                var repository = new IndexFileRepository();
                await repository.OpenAsync(folderPath);

                threadManager = new ThreadManager(repository);
                exporter = new Exporter
                {
                    SourcePath = folderPath
                };

                // 내보내기 UI용 폼을 생성해 전환 (UI 요소 Visible을 토글하는 대신 분리)
                var exportForm = new ExportForm(threadManager, exporter)
                {
                    Owner = this
                };
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
    }
}
