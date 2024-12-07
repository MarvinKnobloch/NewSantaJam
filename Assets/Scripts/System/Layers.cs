using UnityEngine;

namespace Santa
{
    public static class Layers
    {
        public static int EFFECT = 1;

        public static int Bubble = 6;
        public static int Dimension_1 = 7;
        public static int Dimension_2 = 8;
        public static int Trigger = 9;
        public static int Spieler = 10;

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
}