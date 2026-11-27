using System;
using Hotel_app.Enums;

namespace Hotel_app.Classes;

public class Employee : Entity
{
    public string Name { get; private set; }
    public string LastName { get; private set; }
    public int BirthYear { get; private set; }
    public EmployeePosition Position { get; private set; }
    public EmployeeGender Gender { get; private set; }
    public bool IsAssignedToShift { get; private set; }

    public Employee(string name, string lastName, int birthYear, EmployeeGender gender, EmployeePosition position)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be null or empty.", nameof(lastName));
        if (birthYear <= 1900 || birthYear > DateTime.Now.Year)
            throw new ArgumentException("Birth year must be a valid year.", nameof(birthYear));

        Name = name.Trim();
        LastName = lastName.Trim();
        BirthYear = birthYear;
        Gender = gender;
        Position = position;
        IsAssignedToShift = false;
    }

    public void AssignToShift()
    {
        if (IsAssignedToShift)
            throw new InvalidOperationException("Staff member is already assigned to a shift.");
        IsAssignedToShift = true;
        UpdateTimestamp();
    }

    public void RemoveFromShift()
    {
        if (!IsAssignedToShift)
            throw new InvalidOperationException("Staff member is not assigned to any shift.");
        IsAssignedToShift = false;
        UpdateTimestamp();
    }

    public string GetPosition() => Position switch
    {
        EmployeePosition.Receptionist => "Receptionist",
        EmployeePosition.Housekeeper => "Housekeeper",
        EmployeePosition.Cook => "Cook",
        _ => Position.ToString()
    };

    public override string Describe()
    {
        return $"{GetPosition()}: {Name} {LastName} ({BirthYear})";
    }
}