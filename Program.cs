using System;
using System.Globalization;

namespace PragueParkingSingleFile
{
    class Program
    {
        static int space;
        static string plateNumber;
        static string ticket;
        static ConsoleKeyInfo keyInfo;
        public static bool succes;
        static void Main(string[] args)
        {
            //not working on other OS then Windows
            Console.SetWindowSize(200, 45);
            ArrayParkingLot.PopulateSpots();
            bool loop = true;
            while (loop)
            {
                CheckIfClosed();
                Console.Clear();
                Console.Write("Prague Parking");
                string time = $"Time: {DateTime.Now.ToString("g")}\n";
                Console.SetCursorPosition(Console.WindowWidth - time.Length, 0);
                Console.WriteLine(time);
                OutputParkingLot();
                PrintMainMenu();

                bool ok = CheckInput(Console.ReadLine(), out int choice);
                if (ok)
                {
                    switch (choice)
                    {
                        case 0:
                            Console.Clear();
                            loop = false;
                            break;
                        case 1:
                            PrintBikeOrCarMenu();
                            choice = 0;
                            do
                            {
                                keyInfo = Console.ReadKey(false);
                                if (keyInfo.KeyChar == 49)
                                {
                                    choice = 1;
                                    break;
                                }
                                else if (keyInfo.KeyChar == 50)
                                {
                                    choice = 2;
                                    break;
                                }
                                else if (keyInfo.KeyChar != 99)
                                {
                                    InvalidMsg();
                                }
                            } while (keyInfo.Key != ConsoleKey.C);

                            switch (choice)
                            {
                                case 0:
                                    break;
                                case 1:

                                    if (ParkVehical(ticket = CreateTicket(plateNumber = EnterNumberPlate().ToUpper(), choice), space = GetBestEmptySpace()))
                                    {
                                        PrintTicket(ticket, space);
                                    }
                                    break;
                                case 2:
                                    if (ParkBikes(ticket = CreateTicket(plateNumber = EnterNumberPlate().ToUpper(), choice), space = GetBestSpaceBike(), "PARK"))
                                    {
                                        PrintTicket(ticket, space);
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 2:
                            while (true)
                            {
                                space = SearchVehical(plateNumber = EnterNumberPlate());
                                if (space != 404)
                                {
                                    RetriveVehical(space, plateNumber);
                                    break;
                                }
                                else
                                {
                                    NotFoundMsg();
                                    break;
                                }
                            }
                            break;
                        case 3:
                            MoveVehical();
                            break;
                        case 4:
                            while (true)
                            {
                                space = SearchVehical(EnterNumberPlate());
                                if (space != 404)
                                {
                                    PrintSpaceNumber(space);
                                    break;
                                }
                                else
                                {
                                    NotFoundMsg();
                                    break;
                                }
                            }
                            break;
                    }
                }
                else
                {
                    InvalidMsg();
                }
            }
            static bool AreYouSure()
            {
                ConsoleKeyInfo keyInfo;
                while (true)
                {
                    do
                    {
                        Console.Write($"Press 'Y' to continue, 'c' to cancel  ");
                        keyInfo = Console.ReadKey(false);
                        if (keyInfo.KeyChar == 121)
                        {
                            return true;
                        }
                        else if (keyInfo.KeyChar != 99)
                        {
                            InvalidMsg();
                        }
                    } while (keyInfo.Key != ConsoleKey.C);
                    {
                        return false;
                    };
                }
            }
            static void CenterTxt(string txt)
            {
                int screenWidth = Console.WindowWidth;
                int txtWidth = txt.Length;
                int spaces = (screenWidth / 2) + (txtWidth / 2);
                Console.WriteLine(txt.PadLeft(spaces));
            }
            static void CheckIfClosed()
            {
                if (DateTime.Now.Hour == 0 && DateTime.Now.Minute == 0)
                {
                    Console.WriteLine($"Parking is closed.");
                    Console.WriteLine($"Vehicals will be moved");
                    Array.Clear(ArrayParkingLot.parkingSpaces, 0, ArrayParkingLot.parkingSpaces.Length);
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                }
            }
            static bool CheckInput(string input, out int returnInt)
            {
                return succes = int.TryParse(input, out returnInt);
            }
            static string CreateTicket(string plateNumber, int choice)
            {
                string ticket = string.Empty;
                switch (choice)
                {
                    case 1:
                        ticket = $"CAR#{plateNumber}#{DateTime.Now.ToString("yyMMddTHH:mm")}";
                        break;
                    case 2:
                        ticket = $"BIKE#{plateNumber}#{DateTime.Now.ToString("yyMMddTHH:mm")}";
                        break;
                    case 0:
                        break;
                }
                return ticket;
            }
            static string EnterNumberPlate()
            {

                string numberPlate;
                string invalidChars = "ßÜÅÄÖÉÁÑÓÚ";
                string invalidPlateMsg = "Numberplate can't contain special charactars!";
                while (true)
                {
                    Console.Write("\nEnter number plate: ");
                    numberPlate = Console.ReadLine().ToUpper();
                    bool isOK = true; ;
                    char[] numberPlateArray = numberPlate.ToCharArray();
                    for (int i = 0; i < numberPlateArray.Length; i++)
                    {
                        if (!Char.IsLetterOrDigit(numberPlateArray[i]))
                        {
                            Console.WriteLine(invalidPlateMsg);
                            isOK = false;
                            break;
                        }
                    }
                    foreach (var chars in invalidChars)
                    {
                        if (numberPlate.Contains(chars))
                        {
                            isOK = false;
                            Console.WriteLine(invalidPlateMsg);
                            break;
                        }
                    }
                    if (numberPlate.Length < 4 || string.IsNullOrWhiteSpace(numberPlate))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Number plate is too short....");
                        Console.ResetColor();
                    }
                    else if (numberPlate.Length > 10)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Number plate is too long....");
                        Console.ResetColor();
                    }
                    else if (isOK)
                    {
                        break;
                    }
                }
                return numberPlate;
            }
            static int GetBestEmptySpace()
            {
                int space = 101;
                for (int i = 0; i < ArrayParkingLot.parkingSpaces.Length; i++)
                {
                    if (ArrayParkingLot.parkingSpaces[i] == null)
                    {
                        space = i;
                        break;
                    }
                }

                return space;
            }
            static int GetBestSpaceBike()
            {
                int space = 101;
                for (int i = 0; i < ArrayParkingLot.parkingSpaces.Length; i++)
                {
                    if (ArrayParkingLot.parkingSpaces[i] != null)
                    {
                        if (ArrayParkingLot.parkingSpaces[i].StartsWith("BIKE") && !ArrayParkingLot.parkingSpaces[i].Contains("|"))
                        {
                            space = i;
                            break;
                        }
                        else
                        {
                            space = GetBestEmptySpace();
                        }
                    }
                }
                return space;
            }
            static int? GetNewSpace()
            {
                while (true)
                {
                    Console.Write($"Enter new space number: ");
                    string newSpaceAsString = Console.ReadLine();
                    bool succes = CheckInput(newSpaceAsString, out int newSpace);
                    if (succes)
                    {
                        return newSpace - 1;
                    }
                    else
                    {
                        InvalidMsg();
                    }
                }
            }
            static void InvalidMsg()
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nYou entered a invalid choice");
                Console.WriteLine("Please try again");
                Console.ResetColor();
            }
            static void MoveVehical()
            {
                string numberPlate = EnterNumberPlate();
                int oldSpace = SearchVehical(numberPlate);
                int? newSpace;

                if (oldSpace == 404)
                {
                    NotFoundMsg();
                }

                while (oldSpace != 404)
                {
                    newSpace = GetNewSpace(); //Get new space

                    //Check if new space is full, Contains "CAR or "|"
                    if (ArrayParkingLot.parkingSpaces[Convert.ToInt32(newSpace)] != null)
                    {
                        if (ArrayParkingLot.parkingSpaces[Convert.ToInt32(newSpace)].Contains("|") || ArrayParkingLot.parkingSpaces[Convert.ToInt32(newSpace)].Contains("CAR"))
                        {
                            SpaceOccupied();
                        }
                        else
                        {
                            //check if moving a car
                            if (ArrayParkingLot.parkingSpaces[oldSpace].StartsWith("CAR"))
                            {
                                SpaceOccupied();
                            }
                            //is not null, but haves one bike on it
                            else
                            {
                                ParkBikes(ArrayParkingLot.parkingSpaces[oldSpace], Convert.ToInt32(newSpace), "move");
                                ArrayParkingLot.parkingSpaces[oldSpace] = null;
                                break;
                            }
                        }
                    }
                    else
                    {
                        //check if space contains "|", two bikes, split to string[] vehicals
                        //check which one will be moved, 0 or 1, new variable toBeMoved
                        //one not to be moved join to space
                        if (ArrayParkingLot.parkingSpaces[Convert.ToInt32(oldSpace)].Contains("|"))
                        {
                            string[] vehicals = ArrayParkingLot.parkingSpaces[oldSpace].Split("|");
                            int? toBeMoved = null;
                            for (int i = 0; i < vehicals.Length; i++)
                            {
                                if (vehicals[i].Contains(numberPlate))
                                {
                                    toBeMoved = i;
                                    break;
                                }
                            }
                            ArrayParkingLot.parkingSpaces[Convert.ToInt32(newSpace)] = vehicals[Convert.ToInt32(toBeMoved)];
                            int notMove = (toBeMoved == 0) ? 1 : 0;
                            ArrayParkingLot.parkingSpaces[Convert.ToInt32(oldSpace)] = vehicals[Convert.ToInt32(notMove)];
                            break;
                        }
                        //park car on empty space
                        else
                        {
                            ArrayParkingLot.parkingSpaces[Convert.ToInt32(newSpace)] = ArrayParkingLot.parkingSpaces[oldSpace];
                            ArrayParkingLot.parkingSpaces[oldSpace] = null;
                            break;
                        }

                    }
                }
            }
            static void NotFoundMsg()
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Vehical not found...");
                Console.WriteLine($"Press any key to continue...");
                Console.ResetColor();
                Console.ReadKey();
            }
            static void OutputParkingLot()
            {
                int columns = 4;
                int space = 1;
                for (int i = 0; i < ArrayParkingLot.parkingSpaces.Length; i++)
                {
                    if (space <= columns && space % columns == 0)
                    {
                        Console.WriteLine();
                        space = 1;
                    }
                    if (ArrayParkingLot.parkingSpaces[i] == null)
                    {
                        Console.Write(string.Format("{0,3} | ", i + 1));
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(string.Format("{0,-60}", "Empty"));
                        Console.ResetColor();
                        space++;
                    }
                    else if (ArrayParkingLot.parkingSpaces[i].StartsWith("CAR"))
                    {
                        Console.Write(string.Format("{0,3} | ", i + 1));
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write(string.Format("{0,-60}", ArrayParkingLot.parkingSpaces[i]));
                        Console.ResetColor();
                        space++;
                    }
                    else if (ArrayParkingLot.parkingSpaces[i].Contains("|"))
                    {
                        Console.Write(string.Format("{0,3} | ", i + 1));
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write(string.Format("{0,-60}", ArrayParkingLot.parkingSpaces[i]));
                        Console.ResetColor();
                        space++;
                    }
                    else
                    {
                        Console.Write(string.Format("{0,3} | ", i + 1));
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(string.Format("{0,-60}", ArrayParkingLot.parkingSpaces[i]));
                        Console.ResetColor();
                        space++;
                    }
                }
            }
            static bool ParkBikes(string ticket, int space, string method)
            {
                if (method != "move")
                {
                    string[] temp = ticket.Split("#");
                    Console.WriteLine();
                    Console.WriteLine($"Park {temp[1]} on space number: {space + 1}");
                }
                if (AreYouSure())
                {
                    if (ArrayParkingLot.parkingSpaces[space] != null)
                    {
                        string[] vehicals = new string[] { ArrayParkingLot.parkingSpaces[space], ticket };
                        string newSpaceString = string.Join("|", vehicals);
                        ArrayParkingLot.parkingSpaces[space] = newSpaceString;
                    }
                    else
                    {
                        ArrayParkingLot.parkingSpaces[space] = ticket;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            static bool ParkVehical(string ticket, int space)
            {
                string[] temp = ticket.Split("#");
                Console.WriteLine();
                Console.WriteLine($"Park {temp[1]} on space number: {space + 1}");
                if (AreYouSure())
                {
                    ArrayParkingLot.parkingSpaces[space] = ticket;
                    return true;
                }
                else
                {
                    return false;
                }

            }
            static void PrintBikeOrCarMenu()
            {
                PrintLineForMenu();
                Console.WriteLine("{0,-10}{1,-10}{2,-10}", "1. Car", "2. Bike", "C. Cancel");
                PrintLineForMenu();
                Console.Write("Choice: ");
            }
            static void PrintLineForMenu()
            {
                for (int i = 0; i < Console.WindowWidth; i++)
                {
                    Console.Write("─");
                }
            }
            static void PrintMainMenu()
            {
                Console.WriteLine();
                PrintLineForMenu();
                Console.WriteLine("{0,-10}{1,-15}{2,-15}{3,-15}{4,-10}", "1. Park", "2. Retrive", "3. Move", "4. Search", "0. Close application");
                PrintLineForMenu();
                Console.Write("Choice: ");
            }
            static void PrintRecipt(string ticket)
            {
                string vehical = (ticket.StartsWith("C")) ? "Car" : "Bike";
                string[] temp = ticket.Split("#");
                string time = ticket.Substring(ticket.Length - 12);
                DateTime dt = DateTime.ParseExact(time, "yyMMdd'T'HH:mm", CultureInfo.InvariantCulture);
                TimeSpan diff = DateTime.Now.Subtract(dt);
                Console.Clear();
                Console.SetCursorPosition(0, Console.WindowHeight / 2 - 6);
                CenterTxt("Recipt\n");

                CenterTxt($"You had a {vehical} parked at Prague Parking\n");

                CenterTxt($"With plate number: {temp[1]}\n");

                CenterTxt($"It was parked: {dt:g}\n");

                CenterTxt($"It has been parked here for: {diff.Days} days {diff.Hours} hours and {diff.Minutes} minutes\n\n");

                CenterTxt("Press any key to print recipt to customer...");
                Console.CursorVisible = false;
                Console.ReadKey();
                Console.CursorVisible = true;
            }
            static void PrintSpaceNumber(int space)
            {
                Console.WriteLine($"Vehical is park on space: {space + 1}");
                Console.WriteLine($"Press any key to Continue");
                Console.ReadKey();
            }
            static void PrintTicket(string ticket, int parkingSpaceNumber)
            {
                string vehical = (ticket.StartsWith("C")) ? "Car" : "Bike";
                Console.Clear();
                Console.WriteLine($"Park vehical on space: {parkingSpaceNumber + 1}");
                string time = ticket.Substring(ticket.Length - 12);
                DateTime dt = DateTime.ParseExact(time, "yyMMdd'T'HH:mm", CultureInfo.InvariantCulture);
                Console.SetCursorPosition(0, Console.WindowHeight / 2 - 6);
                CenterTxt("Ticket");
                CenterTxt($"You have parked a {vehical} at Prague Parking\n");
                CenterTxt($"It's parked on space number: {parkingSpaceNumber + 1}");
                CenterTxt($"Time parked: {dt:g}");
                Console.WriteLine();
                CenterTxt($"If vehical not picked up before midnight,");
                CenterTxt($"it will be moved to a parkingspace on the outside.");
                Console.WriteLine();
                CenterTxt($"And you will have to pay a fee to get your vehical.");
                Console.WriteLine();
                CenterTxt("Press any key to print ticket to customer.");
                Console.CursorVisible = false;
                Console.ReadKey();
                Console.CursorVisible = true;
            }
            static void RetriveVehical(int space, string plateNumber)
            {
                Console.WriteLine($"Retrive {plateNumber} from space {space + 1}?");
                if (AreYouSure())
                {
                    if (ArrayParkingLot.parkingSpaces[space] != null)
                    {
                        if (!ArrayParkingLot.parkingSpaces[space].Contains("|"))
                        {
                            PrintRecipt(ArrayParkingLot.parkingSpaces[space]);
                            ArrayParkingLot.parkingSpaces[space] = null;
                        }
                        else
                        {
                            int parkingSpace = 404;
                            string[] bikes = ArrayParkingLot.parkingSpaces[space].Split("|");

                            for (int j = 0; j < bikes.Length; j++)
                            {
                                string[] temp = bikes[j].Split("#");
                                if (temp[1] != plateNumber)
                                {
                                    parkingSpace = j;

                                }
                            }
                            if (parkingSpace != 404)

                            {
                                int delete = (parkingSpace == 0) ? 1 : 0;
                                PrintRecipt(bikes[delete]);
                                ArrayParkingLot.parkingSpaces[space] = bikes[parkingSpace];
                            }
                        }
                    }

                }
            }
            static int SearchVehical(string plateNumber)
            {

                int space = 404;
                for (int i = 0; i < ArrayParkingLot.parkingSpaces.Length; i++)
                {
                    if (ArrayParkingLot.parkingSpaces[i] != null)
                    {
                        if (!ArrayParkingLot.parkingSpaces[i].Contains("|"))
                        {
                            string[] temp = ArrayParkingLot.parkingSpaces[i].Split("#");
                            if (temp[1] == plateNumber)
                            {
                                space = i;
                                break;
                            }
                        }
                        else
                        {
                            space = SearchMultipleVehicals(i, plateNumber, space);
                            if (space != 404)
                            {
                                break;
                            }
                        }

                    }
                }
                return space;
            }
            static int SearchMultipleVehicals(int i, string plateNumber, int space)
            {

                string[] bikes = ArrayParkingLot.parkingSpaces[i].Split("|");

                for (int j = 0; j < bikes.Length; j++)
                {
                    string[] temp = bikes[j].Split("#");
                    if (temp[1] == plateNumber)
                    {
                        space = i;
                    }
                }
                return space;
            }
            static void SpaceOccupied()
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"The space is occupied. Please choose another space...");
                Console.ResetColor();
            }
        }
    }
    //not the correct way to have two classes in a file, but I'm doing it this way :)
    class ArrayParkingLot
    {
        static string[] _parkingSpaces = new string[100];
        public static string[] parkingSpaces
        {
            get
            {
                return _parkingSpaces;
            }
            set { }
        }
        //for testing
        public static void PopulateSpots()
        {
            parkingSpaces[0] = "CAR#ABC123#211006T09:22";
            parkingSpaces[1] = "CAR#ABC103#211006T09:23";
            parkingSpaces[2] = "CAR#ABC113#211006T09:23";
            parkingSpaces[3] = "CAR#ABC121#211006T09:42";
            parkingSpaces[10] = "BIKE#ABC193#211006T09:32|BIKE#FED487#211006T09:33";
            parkingSpaces[11] = "CAR#BED203#211006T10:22";
            parkingSpaces[21] = "BIKE#BED313#211006T09:52|BIKE#BED299#211006T09:55";
            parkingSpaces[53] = "BIKE#BED199#211006T09:29";
            parkingSpaces[83] = "CAR#BED999#211006T09:34";
            parkingSpaces[91] = "CAR#BED699#211006T11:02";
        }
    }

}
