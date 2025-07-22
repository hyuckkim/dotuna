using System;
using System.Windows.Forms;
using DoTuna.Features.Title;

namespace DoTuna
{
    static class Program
    {
        [STAThread]
        static void Main() => Application.Run(new TitleForm());
    }
}
