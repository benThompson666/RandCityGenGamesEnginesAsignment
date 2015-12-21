using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Test : MonoBehaviour {

    //Random Polygon Points
    public List<Vector3> points;
    public List<Vector3> sortedV3;

    //Make Random Polygon Varaibales
    public Vector3 centroid;
    public float pointDistanceMin;
    public float pointDistanceMax;
    public float radius;
    public float perimeterVariation;

    public bool timeIsActive;
    public float time; 

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.DrawLine(points[i] + new Vector3(0, 10, 0), new Vector3(0, 10, 0) + points[(i + 1) % points.Count]);
            Gizmos.DrawSphere(points[i], .2f);

        }
    }
        // Use this for initialization
    void Start () {

        //polygon functions

        points    = MyVector3Lib.MakeRandomPolygon(centroid,pointDistanceMin,pointDistanceMax, radius, perimeterVariation);

        Debug.Log("Center in polygon = " + MyVector3Lib.PointInPolygon(centroid.x, centroid.z, points));
        
        //Overcome unity expectations for accuracy between v3 points    
        int index = MyVector3Lib.FindVector3InList(points[4],points);
        Debug.Log("Index:" + index + " Vector : "+ points[4]);

        //sorting Vector3
        sortedV3 = new List<Vector3>(points);
        sortedV3.Sort(MyVector3Lib.Vector3Compare);

        Vector3 centroid2 =MyVector3Lib.CalculateCentroidSimplePolygon(points);
        Debug.Log("centroid = " + centroid2);
                 
        
	}
	
	// Update is called once per frame
	void Update () {
	
        if(timeIsActive)
        {
            Time.timeScale = time;
        }
	}

    void FixedUpdate()
    {
        points = MyVector3Lib.MakeRandomPolygon(centroid, pointDistanceMin, pointDistanceMax, radius, perimeterVariation);
    }
}
