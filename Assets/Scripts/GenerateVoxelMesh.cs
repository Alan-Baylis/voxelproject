using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateVoxelMesh : MonoBehaviour
{
    private List<Vector3> newVertices = new List<Vector3>();
    private List<int> newTriangles = new List<int>();
    private List<Vector2> newUV = new List<Vector2>();

    private float tUnit = 0.25f;
    private Vector2 baseUV = new Vector2(1, 0);

    private Mesh mesh;

    private int faceCount = 0;

    void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
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

        faceCount = 0;
    }

    private void CompleteGeometry()
    {
        newTriangles.Add(faceCount * 4); //1
        newTriangles.Add(faceCount * 4 + 1); //2
        newTriangles.Add(faceCount * 4 + 2); //3
        newTriangles.Add(faceCount * 4); //1
        newTriangles.Add(faceCount * 4 + 2); //3
        newTriangles.Add(faceCount * 4 + 3); //4

        newUV.Add(new Vector2(tUnit * baseUV.x + tUnit, tUnit * baseUV.y));
        newUV.Add(new Vector2(tUnit * baseUV.x + tUnit, tUnit * baseUV.y + tUnit));
        newUV.Add(new Vector2(tUnit * baseUV.x, tUnit * baseUV.y + tUnit));
        newUV.Add(new Vector2(tUnit * baseUV.x, tUnit * baseUV.y));

        faceCount++; 
    }

    #region Mesh ultra boring maths

    private void CubeTop(int x, int y, int z)
    {
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x, y, z));

        CompleteGeometry();
    }

    private void CubeNorth(int x, int y, int z)
    {
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));

        CompleteGeometry();
    }

    private void CubeEast(int x, int y, int z)
    {

        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));

        CompleteGeometry();
    }

    private void CubeSouth(int x, int y, int z)
    {

        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));

        CompleteGeometry();
    }

    private void CubeWest(int x, int y, int z)
    {

        newVertices.Add(new Vector3(x, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x, y - 1, z));

        CompleteGeometry();
    }

    private void CubeBot(int x, int y, int z)
    {

        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));

        CompleteGeometry();
    }

    #endregion
}
