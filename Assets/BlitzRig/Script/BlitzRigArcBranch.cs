using UnityEngine;

namespace BlitzRig
{
    public class BlitzRigArcBranch
    {
        public int maxDepth = 3;
        public Vector3 start = Vector3.zero;
        public Vector3 end = Vector3.zero;
        public uint segmentCount = 32;
        public float widthVariation = 2f;
        public float branchingChance = 0.2f;
        public float branchingDirection = 0.5f;
        public float branchingDirectionVariation = 0.1f;
        public float branchingDistanceFactor = 0.4f;
        public float branchingWidthFactor = 0.5f;
        public float defaultNodeWidth = 0.2f;
        public BlitzRigRandomizerRule randomizerRule = BlitzRigRandomizerRule.PseudoRandom;
        public bool adaptiveBranchSegmentCount = true;
        public bool alternateBranchingDirection = true;
        public uint minimumBranchingStepNodes = 2;
        public uint maximumBranchingStepNodes = 4;
        public float segmentSizeVariationFactor = 0.5f;
        public BlitzRigColoringMode coloringMode = BlitzRigColoringMode.UniColor;
        public Color mainColor = new Color(0.1f,0.1f,0.9f,1f);
        public Gradient colorGradient;
        public bool gradientBranches = true;
        public float baseGradientProgress = 0f;
        public bool enable3D = false;
        public float thickness3D = 0.2f;
        
        private BlitzRigArcNode[] nodes;
        private float distance;
        
        public int depth = 1;

        public Vector3 center;
        public Vector3 tangent;
        public Vector3 normal;

        private bool branchingDirectionToggle = false;

        public int GetNodeCount()
        {
            return nodes.Length;
        }

        public BlitzRigArcNode GetNodeAt(int ParamNodeIndex)
        {
            return nodes[ParamNodeIndex];
        }

        /// <summary>
        /// Update Center, Tangent and Normal.
        /// </summary>
        private void UpdateStuff()
        {
            distance = Vector3.Distance(start, end);
            center = (start + end)/2;
            tangent = end - start;
            normal = (new Vector3(tangent.y, tangent.x * -1, 0)).normalized;
        }
        
        public void GenerateNodes(bool ParamGenerateSubBranches = true)
        {
            UpdateStuff();
            
            // Generate node locations
            BlitzRigArcBranchGenerator branchGen = new BlitzRigArcBranchGenerator(start, end, segmentCount, segmentSizeVariationFactor, widthVariation);
            Vector3[] branchPoints = branchGen.GenerateArcBranchPoints();

            nodes = new BlitzRigArcNode[branchPoints.Length];
            float percentagePerNode = (1f - baseGradientProgress) / (float)segmentCount;

            // Node assignment
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i] = new BlitzRigArcNode(branchPoints[i], this, defaultNodeWidth, enable3D);
                nodes[i].Percentage = baseGradientProgress + (i * percentagePerNode);
                
                // Assign node color
                if (coloringMode == BlitzRigColoringMode.Gradient)
                    nodes[i].NodeColor = colorGradient.Evaluate(nodes[i].Percentage);
                else if (coloringMode == BlitzRigColoringMode.UniColor)
                    nodes[i].NodeColor = mainColor;
                else if (coloringMode == BlitzRigColoringMode.None)
                    nodes[i].NodeColor = Color.white;
            }

            // Mesh data preparation
            for (int i = 0; i < nodes.Length - 1; i++)
            {
                if (i > 0)
                    nodes[i].UpdateMeshPoints(nodes[i-1], nodes[i+1], enable3D);
                else
                    nodes[i].UpdateMeshPoints(null, nodes[i+1], enable3D);
            }

            // Branching Test
            if (depth >= maxDepth)
                return;

