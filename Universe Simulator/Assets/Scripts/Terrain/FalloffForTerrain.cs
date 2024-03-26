using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalloffForTerrain : MonoBehaviour
{
    public static float[,] GenerateFalloffMap(int size)
    {
        // Create a 2D array to store the falloff map
        float[,] map = new float[size, size];

        // Calculate the maximum distance from the center to the corners of the map 
        float maxDistance = Mathf.Sqrt(2 * Mathf.Pow(size / 2f, 2));

        // Loop through each pixel of the map
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                // Normalize the x and y coordinates to range from -1 to 1
                float normalizedX = x / (float)size * 2 - 1;
                float normalizedY = y / (float)size * 2 - 1;

                // Calculate the distance from the center of the map to the current pixel (Euclidean distance formula)
                float distance = Mathf.Sqrt(normalizedX * normalizedX + normalizedY * normalizedY);

                // Evaluate the falloff value for this pixel
                map[x, y] = Evaluate(distance, maxDistance);
            }
        }

        return map;
    }

    // Method to calculate the falloff value based on distance
    private static float Evaluate(float value, float maxDistance)
    {
        float steepness = 4f; // Steepness of the falloff curve, controls how quickly the falloff occurs
        float falloffStart = 4f; // Distance at which falloff starts, affects the start of the falloff curve

        // Calculate the falloff value using a falloff curve equation
        return Mathf.Pow(value, steepness) / (Mathf.Pow(value, steepness) + Mathf.Pow(falloffStart - falloffStart * value, steepness));
    }
}