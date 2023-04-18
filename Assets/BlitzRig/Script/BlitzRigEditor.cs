using UnityEngine;
using UnityEditor;

namespace BlitzRig
{
    #if UNITY_EDITOR

    public class BlitzRigEditor : Editor
    {
        private GUIStyle editorStyle;

        public override void OnInspectorGUI()
        {
            // Generate BG style
            if (editorStyle == null)
            {
                Texture2D tex = new Texture2D(1, 1);
                tex.SetPixel(0,0,Color.gray);
                tex.Apply();

                editorStyle = new GUIStyle(GUI.skin.box);
                editorStyle.normal.background = tex;
            }

            EditorGUILayout.BeginVertical(editorStyle);
            DrawDefaultInspector();
            EditorGUILayout.EndVertical();
        }
    }

    #endif
}