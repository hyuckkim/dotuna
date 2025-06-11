using System;
using System.Drawing;
using System.Windows.Forms;

namespace DoTuna
{
    public partial class ExportForm : Form
    {
        private DataGridView ThreadListGrid;
        private Button GetThreadSourceFileButton;
        private Button ExportFileButton;
        private TextBox FilterAuthorInputField;
        private TextBox FilterTitleInputField;
        private CheckBox SelectAllCheckBox;
        private TextBox DocumentPatternInputField;
        private TextBox ResultPathField;
        private ToolTip PatternToolTip;

        public ExportForm(string resultPath)
        {
            InitializeComponent();
            // 폼 생성 시 export 경로 전달
            ResultPathField.Text = resultPath;
        }
        
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
                AutoGenerateColumns = false
            };
            // 데이터 그리드 컬럼 추가
            var colThreadName = new DataGridViewTextBoxColumn
            {
                HeaderText = "스레드 이름",
                DataPropertyName = "title",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };

            var colUserName = new DataGridViewTextBoxColumn
            {
                HeaderText = "유저 이름",
                DataPropertyName = "username",
                ReadOnly = true,
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
            this.ThreadListGrid.CellContentClick += new DataGridViewCellEventHandler(this.OnCheckBoxClick);

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
            
            // 필터 텍스트 박스
            this.FilterTitleInputField = new TextBox { Width = 200 };
            this.FilterTitleInputField.TextChanged += new EventHandler(this.OnTitleFilterChanged);
            this.FilterAuthorInputField = new TextBox { Width = 200 };
            this.FilterAuthorInputField.TextChanged += new EventHandler(this.OnAuthorFilterChanged);
            
            // 체크박스
            this.SelectAllCheckBox = new CheckBox { Text = "전체 선택" };
            this.SelectAllCheckBox.CheckedChanged += new EventHandler(this.SelectAllCheckBoxChanged);
            
            // 문서 패턴 입력 필드 및 툴팁
            this.DocumentPatternInputField = new TextBox
            {
                Width = 200,
                Text = "{id}",
            };
            this.PatternToolTip = new ToolTip
            {
                AutoPopDelay = 10000,
                InitialDelay = 500,
                ReshowDelay = 100,
            };
            this.PatternToolTip.SetToolTip(this.DocumentPatternInputField,
                "각 문서의 제목입니다.\n" +
                "{id}, {title}, {name}, {created}, {updated}, {size}가\n" +
                "실제 값으로 대체됩니다.\n" +
                "예: \"{title} - {name} ({created})\"");

            // 내보내기 결과 경로 텍스트 필드
            this.ResultPathField = new TextBox
            {
                Dock = DockStyle.Bottom,
                Margin = new Padding(5),
                Width = 0 // 실제 배치에 맞게 조정 가능
            };

            // flowLayout에 컨트롤 추가
            flowLayout.Controls.Add(this.ExportFileButton);
            flowLayout.Controls.Add(this.FilterTitleInputField);
            flowLayout.Controls.Add(this.FilterAuthorInputField);
            flowLayout.Controls.Add(this.SelectAllCheckBox);
            flowLayout.Controls.Add(this.DocumentPatternInputField);

            // 메인 패널 설정
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };
            mainPanel.Controls.Add(this.ThreadListGrid);

            this.Text = "DoTuna - 내보내기";
            this.MinimumSize = new Size(450, 450);
            this.ClientSize = new Size(800, 450);

            // 폼에 패널과 하단 컨트롤 추가
            this.Controls.Add(mainPanel);
            this.Controls.Add(this.ResultPathField);
            this.Controls.Add(flowLayout);
        }
    }
}
