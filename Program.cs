class Program
{
    public static void Main(string[] args)
    {
        PlayerServices player = new PlayerServices();
        player.CreatePlayer();
        player.Out();
    }
}