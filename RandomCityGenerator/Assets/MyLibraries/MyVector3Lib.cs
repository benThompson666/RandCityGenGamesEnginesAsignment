using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyVector3Lib : MonoBehaviour
{
   
    /*
    * Polygon functions
    * */

    public static List<Vector3> MakeRandomPolygon(Vector3 center,float pdMin,float pdMax,float r,float pv)
    {

            List<Vector3> points = new List<Vector3>();

            Vector3 pointB = Random.insideUnitSphere;
            Vector3 start = pointB;
            pointB = pointB.normalized;
            Vector3 pointA = Vector3.forward;


            float dot = Vector3.Dot(pointA, pointB);
            float f = dot / (pointA.magnitude * pointB.magnitude);
            f = Mathf.Acos(f);
            float theta = Mathf.Rad2Deg * f;
            float lrDir = AngleDir(pointA, pointB, Vector3.up);

            if (lrDir == -1.0f)
            {
                f = (-1 * theta) * Mathf.Deg2Rad;
            }

            theta = Mathf.Rad2Deg * f; ;
            float i;
            for (i = theta; i < 180; i += Random.Range(pdMin, pdMax))
            {
                float angle = i * Mathf.Deg2Rad;
                Vector3 temp = Vector3.zero;
                temp.x = (float)Mathf.Sin(angle);
                temp.z = (float)Mathf.Cos(angle);
                temp *= Random.Range(r - pv, r);
                points.Add(temp + center);
            }

            i = ((i % 180) - 180);

            for (; i < theta; i += Random.Range(pdMin, pdMax))
            {
                float angle = i * Mathf.Deg2Rad;
                Vector3 temp = Vector3.zero;
                temp.x = (float)Mathf.Sin(angle);
                temp.z = (float)Mathf.Cos(angle);
                temp *= Random.Range(r - pv, r);
                points.Add(temp + center);
            }

            return points;

        }

    public static  Vector3 CalculateCentroidSimplePolygon(List<Vector3> polygon)
    {
        Vector3 centroid = Vector3.zero;
        foreach (Vector3 v in polygon)
        {
            centroid += v;
        }
        centroid = centroid / polygon.Count;
        return centroid;
    }

    #region PointInPolygon
    public static bool PointInPolygon(float z, float x, List<Vector3> polygon)
    {
        // Get the angle between the point and the
        // first and last vertices.
        int max_point = polygon.Count - 1;
        float total_angle = GetAngle(
            polygon[max_point].z, polygon[max_point].x,
            z, x,
            polygon[0].z, polygon[0].x);

        // Add the angles from the point
        // to each other pair of vertices.
        for (int i = 0; i < max_point; i++)
        {
            total_angle += GetAngle(
                polygon[i].z, polygon[i].x,
                z, x,
                polygon[i + 1].z, polygon[i + 1].x);
        }

        // The total angle should be 2 * PI or -2 * PI if
        // the point is in the polygon and close to zero
        // if the point is outside the polygon.
        return (Mathf.Abs(total_angle) > 0.000001);
    }

    public static float GetAngle(float Ax, float Ay,
        float Bx, float By, float Cx, float Cy)
    {
        // Get the dot product.

        float dot_product = DotProduct(Ax, Ay, Bx, By, Cx, Cy);
    
        float cross_product = CrossProductLength(Ax, Ay, Bx, By, Cx, Cy);

        // Calculate the angle.
        return (float)Mathf.Atan2(cross_product, dot_product);
    }

    private static float DotProduct(float Ax, float Ay,
        float Bx, float By, float Cx, float Cy)
    {
        // Get the vectors' coordinates.
        float BAx = Ax - Bx;
        float BAy = Ay - By;

        float BCx = Cx - Bx;
        float BCy = Cy - By;

        // Calculate the dot product.
        return (BAx * BCx + BAy * BCy);
    }

    public static float CrossProductLength(float Ax, float Ay,
        float Bx, float By, float Cx, float Cy)
    {
        // Get the vectors' coordinates.
        float BAx = Ax - Bx;
        float BAy = Ay - By;
        float BCx = Cx - Bx;
        float BCy = Cy - By;

        // Calculate the Z coordinate of the cross product.
        return (BAx * BCy - BAy * BCx);
    }

    #endregion

    /*
    * Vector3 functions 
    */
    public static float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
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

    public static int FindVector3InList(Vector3 v,List<Vector3> points)
    {
        int index = -1;
        for (int i = 0; i < points.Count; i++)
        {
            if (Vector3Equal(v, points[i]))
            {
                index = i;
                break;
            }
        }
        return index;
    }

    public static bool Vector3Equal(Vector3 a, Vector3 b)
    {
        return Vector3.SqrMagnitude(a - b) < 0.0001;
    }

    public static int Vector3Compare(Vector3 value1, Vector3 value2)
    {
        if (value1.x < value2.x)
        {
            return -1;
        }
        else if (value1.x == value2.x)
        {
            if (value1.y < value2.y)
            {
                return -1;
            }
            else if (value1.y == value2.y)
            {
                if (value1.z < value2.z)
                {
                    return -1;
                }
                else if (value1.z == value2.z)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 1;
            }
        }
        else
        {
            return 1;
        }
    }

    /*
    *  building Meshes with Vector3 functions
    */
    public static void CreateWall(Vector3 start, Vector3 end, GameObject plane, float height, float width, float distFromGround)
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



}
