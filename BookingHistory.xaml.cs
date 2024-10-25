using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class BookingHistory : Page
    {
        string userId = App.userID;
        public BookingViewModel ViewModel { get; set; }
        public BookingHistory()
        {
            this.InitializeComponent();
            ViewModel = new BookingViewModel();
            DataContext = ViewModel;
        }
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
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            FirebaseClient firebaseClient = new FirebaseClient("https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/");

            try
            {
                // Get the button that was clicked
                Button saveButton = (Button)sender;

                // Get the associated BookingRecord from the button's DataContext
                BookingRecord record = (BookingRecord)saveButton.DataContext;

                var Uptodate = new
                {
                    rating = record.Rating,
                    comment = record.Comment,
                };

                // Use the FirebaseKey of the booking record to save the rating and comment
                await firebaseClient.Child("Rating").Child(userId).PostAsync(Uptodate);

                // Provide feedback to the user
                var dialog = new MessageDialog($"Thanks for rating us!\nRating: {record.Rating + " star"}\nComment: {record.Comment}");
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                var errorDialog = new MessageDialog($"Error saving to Firebase: {ex.Message}");
                await errorDialog.ShowAsync();
            }
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await RetrieveActiveBookingsAsync(userId);
            await RetrievePastBookingsAsync(userId);
            await RetrieveCanceledBookingsAsync(userId);
        }

        private async Task RetrieveActiveBookingsAsync(string userId)
        {
            FirebaseClient firebaseClient = new FirebaseClient("https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/");

            var query = await firebaseClient
                .Child("Record")
                .Child(userId)
                .Child("History")
                .OnceAsync<BookingRecord>();

            DateTimeOffset currentDate = DateTimeOffset.Now;

            var filteredAndSortedQuery = query.Select(historyData => new FirebaseBookingRecord
            {
                Key = historyData.Key,
                BookingData = historyData.Object
            })
            .Where(bookingData => !bookingData.BookingData.CancelStatus)
            .Where(bookingData => bookingData.BookingData.CheckOutDate >= currentDate)
            .OrderBy(bookingData => bookingData.BookingData.CheckInDate)
            .ThenBy(bookingData => bookingData.BookingData.CheckOutDate)
            .ThenBy(bookingData => bookingData.BookingData.RoomType)
            .ToList();

            ViewModel.ActiveBookings.Clear();

            foreach (var bookingData in filteredAndSortedQuery)
            {
                bookingData.BookingData.FirebaseKey = bookingData.Key;
                ViewModel.ActiveBookings.Add(bookingData.BookingData);
            }
        }


        private async Task RetrievePastBookingsAsync(string userId)
        {
            FirebaseClient firebaseClient = new FirebaseClient("https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/");

            var query = await firebaseClient
                .Child("Record")
                .Child(userId)
                .Child("History")
                .OnceAsync<BookingRecord>();

            DateTimeOffset currentDate = DateTimeOffset.Now;

            var filteredAndSortedQuery = query.Select(historyData => historyData.Object)
                .Where(bookingRecord => !bookingRecord.CancelStatus)
                .Where(bookingRecord => bookingRecord.CheckOutDate <= currentDate)
                .OrderBy(bookingRecord => bookingRecord.CheckInDate)
                .ThenBy(bookingRecord => bookingRecord.CheckOutDate)
                .ThenBy(bookingRecord => bookingRecord.RoomType)
                .ToList();

            ViewModel.PastBookings.Clear();

            foreach (var bookingRecord in filteredAndSortedQuery)
            {
                ViewModel.PastBookings.Add(bookingRecord);
            }
        }

        private async Task RetrieveCanceledBookingsAsync(string userId)
        {
            FirebaseClient firebaseClient = new FirebaseClient("https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/");

            var query = await firebaseClient
                .Child("Record")
                .Child(userId)
                .Child("History")
                .OnceAsync<BookingRecord>();

            DateTimeOffset currentDate = DateTimeOffset.Now;

            var filteredAndSortedQuery = query.Select(historyData => historyData.Object)
                .Where(bookingRecord => bookingRecord.CancelStatus)
                .OrderBy(bookingRecord => bookingRecord.CheckInDate)
                .ThenBy(bookingRecord => bookingRecord.CheckOutDate)
                .ThenBy(bookingRecord => bookingRecord.RoomType)
                .ToList();

            ViewModel.CanceledBookings.Clear();

            foreach (var bookingRecord in filteredAndSortedQuery)
            {
                ViewModel.CanceledBookings.Add(bookingRecord);
            }
        }

        private void ActiveBookingButton_Click(object sender, RoutedEventArgs e)
        {
            var bookingRecord = ((Button)sender).DataContext as BookingRecord;

            Frame.Navigate(typeof(ActiveBookingDetails), bookingRecord);
        }



        private void PastBookingButton_Click(object sender, RoutedEventArgs e)
        {
            var bookingRecord = ((Button)sender).DataContext as BookingRecord;

            Frame.Navigate(typeof(PastBookingDetails), bookingRecord);
        }



        private void CanceledBookingButton_Click(object sender, RoutedEventArgs e)
        {
            var bookingRecord = ((Button)sender).DataContext as BookingRecord;

            Frame.Navigate(typeof(CancelledBookingDetails), bookingRecord);
        }

    }
}
