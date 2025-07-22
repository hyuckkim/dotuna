using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DoTuna.Core;

namespace DoTuna
{
    public partial class ConverterForm : Form
    {
        readonly ConverterToText converter;
        public ConverterForm(IEnumerable<JsonIndexDocument> files)
        {
            var fileNameMap = new ThreadFileNameMap(files);
            converter = new ConverterToText(fileNameMap, "https://example.com");
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