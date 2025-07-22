using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DoTuna.Core;
using DoTuna.Features.Converter;
using DoTuna.Features.Setting;

namespace DoTuna.Features.Export
{
    public partial class ExportForm : Form
    {
        private readonly string _sourcePath;
        private ThreadListHelper _listHelper;
        private ThreadSelectionHelper _threadManager;

        public ExportForm(IIndexRepository repository, string sourcePath)
        {
            InitializeComponent();

            _sourcePath = sourcePath;
            _threadManager = new ThreadSelectionHelper(repository);

            _listHelper = new ThreadListHelper(
                _threadManager,
                ThreadListGrid,
                SelectAllCheckBox,
                FilterTitleInputField,
                FilterAuthorInputField);

            ResultPathField.Text = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "result");
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

            var exporter = new Exporter(
                new FileHelper(_sourcePath),
                new FileHelper(ResultPathField.Text)
            );

            try
            {
                await exporter.Build(_threadManager.Checked.ToList(), progress);
            }
            finally
            {
                ExportFileButton.Enabled = true;
            }
        }

        private void OpenConverterButtonClick(object sender, EventArgs e)
        {
            var converterForm = new ConverterForm(_threadManager.Checked);
            converterForm.ShowDialog();
        }

        private bool ValidateBeforeExport()
        {
            if (!_threadManager.Checked.Any())
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

            if (string.IsNullOrWhiteSpace(AppSetting.Instance.Pattern))
            {
                var result = MessageBox.Show("제목 템플릿이 비어 있습니다. 기본값으로 계속하시겠습니까?", "알림", MessageBoxButtons.YesNo);
                if (result != DialogResult.Yes)
                    return false;

                AppSetting.Instance.Pattern = "{id}";
            }

            if (!new ThreadFileMap(_threadManager.Checked, AppSetting.Instance.Pattern).IsUnique)
            {
                MessageBox.Show("제목 템플릿이 잘못되어 생성할 파일 이름이 중복됩니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (Directory.Exists(ResultPathField.Text) && Directory.EnumerateFiles(ResultPathField.Text).Any())
            {
                var result = MessageBox.Show("결과 경로에 기존 파일이 있습니다. " + (
                    File.Exists(Path.Combine(ResultPathField.Text, "index.html"))
                    ? "합치시겠습니까?"
                    : "덮어쓰시겠습니까?"
                    ), "확인", MessageBoxButtons.YesNo);

                if (result != DialogResult.Yes)
                    return false;
            }

            return true;
        }

        private void OnSettingButtonClick(object sender, EventArgs e)
        {
            var settingForm = new SettingForm();
            settingForm.ShowDialog();
        }
    }
}
