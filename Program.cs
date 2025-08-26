using RPG_Console.Common;
using RPG_Console.Services;
class Program
{
    public static void Main(string[] args)
    {
        var gameplay = new MainProgress();
        // gameplay.Run();
        var q = new ItemService();
        q.GetItems().ForEach(p => Console.WriteLine($"{p.ID} - {p.Name} - {p.Type} - {p.AttackBuff} - {p.HPRestore} - {p.DefBuff}"));
    }
}