using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Chat;
using Windows.Devices.Enumeration;
using Windows.Devices.PointOfService;
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
    public sealed partial class loginpage : Page
    {


        public loginpage()
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

            private void toSignUpButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(signup));
        }

        private async void signInButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FirebaseClient firebaseClient = new FirebaseClient("https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/");

                var query = await firebaseClient.Child("UserAcc").OnceAsync<UserAcc>();

                var adminquery = await firebaseClient.Child("AdminAcc").OnceAsync<AdminAcc>();

                string ps = passwordBox.Password.ToString();

                string email = txtEmail.Text;

                bool check = false;

                bool admin = false;

                if (email != null && ps != null)
                {

                    foreach (var item in query)
                    {
                        var key = item.Key;
                        var mail = item.Object.Email;
                        var pass = item.Object.Password;
                        if (mail == email && pass == ps)
                        {
                            App.userID = key;
                            check = false;
                            admin = false;
                            this.Frame.Navigate(typeof(MainPage));
                            
                            break;

                        }
                        else
                        {
                            check = true;
                            admin = true;
                        }
                    }

                    if (admin&&check)
                    {
                        foreach (var x in adminquery)
                        {
                            var key = x.Key;
                            var username = x.Object.username;
                            var adminpass = x.Object.password;

                            Debug.WriteLine($"avc: {username} + {adminpass}");
                            Debug.WriteLine($"a: {email} + {ps}");

                            if (username == email && adminpass == ps)
                            {

                                App.adminID = key;
                                check = false;
                                admin = true;
                                this.Frame.Navigate(typeof(Admin));

                                break;

                            }
                            else
                            {
                                check = true;
                                admin = false;
                            }
                        }
                    }

                    if (check&&!admin)
                    {
                        DisplayDialog("Error", "Please enter correct information");
                    }
                }
                else
                    DisplayDialog("Input", "Please key in all the information.");




            }
            catch (Exception ex)
            {
                DisplayDialog("Error login", ex.Message);
            }

        }
    }
}
