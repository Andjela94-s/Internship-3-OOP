class Guests
{
    public string Name { get; set; }
    public string LastName { get; set; }
    public int BirthYear { get; set; }
    public string Email { get; set; }
    private string Password { get; set; }
    public Guid Id { get; init; } = Guid.NewGuid();

    public Guests(string name, string lastName, int birthYear, string email, string password)
    {
        Name = name;
        LastName = lastName;
        BirthYear = birthYear;
        Email = email;
        Password = password;
    }

}