using System;
using System.Drawing;
using System.Windows.Forms;

namespace DoTuna.Features.Export
{
    public partial class ExportForm : Form
    {
        private DataGridView ThreadListGrid;
        private Button ExportFileButton;
        private Button OpenConverterButton;
        private TextBox FilterAuthorInputField;
        private TextBox FilterTitleInputField;
        private CheckBox SelectAllCheckBox;
        private TextBox ResultPathField;
        private Button SettingButton;
        
        private void InitializeComponent()
        {
            this.ThreadListGrid = new DataGridView
            {
                Name = "ThreadListGrid",
                AllowUserToAddRows = false,
                Dock = DockStyle.Fill,
                Visible = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false,
                VirtualMode = true,
            };

            // 데이터 그리드 컬럼 추가
            var colThreadName = new DataGridViewTextBoxColumn
            {
                HeaderText = "스레드 이름",
                DataPropertyName = "title",
                ReadOnly = true,
                Name = "ThreadName",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };

            var colUserName = new DataGridViewTextBoxColumn
            {
                HeaderText = "유저 이름",
                DataPropertyName = "username",
                ReadOnly = true,
                Name = "UserName",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            var colCheck = new DataGridViewCheckBoxColumn
            {
                HeaderText = "",
                Width = 30,
                Name = "IsCheck"
            };

            this.ThreadListGrid.Columns.AddRange(new DataGridViewColumn[]
            {
                colThreadName,
                colUserName,
                colCheck
            });

            // 내보내기 버튼과 관련 입력 필드들을 담은 FlowLayoutPanel 설정
            var flowLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                AutoSize = true,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(5),
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
            };

            // 내보내기 버튼
            this.ExportFileButton = new Button
            {
                Text = "내보내기",
                Width = 120,
                Margin = new Padding(0, 0, 5, 0),
                Padding = new Padding(10, 0, 10, 0)
            };
            this.ExportFileButton.Click += new EventHandler(this.ExportButtonClick);

            this.OpenConverterButton = new Button
            {
                Text = "변환기",
                Width = 120,
                Margin = new Padding(0, 0, 5, 0),
                Padding = new Padding(10, 0, 10, 0)
            };
            this.OpenConverterButton.Click += new EventHandler(this.OpenConverterButtonClick);
            
            // 필터 텍스트 박스
            this.FilterTitleInputField = new TextBox { Width = 200 };
            this.FilterAuthorInputField = new TextBox { Width = 200 };
            
            // 체크박스
            this.SelectAllCheckBox = new CheckBox { Text = "전체 선택" };

            this.SettingButton = new Button
            {
                Text = "설정",
                Width = 80,
                Margin = new Padding(0, 0, 5, 0),
                Padding = new Padding(10, 0, 10, 0)
            };
            this.SettingButton.Click += new EventHandler(this.OnSettingButtonClick);

            // 내보내기 결과 경로 텍스트 필드
            this.ResultPathField = new TextBox
            {
                Dock = DockStyle.Bottom,
                Margin = new Padding(5),
                Width = 0 // 실제 배치에 맞게 조정 가능
            };

            // flowLayout에 컨트롤 추가
            flowLayout.Controls.Add(this.ExportFileButton);
            flowLayout.Controls.Add(this.OpenConverterButton);
            flowLayout.Controls.Add(this.FilterTitleInputField);
            flowLayout.Controls.Add(this.FilterAuthorInputField);
            flowLayout.Controls.Add(this.SelectAllCheckBox);
            flowLayout.Controls.Add(this.SettingButton);

            // 메인 패널 설정
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };
            mainPanel.Controls.Add(this.ThreadListGrid);

            this.Text = "DoTuna - 내보내기";
            this.MinimumSize = new Size(900, 450);
            this.ClientSize = new Size(800, 450);

            // 폼에 패널과 하단 컨트롤 추가
            this.Controls.Add(mainPanel);
            this.Controls.Add(this.ResultPathField);
            this.Controls.Add(flowLayout);
        }
    }
}
