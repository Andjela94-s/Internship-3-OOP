using System;
using Hotel_app.Enums;

namespace Hotel_app.Classes;

public class Reservation : Entity
{
    public Guest Guest { get; private set; }
    public Room Room { get; private set; }
    public DateTime CheckInDate { get; private set; }
    public DateTime CheckOutDate { get; private set; }
    public int NumberOfNights => (CheckOutDate - CheckInDate).Days;
    public decimal TotalPrice => Room.PricePerNight * NumberOfNights;
    public ReservationStatus Status { get; private set; }

    public Reservation(Guest guest, Room room, DateTime checkInDate, DateTime checkOutDate, bool allowPastDates = false)
    {
        if (guest == null)
            throw new ArgumentNullException(nameof(guest), "Guest cannot be null.");
        if (room == null)
            throw new ArgumentNullException(nameof(room), "Room cannot be null.");

        if (!allowPastDates)
            ValidateReservationDates(checkInDate, checkOutDate);
        else if (checkInDate >= checkOutDate)
            throw new ArgumentException("Check-in date must be earlier than check-out date.");

        Guest = guest;
        Room = room;
        CheckInDate = checkInDate.Date;
        CheckOutDate = checkOutDate.Date;
        Status = ReservationStatus.Confirmed;
    }

    public static void ValidateReservationDates(DateTime checkInDate, DateTime checkOutDate)
    {
        if (checkInDate.Date < DateTime.Today)
            throw new ArgumentException("Check-in date must be a future date.");
        if (checkOutDate.Date <= checkInDate.Date)
            throw new ArgumentException("Check-out date must be later than check-in date.");
    }

    public void ChangeReservationDates(DateTime newCheckInDate, DateTime newCheckOutDate)
    {
        ValidateReservationDates(newCheckInDate, newCheckOutDate);
        CheckInDate = newCheckInDate.Date;
        CheckOutDate = newCheckOutDate.Date;
        UpdateTimestamp();
    }

    public bool CanBeCancelled()
    {
        return Status == ReservationStatus.Confirmed && (CheckInDate - DateTime.Now).TotalHours > 48;
    }

    public bool CanBeDeleted()
    {
        return Status != ReservationStatus.Finished && (CheckInDate - DateTime.Now).TotalHours > 48;
    }

    public void Cancel()
    {
        if (!CanBeCancelled())
            throw new InvalidOperationException("Reservation cannot be cancelled. It must be confirmed and at least 48 hours before check-in.");
        Status = ReservationStatus.Cancelled;
        UpdateTimestamp();
    }

    public void CompleteReservation()
    {
        if (Status == ReservationStatus.Confirmed && CheckOutDate.Date <= DateTime.Today)
        {
            Status = ReservationStatus.Finished;
            UpdateTimestamp();
        }
    }

    public string GetStatus() => Status switch
    {
        ReservationStatus.Confirmed => "Confirmed",
        ReservationStatus.Cancelled => "Cancelled",
        ReservationStatus.Finished => "Finished",
        _ => Status.ToString()
    };

    public string GuestDescribe()
    {
        return $"[{Id}] Room {Room.RoomNumber} ({Room.Category}) | {CheckInDate:yyyy-MM-dd} -> {CheckOutDate:yyyy-MM-dd}" +
            $" | Nights: {NumberOfNights} | Total Price: {TotalPrice:0.00} € | Status: {GetStatus()}";
    }

    public override string Describe()
    {
        return $"[{Id}] {Guest.Name} {Guest.LastName} | Room: {Room.RoomNumber} | " +
            $"{CheckInDate:yyyy-MM-dd} -> {CheckOutDate:yyyy-MM-dd} | Nights: {NumberOfNights} | " +
            $"Total Price: {TotalPrice:0.00} € | Status: {GetStatus()}";
    }
}