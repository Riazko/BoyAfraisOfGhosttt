using System;
using System.Collections.Generic;
using BoyAfraisOfGhosttt.Models;

public class World
{
    public List<Ghost> Ghosts { get; set; }
    public int Width { get; set; } = 2000;
    public int Height { get; set; } = 2000;
    public int Score { get; set; } = 0;
    
    public World()
    {
        Ghosts = new List<Ghost>();
    }
    
    public void AddGhost(int x, int y)
    {
        Ghosts.Add(new Ghost(x, y));
    }
    
    public void RemoveDeadGhosts()
    {
        Ghosts.RemoveAll(g => !g.IsAlive);
    }
    
    public void SpawnNewGhostIfNeeded(int maxGhosts)
    {
        if (Ghosts.Count < maxGhosts)
        {
            var rand = new Random();
            AddGhost(rand.Next(0, Width), rand.Next(0, Height));
        }
    }
}