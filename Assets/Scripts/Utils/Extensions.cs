using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class LayerMaskExtensions
{
    public static bool Include(this LayerMask layerMask, int layer)
    {
        return (layerMask == (layerMask | (1 << layer)));
    }
}

public static class TransformExtensions
{
    public static void Lerp(this Transform transfom, Transform t1, Transform t2, float t)
    {
        Vector3 p1;
        Vector3 p2;
        Quaternion r1;
        Quaternion r2;
        t1.GetPositionAndRotation(out p1, out r1);
        t2.GetPositionAndRotation(out p2, out r2);
        transfom.Lerp(p1, r1, p2, r2, t);
    }

    public static void Lerp(this Transform transfom, Vector3 pos1, Quaternion rot1, Vector3 pos2, Quaternion rot2, float t)
    {
        Quaternion rot = Quaternion.Slerp(rot1, rot2, t);
        transfom.SetPositionAndRotation(Vector3.Lerp(pos1, pos2, t), rot);
    }
}

public static class Vector3Extensions
{
    public static Vector3 Round(this Vector3 vector)
    {
        vector.x = Mathf.Round(vector.x);
        vector.y = Mathf.Round(vector.y);
        vector.z = Mathf.Round(vector.z);
        return vector;
    }

    public static void ToFloatVec(this Vector3 vector, ref float[] floats)
    {
        floats[0] = vector.x;
        floats[1] = vector.y;
        floats[2] = vector.z;
    }

    public static float Angle180(Vector3 from, Vector3 to, Vector3 axis)
    {
        float angle = Vector3.SignedAngle(from, to, axis);
        while (angle > 180) angle -= 360;
        while (angle < -180) angle += 360;
        return angle;
    }
}

public static class MethodInfoExtensions
{
    private static object[] oneArgArray = new object[1];

    public static object InvokeOptimized(this MethodInfo method, object obj, object arg0)
    {
        oneArgArray[0] = arg0;
        return method.Invoke(obj, oneArgArray);
    }
}

public static class ListExtensions
{
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
