using System;
using System.Windows.Forms;

namespace DoTuna
{
    public partial class ConverterForm : Form
    {
        readonly ContentConverterToText converter;
        public ConverterForm(ThreadFileNameMap fileNameMap)
        {
            converter = new ContentConverterToText(fileNameMap, "https://example.com");
            InitializeComponent();
        }

        private void BtnConvert_Click(object sender, EventArgs e)
        {
            txtMain.Text = ConvertInput(txtMain.Text);
        }

        private string ConvertInput(string input)
        {
            converter.Url = txtAddress.Text.Trim();
            return converter.ConvertContent(input, -1);
        }
    }
}