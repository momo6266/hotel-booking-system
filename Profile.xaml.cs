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
    public sealed partial class Profile : Page
    {
        string userId = App.userID;

        public Profile()
        {
            this.InitializeComponent();
            InitializeProfileAsync();
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


        private async Task RetrieveProfileAsync(string userId)
        {
            try
            {
                FirebaseClient firebaseClient = new FirebaseClient("https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/");

                var query = await firebaseClient
                    .Child("Customer")
                    .Child("CDetails")
                    .OnceAsync<Customer>();

                foreach (var item in query)
                {
                    var key = item.Key;

                    if (key == userId)
                    {
                        usernameProfileDataTB.Text = item.Object.CName.ToString();
                        emailProfileDataTB.Text = item.Object.CEmail.ToString();
                        nicknameProfileTB.Text = item.Object.CNickname.ToString();
                        dobProfileDataTB.Text = item.Object.CDob.ToShortDateString();
                        phoneNumProfileDataTB.Text = "+60" + item.Object.CPhone.ToString();
                        streetProfileDataTB.Text = item.Object.CStreet.ToString();
                        cityProfileDataTB.Text = item.Object.CCity.ToString();
                        postProfileDataTB.Text = item.Object.CPost.ToString();
                        stateProfileDataTB.Text = item.Object.CState.ToString();

                        break;

                    }
                }
            }

            catch (Exception ex)
            {
                DisplayDialog("Error updating profile status:1 ", ex.Message);
            }

        }

        private async void InitializeProfileAsync()
        {
            await RetrieveProfileAsync(userId);
        }

        private void editProfileButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(EditProfile));
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
