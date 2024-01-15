using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BiomeLevel))]
public class BiomeLevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Get the scriptable object
        BiomeLevel biomeLevel = (BiomeLevel)target;

        // Check if any value is changed
        if (GUI.changed)
        {
            EditorUtility.SetDirty(biomeLevel); // Mark the object as "dirty" or changed
            AssetDatabase.SaveAssets();     // Save the changes to the object
        }
    }
}

