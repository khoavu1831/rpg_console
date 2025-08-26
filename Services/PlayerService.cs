using MySql.Data.MySqlClient;
using RPG_Console.Database;
namespace RPG_Console.Services;

class PlayerService
{
    public void CreatePlayer(string? name)
    {
        using (var conn = ConnectDB.GetConnection())
        {
            conn.Open();
            var createPlayerQuery = "INSERT INTO players (name) VALUES (@name)";
            using (var cmd = new MySqlCommand(createPlayerQuery, conn))
            {
                cmd.Parameters.AddWithValue("@name", name);
                cmd.ExecuteNonQuery();
            }
        }
    }
    public Player? LoadPlayer(string name)
    {
        using (var conn = ConnectDB.GetConnection())
        {
            conn.Open();
            var queryLoadPlayer = "SELECT * FROM players WHERE name = @name";
            using (var cmd = new MySqlCommand(queryLoadPlayer, conn))
            {
                cmd.Parameters.AddWithValue("@name", name);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Player
                    {
                        ID = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString(),
                        Level = Convert.ToInt32(reader["level"]),
                        HP = Convert.ToInt32(reader["hp"]),
                        Attack = Convert.ToInt32(reader["atk"]),
                        Exp = Convert.ToInt32(reader["exp"]),
                        MonsterKilled = Convert.ToInt32(reader["monster_killed"])
                    };
                }
            }
        }
        return null;
    }
    public void GetInfoPlayer(Player player)
    {
        using (var conn = ConnectDB.GetConnection())
        {
            conn.Open();
            var queyGetPlayers = "SELECT * FROM players WHERE id = @id";
            using (var cmd = new MySqlCommand(queyGetPlayers, conn))
            {
                cmd.Parameters.AddWithValue("@id", player.ID);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine($"{reader["id"]} {reader["name"],10} {reader["level"],10} {reader["hp"],10} {reader["atk"],10} {reader["monster_killed"],10}");
                }
            }
        }
    }
    public void GetRankPlayers()
    {
        Console.WriteLine($"{"Top", -6} {"Name", -20} {"Level", -10} {"HP", -10} {"Attack", -10} {"MonsterKilled", -10}");
        using (var conn = ConnectDB.GetConnection())
        {
            conn.Open();
            var queryGetRankPlayers = "SELECT * FROM players ORDER BY monster_killed DESC";
            using (var cmd = new MySqlCommand(queryGetRankPlayers, conn))
            {
                var reader = cmd.ExecuteReader();
                int i = 1;
                while (reader.Read())
                {
                    Console.WriteLine($"{i, -6} {reader["name"],-20} {reader["level"],-10} {reader["hp"], -10} {reader["atk"], -10} {reader["monster_killed"],-10}");
                    i++;
                }
            }
        }
    }
    public bool IsDuplicatePlayerName(string? nameCheck)
    {
        using (var conn = ConnectDB.GetConnection())
        {
            conn.Open();
            var queryCheckNameDuplicate = "SELECT COUNT(*) FROM players WHERE name = @nameCheck";
            using (var cmd = new MySqlCommand(queryCheckNameDuplicate, conn))
            {
                cmd.Parameters.AddWithValue("@nameCheck", nameCheck);
                int count = Convert.ToInt32(cmd.ExecuteScalar());

                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
    public void ShowInfoPlayer(Player player)
    {
        Console.WriteLine($"Name: {player.Name}\nLevel: {player.Level}\nEXP: {player.Exp}\nAttack: {player.Attack}\nHP: {player.HP}\nMonster Killed: {player.MonsterKilled}");
    }
}