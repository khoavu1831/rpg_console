using MySql.Data.MySqlClient;
using RPG_Console.Database;

namespace RPG_Console.Services;

class ItemService
{
    public List<Item> items = new List<Item>();
    public List<Item> GetAllInfoItems()
    {
        using (var conn = ConnectDB.GetConnection())
        {
            items.Clear();
            conn.Open();
            var queryGetItems = "SELECT * FROM items";
            using (var cmd = new MySqlCommand(queryGetItems, conn))
            {
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var item = new Item
                    {
                        ID = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString(),
                        Type = reader["type_item"].ToString(),
                        HPRestore = Convert.ToInt32(reader["hp_restore"]),
                        AttackBuff = Convert.ToInt32(reader["attack_buff"]),
                        DefBuff = Convert.ToInt32(reader["def_buff"]),
                        RateDrop = Convert.ToDouble(reader["rate_drop"])
                    };
                    items.Add(item);
                }
            }
        }
        return items;
    }
    public Item? GetItemFromRateDrop()
    {
        var items = GetAllInfoItems();
        double randomRateDrop = new Random().NextDouble();
        double totalRateDrop = 0.0;
        foreach (var item in items)
        {
            totalRateDrop += item.RateDrop;
            if (randomRateDrop <= totalRateDrop)
            {
                return item;
            }
        }
        return null;
    }
}