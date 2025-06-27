using BookingSystem.Models;
using BookingSystem.DTOs;
using BookingSystem.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Services
{
    public class RoomBookingService
    {
        private readonly BookingDbContext _context;
        public RoomBookingService(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<Booking?> CreateBookingAsync(string userId, BookingRequest request)
        {
            var room = await _context.Rooms.FindAsync(request.RoomId);
            if (room == null) return null;
            // Improved overlap check: any overlap at all
            bool overlap = await _context.Bookings.AnyAsync(b => b.RoomId == request.RoomId &&
                b.StartTime < request.EndTime && request.StartTime < b.EndTime);
            if (overlap) return null;
            var booking = new Booking
            {
                UserId = userId,
                RoomId = request.RoomId,
                StartTime = request.StartTime,
                EndTime = request.EndTime
            };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<List<BookingResponse>> GetUserBookingsAsync(string userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Room)
                .Select(b => new BookingResponse
                {
                    Id = b.Id,
                    RoomId = b.RoomId,
                    RoomName = b.Room != null ? b.Room.Name : string.Empty,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime
                })
                .ToListAsync();
        }

        public async Task<bool> CancelBookingAsync(int bookingId, string userId, bool isAdmin = false)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return false;
            if (!isAdmin && booking.UserId != userId) return false;
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<TimeSlotDto>> GetAvailableTimeSlotsAsync(int roomId, DateTime date)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null) return new List<TimeSlotDto>();
            var bookings = await _context.Bookings
                .Where(b => b.RoomId == roomId && b.StartTime.Date == date.Date)
                .OrderBy(b => b.StartTime)
                .ToListAsync();
            var slots = new List<TimeSlotDto>();
            var startOfDay = date.Date;
            var endOfDay = date.Date.AddDays(1);
            var current = startOfDay;
            foreach (var booking in bookings)
            {
                if (current < booking.StartTime)
                {
                    slots.Add(new TimeSlotDto { StartTime = current, EndTime = booking.StartTime });
                }
                current = booking.EndTime > current ? booking.EndTime : current;
            }
            if (current < endOfDay)
            {
                slots.Add(new TimeSlotDto { StartTime = current, EndTime = endOfDay });
            }
            return slots;
        }
    }
}
