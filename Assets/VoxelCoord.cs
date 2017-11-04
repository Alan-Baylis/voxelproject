using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelCoord
{
    public Coord coord;
	public bool empty = true;
    public Color color = Color.white;

    private List<Color> colors = new List<Color>();

    public VoxelCoord(Coord coord)
    {
        this.coord = coord;
    }

    public void AddColor(Color col)
    {
        if (empty)
            empty = false;
        colors.Add(col);
        ProcessColors();
    }

    private void ProcessColors()
    {

    }
}

public struct Coord
{
    public int x;
    public int y;
    public int z;

    public Coord(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}
