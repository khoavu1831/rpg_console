using MySql.Data.MySqlClient;
using RPG_Console.Database;
using U = RPG_Console.Common.Utils;
namespace RPG_Console.Services;

class LootingService
{
    public List<Item> LootItems()
    {
        var itemService = new ItemService();
        var itemsLoot = new List<Item>();
        var loop = true;

        U.Title("Press \"s\" to stop looting...");

        while (loop)
        {
            var deplay = new Random().Next(3000, 8000);
            Thread.Sleep(deplay);

            var itemDrop = itemService.GetItemFromRateDrop();
            if (itemDrop != null)
            {
                itemsLoot.Add(itemDrop);
                Console.WriteLine($"[{DateTime.Now:T}] You loot [{itemDrop.Name}]");
            }

            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.S)
                {
                    loop = false;
                }

            }
        }
        return itemsLoot;
    }
    public void Looting()
    {
        var itemsAfterLoot = LootItems();
        U.Title("===Summary===");
        int count = 1;
        // itemsAfterLoot.OrderBy(p => p.RateDrop).ToList().ForEach(p => Console.WriteLine(p.RateDrop)); // Check rate 
        foreach (var item in itemsAfterLoot)
        {
            Console.WriteLine($"{count} - {item.Name} - {item.RateDrop}");
            count++;
        }
        Console.WriteLine($"Total: {itemsAfterLoot.ToArray().Length} items");
    }
}