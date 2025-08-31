using MySql.Data.MySqlClient;
using rpg_console.Gamecuakhoa.Database;
using U = rpg_console.Gamecuakhoa.Common.Utils;

namespace rpg_console.Gamecuakhoa.Services;

class InventoryService
{
	public List<(int ItemId, string? Name, string? Type, int Quantity)> GetInventoryItems(int playerId)
	{
		var inventoryWithItems = new List<(int, string?, string?, int)>();
		using (var conn = ConnectDB.GetConnection())
		{
			conn.Open();
			var sql = @"
				SELECT ii.item_id, i.name, i.type_item, ii.quantity
				FROM InventoryItems ii 
				JOIN Items i ON i.id = ii.item_id
				WHERE ii.player_id = @pid
				ORDER BY i.name";
			using (var cmd = new MySqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@pid", playerId);
				var reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					inventoryWithItems.Add((
						Convert.ToInt32(reader["item_id"]),
						reader["name"].ToString(),
						reader["type_item"].ToString(),
						Convert.ToInt32(reader["quantity"])
					));
				}
			}
		}
		return inventoryWithItems;
	}
	public void AddItemToInventory(int playerId, int itemId, int quantity = 1)
	{
		using (var conn = ConnectDB.GetConnection())
		{
			conn.Open();
			var updateSql = "UPDATE InventoryItems SET quantity = quantity + @qty WHERE player_id = @pid AND item_id = @iid";
			using (var update = new MySqlCommand(updateSql, conn))
			{
				update.Parameters.AddWithValue("@qty", quantity);
				update.Parameters.AddWithValue("@pid", playerId);
				update.Parameters.AddWithValue("@iid", itemId);

				int affected = update.ExecuteNonQuery();
				if (affected == 0)
				{
					var insertSql = "INSERT INTO InventoryItems (player_id, item_id, quantity) VALUES (@pid, @iid, @qty)";
					using (var insert = new MySqlCommand(insertSql, conn))
					{
						insert.Parameters.AddWithValue("@pid", playerId);
						insert.Parameters.AddWithValue("@iid", itemId);
						insert.Parameters.AddWithValue("@qty", quantity);
						insert.ExecuteNonQuery();
					}
				}
			}
		}
	}
	public void AddItemsToInventory(int playerId, List<Item> items)
	{
		var grouped = items.GroupBy(i => i.ID).Select(g => new { ItemId = g.Key, Qty = g.Count() });
		foreach (var g in grouped) AddItemToInventory(playerId, g.ItemId, g.Qty);
	}
	public void PrintInventory(Player player)
	{
		U.Title($"=== {player.Name}'s Inventory ===");
		var items = GetInventoryItems(player.ID);
		Console.WriteLine($"{"#",-4} {"Name",-20} {"Type",-10} {"Quantity",-5}");
		if (items.Count == 0) { Console.WriteLine("Empty"); return; }
		int idx = 1;
		foreach (var it in items) Console.WriteLine($"{idx++,-4} {it.Name,-20} {it.Type,-10} {it.Quantity,-5}");
	}
	private void DecrementItem(int playerId, int itemId, int qty)
	{
		using (var conn = ConnectDB.GetConnection())
		{
			conn.Open();
			var sql = "UPDATE InventoryItems SET quantity = quantity - @qty WHERE player_id = @pid AND item_id = @iid AND quantity >= @qty";
			using (var cmd = new MySqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@qty", qty);
				cmd.Parameters.AddWithValue("@pid", playerId);
				cmd.Parameters.AddWithValue("@iid", itemId);
				cmd.ExecuteNonQuery();
			}
			var cleanup = "DELETE FROM InventoryItems WHERE player_id = @pid AND item_id = @iid AND quantity <= 0";
			using (var cmd2 = new MySqlCommand(cleanup, conn))
			{
				cmd2.Parameters.AddWithValue("@pid", playerId);
				cmd2.Parameters.AddWithValue("@iid", itemId);
				cmd2.ExecuteNonQuery();
			}
		}
	}
	public bool UseItem(Player player, int itemId)
	{
		Item? item = null;
		using (var conn = ConnectDB.GetConnection())
		{
			conn.Open();
			var getItem = "SELECT * FROM Items WHERE id = @iid";
			using (var cmd = new MySqlCommand(getItem, conn))
			{
				cmd.Parameters.AddWithValue("@iid", itemId);
				var r = cmd.ExecuteReader();
				if (r.Read())
				{
					item = new Item
					{
						ID = Convert.ToInt32(r["id"]),
						Name = r["name"].ToString(),
						Type = r["type_item"].ToString(),
						HPRestore = Convert.ToInt32(r["hp_restore"]),
						AttackBuff = Convert.ToInt32(r["attack_buff"]),
						DefBuff = Convert.ToInt32(r["def_buff"]),
						RateDrop = Convert.ToDouble(r["rate_drop"])
					};
				}
			}
		}
		if (item == null) { U.ErrorMsg("Item not found"); return false; }

		int qty = GetItemQuantity(player.ID, itemId);
		if (qty <= 0) { U.WarningMsg("You don't have this item"); return false; }

		player.HP = Math.Max(0, player.HP + item.HPRestore);
		player.Attack = Math.Max(0, player.Attack + item.AttackBuff);
		player.Def = Math.Max(0, player.Def + item.DefBuff);
		SavePlayerStats(player);

		DecrementItem(player.ID, itemId, 1);
		return true;
	}

	private int GetItemQuantity(int playerId, int itemId)
	{
		using (var conn = ConnectDB.GetConnection())
		{
			conn.Open();
			var sql = "SELECT quantity FROM InventoryItems WHERE player_id = @pid AND item_id = @iid";
			using (var cmd = new MySqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@pid", playerId);
				cmd.Parameters.AddWithValue("@iid", itemId);
				var obj = cmd.ExecuteScalar();
				return obj == null ? 0 : Convert.ToInt32(obj);
			}
		}
	}
	private void SavePlayerStats(Player player)
	{
		using (var conn = ConnectDB.GetConnection())
		{
			conn.Open();
			var sql = "UPDATE Players SET level=@lv, exp=@exp, atk=@atk, def=@def, hp=@hp, monster_killed=@mk WHERE id=@id";
			using (var cmd = new MySqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@lv", player.Level);
				cmd.Parameters.AddWithValue("@exp", player.Exp);
				cmd.Parameters.AddWithValue("@atk", player.Attack);
				cmd.Parameters.AddWithValue("@def", player.Def);
				cmd.Parameters.AddWithValue("@hp", player.HP);
				cmd.Parameters.AddWithValue("@mk", player.MonsterKilled);
				cmd.Parameters.AddWithValue("@id", player.ID);
				cmd.ExecuteNonQuery();
			}
		}
	}
	public void ShowInventoryMenu(Player player)
	{
		U.Title("=== Inventory ===");
		Console.WriteLine("1. View Items");
		Console.WriteLine("2. Use Item");
		Console.WriteLine("0. Back");
		int c = U.EnterOption("Choose: ");
		switch (c)
		{
			case 1:
				PrintInventory(player);
				ShowInventoryMenu(player);
				break;
			case 2:
				int itemId = U.EnterOption("Enter Item ID to use: ");
				if (UseItem(player, itemId)) U.Title("Used successfully.", ConsoleColor.Green);
				ShowInventoryMenu(player);
				break;
			default:
				break;
		}
	}
}