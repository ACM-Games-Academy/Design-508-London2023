using System.Collections.Generic;
using UnityEngine;

namespace BlitzRig
{
    public class BlitzRigCore : MonoBehaviour
    {
        const int MAX_SEGMENT_COUNT = 4096;
        
        public bool ShowHelperLines = true;
        public float AutoUpdateDuration = 0.2f;
        public BlitzRigRandomizerRule RandomizerRule = BlitzRigRandomizerRule.PseudoRandom;

        [Header("Arc settings")]
        public Transform StartPoint;
        public Transform EndPoint;
        
        [Range(0f, 2f)]
        public float ArcWidth = 0.2f;
        public float WidthVariation = 2;
        
        [Header("Segmentation settings")]
        public uint SegmentCount = 32;
        public bool AdaptiveBranchSegmentCount = true;
        
        [Range(0f, 1f)]
        public float SegmentSizeVariationFactor = 0.5f;
        
        [Header("Branching settings")]
        public int MaxDepth = 3;

        [Range(0f, 1f)]
        public float BranchingChance = 0.2f;

        [Range(0f, 1f)]
        public float BranchingDirection = 0.5f;

        [Range(0f, 0.5f)]
        public float BranchingDirectionVariation = 0.1f;

        [Range(0f, 1f)]
        public float BranchingDistanceFactor = 0.4f;

        [Range(0f, 1f)]
        public float BranchingWidthFactor = 0.5f;

        [Header("Pseudo-Random Parameters")]
        public uint MinimumBranchingStepNodes = 2;
        public uint MaximumBranchingStepNodes = 4;
        public bool AlternateBranchingDirection = true;
        
        [Header("Coloring")]
        public BlitzRigColoringMode ColoringMode = BlitzRigColoringMode.Gradient;
        public Color mainColor = new Color(0.1f,0.1f,0.9f,1f);
        public Gradient colorGradient;
        public bool gradientBranches = true;
        
        [Header("3D")]
        public bool Enable3D = false;
        public float Thickness3D = 0.2f;
        
        // ------------------------------------------------------

        private BlitzRigArcBranch mainBranch;

        Color color = new Color(1, 1, 1, 1);
        Color color_normal = new Color(0.2f, 0.2f, 1f, 1);
        Color color_tangent = new Color(1f, 0.2f, 0.2f, 1);
        Color color_opposite = new Color(0.2f, 1f, 0.2f, 1);

        private float lastUpdateTime = -1f;

        void Start()
        {
            UpdateArcPoints();
        }

        private void UpdateArcPoints()
        {
            if (Time.time < lastUpdateTime + AutoUpdateDuration)
                return;

            if (SegmentCount < 1)
                SegmentCount = 1;
            else if (SegmentCount > MAX_SEGMENT_COUNT)
                SegmentCount = MAX_SEGMENT_COUNT;
            
            mainBranch = new BlitzRigArcBranch();
            mainBranch.start = StartPoint.position;
            mainBranch.end = EndPoint.position;
            mainBranch.segmentCount = SegmentCount;
            mainBranch.widthVariation = WidthVariation;
            mainBranch.branchingChance = BranchingChance;
            mainBranch.maxDepth = MaxDepth;
            mainBranch.branchingDirection = BranchingDirection;
            mainBranch.branchingDirectionVariation = BranchingDirectionVariation;
            mainBranch.branchingDistanceFactor = BranchingDistanceFactor;
            mainBranch.branchingWidthFactor = BranchingWidthFactor;
            mainBranch.defaultNodeWidth = ArcWidth;
            mainBranch.randomizerRule = RandomizerRule;
            mainBranch.minimumBranchingStepNodes = MinimumBranchingStepNodes;
            mainBranch.maximumBranchingStepNodes = MaximumBranchingStepNodes;
            mainBranch.adaptiveBranchSegmentCount = AdaptiveBranchSegmentCount;
            mainBranch.alternateBranchingDirection = AlternateBranchingDirection;
            mainBranch.segmentSizeVariationFactor = SegmentSizeVariationFactor;
            mainBranch.coloringMode = ColoringMode;
            mainBranch.mainColor = mainColor;
            mainBranch.colorGradient = colorGradient;
            mainBranch.gradientBranches = gradientBranches;
            mainBranch.enable3D = Enable3D;
            mainBranch.thickness3D = Thickness3D;
            mainBranch.GenerateNodes();

            // Draw all branches as one mesh
            List<BlitzRigArcBranch> allBranches = GetBranchesRecursively(mainBranch);
            Mesh[] allMeshes = new Mesh[allBranches.Count];

            // print("Branch count : " + allBranches.Count);

            int c = 0;
            foreach (BlitzRigArcBranch branch in allBranches)
            {
                if (Enable3D)
                    allMeshes[c] = BlitzRigMeshGenerator.ArcBranchToMesh3D(branch);
                else
                    allMeshes[c] = BlitzRigMeshGenerator.ArcBranchToMesh(branch);
                
                c++;
            }

            Mesh finalMesh = BlitzRigMeshGenerator.CombineMeshes(allMeshes);

            GetComponent<MeshFilter>().mesh = finalMesh;

            // ---------------------------------------------------------------------------

            lastUpdateTime = Time.time;
        }

        void Update()
        {
            UpdateArcPoints();

            if (mainBranch == null)
                return;

            // ----------------------------------------------------------------------------

            #if UNITY_EDITOR
            
                List<BlitzRigArcBranch> allBranches = GetBranchesRecursively(mainBranch);

                // Draw all branches in editor
                foreach (BlitzRigArcBranch branch in allBranches)
                {
                    DrawDebugBranch(branch);
                }

            #endif
        }

        private List<BlitzRigArcBranch> GetBranchesRecursively(BlitzRigArcBranch ParamBranch, List<BlitzRigArcBranch> ParamBranchResultList = null)
        {
            if (ParamBranchResultList == null)
                ParamBranchResultList = new List<BlitzRigArcBranch>();

            ParamBranchResultList.Add(ParamBranch);

            for (int i = 0; i < ParamBranch.GetNodeCount(); i++)
            {
                if (ParamBranch.GetNodeAt(i).SubBranch != null)
                {
                    GetBranchesRecursively(ParamBranch.GetNodeAt(i).SubBranch, ParamBranchResultList);
                }
            }

            return ParamBranchResultList;
        }

        void DrawDebugBranch(BlitzRigArcBranch ParamBranch)
        {
            for (int i = 0; i < ParamBranch.GetNodeCount()-1; i++)
            {
                Debug.DrawLine(ParamBranch.GetNodeAt(i).Position, ParamBranch.GetNodeAt(i+1).Position, color);
            }
            
            if (ShowHelperLines)
            {
                Debug.DrawLine(ParamBranch.center, ParamBranch.normal + ParamBranch.center, color_normal);
                Debug.DrawLine(ParamBranch.center, ParamBranch.tangent + ParamBranch.center, color_tangent);
                Debug.DrawLine(ParamBranch.center, -ParamBranch.tangent + ParamBranch.center, color_opposite);
            }
        }

        void OnDrawGizmos()
        {
            if (mainBranch == null)
                return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(StartPoint.position, 0.2f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(EndPoint.position, 0.2f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(mainBranch.center, 0.2f);
        }
    }

    public enum BlitzRigRandomizerRule {Random, PseudoRandom}
    public enum BlitzRigColoringMode {UniColor, Gradient, None}
}