using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DoTuna.Core;

namespace DoTuna.Features.Converter
{
    public partial class ConverterForm : Form
    {
        readonly LinkConverterToText converter;
        public ConverterForm(IEnumerable<JsonIndexDocument> files)
        {
            var fileNameMap = new ThreadFileMap(files, AppSetting.Instance.Pattern);
            converter = new LinkConverterToText(fileNameMap, "https://example.com");
            InitializeComponent();
        }

        private void BtnConvert_Click(object sender, EventArgs e)
        {
            txtMain.Text = ConvertInput(txtMain.Text);
        }

        private string ConvertInput(string input)
        {
            converter.Url = txtAddress.Text.Trim();
            return converter.ConvertContent(input, 0UL);
        }
    }
}