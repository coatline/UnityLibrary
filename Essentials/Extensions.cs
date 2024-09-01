using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Extensions
{
    public static Vector2 VectorAverage(Vector2 first,  Vector2 second)
    {
        return (first + second) / 2f;
    }

    public static bool IsInViewport(Transform target, Camera camera)
    {
        // Convert the target's position from world space to viewport space
        Vector3 viewportPosition = camera.WorldToViewportPoint(target.position);

        // Check if the target is within the camera's viewport
        bool isInViewport = viewportPosition.x >= 0 && viewportPosition.x <= 1 &&
                            viewportPosition.y >= 0 && viewportPosition.y <= 1 &&
                            viewportPosition.z >= 0;

        return isInViewport;
    }

    public static int GetValueCountFromDictionaryWithList<T, G>(Dictionary<T, List<G>> dict, T key)
    {
        if (dict.TryGetValue(key, out List<G> list))
            return list.Count;
        return 0;
    }

    public static void RemoveFromDictionaryWithList<T, G>(Dictionary<T, List<G>> dict, T key, G value)
    {
        if (dict.TryGetValue(key, out List<G> list))
        {
            list.Remove(value);

            if (list.Count == 0)
                dict.Remove(key);
        }
    }

    public static void AddToDictionaryWithList<T, G>(Dictionary<T, List<G>> dict, T key, G value)
    {
        if (dict.TryGetValue(key, out List<G> list))
            list.Add(value);
        else
            dict.Add(key, new List<G> { value });
    }

    public static float AngleFromPosition(Vector3 pivotPosition, Vector3 pos)
    {
        float angleRad = Mathf.Atan2(pos.y - pivotPosition.y, pos.x - pivotPosition.x);
        float angleDeg = (180 / Mathf.PI) * angleRad;
        return angleDeg;
    }

    public static Vector2Int ToOctant(Vector2 vec)
    {
        float angle = Mathf.Atan2(vec.y, vec.x);
        int octant = Mathf.RoundToInt(8 * angle / (2 * Mathf.PI) + 8) % 8;

        CompassDir dir = (CompassDir)octant;

        switch (dir)
        {
            case CompassDir.NE: return new Vector2Int(1, 1);
            case CompassDir.N: return new Vector2Int(0, 1);
            case CompassDir.E: return new Vector2Int(1, 0);
            case CompassDir.SE: return new Vector2Int(1, -1);
            case CompassDir.S: return new Vector2Int(0, -1);
            case CompassDir.SW: return new Vector2Int(-1, -1);
            case CompassDir.W: return new Vector2Int(-1, 0);
            case CompassDir.NW: return new Vector2Int(-1, 1);
            default: return Vector2Int.zero;
        }
    }

    public static Object FindObjectOfNameFromArray(string name, Object[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].name == name)
                return array[i];
        }

        return null;
    }

    public static Vector3 MultiplyVector3s(Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    }

    enum CompassDir
    {
        E = 0, NE = 1,
        N = 2, NW = 3,
        W = 4, SW = 5,
        S = 6, SE = 7
    };
}