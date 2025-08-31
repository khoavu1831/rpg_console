namespace rpg_console.Gamecuakhoa.Common;

public static class Utils
{
    public static void Title(string msg)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine(msg);
        Console.ResetColor();
    }
    public static void Title(string msg, ConsoleColor color)
    {
        Console.WriteLine();
        Console.ForegroundColor = color;
        Console.WriteLine(msg);
        Console.ResetColor();
    }
    public static void TitleNonEndline(string msg)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write(msg);
        Console.ResetColor();
    }
    public static void TitleNonEndline(string msg, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(msg);
        Console.ResetColor();
    }
    public static void ErrorMsg(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("Error: ");
        Console.ResetColor();
        Console.WriteLine(msg);
    }

    public static void WarningMsg(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Warning: ");
        Console.ResetColor();
        Console.WriteLine(msg);
    }
    public static string EnterRequest(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(msg);
        Console.ResetColor();
        return Console.ReadLine() ?? "";
    }
    public static int EnterOption(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(msg);
        Console.ResetColor();
        int choice = Convert.ToInt32(Console.ReadLine());
        return choice;
    }
    public static int EnterOptionEndline(string question, string choice1, string choice2)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"{question}");
        Console.ResetColor();
        Console.WriteLine($"\n{choice1}\n{choice2}");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Please choose: ");
        Console.ResetColor();
        int choice = Convert.ToInt32(Console.ReadLine());
        return choice;
    }
    public static int EnterOptionInline(string question, string choice1, string choice2)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"{question}");
        Console.ResetColor();
        Console.Write($" {choice1} / {choice2}: ");

        int choice = Convert.ToInt32(Console.ReadLine());
        return choice;
    }
    public static int EnterOptionInline(string question, string choice1, string choice2, string choice3)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"{question}");
        Console.ResetColor();
        Console.Write($" {choice1} / {choice2} / {choice3}: ");

        int choice = Convert.ToInt32(Console.ReadLine());
        return choice;
    }
}