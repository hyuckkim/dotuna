using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using DoTuna.Export;
using DoTuna.Thread;

namespace DoTuna;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnGetFolderClick(object sender, RoutedEventArgs e)
    {
        var folderDialog = new Microsoft.Win32.OpenFolderDialog
        {
            Title = "폴더 선택",
            Multiselect = false,
        };

        if (folderDialog.ShowDialog() == true)
        {
            var selectedPath = folderDialog.FolderName;
            HandleFolderPath(selectedPath);
        }
    }

    private void OnGetThreadSourceFileClick(object sender, RoutedEventArgs e)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = "https://mega.nz/folder/COpUVSxY#AEbhRcjb2lzLQ0K9t0n9ng/folder/Pb43BLDK",
            UseShellExecute = true
        });
    }

    private void OnFolderDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (droppedFiles.Length > 0)
            {
                var folderPath = droppedFiles[0];
                if (Directory.Exists(folderPath))
                {
                    HandleFolderPath(folderPath);
                }
            }
        }
    }


    private void HandleFolderPath(string folderPath)
    {
        try
        {
            ThreadManager.Open(folderPath);
            ThreadListGrid.Visibility = Visibility.Visible;
            ReadyButtonPanel.Visibility = Visibility.Hidden;
            RunningButtonPanel.Visibility = Visibility.Visible;

            ThreadListGrid.ItemsSource = ThreadManager.Index;
        }
        catch (DirectoryNotFoundException)
        {
            GetFolderButton.Content = @"폴더 가져오기
(파일 참조가 잘못되었습니다)";
        }
        catch (FileNotFoundException)
        {
            GetFolderButton.Content = @"폴더 가져오기
(유효한 아카이빙 폴더가 아닙니다)";
        }
        catch (JsonException)
        {
            GetFolderButton.Content = @"폴더 가져오기
(index.json 파일이 유효하지 않습니다)";
        }
        catch (Exception ex)
        {
            GetFolderButton.Content = $@"폴더 가져오기
(예상치 못한 오류: {ex.Message})";
        }   
    }
    private void OnCheckBoxClick(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox && ThreadListGrid.SelectedItem is JsonIndexDocument selectedItem)
        {
            // CheckBox의 상태를 데이터 모델에 반영
            selectedItem.IsCheck = checkBox.IsChecked == true;
        }
    }
    private void OnSelectAllCheckBoxClick(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox selectAllCheckBox) return;

        bool isChecked = selectAllCheckBox.IsChecked == true;

        // ThreadListGrid의 ItemsSource에서 필터링된 항목만 순회하며 IsCheck 값을 설정
        if (ThreadListGrid.ItemsSource is IEnumerable<JsonIndexDocument> items)
        {
            foreach (var item in ThreadListGrid.Items)
            {
                if (item is JsonIndexDocument filteredItem)
                {
                    filteredItem.IsCheck = isChecked;
                }
            }

            // DataGrid를 강제로 업데이트
            ThreadListGrid.Items.Refresh();
        }
    }
    private async void ExportButtonClick(object sender, RoutedEventArgs e)
    {
        ExportFileButton.IsEnabled = false;
        var progress = new Progress<string>(message =>
        {
            ExportFileButton.Content = message;
        });

        await ExportManager.Build(progress);
        ExportFileButton.IsEnabled = true;
        ExportFileButton.Content = "내보내기";

    }
}