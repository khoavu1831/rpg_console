using RPG_Console.Services;
using U = RPG_Console.Common.Utils;
class MainProgress
{
    private bool isRunning = true;
    public void Run()
    {
        U.Title("--- WELCOME TO RPG_CONSOLE ---", ConsoleColor.DarkRed);

        var playerService = new PlayerService();
        var lootingService = new LootingService();

        // Create new or Load player
        Player? player = playerService.InitPlayer();
        if (player == null) return;

        // Main loop
        while (isRunning)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("\n===== RPG CONSOLE =====");
            Console.ResetColor();
            Console.WriteLine("1. Looting");
            Console.WriteLine("2. Fighting");
            Console.WriteLine("3. Inventory");
            Console.WriteLine("4. Infomation");
            Console.WriteLine("5. Ranking");
            Console.WriteLine("0. Exit game");

            var choice = U.EnterChoiceOption("Choose your option: ");

            switch (choice)
            {
                case 1:
                    lootingService.Looting();
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    playerService.PrintInfoPlayer(player);
                    break;
                case 5:
                    playerService.PrintRanking();
                    break;
                case 0:
                    U.Title("You exited. See you again!");
                    isRunning = false;
                    break;
                default:
                    U.WarningMsg("Please choose from 1 to 4!");
                    break;
            }
        }
    }
}