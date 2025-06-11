using System.Windows.Forms;

namespace DoTuna
{
    partial class ConverterForm : Form
    {

        private TextBox txtMain;
        private TextBox txtAddress;
        private Button btnConvert;
        private Panel panelBottom;
        private void InitializeComponent()
        {
            this.txtMain = new TextBox();
            this.txtAddress = new TextBox();
            this.btnConvert = new Button();
            this.panelBottom = new Panel();
            // 
            // txtMain
            // 
            this.txtMain.Dock = DockStyle.Fill;
            this.txtMain.Multiline = true;
            this.txtMain.Name = "txtMain";
            this.txtMain.ScrollBars = ScrollBars.Vertical;
            // 
            // panelBottom
            // 
            this.panelBottom.Dock = DockStyle.Bottom;
            this.panelBottom.Name = "panelBottom";
            // 
            // txtAddress
            // 
            this.txtAddress.Dock = DockStyle.Left;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Text = "https://example.com";
            // 
            // btnConvert
            // 
            this.btnConvert.Dock = DockStyle.Right;
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Text = "변환!";

            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.txtMain);
            this.Name = "ConverterForm";
            this.Text = "DoTuna - 텍스트 링크 변환";
        }
    }
}