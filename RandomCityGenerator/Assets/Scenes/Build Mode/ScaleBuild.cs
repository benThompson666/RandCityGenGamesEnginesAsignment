using UnityEngine;
using System.Collections;

public class ScaleBuild : MonoBehaviour
{

    GameObject go;
    Vector3 buildScale;
    public float speed = 1.0f;


    //Shape prefabs
    public Transform cubePrefab;
    public Transform spherePrefab;
    public Transform cylinderPrefab;
    public Transform conePrefab;

    //materials


    //current shape for instatiation
    Transform currentShape;

    // Use this for initialization
    void Start()
    {
        currentShape = cubePrefab;

        go = GameObject.Find("Builder");
        buildScale = go.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {

        transform.LookAt(go.transform.position);//looking at building object


        transform.LookAt(go.transform.position);//looking at building object

        //build shape if press enter
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Instantiate(currentShape, go.transform.position, Quaternion.identity);
        }

        EditScale();
    }




    void EditScale()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float y = 0;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            y = -1;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            y = 1;
        }

        if (x != 0 || y != 0 || z != 0)
        {
            buildScale = buildScale + new Vector3(x * speed, y * speed, z * speed);
        }

        if (Input.GetKey(KeyCode.Q))
            buildScale = buildScale + new Vector3(1 * speed, 1 * speed, 1 * speed);

        if (Input.GetKey(KeyCode.E))
            buildScale = buildScale + new Vector3(-1 * speed, -1 * speed, -1 * speed);

        go.transform.localScale = buildScale;
    }
}