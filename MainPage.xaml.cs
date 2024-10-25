using Firebase.Database;
using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace radAssignment2
{
   
    // <summary>
    // An empty page that can be used on its own or navigated to within a Frame.
    // </summary>
    public sealed partial class MainPage : Page
    {
        string buttonName;
        FirebaseStorage storage = new FirebaseStorage("radassignment-59ca9.appspot.com");
        public MainPage()
        {
            this.InitializeComponent();

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await RetrieveRooms();

            await GetPhotosFromFolderAsync();

        }

        private async Task RetrieveRooms()
        {
            FirebaseClient firebaseClient = new FirebaseClient("https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/");

            var query = await firebaseClient
                .Child("Room")
                .OnceAsync<RoomInfo>();

            List<RoomInfo> roomInfoList = new List<RoomInfo>();

            foreach (var item in query)
            {
                string roomName = item.Key;

                int price = item.Object.Price;

                var roomInfo = new RoomInfo
                {
                    Price = price,
                    RoomType = roomName


                };
                roomInfoList.Add(roomInfo);

            }

            roomlist.ItemsSource = roomInfoList;
            pricelist.ItemsSource = roomInfoList;
            
        }



        private async Task GetPhotosFromFolderAsync()
        {
            Debug.WriteLine("in");
            FirebaseClient firebaseClient = new FirebaseClient("https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/");

            var query2 = await firebaseClient
                .Child("Image")
                .OnceAsync<ImageItem>();



            List<ImageItem> allImageTitle = new List<ImageItem>();

            Debug.WriteLine("in");

            foreach (var item2 in query2)
            {
                Debug.WriteLine("in");
                string name = item2.Object.ImageName;
                string url = item2.Object.ImageURL;
                Debug.WriteLine($"Error: {url}");
                var image = new ImageItem
                {
                    ImageName = name,
                    ImageURL = url

                };

                allImageTitle.Add(image);



            }

            galleryGrid.ItemsSource = allImageTitle;


        }


        //commandbar start
        private void bookingAppButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Booking));
        }

        private void profileAppButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Profile));
        }

        private void bookingHistoryAppButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BookingHistory));
        }

        private void feedbackAppButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Feedback));
        }

        private void signoutAppButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(loginpage));
        }

        private void homeAppButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }




        //commandbar end
    }
}
