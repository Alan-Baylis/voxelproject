using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessMesh : MonoBehaviour 
{
    public Mesh meshToCompute;
    private VoxelUnit[,,,] map;
	
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
    }

    private void GenerateVoxel()
    {
        ///Generate VoxelMesh according to map content
    }

    private void AddVertToMap(Vector3 vert)
    {
        ///Add vertice info to map
    }

    private Coord WorldToCoord(Vector3 worldPos)
    {
        //Transform a point on the mesh into a map coordinate, according to mesh position and voxel resolution

        return new Coord(0,0,0);
    }
}
