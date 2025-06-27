using BookingSystem.DTOs;
using BookingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomBookingController : ControllerBase
    {
        private readonly RoomBookingService _bookingService;
        public RoomBookingController(RoomBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> BookRoom(BookingRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { success = false, message = "User not authenticated." });
            var booking = await _bookingService.CreateBookingAsync(userId, request);
            if (booking == null)
                return BadRequest(new { success = false, message = "Room is not available for the selected time or does not exist." });
            return Ok(new { success = true, message = "Booking successful.", data = new { booking.Id, booking.RoomId, booking.StartTime, booking.EndTime } });
        }

        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserBookings(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            if (currentUserId != userId && !isAdmin)
                return Forbid();
            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            return Ok(new { success = true, data = bookings });
        }

        [Authorize]
        [HttpDelete("{bookingId}")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            var result = await _bookingService.CancelBookingAsync(bookingId, userId, isAdmin);
            if (!result)
                return Forbid();
            return Ok(new { success = true, message = "Booking cancelled successfully." });
        }

        [Authorize]
        [HttpPost("available-timeslots")]
        public async Task<IActionResult> GetAvailableTimeSlots([FromBody] AvailableTimeSlotsRequest request)
        {
            var slots = await _bookingService.GetAvailableTimeSlotsAsync(request.RoomId, request.Date);
            return Ok(new { success = true, data = slots });
        }
    }
}
