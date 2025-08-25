using MySql.Data.MySqlClient;
using RPG_Console.Database;
namespace RPG_Console.Services;

class PlayerServices
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
                        ID = Convert.ToInt32(reader["id_player"]),
                        Name = reader["name"].ToString(),
                        Level = Convert.ToInt32(reader["level"])
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
            var queyGetPlayers = "SELECT * FROM players WHERE id_player = @id";
            using (var cmd = new MySqlCommand(queyGetPlayers, conn))
            {
                cmd.Parameters.AddWithValue("@id_player", player.ID);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine($"{reader["id_player"]} {reader["name"],10} {reader["level"]} {reader["hp"]} {reader["attack"]}");
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
    public int EnterChoice()
    {
        int choice = Convert.ToInt32(Console.ReadLine());
        return choice;
    }
}