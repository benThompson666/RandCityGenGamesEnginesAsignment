using UnityEngine;
using System.Collections;
using System;

public class terrainGenerator : MonoBehaviour {


    public float n;
    public float frequency;
    public float sampleRate;
    public float amplitude;
    public float tileSize;

    public Terrain _Terrain2;

    public GameObject TreePrefab;
    public GameObject prefab;
    public Texture2D[] TerrainTextures;
    //public TerrainData terraindata;
    //public Terrain terrain;
    public TerrainData _TerrainData;

    public perimeter script;

    void Start()
    {
        GameObject go = GameObject.Find("CityGenerator");
        script = (perimeter)go.GetComponent(typeof(perimeter));


        GameObject TerrainObj = new GameObject("TerrainObj");
        //TerrainObj.tag = "";

        _TerrainData = new TerrainData();

        _TerrainData.size = new Vector3(63, 600, 63);
        _TerrainData.heightmapResolution = 512;
        _TerrainData.baseMapResolution = 1024;
        _TerrainData.SetDetailResolution(1024, 16);

        int _heightmapWidth = _TerrainData.heightmapWidth;
        int _heightmapHeight = _TerrainData.heightmapHeight;

        SplatPrototype[] tex = new SplatPrototype[TerrainTextures.Length];
        for (int i = 0; i < TerrainTextures.Length; i++)
        {
            tex[i] = new SplatPrototype();
            tex[i].texture = TerrainTextures[i];    //Sets the texture
            tex[i].tileSize = new Vector2(10, 10);    //Sets the size of the texture
        }
        _TerrainData.splatPrototypes = tex;


        TerrainCollider _TerrainCollider = TerrainObj.AddComponent<TerrainCollider>();
        _Terrain2 = TerrainObj.AddComponent<Terrain>();

        _TerrainCollider.terrainData = _TerrainData;
        _Terrain2.terrainData = _TerrainData;


        EditTerrain(_Terrain2);









        Vector3 spawnPos = ConvertWordCor2TerrCor(new Vector3(1, 0, 1));


        TreeInstance treeInstance = new TreeInstance();

        TreePrototype tp = new TreePrototype();
        //tp.prefab = TreePrefab;


        TreePrototype[] tpro = new TreePrototype[1];
        tpro[0] = new TreePrototype();
        tpro[0].prefab = TreePrefab;
        _TerrainData.treePrototypes = tpro;

        treeInstance.prototypeIndex = 0;
        //Vector3 position = spawnPos;
        //position.y = _TerrainData.GetInterpolatedHeight(position.x,position.z);
        //treeInstance.position = position;
        //newtree.prototypeIndex = 0; // From terrain tree prototypes list index
        treeInstance.color = new Color(1, 1, 1);
        treeInstance.lightmapColor = new Color(1, 1, 1);
        treeInstance.heightScale = 1;
        treeInstance.widthScale = 1;


        var terrainLocalPos = new Vector3(20, 0, 20) - _Terrain2.transform.position;
        var normalizedPos = new Vector2(Mathf.InverseLerp(0.0f, _Terrain2.terrainData.size.x, terrainLocalPos.x),
                                    Mathf.InverseLerp(0.0f, _Terrain2.terrainData.size.z, terrainLocalPos.z));
        var terrainNormal = _Terrain2.terrainData.GetInterpolatedNormal(normalizedPos.x, normalizedPos.y);

        /*
        Vector3 treeDisplacement = new Vector3(20, 0, 20);// World Space Coords 
        treeInstance.position = new Vector3(treeDisplacement.x,treeDisplacement.y,treeDisplacement.z);
        treeInstance.position.y = _Terrain2.terrainData.GetInterpolatedHeight(treeInstance.position.x, treeInstance.position.z);  // Dont sure if I have to normalize coords 

        var newtreeterrainLocalPos = treeInstance.position - _Terrain2.transform.position;
        var newtreenormalizedPos = new Vector2(Mathf.InverseLerp(0.0f, _Terrain2.terrainData.size.x, newtreeterrainLocalPos.x),

            Mathf.InverseLerp(0.0f, _Terrain2.terrainData.size.z, newtreeterrainLocalPos.z));
        var newtreeterrainNormal = _Terrain2.terrainData.GetInterpolatedNormal(newtreenormalizedPos.x, newtreenormalizedPos.y);
        */

        Vector3 treePos = new Vector3(normalizedPos.x, 0, normalizedPos.y) + terrainNormal;

        treeInstance.position = treePos;


        Debug.Log(terrainNormal + " " + normalizedPos.x + " " + normalizedPos.y);
        _Terrain2.AddTreeInstance(treeInstance);
        _Terrain2.Flush();
        // Instantiate(prefab, new Vector3(1, 0, 1), Quaternion.identity);
        //print(terrain.terrainData.treeInstances.Length);

        /*
        for (int i = 0; i < 5; i++)
        {
            TerrainObj.AddComponent(Type.GetType("TerrainPerlinNoise"));
        }*/
    }
    void UpdateTerrainTexture(TerrainData terrainData, int textureNumberFrom, int textureNumberTo)
    {
        //get current paint mask
        float[,,] alphas = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        // make sure every grid on the terrain is modified
        for (int i = 0; i < terrainData.alphamapWidth; i++)
        {
            for (int j = 0; j < terrainData.alphamapHeight; j++)
            {
                //for each point of mask do:
                //paint all from old texture to new texture (saving already painted in new texture)
                alphas[i, j, textureNumberTo] = Mathf.Max(alphas[i, j, textureNumberFrom], alphas[i, j, textureNumberTo]);
                //set old texture mask to zero
                alphas[i, j, textureNumberFrom] = 0f;
            }
        }
        // apply the new alpha
        terrainData.SetAlphamaps(0, 0, alphas);
    }


