// using UnityEditor;
// using UnityEngine;

// [CustomEditor(typeof(MapPreview))]
// public class MapPreviewEditor : Editor 
// {
//     public override void OnInspectorGUI() 
//     {
//         MapPreview mapPreview = (MapPreview)target;

//         if (DrawDefaultInspector()) 
//         {
//             if (mapPreview.AutoUpdate) 
//             {
//                 mapPreview.UpdateMapPreview();
//             }
//         }

//         if (GUILayout.Button("Generate")) 
//         {
//             mapPreview.UpdateMapPreview();
//         }
//     }
// }
