using System.Drawing;

public class Player
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Radius { get; set; } = 120;
    public int LightCooldown { get; set; } = 0;
    public int LightCooldownMax { get; set; } = 90; 
    public bool IsLightReady => LightCooldown <= 0;
    
    public Player(int startX, int startY)
    {
        X = startX;
        Y = startY;
    }
    
    public void Update()
    {
        if (LightCooldown > 0)
            LightCooldown--;
    }
    
    public void UseLight()
    {
        if (IsLightReady)
            LightCooldown = LightCooldownMax;
    }
}