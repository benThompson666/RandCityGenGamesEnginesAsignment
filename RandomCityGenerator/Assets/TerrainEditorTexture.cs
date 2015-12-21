using UnityEngine;
using System.Collections;

public class TerrainEditorTexture : MonoBehaviour {

    float[,,] element;
    int mapX, mapY;
    TerrainData terrainData;
    Vector3 terrainPosition;
    public Terrain myTerrain;
    float[,,] map;
    private Vector3 lastPos;

    void Start()
    {
       

        lastPos = transform.position;
    }

    void Update()

    {
        if (lastPos != transform.position)
        {
            perimeter script;
            GameObject go = GameObject.Find("CityGenerator");
            script = (perimeter)go.GetComponent(typeof(perimeter));

            map = new float[myTerrain.terrainData.alphamapWidth, myTerrain.terrainData.alphamapHeight, myTerrain.terrainData.alphamapLayers];

            element = new float[1, 1, myTerrain.terrainData.alphamapLayers];
            terrainData = myTerrain.terrainData;
            terrainPosition = myTerrain.transform.position;

            /*
            for (int x=0;x< myTerrain.terrainData.alphamapWidth; x++)
            {
                for(int z=0;z<myTerrain.terrainData.alphamapHeight;z++)
                {

                    mapX = (int)(((x - terrainPosition.x) / terrainData.size.x) * terrainData.alphamapWidth);
                    mapY = (int)(((z - terrainPosition.z) / terrainData.size.z) * terrainData.alphamapHeight);
                    if (script.PointInPolygon(mapX,mapY))
                    {

                        map[mapY, mapX, 0] = element[0, 0, 0] = 0;
                        map[mapY, mapX, 1] = element[0, 0, 1] = 1;

                        myTerrain.terrainData.SetAlphamaps(mapX, mapY, element);
                    }
                }
            }*/
            element = new float[1, 1, myTerrain.terrainData.alphamapLayers];
            terrainData = myTerrain.terrainData;
            terrainPosition = myTerrain.transform.position;
            UpdateMapOnTheTarget();
            lastPos = transform.position;
        }
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
