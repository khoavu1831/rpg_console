using khoaUtils = rpg_console.Gamecuakhoa.Common.Utils;
using khoaService = rpg_console.Gamecuakhoa.Services;

namespace rpg_console;
class Program
{
    public static void Main(string[] args)
    {
        khoaUtils.Title("=== CONSOLE GAMES ===");
        Console.WriteLine("1. gamecuakhoa");
        Console.WriteLine("2. gamecualuu");
        Console.WriteLine("3. gamecuaphong");
        var choice = khoaUtils.EnterOption("Choose your option: ");
        switch (choice)
        {
            case 1:
                var gameplay = new khoaService.MainProgress();
                gameplay.Run();
                break;
            case 2:
                break;
            case 3:
                break;
            default:
                break;
        }

    }
}