using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Material)), CanEditMultipleObjects]
public class CustomMaterialEditor : MaterialEditor
{
    MaterialDefinitionGlobal[] _definitions;

    public override void OnEnable()
    {
        base.OnEnable();

        var children = AssetDatabase.FindAssets("t:MaterialDefinitionGlobal").Select(x => AssetDatabase.LoadAssetAtPath<MaterialDefinitionGlobal>(AssetDatabase.GUIDToAssetPath(x))).Where(x => x != null).ToArray();
        _definitions = new MaterialDefinitionGlobal[Enum.GetValues(typeof(MaterialKind)).Length];
        foreach (var child in children)
        {
            _definitions[(int)child.Kind] = child;
        }
    }
    public override void OnInspectorGUI()
    {
        var materials = targets.OfType<Material>().ToArray();

        var kinds = materials.Select(x => _definitions.FirstOrDefault(y => y != null && y.Materials.Contains(x))?.Kind ?? MaterialKind.Unknown).Distinct().ToArray();
        if (kinds.Length == 0) return;

        if (kinds.Length > 1)
        {
            EditorGUI.showMixedValue = true;
        }
        EditorGUI.BeginChangeCheck();
        var change = (MaterialKind)EditorGUILayout.EnumPopup("Material Kind", kinds[0]);
        if (EditorGUI.EndChangeCheck())
        {
            foreach (var material in materials)
            {
                var prev = _definitions.FirstOrDefault(y => y != null && y.Materials.Contains(material));
                if (prev && prev.Kind != change)
                {
                    var so = new SerializedObject(prev);
                    so.FindProperty("_materials").DeleteArrayElementAtIndex(Array.FindIndex(prev.Materials, x => x == material));
                    so.ApplyModifiedProperties();
                }
                if (!prev || prev.Kind != change)
                {
                    var next = _definitions[(int)change];
                    if (next != null)
                    {
                        var so = new SerializedObject(next);
                        so.FindProperty("_materials").InsertArrayElementAtIndex(so.FindProperty("_materials").arraySize);
                        so.FindProperty("_materials").GetArrayElementAtIndex(so.FindProperty("_materials").arraySize - 1).objectReferenceValue = material;
                        so.ApplyModifiedProperties();
                    }
                }
            }
        }
        EditorGUI.showMixedValue = false;

        base.OnInspectorGUI();
    }
}
