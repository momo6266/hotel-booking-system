using Firebase.Database;
using Firebase.Database.Query;
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


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace radAssignment2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ActiveBookingDetails : Page
    {
        string userId = App.userID;

        private BookingRecord BookingRecord;
        public ActiveBookingDetails()
        {
            this.InitializeComponent();
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is BookingRecord bookingRecord)
            {
                BookingRecord = bookingRecord;

                CheckInDateDataTB.Text = BookingRecord.CheckInDate.ToString();
                CheckOutDateDataTB.Text = BookingRecord.CheckOutDate.ToString();
                adultNumDataTB.Text = BookingRecord.NumAdults.ToString();
                childrenNumDataTB.Text = BookingRecord.NumChildren.ToString();
                roomTypeDataTB.Text = BookingRecord.RoomType;
                requestDataTB.Text = BookingRecord.SpecialRequests;
                roomNumDataTB.Text = BookingRecord.NumRooms.ToString();
                priceDataTB.Text = "RM" + BookingRecord.TotalPrice.ToString();
            }

            base.OnNavigatedTo(e);
        }

        private async void cancellationButton_Click(object sender, RoutedEventArgs e)
        {
            if (BookingRecord != null)
            {
                ContentDialog confirmCancelDialog = new ContentDialog
                {
                    Title = "Confirm Cancellation",
                    Content = "Are you sure you want to cancel this booking?",
                    PrimaryButtonText = "No",
                    SecondaryButtonText = "Yes"
                };

                ContentDialogResult result = await confirmCancelDialog.ShowAsync();

                if (result == ContentDialogResult.Secondary)
                {
                    string firebaseKey = BookingRecord.FirebaseKey;

                    await UpdateBookingCancellationStatus(firebaseKey);

                    DisplayDialog("Cancellation Successful", "The booking has been canceled successfully.");

                    Frame.Navigate(typeof(BookingHistory));

                }
            }
        }

        private async Task UpdateBookingCancellationStatus(string firebaseKey)
        {
            FirebaseClient firebaseClient = new FirebaseClient("https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/");

            try
            {

                await firebaseClient
                    .Child("Record")
                    .Child(userId)
                    .Child("History")
                    .Child(firebaseKey)
                    .Child("CancelStatus")
                    .PutAsync(true);

            }
            catch (Exception ex)
            {
                DisplayDialog("Error updating booking cancellation status: ", ex.Message);
            }
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

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BookingHistory));
        }
    }
}
