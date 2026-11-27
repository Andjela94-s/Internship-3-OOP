void Main()
{
    while(true)
    {
        Console.WriteLine("Welcome to the Hotel Management System");
       Console.WriteLine("1-Guests");
        Console.WriteLine("2-Reservations");
        Console.WriteLine("3-Rooms");
        Console.WriteLine("4-Staff");
        Console.WriteLine("5-Exit");
        Console.Write("Please select an option: ");
        string option = Console.ReadLine();
        switch (option)
        {
            case "1":
                GuestsMenu();
                break;
            case "2":
                ReservationsMenu();
                break;
            case "3":
                RoomsMenu();
                break;
            case "4":
                StaffMenu();
                break;
            case "5":
                return;
            default:
                Console.WriteLine("Invalid option. Please try again.");
                break;
        }
    }
}
void GuestsMenu()
{
    while(true)
    {
        Console.WriteLine("Guests Menu");
        Console.WriteLine("1-Registration");
        Console.WriteLine("2-Sign in")
            Console.WriteLine("3-Back to Main Menu");
        Console.Write("Please select an option: ");
        string option = Console.ReadLine();
        switch (option)
        {
            case "1":
                Registration();
                break;
            case "2":
                SignIn();
                break;
            case "3":
                return;
            default:
                Console.WriteLine("Invalid option. Please try again.");
                break;
        }
    }
}