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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace radAssignment2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class signupFillDetails : Page
    {
        string Mail;
        public signupFillDetails()
        {
            this.InitializeComponent();
            LoadState();
        }

        private async void DisplayDialog(string title, string content)
        {
            ContentDialog bookingDialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await bookingDialog.ShowAsync();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null)
            {
                string email = e.Parameter.ToString();
                Mail = email;
            }
        }

        private async void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FirebaseClient firebaseClient = new FirebaseClient("https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/");
                

                if (string.IsNullOrWhiteSpace(dobDatePicker.Date.ToString()) ||
                string.IsNullOrWhiteSpace(NameTextBox.Text.ToString()) ||
                StateComboBox.SelectedItem == null ||
                !int.TryParse(postTextBox.Text, out int post) ||
                !int.TryParse(phoneNumTextBox.Text, out int phone) ||
                NameTextBox.Text == null ||
                nicknameTextBox.Text == null ||
                streetTextBox.Text == null ||
                cityTextBox.Text == null ||
                cityTextBox.Text == null||
                dobDatePicker.Date > DateTime.Today||
                phoneNumTextBox.Text.Length<9||
                phoneNumTextBox.Text.Length>10  
                )
                {
                    string temp = "Please fill out all required fields correctly.";
                    DisplayDialog("Validation Error", temp);
                    return;
                }

                else
                {
                    string name = NameTextBox.Text;
                    string nickname = nicknameTextBox.Text;
                    DateTime dob = dobDatePicker.Date.Date;
                    string street = streetTextBox.Text;
                    string city = cityTextBox.Text;
                    string state = StateComboBox.SelectedItem.ToString();
                    string key = null;

                    var query = await firebaseClient.Child("UserAcc").OnceAsync<UserAcc>();

                    foreach (var item in query) 
                    {
                        if (Mail == item.Object.Email.ToString())
                        {
                            key = item.Key;
                            break;

                        }
                    }

                    var addprofile = new Customer
                    {
                        CNickname = nickname,
                        CPhone = phone,
                        CStreet = street,
                        CCity = city,
                        CPost = post,
                        CState = state,
                        CName = name,
                        CEmail = Mail,
                        CDob = dob,

                    };

                    var reference = firebaseClient.Child("Customer").Child("CDetails").Child(key);

                    await reference.PutAsync(addprofile);

                    DisplayDialog("Success", "You have successfully add your profile! Please proceed to login");

                    this.Frame.Navigate(typeof(loginpage));
                }
            }
            catch( Exception ex )
            {
                DisplayDialog("Error insert profile status ", ex.Message);
            }



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
    }
}
