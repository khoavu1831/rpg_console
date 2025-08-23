
using MySql.Data.MySqlClient;

public class ConnectDB
{
    public static string connectionString = $"server=localhost;user=root;password=;database=rpg_console";
    public static MySqlConnection GetConnection()
    {
        return new MySqlConnection(connectionString);
    }
}