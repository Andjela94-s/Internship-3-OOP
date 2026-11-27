using Hotel_app.Enums;

namespace Hotel_app.Classes;

public class Shift : Entity
{
    public ShiftType Type { get; private set; }
    public DateTime Date { get; private set; }
    public Staff Receptionist { get; private set; }
    public List <Staff> Housekeepers { get; private set; }
    public Staff Cook { get; private set; }
    public Shift(ShiftType type, DateTime date, Staff receptionist, List<Staff> housekeepers, Staff cook)
    {
        if (receptionist == null || cook == null)
            throw new ArgumentException("Shift needs a receptionist and cook!");
        if(housekeepers == null || housekeepers.Count != 2)
            throw new ArgumentException("Shift needs to have two housekeepers!");
        if(receptionist.Position != EmployeePosition.Receptionist)
            throw new ArgumentException("The selected employee is not a receptionist!");
        if (cook.Position != EmployeePosition.Cook)
            throw new ArgumentException("The selected employee is not a cook!");
        if (housekeepers.Any(e => e.Position != EmployeePosition.Housekeeper))
            throw new ArgumentException("The selected employees are not housekeepers!");

        var allEmployees = new List<Employee> { receptionist, cook };
        allEmployees.AddRange(housekeepers);
        if (allEmployees.Distinct().Count() != 4 || allEmployees.Any(e => e.IsAssignedToShift))
            throw new ArgumentException("All employees can't be in the same shift!");

        Type = type;
        Date = date.Date;
        Receptionist = receptionist;
        Housekeepers = new List<Staff>(housekeepers);
        Cook = cook;

        foreach (var employee in allEmployees)
            employee.AssignToShift();
    }
    public override string Describe()
    {
        return $"{Type} {Date:yyyy-mm-dd}";
    }
}