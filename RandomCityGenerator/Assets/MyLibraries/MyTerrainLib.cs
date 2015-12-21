using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyTerrainLib : MonoBehaviour {


    public static Vector3 ConvertWorldCor2TerrCor(Vector3 wordCor)
    {
        Vector3 vecRet = new Vector3();
        Terrain ter = Terrain.activeTerrain;
        Vector3 terPosition = ter.transform.position;
        vecRet.x = ((wordCor.x - terPosition.x) / ter.terrainData.size.x) * ter.terrainData.alphamapWidth;
        vecRet.z = ((wordCor.z - terPosition.z) / ter.terrainData.size.z) * ter.terrainData.alphamapHeight;
        return vecRet;
    }

    public static void PlaceTree(Vector3 pos,GameObject treePrefab)
    {

        Vector3 spawnPos = ConvertWorldCor2TerrCor(pos);
        TreeInstance treeInstance = new TreeInstance();
        TreePrototype tp = new TreePrototype();

        TreePrototype[] tpro = new TreePrototype[1];
        tpro[0] = new TreePrototype();
        tpro[0].prefab = treePrefab;
        Terrain _Terrain2 = Terrain.activeTerrain;
        _Terrain2.terrainData.treePrototypes = tpro;

        treeInstance.color = new Color(1, 1, 1);
        treeInstance.lightmapColor = new Color(1, 1, 1);
        treeInstance.heightScale = 1;
        treeInstance.widthScale = 1;

        var terrainLocalPos = pos - _Terrain2.transform.position;
        var normalizedPos = new Vector2(Mathf.InverseLerp(0.0f, _Terrain2.terrainData.size.x, terrainLocalPos.x),
                                    Mathf.InverseLerp(0.0f, _Terrain2.terrainData.size.z, terrainLocalPos.z));
        var terrainNormal = _Terrain2.terrainData.GetInterpolatedNormal(normalizedPos.x, normalizedPos.y);
        Vector3 treePos = new Vector3(normalizedPos.x, 0, normalizedPos.y) + terrainNormal;

        treeInstance.position = treePos;


        Debug.Log(terrainNormal + " " + normalizedPos.x + " " + normalizedPos.y);
        _Terrain2.AddTreeInstance(treeInstance);
        _Terrain2.Flush();

    }

    public static void RemoveTrees()
    {
        Terrain _Terrain2=Terrain.activeTerrain;
       
            List<TreeInstance> newTrees = new List<TreeInstance>(_Terrain2.terrainData.treeInstances);
            
            // modify the collection
            newTrees.Clear();

            // replace the trees back into the terrainData as a built-in array
            _Terrain2.terrainData.treeInstances = newTrees.ToArray();
        
    } 


}
