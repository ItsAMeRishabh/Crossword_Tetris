using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileGrid))]
 public class GameObjectGeneratorEditor : Editor
 {
     public override void OnInspectorGUI()
     {
         base.OnInspectorGUI();

         TileGrid yourScript = (TileGrid)target;

         if (GUILayout.Button("Delete and Generate New Grid"))
         {
             yourScript.GenerateGameObjects();
         }
         if (GUILayout.Button("Delete Grid"))
         {
             yourScript.RemoveGameObjects();
         }
     }
 }