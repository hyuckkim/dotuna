using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoTuna.Features.Title
{
    public static class DragDropHelper
    {
        public static void RegisterDragDropEvents(Control control, Func<string, Task> handleFolderPath)
        {
            control.AllowDrop = true;
            control.DragEnter += (sender, e) => OnDragEnter(e);
            control.DragDrop += async (sender, e) => await OnDragDropAsync(e, handleFolderPath);
        }

        private static void OnDragEnter(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private static async Task OnDragDropAsync(DragEventArgs e, Func<string, Task> handleFolderPath)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            var droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            var folderPath = droppedFiles.FirstOrDefault(Directory.Exists);
            if (folderPath == null)
                return;

            await handleFolderPath(folderPath);
        }
    }
}
