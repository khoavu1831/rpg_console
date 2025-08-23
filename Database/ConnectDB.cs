using MySql.Data.MySqlClient;

class ConnectDB
{
    private string? connectString;
    public ConnectDB(string server, string user, string password, string database)
    {
        connectString = $"server={server}; user={user}; password={password}; database={database}";
    }
    public MySqlConnection GetConnection()
    {
        return new MySqlConnection(connectString);
    }
}