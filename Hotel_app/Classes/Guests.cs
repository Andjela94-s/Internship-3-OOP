using Hotel_app.Classes;

namespace Hotel_app.Classes;

    public class Guests : Entity
    {
    public string Name { get;private set; }
    public string LastName { get;private set; }
    public int BirthYear { get;private set; }
    public string Email { get;private set; }
    private string Password { get;private set; }

    public Guests(string name, string lastName, int birthYear, string email, string password)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));
        }
        if(string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name cannot be null or empty.", nameof(lastName));
        }
        if(birthYear <= 1900 || birthYear > DateTime.Now.Year)
        {
            throw new ArgumentException("Birth year must be a valid year.", nameof(birthYear));
        }
        if(string.IsNullOrWhiteSpace(email) || !email.Contains("@") || !email.Contains("."))
        {
            throw new ArgumentException("Email cannot be null or empty and must contain '@' and '.'.", nameof(email));
        }
        if(string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
            throw new ArgumentException("Password cannot be null or empty and must be at least 6 characters long.", nameof(password));
        }

        Name = name;
        LastName = lastName;
        BirthYear = birthYear;
        Email = email;
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
    public override string Describe()
    {
        return $"Guest: {Name} {LastName}, Email: {Email}";
    }
}