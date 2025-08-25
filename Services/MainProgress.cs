using RPG_Console.Services;

class MainProgress
{
    bool isRunning = true;
    string? name;
    int choice;
    public void Run()
    {
        var playerServices = new PlayerServices();
        Player? player = null;

        // Create new or Load player
        Console.WriteLine("---WELCOME TO RPG_CONSOLE---");
        Console.WriteLine("Do you want to play new or load?\n1. New\n2. Load");
        Console.Write("Your choice: ");

        choice = playerServices.EnterChoice();
        switch (choice)
        {
            // Create new player
            case 1:
                while (true)
                {
                    Console.Write("Enter your nickname: ");
                    name = Console.ReadLine();

                    bool isDuplicateName = playerServices.IsDuplicatePlayerName(name);
                    if (!isDuplicateName)
                    {
                        playerServices.CreatePlayer(name);
                        player = playerServices.LoadPlayer(name);
                        Console.WriteLine($"Hi {name}! Having fun!");
                        break;
                    }

                    Console.WriteLine("Error: Name is existed, please choose another nickname");
                    Console.Write(@"Please choose: 1. Enter another nickname / 0. Exit: ");
                    choice = playerServices.EnterChoice();

                    if (choice == 0)
                    {
                        Console.WriteLine("You exited.");
                        return;
                    }
                }
                break;

            // Load player
            case 2:
                while (true)
                {
                    Console.Write("Enter your nickname: ");
                    name = Console.ReadLine();

                    player = playerServices.LoadPlayer(name);

                    if (player == null)
                    {
                        Console.WriteLine("Error: Not found player.");
                        Console.Write(@"Please choose: 1. Try again / 0. Exit: ");
                        choice = playerServices.EnterChoice();
                    }

                    if (choice == 0)
                    {
                        Console.WriteLine("You exited.");
                        return;
                    }
                }

            default:
                break;
        }

        // Loop create new / load




        while (isRunning)
        {
            Console.WriteLine("-----RPG CONSOLE-----");
            Console.WriteLine("1. Farming");
            Console.WriteLine("2. Fighting");
            Console.WriteLine("3. Inventory");
            Console.WriteLine("4. Infomation");
            Console.WriteLine("5. Exit game");

            Console.Write("Choose your option: ");
            choice = playerServices.EnterChoice();

            switch (choice)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    ShowInfoPlayer(player);
                    break;
                case 5:
                    Console.WriteLine("You exited. See you again!");
                    isRunning = false;
                    break;
                default:
                    Console.WriteLine("Please choose from 1 to 4!");
                    break;
            }
        }
    }
    

    public void ShowInfoPlayer(Player player)
    {
        Console.WriteLine($"Name: {player.Name}\nLevel: {player.Level}\nEXP: {player.Exp}\nAttack: {player.Attack}\nHP: {player.HP}\nMonster Killed: {player.MonsterKilled}");
    }

}