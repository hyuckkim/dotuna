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
            this.txtMain.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom)
            | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.txtMain.Location = new System.Drawing.Point(12, 12);
            this.txtMain.Multiline = true;
            this.txtMain.Name = "txtMain";
            this.txtMain.ScrollBars = ScrollBars.Vertical;
            this.txtMain.Size = new System.Drawing.Size(360, 180);
            this.txtMain.TabIndex = 0;
            // 
            // panelBottom
            // 
            this.panelBottom.Anchor = ((AnchorStyles)(((AnchorStyles.Bottom | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.panelBottom.Location = new System.Drawing.Point(0, 200);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(384, 50);
            this.panelBottom.TabIndex = 3;
            this.panelBottom.SuspendLayout();
            // 
            // txtAddress
            // 
            this.txtAddress.Anchor = ((AnchorStyles)(((AnchorStyles.Bottom | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.txtAddress.Location = new System.Drawing.Point(12, 215);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(260, 23);
            this.txtAddress.TabIndex = 1;
            this.txtAddress.Text = "https://example.com";
            // 
            // btnConvert
            // 
            this.btnConvert.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            this.btnConvert.Location = new System.Drawing.Point(285, 213);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(87, 27);
            this.btnConvert.TabIndex = 2;
            this.btnConvert.Text = "변환!";
            this.btnConvert.UseVisualStyleBackColor = true;
            // 
            // ConverterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 261);
            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.txtMain);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConverterForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Converter";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}