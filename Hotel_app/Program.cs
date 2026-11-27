using System;
using Hotel_app.Services;

namespace Hotel_app
{
    internal class Program
    {
       static void Main(string[] args)
        {
            HotelManager hotel = new HotelManager();

            bool work = true;

            while (work)
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
                        GuestsMenu(hotel);
                        break;
                    case "2":
                        ReservationsMenu(hotel);
                        break;
                    case "3":
                        RoomsMenu(hotel);
                        break;
                    case "4":
                        StaffMenu(hotel);
                        break;
                    case "5":
                        work = false;
                        Console.WriteLine("Exiting the app...");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        Pause();
                        break;
                }
            }
        }
        void GuestsMenu()
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
                    hotel.RegisterGuest();
                        break;
                    case "2":
                    hotel.GuestLogin();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            Pause();
        }

    }
}