using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(TileGrid))]
public class GameObjectGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TileGrid yourScript = (TileGrid)target;

        if (GUILayout.Button("Generate Grid"))
        {
            yourScript.GenerateGameObjects();
        }
        if (GUILayout.Button("Delete Grid"))
        {
            yourScript.RemoveGameObjects();
        }
    }
}