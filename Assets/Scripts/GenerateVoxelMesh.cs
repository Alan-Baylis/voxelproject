using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateVoxelMesh : MonoBehaviour
{
    private List<Vector3> newVertices = new List<Vector3>();
    private List<int> newTriangles = new List<int>();
    private List<Vector2> newUV = new List<Vector2>();

    private float tUnit = 0.25f;
    private Vector2 tStone = new Vector2(1, 0);
    private Vector2 tGrass = new Vector2(0, 1);

    private Mesh mesh;
    private MeshCollider col;

    private int faceCount = 0;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
       
        // CubeTop(0, 0, 0);
        // CubeNorth(0, 0, 0);
        // CubeSouth(0, 0, 0);
        // CubeEast(0, 0, 0);
        // CubeWest(0, 0, 0);
        // CubeBot(0, 0, 0);
        // UpdateMesh();
    }

    public void GenerateMesh(VoxelUnit[,,] map, Coord bounds)
    {
        for (int x = 0; x < bounds.x; ++x)
        {
            for (int y = 0; y < bounds.y; ++y)
            {
                for (int z = 0; z < bounds.z; ++z)
                {
                    if (map[x,y,z].empty == false)
                    {
                        if (!IsInMapRange(x,y+1,z, bounds) || map[x,y+1,z].empty)
                        {
                            CubeTop(x, y, z);
                        }
                        if (!IsInMapRange(x,y-1,z, bounds) || map[x, y-1 ,z].empty)
                        {
                            CubeBot(x, y, z);
                        }
                        if (!IsInMapRange(x+1,y,z, bounds) || map[x+1,y,z].empty)
                        {
                            CubeEast(x, y, z);
                        }
                        if (!IsInMapRange(x-1,y,z, bounds) || map[x-1,y,z].empty)
                        {
                            CubeWest(x, y, z);
                        }
                        if (!IsInMapRange(x,y,z+1, bounds) || map[x,y,z+1].empty)
                        {
                            CubeNorth(x, y, z);
                        }
                        if (!IsInMapRange(x,y,z-1, bounds) || map[x,y,z-1].empty)
                        {
                            CubeSouth(x, y, z);
                        }
                    }
                }
            }
        }
        UpdateMesh();

    }

    private bool IsInMapRange(int x, int y, int z, Coord bounds)
    {
        return x >= 0 && x < bounds.x && y >= 0 && y < bounds.y && z >= 0 && z < bounds.z;
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.uv = newUV.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.RecalculateNormals();

        newVertices.Clear();
        newUV.Clear();
        newTriangles.Clear();

        faceCount = 0; //Fixed: Added this thanks to a bug pointed out by ratnushock!
    }

    void Cube(Vector2 texturePos)
    {
        newTriangles.Add(faceCount * 4); //1
        newTriangles.Add(faceCount * 4 + 1); //2
        newTriangles.Add(faceCount * 4 + 2); //3
        newTriangles.Add(faceCount * 4); //1
        newTriangles.Add(faceCount * 4 + 2); //3
        newTriangles.Add(faceCount * 4 + 3); //4

        newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y));
        newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y + tUnit));
        newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y + tUnit));
        newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y));

        faceCount++; // Add this line
    }

    #region Mesh ultra boring maths

    private void CubeTop(int x, int y, int z)
    {
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x, y, z));

        Vector2 texturePos;

        texturePos = tStone;

        Cube(texturePos);
    }

    private void CubeNorth(int x, int y, int z)
    {
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));

        Vector2 texturePos;

        texturePos = tStone;

        Cube(texturePos);
    }

    private void CubeEast(int x, int y, int z)
    {

        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));

        Vector2 texturePos;

        texturePos = tStone;

        Cube(texturePos);
    }

    private void CubeSouth(int x, int y, int z)
    {

        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));

        Vector2 texturePos;

        texturePos = tStone;

        Cube(texturePos);
    }

    private void CubeWest(int x, int y, int z)
    {

        newVertices.Add(new Vector3(x, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x, y - 1, z));

        Vector2 texturePos;

        texturePos = tStone;

        Cube(texturePos);
    }

    private void CubeBot(int x, int y, int z)
    {

        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));

        Vector2 texturePos;

        texturePos = tStone;

        Cube(texturePos);
    }

    #endregion


}
