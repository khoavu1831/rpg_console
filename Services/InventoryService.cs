using MySql.Data.MySqlClient;
using RPG_Console.Database;
using U = RPG_Console.Common.Utils;

namespace RPG_Console.Services;

class InventoryService
{
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

	public List<(int ItemId, string? Name, string? Type, int Quantity)> GetInventoryItems(int playerId)
	{
		var result = new List<(int, string?, string?, int)>();
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
					result.Add((
						Convert.ToInt32(reader["item_id"]),
						reader["name"].ToString(),
						reader["type_item"].ToString(),
						Convert.ToInt32(reader["quantity"])
					));
				}
			}
		}
		return result;
	}

	public void PrintInventory(Player player)
	{
		U.Title($"=== {player.Name}'s Inventory ===");
		var items = GetInventoryItems(player.ID);
		Console.WriteLine($"{"#",-4} {"Name",-20} {"Type",-10} {"Qty",-5}");
		if (items.Count == 0) { Console.WriteLine("Empty"); return; }
		int idx = 1;
		foreach (var it in items) Console.WriteLine($"{idx++,-4} {it.Name,-20} {it.Type,-10} {it.Quantity,-5}");
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

	public void EquipWeapon(Player player, int weaponId)
	{
		int wAtk = 0, wDef = 0;
		using (var conn = ConnectDB.GetConnection())
		{
			conn.Open();
			var getW = "SELECT atk, def FROM Weapons WHERE id = @wid";
			using (var cmd = new MySqlCommand(getW, conn))
			{
				cmd.Parameters.AddWithValue("@wid", weaponId);
				var r = cmd.ExecuteReader();
				if (r.Read())
				{
					wAtk = Convert.ToInt32(r["atk"]);
					wDef = Convert.ToInt32(r["def"]);
				}
				else { U.ErrorMsg("Weapon not found"); return; }
			}

			int? current = null; int cAtk = 0, cDef = 0;
			var getEquipped = @"
				SELECT w.id, w.atk, w.def
				FROM InventoryWeapons iw JOIN Weapons w ON w.id = iw.weapon_id
				WHERE iw.player_id = @pid AND iw.is_equipped = 1
				LIMIT 1";
			using (var cmd2 = new MySqlCommand(getEquipped, conn))
			{
				cmd2.Parameters.AddWithValue("@pid", player.ID);
				var r2 = cmd2.ExecuteReader();
				if (r2.Read())
				{
					current = Convert.ToInt32(r2["id"]);
					cAtk = Convert.ToInt32(r2["atk"]);
					cDef = Convert.ToInt32(r2["def"]);
				}
			}

			var unequip = "UPDATE InventoryWeapons SET is_equipped = 0 WHERE player_id = @pid";
			using (var cmd3 = new MySqlCommand(unequip, conn))
			{
				cmd3.Parameters.AddWithValue("@pid", player.ID);
				cmd3.ExecuteNonQuery();
			}
			var ensureOwned = "SELECT COUNT(*) FROM InventoryWeapons WHERE player_id = @pid AND weapon_id = @wid";
			using (var check = new MySqlCommand(ensureOwned, conn))
			{
				check.Parameters.AddWithValue("@pid", player.ID);
				check.Parameters.AddWithValue("@wid", weaponId);
				var cnt = Convert.ToInt32(check.ExecuteScalar());
				if (cnt == 0)
				{
					var insert = "INSERT INTO InventoryWeapons (player_id, weapon_id, is_equipped) VALUES (@pid, @wid, 1)";
					using (var ins = new MySqlCommand(insert, conn))
					{
						ins.Parameters.AddWithValue("@pid", player.ID);
						ins.Parameters.AddWithValue("@wid", weaponId);
						ins.ExecuteNonQuery();
					}
				}
				else
				{
					var equip = "UPDATE InventoryWeapons SET is_equipped = 1 WHERE player_id = @pid AND weapon_id = @wid";
					using (var up = new MySqlCommand(equip, conn))
					{
						up.Parameters.AddWithValue("@pid", player.ID);
						up.Parameters.AddWithValue("@wid", weaponId);
						up.ExecuteNonQuery();
					}
				}
			}

			player.Attack = player.Attack - cAtk + wAtk;
			player.Def = player.Def - cDef + wDef;
			SavePlayerStats(player);
		}
	}

	public void PrintWeapons(Player player)
	{
		U.Title($"=== {player.Name}'s Weapons ===");
		using (var conn = ConnectDB.GetConnection())
		{
			conn.Open();
			var sql = @"
				SELECT iw.weapon_id, w.name, w.atk, w.def, iw.is_equipped
				FROM InventoryWeapons iw JOIN Weapons w ON w.id = iw.weapon_id
				WHERE iw.player_id = @pid ORDER BY iw.is_equipped DESC, w.name";
			using (var cmd = new MySqlCommand(sql, conn))
			{
				cmd.Parameters.AddWithValue("@pid", player.ID);
				var r = cmd.ExecuteReader();
				int i = 1;
				Console.WriteLine($"{"#",-4} {"Name",-20} {"ATK",-5} {"DEF",-5} {"Equipped",-8}");
				while (r.Read())
				{
					Console.WriteLine($"{i++,-4} {r["name"],-20} {r["atk"],-5} {r["def"],-5} {(Convert.ToInt32(r["is_equipped"]) == 1 ? "Yes" : "No"),-8}");
				}
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
		Console.WriteLine("3. View Weapons");
		Console.WriteLine("4. Equip Weapon");
		Console.WriteLine("0. Back");
		int c = U.EnterOption("Choose: ");
		switch (c)
		{
			case 1:
				PrintInventory(player);
				break;
			case 2:
				int itemId = U.EnterOption("Enter Item ID to use: ");
				if (UseItem(player, itemId)) U.Title("Used successfully.", ConsoleColor.Green);
				break;
			case 3:
				PrintWeapons(player);
				break;
			case 4:
				int wid = U.EnterOption("Enter Weapon ID to equip: ");
				EquipWeapon(player, wid);
				break;
			default:
				break;
		}
	}
}