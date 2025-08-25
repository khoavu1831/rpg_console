using MySql.Data.MySqlClient;

namespace RPG_Console.Database;

public class ConnectDB
{
    private static readonly string? connectionString = $"server=localhost;user=root;password=;database=rpg_console";
    public static MySqlConnection GetConnection()
    {
        return new MySqlConnection(connectionString);
    }
}