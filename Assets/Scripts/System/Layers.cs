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
        public static int InteractAble = 12;

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

        public static bool CheckLayer(LayerMask objLayer, LayerMask layerToCheck)
        {
            if (((1 << objLayer) & layerToCheck) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}