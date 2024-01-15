using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FeaturePack))]
public class FeatureEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();
        
        // Get the scriptable object
        FeaturePack feature = (FeaturePack)target;

        // Check if any value is changed
        if (GUI.changed)
        {
            EditorUtility.SetDirty(feature); // Mark the object as "dirty" or changed
            AssetDatabase.SaveAssets();     // Save the changes to the object
        }
    }
}
