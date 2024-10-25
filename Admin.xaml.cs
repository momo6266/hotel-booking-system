using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace radAssignment2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Admin : Page
    {
        public AdminModel adminModel { get; set; }
        string userId = "-NeClNDrjBA0LSvZPCsM";


        public BookingViewModel ViewModel { get; set; }
        private List<RoomInfo> allRooms = new List<RoomInfo>();
        private string itemToEditID = "";
        public Admin()
        {
            this.InitializeComponent();
            adminModel = new AdminModel();
            ViewModel = new BookingViewModel();
            this.DataContext = ViewModel;


            
        }

        //commandbar start


        private async void feedback_tapped(object sender, TappedRoutedEventArgs e)
        {
            var firebaseDatabaseUrl = "https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/";


            FirebaseClient firebaseclient = new FirebaseClient(firebaseDatabaseUrl);

            try
            {
                var feedbackEntries = await firebaseclient.Child("Feedbacks").Child(userId).OnceAsync<FeedbackModel>();

                var feedbackList = new List<FeedbackModel>();

                foreach (var feedback in feedbackEntries)
                {
                    feedbackList.Add(feedback.Object);
                }

                DisplayPreviousFeedbacks(feedbackList);
            }
            catch (Exception ex)
            {
                // Handle any error that occurs during data retrieval.
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"An error occurred: {ex.Message}",
                    CloseButtonText = "OK"
                };

                await errorDialog.ShowAsync();
            }
        }

        private async void DisplayPreviousFeedbacks(List<FeedbackModel> feedbacks)
        {
            string feedbackSummary = "";

            foreach (var feedback in feedbacks)
            {
                feedbackSummary += $"Name: {feedback.NameText}\n";
                feedbackSummary += $"Feedback: {feedback.FeedbackText}\n";
                feedbackSummary += $"Date: {feedback.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")}\n";
                feedbackSummary += "----------------------------------\n";
            }

            ContentDialog feedbackDialog = new ContentDialog
            {
                Title = "Previous Feedbacks",
                Content = feedbackSummary,
                CloseButtonText = "Close"
            };

            await feedbackDialog.ShowAsync();
        }

        private void viewRoom_tapped(object sender, TappedRoutedEventArgs e)
        {
            myGrid.Visibility = Visibility.Visible;
            SelectRoom();
            
            
        }

        private void roomManage_tapped(object sender, TappedRoutedEventArgs e)
        {
            myGrid.Visibility = Visibility.Collapsed;
            Start.Visibility = Visibility.Visible;
            Start1.Visibility = Visibility.Visible;
            Start2.Visibility = Visibility.Visible;
            Start3.Visibility = Visibility.Visible;
        }

        //commandbar end

        private async void DisplayDialog(string title, string content)
        {
            ContentDialog noDialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noDialog.ShowAsync();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await adminModel.LoadRoomTypes();
            

        }

        private async Task DisplayRoomTypeDetailsAsync(string roomTypeName)
        {
            FirebaseClient firebaseClient = new FirebaseClient("https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/");

            // Retrieve room type details based on roomTypeName
            var roomTypeDetails = await firebaseClient
                .Child("Room")
                .Child(roomTypeName)
                .OnceSingleAsync<RoomInfo>();
           
        }

        private async void addButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((!string.IsNullOrEmpty(roomTypetxt.Text)) && (!string.IsNullOrEmpty(roomPricetxt.Text)) && (!string.IsNullOrEmpty(roomLimittxt.Text)))
                {
                    RoomInfo r = new RoomInfo();
                    r.RoomType = roomTypetxt.Text;
                    r.Price = Convert.ToInt32(roomPricetxt.Text);
                    r.Limit = Convert.ToInt32(roomLimittxt.Text);

                    await adminModel.AddRoom(r);
                    roomTypetxt.Text = string.Empty;
                    roomPricetxt.Text = string.Empty;
                    roomLimittxt.Text = string.Empty;
                    DisplayDialog("Success", "Room Type Added Successfully");
                    SelectRoom();


                }
                else
                    DisplayDialog("Input", "Please key in all the information.");


            }
            catch (Exception theException)
            {
                // Handle all other exceptions.
                DisplayDialog("Error", "Error Message: " + theException.Message);
            }
        }
        public ObservableCollection<RoomTypeItem> RoomTypes { get; } = new ObservableCollection<RoomTypeItem>();

        private async void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                string roomType = btn.CommandParameter as string;
                var roomTypeToRemove = RoomTypes.FirstOrDefault(rt => rt.RoomType == roomType);
                if (roomTypeToRemove != null)
                {
                    RoomTypes.Remove(roomTypeToRemove);
                }
                await adminModel.DeleteRoom(roomType);
                DisplayDialog("Success", "Room Type Deleted Successfully");
                SelectRoom();
            }
            catch (Exception theException)
            {
                // Handle all other exceptions.
                DisplayDialog("Error", "Error Message: " + theException.Message);
            }
        }
        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            itemToEditID = btn.Tag.ToString();

            var itemToEdit = allRooms.SingleOrDefault(r => r.RoomType == btn.Tag.ToString());
            if (itemToEdit != null)
            {
                roomTypetxt.Text = itemToEdit.RoomType.ToString();
                roomPricetxt.Text = itemToEdit.Price.ToString();
                roomLimittxt.Text = itemToEdit.Limit.ToString();


            }


        }
        private async void updatedButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((!string.IsNullOrEmpty(roomTypetxt.Text)) && (!string.IsNullOrEmpty(roomPricetxt.Text)) && (!string.IsNullOrEmpty(roomLimittxt.Text)))
                {
                    RoomInfo r = new RoomInfo();
                    r.RoomType = roomTypetxt.Text;
                    r.Price = Convert.ToInt32(roomPricetxt.Text);
                    r.Limit = Convert.ToInt32(roomLimittxt.Text);

                    await adminModel.UpdatePerson(itemToEditID, r);
                    roomTypetxt.Text = string.Empty;
                    roomPricetxt.Text = string.Empty;
                    roomLimittxt.Text = string.Empty;
                    DisplayDialog("Success", "Person Updated Successfully");
                    SelectRoom();


                }
                else
                    DisplayDialog("Input", "Please key in all the information.");
            }
            catch (Exception theException)
            {
                // Handle all other exceptions.Fde
                DisplayDialog("Error", "Error Message: " + theException.Message);
            }

        }

        private Dictionary<string, int> LoadRoomPrices()
        {
            Dictionary<string, int> roomPrices = new Dictionary<string, int>();

            try
            {
                var database = new FirebaseClient("https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/");
                var roomPricesData = database.Child("Room").OnceAsync<RoomInfo>().Result;

                foreach (var roomPrice in roomPricesData)
                {
                    roomPrices.Add(roomPrice.Key, roomPrice.Object.Price);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading room prices: {ex.Message}");
            }

            return roomPrices;
        }

        

        //private async Task<int> GetNumberOfAvailableRooms(string selectedRoomType, DateTime checkInDate, DateTime checkOutDate)
        //{
        //    try
        //    {
        //        var database = new FirebaseClient("https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/");

        //        var roomInfo = await database
        //            .Child("Room")
        //            .Child(selectedRoomType)
        //            .OnceSingleAsync<RoomInfo>();

        //        if (roomInfo == null)
        //        {
        //            return 0;
        //        }

        //        var userBookings = await database
        //            .Child("Record")
        //            .Child(userId)
        //            .Child("History")
        //            .OrderByKey()
        //            .OnceAsync<BookingModel>();

        //        int totalOccupiedRooms = 0;

        //        foreach (var booking in userBookings)
        //        {
        //            var bookingCheckInDate = DateTime.Parse(booking.Object.CheckInDate);
        //            var bookingCheckOutDate = DateTime.Parse(booking.Object.CheckOutDate);

        //            if (!booking.Object.CancelStatus && booking.Object.RoomType == selectedRoomType)
        //            {
        //                if (bookingCheckInDate < checkOutDate && bookingCheckOutDate > checkInDate)
        //                {
        //                    totalOccupiedRooms += booking.Object.NumRooms;
        //                }
        //            }
        //        }

        //        int maxRoomLimit = roomInfo.Limit;

        //        int availableRooms = maxRoomLimit - totalOccupiedRooms;

        //        if (availableRooms < 0)
        //        {
        //            availableRooms = 0;
        //        }

        //        return availableRooms;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }


        //}

        private async void SelectRoom()
        {
            try
            {
                FirebaseClient firebaseClient = new FirebaseClient("https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/");
                allRooms = await adminModel.GetAllRooms();
                lstRooms.ItemsSource = allRooms;


            }
            catch (Exception theException)
            {
                // Handle all other exceptions.
                DisplayDialog("Error", "Error Message: " + theException.Message);
            }

        }

        private void signout_tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(loginpage));
        }

    }
}
