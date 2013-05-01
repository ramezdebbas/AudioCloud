using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AudioCloud.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace AudioCloud
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class RecordedFiles : AudioCloud.Common.LayoutAwarePage
    {
        public IReadOnlyList<StorageFile> LstFiles { get; set; }
        public RecordedFiles()
        {
            this.InitializeComponent();
            LoadFiles();
        }

        public async void LoadFiles()
        {
           LstFiles= await Windows.Storage.KnownFolders.MusicLibrary.GetFilesAsync();
           lstView.ItemsSource = LstFiles.OrderByDescending(x=>x.DateCreated);
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private async void Play_OnClick(object sender, RoutedEventArgs e)
        {
            
            var listView = sender as Button;
            if (listView != null)
            {
                var recordStorageFile = LstFiles.First(r => r.DisplayName == listView.Tag.ToString());
                 
                if (recordStorageFile != null)
                {
                    var stream = await recordStorageFile.OpenAsync(Windows.Storage.FileAccessMode.Read);

                    playbackElement.AutoPlay = true;
                    playbackElement.SetSource(stream, recordStorageFile.FileType);

                }
            }
            playbackElement.Play(); 
        }

        private void Stop_OnClick(object sender, RoutedEventArgs e)
        {
            playbackElement.Stop();
        }

        private async void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            var listView = sender as Button;
            if (listView != null)
            {
                var recordStorageFile = LstFiles.First(r => r.DisplayName == listView.Tag.ToString());

                if (recordStorageFile != null)
                {
                    MessageDialog md = new MessageDialog("Are you sure you want to delete?", "Delete confirmation");
                    bool? result = null;
                    md.Commands.Add(
                       new UICommand("OK", new UICommandInvokedHandler((cmd) => result = true)));
                    md.Commands.Add(
                       new UICommand("Cancel", new UICommandInvokedHandler((cmd) => result = false)));

                    await md.ShowAsync();
                    if (result == true)
                    {
                        await recordStorageFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                        LstFiles = await Windows.Storage.KnownFolders.MusicLibrary.GetFilesAsync();
                        lstView.ItemsSource = LstFiles.OrderByDescending(x => x.DateCreated);
                    }
                    
                }
            }
        }
    }
}
