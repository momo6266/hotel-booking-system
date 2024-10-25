using System;
using Firebase.Auth;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Firebase.Database;
using Firebase.Storage;
using Firebase;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.Storage;
using Firebase.Auth.Providers;


namespace radAssignment2
{
    sealed partial class App : Application
    {

        public static string userID {  get; set; }
        public static string adminID { get; set; }

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            InitializeFirebase();


            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(loginpage), e.Arguments);
                }

                Window.Current.Activate();
            }
        }

        private void InitializeFirebase()
        {
            var firebaseDatabaseUrl = "https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/";
            var webApiKey = " AIzaSyD9qDdQB_gCnqdaKxOe6TmpFcFyBvp5lyM";

            var firebaseClient = new FirebaseClient(firebaseDatabaseUrl, new FirebaseOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(webApiKey)
            });

            Booking.FirebaseClient = firebaseClient;

        }


        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}

