using UnityEngine;
using System.Collections;

using UnityEditor;
using UnityEngine;
using System.Collections;

public class TerrainPerlinNoise : MonoBehaviour
{

    public float Tiling = 10.0f;

    void Start()
    {
        Tiling = Random.Range(10, 1000);
        GameObject obj = gameObject;

        if (obj.GetComponent<Terrain>())
        {
            GenerateHeights(obj.GetComponent<Terrain>(), Tiling);
        }
    }
 /*   
    void Update()
    {
        
        GameObject obj = gameObject;

        if (obj.GetComponent<Terrain>())
        {
            GenerateHeights(obj.GetComponent<Terrain>(), Tiling);
        }
    }*/

    public void GenerateHeights(Terrain terrain, float tileSize)
    {
        float[,] heights = new float[terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight];

        for (int i = 0; i < terrain.terrainData.heightmapWidth; i++)
        {
            for (int k = 0; k < terrain.terrainData.heightmapHeight; k++)
            {
                heights[i, k] = Mathf.PerlinNoise(((float)i / (float)terrain.terrainData.heightmapWidth) * tileSize, ((float)k / (float)terrain.terrainData.heightmapHeight) * tileSize) / 10.0f;
            }
        }

        terrain.terrainData.SetHeights(0, 0, heights);
    }
}