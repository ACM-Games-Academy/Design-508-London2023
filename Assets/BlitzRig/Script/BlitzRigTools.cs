using UnityEngine;

namespace BlitzRig
{
    public class BlitzRigTools
    {
        public static bool RandomBool()
        {
            if (Random.Range(0, 2) == 1)
                return true;

            return false;
        }

        public static int RandomNegator()
        {
            if (Random.Range(0, 2) == 1)
                return 1;

            return -1;
        }
    }
}