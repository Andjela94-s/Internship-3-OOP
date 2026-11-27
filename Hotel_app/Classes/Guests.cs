using System;
using System.Collections.Generic;
using System.Linq;

namespace Hotel_app.Classes;

public class Guest : Entity
{
    public string Name { get; private set; }
    public string LastName { get; private set; }
    public int BirthYear { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }

    private readonly List<Room> _favoriteRooms = new();
    public IReadOnlyList<Room> FavoriteRooms => _favoriteRooms.AsReadOnly();

    public Guest(string name, string lastName, int birthYear, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be null or empty.", nameof(lastName));
        if (birthYear <= 1900 || birthYear > DateTime.Now.Year)
            throw new ArgumentException("Birth year must be a valid year.", nameof(birthYear));
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@") || !email.Contains("."))
            throw new ArgumentException("Email cannot be null or empty and must contain '@' and '.'.", nameof(email));
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            throw new ArgumentException("Password cannot be null or empty and must be at least 6 characters long.", nameof(password));

        Name = name.Trim();
        LastName = lastName.Trim();
        BirthYear = birthYear;
        Email = email.Trim();
        Password = password;
    }

    public bool VerifyPassword(string password)
    {
        return Password == password;
    }

    public void ChangeEmail(string newEmail)
    {
        if (string.IsNullOrWhiteSpace(newEmail) || !newEmail.Contains("@") || !newEmail.Contains("."))
            throw new ArgumentException("New email cannot be null or empty and must contain '@' and '.'.", nameof(newEmail));
        Email = newEmail.Trim();
        UpdateTimestamp();
    }

    public bool ToggleFavoriteRoom(Room room)
    {
        var existing = _favoriteRooms.FirstOrDefault(r => r.Id == room.Id);
        if (existing != null)
        {
            _favoriteRooms.Remove(existing);
            UpdateTimestamp();
            return false;
        }
        _favoriteRooms.Add(room);
        UpdateTimestamp();
        return true;
    }

    public bool IsFavorite(Room room) => _favoriteRooms.Any(r => r.Id == room.Id);

    public override string Describe()
    {
        return $"Guest: {Name} {LastName}, Email: {Email}";
    }
}