using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

[CustomEditor(typeof(CreatureStats))]
[CanEditMultipleObjects]
public class CreatureStatsEditor : Editor
{
    FieldInfo[] statFields;
    string[] statFieldNames;
    FieldInfo flatBonusField;
    FieldInfo multiplierBonusField;

    bool showPrivateValues = false;

    void OnEnable()
    {
        var t = typeof(CreatureStats);

        // Cache all public Stat fields
        statFields = t.GetFields(BindingFlags.Public | BindingFlags.Instance)
                      .Where(f => typeof(Stat).IsAssignableFrom(f.FieldType))
                      .ToArray();
        statFieldNames = statFields.Select(f => f.Name).ToArray();

        // Cache private fields inside Stat
        var statType = typeof(Stat);
        flatBonusField = statType.GetField("_flatBonus", BindingFlags.NonPublic | BindingFlags.Instance);
        multiplierBonusField = statType.GetField("_multiplierBonus", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Show script reference (read-only)
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
        }

        EditorGUILayout.Space();
        showPrivateValues = EditorGUILayout.ToggleLeft("Show private values (debug)", showPrivateValues);
        EditorGUILayout.Space();

        // Draw all non-Stat properties
        DrawPropertiesExcluding(serializedObject, statFieldNames.Concat(new[] { "m_Script" }).ToArray());

        var targetObj = (CreatureStats)target;

        foreach (var f in statFields)
        {
            var header = f.GetCustomAttribute<HeaderAttribute>();
            if (header != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(header.header, EditorStyles.boldLabel);
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(f.Name), EditorStyles.boldLabel);

            // Editable BaseValue
            var statProp = serializedObject.FindProperty(f.Name);
            var baseProp = statProp?.FindPropertyRelative("BaseValue");
            if (baseProp != null) EditorGUILayout.PropertyField(baseProp, new GUIContent("Base Value"));

            // Read-only Value
            var statInstance = f.GetValue(targetObj) as Stat;
            if (statInstance != null)
            {
                EditorGUILayout.LabelField("Value", statInstance.Value.ToString("F2"));

                // Optional private fields (debug)
                if (showPrivateValues)
                {
                    float flat = (float)flatBonusField.GetValue(statInstance);
                    float mult = (float)multiplierBonusField.GetValue(statInstance);
                    EditorGUILayout.LabelField("Flat bonus (private)", flat.ToString("F2"));
                    EditorGUILayout.LabelField("Multiplier bonus (private)", mult.ToString("F4"));
                }
            }
            else
            {
                EditorGUILayout.HelpBox($"Stat '{f.Name}' is null.", MessageType.Warning);
            }

            EditorGUILayout.EndVertical();
        }

        serializedObject.ApplyModifiedProperties();

        if (Application.isPlaying) Repaint();
    }
}
