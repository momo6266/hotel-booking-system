using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace radAssignment2
{
    public class BookingRecord
    {
        public string FirebaseKey { get; set; } // New property to store the Firebase key
        public bool CancelStatus { get; set; }
        public DateTimeOffset CheckInDate { get; set; }
        public DateTimeOffset CheckOutDate { get; set; }
        public int NumAdults { get; set; }
        public int NumChildren { get; set; }
        public string RoomType { get; set; }
        public string SpecialRequests { get; set; }
        public int NumRooms { get; set; }

        public float TotalPrice { get; set; }

        //yck
        public double Rating { get; set; }
        public string Comment { get; set; }
    }
}
