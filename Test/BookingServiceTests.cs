using Xunit;
using BookingSystem.Services;
using BookingSystem.Models;
using BookingSystem.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using BookingSystem.Configurations;

namespace BookingSystem.Tests
{
    public class BookingServiceTests
    {
        private RoomBookingService GetBookingService(DbContextOptions<BookingDbContext> options)
        {
            var context = new BookingDbContext(options);
            return new RoomBookingService(context);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldCreateBooking()
        {
            var options = new DbContextOptionsBuilder<BookingDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateBookingDb").Options;
            using (var context = new BookingDbContext(options))
            {
                context.Rooms.Add(new Room { Id = 1, Name = "Room1" });
                context.SaveChanges();
            }
            var service = GetBookingService(options);
            var request = new BookingRequest { RoomId = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) };
            var result = await service.CreateBookingAsync("user1", request);
            Assert.NotNull(result);
            Assert.Equal(1, result.RoomId);
        }

        [Fact]
        public async Task GetUserBookingsAsync_ShouldReturnBookings()
        {
            var options = new DbContextOptionsBuilder<BookingDbContext>()
                .UseInMemoryDatabase(databaseName: "GetUserBookingsDb").Options;
            using (var context = new BookingDbContext(options))
            {
                context.Rooms.Add(new Room { Id = 1, Name = "Room1" });
                context.Bookings.Add(new Booking { Id = 1, UserId = "user1", RoomId = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) });
                context.SaveChanges();
            }
            var service = GetBookingService(options);
            var bookings = await service.GetUserBookingsAsync("user1");
            Assert.Single(bookings);
            Assert.Equal(1, bookings[0].RoomId);
        }

        [Fact]
        public async Task CancelBookingAsync_ShouldCancelOwnBooking()
        {
            var options = new DbContextOptionsBuilder<BookingDbContext>()
                .UseInMemoryDatabase(databaseName: "CancelOwnBookingDb").Options;
            using (var context = new BookingDbContext(options))
            {
                context.Rooms.Add(new Room { Id = 1, Name = "Room1" });
                context.Bookings.Add(new Booking { Id = 1, UserId = "user1", RoomId = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) });
                context.SaveChanges();
            }
            var service = GetBookingService(options);
            var result = await service.CancelBookingAsync(1, "user1");
            Assert.True(result);
        }

        [Fact]
        public async Task CancelBookingAsync_ShouldNotCancelOthersBooking()
        {
            var options = new DbContextOptionsBuilder<BookingDbContext>()
                .UseInMemoryDatabase(databaseName: "CancelOthersBookingDb").Options;
            using (var context = new BookingDbContext(options))
            {
                context.Rooms.Add(new Room { Id = 1, Name = "Room1" });
                context.Bookings.Add(new Booking { Id = 1, UserId = "user1", RoomId = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) });
                context.SaveChanges();
            }
            var service = GetBookingService(options);
            var result = await service.CancelBookingAsync(1, "user2");
            Assert.False(result);
        }

        [Fact]
        public async Task CancelBookingAsync_AdminCanCancelAnyBooking()
        {
            var options = new DbContextOptionsBuilder<BookingDbContext>()
                .UseInMemoryDatabase(databaseName: "AdminCancelBookingDb").Options;
            using (var context = new BookingDbContext(options))
            {
                context.Rooms.Add(new Room { Id = 1, Name = "Room1" });
                context.Bookings.Add(new Booking { Id = 1, UserId = "user1", RoomId = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) });
                context.SaveChanges();
            }
            var service = GetBookingService(options);
            var result = await service.CancelBookingAsync(1, "admin", true);
            Assert.True(result);
        }

        [Fact]
        public async Task GetAvailableTimeSlotsAsync_ShouldReturnAvailableSlots()
        {
            var options = new DbContextOptionsBuilder<BookingDbContext>()
                .UseInMemoryDatabase(databaseName: "AvailableTimeSlotsDb").Options;
            var date = DateTime.Today;
            using (var context = new BookingDbContext(options))
            {
                context.Rooms.Add(new Room { Id = 1, Name = "Room1" });
                context.Bookings.Add(new Booking { Id = 1, UserId = "user1", RoomId = 1, StartTime = date.AddHours(9), EndTime = date.AddHours(10) });
                context.Bookings.Add(new Booking { Id = 2, UserId = "user2", RoomId = 1, StartTime = date.AddHours(12), EndTime = date.AddHours(13) });
                context.SaveChanges();
            }
            var service = GetBookingService(options);
            var slots = await service.GetAvailableTimeSlotsAsync(1, date);
            Assert.NotEmpty(slots);
            Assert.Contains(slots, s => s.StartTime == date && s.EndTime == date.AddHours(9));
            Assert.Contains(slots, s => s.StartTime == date.AddHours(10) && s.EndTime == date.AddHours(12));
            Assert.Contains(slots, s => s.StartTime == date.AddHours(13) && s.EndTime == date.AddDays(1));
        }
    }
}
