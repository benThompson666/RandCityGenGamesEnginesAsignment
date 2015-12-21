using UnityEngine;
using System.Collections;

public class BuildMode : MonoBehaviour {


    //current shape for instatiation
    public Transform currentShape;
    GameObject buildObject;
    //Variables for point of builder GameObject
    GameObject go;
    Vector3 buildPosition;

    bool shapeChange=false;

    public float speed = 2.0f;
    bool grid = true;

    // Use this for initialization
    void Start () {

        Inventory inventory = GetComponent<Inventory>();
        currentShape = inventory.cubePrefab;//setting current shape

        //getting Build object and setting its position
        go = GameObject.Find("Builder");
        GameObject fpc = GameObject.Find("FPSController");

        //setting new position
        go.transform.position = fpc.transform.position+ new Vector3(0,0,3);
        buildPosition = go.transform.position;
        

        //instantiating the build object in the Builder gameObject

        Transform t = Instantiate(currentShape, go.transform.position, Quaternion.identity) as Transform;
        buildObject = t.gameObject;
        buildObject.transform.parent = go.transform;

        //GameObject childObject = Instantiate(currentShape) as GameObject;

    }
	
	// Update is called once per frame
	void Update () {

  
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
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
            Instantiate(currentShape, go.transform.position, Quaternion.identity);
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
