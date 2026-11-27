using System;
using System.Collections.Generic;
using System.Linq;
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

    public void SeedData()
    {
        var g1 = new Guest("Ana", "Anic", 1995, "ana@mail.com", "lozinka123");
        var g2 = new Guest("Ante", "Antic", 2001, "ante@mail.com", "password456");
        var g3 = new Guest("Iva", "Ivic", 1985, "iva@mail.com", "hotteli789");
        Guests.AddRange(new[] { g1, g2, g3 });

        var r101 = new Room(101, 1, RoomCategory.Standard, 2, 75.00m);
        var r102 = new Room(102, 1, RoomCategory.Standard, 3, 90.00m);
        var r204 = new Room(204, 2, RoomCategory.Deluxe, 2, 120.00m);
        var r205 = new Room(205, 2, RoomCategory.Deluxe, 4, 150.00m);
        var r301 = new Room(301, 3, RoomCategory.Suite, 4, 250.00m);
        Rooms.AddRange(new[] { r101, r102, r204, r205, r301 });

        g1.ToggleFavoriteRoom(r204);
        g1.ToggleFavoriteRoom(r301);

        var res1 = new Reservation(g1, r204, DateTime.Today.AddDays(-10), DateTime.Today.AddDays(-7), allowPastDates: true);
        var res2 = new Reservation(g1, r101, DateTime.Today.AddDays(5), DateTime.Today.AddDays(8));
        var res3 = new Reservation(g2, r204, DateTime.Today.AddDays(10), DateTime.Today.AddDays(14));
        var res4 = new Reservation(g2, r301, DateTime.Today.AddDays(1), DateTime.Today.AddDays(3));
        var res5 = new Reservation(g3, r102, DateTime.Today.AddDays(20), DateTime.Today.AddDays(25));
        Reservations.AddRange(new[] { res1, res2, res3, res4, res5 });

        var e1 = new Employee("Marko", "Maric", 1990, EmployeeGender.Male, EmployeePosition.Receptionist);
        var e2 = new Employee("Ivan", "Ivko", 1994, EmployeeGender.Male, EmployeePosition.Housekeeper);
        var e3 = new Employee("Marija", "Markic", 2002, EmployeeGender.Female, EmployeePosition.Housekeeper);
        var e4 = new Employee("Anita", "Anic", 1987, EmployeeGender.Female, EmployeePosition.Cook);

        var e5 = new Employee("Ela", "Elic", 1985, EmployeeGender.Female, EmployeePosition.Receptionist);
        var e6 = new Employee("Lucija", "Lukic", 2000, EmployeeGender.Female, EmployeePosition.Housekeeper);
        var e7 = new Employee("Nina", "Novak", 1993, EmployeeGender.Female, EmployeePosition.Housekeeper);
        var e8 = new Employee("Borna", "Babic", 1983, EmployeeGender.Male, EmployeePosition.Cook);
        Employees.AddRange(new[] { e1, e2, e3, e4, e5, e6, e7, e8 });

        var shift1 = new Shift(ShiftType.Morning, DateTime.Today.AddDays(1), e1, new List<Employee> { e2, e3 }, e4);
        Shifts.Add(shift1);
    }

    public bool RegisterGuest(Guest guest)
    {
        if (Guests.Any(g => g.Email.Equals(guest.Email, StringComparison.OrdinalIgnoreCase)))
            return false;
        Guests.Add(guest);
        return true;
    }

    public Guest? Login(string email, string password)
    {
        return Guests.FirstOrDefault(g => g.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && g.VerifyPassword(password));
    }

    public bool AddRoom(Room room)
    {
        if (Rooms.Any(r => r.RoomNumber == room.RoomNumber))
            return false;
        Rooms.Add(room);
        return true;
    }

    public int GetRoomReservationCount(Guid roomId)
    {
        return Reservations.Count(r => r.Room.Id == roomId);
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
            throw new InvalidOperationException("The room is not free during the chosen time.");
        var reservation = new Reservation(guest, room, arrival, departure);
        Reservations.Add(reservation);
        return reservation;
    }

    public void UpdateReservationDates(Reservation reservation, DateTime newArrival, DateTime newDeparture)
    {
        bool isAvailable = !Reservations.Any(r => r.Id != reservation.Id && r.Room.Id == reservation.Room.Id &&
            r.Status == ReservationStatus.Confirmed && newArrival.Date < r.CheckOutDate && newDeparture.Date > r.CheckInDate);

        if (!isAvailable)
            throw new InvalidOperationException("The room is not available for the selected dates.");

        reservation.ChangeReservationDates(newArrival, newDeparture);
    }

    public bool DeleteReservation(Guid reservationId)
    {
        var reservation = Reservations.FirstOrDefault(r => r.Id == reservationId);
        if (reservation == null || !reservation.CanBeDeleted())
            return false;

        Reservations.Remove(reservation);
        return true;
    }

    public void UpdateFinishedReservation()
    {
        foreach (var reservation in Reservations)
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
        if (Reservations.Any(r => r.Room.Id == roomId && r.Status == ReservationStatus.Confirmed))
            return false;
        Rooms.Remove(room);
        return true;
    }
}