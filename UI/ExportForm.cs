using System;
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

            // ExportForm 전용 초기화 작업 (예: 그리드 내용 갱신)
            RefreshGrid();
            SetCheckAllBox();
        }

        private void OnCheckBoxClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var row = ThreadListGrid.Rows[e.RowIndex];
            if (!(row?.DataBoundItem is JsonIndexDocument item))
                return;

            threadManager.Toggle(item);
            SetCheckAllBox();
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
            RefreshGrid();

            // 그리드의 체크박스 상태를 업데이트
            foreach (DataGridViewRow row in ThreadListGrid.Rows)
            {
                if (row.DataBoundItem is JsonIndexDocument doc)
                {
                    row.Cells["IsCheck"].Value = threadManager.IsChecked(doc);
                }
            }
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

            exporter.ResultPath = ResultPathField.Text;
            exporter.TitleTemplate = DocumentPatternInputField.Text;
            await exporter.Build(
                threadManager.Checked.ToList(),
                progress
            );

            ExportFileButton.Enabled = true;
        }
    }
}
