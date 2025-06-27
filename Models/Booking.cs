using System;

namespace BookingSystem.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public User? User { get; set; }
        public Room? Room { get; set; }
    }
}
