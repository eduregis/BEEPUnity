#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PhaseData))]
public class PhaseDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PhaseData phaseData = (PhaseData)target;

        GUILayout.Space(10);
        GUILayout.Label("Map Preview", EditorStyles.boldLabel);

        int[,] matrix = phaseData.GetMapMatrix();
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        // Alteração principal: Removemos o y-- e iteramos de 0 para rows-1
        for (int y = 0; y < rows; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < cols; x++)
            {
                matrix[y, x] = EditorGUILayout.IntField(matrix[y, x], GUILayout.Width(30));
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUI.changed)
        {
            // Atualiza a lista mapData com os valores editados
            phaseData.mapData.Clear();
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    phaseData.mapData.Add(matrix[y, x]);
                }
            }

            EditorUtility.SetDirty(phaseData);
        }
    }
}
#endif