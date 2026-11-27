using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Hotel_app.Enums;
using Hotel_app.Classes;
using Hotel_app.Services;

namespace Hotel_app
{
    internal class Program
    {
        private static readonly HotelManager hotel = new();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            hotel.SeedData();
            hotel.UpdateFinishedReservation();

            RunMainMenu();
        }

        #region Main Menu
        private static void RunMainMenu()
        {
            bool work = true;

            while (work)
            {
                Console.Clear();
                Console.WriteLine("Welcome to the Hotel Management System");
                Console.WriteLine("1-Guests");
                Console.WriteLine("2-Reservations");
                Console.WriteLine("3-Rooms");
                Console.WriteLine("4-Staff");
                Console.WriteLine("5-Exit");
                Console.Write("Please select an option: ");
                string? option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        GuestsMenu();
                        break;
                    case "2":
                        AdminReservationsMenu();
                        break;
                    case "3":
                        RoomsMenu();
                        break;
                    case "4":
                        StaffMenu();
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
        #endregion

        #region 1. Guests 
        private static void GuestsMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Guests Menu");
                Console.WriteLine("1-Registration");
                Console.WriteLine("2-Sign in");
                Console.WriteLine("3-Back to Main Menu");
                Console.Write("Please select an option: ");
                string? option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        RegisterGuest();
                        break;
                    case "2":
                        LoginGuest();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        Pause();
                        break;
                }
            }
        }

        private static void RegisterGuest()
        {
            Console.WriteLine("Registration of a new user");

            try
            {
                string firstname = ReadText("First name: ");
                string lastname = ReadText("Lastname: ");
                int birthYear = ReadInt("Year of birth: ", 1900, DateTime.Now.Year);
                string email = ReadText("Email: ");
                string password = ReadText("Password (min. 6 symbols): ");

                var guest = new Guest(firstname, lastname, birthYear, email, password);
                if (!Confirm($"Confirm the registration for guest {guest}?"))
                {
                    Msg("Registration cancelled.");
                    return;
                }
                if (hotel.RegisterGuest(guest))
                    Msg("Registration successful!");
                else
                    Msg("Error! Guest with that email exists.");
            }
            catch (Exception ex) { Msg($"Error: {ex.Message}"); }
        }

        private static void LoginGuest()
        {
            Console.WriteLine("Login: (Press 0 to return)");
            while (true)
            {
                string email = ReadText("Email: ");
                if (email == "0") return;
                string password = ReadText("Password: ");

                var guest = hotel.Login(email, password);
                if (guest != null)
                {
                    Msg($"Welcome, {guest.Name}!");
                    LoggedInGuestMenu(guest);
                    return;
                }
                Console.WriteLine("Incorrect email or password! Try again.");
            }
        }

        private static void LoggedInGuestMenu(Guest guest)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Guest: {guest.Name} {guest.LastName}");
                Console.WriteLine("1-My reservations\n2-New reservation\n3-Search rooms\n" +
                    "4-Cancel reservation\n5-Favourites\n6-Return to previous menu");
                Console.Write("Choose: ");
                switch (Console.ReadLine()?.Trim())
                {
                    case "1":
                        var mine = hotel.Reservations.Where(r => r.Guest.Id == guest.Id).ToList();
                        if (!mine.Any()) { Msg("You don't have reservations."); break; }
                        foreach (var r in SortReservations(mine, false))
                            Console.WriteLine(r.GuestDescribe());
                        Pause();
                        break;
                    case "2":
                        CreateReservation(guest);
                        break;
                    case "3":
                        SearchRooms(guest);
                        break;
                    case "4":
                        CancelReservation(guest);
                        break;
                    case "5":
                        FavoritesMenu(guest);
                        break;
                    case "6":
                        return;
                    default:
                        Msg("Wrong choice.");
                        break;
                }
            }
        }

        private static void CreateReservation(Guest guest)
        {
            Console.Clear();
            Console.WriteLine($"New reservation ({guest.Name} {guest.LastName}) ");
            try
            {
                DateTime arrival = ReadDate("Date of arrival (yyyy-MM-dd): ", DateTime.Today);
                DateTime departure = ReadDate("Date of departure (yyyy-MM-dd): ", arrival.AddDays(1));
                var freeRooms = hotel.FindAvailableRooms(arrival, departure);

                if (!freeRooms.Any()) { Msg("Nema slobodnih soba u tom razdoblju."); return; }

                int nights = (departure - arrival).Days;
                for (int i = 0; i < freeRooms.Count; i++)
                    Console.WriteLine($"{i + 1}. {freeRooms[i].DetailedDescription(hotel.GetRoomReservationCount(freeRooms[i].Id))} | Total: {nights * freeRooms[i].PricePerNight:0.00} €");

                int choice = ReadInt("Pick a room (0 to quit): ", 0, freeRooms.Count);
                if (choice == 0) { Msg("Action cancelled."); return; }

                var room = freeRooms[choice - 1];
                if (Confirm($"Confirm room reservation {room.RoomNumber} ({nights * room.PricePerNight:0.00} €)?"))
                {
                    hotel.AddReservation(guest, room, arrival, departure);
                    Msg("Reservation successfully created!");
                }
                else Msg("Akcija otkazana.");
            }
            catch (Exception ex) { Msg($"Error: {ex.Message}"); }
        }

        private static void CancelReservation(Guest guest)
        {
            Console.Clear();
            var mine = hotel.Reservations.Where(r => r.Guest.Id == guest.Id).ToList();
            if (!mine.Any()) { Msg("You don't have reservations."); return; }

            for (int i = 0; i < mine.Count; i++)
                Console.WriteLine($"{i + 1}. {mine[i].GuestDescribe()} {(mine[i].CanBeCancelled() ? "" : "[Can't be cancelled]")}");

            int choice = ReadInt("Choose reservation to cancel (0 to quit): ", 0, mine.Count);
            if (choice == 0) return;

            var res = mine[choice - 1];
            if (!res.CanBeCancelled()) { Msg("It's only possible to cancel a reservation 48h before arrival."); return; }

            if (Confirm($"Cancel reservation for room {res.Room.RoomNumber}?"))
            {
                res.Cancel();
                Msg("Reservation cancelled.");
            }
            else Msg("Action stopped.");
        }

        private static void FavoritesMenu(Guest guest)
        {
            Console.Clear();
            Console.WriteLine("1 - Show favourites\n2 - Add/Remove room from favourites");
            string? ch = Console.ReadLine()?.Trim();
            if (ch == "1")
            {
                if (!guest.FavoriteRooms.Any()) Console.WriteLine("No favourites.");
                foreach (var r in guest.FavoriteRooms) Console.WriteLine(r.DetailedDescription(hotel.GetRoomReservationCount(r.Id)));
                Pause();
            }
            else if (ch == "2")
            {
                for (int i = 0; i < hotel.Rooms.Count; i++)
                    Console.WriteLine($"{i + 1}. {hotel.Rooms[i]} {(guest.IsFavorite(hotel.Rooms[i]) ? "[FAVOURITE]" : "")}");
                int idx = ReadInt("Choose a room (0 to cancel): ", 0, hotel.Rooms.Count);
                if (idx > 0 && Confirm("Change status of favourites?"))
                {
                    bool added = guest.ToggleFavoriteRoom(hotel.Rooms[idx - 1]);
                    Msg(added ? "Added to favourites." : "Removed from favourites.");
                }
            }
        }
        #endregion

        #region 2. Reservations (Admin)
        private static void AdminReservationsMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== RESERVATIONS ===\n1 - Show all\n2 - Add\n3 - Search\n4 - Edit\n5 - Delete\n6 - Return");
                Console.Write("Choice: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1":
                        Console.Clear();
                        foreach (var r in SortReservations(hotel.Reservations.ToList(), true)) Console.WriteLine(r);
                        Pause();
                        break;
                    case "2":
                        Console.Clear();
                        for (int i = 0; i < hotel.Guests.Count; i++) Console.WriteLine($"{i + 1}. {hotel.Guests[i]}");
                        int gIdx = ReadInt("Choose guest (0 to quit): ", 0, hotel.Guests.Count);
                        if (gIdx > 0) CreateReservation(hotel.Guests[gIdx - 1]);
                        break;
                    case "3": SearchReservations(); break;
                    case "4": EditReservation(); break;
                    case "5": DeleteReservation(); break;
                    case "6": return;
                    default: Msg("Wrong choice"); break;
                }
            }
        }

        private static void SearchReservations()
        {
            Console.Clear();
            Console.Write("Search by: 1 - ID, 2 - guests lastname: ");
            string? ch = Console.ReadLine()?.Trim();
            var list = ch == "1"
                ? hotel.Reservations.Where(r => r.Id.ToString().Contains(ReadText("ID: "), StringComparison.OrdinalIgnoreCase)).ToList()
                : ch == "2"
                    ? hotel.Reservations.Where(r => r.Guest.LastName.Contains(ReadText("Prezime: "), StringComparison.OrdinalIgnoreCase)).ToList()
                    : new List<Reservation>();

            foreach (var r in list) Console.WriteLine($"{r}\n   Created: {r.CreatedAt} | Updated: {r.UpdatedAt}");
            if (!list.Any()) Console.WriteLine("No results.");
            Pause();
        }

        private static void EditReservation()
        {
            Console.Clear();
            foreach (var r in hotel.Reservations) Console.WriteLine(r);
            if (!Guid.TryParse(ReadText("\nID reservation to edit: "), out Guid id)) { Msg("Incorrect ID."); return; }

            var res = hotel.Reservations.FirstOrDefault(r => r.Id == id);
            if (res == null) { Msg("Reservation not found."); return; }

            try
            {
                DateTime arr = ReadDate("New arrival (yyyy-MM-dd): ", DateTime.Today);
                DateTime dep = ReadDate("New departure (yyyy-MM-dd): ", arr.AddDays(1));
                if (Confirm("Confirm change of date(s)?"))
                {
                    hotel.UpdateReservationDates(res, arr, dep);
                    Msg("Reservation updated!");
                }
                else Msg("Editing cancelled.");
            }
            catch (Exception ex) { Msg($"Error: {ex.Message}"); }
        }

        private static void DeleteReservation()
        {
            Console.Clear();
            foreach (var r in hotel.Reservations) Console.WriteLine(r);
            if (!Guid.TryParse(ReadText("\nID reservation to delete: "), out Guid id)) { Msg("Incorrect ID."); return; }

            var res = hotel.Reservations.FirstOrDefault(r => r.Id == id);
            if (res == null || !res.CanBeDeleted()) { Msg("Deletion not allowed (arrival has to be >48h and status can't be finished)."); return; }

            if (Confirm("Permanently delete reservation?"))
            {
                hotel.DeleteReservation(id);
                Msg("Reservation deleted.");
            }
            else Msg("Deleting cancelled.");
        }
        #endregion

        #region 3. Rooms
        private static void RoomsMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Rooms:");
                Console.WriteLine("\n1 - Show all\n2 - Add room\n3 - Search room\n4 - Delete room\n5 - Return");
                Console.Write("Choose: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1":
                        Console.Clear();
                        foreach (var r in SortRooms(hotel.Rooms.ToList()))
                            Console.WriteLine(r.DetailedDescription(hotel.GetRoomReservationCount(r.Id)));
                        Pause();
                        break;
                    case "2": AddRoom(); break;
                    case "3": SearchRooms(); break;
                    case "4": DeleteRoom(); break;
                    case "5": return;
                    default: Msg("Wrong choice."); break;
                }
            }
        }

        private static void AddRoom()
        {
            Console.Clear();
            try
            {
                int num = ReadInt("Room number: ", 1, 9999);
                int floor = ReadInt("Floor: ", 0, 100);
                var cat = (RoomCategory)(ReadInt("Category (1-Standard, 2-Deluxe, 3-Suite): ", 1, 3) - 1);
                int cap = ReadInt("Capacity: ", 1, 20);
                decimal price = ReadDecimal("Price of night (€): ");

                var room = new Room(num, floor, cat, cap, price);
                if (!Confirm($"Add room: {room}?")) { Msg("Aborted."); return; }

                if (hotel.AddRoom(room)) Msg("Room successfully added!");
                else Msg("Error: Room with that number already exists.");
            }
            catch (Exception ex) { Msg($"Error: {ex.Message}"); }
        }

        private static void SearchRooms(Guest? guest = null)
        {
            Console.Clear();
            Console.Write("Search by: 1 - Room number, 2 - Category: ");
            string? ch = Console.ReadLine()?.Trim();
            var list = ch == "1"
                ? hotel.Rooms.Where(r => r.RoomNumber == ReadInt("Room number: ", 1, 9999)).ToList()
                : ch == "2"
                    ? hotel.Rooms.Where(r => r.Category == (RoomCategory)(ReadInt("Category (1-Standard, 2-Deluxe, 3-Suite): ", 1, 3) - 1)).ToList()
                    : new List<Room>();

            foreach (var r in list)
                Console.WriteLine($"{r.DetailedDescription(hotel.GetRoomReservationCount(r.Id))} " +
                    $"{(guest != null && guest.IsFavorite(r) ? "[FAVORITE]" : "")}\n   Created: {r.CreatedAt} | Updated: {r.UpdatedAt}");
            if (!list.Any()) Console.WriteLine("No results.");
            Pause();
        }

        private static void DeleteRoom()
        {
            Console.Clear();
            foreach (var r in hotel.Rooms) Console.WriteLine(r.DetailedDescription(hotel.GetRoomReservationCount(r.Id)));
            Console.Write("\nDelete by: 1 - ID, 2 - Room number: ");
            string? ch = Console.ReadLine()?.Trim();

            Room? room = ch == "1" && Guid.TryParse(ReadText("ID: "), out Guid id) ? hotel.Rooms.FirstOrDefault(r => r.Id == id)
                       : ch == "2" ? hotel.Rooms.FirstOrDefault(r => r.RoomNumber == ReadInt("Room number: ", 1, 9999))
                       : null;

            if (room == null) { Msg("Room not found."); return; }
            if (!Confirm($"Delete room {room.RoomNumber}?")) { Msg("Aborted."); return; }

            if (hotel.DeleteRoom(room.Id)) Msg("Room deleted.");
            else Msg("Error: Room has an active (Confirmed) reservation.");
        }
        #endregion

        #region 4. Staff
        private static void StaffMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Staff:");
                Console.WriteLine("1 - Show all shifts\n2 - Create new shifts\n3 - Add employees\n4 - Return");
                Console.Write("Choose: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1":
                        Console.Clear();
                        foreach (var s in hotel.Shifts)
                        {
                            Console.WriteLine($"Shift: {s}\n  {s.Receptionist}");
                            foreach (var hk in s.Housekeepers) Console.WriteLine($"  {hk}");
                            Console.WriteLine($"  {s.Cook}\n");
                        }
                        Pause();
                        break;
                    case "2": CreateShift(); break;
                    case "3": AddEmployee(); break;
                    case "4": return;
                    default: Msg("Incorrect choice."); break;
                }
            }
        }

        private static void CreateShift()
        {
            Console.Clear();
            var recs = hotel.Employees.Where(e => !e.IsAssignedToShift && e.Position == EmployeePosition.Receptionist).ToList();
            var hks = hotel.Employees.Where(e => !e.IsAssignedToShift && e.Position == EmployeePosition.Housekeeper).ToList();
            var cooks = hotel.Employees.Where(e => !e.IsAssignedToShift && e.Position == EmployeePosition.Cook).ToList();

            if (recs.Count < 1 || hks.Count < 2 || cooks.Count < 1)
            {
                Msg("Not enough employees for a new shift.");
                return;
            }

            try
            {
                var type = (ShiftType)(ReadInt("Type of shift (1-Morning, 2-Afternoon, 3-Night): ", 1, 3) - 1);
                DateTime date = ReadDate("Date of shift (yyyy-MM-dd): ", DateTime.Today);

                var rec = PickEmployee(recs, "Choose receptionist:");
                var hk1 = PickEmployee(hks, "Choose 1. housekeeper:");
                var hk2 = PickEmployee(hks.Where(h => h.Id != hk1.Id).ToList(), "Choose 2. housekeeper:");
                var cook = PickEmployee(cooks, "Choose cook:");

                if (Confirm("Confirm new shift?"))
                {
                    hotel.AddShift(new Shift(type, date, rec, new List<Employee> { hk1, hk2 }, cook));
                    Msg("Shift successfully created!");
                }
                else Msg("Creating shift cancelled.");
            }
            catch (Exception ex) { Msg($"Error: {ex.Message}"); }
        }

        private static Employee PickEmployee(List<Employee> list, string title)
        {
            Console.WriteLine($"\n{title}");
            for (int i = 0; i < list.Count; i++) Console.WriteLine($"{i + 1}. {list[i]}");
            return list[ReadInt("Odabir: ", 1, list.Count) - 1];
        }

        private static void AddEmployee()
        {
            Console.Clear();
            try
            {
                string first = ReadText("Name: ");
                string last = ReadText("Lastname: ");
                int year = ReadInt("Year of birth: ", 1900, DateTime.Now.Year - 16);
                var gender = (EmployeeGender)(ReadInt("Gender (1-Male, 2-Female): ", 1, 2) - 1);
                var pos = (EmployeePosition)(ReadInt("Position (1-Cook, 2-Receptionist, 3-Housekeeper): ", 1, 3) - 1);

                var emp = new Employee(first, last, year, gender, pos);
                if (Confirm($"Add employee ({emp})?"))
                {
                    hotel.AddEmployee(emp);
                    Msg("Employee added!");
                }
                else Msg("Aborted.");
            }
            catch (Exception ex) { Msg($"Error: {ex.Message}"); }
        }
        #endregion

        #region Sort and helper functions
        private static List<Reservation> SortReservations(List<Reservation> list, bool withGuestName)
        {
            Console.WriteLine("Sort: 1-Default | 2-Arrival ↑ | 3-Arrival ↓ | 4-Nights ↑ | 5-Nights ↓" +
                " | 6-Price ↑ | 7-Price ↓" + (withGuestName ? " | 8-Lastname A-Z" : ""));
            return Console.ReadLine()?.Trim() switch
            {
                "2" => list.OrderBy(r => r.CheckInDate).ToList(),
                "3" => list.OrderByDescending(r => r.CheckInDate).ToList(),
                "4" => list.OrderBy(r => r.NumberOfNights).ToList(),
                "5" => list.OrderByDescending(r => r.NumberOfNights).ToList(),
                "6" => list.OrderBy(r => r.TotalPrice).ToList(),
                "7" => list.OrderByDescending(r => r.TotalPrice).ToList(),
                "8" when withGuestName => list.OrderBy(r => r.Guest.LastName).ToList(),
                _ => list
            };
        }

        private static List<Room> SortRooms(List<Room> list)
        {
            Console.WriteLine("Sort: 1-Date added | 2-Price ↑ | 3-Price ↓ | 4-Number of reservations ↑ | 5-Number of reservations ↓");
            return Console.ReadLine()?.Trim() switch
            {
                "2" => list.OrderBy(r => r.PricePerNight).ToList(),
                "3" => list.OrderByDescending(r => r.PricePerNight).ToList(),
                "4" => list.OrderBy(r => hotel.GetRoomReservationCount(r.Id)).ToList(),
                "5" => list.OrderByDescending(r => hotel.GetRoomReservationCount(r.Id)).ToList(),
                _ => list.OrderBy(r => r.CreatedAt).ToList()
            };
        }

        private static string ReadText(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? s = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(s)) return s;
                Console.WriteLine("Input can't be empty.");
            }
        }

        private static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int v) && v >= min && v <= max) return v;
                Console.WriteLine($"Add number between {min} and {max}.");
            }
        }

        private static decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (decimal.TryParse(Console.ReadLine()?.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal v) && v > 0) return v;
                Console.WriteLine("Enter a positive number.");
            }
        }

        private static DateTime ReadDate(string prompt, DateTime min)
        {
            while (true)
            {
                Console.Write(prompt);
                if (DateTime.TryParseExact(Console.ReadLine()?.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d) && d >= min) return d;
                Console.WriteLine($"Enter correct date (yyyy-MM-dd), min. {min:yyyy-MM-dd}.");
            }
        }

        private static bool Confirm(string q)
        {
            while (true)
            {
                Console.Write($"\n{q} (y/n): ");
                string? a = Console.ReadLine()?.Trim().ToLower();
                if (a == "y") return true;
                if (a == "n") return false;
            }
        }

        private static void Msg(string text) { Console.WriteLine($"\n{text}"); Pause(); }
        private static void Pause() { Console.WriteLine("\nPress any key..."); Console.ReadKey(true); }
        #endregion
    }
}