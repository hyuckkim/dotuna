using System.Windows.Forms;

namespace DoTuna
{
    public partial class SettingForm : Form
    {
        public void InitializeComponent()
        {
            this.Text = "DoTuna - 설정";
            var table = new TableLayoutPanel
            {
                RowCount = 1,
                ColumnCount = 2,
                AutoSize = true,
                Dock = DockStyle.Top,
                Padding = new Padding(10),
            };

            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Label
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // TextBox

            var label = new Label { Text = "패턴", Anchor = AnchorStyles.Left, AutoSize = true };
            var textbox = new TextBox { Text = "{id}", Width = 300 };
            textbox.TextChanged += (sender, e) =>
            {
                Setting.Instance.Pattern = textbox.Text;
            };

            table.Controls.Add(label, 0, 0);
            table.Controls.Add(textbox, 1, 0);

            var patternToolTip = new ToolTip
            {
                AutoPopDelay = 10000,
                InitialDelay = 500,
                ReshowDelay = 100,
            };
            patternToolTip.SetToolTip(textbox,
                "각 문서의 제목입니다.\n" +
                "{id}, {title}, {name}, {created}, {updated}, {size}가\n" +
                "실제 값으로 대체됩니다.\n\n" +
                "글자 자르기:\n" +
                "{title 10..}  → 앞 10글자만 사용하고 잘리면 '..' 추가\n" +
                "{name _20}    → 뒤 20글자만 사용하고 잘리면 '_' 추가\n" +
                "{user 10_10}  → 앞 10글자, 뒤 10글자 사용\n\n" +
                "예: \"{title} - {name} ({created})\"");

            this.Controls.Add(table);
        }
    }
}
