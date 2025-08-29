using MySql.Data.MySqlClient;
using RPG_Console.Database;

namespace RPG_Console.Services;

class MonsterService
{
    private static readonly Random _rand = new();
    public List<Monster> GetAllMonsters()
    {
        var list = new List<Monster>();
        using (var conn = ConnectDB.GetConnection())
        {
            conn.Open();
            var sql = "SELECT * FROM Monsters";
            using (var cmd = new MySqlCommand(sql, conn))
            {
                var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    list.Add(new Monster
                    {
                        ID = Convert.ToInt32(r["id"]),
                        Name = r["name"].ToString(),
                        HP = Convert.ToInt32(r["hp"]),
                        Attack = Convert.ToInt32(r["attack"]),
                        ExpDrop = Convert.ToInt32(r["exp_drop"])
                    });
                }
            }
        }
        return list;
    }
    public Monster GetRandomMonsterForLevel(int playerLevel)
    {
        var monsters = GetAllMonsters();
        if (monsters.Count == 0) throw new Exception("No monsters found");
        var bias = Math.Min(playerLevel / 3.0, 3.0);
        var sorted = monsters.OrderBy(m => m.Attack + m.HP / 10).ToList();
        int idx = (int)Math.Min(sorted.Count - 1, _rand.NextDouble() * (sorted.Count * (0.4 + 0.6 * bias / 3.0)));
        return sorted[Math.Max(0, idx)];
    }

}