using RPG_Console.Services;
using U = RPG_Console.Common.Utils;
class MainProgress
{
    private bool isRunning = true;
    private string? name;
    private int choice;
    public void Run()
    {
        var playerService = new PlayerService();
        Player? player = null;

        // Create new or Load player
        Console.WriteLine("---WELCOME TO RPG_CONSOLE---");
        choice = U.EnterChoiceOptionEndline("Do you want to play new or load?", "1. New", "2. Load");

        switch (choice)
        {
            // Create new player
            case 1:
                while (true)
                {
                    while (true)
                    {
                        name = U.EnterRequest("Enter your nickname: ");
                        if (string.IsNullOrEmpty(name))
                        {
                            U.WarningMsg("Name cant null or empty");
                        }
                        else
                        {
                            break;
                        }
                    }

                    bool isDuplicateName = playerService.IsDuplicatePlayerName(name);
                    if (!isDuplicateName)
                    {
                        playerService.CreatePlayer(name);
                        player = playerService.LoadPlayer(name);
                        Console.WriteLine($"Hi {name}! Having fun!");
                        break;
                    }

                    U.ErrorMsg("Name is existed, please choose another nickname");
                    choice = U.EnterChoiceOptionInline("Please choose:", "1. Enter another nickname", "0. Exit");

                    if (choice == 0)
                    {
                        Console.WriteLine("You exited.");
                        return;
                    }

                    if (player != null)
                    {
                        Console.WriteLine($"Welcome back! {player.Name}");
                        break;
                    }
                }
                break;

            // Load player
            case 2:
                while (true)
                {
                    while (true)
                    {
                        name = U.EnterRequest("Enter your nickname: ");
                        if (string.IsNullOrEmpty(name))
                        {
                            U.WarningMsg("Warning: Name cant null or empty.");
                        }
                        else
                        {
                            break;
                        }
                    }

                    player = playerService.LoadPlayer(name);

                    if (player == null)
                    {
                        U.ErrorMsg("Not found player.");
                        choice = U.EnterChoiceOptionInline("Please choose: ", "1. Try again", "2. Create new?", "0. Exit");
                    }
                    else
                    {
                        break;
                    }

                    if (choice == 2)
                    {
                        playerService.CreatePlayer(name);
                        player = playerService.LoadPlayer(name);
                        Console.WriteLine($"Hi {name}! Having fun!");
                        break;
                    }

                    if (choice == 0)
                    {
                        Console.WriteLine("You exited.");
                        return;
                    }
                }
                break;

            default:
                break;
        }

        // Main loop
        while (isRunning)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("=====RPG CONSOLE=====");
            Console.ResetColor();
            Console.WriteLine("1. Farming");
            Console.WriteLine("2. Fighting");
            Console.WriteLine("3. Inventory");
            Console.WriteLine("4. Infomation");
            Console.WriteLine("5. Ranking");
            Console.WriteLine("0. Exit game");

            choice = U.EnterChoiceOption("Choose your option: ");

            switch (choice)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    if (player != null)
                    {
                        playerService.GetInfoPlayer(player);
                    }
                    else
                    {
                        U.ErrorMsg("Player not loaded");
                    }
                    break;
                case 5:
                    U.Title("==RANKING==");
                    playerService.GetRankPlayers();
                    break;
                case 0:
                    Console.WriteLine("You exited. See you again!");
                    isRunning = false;
                    break;
                default:
                    U.WarningMsg("Please choose from 1 to 4!");
                    break;
            }
        }
    }
}