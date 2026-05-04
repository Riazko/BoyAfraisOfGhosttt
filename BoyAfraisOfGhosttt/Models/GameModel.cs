public class GameModel
{
    public PlayerModel Player { get; set; }
    public World World { get; set; }
    public bool IsGameOver { get; set; } = false;
    public int MaxGhosts { get; set; } = 5;
    
    public GameModel(int screenWidth, int screenHeight)
    {
        Player = new PlayerModel(screenWidth / 2, screenHeight / 2);
        World = new World();
        
        for (int i = 0; i < 3; i++)
        {
            World.AddGhost(Player.X + 200, Player.Y + 200);
        }
    }
}