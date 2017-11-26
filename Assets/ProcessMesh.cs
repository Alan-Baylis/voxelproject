using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessMesh : MonoBehaviour 
{
    public Mesh meshToCompute;

    [Header("Voxellization Parameters")]
    [Range(1.0f, 15.0f)]
    public float definition = 2.0f;


    private VoxelUnit[,,] map;
	private int xSize;
    private int ySize;
    private int zSize;

    void Start()
    {
        Process();
    }

    ///Fonction "main" qui va appeller toutes les autres fonctions
    public void Process()
    {
        SetupMap();

        foreach(Vector3 vert in meshToCompute.vertices)
        {
            AddVertToMap(vert);
        }

        GenerateVoxel();
    }

    private void SetupMap()
    {
        ///Generate map according to mesh boundaries and Voxel resolution

        Vector3 size = meshToCompute.bounds.size;
        size *= definition;
        xSize = Mathf.CeilToInt(size.x);
        ySize = Mathf.CeilToInt(size.y);
        zSize = Mathf.CeilToInt(size.z);
        map = new VoxelUnit[xSize, ySize, zSize];

         for (int x = 0; x < xSize; ++x)
        {
            for (int y = 0; y < ySize; ++y)
            {
                for (int z = 0; z < zSize; ++z)
                {
                    map[x, y , z] = new VoxelUnit();
                }
            }
        }
    }

    private void GenerateVoxel()
    {
        ///Generate VoxelMesh according to map content

        for (int x = 0; x < xSize; ++x)
        {
            for (int y = 0; y < ySize; ++y)
            {
                for (int z = 0; z < zSize; ++z)
                {
                    if (map[x, y, z].empty == false)
                    {
                        Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), new Vector3(x, y, z), Quaternion.identity);
                    }
                }
            }
        }   
    }

    ///Add vertice info to map
    private void AddVertToMap(Vector3 vert)
    {
        Coord c = WorldToCoord(vert);
        map[c.x, c.y, c.z].AddColor(Color.white);
    }

    //Transform a point on the mesh into a map coordinate, according to mesh position and voxel resolution
    private Coord WorldToCoord(Vector3 worldPos)
    {
        int x,y,z;
        x =  Mathf.FloorToInt(worldPos.x*definition);
        y =  Mathf.FloorToInt(worldPos.y*definition);
        z =  Mathf.FloorToInt(worldPos.z*definition);
        return ClampIntoBounds(new Coord(x,y,z));
    }

    private Coord ClampIntoBounds(Coord c)
    {
        c.x = c.x > xSize ? xSize : c.x;
        c.y = c.y > ySize ? ySize : c.y;
        c.z = c.z > zSize ? zSize : c.z;
        return c;
    }
}
