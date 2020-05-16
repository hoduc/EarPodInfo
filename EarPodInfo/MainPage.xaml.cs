using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EarPodInfo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class MainPage : Page
    {
        //public static DeviceClass BLUE_TOOTH_DEVICE_CLASS = DeviceClass.
        private ObservableCollection<DeviceDisplay> ResultCollection = new ObservableCollection<DeviceDisplay>();

        private DeviceWatcher deviceWatcher = null;


        public MainPage()
        {
            this.InitializeComponent();     
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ConnectedDevicesListView.ItemsSource = ResultCollection;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            StopWatcher();
        }

        private void StopWatcher()
        {
            StopButton.IsEnabled = false;
            if (null != deviceWatcher)
            {
                if ((DeviceWatcherStatus.Started == deviceWatcher.Status ||
                     DeviceWatcherStatus.EnumerationCompleted == deviceWatcher.Status))
                {                    
                    deviceWatcher.Stop();
                }
                deviceWatcher = null;
            }            
            RunButton.IsEnabled = true;
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {         
            StartWatcher();
        }

        private void StartWatcher()
        {
            RunButton.IsEnabled = false;            
            ResultCollection.Clear();
            deviceWatcher = DeviceInformation.CreateWatcher(BluetoothDevice.GetDeviceSelectorFromPairingState(true));
            deviceWatcher.Start();
            deviceWatcher.Added += OnWatcherDeviceAdded;
            deviceWatcher.Updated += OnWatcherUpdated;
            deviceWatcher.Removed += OnWatcherRemoved;
            deviceWatcher.Stopped += OnWatcherStopped;
            StopButton.IsEnabled = true;
        }           

        

        private void OnWatcherDeviceAdded(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            ResultCollection.Add(new DeviceDisplay(deviceInfo));
        }

        private void OnWatcherUpdated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            throw new NotImplementedException();
        }

        private void OnWatcherRemoved(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            throw new NotImplementedException();
        }

        private void OnWatcherStopped(DeviceWatcher sender, object args)
        {
            throw new NotImplementedException();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {                   
            StopWatcher();            
        }

    }

    // In the list view
    public class DeviceDisplay : INotifyPropertyChanged
    {
        private DeviceInformation deviceInfo;

        public DeviceDisplay(DeviceInformation deviceInfoIn)
        {
            deviceInfo = deviceInfoIn;
            UpdateGlyphBitmapImage();
        }

        public DeviceInformation DeviceInformation
        {
            get
            {
                return deviceInfo;
            }

            private set
            {
                deviceInfo = value;
            }
        }

        public string Id
        {
            get
            {
                return deviceInfo.Id;
            }
        }

        public string Name
        {
            get
            {
                return deviceInfo.Name;
            }
        }

        public BitmapImage GlyphBitmapImage
        {
            get;
            private set;
        }

        public void Update(DeviceInformationUpdate deviceInfoUpdate)
        {
            deviceInfo.Update(deviceInfoUpdate);
            UpdateGlyphBitmapImage();
        }

        private async void UpdateGlyphBitmapImage()
        {
            DeviceThumbnail deviceThumbnail = await deviceInfo.GetGlyphThumbnailAsync();
            BitmapImage glyphBitmapImage = new BitmapImage();
            await glyphBitmapImage.SetSourceAsync(deviceThumbnail);
            GlyphBitmapImage = glyphBitmapImage;
            OnPropertyChanged("GlyphBitmapImage");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
