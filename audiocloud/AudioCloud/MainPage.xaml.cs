using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AudioCloud
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private String fileName;
        MediaCapture mediaCaptureMgr;
        StorageFile recordStorageFile;

        public MainPage()
        {
            this.InitializeComponent();
            SettingsPane.GetForCurrentView().CommandsRequested += MainPage_CommandsRequested;
            mediaCaptureMgr = new MediaCapture();
        }

        void MainPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            bool afound = false;
            bool sfound = false;
            bool pfound = false;
            foreach (var command in args.Request.ApplicationCommands.ToList())
            {
                if (command.Label == "About")
                {
                    afound = true;
                }
                if (command.Label == "Settings")
                {
                    sfound = true;
                }
                if (command.Label == "Policy")
                {
                    pfound = true;
                }
            }
            if (!afound)
                args.Request.ApplicationCommands.Add(new SettingsCommand("s", "About", (p) => { cfoAbout.IsOpen = true; }));
            //if (!sfound)
            //    args.Request.ApplicationCommands.Add(new SettingsCommand("s", "Settings", (p) => { cfoSettings.IsOpen = true; }));
            //if (!pfound)
            //    args.Request.ApplicationCommands.Add(new SettingsCommand("s", "Policy", (p) => { cfoPolicy.IsOpen = true; }));
            args.Request.ApplicationCommands.Add(new SettingsCommand("privacypolicy", "Privacy policy", OpenPrivacyPolicy));
        }

        private async void OpenPrivacyPolicy(IUICommand command)
        {
            var uri = new Uri("http://www.thatslink.com/privacy-statment/ ");
            await Launcher.LaunchUriAsync(uri);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var settings = new MediaCaptureInitializationSettings {StreamingCaptureMode = StreamingCaptureMode.Audio};
            await mediaCaptureMgr.InitializeAsync(settings);
        }

        private async void UIElement_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            imgRecording.Visibility = Visibility.Visible;
            progress.Visibility = Visibility.Visible;
            Record.IsTapEnabled = false;



            fileName = DateTime.Today.Year + "_" + DateTime.Today.Month + "_" + DateTime.Today.Day + "_" + DateTime.Today.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_" + DateTime.Now.Millisecond + ".m4a";
            recordStorageFile = await Windows.Storage.KnownFolders.MusicLibrary.CreateFileAsync(fileName);

            Windows.Media.MediaProperties.MediaEncodingProfile recordProfile = null;

            recordProfile = Windows.Media.MediaProperties.MediaEncodingProfile.CreateM4a(Windows.Media.MediaProperties.AudioEncodingQuality.Auto);
            
            await mediaCaptureMgr.StartRecordToStorageFileAsync(recordProfile, recordStorageFile);
        }

        private async void ImgRecording_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {

            await mediaCaptureMgr.StopRecordAsync();
            var stream = await recordStorageFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
            
            imgRecording.Visibility = Visibility.Collapsed;
            progress.Visibility = Visibility.Collapsed;
            Record.IsTapEnabled = true;

            new Windows.UI.Popups.MessageDialog("File Recorded and saved successfully.").ShowAsync();
            Frame.Navigate(typeof (RecordedFiles));
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RecordedFiles));
        }
    }
}
