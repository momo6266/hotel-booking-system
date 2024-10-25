using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace radAssignment2
{
    public class AdminModel : INotifyPropertyChanged
    {
        public BookingViewModel ViewModel { get; set; }
        public string RoomType { get; set; }
        private FirebaseClient firebaseClient;
        
        public AdminModel()
        {
            // Initialize your FirebaseClient with your Firebase URL
            firebaseClient = new FirebaseClient("https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/");
            ViewModel = new BookingViewModel();
        }
        public async Task<List<RoomInfo>> GetAllRooms()
        {

            return (await firebaseClient
               .Child("Room")
              .OnceAsync<RoomInfo>()).Select(item => new RoomInfo
            {
                  RoomType = item.Key,
                  Price = item.Object.Price,
                  Limit = item.Object.Limit
              }).ToList();
        }
        public ObservableCollection<RoomTypeItem> RoomTypes { get; } = new ObservableCollection<RoomTypeItem>();

        public async Task AddRoom(RoomInfo r)
        {
            await firebaseClient
           .Child("Room")
           .Child(r.RoomType)
           .PutAsync(r);
        }

        public async Task DeleteRoom(string roomType)
        {
            firebaseClient = new FirebaseClient("https://radassignment-59ca9-default-rtdb.asia-southeast1.firebasedatabase.app/");
            await firebaseClient.Child("Room").Child(roomType).DeleteAsync(); 

        }
        public async Task UpdatePerson(string RoomType, RoomInfo r)
        {
            await firebaseClient
           .Child("Room")
           .Child(RoomType)
           .PutAsync(JsonConvert.SerializeObject(r));
        }
        public async Task AddNewRoomTypeAsync(string roomTypeName, int price, int limit)
        {
            var roomTypeItem = new RoomTypeItem
            {
                RoomType = roomTypeName,
                RoomInfo = new RoomInfo
                {
                    Price = price,
                    Limit = limit
                }
            };

            await firebaseClient.Child("Room").PostAsync(roomTypeItem);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task LoadRoomTypes()
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

    }
}


