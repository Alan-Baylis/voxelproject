using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessMesh : MonoBehaviour
{
    public Mesh meshToCompute;
    public GameObject voxelPrefab;

    [Header("Voxellization Parameters")]
    [Range(1.0f, 100.0f)]
    public float definition = 2.0f;

    private VoxelUnit[,,] map;
    private Triangle[] triangles;
    private int xSize;
    private int ySize;
    private int zSize;
    private Vector3 boundsTransformer;

    void Start()
    {
        Process();
    }

    ///Fonction "main" qui va appeller toutes les autres fonctions
    public void Process()
    {
        SetupMap();
        SetupTriangles();
        /* 
        foreach (Vector3 vert in meshToCompute.vertices)
        {
            AddVertToMap(vert);
        }
        */
        ConnectPoints();
        FillTriangles();
        FillMap();
        // GenerateVoxel();
        GetComponent<GenerateVoxelMesh>().GenerateMesh(map, new Coord(xSize, ySize, zSize));
    }

    ///Generate map according to mesh boundaries and Voxel resolution
    private void SetupMap()
    {
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
                    map[x, y, z] = new VoxelUnit();
                }
            }
        }
    }

    ///Generate Triangle array
    private void SetupTriangles()
    {
        List<Triangle> tris = new List<Triangle>();
        for (int i = 2; i < meshToCompute.triangles.Length; i += 3)
        {
            Coord a = WorldToCoord(meshToCompute.vertices[meshToCompute.triangles[i - 2]]);
            Coord b = WorldToCoord(meshToCompute.vertices[meshToCompute.triangles[i - 1]]);
            Coord c = WorldToCoord(meshToCompute.vertices[meshToCompute.triangles[i]]);
            Triangle tri = new Triangle(a, b, c);
            tris.Add(tri);
        }
        // Debug.Log(meshToCompute.triangles.Length);
        // Debug.Log(tris.Count);
        triangles = tris.ToArray();
    }

    private void ConnectPoints()
    {
        foreach (Triangle tri in triangles)
        {
            DrawLine(tri.a, tri.b);
            DrawLine(tri.b, tri.c);
            DrawLine(tri.a, tri.c);
        }
    }

    private void FillTriangles()
    {
        foreach (Triangle tri in triangles)
        {
            FillTriangle(tri);
        }
    }

    private void FillTriangle(Triangle tri)
    {
        Coord[] coords = tri.Array();
        int minX = xSize;
        int minY = ySize;
        int minZ = zSize;
        int maxX = 0;
        int maxY = 0;
        int maxZ = 0;
        foreach (Coord c in coords)
        {
            if (IsInMapRange(c))
            {
                if (c.x <= minX)
                {
                    minX = c.x;
                }
                if (c.x >= maxX)
                {
                    maxX = c.x;
                }

                if (c.y <= minY)
                {
                    minY = c.y;
                }
                if (c.y >= maxY)
                {
                    maxY = c.y;
                }

                if (c.z <= minZ)
                {
                    minZ = c.z;
                }
                if (c.z >= maxZ)
                {
                    maxZ = c.z;
                }
            }
            else
            {
                // Debug.Log("Ya");
            }
        }

        for (int x = minX; x < maxX; ++x)
        {
            for (int y = minY; y < maxY; ++y)
            {
                for (int z = minZ; z < maxZ; ++z)
                {
                    if (map[x, y, z].empty)
                    {
                        Coord m = new Coord(x, y, z);
                        if (IsInsideTriangle(m, tri))
                        {
                            // Debug.Log("go");
                            map[x, y, z].AddColor(Color.white);
                        }
                    }
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
                        Instantiate(voxelPrefab, new Vector3(x, y, z) - meshToCompute.bounds.extents, Quaternion.identity, transform);
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
        worldPos += meshToCompute.bounds.extents;
        int x, y, z;
        x = Mathf.FloorToInt(worldPos.x * definition);
        y = Mathf.FloorToInt(worldPos.y * definition);
        z = Mathf.FloorToInt(worldPos.z * definition);
        return ClampIntoBounds(new Coord(x, y, z));
    }

    private void DrawLine(Coord a, Coord b)
    {
        List<Coord> line = GetLine(a, b);
        foreach (Coord c in line)
        {
            if (map[c.x, c.y, c.z].empty)
            {
                map[c.x, c.y, c.z].AddColor(Color.white);
            }
        }
    }

    private List<Coord> GetLine(Coord from, Coord to)
    {
        List<Coord> line = new List<Coord>();

        int fromX = from.x;
        int fromY = from.y;
        int fromZ = from.z;

        int toX = to.x;
        int toY = to.y;
        int toZ = to.z;

        Vector3 director = Vector3.Normalize(new Vector3(toX - fromX, toY - fromY, toZ - fromZ));
        // Debug.Log(director);

        float distance = Vector3.Distance(new Vector3(fromX, fromY, fromZ), new Vector3(toX, toY, toZ));

        for (float t = 0; t <= distance; t += .5f)
        {
            int x = (int)(director.x * t + fromX);
            int y = (int)(director.y * t + fromY);
            int z = (int)(director.z * t + fromZ);

            Coord c = new Coord(x, y, z);

            if (IsInMapRange(c))
            {
                line.Add(c);
            }
        }

        return line;
    }

    //It's fucking wizardry : https://www.developpez.net/forums/d3690/general-developpement/algorithme-mathematiques/general-algorithmique/point-l-interieur-d-triangle/
    private bool IsInsideTriangle(Coord m, Triangle tri)
    {
        int check = 0;
        Vector3 ab = Coord.Substract(tri.b, tri.a).Vect();
        Vector3 ac = Coord.Substract(tri.c, tri.a).Vect();
        Vector3 am = Coord.Substract(m, tri.a).Vect();
        Vector3 ba = Coord.Substract(tri.a, tri.b).Vect();
        Vector3 bm = Coord.Substract(m, tri.b).Vect();
        Vector3 bc = Coord.Substract(tri.c, tri.b).Vect();
        Vector3 ca = Coord.Substract(tri.a, tri.c).Vect();
        Vector3 cm = Coord.Substract(m, tri.c).Vect();
        Vector3 cb = Coord.Substract(tri.b, tri.c).Vect();
        if (Vector3.Dot(Vector3.Cross(ab, am), Vector3.Cross(am, ac)) >= 0.0f) check++;
        if (Vector3.Dot(Vector3.Cross(ba, bm), Vector3.Cross(bm, bc)) >= 0.0f) check++;
        if (Vector3.Dot(Vector3.Cross(ca, cm), Vector3.Cross(cm, cb)) >= 0.0f) check++;
        Debug.Log(check);
        if (check == 3)
            return true;
        else
            return false;
    }

    private void FillMap()
    {
        List<Coord> coordsToPrint = new List<Coord>();
        for (int x = 0; x < xSize; ++x)
        {
            for (int y = 0; y < ySize; ++y)
            {
                for (int z = 0; z < zSize; ++z)
                {
                    if (map[x,y,z].empty)
                    {
                        Coord c = new Coord(x, y, z);
                        if (GetBoundaries(c) >= 5)
                        {
                            coordsToPrint.Add(c);
                        }
                    }                  
                }
            }
        }
        foreach(Coord c in coordsToPrint)
        {
            map[c.x, c.y, c.z].AddColor(Color.white);
        }
    }

    private int GetBoundaries(Coord c)
    {
        int result = 0;
        int debug = 0;
        for (int x = c.x - 1; x <= c.x + 1; ++x)
        {
            for (int y = c.y - 1; y <= c.y + 1; ++y)
            {
                for (int z = c.z - 1; z <= c.z + 1; ++z)
                {
                    if (IsInMapRange(new Coord(x, y, z)))
                    {
                        if (map[x, y, z].empty == false)
                        {
                            result++;
                        }
                    }
                }
            }
        }
        // Debug.Log(debug);
        return result;
    }

    private Coord ClampIntoBounds(Coord c)
    {
        c.x = c.x > xSize ? xSize : c.x;
        c.y = c.y > ySize ? ySize : c.y;
        c.z = c.z > zSize ? zSize : c.z;
        return c;
    }

    private bool IsInMapRange(Coord c)
    {
        return c.x >= 0 && c.x < xSize && c.y >= 0 && c.y < ySize && c.z >= 0 && c.z < zSize;
    }


}
