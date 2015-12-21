using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]

public class perimeter : MonoBehaviour
{

    public float skySkraperHeightMin;
    public float skySkraperHeightMax;
    public float buildingHeightMin;
    public float buildingHeightMax;

    public int skySkraperRatio;
    public int normalBuildingRatio;

    public Texture2D wall_Texture;
    public GameObject tree1;

    //Mesh Varibles
    public GameObject cube;
    public GameObject emptyPrefab;
    public List<GameObject> city_blocks;
    public List<GameObject> walls;
    public List<GameObject> paths;

    //blocks fundamentals
    public List<buildingBlocks> bbs;

    //Debug options
    public bool showEdges = true;
    public bool showGreen = true;
    public bool showTri = true;
    public bool showPerimeter = true;
    public bool showPointsInPerimeter = true;
    public bool showAttached = true;
    public int atachedTo = 0;
    public bool createMesh = false;

    //layout variables
    public List<Vector3> perimeterPolygon;
    public Vector3 v;
    public List<Vector3> pointsInPerimeter;
    public List<List<Vector3>> pointsAttachedToPointsInPerimeter;
    public List<List<float>> anglesOfPoints;
    public List<List<Vector3>> edgePoints;
    public List<Vector3> centerOfBlock;


    /*
    *   Blocks 
    */
    public List<Block> cityBlocks;
    List<GameObject> cityB;

    /*
    *  Delaunay Variables
    * */
    public int m_pointCount = 300;
    private List<Vector2> m_points;
    public float m_mapWidth = 1000;
    public float m_mapHeight = 1000;
    private List<Delaunay.Geo.LineSegment> m_edges = null;
    private List<Delaunay.Geo.LineSegment> m_spanningTree;
    private List<Delaunay.Geo.LineSegment> m_delaunayTriangulation;
    List<uint> colors;

    /*
    * City Perimeter start Point
    */
    Vector3 pointA;
    Vector3 pointB;
    Vector3 start;

    /*
    * perimeter variation variables
    * */
    public float pdmin, pdmax;
    public float time;
    public bool timeIsActive=false;
    public float r, pv;
    public bool cityCenterIsRandom = true;
    public Vector3 chosenCityCenter;


    public ReCalcCubeTexture script;