    void CreateParkArea(TerrainData terrainData, int textureNumberFrom, int textureNumberTo)
    {
        //get current paint mask
        float[,,] alphas = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        // make sure every grid on the terrain is modified

        for (int i = 0; i < terrainData.alphamapWidth; i++)
        {
            for (int j = 0; j < terrainData.alphamapHeight; j++)
            {

                if (MyVector3Lib.PointInPolygon(i, j, script.perimeterPolygon))
                {
                    //for each point of mask do:
                    //paint all from old texture to new texture (saving already painted in new texture)
                    alphas[i, j, textureNumberTo] = Mathf.Max(alphas[i, j, textureNumberFrom], alphas[i, j, textureNumberTo]);
                    //set old texture mask to zero
                    alphas[i, j, textureNumberFrom] = 0f;
                }
            }
        }
        // apply the new alpha
        terrainData.SetAlphamaps(0, 0, alphas);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {

            ResetTerrain(_Terrain2);
            EditTerrain(_Terrain2);

        }
        if (Input.GetKeyDown(KeyCode.M))
        {


            ResetTerrain(_Terrain2);
            GenerateHeights(_Terrain2, tileSize);

        }

    }
    /*
        void UpdateTextureAt(TerrainData td,Vector3 start,Vector3 end)
        {
            Vector3 place1 = start-transform.position;
            //place1  =   place1 / td.size;
      */
    public Vector3 ConvertWordCor2TerrCor(Vector3 wordCor) {
        Vector3 vecRet = new Vector3();
        Terrain ter = Terrain.activeTerrain;
        Vector3 terPosition = ter.transform.position;
        vecRet.x = ((wordCor.x - terPosition.x) / ter.terrainData.size.x) * ter.terrainData.alphamapWidth;
        vecRet.z = ((wordCor.z - terPosition.z) / ter.terrainData.size.z) * ter.terrainData.alphamapHeight;
        return vecRet;
    }

    void EditTerrain(Terrain terain) {
        int heightMapWidth = terain.terrainData.heightmapWidth;
        int heightMapHeight = terain.terrainData.heightmapHeight;

        float[,] heights = terain.terrainData.GetHeights(0, 0, heightMapWidth, heightMapHeight);

        for (int y = 0; y < heightMapHeight; y++)
        {
            for (int x = 0; x < heightMapWidth; x++)
            {
                float _y = (float)(amplitude * Math.Sin((2 * Math.PI * n * frequency) / sampleRate));
                float _x = Mathf.Cos(x);
                heights[x, y] = (_x - _y) / 100;
            }
        }
        terain.terrainData.SetHeights(0, 0, heights);
    }

    void ResetTerrain(Terrain terain)
    {
        int heightMapWidth = terain.terrainData.heightmapWidth;
        int heightMapHeight = terain.terrainData.heightmapHeight;

        float[,] heights = terain.terrainData.GetHeights(0, 0, heightMapWidth, heightMapHeight);

        for (int y = 0; y < heightMapHeight; y++)
        {
            for (int x = 0; x < heightMapWidth; x++)
            {
                float _y = 0;
                float _x = 0;
                heights[x, y] = (_x - _y);
            }
        }
        terain.terrainData.SetHeights(0, 0, heights);
    }


    public void GenerateHeights(Terrain terrain, float tileSize)
    {
        float[,] heights = new float[terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight];

        //terrain.terrainData.heightmapWidth
        for (int i = 20; i < 100; i++)
        {
            for (int k = 50; k < 70; k++)
            {
                heights[i, k] = (Mathf.PerlinNoise(((float)i / (float)terrain.terrainData.heightmapWidth) * tileSize, ((float)k / (float)terrain.terrainData.heightmapHeight) * tileSize) / 10.0f)* amplitude;
            }
        }

        terrain.terrainData.SetHeights(0, 0, heights);
    }
    /*
    }*/



}
