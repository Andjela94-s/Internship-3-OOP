using hotel_app.Enums;

namespace Hotel_app.Classes;

class Staff : Entity
{
    public string Name { get;private set; }
    public string LastName { get; private set; }
    public int BirthYear { get; private set; }
    public string EmployeePosition Position { get; private set; }
    public EmployeeGender Gender { get; private set; }
    public bool IsAssignedToShift { get; private set; }

    public Staff(string name, string lastName, int birthYear, EmployeePosition position, EmployeeGender gender)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));
        }
        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name cannot be null or empty.", nameof(lastName));
        }
        if (birthYear <= 1900 || birthYear > DateTime.Now.Year)
        {
            throw new ArgumentException("Birth year must be a valid year.", nameof(birthYear));
        }
        Name = name;
        LastName = lastName;
        BirthYear = birthYear;
        Position = position;
        Gender = gender;
        IsAssignedToShift = false;
    }
    public void AssignToShift()
    {
        if(IsAssignedToShift)
        {
            throw new InvalidOperationException("Staff member is already assigned to a shift.");
        }
        IsAssignedToShift = true;
        UpdateTimestamp();
    }
    public void RemoveFromShift()
    {
        if (!IsAssignedToShift)
        {
            throw new InvalidOperationException("Staff member is not assigned to any shift.");
        }
        IsAssignedToShift = false;
        UpdateTimestamp();
    }
    public override string Describe()
    {
        return $"{Position}: {Name} {LastName}, ({BirthYear}) ";
    }
}