using System;
using System.Windows.Forms;

namespace DoTuna
{
    public partial class SettingForm : Form
    {
        private TableLayoutPanel _table;
        private ToolTip _tip = new ToolTip { AutoPopDelay = 10000, InitialDelay = 500, ReshowDelay = 100 };
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
        (Control, Control) AddSettingControl(string labelText, string value, Action<string> setter)
        {
            var label = new Label { Text = labelText, Anchor = AnchorStyles.Left, AutoSize = true };
            var textbox = new TextBox { Text = value, Dock = DockStyle.Fill };
            textbox.TextChanged += (sender, e) => setter(textbox.Text);

            _table.Controls.Add(label, 0, _settingRow);
            _table.Controls.Add(textbox, 1, _settingRow);
            _settingRow++;
            return (label, textbox);
        }
        (Control, Control) AddSettingControl(string labelText, bool value, Action<bool> setter)
        {
            var label = new Label { Text = labelText, Anchor = AnchorStyles.Left, AutoSize = true };
            var checkbox = new CheckBox { Checked = value, AutoSize = true };
            checkbox.CheckedChanged += (sender, e) => setter(checkbox.Checked);

            _table.Controls.Add(label, 0, _settingRow);
            _table.Controls.Add(checkbox, 1, _settingRow);
            _settingRow++;
            return (label, checkbox);
        }

        class NumericProp
        {
            public int min = 0;
            public int max = 1000000;
        }
        (Control, Control) AddSettingControl(string labelText, int value, Action<int> setter, NumericProp prop = null)
        {
            if (prop == null) prop = new NumericProp();

            var label = new Label { Text = labelText, Anchor = AnchorStyles.Left, AutoSize = true };
            var numericUpDown = new NumericUpDown { Value = value, Minimum = prop.min, Maximum = prop.max, Dock = DockStyle.Fill };
            numericUpDown.ValueChanged += (sender, e) => setter((int)numericUpDown.Value);

            _table.Controls.Add(label, 0, _settingRow);
            _table.Controls.Add(numericUpDown, 1, _settingRow);
            _settingRow++;
            return (label, numericUpDown);
        }
        void AddTooltip((Control a, Control b) controls, string text)
        {
            _tip.SetToolTip(controls.a, text);
            _tip.SetToolTip(controls.b, text);
        }
    }
}
