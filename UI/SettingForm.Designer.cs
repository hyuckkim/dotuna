using System;
using System.Windows.Forms;

namespace DoTuna
{
    public partial class SettingForm : Form
    {
        private TableLayoutPanel _table;
        private int _settingRow;
        public void InitializeComponent()
        {
            this.Text = "DoTuna - 설정";
            _table = new TableLayoutPanel
            {
                RowCount = 1,
                ColumnCount = 2,
                AutoSize = true,
                Dock = DockStyle.Top,
                Padding = new Padding(10),
            };

            _table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            _settingRow = 0;
            AddSettingControl("패턴", Setting.Instance.Pattern, v => Setting.Instance.Pattern = v,
                "각 문서의 제목입니다.\n" +
                "{id}, {title}, {name}, {created}, {updated}, {size}가\n" +
                "실제 값으로 대체됩니다.\n\n" +
                "글자 자르기:\n" +
                "{title 10..}  → 앞 10글자만 사용하고 잘리면 '..' 추가\n" +
                "{name _20}    → 뒤 20글자만 사용하고 잘리면 '_' 추가\n" +
                "{user 10_10}  → 앞 10글자, 뒤 10글자 사용\n\n" +
                "예: \"{title} - {name} ({created})\""
            );
            AddSettingControl("이미지 임베딩", Setting.Instance.SingleHTML, (bool v) => Setting.Instance.SingleHTML = v,
                "이미지를 html 문서 안에 포함합니다.\n" +
                "이미지 파일이 따로 생성되지 않아, html 파일만 보관할 수 있습니다.\n" +
                "이미지가 base64로 인코딩되어 이미지의 용량이 33%정도 커집니다.");
            AddSettingControl("css 파일 사용", Setting.Instance.UseCssFile, (bool v) => Setting.Instance.UseCssFile = v,
                "css 파일을 사용합니다.\n" +
                "css 파일이 따로 생성되어 모든 스레드 파일들이 css 파일을 공유합니다.\n" +
                "이로 인해 용량 길이가 아주 매우 진짜 조금 줄어듭니다." +
                "css 파일을 사용하지 않으면, html 문서 안에 css가 포함됩니다.");

            this.Controls.Add(_table);
            this.KeyPreview = true;
            this.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
            };
        }

        // _settingRow는 자동 증가
        void AddSettingControl(string labelText, string value, Action<string> setter, string tooltip)
        {
            var label = new Label { Text = labelText, Anchor = AnchorStyles.Left, AutoSize = true };
            var textbox = new TextBox { Text = value, Width = 300 };
            textbox.TextChanged += (sender, e) => setter(textbox.Text);
            _table.Controls.Add(label, 0, _settingRow);
            _table.Controls.Add(textbox, 1, _settingRow);
            var tip = new ToolTip { AutoPopDelay = 10000, InitialDelay = 500, ReshowDelay = 100 };
            tip.SetToolTip(textbox, tooltip);
            _settingRow++;
        }
        void AddSettingControl(string labelText, bool value, Action<bool> setter, string tooltip)
        {
            var label = new Label { Text = labelText, Anchor = AnchorStyles.Left, AutoSize = true };
            var checkbox = new CheckBox { Checked = value, AutoSize = true };
            checkbox.CheckedChanged += (sender, e) => setter(checkbox.Checked);
            _table.Controls.Add(label, 0, _settingRow);
            _table.Controls.Add(checkbox, 1, _settingRow);
            var tip = new ToolTip { AutoPopDelay = 10000, InitialDelay = 500, ReshowDelay = 100 };
            tip.SetToolTip(checkbox, tooltip);
            _settingRow++;
        }
    }
}
