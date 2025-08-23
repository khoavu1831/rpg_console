class Program
{
    public static void Main(string[] args)
    {
        ConnectDB connectDB = new ConnectDB("localhost", "root", "", "rpg_console");
        try
        {
            connectDB.GetConnection();
            Console.WriteLine("yessir");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);   
        }
    }
}