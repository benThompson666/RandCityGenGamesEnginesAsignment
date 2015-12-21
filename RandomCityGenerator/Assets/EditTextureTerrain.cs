using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditTextureTerrain : MonoBehaviour
{

    float[,,] element;
    int mapX, mapY;
    
    TerrainData terrainData;
    Vector3 terrainPosition;
    public Terrain myTerrain;
    float[,,] map;
    private Vector3 lastPos;

    void Awake()
    {
        map = new float[myTerrain.terrainData.alphamapWidth, myTerrain.terrainData.alphamapHeight, myTerrain.terrainData.alphamapLayers];

        element = new float[1, 1, myTerrain.terrainData.alphamapLayers];
        terrainData = myTerrain.terrainData;
        terrainPosition = myTerrain.transform.position;

        lastPos = transform.position;
    }

    void Update()
    {
        UpdateMapOnTheTarget();
    }

    void UpdateMapOnTheTarget()
    {
        //just update if you move
        if (Vector3.Distance(transform.position, lastPos) > 1)
        {
            print("paint");
            //convert world coords to terrain coords
            mapX = (int)(((transform.position.x - terrainPosition.x) / terrainData.size.x) * terrainData.alphamapWidth);
            mapY = (int)(((transform.position.z - terrainPosition.z) / terrainData.size.z) * terrainData.alphamapHeight);

            map[mapY, mapX, 0] = element[0, 0, 0] = 0;
            map[mapY, mapX, 1] = element[0, 0, 1] = 1;

            myTerrain.terrainData.SetAlphamaps(mapX, mapY, element);

            lastPos = transform.position;
        }
    }

  
}
