using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelUnit
{
	public bool empty = true;
    public Color color = Color.white;

    private List<Color> colors = new List<Color>();

    public void AddColor(Color col)
    {
        if (empty)
            empty = false;
        colors.Add(col);
        ProcessColors();
    }

    private void ProcessColors()
    {
        float r=0.0f, g=0.0f, b=0.0f, a = 0.0f;
        int count = colors.Count;
        foreach(Color col in colors)
        {
            r += col.r;
            g += col.g;
            b += col.b;
            a += col.a;
        }
        r /= count;
        g /= count;
        b /= count;
        a /= count;

        color = new Color(r, g, b, a);
    }
}

[System.Serializable]
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

    public static Coord Substract(Coord a, Coord b)
    {
        return new Coord(a.x - b.x, a.y - b.y, a.z - b.z);
    }
    
    public Vector3 Vect()
    {
        return new Vector3(x, y, z);
    }

    public string Print()
    {
        return x.ToString() + ' ' + y.ToString() + ' ' + z.ToString();
    }
    
}

public struct Triangle
{
    public Coord a;
    public Coord b;
    public Coord c;

    public Triangle (Coord a, Coord b, Coord c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
    }

    public Coord[] Array()
    {
        return new Coord[3] {a, b, c};
    }
    
}
