using Hotel_app.Enums;
using Hotel_app.Classes;

namespace Hotel_app.Services;

public class HotelManager
{
    public List<Guest> Guests { get; private set; } = new();
    public List<Room> Rooms { get; private set; } = new();
    public List<Reservation> Reservations { get; private set; } = new();
    public List<Employee> Employees { get; private set; } = new();
    public List<Shift> Shifts { get; private set; } = new();

    public bool RegisterGuest(Guest guest)
    {
        if (Guests.Any(g => g.Email.Equals(guest.Email, StringComparison.OrdinalIgnoreCase)))
            return false;
        Guests.Add(guest);
        return true;
    }
    public Guest?Login(string email, string password)
    {
        return Guests.FirstOrDefault(g => g.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && g.VerifyPassword(password));
    }
    public bool AddRoom(Room room)
    {
        if (Rooms.Any(r => r.Roo.Number == room.Number))
            return false;
        Rooms.Add(room);
        return true;
    }
    public List<Room> FindAvailableRooms(DateTime arrival, DateTime departure)
    {
        if (departure.Date <= arrival.Date)
            return new List<Room>();

        return Rooms.Where(room => !Reservations.Any(reservation => reservation.Room.Id == room.Id &&
        reservation.Status == ReservationStatus.Confirmed && arrival.Date < reservation.CheckOutDate &&
        departure.Date > reservation.CheckInDate)).ToList();
    }
    public Reservation AddReservation(Guest guest, Room room, DateTime arrival, DateTime departure)
    {
        if (!FindAvailableRooms(arrival, departure).Any(r => r.Id == room.Id))
            throw new invalidOperationException("The room is not free during the chosen time.");
        var reservation = new Reservation(guest, room, arrival, departure);
        Reservations.Add(reservation);
        return reservation;
    }
    public void UpdateFinishedReservation()
    {
        foreach (car reservation in Reservations)
            reservation.CompleteReservation();
    }
    public void AddEmployee(Employee employee)
    {
        Employees.Add(employee);
    }
    public void AddShift(Shift shift)
    {
        Shifts.Add(shift);
    }
    public bool DeleteRoom(Guid roomId)
    {
        var room = Rooms.FirstOrDefault(r => r.Id == roomId);
        if (room == null)
            return false;
        if (Reservations.Any(r => r.Room.Id == roomId && r.Status == ResernationStatus.Confirmed))
            return false;
        Rooms.Remove(room);
        return true;
    }
}