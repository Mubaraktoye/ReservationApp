# BookingSystem.Api

A robust .NET 8 Web API for managing room bookings, featuring JWT authentication, role-based authorization, and SQLite persistence. The API supports user registration, login, room booking, cancellation, and querying available time slots, with a clean separation of concerns and comprehensive test coverage.

## Features

- **User Management**: Register and authenticate users with secure password hashing.
- **Role-Based Authorization**: Supports "User" and "Admin" roles for access control.
- **Room Booking**: Book, cancel, and view room reservations.
- **Time Slot Availability**: Query available time slots for rooms.
- **RESTful API**: Clean, versioned endpoints for all operations.
- **JWT Authentication**: Secure endpoints with JSON Web Tokens.
- **Entity Framework Core**: SQLite database for persistence.
- **Swagger/OpenAPI**: Interactive API documentation.
- **Error Handling**: Centralized middleware for consistent error responses.
- **Testing**: xUnit-based unit and integration tests.

## Project Structure

- `Models/` — Entity models: `User`, `Room`, `Booking`
- `DTOs/` — Data transfer objects for requests and responses
- `Services/` — Business logic: `UserService`, `RoomBookingService`
- `Controllers/` — API endpoints: `AuthController`, `RoomBookingController`
- `Configurations/` — EF Core context, data seeder
- `Middleware/` — Error handling middleware
- `Test/` — xUnit test project for services

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Setup & Run

1. **Clone the repository**
2. **Restore dependencies**dotnet restore3. **Apply database migrations**dotnet ef migrations add InitialCreate
dotnet ef database update4. **Run the API**dotnet runThe API will be available at `https://localhost:5001` or `http://localhost:5000`.

### Default Data

- **Rooms**: Seeded with three sample rooms.
- **Admin User**:  
  - Email: `admin@bookingsystem.com`  
  - Password: `Admin@123`  
  - Role: `Admin`

## API Endpoints

### Authentication

- `POST /api/auth/register` — Register a new user
- `POST /api/auth/login` — Login and receive JWT

### Room Booking

- `POST /api/roombooking` — Book a room (JWT required)
- `GET /api/roombooking/user/{userId}` — Get bookings for a user (JWT required, user or admin)
- `DELETE /api/roombooking/{bookingId}` — Cancel a booking (JWT required, user or admin)
- `POST /api/roombooking/available-timeslots` — Get available time slots for a room (JWT required)

### Example Requests

#### Register
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"userName":"user1","email":"user1@example.com","password":"User123!"}'
#### Login
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user1@example.com","password":"User123!"}'
#### Book a Room
curl -X POST https://localhost:5001/api/roombooking \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"roomId":1,"startTime":"2025-07-01T09:00:00","endTime":"2025-07-01T10:00:00"}'
#### Get Available Time Slots
curl -X POST https://localhost:5001/api/roombooking/available-timeslots \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"roomId":1,"date":"2025-07-01"}'
## Configuration

- **JWT settings**: `appsettings.json` under `Jwt` section
- **Database**: SQLite, connection string in `appsettings.json`
- **Environment**: Set `ASPNETCORE_ENVIRONMENT` to `Development` for Swagger UI

## Testing

Run all unit and integration tests:
dotnet test
## API Documentation

- Swagger UI available at `/swagger` when running in development mode.

---

**For more details, see the source code and Swagger UI.**
