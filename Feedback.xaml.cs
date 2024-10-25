using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace radAssignment2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Feedback : Page
    {
        private FirebaseClient FirebaseClient { get; set; }
        public static FirebaseStorage FirebaseStorage { get; private set; }

        string userId = App.userID;

        public Feedback()
        {
            this.InitializeComponent();

            // Initialize Firebase client
            var firebaseDatabaseUrl = "https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/";
            var firebaseStorageUrl = "radassignment-59ca9.appspot.com";

            FirebaseClient = new FirebaseClient(firebaseDatabaseUrl);
            FirebaseStorage = new FirebaseStorage(firebaseStorageUrl);
        }

        private void homeAppButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void profileAppButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Profile));
        }

        private void bookingAppButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Booking));
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string feedbackText = FeedbackTextBox.Text;
            string nameText = NameTextBox.Text;

            if (string.IsNullOrWhiteSpace(feedbackText))
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Feedback Error",
                    Content = "Please provide feedback.",
                    CloseButtonText = "OK"
                };

                await errorDialog.ShowAsync();
                return;
            }

            // Create feedback object
            var feedback = new
            {
                NameText = nameText,
                FeedbackText = feedbackText,
                Timestamp = DateTime.UtcNow
            };

            // Store feedback to Firebase
            await FirebaseClient.Child("Feedbacks").Child(userId).PostAsync(feedback);

            // Thank you message
            ContentDialog thankYouDialog = new ContentDialog
            {
                Title = "Feedback Submitted",
                Content = "Thank you for your feedback!\nKindly wait for our email reply.",
                CloseButtonText = "OK"
            };

            await thankYouDialog.ShowAsync();

            // Optionally, clear feedback form
            FeedbackTextBox.Text = "";
            NameTextBox.Text = "";
        }

        private async void PreviousFeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var feedbackEntries = await FirebaseClient.Child("Feedbacks").Child(userId).OnceAsync<FeedbackModel>();

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
    }
}
