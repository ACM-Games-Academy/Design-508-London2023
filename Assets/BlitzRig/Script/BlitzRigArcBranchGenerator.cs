using UnityEngine;

namespace BlitzRig
{
    public class BlitzRigArcBranchGenerator
    {
        const int MAX_SEGMENT_COUNT = 65536;
        // const float X_INTERPOLATION_FACTOR = 0.9f;
        
        public readonly Vector3 start;
        public readonly Vector3 end;
        public readonly uint segmentCount;
        public readonly float widthVariation;
        public readonly Vector3 center;
        public readonly Vector3 tangent;
        public readonly Vector3 normal;
        public readonly Vector3 widthVariationVector;
        public readonly float segmentSizeVariationFactor = 0.5f;
        
        public BlitzRigArcBranchGenerator(Vector3 ParamStart, Vector3 ParamEnd, uint ParamSegmentCount, float ParamSegSize, float ParamWidthVariation = 2f)
        {
            start = ParamStart;
            end = ParamEnd;

            // -------------------------------------------
            if (ParamSegmentCount < 1)
                ParamSegmentCount = 1;

            if (ParamSegmentCount > MAX_SEGMENT_COUNT)
                ParamSegmentCount = MAX_SEGMENT_COUNT;

            segmentCount = ParamSegmentCount;
            // -------------------------------------------

            widthVariation = ParamWidthVariation;
            segmentSizeVariationFactor = ParamSegSize;

            // --------------------------------------------
            center = (start + end)/2;
            tangent = end - start;
            normal = (new Vector3(tangent.y, tangent.x * -1, 0)).normalized;
            widthVariationVector = normal * widthVariation;
        }

        public Vector3[] GenerateArcBranchPoints()
        {
            float distance = Vector3.Distance(start, end);

            Vector3[] result = new Vector3[segmentCount + 1];
            float segmentLength = distance / (segmentCount);
            
            result[0] = start;

            float maxInterp = 1f / (segmentCount);
            maxInterp *= segmentSizeVariationFactor;

            for (int i = 1; i < segmentCount; i++)
            {
                // X
                float t = (i * segmentLength) / distance; // Lerp ratio
                t = t + Random.Range(-maxInterp, maxInterp); // Randomize lerp ratio
                Vector3 p = Vector3.Lerp(start, end, t); // Get point at t between start and end

                // Y
                t = Random.Range(0.01f, 0.99f);
                
                if (BlitzRigTools.RandomBool())
                    result[i] = new Vector3(p.x, p.y , p.z) + Vector3.Lerp(Vector3.zero, widthVariationVector, t);
                else
                    result[i] = new Vector3(p.x, p.y , p.z) + Vector3.Lerp(Vector3.zero, widthVariationVector * -1, t);
            }
            
            result[segmentCount] = end;

            return result;
        }
    }
}