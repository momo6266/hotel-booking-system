using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace radAssignment2
{
    public class BookingModel
    {
        public string CheckInDate { get; set; }
        public string CheckOutDate { get; set; }
        public string RoomType { get; set; }
        public int NumAdults { get; set; }
        public int NumChildren { get; set; }
        public string SpecialRequests { get; set; }
        public bool CancelStatus { get; set; }
        public int NumRooms { get; set; }

        public float TotalPrice { get; set; }
    }
}
