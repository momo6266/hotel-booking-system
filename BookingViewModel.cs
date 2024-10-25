using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace radAssignment2
{
    public class BookingViewModel : INotifyPropertyChanged
    {
        public DateTimeOffset CheckInDate { get; set; }
        public DateTimeOffset CheckOutDate { get; set; }
        public Dictionary<string, int> RoomPrices { get; set; }
        public Dictionary<string, int> RoomLimits { get; set; }

        private ObservableCollection<BookingRecord> activeBookings;
        public ObservableCollection<BookingRecord> ActiveBookings { get; private set; }

        private ObservableCollection<BookingRecord> pastBookings;
        public ObservableCollection<BookingRecord> PastBookings { get; private set; }

        private ObservableCollection<BookingRecord> cancelBookings;
        public ObservableCollection<BookingRecord> CanceledBookings { get; private set; }

        public BookingViewModel()
        {
            RoomPrices = new Dictionary<string, int>();
            RoomLimits = new Dictionary<string, int>();
            ActiveBookings = new ObservableCollection<BookingRecord>();
            PastBookings = new ObservableCollection<BookingRecord>();
            CanceledBookings = new ObservableCollection<BookingRecord>();
        }

        public ObservableCollection<RoomTypeItem> RoomTypes { get; } = new ObservableCollection<RoomTypeItem>();
        public string SelectedRoomTypeKey { get; set; }

        private string roomPriceText;
        public string RoomPriceText
        {
            get { return roomPriceText; }
            set
            {
                if (roomPriceText != value)
                {
                    roomPriceText = value;
                    OnPropertyChanged("RoomPriceText");
                }
            }
        }

        private string _roomAvailabilityText;
        public string RoomAvailabilityText
        {
            get { return _roomAvailabilityText; }
            set
            {
                if (_roomAvailabilityText != value)
                {
                    _roomAvailabilityText = value;
                    OnPropertyChanged(nameof(RoomAvailabilityText));
                }
            }
        }

        private int _selectedNumRooms;
        public int SelectedNumRooms
        {
            get { return _selectedNumRooms; }
            set
            {
                if (_selectedNumRooms != value)
                {
                    _selectedNumRooms = value;
                    OnPropertyChanged(nameof(SelectedNumRooms));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
