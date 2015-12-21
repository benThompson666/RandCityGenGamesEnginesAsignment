using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineRender : MonoBehaviour {

    public List<Vector3> points;
    // Use this for initialization
    //current shape for instatiation
    Transform currentShape;


    LineRenderer l;

    GameObject buildObject;
    //Variables for point of builder GameObject
    GameObject go;
    Vector3 buildPosition;

    int count;
    bool shapeChange = false;

    public float speed = 2.0f;
    bool grid = true;

    // Use this for initialization
    void Start()
    {
        l = gameObject.GetComponent<LineRenderer>();

        count = 0;
        Inventory inventory = GetComponent<Inventory>();
        currentShape = inventory.spherePrefab;//setting current shape

        //getting Build object and setting its position
        go = GameObject.Find("Builder");
        GameObject fpc = GameObject.Find("FPSController");

        //setting new position
        go.transform.position = fpc.transform.position + new Vector3(0, 0, 3);
        buildPosition = go.transform.position;


        //instantiating the build object in the Builder gameObject

        Transform t = Instantiate(currentShape, go.transform.position, Quaternion.identity) as Transform;
        buildObject = t.gameObject;
        buildObject.transform.parent = go.transform;

        
        //GameObject childObject = Instantiate(currentShape(GameObject)) as GameObject;

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Transform t = Instantiate(currentShape, go.transform.position, Quaternion.identity) as Transform;
            buildObject = t.gameObject;

            go = GameObject.Find("Builder");
            GameObject fpc = GameObject.Find("FPSController");

            //setting new position

            go.transform.position = fpc.transform.position + new Vector3(0, 0, 3);
            buildPosition = go.transform.position;
        }

        transform.LookAt(go.transform.position);//looking at building object

        //build shape if press enter
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            
            points.Add(go.transform.position);
            l.SetVertexCount(points.Count);
            Debug.Log(points.Count);
            //for (int i = 0; i < points.Count-1; i++)
            
           l.SetPosition(count, go.transform.position);
            //}
            Debug.Log(go.transform.position);
            //Instantiate(currentShape, go.transform.position, Quaternion.identity);
            count++;
           
        }

        SelectShape();
        MoveBuilder();

    }




    void SelectShape()
    {
        if (shapeChange)
        {

        }
    }
    void MoveBuilder()
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

        //if player in front of builder flip left and right 
        if (transform.position.z > buildPosition.z)
            x *= -1;

        if (x != 0 || y != 0 || z != 0)
        {
            buildPosition = buildPosition + new Vector3(x * speed, y * speed, z * speed);
        }

        go.transform.position = buildPosition;

    }

}

