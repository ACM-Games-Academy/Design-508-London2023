using UnityEngine;

namespace BlitzRig
{
    public class BlitzRigArcNode
    {
        public Vector3 Position;
        public BlitzRigArcBranch ParentBranch;
        public BlitzRigArcBranch SubBranch;
        public float Width = 0.05f;
        
        public Vector3 P1;
        public Vector3 P2;
        public Vector3 P3;
        public Vector3 P4;

        // 3D
        public Vector3 P5;
        public Vector3 P6;
        public Vector3 P7;
        public Vector3 P8;
        
        public float Percentage;
        public Color NodeColor;
        public Color C1;
        public Color C2;
        public Color C3;
        public Color C4;

        public BlitzRigArcNode(Vector3 ParamPosition, BlitzRigArcBranch ParamParent, float ParamWidth, bool Param3D)
        {
            Position = ParamPosition;
            ParentBranch = ParamParent;
            Width = ParamWidth;
        }

        /// <summary>
        /// Update P1, P2, P3 and P4, the 4 positions of the vertices that form the node quad.
        /// Update C1, C2, C3 and C4, the 4 vertex colors.
        /// </summary>
        public void UpdateMeshPoints(BlitzRigArcNode ParamPreviousNode, BlitzRigArcNode ParamNextNode, bool Param3D) // TODO : Branching support
        {
            if (ParamNextNode == null)
                return;
            
            Vector3 tangent = ParamNextNode.Position - Position;
            Vector3 normal = (new Vector3(tangent.y, tangent.x * -1, 0)).normalized;

            float t = Mathf.Abs(ParentBranch.thickness3D) * -1;
            Vector3 thickness3D = new Vector3(0f,0f,t);

            // --------------------------------------------------------------------------------------------- 2D
            // Clock Wise
            if (ParamPreviousNode == null)
            {
                P1 = Position + normal * Width;
                C1 = NodeColor;
            }
            else
            {
                P1 = ParamPreviousNode.P2;
                C1 = ParamPreviousNode.C2;
            }

            P2 = ParamNextNode.Position + normal * Width;
            C2 = ParamNextNode.NodeColor;

            P3 = ParamNextNode.Position + normal * Width * -1;
            C3 = ParamNextNode.NodeColor;
            
            if (ParamPreviousNode == null)
            {
                P4 = Position + normal * Width * -1;
                C4 = NodeColor;
            }
            else
            {
                P4 = ParamPreviousNode.P3;
                C4 = ParamPreviousNode.C3;
            }
            // --------------------------------------------------------------------------------------------- 3D
            // Clock Wise
            P5 = P1 + thickness3D;
            P6 = P2 + thickness3D;
            P7 = P3 + thickness3D;
            P8 = P4 + thickness3D;
            // --------------------------------------------------------------------------------------------- END
        }
    }
}