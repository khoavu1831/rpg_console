using MySql.Data.MySqlClient;
using rpg_console.Gamecuakhoa.Database;

namespace rpg_console.Gamecuakhoa.Services;

class ItemService
{
    private static readonly Random _random = new();
    public List<Item> GetAllItems()
    {
        var items = new List<Item>();
        using (var conn = ConnectDB.GetConnection())
        {
            conn.Open();
            var queryGetItems = "SELECT * FROM items";
            using (var cmd = new MySqlCommand(queryGetItems, conn))
            {
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    items.Add(new Item
                    {
                        ID = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString(),
                        Type = reader["type_item"].ToString(),
                        HPRestore = Convert.ToInt32(reader["hp_restore"]),
                        AttackBuff = Convert.ToInt32(reader["attack_buff"]),
                        DefBuff = Convert.ToInt32(reader["def_buff"]),
                        RateDrop = Convert.ToDouble(reader["rate_drop"])
                    });
                }
            }
        }
        return items;
    }
    public Item? GetItemFromRateDrop()
    {
        var items = GetAllItems();
        double randomRate = _random.NextDouble();
        double cumulative = 0.0;

        return items.FirstOrDefault(item =>
        {
            cumulative += item.RateDrop;
            return randomRate <= cumulative;
        });
    }
}