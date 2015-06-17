using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Microsoft.Band;
using Microsoft.Band.Sensors;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;
using Newtonsoft.Json;

namespace MemoDemo.WinPhone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private IBandClient bandClient;
        private BandTile myTile;
        private bool pedometerRunning = false;
        private double totalSteps = 0;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.

        }


        private async void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            StatusMessage.Text = "Running ...";

            try
            {
                // Get the list of Microsoft Bands paired to the phone/tablet/PC.
                IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
                if (pairedBands.Length < 1)
                {
                    StatusMessage.Text = "This sample app requires a Microsoft Band paired to your device. Also make sure that you have the latest firmware installed on your Band, as provided by the latest Microsoft Health app.";
                    return;
                }

                // Connect to Microsoft Band.
                bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]);

                Guid myTileId = new Guid("D781F673-6D05-4D69-BCFF-EA7E706C3418");
                myTile = new BandTile(myTileId)
                {
                    Name = "Jokes App",
                    TileIcon = await LoadIcon("ms-appx:///Assets/SampleTileIconLarge.png"),
                    SmallIcon = await LoadIcon("ms-appx:///Assets/SampleTileIconSmall.png")
                };
                myTile.AdditionalIcons.Add(await LoadIcon("ms-appx:///Assets/SampleTileIconWeather.png"));
                myTile.PageLayouts.Clear();
                myTile.PageLayouts.Add(BandLayout.Load());

                var panel = new ScrollFlowPanel
                {
                    Rect = new PageRect(0, 0, 245, 102),
                    Orientation = FlowPanelOrientation.Vertical
                };

                panel.Elements.Add(new Microsoft.Band.Tiles.Pages.TextBlock
                {
                    ElementId = 11,
                    Rect = new PageRect(0, 0, 245, 102),
                    Margins = new Margins(15, 0, 15, 0),
                    Color = new BandColor(0xFF, 0xFF, 0xFF),
                    Font = TextBlockFont.Small
                });

                var pageLayout = new PageLayout(panel);
                myTile.PageLayouts.Add(pageLayout);

                await bandClient.TileManager.RemoveTileAsync(myTile.TileId);
                await bandClient.TileManager.AddTileAsync(myTile);

                StatusMessage.Text = "Done. Check the Tile on your Band (it's the last Tile).";
            }
            catch (Exception ex)
            {
                StatusMessage.Text = ex.ToString();
            }
        }

        private async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (bandClient == null) { return; }

            await bandClient.TileManager.RemoveTileAsync(myTile.TileId);
        }

        private async void StartPedButton_Click(object sender, RoutedEventArgs e)
        {
            await InitPedometerTracking();
        }

        private async void StopPedButton_Click(object sender, RoutedEventArgs e)
        {
            if (bandClient == null) { return; }
            if (!pedometerRunning) { return; }

            pedometerRunning = false;
            await bandClient.SensorManager.Pedometer.StopReadingsAsync();
            await bandClient.SensorManager.Accelerometer.StopReadingsAsync();
            StepsTextBlock.Text = "Steps: -";
            totalSteps = 0;
        }

        private async Task<BandIcon> LoadIcon(string uri)
        {
            StorageFile imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));

            using (IRandomAccessStream fileStream = await imageFile.OpenAsync(FileAccessMode.Read))
            {
                WriteableBitmap bitmap = new WriteableBitmap(1, 1);
                await bitmap.SetSourceAsync(fileStream);
                return bitmap.ToBandIcon();
            }
        }

        private async Task InitPedometerTracking()
        {
            if (bandClient == null) { return; }
            if (pedometerRunning) { return; }

            // Subscribe to Pedometer data.
            bandClient.SensorManager.Pedometer.ReadingChanged += Pedometer_ReadingChanged;
            await bandClient.SensorManager.Pedometer.StartReadingsAsync();
            pedometerRunning = true;
        }

        private async void Pedometer_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandPedometerReading> e)
        {
            if (e.SensorReading.TotalSteps - totalSteps > 10)
            {
                totalSteps = e.SensorReading.TotalSteps;
                Debug.WriteLine("ShowNewJoke success: " + await ShowNewJoke());

                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => { StepsTextBlock.Text = "Steps: JOKE"; });
            }
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => { StepsTextBlock.Text = "Steps: " + totalSteps; });
            Debug.WriteLine("Steps: " + totalSteps);
        }

        private async Task<bool> ShowNewJoke()
        {
            if (bandClient == null) { return false; }
            if (myTile == null) { return false; }

            var client = new HttpClient();
            string jokeJson = await client.GetStringAsync("http://api.icndb.com/jokes/random?limitTo=[nerdy]");

            var jokeObj = JsonConvert.DeserializeObject<JokeContainer>(jokeJson);

            var msg = jokeObj.value.joke;
            if (msg.Length > 192)
            {
                msg = msg.Remove(192);
            }

            var pageData = new PageData(
                Guid.NewGuid(), // the Id for the page
                0,
                new WrappedTextBlockData(11, msg));

            await bandClient.NotificationManager.ShowDialogAsync(myTile.TileId, "New Joke", "...just for you!");
            return await bandClient.TileManager.SetPagesAsync(myTile.TileId, pageData);
        }
    }


    public class JokeContainer
    {
        public string type { get; set; }

        public Joke value { get; set; }
    }

    public class Joke
    {
        public string id { get; set; }
        public string joke { get; set; }
        public List<string> categories { get; set; }
    }
}
