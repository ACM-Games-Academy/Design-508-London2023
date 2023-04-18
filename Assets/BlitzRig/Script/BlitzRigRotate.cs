using UnityEngine;

namespace BlitzRig
{
    public class BlitzRigRotate : MonoBehaviour
    {
        public float RotationSpeed = 1;
        
        void Update ()
        {
            transform.Rotate (Vector3.up * (RotationSpeed * Time.deltaTime));
        }
    }
}