using System.Windows.Forms;

namespace DoTuna
{
    public partial class SettingForm : Form
    {
        public void InitializeComponent()
        {
            this.Text = "DoTuna - 설정";
            FlowLayoutPanel panel = new FlowLayoutPanel();
            panel.FlowDirection = FlowDirection.TopDown; // 위에서 아래로
            panel.WrapContents = false;                  // 줄 바꿈 방지
            panel.Dock = DockStyle.Fill;                 // 전체 채우기

            TextBox patternTextBox = new TextBox();
            patternTextBox.Text = Setting.Instance.Pattern;
            patternTextBox.Width = 300;
            patternTextBox.TextChanged += (sender, e) =>
            {
                Setting.Instance.Pattern = patternTextBox.Text;
            };
            panel.Controls.Add(patternTextBox);

            this.Controls.Add(panel);
        }
    }
}
