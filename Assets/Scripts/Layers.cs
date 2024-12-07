using UnityEngine;

public class Layers
{
    public static int Bubble = 6;
    public static int Dimension_1 = 7;
    public static int Dimension_2 = 8;

    // Gibt eine Maske zurück, die alle übergebenen Layer enthält
    public static int Mask(params int[] layers)
    {
        int mask = 0;
        foreach (int layer in layers)
        {
            mask |= 1 << layer;
        }
        return mask;
    }
}
