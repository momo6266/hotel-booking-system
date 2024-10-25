using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Devices.Enumeration;
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
    public sealed partial class signup : Page
    {


        public signup()
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

        private async void signUpButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FirebaseClient firebaseClient = new FirebaseClient("https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/");

                var query = await firebaseClient.Child("UserAcc").OnceAsync<UserAcc>();

                string ps = passwordTextBox.Text;

                string email = emailTextBox.Text;

                bool check = false;

                if ((!string.IsNullOrEmpty(emailTextBox.Text)) && (!string.IsNullOrEmpty(passwordTextBox.Text)))
                {
                    foreach (var item in query)
                    {
                        
                        var mail = item.Object.Email;
                        
                        if (mail != email)
                        {
                            check = false;

                        }
                        else
                        {
                            check = true;
                            break;
                        }
                    }
                    if (check)
                    {
                        DisplayDialog("Error", "Account already exist, please proceed to login");
                        this.Frame.Navigate(typeof(loginpage));
                    }
                    else
                    {
                        var editprofile = new UserAcc
                        {
                            Email = emailTextBox.Text,
                            Password = passwordTextBox.Text,

                        };

                        var reference = firebaseClient.Child("UserAcc");

                        await reference.PostAsync(editprofile);

                        //DisplayDialog("Success", "You have successfully registered an account! Please proceed to login");
                        this.Frame.Navigate(typeof(signupFillDetails), email);
                    }

                    
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

        private void toSignInButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(loginpage));
        }
    }
}
