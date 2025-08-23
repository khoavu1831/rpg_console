using MySql.Data.MySqlClient;

class PlayerServices
{
    public void Login()
    {
        Console.Write("Nhap ten nguoi choi:");
        string? name = Console.ReadLine();

        using (var conn = ConnectDB.GetConnection())
        {
            conn.Open();
            var query = "INSERT INTO players (name) VALUES (@name)";
            var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.ExecuteNonQuery();
        }
    }
    public void Out()
    {
        using (var conn = ConnectDB.GetConnection())
        {
            conn.Open();
            var query = "SELECT * FROM players";
            var cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"id={reader["id_player"]}-name={reader["name"]}-level={reader["level"]}");
            }
        }
    }
    public void CreatePlayer()
    {
        Console.WriteLine("Nhap ten nguoi choi: ");
        string? nameInput = Console.ReadLine();

        using (var conn = ConnectDB.GetConnection())
        {
            conn.Open();
            var query = "INSERT INTO players (name) VALUES (@name)";
            var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", nameInput);
            cmd.ExecuteNonQuery();
        }
    }
}