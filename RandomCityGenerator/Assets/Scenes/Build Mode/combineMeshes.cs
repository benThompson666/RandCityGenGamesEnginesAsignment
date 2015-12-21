using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class combineMeshes : MonoBehaviour
{

    GameObject[] array;
    ProceduralCube pc;
    // Use this for initialization
    void Start()
    {

    }
    // Update is called once per frame
    //on precedural gerneration

    void Update()
    {

        array = GameObject.FindGameObjectsWithTag("Ground");

        if (array.Length > 0)
        {
            pc = array[0].GetComponent<ProceduralCube>();
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];

            if (combine.Length > 0)
            {
                int i = 0;
                while (i < meshFilters.Length)
                {
                    combine[i].mesh = meshFilters[i].sharedMesh;
                    combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                    meshFilters[i].gameObject.SetActive(false);
                    i++;
                }
                transform.GetComponent<MeshFilter>().mesh = new Mesh();
                transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
                transform.gameObject.SetActive(true);
                Debug.Log("combined");
            }
            Array.Clear(array, 0, array.Length);
        }
    }





    void checkForObjectCombine()
    {
        array = GameObject.FindGameObjectsWithTag("Ground");

        if (array.Length > 0)
        {
            pc = array[0].GetComponent<ProceduralCube>();



            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];

            if (combine.Length > 0)
            {
                int i = 0;
                while (i < meshFilters.Length)
                {
                    combine[i].mesh = meshFilters[i].sharedMesh;
                    combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                    meshFilters[i].gameObject.SetActive(false);
                    i++;
                }
                transform.GetComponent<MeshFilter>().mesh = new Mesh();
                transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
                transform.gameObject.SetActive(true);
                Debug.Log("combined");
            }
            Array.Clear(array, 0, array.Length);


        }
    }
}
