using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Island))]
public class IslandEditor : Editor 
{
    public override void OnInspectorGUI() 
    {
        Island island = (Island)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Print HeightMap distribution")) 
        {
            island.PrintHeightMapDistribution();
        }
    }
}
