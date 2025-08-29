using U = RPG_Console.Common.Utils;
namespace RPG_Console.Services;

class LootingService
{
	private readonly ItemService _itemService = new();
	private readonly InventoryService _inventoryService = new();

	public List<Item> LootItems()
	{
		var lootedItems = new List<Item>();
		bool isLooting = true;

		while (isLooting)
		{
			Thread.Sleep(new Random().Next(3000, 8000));
			var itemDrop = _itemService.GetItemFromRateDrop();

			if (itemDrop != null)
			{
				lootedItems.Add(itemDrop);
				Console.WriteLine($"[{DateTime.Now:T}] You loot [{itemDrop.Name}]");
			}

			Console.SetCursorPosition(0, Console.CursorTop);
			U.TitleNonEndline("Press \"s\" to stop looting...".PadRight(Console.WindowWidth), ConsoleColor.Yellow);
			Console.SetCursorPosition(0, Console.WindowHeight - 1);

			if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.S)
			{
				isLooting = false;
			}
		}

		return lootedItems;
	}

	public void Looting(Player player)
	{
		U.Title("Looting...");
		var items = LootItems();
		U.Title("You stopped loot.", ConsoleColor.Green);

		if (items.Count > 0) _inventoryService.AddItemsToInventory(player.ID, items);

		U.Title("=== Summary ===");
		items.Select((item, index) => new { Index = index + 1, Item = item })
			.ToList()
			.ForEach(x => Console.WriteLine($"{x.Index}. {x.Item.Name} - DropRate: {x.Item.RateDrop}"));

		Console.WriteLine($"Total: {items.Count} items");
	}
}