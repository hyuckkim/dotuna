using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DoTuna
{
    public partial class ExportForm : Form
    {
        private readonly ThreadManager threadManager;
        private readonly Exporter exporter;

        public ExportForm(ThreadManager threadManager, Exporter exporter)
        {
            InitializeComponent();
            this.threadManager = threadManager;
            this.exporter = exporter;
            ResultPathField.Text = exporter.ResultPath;

            RefreshGrid();
            SetCheckAllBox();
        }

        private void OnCheckBoxClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= threadManager.Filtered.Count())
                return;

            threadManager.Toggle(threadManager.Filtered.ElementAt(e.RowIndex));
            SetCheckAllBox();
        }

        private void ThreadListGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= threadManager.Filtered.Count())
                return;

            var doc = threadManager.Filtered.ElementAt(e.RowIndex);
            if (doc == null)
                return;

            switch (ThreadListGrid.Columns[e.ColumnIndex].Name)
            {
                case "IsCheck":
                    e.Value = threadManager.IsChecked(doc);
                    break;
                case "ThreadName":
                    e.Value = doc.title;
                    break;
                case "UserName":
                    e.Value = doc.username;
                    break;
            }
        }

        private void SetCheckAllBox()
        {
            SelectAllCheckBox.CheckedChanged -= SelectAllCheckBoxChanged;
            SelectAllCheckBox.Checked = threadManager.Filtered.All(doc => threadManager.IsChecked(doc));
            SelectAllCheckBox.CheckedChanged += SelectAllCheckBoxChanged;
        }

        private void OnTitleFilterChanged(object sender, EventArgs e)
        {
            threadManager.TitleFilter = FilterTitleInputField.Text;
            RefreshGrid();
            SetCheckAllBox();
        }

        private void OnAuthorFilterChanged(object sender, EventArgs e)
        {
            threadManager.AuthorFilter = FilterAuthorInputField.Text;
            RefreshGrid();
            SetCheckAllBox();
        }

        private void SelectAllCheckBoxChanged(object sender, EventArgs e)
        {
            if (SelectAllCheckBox.Checked)
            {
                foreach (JsonIndexDocument doc in threadManager.Filtered)
                {
                    threadManager.Check(doc);
                }
            }
            else
            {
                foreach (JsonIndexDocument doc in threadManager.All)
                {
                    threadManager.Uncheck(doc);
                }
            }
            ThreadListGrid.Invalidate();
        }

        private void RefreshGrid()
        {
            ThreadListGrid.RowCount = threadManager.Filtered.Count();
            ThreadListGrid.Invalidate();
        }

        private async void ExportButtonClick(object sender, EventArgs e)
        {
            if (!ValidateBeforeExport())
                return;
            ExportFileButton.Enabled = false;

            var progress = new Progress<string>(message =>
            {
                ExportFileButton.Text = message;
            });

            exporter.ResultPath = ResultPathField.Text;
            exporter.TitleTemplate = DocumentPatternInputField.Text;
            try
            {
                await exporter.Build(
                    threadManager.Checked.ToList(),
                    progress
                );
            }
            finally
            {
                ExportFileButton.Enabled = true;
            }
        }
        private void OpenConverterButtonClick(object sender, EventArgs e)
        {
            var fileNameMap = new ThreadFileNameMap(threadManager.Checked.ToList(), DocumentPatternInputField.Text);
            var converterForm = new ConverterForm(fileNameMap);
            converterForm.Show();
        }
        private bool ValidateBeforeExport()
        {
            if (!threadManager.Checked.Any())
            {
                var result = MessageBox.Show("선택된 항목이 없습니다. 계속하시겠습니까?", "경고", MessageBoxButtons.YesNo);
                if (result != DialogResult.Yes)
                    return false;
            }

            if (string.IsNullOrWhiteSpace(ResultPathField.Text))
            {
                MessageBox.Show("결과 경로가 비어 있습니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(DocumentPatternInputField.Text))
            {
                var result = MessageBox.Show("제목 템플릿이 비어 있습니다. 기본값으로 계속하시겠습니까?", "알림", MessageBoxButtons.YesNo);
                if (result != DialogResult.Yes)
                    return false;
                DocumentPatternInputField.Text = "{id}";
            }
            
            var fileNameMap = new ThreadFileNameMap(threadManager.Checked.ToList(), DocumentPatternInputField.Text);
            if (fileNameMap.Size != threadManager.Checked.Count())
            {
                MessageBox.Show("제목 템플릿이 잘못되어 생성할 파일 이름이 중복됩니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (Directory.Exists(ResultPathField.Text) && Directory.EnumerateFiles(ResultPathField.Text).Any())
            {
                var result = MessageBox.Show("결과 경로에 기존 파일이 있습니다. 덮어쓰시겠습니까?", "확인", MessageBoxButtons.YesNo);
                if (result != DialogResult.Yes)
                    return false;
            }

            return true;
        }
    }
}
