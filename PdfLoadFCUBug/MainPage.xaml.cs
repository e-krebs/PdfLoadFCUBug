using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Data.Pdf;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PdfLoadFCUBug
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string TestFileName = "TestPdf.pdf";
        public MainPage()
        {
            InitializeComponent();
            MainTextBlock.Text = "Please click on the START button above to start";
        }

        public async Task InitializeAsync()
        {
            MainTextBlock.Text = string.Empty;

            // deleting file from a previous test
            Delete();

            // copying file from app internal folder to LocalState folder
            AppendText("copying Testpdf into app LocalState Folder");
            StorageFolder assetsFolder = await Package.Current.InstalledLocation.GetFolderAsync(@"Assets");
            StorageFile testFile = await assetsFolder.GetFileAsync(TestFileName);
            await testFile.CopyAsync(ApplicationData.Current.LocalFolder);
            AppendText($" - OK {Environment.NewLine}Testpdf copied in path {ApplicationData.Current.LocalFolder.Path}", false);

            // opening file with PdfDocument.LoadFromFileAsync
            AppendText("opening pdf with PdfDocument.LoadFromFileAsync");
            StorageFile targetFile = await ApplicationData.Current.LocalFolder.GetFileAsync(TestFileName);
            await PdfDocument.LoadFromFileAsync(targetFile);
            AppendText(" - OK", false);

            AppendText(Environment.NewLine, false);
            AppendText("now, clicking on the 'DELETE File' button should delete the file we copied in LocalState and opened");
            AppendText("if your version of Windows is Fall Creators Update (1709), you'll notice it crahes the app after a few seconds");
            AppendText("the Exception says the file is being used by another process");
            AppendText("this is not normal");
        }

        public void Delete()
        {
            // deleting file
            string destTestFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, TestFileName);
            if (File.Exists(destTestFilePath))
            {
                AppendText("Deleting file from previous test");
                File.Delete(destTestFilePath);
                AppendText(" - OK", false);
            }
        }

        private async void Start_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            await InitializeAsync();
        }

        private void Delete_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            Delete();
        }

        private void AppendText(string text, bool newLine = true, TextBlock textBlock = null)
        {
            if (textBlock == null) textBlock = MainTextBlock;
            if (string.IsNullOrWhiteSpace(textBlock.Text))
            {
                textBlock.Text = text;
            }
            else
            {
                textBlock.Text = textBlock.Text
                    + (newLine ? Environment.NewLine : string.Empty)
                    + text;
            }
        }
    }
}
