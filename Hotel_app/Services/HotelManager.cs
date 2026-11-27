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


}