using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extentions
{
    public static Vector3 SwapCoords(this Vector3 vect, float x, float y, float z)
    {
        if (x != 0) vect.x = x;
        if (y != 0) vect.y = y;
        if (z != 0) vect.z = z;

        return vect;
    }

    public static Vector3 AddCoords(this Vector3 vect, float x, float y, float z)
    {
        vect.x += x;
        vect.y += y;
        vect.z += z;

        return vect;
    }
}
