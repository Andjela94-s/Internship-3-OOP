using Hotel_app.Enums;

namespace Hotel_app.Classes;

public class Reservations: Entity
{
    public Guest Guest { get; private set; }
    public Room Room { get; private set; }
    public DateTime CheckInDate { get; private set; }
    public DateTime CheckOutDate { get; private set; }
    public int NumberOfNights => (CheckOutDate - CheckInDate).Days;
    public decimal TotalPrice => Room.PricePerNight * NumberOfNights;
    public ReservationStatus Status { get; private set; }

    public Reservations(Guest guest, Room room, DateTime checkInDate, DateTime checkOutDate)
    {
        if (guest == null)
        {
            throw new ArgumentNullException(nameof(guest), "Guest cannot be null.");
        }
        if (room == null)
        {
            throw new ArgumentNullException(nameof(room), "Room cannot be null.");
        }
        if (checkInDate >= checkOutDate)
        {
            throw new ArgumentException("Check-in date must be earlier than check-out date.");
        }
        Guest = guest;
        Room = room;
        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
        Status = ReservationStatus.Confirmed;
    }
    public static void ValidateReservationDates(DateTime checkInDate, DateTime checkOutDate)
    {
        if (checkInDate.Date < DateTime.Today)
        {
            throw new ArgumentException("Check-in date must be a future date.");
        }
        if (checkOutDate.Date <= checkInDate.Date)
        {
            throw new ArgumentException("Check-out date must be later than check-in date.");
        }
    }
    public void ChangeReservationDates(DateTime newCheckInDate, DateTime newCheckOutDate)
    {
        ValidateReservationDates(newCheckInDate, newCheckOutDate);
        CheckInDate = newCheckInDate;
        CheckOutDate = newCheckOutDate;
        UpdateTimestamp();
    }
    public bool CanBeCancelled()
    {
        return Status == ReservationStatus.Confirmed && (CheckInDate - DateTime.Now).TotalHours > 48;
    }
    public void CancelReservation()
    {
        if (!CanBeCancelled())
        {
            throw new InvalidOperationException("Reservation cannot be cancelled. It must be confirmed and at least 48 hours before check-in.");
        }
        Status = ReservationStatus.Cancelled;
        UpdateTimestamp();
    }
    public void CompleteReservation()
    {
        if (Status != ReservationStatus.Confirmed)
        {
            throw new InvalidOperationException("Only confirmed reservations can be completed.");
        }
        Status = ReservationStatus.Completed;
        UpdateTimestamp();
    }
    public override string Describe()
    {
        return $"{Id} {Guest.Name} {Guest.LastName}|Room: {Room.RoomNumber}|" +
            $"{CheckInDate.ToShortDateString()} -> {CheckOutDate.ToShortDateString()}| Nights: {NumberOfNights}|" +
            $",Total Price: {TotalPrice:C}| Status: {Status}";
    }
}