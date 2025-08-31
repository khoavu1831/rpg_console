using MySql.Data.MySqlClient;
using rpg_console.Gamecuakhoa.Database;
using U = rpg_console.Gamecuakhoa.Common.Utils;

namespace rpg_console.Gamecuakhoa.Services;

class PlayerService
{
    public List<Player> GetAllPlayers()
    {
        var players = new List<Player>();
        using (var conn = ConnectDB.GetConnection())
        {
            conn.Open();
            var query = "SELECT * FROM players";
            using (var cmd = new MySqlCommand(query, conn))
            {
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    players.Add(new Player
                    {
                        ID = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString(),
                        Level = Convert.ToInt32(reader["level"]),
                        HP = Convert.ToInt32(reader["hp"]),
                        Attack = Convert.ToInt32(reader["atk"]),
                        Def = Convert.ToInt32(reader["def"]),
                        Exp = Convert.ToInt32(reader["exp"]),
                        MonsterKilled = Convert.ToInt32(reader["monster_killed"])
                    });
                }
            }
        }
        return players;
    }
    public void CreatePlayer(string? name)
    {
        using (var conn = ConnectDB.GetConnection())
        {
            conn.Open();
            var query = "INSERT INTO players (name) VALUES (@name)";
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", name);
                cmd.ExecuteNonQuery();
            }
        }
    }
    public Player? LoadPlayer(string name)
    {
        return GetAllPlayers().FirstOrDefault(player => player.Name == name);
    }
    public void PrintInfoPlayer(Player player)
    {
        U.Title("===Information===");
        var info = GetAllPlayers().FirstOrDefault(p => p.ID == player.ID);

        if (player != null)
        {
            Console.WriteLine($"Name: {player.Name,-20}\nLevel: {player.Level,-10}\nEXP: {player.Exp,-10}\nHP: {player.HP,-10}\nAttack: {player.Attack,-10}\nMonster Killed: {player.MonsterKilled,-10}");
        }
    }
    public void PrintRanking()
    {
        U.Title("=== RANKING ===");
        Console.WriteLine($"{"Top",-6} {"Name",-20} {"Level",-10} {"HP",-10} {"Attack",-10} {"MonsterKilled",-10}");

        var ranked = GetAllPlayers()
                    .OrderByDescending(p => p.MonsterKilled)
                    .Select((p, index) => new { Rank = index + 1, Player = p });
        foreach (var item in ranked)
        {
            Console.WriteLine($"{item.Rank,-6} {item.Player.Name,-20} {item.Player.Level,-10} {item.Player.HP,-10} {item.Player.Attack,-10} {item.Player.MonsterKilled,-10}");
        }
    }
    public bool IsDuplicatePlayerName(string? name)
    {
        return GetAllPlayers().Any(player => player.Name == name);
    }
    public Player? InitPlayer()
    {
        Player? player = null;
        var choice = U.EnterOptionEndline("Do you want to play new or load?", "1. New", "2. Load");
        switch (choice)
        {
            // Create new player
            case 1:
                while (true)
                {
                    var name = U.EnterRequest("Enter your nickname: ");
                    if (string.IsNullOrEmpty(name))
                    {
                        U.WarningMsg("Name cant be null or empty");
                        continue;
                    }
                    if (IsDuplicatePlayerName(name))
                    {
                        U.ErrorMsg("Name is existed, please choose another nickname");
                        continue;
                    }

                    CreatePlayer(name);
                    player = LoadPlayer(name);
                    Console.WriteLine($"Hi {name}! Having fun!");
                    break;
                }
                break;

            // Load player
            case 2:
                while (true)
                {
                    var name = U.EnterRequest("Enter your nickname: ");
                    player = LoadPlayer(name);
                    if (player == null)
                    {
                        U.ErrorMsg("Not found player.");
                        choice = U.EnterOptionInline("Please choose: ", "1. Try again", "2. Create new?", "0. Exit");
                        if (choice == 2)
                        {
                            CreatePlayer(name);
                            player = LoadPlayer(name);
                            Console.WriteLine($"Hi {name}! Having fun!");
                            break;
                        }
                        else if (choice == 0) return null;
                    }
                    else break;
                }
                break;
        }
        return player;
    }
}