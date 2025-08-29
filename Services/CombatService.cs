using MySql.Data.MySqlClient;
using RPG_Console.Database;
using U = RPG_Console.Common.Utils;

namespace RPG_Console.Services;

class CombatService
{
	private readonly MonsterService _monsterService = new();
	private readonly InventoryService _inventoryService = new();
	private static readonly Random _rand = new();
	private void SavePlayer(Player player)
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
	public void Fight(Player player)
	{
		var monster = _monsterService.GetRandomMonsterForLevel(player.Level);
		U.Title($"A wild {monster.Name} appears! (HP:{monster.HP} ATK:{monster.Attack})", ConsoleColor.DarkYellow);

		int mHP = monster.HP;
		bool playerTurn = true;

		while (mHP > 0 && player.HP > 0)
		{
			Console.WriteLine("\n1. Attack  |  0. Run (lose some EXP)");
			int choice = U.EnterOption("Choose: ");
			if (choice == 0)
			{
				int expLoss = Math.Max(1, (int)(player.Level * 2));
				player.Exp = Math.Max(0, player.Exp - expLoss);
				SavePlayer(player);
				U.WarningMsg($"You ran away and lost {expLoss} EXP.");
				return;
			}
			else if (choice != 1)
			{
				U.WarningMsg("Invalid choice.");
				continue;
			}

			// Player attack
			if (playerTurn)
			{
				int dmg = Math.Max(1, player.Attack - _rand.Next(0, 3));
				mHP = Math.Max(0, mHP - dmg);
				Console.WriteLine($"You hit {monster.Name} for {dmg} dmg. Monster HP: {mHP}");
				playerTurn = false;
			}
			else
			{
				// Monster attack with delay 2-3s
				int wait = _rand.Next(1000, 3000);
				U.TitleNonEndline("Enemy acting...", ConsoleColor.Red);
				Thread.Sleep(wait);
				int dmg = Math.Max(1, monster.Attack - Math.Max(0, player.Def / 3));
				player.HP = Math.Max(0, player.HP - dmg);
				Console.WriteLine($"{monster.Name} hits you for {dmg}. Your HP: {player.HP}");
				playerTurn = true;
			}
		}

		if (player.HP <= 0)
		{
			U.ErrorMsg("You are defeated! You keep your remaining HP (0). Use items to recover.");
			SavePlayer(player);
			return;
		}

		// Victory
		U.Title($"You defeated {monster.Name}!", ConsoleColor.Green);
		player.MonsterKilled += 1;
		GainExpAndLevelUp(player, monster.ExpDrop);

		// Rate drop weapons
		if (_rand.NextDouble() < 0.15) // 15% chance
		{
			int weaponId = _rand.Next(1, 10);
			GrantWeapon(player, weaponId);
		}

		SavePlayer(player);
	}

	private void GrantWeapon(Player player, int weaponId)
	{
		using (var conn = RPG_Console.Database.ConnectDB.GetConnection())
		{
			conn.Open();
			var check = "SELECT COUNT(*) FROM InventoryWeapons WHERE player_id=@pid AND weapon_id=@wid";
			using (var cmd = new MySqlCommand(check, conn))
			{
				cmd.Parameters.AddWithValue("@pid", player.ID);
				cmd.Parameters.AddWithValue("@wid", weaponId);
				var cnt = Convert.ToInt32(cmd.ExecuteScalar());
				if (cnt == 0)
				{
					var ins = "INSERT INTO InventoryWeapons (player_id, weapon_id, is_equipped) VALUES (@pid, @wid, 0)";
					using (var cmd2 = new MySqlCommand(ins, conn))
					{
						cmd2.Parameters.AddWithValue("@pid", player.ID);
						cmd2.Parameters.AddWithValue("@wid", weaponId);
						cmd2.ExecuteNonQuery();
					}
				}
			}
		}
		U.Title("You found an equipment!", ConsoleColor.Cyan);
	}
	private void GainExpAndLevelUp(Player player, int expGain)
	{
		player.Exp += expGain;
		Console.WriteLine($"You gained {expGain} EXP. Total EXP: {player.Exp}");

		bool leveled = false;
		while (player.Exp >= player.Level * 50)
		{
			player.Exp -= player.Level * 50;
			player.Level += 1;
			player.Attack += 3;
			player.Def += 2;
			player.HP += 20;
			leveled = true;
		}
		if (leveled) U.Title($"Level Up! You are now level {player.Level}. Stats increased.", ConsoleColor.Magenta);
	}
}
