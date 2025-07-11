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
