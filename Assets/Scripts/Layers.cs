using UnityEngine;

public class Layers
{
    public static int Bubble = 6;
    public static int Dimension_1 = 7;
    public static int Dimension_2 = 8;

    // Gibt eine Maske zur�ck, die alle �bergebenen Layer enth�lt
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
