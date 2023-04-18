using UnityEngine;

namespace BlitzRig
{
    public class BlitzRigMeshGenerator
    {
        private BlitzRigArcBranch branch;
        private Mesh mesh;

        public static Mesh ArcBranchToMesh(BlitzRigArcBranch ParamBranch)
        {
            Mesh result = new Mesh();
            int nodeCount = ParamBranch.GetNodeCount();
            
            int vertexCount = (nodeCount-1) * 4;
            int trianglePointCount = (nodeCount-1) * 6;
            
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[trianglePointCount];
            Color[] colors = new Color[vertexCount];

            int v_index = 0;
            int t_index = 0;
            
            for (int i = 0; i < nodeCount - 1; i++)
            {
                vertices[v_index + 0] = ParamBranch.GetNodeAt(i).P1;
                vertices[v_index + 1] = ParamBranch.GetNodeAt(i).P2;
                vertices[v_index + 2] = ParamBranch.GetNodeAt(i).P3;
                vertices[v_index + 3] = ParamBranch.GetNodeAt(i).P4;

                triangles[t_index + 0] = v_index + 0;
                triangles[t_index + 1] = v_index + 1;
                triangles[t_index + 2] = v_index + 2;

                triangles[t_index + 3] = v_index + 0;
                triangles[t_index + 4] = v_index + 2;
                triangles[t_index + 5] = v_index + 3;

                colors[v_index + 0] = ParamBranch.GetNodeAt(i).C1;
                colors[v_index + 1] = ParamBranch.GetNodeAt(i).C2;
                colors[v_index + 2] = ParamBranch.GetNodeAt(i).C3;
                colors[v_index + 3] = ParamBranch.GetNodeAt(i).C4;

                v_index += 4;
                t_index += 6;
            }

            result.vertices = vertices;
            result.triangles = triangles;
            result.colors = colors;

            return result;
        }

        public static Mesh ArcBranchToMesh3D(BlitzRigArcBranch ParamBranch)
        {
            Mesh result = new Mesh();
            int nodeCount = ParamBranch.GetNodeCount();
            
            int vertexCount = (nodeCount-1) * 8;
            int trianglePointCount = (nodeCount-1) * 24;
            
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[trianglePointCount];
            Color[] colors = new Color[vertexCount];

            int v_index = 0;
            int t_index = 0;
            
            for (int i = 0; i < nodeCount - 1; i++)
            {
                vertices[v_index + 0] = ParamBranch.GetNodeAt(i).P1;
                vertices[v_index + 1] = ParamBranch.GetNodeAt(i).P2;
                vertices[v_index + 2] = ParamBranch.GetNodeAt(i).P3;
                vertices[v_index + 3] = ParamBranch.GetNodeAt(i).P4;
                vertices[v_index + 4] = ParamBranch.GetNodeAt(i).P5;
                vertices[v_index + 5] = ParamBranch.GetNodeAt(i).P6;
                vertices[v_index + 6] = ParamBranch.GetNodeAt(i).P7;
                vertices[v_index + 7] = ParamBranch.GetNodeAt(i).P8;

                // Front 1
                triangles[t_index + 0]  = v_index + 0;
                triangles[t_index + 1]  = v_index + 1;
                triangles[t_index + 2]  = v_index + 2;

                // Front 2
                triangles[t_index + 3]  = v_index + 0;
                triangles[t_index + 4]  = v_index + 2;
                triangles[t_index + 5]  = v_index + 3;

                // Back 1
                triangles[t_index + 6]  = v_index + 6;
                triangles[t_index + 7]  = v_index + 5;
                triangles[t_index + 8]  = v_index + 4;

                // Back 2
                triangles[t_index + 9]  = v_index + 7;
                triangles[t_index + 10] = v_index + 6;
                triangles[t_index + 11] = v_index + 4;

                // Right 1
                triangles[t_index + 12] = v_index + 0;
                triangles[t_index + 13] = v_index + 4;
                triangles[t_index + 14] = v_index + 5;

                // Right 2
                triangles[t_index + 15] = v_index + 0;
                triangles[t_index + 16] = v_index + 5;
                triangles[t_index + 17] = v_index + 1;

                // Left 1
                triangles[t_index + 18] = v_index + 6;
                triangles[t_index + 19] = v_index + 7;
                triangles[t_index + 20] = v_index + 3;

                // Left 2
                triangles[t_index + 21] = v_index + 2;
                triangles[t_index + 22] = v_index + 6;
                triangles[t_index + 23] = v_index + 3;

                colors[v_index + 0] = ParamBranch.GetNodeAt(i).C1;
                colors[v_index + 1] = ParamBranch.GetNodeAt(i).C2;
                colors[v_index + 2] = ParamBranch.GetNodeAt(i).C3;
                colors[v_index + 3] = ParamBranch.GetNodeAt(i).C4;
                colors[v_index + 4] = ParamBranch.GetNodeAt(i).C1;
                colors[v_index + 5] = ParamBranch.GetNodeAt(i).C2;
                colors[v_index + 6] = ParamBranch.GetNodeAt(i).C3;
                colors[v_index + 7] = ParamBranch.GetNodeAt(i).C4;

                v_index += 8;
                t_index += 24;
            }

            result.vertices = vertices;
            result.triangles = triangles;
            result.colors = colors;

            return result;
        }

        public static Mesh CombineMeshes(Mesh[] ParamMeshArray)
        {
            Mesh result = new Mesh();
            
            int vert_count = 0;
            int tri_count = 0;

            for (int i = 0; i < ParamMeshArray.Length; i++)
            {
                vert_count += ParamMeshArray[i].vertices.Length;
                tri_count += ParamMeshArray[i].triangles.Length;
            }

            Vector3[] vertices = new Vector3[vert_count];
            int[] triangles = new int[tri_count];
            Color[] colors = new Color[vert_count];

            int v_index = 0;
            int t_index = 0;

            for (int i = 0; i < ParamMeshArray.Length; i++)
            {
                for (int j = 0; j < ParamMeshArray[i].vertices.Length; j++)
                {
                    vertices[v_index + j] = ParamMeshArray[i].vertices[j];
                }

                for (int j = 0; j < ParamMeshArray[i].triangles.Length; j++)
                {
                    triangles[t_index + j] = ParamMeshArray[i].triangles[j] + v_index;
                }

                for (int j = 0; j < ParamMeshArray[i].vertices.Length; j++)
                {
                    colors[v_index + j] = ParamMeshArray[i].colors[j];
                }

                v_index += ParamMeshArray[i].vertices.Length;
                t_index += ParamMeshArray[i].triangles.Length;
            }

            result.vertices = vertices;
            result.triangles = triangles;
            result.colors = colors;

            return result;
        }
    }
}