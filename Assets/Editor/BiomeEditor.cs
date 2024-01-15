using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Biome))]
public class BiomeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();
        
        // Get the scriptable object
        Biome biome = (Biome)target;

        // Check if any value is changed
        if (GUI.changed)
        {
            EditorUtility.SetDirty(biome); // Mark the object as "dirty" or changed
            AssetDatabase.SaveAssets();    // Save the changes to the object
        }
    }
}
