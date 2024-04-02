using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CalculosMatematicos : MonoBehaviour
{
    public static float ObtenerDireccion(float3 objectPosition, float3 targetPosition)
    {
        var x = objectPosition.x - targetPosition.x;
        var y = objectPosition.z - targetPosition.z;
        return math.atan2(x, y) + math.PI;
    }
}
