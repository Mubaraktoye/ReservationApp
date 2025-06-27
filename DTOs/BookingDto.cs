using System;

namespace BookingSystem.DTOs
{
    public class BookingRequest
    {
        public int RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class BookingResponse
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class TimeSlotDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class CancelBookingRequest
    {
        public int BookingId { get; set; }
    }

    public class AvailableTimeSlotsRequest
    {
        public int RoomId { get; set; }
        public DateTime Date { get; set; }
    }
}