            if (randomizerRule == BlitzRigRandomizerRule.Random)
                PerformBranching_Random();
            else if (randomizerRule == BlitzRigRandomizerRule.PseudoRandom)
                PerformBranching_PseudoRandom();
        }

        private void PerformBranching_Random()
        {
            // Preparing Normals
            Vector3 normalPlus = normal * distance * branchingDistanceFactor;
            Vector3 normalMinus = normal * distance * branchingDistanceFactor * -1;

            // Branching
            for (int i = 0; i < nodes.Length; i++)
            {
                float rand = Random.Range(0f, 1f);

                if (rand <= branchingChance)
                {
                    CreateBranch(normalPlus, normalMinus, i);
                }
            }
        }

        private void PerformBranching_PseudoRandom()
        {
            // Preparing Normals
            Vector3 normalPlus = normal * distance * branchingDistanceFactor;
            Vector3 normalMinus = normal * distance * branchingDistanceFactor * -1;

            uint min = minimumBranchingStepNodes;
            uint max = maximumBranchingStepNodes;

            if (min < max)
            {
                uint tmp = min;
                min = max;
                max = tmp;
            }

            float rngPercentagePerStep =  1f / ((max + 1) - min);
            int c = 0;
            // Debug.Log("PPS = " + percentagePerStep);

            // Branching
            for (int i = 0; i < nodes.Length; i++)
            {
                bool resetCounter = false;
                
                // Check if branching is possible
                if (c == max)
                {
                    CreateBranch(normalPlus, normalMinus, i);
                    resetCounter = true;
                }
                else if (c >= min && c < max)
                {
                    float rand_ceil = (c - min + 1) * rngPercentagePerStep;
                    
                    if (Random.Range(0f, 1f) <= rand_ceil)
                    {
                        CreateBranch(normalPlus, normalMinus, i);
                        resetCounter = true;
                    }
                }

                if (resetCounter)
                    c = 0;
                else
                    c++;
            }
        }

        private void CreateBranch(Vector3 normalPlus, Vector3 normalMinus, int i)
        {
            // Create branch
            nodes[i].SubBranch = new BlitzRigArcBranch();
            nodes[i].SubBranch.start = nodes[i].Position;

            // Branching direction
            float t = branchingDirection + (Random.Range(0f, branchingDirectionVariation) * BlitzRigTools.RandomNegator());

            if (GetBranchingDirection())
                nodes[i].SubBranch.end = nodes[i].Position + Vector3.Lerp(tangent * branchingDistanceFactor, normalPlus, t);
            else
                nodes[i].SubBranch.end = nodes[i].Position + Vector3.Lerp(tangent * branchingDistanceFactor, normalMinus, t);

            // Segment count
            nodes[i].SubBranch.adaptiveBranchSegmentCount = nodes[i].ParentBranch.adaptiveBranchSegmentCount;

            if (nodes[i].ParentBranch.adaptiveBranchSegmentCount)
                nodes[i].SubBranch.segmentCount = (uint)Mathf.CeilToInt(nodes[i].ParentBranch.segmentCount * branchingDistanceFactor);
            else
                nodes[i].SubBranch.segmentCount = nodes[i].ParentBranch.segmentCount;

            // Other parameters
            nodes[i].SubBranch.widthVariation = nodes[i].ParentBranch.widthVariation * branchingWidthFactor;
            nodes[i].SubBranch.depth = nodes[i].ParentBranch.depth + 1;
            nodes[i].SubBranch.defaultNodeWidth = nodes[i].ParentBranch.defaultNodeWidth * branchingWidthFactor;
            nodes[i].SubBranch.randomizerRule = nodes[i].ParentBranch.randomizerRule;
            nodes[i].SubBranch.alternateBranchingDirection = nodes[i].ParentBranch.alternateBranchingDirection;
            nodes[i].SubBranch.minimumBranchingStepNodes = nodes[i].ParentBranch.minimumBranchingStepNodes;
            nodes[i].SubBranch.maximumBranchingStepNodes = nodes[i].ParentBranch.maximumBranchingStepNodes;
            
            if (nodes[i].ParentBranch.gradientBranches)
            {
                nodes[i].SubBranch.colorGradient = nodes[i].ParentBranch.colorGradient;
                nodes[i].SubBranch.coloringMode = nodes[i].ParentBranch.coloringMode;
                nodes[i].SubBranch.mainColor = nodes[i].NodeColor;
                nodes[i].SubBranch.baseGradientProgress = nodes[i].Percentage;
            }
            else
            {
                nodes[i].SubBranch.coloringMode = BlitzRigColoringMode.UniColor;
                nodes[i].SubBranch.mainColor = nodes[i].NodeColor;
            }

            nodes[i].SubBranch.GenerateNodes();
        }

        private bool GetBranchingDirection()
        {
            if (alternateBranchingDirection)
            {
                branchingDirectionToggle = !branchingDirectionToggle;
                return branchingDirectionToggle;
            }
            
            return BlitzRigTools.RandomBool();
        }
    }
}