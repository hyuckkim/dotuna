using System;
using System.Drawing;
using System.Windows.Forms;

namespace DoTuna
{
    public partial class MainForm : Form
    {
        private Button GetFolderButton;

        private void InitializeComponent()
        {
            // 폴더 선택 버튼 설정
            this.GetFolderButton = new Button
            {
                Name = "GetFolderButton",
                Text = "폴더 가져오기",
                Dock = DockStyle.Fill
            };
            this.GetFolderButton.Click += new EventHandler(this.OnGetFolderClick);

            // main panel 및 form 설정
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };
            mainPanel.Controls.Add(this.GetFolderButton);

            this.Text = "DoTuna - 폴더 선택";
            this.MinimumSize = new Size(450, 450);
            this.ClientSize = new Size(800, 450);
            this.Controls.Add(mainPanel);
        }
    }
}
