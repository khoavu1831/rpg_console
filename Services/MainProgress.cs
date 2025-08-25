using RPG_Console.Services;

class MainProgress
{
    public void Run()
    {
        bool isRunning = true;
        var player = new PlayerServices();
        string? name;
        int choose = 0;

        Console.WriteLine("---WELCOME TO RPG_CONSOLE---");
        Console.WriteLine("Do you want to play new or create new?\n1. New\n2. Load");
        // choose = Convert.ToInt32(Console.ReadLine());
        // switch (choose)
        // {
        //     case 1:
        //         break;
        //     case 2:
        //         break;
        //     default:
            
        // }
        do
        {
            Console.Write("Enter your nickname: ");
            name = Console.ReadLine();
            bool isDuplicateName = player.IsDuplicatePlayerName(name);

            if (isDuplicateName)
            {
                Console.WriteLine("Error: , please choose another nickname");
                Console.Write(@"Please choose: 1. Enter nickname again / 0. Exit: ");
                choose = Convert.ToInt32(Console.ReadLine());
                switch (choose)
                {
                    case 0:
                        Console.WriteLine("You exited.");
                        return;
                    case 1:
                        break;
                    default:
                        break;
                }
            }
        } while (player.IsDuplicatePlayerName(name));

        player.CreatePlayer(name);
        Console.WriteLine($"Hi {name}! Having fun!");

        while (isRunning)
        {
            Console.WriteLine("-----RPG CONSOLE-----");
            Console.WriteLine("1. Farming");
            Console.WriteLine("2. Fighting");
            Console.WriteLine("3. Inventory");
            Console.WriteLine("4. Infomation");
            Console.WriteLine("5. Exit game");

            Console.Write("Choose your option: ");
            choose = Convert.ToInt32(Console.ReadLine());

            switch (choose)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    isRunning = false;
                    break;
                default:
                    Console.WriteLine("Please choose from 1 to 4!");
                    break;
            }
        }


    }
}