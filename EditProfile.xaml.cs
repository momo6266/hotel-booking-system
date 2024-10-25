using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public sealed partial class EditProfile : Page
    {
        string userId = App.userID;
        public CustomerView View { get; set; }

        public EditProfile()
        {
            this.InitializeComponent();
            View = new CustomerView();
            
            InitializeEditProfileAsync();
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

        private async Task LoadState()
        {
            try
            {
                var database = new FirebaseClient("https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/");
                var query = await database.Child("State").OnceAsync<State>();

                foreach (var item in query)
                {
                    string stateName = item.Key;
                    StateComboBox.Items.Add(stateName);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task RetrieveEditProfileAsync(string userId)
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
                        phoneNumProfileDataTB.Text = item.Object.CPhone.ToString();
                        streetProfileDataTB.Text = item.Object.CStreet.ToString();
                        cityProfileDataTB.Text = item.Object.CCity.ToString();
                        postProfileDataTB.Text = item.Object.CPost.ToString();
                        StateComboBox.PlaceholderText = item.Object.CState.ToString();

                        break;

                    }
                }
            }

            catch (Exception ex)
            {
                DisplayDialog("Error updating profile status: ", ex.Message);
            }

        }

        private async void InitializeEditProfileAsync()
        {
            await RetrieveEditProfileAsync(userId);
            await LoadState();
        }


        private async void submitProfileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FirebaseClient firebaseClient = new FirebaseClient("https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/");
                DateTime DOB = DateTime.Now;

                if (!int.TryParse(phoneNumProfileDataTB.Text, out int phone)|| phoneNumProfileDataTB.Text.Length < 9 || phoneNumProfileDataTB.Text.Length > 10 || !int.TryParse(postProfileDataTB.Text, out int post))
                {
                    string temp = "Please fill out all required fields correctly.";
                    DisplayDialog("Validation Error", temp);
                    return;
                }

                var query = await firebaseClient
                    .Child("Customer")
                    .Child("CDetails")
                    .OnceAsync<Customer>();

                foreach (var item in query)
                {
                    var key = item.Key;

                    if (key == userId)
                    {
                        DOB = item.Object.CDob;
                        dobProfileDataTB.Text = DOB.ToShortDateString();
                       
                        break;

                    }
                }

                string name = usernameProfileDataTB.Text.ToString();
                string email = emailProfileDataTB.Text.ToString();
                
                string nickname = nicknameProfileTB.Text.ToString();
                string street = streetProfileDataTB.Text.ToString();
                string city = cityProfileDataTB.Text.ToString();
                string state;

                if (StateComboBox.SelectedItem != null)
                {
                    state = StateComboBox.SelectedItem.ToString();
                    
                }

                else
                {
                    state = StateComboBox.PlaceholderText.ToString();
                }

                var editprofile = new Customer
                {
                    CNickname = nickname,
                    CPhone = phone,
                    CStreet = street,
                    CCity = city,
                    CPost = post,
                    CState = state,
                    CName = name,
                    CEmail = email,
                    CDob = DOB,
                    
                };

                var reference = firebaseClient.Child("Customer").Child("CDetails").Child(userId);

                await reference.PutAsync(editprofile);

                DisplayDialog("You have successfully edit your profile!","");



            }
            catch (Exception ex)
            {
                DisplayDialog("Error updating profile status: ", ex.Message);
            }
            this.Frame.Navigate(typeof(Profile));
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
