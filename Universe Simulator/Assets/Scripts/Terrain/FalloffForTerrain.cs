using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalloffForTerrain : MonoBehaviour
{
    public static float[,] GenerateFalloffMap(int size)
    {
        float[,] map = new float[size, size];

        float maxDistance = Mathf.Sqrt(2 * Mathf.Pow(size / 2f, 2));

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float normalizedX = x / (float)size * 2 - 1;
                float normalizedY = y / (float)size * 2 - 1;

                float distance = Mathf.Sqrt(normalizedX * normalizedX + normalizedY * normalizedY);
                map[x, y] = Evaluate(distance, maxDistance);
            }
        }

        return map;
    }

    private static float Evaluate(float value, float maxDistance)
{
    float steepness = 4f; 
    float falloffStart = 4f; 

    return Mathf.Pow(value, steepness) / (Mathf.Pow(value, steepness) + Mathf.Pow(falloffStart - falloffStart * value, steepness));
}
}