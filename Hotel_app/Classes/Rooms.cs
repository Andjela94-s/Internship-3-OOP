using hotel_app.Enums;

namespace Hotel_app.Classes;

class Rooms : Entity
{
    public int RoomNumber { get;private set; }
    public int Floor { get; private set; }
    public RoomCategory Category { get; private set; }
    public int Capacity { get; private set; }
    public decimal PricePerNight { get; private set; }

    public Rooms(int roomNumber, int floor, RoomCategory category,int capacity, decimal pricePerNight)
    {
        if (roomNumber <= 0)
        {
            throw new ArgumentException("Room number must be a positive integer.", nameof(roomNumber));
        }
        if (floor < 0)
        {
            throw new ArgumentException("Floor must be a non-negative integer.", nameof(floor));
        }
        if(capacity <=0)
        {
            throw new ArgumentException("Capacity must be a positive integer.", nameof(capacity));
        }
        if (pricePerNight <= 0)
        {
            throw new ArgumentException("Price per night must be a positive value.", nameof(pricePerNight));
        }
        RoomNumber = roomNumber;
        Floor = floor;
        Category = category;
        Capacity = capacity;
        PricePerNight = pricePerNight;
    }
    public void UpdateDetails(int floor, RoomCategory category, int capacity, decimal pricePerNight)
    {
        if (floor < 0 || capacity <= 0 || pricePerNight <= 0)
            throw new ArgumentException("Room details are wrong!");
        Floor = floor;
        Category = category;
        Capacity = capacity;
        PricePerNight = pricePerNight;
        UpdateTimestamp();
    }
    public string DetailedDescription()
    {
        return $"[{Id}] Room {RoomNumber} | {Floor} floor | {Category} | {Capacity} people | {PricePerNight:0.00} eur/night | reservations {reservationCount}";
    }
    public override string Describe()
    {
        return $"Room {RoomNumber} | {Floor} floor | {Category} | {Capacity} people | {PricePerNight:0.00} eur/night";
    }
}