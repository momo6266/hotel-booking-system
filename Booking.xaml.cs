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
    public sealed partial class Booking : Page
    {
        string userId = App.userID;

        float totalPrice = 0;
        public BookingViewModel ViewModel { get; set; }
        public static FirebaseClient FirebaseClient { get; set; }
        public static FirebaseStorage FirebaseStorage { get; private set; }
        public Booking()
        {
            this.InitializeComponent();

            ViewModel = new BookingViewModel
            {
                CheckInDate = DateTimeOffset.Now,
                CheckOutDate = DateTimeOffset.Now.AddDays(1),
                RoomPrices = LoadRoomPrices()
            };

            this.DataContext = ViewModel;

            var firebaseDatabaseUrl = "https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/";
            var firebaseStorageUrl = "radassignment-59ca9.appspot.com";

            FirebaseClient = new FirebaseClient(firebaseDatabaseUrl);
            FirebaseStorage = new FirebaseStorage(firebaseStorageUrl);

            _ = LoadRoomTypes();
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

        private void CheckInDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            RoomTypeComboBox.SelectedIndex = -1;
            NumRoomsComboBox.Items.Clear();
            ViewModel.RoomAvailabilityText = string.Empty;
        }

        private void CheckOutDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            //CheckOutDatePicker.Date = CheckInDatePicker.Date.AddDays(1);
            RoomTypeComboBox.SelectedIndex = -1;
            NumRoomsComboBox.Items.Clear();
            ViewModel.RoomAvailabilityText = string.Empty;
        }

        private void ClearInputFields()
        {
            CheckInDatePicker.Date = DateTime.Today;
            CheckOutDatePicker.Date = DateTime.Today.AddDays(1);
            RoomTypeComboBox.SelectedIndex = -1;
            NumAdultsTextBox.Text = "";
            NumChildrenTextBox.Text = "";
            SpecialRequestsTextBox.Text = "";
        }

        private async void BookNowButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CheckInDatePicker.Date.ToString()) ||
                    string.IsNullOrWhiteSpace(CheckOutDatePicker.Date.ToString()) ||
                    RoomTypeComboBox.SelectedItem == null ||
                    !int.TryParse(NumAdultsTextBox.Text, out _) ||
                    !int.TryParse(NumChildrenTextBox.Text, out _))
                {
                    string temp = "Please fill out all required fields correctly.";
                    DisplayDialog("Validation Error", temp);
                    return;
                }

                DateTime checkInDate = CheckInDatePicker.Date.Date;
                DateTime checkOutDate = CheckOutDatePicker.Date.Date;


                if (checkOutDate < DateTime.Today)
                {
                    DisplayDialog("Validation Error", "Check-out date cannot be in the past.");
                    return;
                }

                if (checkInDate < DateTime.Today)
                {
                    DisplayDialog("Validation Error", "Check-in date cannot be in the past.");
                    return;
                }


                if (checkInDate >= checkOutDate)
                {
                    DisplayDialog("Validation Error", "Check-out date must be after the check-in date.");
                    return;
                }

                RoomTypeItem selectedRoomTypeItem = (RoomTypeItem)RoomTypeComboBox.SelectedItem;
                string selectedRoomType = selectedRoomTypeItem.Key;

                int numAdults = int.Parse(NumAdultsTextBox.Text);
                int numChildren = int.Parse(NumChildrenTextBox.Text);
                string specialRequests = SpecialRequestsTextBox.Text;

                int selectedNumRooms = (int)NumRoomsComboBox.SelectedItem;

                if (selectedNumRooms <= 0)
                {
                    DisplayDialog("Validation Error", "Please select at least one room.");
                    return;
                }



                string text = await SimulateBooking(checkInDate, checkOutDate, selectedRoomType, numAdults, numChildren, specialRequests, selectedNumRooms);

                DisplayDialog("Booking Confirmation", text);

                ClearInputFields();
            }
            catch (Exception ex)
            {
                DisplayDialog("Booking Error", ex.Message);
            }
        }


        private async Task<string> SimulateBooking(DateTime checkInDate, DateTime checkOutDate, string roomType, int numAdults, int numChildren, string specialRequests, int selectedNumRooms)
        {
            try
            {
                if (selectedNumRooms <= 0)
                {
                    return "Please select at least one room.";
                }

                bool isRoomAvailable = await IsRoomAvailable(roomType, checkInDate, checkOutDate, selectedNumRooms);

                if (!isRoomAvailable)
                {
                    return "Room is not available for the selected date range.";
                }

                var booking = new BookingModel
                {
                    CheckInDate = checkInDate.ToString("yyyy-MM-dd"),
                    CheckOutDate = checkOutDate.ToString("yyyy-MM-dd"),
                    RoomType = roomType,
                    NumAdults = numAdults,
                    NumChildren = numChildren,
                    SpecialRequests = specialRequests,
                    CancelStatus = false,
                    NumRooms = selectedNumRooms,
                    TotalPrice = totalPrice,
                };

                var userBookingPath = FirebaseClient.Child("Record").Child(userId).Child("History");

                var bookingResult = await userBookingPath.PostAsync(booking);

                if (!string.IsNullOrEmpty(bookingResult.Key))
                {
                    return "Booking Successful!";
                }
                else
                {
                    return "Booking Failed!";
                }
            }
            catch (Exception ex)
            {
                return $"Booking Error: {ex.Message}";
            }
        }

        private async Task<bool> IsRoomAvailable(string roomType, DateTime checkInDate, DateTime checkOutDate, int numRooms)
        {
            try
            {
                var database = new FirebaseClient("https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/");

                var roomInfo = await database.Child("Room").Child(roomType).OnceSingleAsync<RoomInfo>();

                if (roomInfo == null)
                {
                    return false;
                }

                var userBookings = await database
                    .Child("Record")
                    .Child(userId)
                    .Child("History")
                    .OrderByKey()
                    .OnceAsync<BookingModel>();

                int totalOccupiedRooms = 0;

                foreach (var booking in userBookings)
                {
                    var bookingCheckInDate = DateTime.Parse(booking.Object.CheckInDate);
                    var bookingCheckOutDate = DateTime.Parse(booking.Object.CheckOutDate);

                    if (!booking.Object.CancelStatus)
                    {
                        if (booking.Object.RoomType == roomType &&
                            bookingCheckInDate < checkOutDate &&
                            bookingCheckOutDate > checkInDate)
                        {
                            totalOccupiedRooms += booking.Object.NumRooms;
                        }
                    }
                }

                int maxRoomLimit = roomInfo.Limit;

                int availableRooms = maxRoomLimit - totalOccupiedRooms;

                return availableRooms >= numRooms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task LoadRoomTypes()
        {
            try
            {
                var database = new FirebaseClient("https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/");
                var roomTypes = await database.Child("Room").OnceAsync<RoomInfo>();

                ViewModel.RoomTypes.Clear();

                foreach (var roomType in roomTypes)
                {
                    ViewModel.RoomTypes.Add(new RoomTypeItem
                    {
                        RoomType = roomType.Key,
                        Key = roomType.Key
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
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

        private async void RoomTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoomTypeComboBox.SelectedItem is RoomTypeItem selectedRoom)
            {
                ViewModel.SelectedRoomTypeKey = selectedRoom.RoomType;

                NumRoomsComboBox.SelectedIndex = -1;
                NumRoomsComboBox.Items.Clear();
                ViewModel.RoomAvailabilityText = string.Empty;

                DateTime checkInDate = CheckInDatePicker.Date.Date;
                DateTime checkOutDate = CheckOutDatePicker.Date.Date;

                string selectedRoomType = selectedRoom.RoomType;

                int numRooms = await GetNumberOfAvailableRooms(selectedRoomType, checkInDate, checkOutDate);

                bool isAvailable = await IsRoomAvailable(selectedRoomType, checkInDate, checkOutDate, numRooms);

                if (isAvailable)
                {
                    ViewModel.RoomAvailabilityText = $"{numRooms}";
                }
                else
                {
                    ViewModel.RoomAvailabilityText = "Room not available for the selected date range or number of rooms.";
                }

                await UpdateNumRoomsComboBox();
            }
        }

        private async Task<int> GetNumberOfAvailableRooms(string selectedRoomType, DateTime checkInDate, DateTime checkOutDate)
        {
            try
            {
                var database = new FirebaseClient("https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/");

                var roomInfo = await database
                    .Child("Room")
                    .Child(selectedRoomType)
                    .OnceSingleAsync<RoomInfo>();

                if (roomInfo == null)
                {
                    return 0;
                }

                var userBookings = await database
                    .Child("Record")
                    .Child(userId)
                    .Child("History")
                    .OrderByKey()
                    .OnceAsync<BookingModel>();

                int totalOccupiedRooms = 0;

                foreach (var booking in userBookings)
                {
                    var bookingCheckInDate = DateTime.Parse(booking.Object.CheckInDate);
                    var bookingCheckOutDate = DateTime.Parse(booking.Object.CheckOutDate);

                    if (!booking.Object.CancelStatus && booking.Object.RoomType == selectedRoomType)
                    {
                        if (bookingCheckInDate < checkOutDate && bookingCheckOutDate > checkInDate)
                        {
                            totalOccupiedRooms += booking.Object.NumRooms;
                        }
                    }
                }

                int maxRoomLimit = roomInfo.Limit;

                int availableRooms = maxRoomLimit - totalOccupiedRooms;

                if (availableRooms < 0)
                {
                    availableRooms = 0;
                }

                return availableRooms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void NumRoomsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NumRoomsComboBox.SelectedItem is int selectedNumRooms)
            {
                if (ViewModel.RoomPrices.TryGetValue(ViewModel.SelectedRoomTypeKey, out int roomPrice))
                {
                    totalPrice = selectedNumRooms * roomPrice;
                    ViewModel.RoomPriceText = $"RM{totalPrice}";
                }

                ViewModel.SelectedNumRooms = selectedNumRooms;
            }
        }

        private async Task UpdateNumRoomsComboBox()
        {
            if (ViewModel.SelectedRoomTypeKey != null)
            {
                int maxAvailableRooms = await GetNumberOfAvailableRooms(ViewModel.SelectedRoomTypeKey, ViewModel.CheckInDate.Date, ViewModel.CheckOutDate.Date);

                NumRoomsComboBox.Items.Clear();
                for (int i = 0; i <= maxAvailableRooms; i++)
                {
                    NumRoomsComboBox.Items.Add(i);
                }
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
    }
}