    // Use this for initialization
    void Start()
    {
        script = gameObject.GetComponent<ReCalcCubeTexture>();
      //  time = 0.0f;
        Time.timeScale = time;

        //Layout Variables 
        perimeterPolygon = new List<Vector3>();
        pointsInPerimeter = new List<Vector3>();
        pointsAttachedToPointsInPerimeter = new List<List<Vector3>>();

        //Angles for determining most Right
        anglesOfPoints = new List<List<float>>();


        cityBlocks = new List<Block>();

        //debug for holding edges of ppoints
        edgePoints = new List<List<Vector3>>();
        centerOfBlock = new List<Vector3>();
        bbs = new List<buildingBlocks>();


        //Delauney Variable
        m_points = new List<Vector2>();

        //mesh varaibles
        city_blocks = new List<GameObject>();
        walls = new List<GameObject>();
        cityB = new List<GameObject>();


    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, start); // v1 and v2 are the start and end points of the line. They are of type Vector3

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pointsInPerimeter[atachedTo], 1.0f);
        Gizmos.color = Color.yellow;

        for (int i = 0; i < pointsAttachedToPointsInPerimeter[atachedTo].Count; i++)
        {
            //Debug.Log(PIPattachedTo[atachedTo].Count);
            Gizmos.DrawWireSphere(pointsAttachedToPointsInPerimeter[atachedTo][i], 1.0f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, .1f);
        Gizmos.DrawSphere(start, .2f);

        if (showPointsInPerimeter)
        {
            if (pointsInPerimeter != null)
            {
                for (int i = 0; i < pointsInPerimeter.Count; i++)
                {
                    Gizmos.DrawWireSphere(pointsInPerimeter[i], 2.0f);
                }
            }
        }


        Vector3 y = new Vector3(0, 2, 0);

        if (cityBlocks != null)
        {
            for (int j = 0; j < cityBlocks.Count; j++)
            {
                Gizmos.DrawLine(cityBlocks[j].point + y,
                    pointsInPerimeter[cityBlocks[j].points[0]] + y);

                for (int i = 0; i < cityBlocks[j].points.Count - 1; i++)
                {
                    Gizmos.DrawLine(pointsInPerimeter[cityBlocks[j].points[i]] + y, pointsInPerimeter[cityBlocks[j].points[(i + 1)]] + y);
                }
                Gizmos.DrawLine(pointsInPerimeter[cityBlocks[j].points[0]] + y, cityBlocks[j].point + y);
            }
        }


        Gizmos.color = Color.red;
        if (pointsAttachedToPointsInPerimeter != null && showAttached)
        {
            for (int i = 0; i < pointsAttachedToPointsInPerimeter.Count; i++)
            {
                foreach (Vector3 v in pointsAttachedToPointsInPerimeter[i])
                {
                    Gizmos.DrawSphere(v, 2.0f);
                }
            }
        }

        Gizmos.color = Color.yellow;

        foreach (List<Vector3> perim in edgePoints)
        {
            for (int i = 0; i < perim.Count; i++)
            {
                Gizmos.DrawLine(perim[i], perim[(i + 1) % perim.Count]);
            }
        }




        if (showPerimeter)
        {
            for (int i = 0; i < perimeterPolygon.Count; i++)
            {
                Gizmos.DrawLine(perimeterPolygon[i] + new Vector3(0, 10, 0), new Vector3(0, 10, 0) + perimeterPolygon[(i + 1) % perimeterPolygon.Count]);
                Gizmos.DrawSphere(perimeterPolygon[i], .2f);
            }
        }

        Gizmos.color = Color.white;
        if (showEdges)
        {
            if (m_edges != null)
            {
                //m_edges.Count
               
                for (int i = 0; i < m_edges.Count; i++)
                {
                    Vector2 left = (Vector2)m_edges[i].p0;
                    Vector2 right = (Vector2)m_edges[i].p1;

                    Gizmos.DrawLine(new Vector3(left.x, 5, left.y), new Vector3(right.x, 5, right.y));
                }
            }
        }

        Gizmos.color = Color.cyan;
        if (showTri)
        {
            if (m_delaunayTriangulation != null)
            {
                for (int i = 0; i < m_delaunayTriangulation.Count; i++)
                {
                    Vector2 left = (Vector2)m_delaunayTriangulation[i].p0;
                    Vector2 right = (Vector2)m_delaunayTriangulation[i].p1;
                    Gizmos.DrawLine(new Vector3(left.x, 5, left.y), new Vector3(right.x, 5, right.y));
                }
            }
        }
        if (showGreen)
        {
            if (m_spanningTree != null)
            {
                Gizmos.color = Color.green;
                for (int i = 0; i < m_spanningTree.Count; i++)
                {
                    Delaunay.Geo.LineSegment seg = m_spanningTree[i];
                    Vector2 left = (Vector2)seg.p0;
                    Vector2 right = (Vector2)seg.p1;
                    Gizmos.DrawLine(new Vector3(left.x, 25, left.y), new Vector3(right.x, 25, right.y));
                }
            }
        }


    }
    
    void FixedUpdate() {

        if (timeIsActive)
        {
            //Make map from polygon combined with Random points
            CreateMap();
        }

    }
    

    void Update()
    {
        //preventing perimeter polygon from being null : quick fix prevents crash with null perimeter polygon
        if (Vector3.zero != v)
        {
            perimeterPolygon.Add(v);
            v = Vector3.zero;
        }
        //Make Map
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            CreateMap();
        }
        Time.timeScale = time;
    }

    private void CreateMap()
    {
        //clear the Map
        ClearMap();

        //Get a center point for city
        Vector3 cityCenter = GetCenterPointForCity();

        //Make perimeter of city polygon using center point
        perimeterPolygon = MyVector3Lib.MakeRandomPolygon(cityCenter, pdmin, pdmax, r, pv);

        //Set the Map Points includes perimeter points
        m_points = SetMapPoints();

        //setting points to geometic shapes using Voronoi Diagram
        Delaunay.Voronoi v = new Delaunay.Voronoi(m_points, colors, new Rect(0, 0, m_mapHeight, m_mapWidth));
        m_edges = v.VoronoiDiagram();
        m_spanningTree = v.SpanningTree(Delaunay.KruskalType.MINIMUM);
        m_delaunayTriangulation = v.DelaunayTriangulation();

        //Define Points in perimeter and points attached to points in the perimeter
        GetPointsInPerimeter();

        //Defining the blocks of the city
        DefineBlocks();

        //Removing duplicate blocks found in define Blocks
        RemoveDuplicateBlocks();

        //Define perimeter of build areas
        DefineEdgePoints();

        //defining paths 
        CreatePaths();

        //define each block type 
        AssignBlockTypes();
    }

    public void CreatePaths()
    {
        for (int z = 0; z < bbs.Count; z++)
        {
            for (int y = 0; y < bbs[z].perimeter.Count; y++)
            {
                GameObject t = Instantiate(cube, Vector3.zero, Quaternion.identity) as GameObject;
                createWall(bbs[z].perimeter[y],
                    bbs[z].perimeter[(y + 1) % bbs[z].perimeter.Count],
                    t, .1f, 4.0f, 0f);
                paths.Add(t);
            }
        }
    }


    public void ClearMap()
    {
        MyTerrainLib.RemoveTrees();
        perimeterPolygon.Clear();
        edgePoints.Clear();
        bbs.Clear();
        foreach (GameObject g in cityB)
        {
            GameObject.Destroy(g);
        }
        cityB.Clear();
        foreach (GameObject g in walls)
        {

            GameObject.Destroy(g);
        }
        walls.Clear();
        foreach (GameObject g in paths)
        {
            GameObject.Destroy(g);
        }
        paths.Clear();

        pointsAttachedToPointsInPerimeter.Clear();
        pointsInPerimeter.Clear();
    }

    public Vector3 GetCenterPointForCity()
    {
        Vector3 v;
        if (cityCenterIsRandom)
        {
            v = new Vector3(Random.Range(r, m_mapWidth - r), 0, Random.Range(r, m_mapHeight - r));
        }
        else
        {
            v = chosenCityCenter; //use chosen city point
        }
        return v;
    }

    void MakeShopingCenter(Vector3 center, Block a)
    {
        float pdmin = 30;
        float pdmax = 30;
        float r = 10;
        float pv = 0;
        //getting random start point for perimter
        pointB = Random.insideUnitSphere;
        start = pointB;

        pointB = pointB.normalized;
        pointA = transform.forward;

        float dot = Vector3.Dot(pointA, pointB);
        float f = dot / (pointA.magnitude * pointB.magnitude);
        f = Mathf.Acos(f);
        float theta = Mathf.Rad2Deg * f;
        float lrDir = AngleDir(pointA, pointB, transform.up);

        if (lrDir == -1.0f)
        {
            f = (-1 * theta) * Mathf.Deg2Rad;
        }

        theta = Mathf.Rad2Deg * f; ;
        float i;
        for (i = theta; i < 180; i += Random.Range(pdmin, pdmax))
        {
            float angle = i * Mathf.Deg2Rad;
            Vector3 temp = Vector3.zero;
            temp.x = (float)Mathf.Sin(angle);
            temp.z = (float)Mathf.Cos(angle);
            temp *= Random.Range(r - pv, r);
            //if (PointInBlock(temp,a))
            //{  
            // Debug.Log("var:"+ temp);
            GameObject t = Instantiate(cube, Vector3.zero, Quaternion.identity) as GameObject;
            Vector3 close = temp.normalized;
            close = temp - (temp.normalized * 6);

            //createWall(center + close, temp + center, t, 10.0f, 0);
            //}
            // points.Add(temp + center);
        }

        i = ((i % 180) - 180);

        for (; i < theta; i += Random.Range(pdmin, pdmax))
        {
            float angle = i * Mathf.Deg2Rad;
            Vector3 temp = Vector3.zero;
            temp.x = (float)Mathf.Sin(angle);
            temp.z = (float)Mathf.Cos(angle);
            temp *= Random.Range(r - pv, r);
            //if (PointInBlock(temp, a))
            //{ 
            GameObject t = Instantiate(cube, Vector3.zero, Quaternion.identity) as GameObject;
            Vector3 close = temp.normalized;
            close = temp + (temp.normalized * 6);
            // createWall((center + close), temp + center, t, 10.0f, 0);


            // points.Add(temp + center);
        }
    }


    private List<Vector2> SetMapPoints()
    {
        colors = new List<uint>();
        List<Vector2> map_points = new List<Vector2>();

        for (int z = 0; z < (m_pointCount - perimeterPolygon.Count); z++)
        {
            colors.Add(0);
            map_points.Add(new Vector2(
                    UnityEngine.Random.Range(0, m_mapWidth),
                    UnityEngine.Random.Range(0, m_mapHeight))
            );
        }

        for (int j = 0; j < perimeterPolygon.Count; j++)
        {
            colors.Add(0);
            map_points.Add(new Vector2(perimeterPolygon[j].x, perimeterPolygon[j].z));
        }

        return map_points;
    }


    int FindMostRight(int last, int current)
    {
        Vector3 toCurrent;

        if (last < 0)//if no last i.e. start use transform forward
        {
            toCurrent = pointsInPerimeter[current] - transform.forward;
            toCurrent.Normalize();
        }
        else
        {
            toCurrent = pointsInPerimeter[current] - pointsInPerimeter[last];
            toCurrent.Normalize();
        }

        for (int j = 0; j < pointsAttachedToPointsInPerimeter[current].Count; j++)
        {
            if (last < 0)
            {
                Vector3 toOther = pointsAttachedToPointsInPerimeter[current][j] - pointsInPerimeter[current];
                toOther.Normalize();
                //Vector3 axis = Vector3.Cross(toLast, toOther);
                float angle = Mathf.Acos(Vector3.Dot(toCurrent, toOther));

                float lrDir = AngleDir(toCurrent, toOther, transform.up);

                if (lrDir == -1.0f)
                {
                    angle = (-1 * angle);
                }

                angle = angle * Mathf.Rad2Deg;

                if (angle < 0)
                {
                    angle += 360;
                }

                anglesOfPoints[current][j] = angle;

                //Debug.Log(j + ":" + anglesOfPoints[current][j]);


            }

            else
            {
                if (!MyVector3Lib.Vector3Equal(pointsInPerimeter[last], pointsAttachedToPointsInPerimeter[current][j]))
                {
                    Vector3 toOther = pointsAttachedToPointsInPerimeter[current][j] - pointsInPerimeter[current];
                    toOther.Normalize();
                    //Vector3 axis = Vector3.Cross(toLast, toOther);
                    float angle = Mathf.Acos(Vector3.Dot(toCurrent, toOther));

                    float lrDir = AngleDir(toCurrent, toOther, transform.up);

                    if (lrDir == -1.0f)
                    {
                        angle = (-1 * angle);
                    }

                    angle = angle * Mathf.Rad2Deg;

                    if (angle < 0)
                    {
                        angle += 360;
                    }

                    anglesOfPoints[current][j] = angle;

                    //Debug.Log(j + ":" + anglesOfPoints[current][j]);

                }
                else
                {
                    anglesOfPoints[current][j] = 0;

                }
            }
        }
        float max = -1.0f;//right
        //int leftIndex = -1;
        int rightIndex = -1;
        //float min = 361.0f;//left
        if (last != -1)
        {
            for (int j = 0; j < anglesOfPoints[current].Count; j++)
            {
                if (anglesOfPoints[current][j] > max && !MyVector3Lib.Vector3Equal(pointsInPerimeter[last], pointsAttachedToPointsInPerimeter[current][j]))
                {
                    max = anglesOfPoints[current][j];
                    rightIndex = j;
                }
                // Debug.Log("angles Of points " + j + " :" + anglesOfPoints[current][j]);
                /*if (anglesOfPoints[current][j] < min && !V3Equal(pointsInPerimeter[last], PIPattachedTo[current][j]))
                {
                    max = anglesOfPoints[current][j];
                    leftIndex = j;
                }*/
            }
        }
        else
        {
            for (int j = 0; j < anglesOfPoints[current].Count; j++)
            {
                if (anglesOfPoints[current][j] > max)
                {
                    max = anglesOfPoints[current][j];
                    rightIndex = j;
                }
                /*if (anglesOfPoints[current][j] < min)
                {
                    max = anglesOfPoints[current][j];
                    leftIndex = j;
                }*/
            }
        }
        // Debug.Log("Left: " + anglesOfPoints[current][rightIndex]);
        // Debug.Log(rightIndex);
        if (rightIndex == -1)
        {
            return -1;
        }
        else
        {
            return MyVector3Lib.FindVector3InList(pointsAttachedToPointsInPerimeter[current][rightIndex], pointsInPerimeter);

        }
    }

    bool PointInBlock(Vector3 point, Block b)
    {

        // Get the angle between the point and the
        // first and last vertices.
        int max_point = b.points.Count - 1;

        float total_angle = MyVector3Lib.GetAngle(
            pointsInPerimeter[b.points[max_point]].z,
            pointsInPerimeter[b.points[max_point]].x,
            point.z, point.x,
            pointsInPerimeter[b.points[0]].z,
            pointsInPerimeter[b.points[0]].x);

        // Add the angles from the point
        // to each other pair of vertices.
        for (int i = 0; i < max_point; i++)
        {
            total_angle += MyVector3Lib.GetAngle(
                pointsInPerimeter[b.points[i]].z,
                pointsInPerimeter[b.points[i]].x,


                point.z, point.x,
                pointsInPerimeter[b.points[i + 1]].z,
                pointsInPerimeter[b.points[i + 1]].x);
        }

        // The total angle should be 2 * PI or -2 * PI if
        // the point is in the polygon and close to zero
        // if the point is outside the polygon.
        return (Mathf.Abs(total_angle) > 0.000001);

    }

    void GetPointsInPerimeter()
    {

        foreach (Delaunay.Geo.LineSegment line in m_edges)
        {
            Vector2 left = (Vector2)line.p0;
            Vector2 right = (Vector2)line.p1;
            Vector3 p1 = new Vector3(left.x, 5, left.y);
            Vector3 p2 = new Vector3(right.x, 5, right.y);
            PointInPerimeter_Or_PointAttachedToPointsInPerimeter(p1, p2);
            PointInPerimeter_Or_PointAttachedToPointsInPerimeter(p2, p1);
        }

    }

    void PointInPerimeter_Or_PointAttachedToPointsInPerimeter(Vector3 p1, Vector3 p2)
    {
        bool existsAlready = false;
        int index = -1;

        //check if point p1 already in points In perimeter
        for (int i = 0; i < pointsInPerimeter.Count; i++)
        {
            if (MyVector3Lib.Vector3Equal(pointsInPerimeter[i], p1))
            {
                existsAlready = true;
                index = i;
                break;
            }
        }


        if (existsAlready)//if P1 alreading in perimeter 
        {
            if (MyVector3Lib.PointInPolygon(p2.z, p2.x, perimeterPolygon)) //if p2 is in the perimeter then add to points attached to p1
            {
                pointsAttachedToPointsInPerimeter[index].Add(p2);
            }
        }
        else
        {
            if (MyVector3Lib.PointInPolygon(p1.z, p1.x, perimeterPolygon))//if p1 is in polgon
            {
                pointsInPerimeter.Add(p1); //add to points in polygon

                if (MyVector3Lib.PointInPolygon(p2.z, p2.x, perimeterPolygon)) //if the point p2 is also in polygon attach the to points attached
                {
                    pointsAttachedToPointsInPerimeter.Add(new List<Vector3> { p2 });
                }
                else
                {
                    pointsAttachedToPointsInPerimeter.Add(new List<Vector3> { });
                }
            }
        }

    }

    public class Block
    {
        int block_no;
        public Vector3 point;
        public List<int> points;

        public Block(int _block_no, Vector3 p)
        {
            points = new List<int>();
            block_no = _block_no;
            point = p;
        }
    }


    public float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0.0f)
        {
            return 1.0f;
        }
        else if (dir < 0.0f)
        {
            return -1.0f;
        }
        else
        {
            return 0.0f;
        }
    }




    void createWall(Vector3 start, Vector3 end, GameObject plane, float height, float width, float distFromGround)
    {

        float factor = 1.0f;
        start.y = 0;
        end.y = 0;
        var dir = end - start;
        var mid = (dir) / 2.0f + start;
        mid.y = height / 2;
        mid.y += distFromGround;
        plane.transform.position = mid;
        plane.transform.LookAt(end);

        // plane.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);

        plane.transform.eulerAngles = new Vector3(90, plane.transform.eulerAngles.y, plane.transform.eulerAngles.z);
        Vector3 scale = plane.transform.localScale;
        scale.y = dir.magnitude * factor;
        scale.x = width;
        scale.z = height - (distFromGround * 2);
        plane.transform.localScale = scale;
    }

    void GenerateTop(int i, float height)
    {

        List<Vector2> temp = new List<Vector2>();

        for (int j = 0; j < edgePoints[i].Count; j++)
        {
            temp.Add(
                    new Vector2(edgePoints[i][j].x,
                   edgePoints[i][j].z));

        }

        GameObject a = Instantiate(emptyPrefab) as GameObject;
        cityB.Add(a);
        float y = height;
        Vector2[] t = new Vector2[temp.Count];

        //   Debug.Log(l.Count + ":number");
        for (int z = 0; z < temp.Count; z++)
        {
            t[z] = temp[z];
            // Debug.Log(l[i].y);
        }

        triangulator1 tr = new triangulator1(t);
        int[] indices = tr.Triangulate_();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[t.Length];

        for (int x = 0; x < vertices.Length; x++)
        {

            vertices[x] = new Vector3(t[x].x, y, t[x].y);
        }

        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        // Set up game object with mesh;
        // a.AddComponent(typeof(MeshRenderer));
        MeshFilter filter = a.AddComponent(typeof(MeshFilter)) as MeshFilter;

        filter.mesh = msh;
        MeshRenderer meshRenderer = a.AddComponent<MeshRenderer>();
        Texture2D texture;
        texture = new Texture2D((int)t.Length, (int)t.Length);
        texture.filterMode = FilterMode.Point;

        /*
        for (int z = 0; z < temp[temp.Length-1].y; z++)
        {
            for (int x = 0; x < temp[temp.Length-1].x; x++)
            {
                texture.SetPixel(x, z, Color.blue);
            }
        }
        texture.Apply();
        */
        meshRenderer.material.SetTexture(0, texture);
       // a.AddComponent<doubleSidedMesh>();
        city_blocks.Add(a);
        
    }

    public class buildingBlocks
    {
        public Vector3 centroid;
        public List<Vector3> perimeter;
        public List<int> per;

        public buildingBlocks(Vector3 _centroid, List<Vector3> _perim, List<int> _per)
        {
            centroid = _centroid;
            perimeter = _perim;
            per = _per;
        }
    }

    public void DefineBlocks()
    {
        #region Variables initialized for finding blocks

        List<bool> perimeterPointsTraveled = new List<bool>();
        for (int x = 0; x < pointsInPerimeter.Count; x++)
        {
            perimeterPointsTraveled.Add(false);
        }

        anglesOfPoints.Clear();
        cityBlocks.Clear();
        for (int i = 0; i < pointsInPerimeter.Count; i++)
        {

            anglesOfPoints.Add(new List<float> { });
            for (int j = 0; j < pointsAttachedToPointsInPerimeter[i].Count; j++)
            {
                anglesOfPoints[i].Add(0.0f);
            }
        }
        bool untraveledPoints = true;

        #endregion

        #region Finding blocks by traveling right

        while (untraveledPoints)
        {
            #region Get Start Points
            /*
            * Get start points
            */
            int start = -1;
            for (int i = 0; i < pointsInPerimeter.Count; i++)
            {
                if (!perimeterPointsTraveled[i])
                {
                    start = i;
                    perimeterPointsTraveled[i] = true;
                    if (i == pointsInPerimeter.Count - 1)
                    {
                        untraveledPoints = false;
                        break;
                    }
                    break;
                }
            }
            #endregion

            #region Going Right starting from the worlds transform forward
            Block temp = new Block(0, pointsInPerimeter[start]);
            //Find next point
            int next = -1;
            next = FindMostRight(-1, start);
            if (next != -1)
            {
                temp.points.Add(next);

                //define next point
                int mostRight = -1;
                mostRight = FindMostRight(start, next);

                if (mostRight != -1)
                {
                    temp.points.Add(mostRight);
                    int last = next;
                    int current = mostRight;
                    int counter = -1;

                    while (mostRight != start)
                    {
                        bool PointIsTraveled = false;
                        counter++;
                        mostRight = FindMostRight(last, current);

                        if (mostRight == -1)
                        {
                            break;
                        }

                        temp.points.Add(mostRight);

                        last = current;
                        current = mostRight;


                        if (counter > 10 || PointIsTraveled)
                        {
                            // Debug.Log("No match");
                            break;
                        }

                        if (mostRight == start)
                        {
                            // Debug.Log("Match");
                            cityBlocks.Add(temp);
                            break;
                        }
                    }
                }
            }
            #endregion
        }
        #endregion
    }


    Vector2 LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2,
   Vector2 pe2)
    {
        // Get A,B,C of first line - points : ps1 to pe1
        float A1 = pe1.y - ps1.y;
        float B1 = ps1.x - pe1.x;
        float C1 = A1 * ps1.x + B1 * ps1.y;

        // Get A,B,C of second line - points : ps2 to pe2
        float A2 = pe2.y - ps2.y;
        float B2 = ps2.x - pe2.x;
        float C2 = A2 * ps2.x + B2 * ps2.y;

        // Get delta and check if the lines are parallel
        float delta = A1 * B2 - A2 * B1;
        if (delta == 0)
            throw new System.Exception("Lines are parallel");

        // now return the Vector2 intersection point
        return new Vector2(
            (B2 * C1 - B1 * C2) / delta,
            (A1 * C2 - A2 * C1) / delta
        );
    }

    public void RemoveDuplicateBlocks()
    {
        List<Vector3> perim_ = new List<Vector3>();
        for (int u = 0; u < cityBlocks[0].points.Count; u++)
        {
            perim_.Add(pointsInPerimeter[cityBlocks[0].points[u]]);
        }

        Vector3 centroid_ = MyVector3Lib.CalculateCentroidSimplePolygon(perim_);

        bbs.Add(new buildingBlocks(centroid_, perim_, cityBlocks[0].points));


        //getting rid of duplicates and defining in proper class
        foreach (Block b in cityBlocks)
        {
            bool duplicate = false;
            foreach (buildingBlocks c in bbs)
            {
                List<int> dif = b.points.Except(c.per).ToList();
                if (dif.Count == 0)
                {
                    duplicate = true;
                    break;
                }
            }
            if (!duplicate)
            {
                //defining perimeter of block
                List<Vector3> perim = new List<Vector3>();
                for (int u = 0; u < b.points.Count; u++)
                {
                    perim.Add(pointsInPerimeter[b.points[u]]);
                }

                Vector3 centroid = Vector3.zero;
                foreach (Vector3 _v in perim)
                {
                    centroid += _v;
                }
                centroid = centroid / perim.Count;

                bbs.Add(new buildingBlocks(centroid, perim, b.points));
            }
        }

    }

    void DefineEdgePoints()
    {
        //defining edgePoints for blocks
        foreach (buildingBlocks b in bbs)
        {
            edgePoints.Add(new List<Vector3>());
        }
        //defining edgepoints
        int m = 0;
        foreach (buildingBlocks b in bbs)
        {
            foreach (Vector3 _v in b.perimeter)
            {
                Vector3 toCenter = b.centroid - _v;
                toCenter.Normalize();
                toCenter *= 7;
                Vector3 edge = _v + toCenter;
                edgePoints[m].Add(edge);

            }
            m++;
        }
    }

    void AssignBlockTypes()
    {
        int count = 0;
        foreach (List<Vector3> perim in edgePoints)
        { 
            int block_ratio = Random.Range(0, 100);
            if (block_ratio < skySkraperRatio)
            {
                CreateSkySkraper(perim, count,skySkraperHeightMin, skySkraperHeightMax);
            }
            else if (block_ratio < (skySkraperRatio + normalBuildingRatio))
            {
                CreateSkySkraper(perim, count, buildingHeightMin, buildingHeightMax);
            }
            else
            {
                Vector3 v=MyVector3Lib.CalculateCentroidSimplePolygon(perim);
                CreatePark(v,count);
            }

            count++;
        }
    }

    void CreatePark(Vector3 v,int count)
    {
        MyTerrainLib.PlaceTree(v,tree1);
        List<Vector3> points=new List<Vector3>();
        //List<Vector3> points=MyVector3Lib.MakeRandomPolygon(bbs[count].centroid, 1, 2,5, 4);
        for (int i = 0; i < 10; i++)
        {
            points.Add(new Vector3(Random.Range(5.0f, 20f), 0, Random.Range(5.0f, 20f)) + v);
        }
        foreach (Vector3 p in points)
        {
            if(MyVector3Lib.PointInPolygon(p.z,p.x,bbs[count].perimeter))
            {
                MyTerrainLib.PlaceTree(p, tree1);
           }
        }
    }


    void CreateSkySkraper(List<Vector3> perim,int count,float h_min,float h_max)
    {
            float height = Random.Range(h_min, h_max);
            GenerateTop(count, height);//create roof
            GenerateTop(count, 0.01f);
            float rand = Random.Range(0, 100);
            float rDoor = Random.Range(20, 80);
            rDoor /= 100;
            rand /= 100;
            Color color = Color.Lerp(Color.red, Color.yellow, Random.Range(0.5f, .8f));
            // Color color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255));
            for (int i = 0; i < perim.Count; i++)
            {
                GameObject t1 = Instantiate(cube, Vector3.zero, Quaternion.identity) as GameObject;
                GameObject d = Instantiate(cube, Vector3.zero, Quaternion.identity) as GameObject;
                GameObject t2 = Instantiate(cube, Vector3.zero, Quaternion.identity) as GameObject;

                Renderer g1 = t1.GetComponent<Renderer>();
                Renderer g2 = d.GetComponent<Renderer>();
                Renderer g3 = t2.GetComponent<Renderer>();

                g1.material.mainTexture = wall_Texture;
                g2.material.mainTexture = wall_Texture;
                g3.material.mainTexture = wall_Texture;

                script.reCalcCubeTexture(t1);
                script.reCalcCubeTexture(d);
                script.reCalcCubeTexture(t2);

                //g1.material.color = color;
                //g2.material.color = color;
                //g3.material.color = color;
                /*
                g1.material.color = Color.Lerp(Color.blue,Color.white, rand);
                g2.material.color = Color.Lerp(Color.blue, Color.white, rand);
                g3.material.color = Color.Lerp(Color.blue, Color.white, rand);*/

                Vector3 doorStart = Vector3.Lerp(perim[i], perim[(i + 1) % perim.Count], rDoor);
                Vector3 doorEnd = perim[(i + 1) % perim.Count] - perim[i];
                doorEnd.Normalize();

                createWall(perim[i], doorStart, t1, height, .3f, 0);


                createWall(doorStart, doorStart + (doorEnd * 2), d, height, 0.3f, 2);
                createWall((doorEnd * 2) + doorStart, perim[(i + 1) % perim.Count], t2, height, 0.3f, 0);

                //createWall(perim[i], perim[(i + 1) % perim.Count], t, height);
                walls.Add(t1);
                walls.Add(d);
                walls.Add(t2);

            }

        }
    }


